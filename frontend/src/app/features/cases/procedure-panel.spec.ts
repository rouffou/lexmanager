import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ProcedurePanel } from './procedure-panel';
import { ProcedurePlan } from '../../core/models';
import { environment } from '../../../environments/environment';

describe('ProcedurePanel', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProcedurePanel],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideNoopAnimations()],
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('shows the generate state when no plan exists for the case', async () => {
    const fixture = TestBed.createComponent(ProcedurePanel);
    fixture.componentRef.setInput('caseId', 'case-1');
    fixture.detectChanges();

    httpMock
      .expectOne(`${environment.apiBaseUrl}/cases/case-1/procedure`)
      .flush('not found', { status: 404, statusText: 'Not Found' });

    fixture.detectChanges();
    await fixture.whenStable();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain("Générer l'arbre");
  });

  it('renders the interactive timeline when a plan exists', async () => {
    const plan: ProcedurePlan = {
      id: 'plan-1',
      caseId: 'case-1',
      type: 'DebtRecovery',
      referenceOnUtc: '2026-01-15T00:00:00Z',
      progressPercent: 25,
      currentStageOrder: 2,
      isComplete: false,
      stages: [
        { order: 1, name: 'Mise en demeure', phase: 'Phase amiable', status: 'Completed', plannedOnUtc: null, completedOnUtc: '2026-01-20T00:00:00Z' },
        { order: 2, name: 'Citation / requête en paiement', phase: 'Introduction', status: 'Current', plannedOnUtc: null, completedOnUtc: null },
        { order: 3, name: 'Audience d\'introduction', phase: 'Introduction', status: 'Pending', plannedOnUtc: null, completedOnUtc: null },
      ],
      createdOnUtc: '2026-01-15T09:00:00Z',
    };

    const fixture = TestBed.createComponent(ProcedurePanel);
    fixture.componentRef.setInput('caseId', 'case-1');
    fixture.detectChanges();

    httpMock.expectOne(`${environment.apiBaseUrl}/cases/case-1/procedure`).flush(plan);

    fixture.detectChanges();
    await fixture.whenStable();
    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Recouvrement de créance');
    expect(text).toContain('25 %');
    expect(text).toContain('Mise en demeure');
    expect(text).toContain('Franchir');
  });
});
