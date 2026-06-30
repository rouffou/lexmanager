import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ClientDetail, ClientSummary, CreateClientRequest, PagedList } from '../models';

@Injectable({ providedIn: 'root' })
export class ClientsApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/clients`;

  list(page = 1, pageSize = 25, search?: string): Observable<PagedList<ClientSummary>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<PagedList<ClientSummary>>(this.base, { params });
  }

  getById(id: string): Observable<ClientDetail> {
    return this.http.get<ClientDetail>(`${this.base}/${id}`);
  }

  create(request: CreateClientRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.base, request);
  }
}
