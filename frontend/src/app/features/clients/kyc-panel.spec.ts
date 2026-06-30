import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { KycPanel } from './kyc-panel';
import { DueDiligence } from '../../core/models';
import { environment } from '../../../environments/environment';

describe('KycPanel', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KycPanel],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideNoopAnimations()],
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('shows the start state when no due-diligence file exists', async () => {
    const fixture = TestBed.createComponent(KycPanel);
    fixture.componentRef.setInput('clientId', 'client-1');
    fixture.detectChanges();

    httpMock
      .expectOne(`${environment.apiBaseUrl}/clients/client-1/due-diligence`)
      .flush('not found', { status: 404, statusText: 'Not Found' });

    fixture.detectChanges();
    await fixture.whenStable();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Démarrer la vigilance');
  });

  it('renders the compliance score and required checks when a file exists', async () => {
    const file: DueDiligence = {
      id: 'dd-1',
      clientId: 'client-1',
      status: 'InProgress',
      riskLevel: 'Standard',
      isPoliticallyExposed: false,
      complianceScore: 50,
      canApprove: false,
      requiredChecks: ['IdentityDocument', 'AddressProof'],
      checks: [
        { kind: 'IdentityDocument', reference: 'PASS-1', cleared: true, notes: null, recordedOnUtc: '2026-01-01T10:00:00Z' },
      ],
      openedOnUtc: '2026-01-01T09:00:00Z',
      decidedOnUtc: null,
      decisionReason: null,
    };

    const fixture = TestBed.createComponent(KycPanel);
    fixture.componentRef.setInput('clientId', 'client-1');
    fixture.detectChanges();

    httpMock.expectOne(`${environment.apiBaseUrl}/clients/client-1/due-diligence`).flush(file);

    fixture.detectChanges();
    await fixture.whenStable();
    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('50 / 100');
    expect(text).toContain('Justificatif de domicile');
  });
});
