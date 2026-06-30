import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CaseDetail, CaseSummary, CreateCaseRequest, PagedList } from '../models';

@Injectable({ providedIn: 'root' })
export class CasesApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/cases`;

  list(page = 1, pageSize = 25, includeArchived = false): Observable<PagedList<CaseSummary>> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('includeArchived', includeArchived);
    return this.http.get<PagedList<CaseSummary>>(this.base, { params });
  }

  getById(id: string): Observable<CaseDetail> {
    return this.http.get<CaseDetail>(`${this.base}/${id}`);
  }

  create(request: CreateCaseRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.base, request);
  }

  close(id: string): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}/close`, {});
  }
}
