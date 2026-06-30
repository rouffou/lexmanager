import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DueDiligence, RiskLevel, VerificationKind } from '../models';

/// Read/write port for the client due-diligence (KYC / LCB-FT) workflow (SRD V11 §30).
@Injectable({ providedIn: 'root' })
export class KycApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/clients`;

  byClient(clientId: string): Observable<DueDiligence> {
    return this.http.get<DueDiligence>(`${this.base}/${clientId}/due-diligence`);
  }

  start(clientId: string, riskLevel: RiskLevel, isPoliticallyExposed: boolean): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.base}/${clientId}/due-diligence`, { riskLevel, isPoliticallyExposed });
  }

  recordCheck(
    fileId: string,
    kind: VerificationKind,
    reference: string,
    cleared: boolean,
    notes: string | null,
  ): Observable<void> {
    return this.http.post<void>(`${this.base}/due-diligence/${fileId}/checks`, { kind, reference, cleared, notes });
  }

  decide(fileId: string, approve: boolean, reason: string | null): Observable<void> {
    return this.http.post<void>(`${this.base}/due-diligence/${fileId}/decision`, { approve, reason });
  }
}
