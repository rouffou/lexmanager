import { ChangeDetectionStrategy, Component, effect, inject, input, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { KycApi } from '../../core/api/kyc-api';
import { DueDiligence, RiskLevel, VerificationKind } from '../../core/models';
import { StatusChipPipe } from '../shared/status-chip-pipe';

/// Anti-money-laundering due-diligence (KYC / LCB-FT) panel for a client (SRD V11 §30).
@Component({
  selector: 'lex-kyc-panel',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, ReactiveFormsModule, MatButtonModule, MatCheckboxModule, MatFormFieldModule,
    MatInputModule, MatIconModule, MatProgressBarModule, MatSelectModule, StatusChipPipe,
  ],
  templateUrl: './kyc-panel.html',
  styleUrl: './kyc-panel.scss',
})
export class KycPanel {
  private readonly api = inject(KycApi);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  readonly clientId = input.required<string>();

  protected readonly file = signal<DueDiligence | null>(null);
  protected readonly loading = signal(false);
  protected readonly notFound = signal(false);
  protected readonly saving = signal(false);

  protected readonly allKinds: { value: VerificationKind; label: string }[] = [
    { value: 'IdentityDocument', label: "Pièce d'identité (CNIE/passeport)" },
    { value: 'AddressProof', label: 'Justificatif de domicile' },
    { value: 'CompanyRegistry', label: 'Extrait registre (BCE/Kbis)' },
    { value: 'BeneficialOwner', label: 'Bénéficiaire effectif (UBO)' },
    { value: 'PepScreening', label: 'Personne politiquement exposée (PEP)' },
    { value: 'SanctionsScreening', label: 'Criblage sanctions' },
  ];

  protected readonly startForm = this.fb.group({
    riskLevel: this.fb.nonNullable.control<RiskLevel>('Standard'),
    isPoliticallyExposed: this.fb.nonNullable.control(false),
  });

  protected readonly checkForm = this.fb.group({
    kind: this.fb.nonNullable.control<VerificationKind>('IdentityDocument'),
    reference: this.fb.nonNullable.control('', Validators.required),
    cleared: this.fb.nonNullable.control(true),
    notes: this.fb.control<string | null>(null),
  });

  protected readonly rejectReason = this.fb.nonNullable.control('');

  constructor() {
    effect(() => this.load(this.clientId()));
  }

  protected kindLabel(kind: VerificationKind): string {
    return this.allKinds.find((entry) => entry.value === kind)?.label ?? kind;
  }

  protected isCleared(kind: VerificationKind): boolean {
    return this.file()?.checks.some((check) => check.kind === kind && check.cleared) ?? false;
  }

  private load(clientId: string): void {
    this.loading.set(true);
    this.notFound.set(false);
    this.api.byClient(clientId).subscribe({
      next: (file) => {
        this.file.set(file);
        this.loading.set(false);
      },
      error: () => {
        this.file.set(null);
        this.notFound.set(true);
        this.loading.set(false);
      },
    });
  }

  protected start(): void {
    const { riskLevel, isPoliticallyExposed } = this.startForm.getRawValue();
    this.saving.set(true);
    this.api.start(this.clientId(), riskLevel, isPoliticallyExposed).subscribe({
      next: () => {
        this.saving.set(false);
        this.snackBar.open('Dossier de vigilance ouvert.', 'Fermer', { duration: 3000 });
        this.load(this.clientId());
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Ouverture impossible (dossier déjà existant ?).', 'Fermer', { duration: 4000 });
      },
    });
  }

  protected recordCheck(): void {
    const file = this.file();
    if (this.checkForm.invalid || file === null) {
      return;
    }
    const { kind, reference, cleared, notes } = this.checkForm.getRawValue();
    this.saving.set(true);
    this.api.recordCheck(file.id, kind, reference, cleared, notes ?? null).subscribe({
      next: () => {
        this.saving.set(false);
        this.checkForm.reset({ kind, reference: '', cleared: true, notes: null });
        this.load(this.clientId());
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Enregistrement refusé.', 'Fermer', { duration: 4000 });
      },
    });
  }

  protected approve(): void {
    const file = this.file();
    if (file === null) {
      return;
    }
    this.saving.set(true);
    this.api.decide(file.id, true, null).subscribe({
      next: () => {
        this.saving.set(false);
        this.snackBar.open('Mandat accepté : vigilance validée.', 'Fermer', { duration: 3000 });
        this.load(this.clientId());
      },
      error: (err: { status?: number }) => {
        this.saving.set(false);
        const message = err?.status === 409
          ? 'Vigilance incomplète : score de conformité inférieur à 100.'
          : 'Décision refusée.';
        this.snackBar.open(message, 'Fermer', { duration: 5000 });
      },
    });
  }

  protected reject(): void {
    const file = this.file();
    const reason = this.rejectReason.value.trim();
    if (file === null || reason.length === 0) {
      this.snackBar.open('Un motif est requis pour refuser le mandat.', 'Fermer', { duration: 4000 });
      return;
    }
    this.saving.set(true);
    this.api.decide(file.id, false, reason).subscribe({
      next: () => {
        this.saving.set(false);
        this.rejectReason.reset('');
        this.snackBar.open('Mandat refusé.', 'Fermer', { duration: 3000 });
        this.load(this.clientId());
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Décision refusée.', 'Fermer', { duration: 4000 });
      },
    });
  }
}
