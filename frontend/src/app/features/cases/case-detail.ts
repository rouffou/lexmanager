import { ChangeDetectionStrategy, Component, effect, inject, input, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { CasesApi } from '../../core/api/cases-api';
import { BillingApi, DocumentsApi, TimeApi } from '../../core/api/case-extras-api';
import { BillingDocumentSummary, CaseDetail as CaseDetailModel, CaseTimeSummary, DocumentSummary } from '../../core/models';
import { StatusChipPipe } from '../shared/status-chip-pipe';

@Component({
  selector: 'lex-case-detail',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, DecimalPipe, RouterLink, MatCardModule, MatTabsModule, MatTableModule,
    MatButtonModule, MatIconModule, MatProgressBarModule, StatusChipPipe,
  ],
  templateUrl: './case-detail.html',
  styleUrl: './case-detail.scss',
})
export class CaseDetail {
  private readonly casesApi = inject(CasesApi);
  private readonly documentsApi = inject(DocumentsApi);
  private readonly timeApi = inject(TimeApi);
  private readonly billingApi = inject(BillingApi);
  private readonly snackBar = inject(MatSnackBar);

  readonly id = input.required<string>();

  protected readonly docColumns = ['fileName', 'category', 'currentVersion'];
  protected readonly billColumns = ['number', 'kind', 'status', 'total'];

  protected readonly detail = signal<CaseDetailModel | null>(null);
  protected readonly documents = signal<DocumentSummary[]>([]);
  protected readonly time = signal<CaseTimeSummary | null>(null);
  protected readonly billing = signal<BillingDocumentSummary[]>([]);
  protected readonly loading = signal(false);

  constructor() {
    effect(() => this.load(this.id()));
  }

  private load(caseId: string): void {
    this.loading.set(true);
    this.casesApi.getById(caseId).subscribe({
      next: (d) => {
        this.detail.set(d);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Dossier introuvable (API indisponible ?).', 'Fermer', { duration: 4000 });
      },
    });
    this.documentsApi.byCase(caseId).subscribe({ next: (p) => this.documents.set(p.items), error: () => {} });
    this.timeApi.summary(caseId).subscribe({ next: (t) => this.time.set(t), error: () => {} });
    this.billingApi.byCase(caseId).subscribe({ next: (p) => this.billing.set(p.items), error: () => {} });
  }

  protected close(): void {
    this.casesApi.close(this.id()).subscribe({
      next: () => {
        this.snackBar.open('Dossier clôturé.', 'Fermer', { duration: 3000 });
        this.load(this.id());
      },
      error: () => this.snackBar.open('Clôture impossible.', 'Fermer', { duration: 4000 }),
    });
  }
}
