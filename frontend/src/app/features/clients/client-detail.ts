import { ChangeDetectionStrategy, Component, effect, inject, input, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { ClientsApi } from '../../core/api/clients-api';
import { ClientDetail as ClientDetailModel } from '../../core/models';
import { KycPanel } from './kyc-panel';

@Component({
  selector: 'lex-client-detail',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, RouterLink, MatCardModule, MatTabsModule, MatButtonModule,
    MatIconModule, MatProgressBarModule, KycPanel,
  ],
  templateUrl: './client-detail.html',
  styleUrl: './client-detail.scss',
})
export class ClientDetail {
  private readonly api = inject(ClientsApi);
  private readonly snackBar = inject(MatSnackBar);

  readonly id = input.required<string>();

  protected readonly client = signal<ClientDetailModel | null>(null);
  protected readonly loading = signal(false);

  constructor() {
    effect(() => this.load(this.id()));
  }

  private load(clientId: string): void {
    this.loading.set(true);
    this.api.getById(clientId).subscribe({
      next: (client) => {
        this.client.set(client);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Client introuvable (API indisponible ?).', 'Fermer', { duration: 4000 });
      },
    });
  }
}
