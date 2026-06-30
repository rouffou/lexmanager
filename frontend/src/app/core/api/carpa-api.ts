import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CarpaAccount } from '../models';

/// Read/write port for CARPA third-party accounts (comptes rubriqués, SRD V11 §5).
@Injectable({ providedIn: 'root' })
export class CarpaApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/billing/carpa`;

  byCase(caseId: string): Observable<CarpaAccount> {
    return this.http.get<CarpaAccount>(`${this.base}/cases/${caseId}/account`);
  }

  open(caseId: string, clientId: string, currency = 'EUR'): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.base}/accounts`, { caseId, clientId, currency });
  }

  deposit(accountId: string, amount: number, description: string, counterparty: string | null): Observable<void> {
    return this.http.post<void>(`${this.base}/accounts/${accountId}/deposits`, { amount, description, counterparty });
  }

  disburse(accountId: string, amount: number, description: string, counterparty: string | null): Observable<void> {
    return this.http.post<void>(`${this.base}/accounts/${accountId}/disbursements`, { amount, description, counterparty });
  }
}
