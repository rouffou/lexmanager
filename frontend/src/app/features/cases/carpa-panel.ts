import { ChangeDetectionStrategy, Component, effect, inject, input, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { CarpaApi } from '../../core/api/carpa-api';
import { CarpaAccount } from '../../core/models';

/// Third-party (CARPA / rubriqué) account panel for a case (SRD V11 §5).
@Component({
  selector: 'lex-carpa-panel',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, DecimalPipe, ReactiveFormsModule, MatButtonModule, MatButtonToggleModule,
    MatFormFieldModule, MatInputModule, MatIconModule, MatProgressBarModule, MatTableModule,
  ],
  templateUrl: './carpa-panel.html',
  styleUrl: './carpa-panel.scss',
})
export class CarpaPanel {
  private readonly api = inject(CarpaApi);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  readonly caseId = input.required<string>();
  readonly clientId = input<string | null>(null);

  protected readonly columns = ['occurredOnUtc', 'type', 'amount', 'description', 'counterparty'];
  protected readonly account = signal<CarpaAccount | null>(null);
  protected readonly loading = signal(false);
  protected readonly notFound = signal(false);
  protected readonly saving = signal(false);
  protected readonly showForm = signal(false);

  protected readonly form = this.fb.group({
    type: this.fb.nonNullable.control<'deposit' | 'disbursement'>('deposit'),
    amount: this.fb.control<number | null>(null, [Validators.required, Validators.min(0.01)]),
    description: this.fb.nonNullable.control('', Validators.required),
    counterparty: this.fb.control<string | null>(null),
  });

  constructor() {
    effect(() => this.load(this.caseId()));
  }

  private load(caseId: string): void {
    this.loading.set(true);
    this.notFound.set(false);
    this.api.byCase(caseId).subscribe({
      next: (account) => {
        this.account.set(account);
        this.loading.set(false);
      },
      error: () => {
        this.account.set(null);
        this.notFound.set(true);
        this.loading.set(false);
      },
    });
  }

  protected toggleForm(): void {
    this.showForm.update((open) => !open);
  }

  protected openAccount(): void {
    const clientId = this.clientId();
    if (!clientId) {
      this.snackBar.open('Client du dossier inconnu : impossible d’ouvrir le compte.', 'Fermer', { duration: 4000 });
      return;
    }
    this.saving.set(true);
    this.api.open(this.caseId(), clientId).subscribe({
      next: () => {
        this.saving.set(false);
        this.snackBar.open('Compte de tiers ouvert.', 'Fermer', { duration: 3000 });
        this.load(this.caseId());
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Ouverture impossible (compte déjà existant ?).', 'Fermer', { duration: 4000 });
      },
    });
  }

  protected submit(): void {
    const account = this.account();
    if (this.form.invalid || account === null) {
      return;
    }

    const { type, amount, description, counterparty } = this.form.getRawValue();
    this.saving.set(true);

    const movement = type === 'deposit'
      ? this.api.deposit(account.id, amount!, description, counterparty ?? null)
      : this.api.disburse(account.id, amount!, description, counterparty ?? null);

    movement.subscribe({
      next: () => {
        this.saving.set(false);
        this.snackBar.open(type === 'deposit' ? 'Dépôt enregistré.' : 'Décaissement enregistré.', 'Fermer', { duration: 3000 });
        this.form.reset({ type, amount: null, description: '', counterparty: null });
        this.showForm.set(false);
        this.load(this.caseId());
      },
      error: (err: { status?: number }) => {
        this.saving.set(false);
        const message = err?.status === 409
          ? 'Solde insuffisant : les fonds de tiers ne peuvent être à découvert.'
          : 'Mouvement refusé.';
        this.snackBar.open(message, 'Fermer', { duration: 5000 });
      },
    });
  }
}
