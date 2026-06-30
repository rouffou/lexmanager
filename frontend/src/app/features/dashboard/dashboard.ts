import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

interface DashboardTile {
  readonly title: string;
  readonly description: string;
  readonly path: string;
  readonly icon: string;
  readonly color: string;
}

@Component({
  selector: 'lex-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, MatCardModule, MatIconModule, MatButtonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  protected readonly tiles: readonly DashboardTile[] = [
    {
      title: 'Clients',
      description: "Fiches clients, personnes physiques et morales, conflits d'intérêts.",
      path: '/clients',
      icon: 'groups',
      color: '#1E293B',
    },
    {
      title: 'Dossiers',
      description: 'Cycle de vie des affaires, parties adverses, juridictions.',
      path: '/cases',
      icon: 'gavel',
      color: '#B45309',
    },
  ];
}
