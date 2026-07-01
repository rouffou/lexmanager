import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { DocumentSearchPage } from './document-search-page';
import { DocumentSearchResult, PagedList } from '../../core/models';
import { environment } from '../../../environments/environment';

describe('DocumentSearchPage', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DocumentSearchPage],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideNoopAnimations(), provideRouter([])],
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('searches and renders matching documents', async () => {
    const fixture = TestBed.createComponent(DocumentSearchPage);
    fixture.detectChanges();

    const component = fixture.componentInstance as unknown as {
      form: { setValue: (v: { term: string }) => void };
      submit: () => void;
    };
    component.form.setValue({ term: 'bail' });
    component.submit();

    const req = httpMock.expectOne((r) => r.url === `${environment.apiBaseUrl}/documents/search`);
    expect(req.request.params.get('q')).toBe('bail');

    const page: PagedList<DocumentSearchResult> = {
      items: [
        {
          id: 'd-1',
          caseId: 'c-1',
          fileName: 'note-bail.txt',
          category: 'ProcedureDocument',
          highlight: '… contrat de bail commercial …',
          createdOnUtc: '2026-01-01T10:00:00Z',
        },
      ],
      page: 1,
      pageSize: 25,
      totalCount: 1,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false,
    };
    req.flush(page);

    fixture.detectChanges();
    await fixture.whenStable();
    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('note-bail.txt');
    expect(text).toContain('contrat de bail');
  });

  it('shows an empty message when there are no matches', async () => {
    const fixture = TestBed.createComponent(DocumentSearchPage);
    fixture.detectChanges();

    const component = fixture.componentInstance as unknown as {
      form: { setValue: (v: { term: string }) => void };
      submit: () => void;
    };
    component.form.setValue({ term: 'hypothèque' });
    component.submit();

    const page: PagedList<DocumentSearchResult> = {
      items: [],
      page: 1,
      pageSize: 25,
      totalCount: 0,
      totalPages: 0,
      hasPreviousPage: false,
      hasNextPage: false,
    };
    httpMock.expectOne((r) => r.url === `${environment.apiBaseUrl}/documents/search`).flush(page);

    fixture.detectChanges();
    await fixture.whenStable();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Aucune pièce ne correspond');
  });
});
