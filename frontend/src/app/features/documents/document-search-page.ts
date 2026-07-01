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
import { DocumentsApi } from '../../core/api/case-extras-api';
import { DocumentSearchResult } from '../../core/models';

/// Full-text / OCR document search across the DMS (SRD V11 §7.2).
@Component({
  selector: 'lex-document-search-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, ReactiveFormsModule, MatCardModule, MatTableModule, MatButtonModule,
    MatIconModule, MatFormFieldModule, MatInputModule, MatProgressBarModule,
  ],
  templateUrl: './document-search-page.html',
  styleUrl: './document-search-page.scss',
})
export class DocumentSearchPage {
  private readonly api = inject(DocumentsApi);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly columns = ['fileName', 'category', 'highlight', 'createdOnUtc'];
  protected readonly results = signal<DocumentSearchResult[]>([]);
  protected readonly loading = signal(false);
  protected readonly searched = signal(false);

  protected readonly form = this.fb.group({
    term: this.fb.nonNullable.control('', [Validators.required, Validators.minLength(2)]),
  });

  protected submit(): void {
    if (this.form.invalid) {
      return;
    }

    const term = this.form.getRawValue().term.trim();
    this.loading.set(true);
    this.api.search(term).subscribe({
      next: (page) => {
        this.results.set(page.items);
        this.searched.set(true);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Recherche indisponible (API injoignable ?).', 'Fermer', { duration: 4000 });
      },
    });
  }

  protected openCase(result: DocumentSearchResult): void {
    this.router.navigate(['/cases', result.caseId]);
  }
}
