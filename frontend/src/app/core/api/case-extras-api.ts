import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BillingDocumentSummary, CaseTimeSummary, DocumentSummary, PagedList } from '../models';

@Injectable({ providedIn: 'root' })
export class DocumentsApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/documents`;

  byCase(caseId: string, page = 1, pageSize = 25): Observable<PagedList<DocumentSummary>> {
    const params = new HttpParams().set('caseId', caseId).set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<DocumentSummary>>(this.base, { params });
  }
}

@Injectable({ providedIn: 'root' })
export class TimeApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/time-entries`;

  summary(caseId: string): Observable<CaseTimeSummary> {
    const params = new HttpParams().set('caseId', caseId);
    return this.http.get<CaseTimeSummary>(`${this.base}/summary`, { params });
  }

  log(caseId: string, userId: string, description: string, durationMinutes: number): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.base, { caseId, userId, description, durationMinutes, isBillable: true });
  }
}

@Injectable({ providedIn: 'root' })
export class BillingApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/billing/documents`;

  byCase(caseId: string, page = 1, pageSize = 25): Observable<PagedList<BillingDocumentSummary>> {
    const params = new HttpParams().set('caseId', caseId).set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<BillingDocumentSummary>>(this.base, { params });
  }
}
