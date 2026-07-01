import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProcedurePlan, ProcedureType } from '../models';

/// Interactive procedure-tree API (SRD V11 §36).
@Injectable({ providedIn: 'root' })
export class ProcedureApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/cases`;

  byCase(caseId: string): Observable<ProcedurePlan> {
    return this.http.get<ProcedurePlan>(`${this.base}/${caseId}/procedure`);
  }

  generate(caseId: string, procedureType: ProcedureType, referenceOnUtc: string): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.base}/${caseId}/procedure`, { procedureType, referenceOnUtc });
  }

  advance(planId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/procedure/${planId}/advance`, {});
  }

  skip(planId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/procedure/${planId}/skip`, {});
  }

  schedule(planId: string, order: number, plannedOnUtc: string): Observable<void> {
    return this.http.post<void>(`${this.base}/procedure/${planId}/stages/${order}/schedule`, { plannedOnUtc });
  }
}
