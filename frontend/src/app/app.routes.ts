import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    title: 'Tableau de bord — LexManager',
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
  },
  {
    path: 'clients',
    title: 'Clients — LexManager',
    loadComponent: () => import('./features/clients/clients-page').then((m) => m.ClientsPage),
  },
  {
    path: 'cases',
    title: 'Dossiers — LexManager',
    loadComponent: () => import('./features/cases/cases-page').then((m) => m.CasesPage),
  },
  {
    path: 'cases/:id',
    title: 'Dossier — LexManager',
    loadComponent: () => import('./features/cases/case-detail').then((m) => m.CaseDetail),
  },
  { path: '**', redirectTo: 'dashboard' },
];
