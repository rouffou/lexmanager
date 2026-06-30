import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { ClientsApi } from '../../core/api/clients-api';
import { ClientSummary } from '../../core/models';

@Component({
  selector: 'lex-clients-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, ReactiveFormsModule, MatCardModule, MatTableModule, MatButtonModule,
    MatIconModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatProgressBarModule,
  ],
  templateUrl: './clients-page.html',
  styleUrl: './clients-page.scss',
})
export class ClientsPage {
  private readonly api = inject(ClientsApi);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly columns = ['displayName', 'type', 'email', 'createdOnUtc'];
  protected readonly clients = signal<ClientSummary[]>([]);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly showForm = signal(false);

  protected readonly form = this.fb.group({
    type: this.fb.nonNullable.control<'PhysicalPerson' | 'LegalPerson'>('PhysicalPerson'),
    email: this.fb.nonNullable.control('', [Validators.required, Validators.email]),
    firstName: this.fb.control<string | null>(null),
    lastName: this.fb.control<string | null>(null),
    nationalIdentityNumber: this.fb.control<string | null>(null),
    companyName: this.fb.control<string | null>(null),
    registrationNumber: this.fb.control<string | null>(null),
    legalRepresentative: this.fb.control<string | null>(null),
  });

  constructor() {
    this.load();
  }

  protected toggleForm(): void {
    this.showForm.update((open) => !open);
  }

  protected load(): void {
    this.loading.set(true);
    this.api.list(1, 50).subscribe({
      next: (page) => {
        this.clients.set(page.items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Impossible de charger les clients (API indisponible ?).', 'Fermer', { duration: 4000 });
      },
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      return;
    }
    this.saving.set(true);
    this.api.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.saving.set(false);
        this.showForm.set(false);
        this.form.reset({ type: 'PhysicalPerson', email: '' });
        this.snackBar.open('Client créé.', 'Fermer', { duration: 3000 });
        this.load();
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open("Échec de la création (vérifiez les champs / conflit d'intérêts).", 'Fermer', { duration: 4000 });
      },
    });
  }
}
