import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { CasesApi } from '../../core/api/cases-api';
import { CaseSummary } from '../../core/models';
import { StatusChipPipe } from '../shared/status-chip-pipe';

@Component({
  selector: 'lex-cases-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, ReactiveFormsModule, MatCardModule, MatTableModule, MatButtonModule,
    MatIconModule, MatFormFieldModule, MatInputModule, MatProgressBarModule, StatusChipPipe,
  ],
  templateUrl: './cases-page.html',
  styleUrl: './cases-page.scss',
})
export class CasesPage {
  private readonly api = inject(CasesApi);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly columns = ['title', 'status', 'openedOnUtc'];
  protected readonly cases = signal<CaseSummary[]>([]);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly showForm = signal(false);

  protected readonly form = this.fb.group({
    title: this.fb.nonNullable.control('', Validators.required),
    clientId: this.fb.nonNullable.control('', Validators.required),
    courtName: this.fb.control<string | null>(null),
    generalRegisterNumber: this.fb.control<string | null>(null),
  });

  constructor() {
    this.load();
  }

  protected toggleForm(): void {
    this.showForm.update((open) => !open);
  }

  protected open(row: CaseSummary): void {
    this.router.navigate(['/cases', row.id]);
  }

  protected load(): void {
    this.loading.set(true);
    this.api.list(1, 50).subscribe({
      next: (page) => {
        this.cases.set(page.items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Impossible de charger les dossiers (API indisponible ?).', 'Fermer', { duration: 4000 });
      },
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      return;
    }
    this.saving.set(true);
    this.api.create(this.form.getRawValue()).subscribe({
      next: ({ id }) => {
        this.saving.set(false);
        this.snackBar.open('Dossier ouvert.', 'Fermer', { duration: 3000 });
        this.router.navigate(['/cases', id]);
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open("Échec de l'ouverture (client introuvable ?).", 'Fermer', { duration: 4000 });
      },
    });
  }
}
