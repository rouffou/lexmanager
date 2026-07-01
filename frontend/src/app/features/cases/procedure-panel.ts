import { ChangeDetectionStrategy, Component, effect, inject, input, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { ProcedureApi } from '../../core/api/procedure-api';
import { ProcedurePlan, ProcedureType } from '../../core/models';

/// Interactive procedure tree for a case — de la mise en demeure à l'exécution forcée (SRD V11 §36).
@Component({
  selector: 'lex-procedure-panel',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, ReactiveFormsModule, MatButtonModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatIconModule, MatProgressBarModule,
  ],
  templateUrl: './procedure-panel.html',
  styleUrl: './procedure-panel.scss',
})
export class ProcedurePanel {
  private readonly api = inject(ProcedureApi);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  readonly caseId = input.required<string>();

  protected readonly plan = signal<ProcedurePlan | null>(null);
  protected readonly loading = signal(false);
  protected readonly notFound = signal(false);
  protected readonly saving = signal(false);

  protected readonly procedureTypes: readonly { value: ProcedureType; label: string }[] = [
    { value: 'DebtRecovery', label: 'Recouvrement de créance' },
    { value: 'CivilLitigation', label: 'Procédure civile au fond' },
    { value: 'SummaryProceedings', label: 'Référé' },
    { value: 'LabourDispute', label: 'Litige social' },
    { value: 'Appeal', label: 'Appel' },
  ];

  protected readonly form = this.fb.group({
    procedureType: this.fb.nonNullable.control<ProcedureType>('DebtRecovery'),
    referenceOn: this.fb.nonNullable.control(new Date().toISOString().slice(0, 10), Validators.required),
  });

  constructor() {
    effect(() => this.load(this.caseId()));
  }

  private load(caseId: string): void {
    this.loading.set(true);
    this.notFound.set(false);
    this.api.byCase(caseId).subscribe({
      next: (plan) => {
        this.plan.set(plan);
        this.loading.set(false);
      },
      error: () => {
        this.plan.set(null);
        this.notFound.set(true);
        this.loading.set(false);
      },
    });
  }

  protected generate(): void {
    if (this.form.invalid) {
      return;
    }
    const { procedureType, referenceOn } = this.form.getRawValue();
    this.saving.set(true);
    this.api.generate(this.caseId(), procedureType, new Date(referenceOn).toISOString()).subscribe({
      next: () => {
        this.saving.set(false);
        this.snackBar.open('Arbre de procédure généré.', 'Fermer', { duration: 3000 });
        this.load(this.caseId());
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Génération impossible (un arbre existe déjà ?).', 'Fermer', { duration: 4000 });
      },
    });
  }

  protected advance(): void {
    this.mutate((planId) => this.api.advance(planId), 'Étape franchie.');
  }

  protected skip(): void {
    this.mutate((planId) => this.api.skip(planId), 'Étape ignorée.');
  }

  protected schedule(order: number, value: string): void {
    const plan = this.plan();
    if (!plan || !value) {
      return;
    }
    this.api.schedule(plan.id, order, new Date(value).toISOString()).subscribe({
      next: () => {
        this.snackBar.open('Date planifiée.', 'Fermer', { duration: 2000 });
        this.load(this.caseId());
      },
      error: () => this.snackBar.open('Planification impossible.', 'Fermer', { duration: 4000 }),
    });
  }

  protected typeLabel(type: ProcedureType): string {
    return this.procedureTypes.find((entry) => entry.value === type)?.label ?? type;
  }

  private mutate(action: (planId: string) => Observable<void>, success: string): void {
    const plan = this.plan();
    if (!plan) {
      return;
    }
    this.saving.set(true);
    action(plan.id).subscribe({
      next: () => {
        this.saving.set(false);
        this.snackBar.open(success, 'Fermer', { duration: 2000 });
        this.load(this.caseId());
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Action impossible.', 'Fermer', { duration: 4000 });
      },
    });
  }
}
