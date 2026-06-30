import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { CarpaPanel } from './carpa-panel';
import { CarpaAccount } from '../../core/models';
import { environment } from '../../../environments/environment';

describe('CarpaPanel', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarpaPanel],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideNoopAnimations()],
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('shows the empty state when no account exists for the case', async () => {
    const fixture = TestBed.createComponent(CarpaPanel);
    fixture.componentRef.setInput('caseId', 'case-1');
    fixture.componentRef.setInput('clientId', 'client-1');
    fixture.detectChanges();

    httpMock
      .expectOne(`${environment.apiBaseUrl}/billing/carpa/cases/case-1/account`)
      .flush('not found', { status: 404, statusText: 'Not Found' });

    fixture.detectChanges();
    await fixture.whenStable();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Aucun compte de tiers');
  });

  it('renders the balance and movement trace when the account exists', async () => {
    const account: CarpaAccount = {
      id: 'acc-1',
      caseId: 'case-1',
      clientId: 'client-1',
      currency: 'EUR',
      balance: 600,
      transactions: [
        { type: 'Deposit', amount: 1000, description: 'Provision', counterparty: 'Client', occurredOnUtc: '2026-01-01T10:00:00Z' },
        { type: 'Disbursement', amount: 400, description: 'Versement', counterparty: 'Tiers', occurredOnUtc: '2026-02-01T10:00:00Z' },
      ],
      openedOnUtc: '2026-01-01T09:00:00Z',
    };

    const fixture = TestBed.createComponent(CarpaPanel);
    fixture.componentRef.setInput('caseId', 'case-1');
    fixture.componentRef.setInput('clientId', 'client-1');
    fixture.detectChanges();

    httpMock.expectOne(`${environment.apiBaseUrl}/billing/carpa/cases/case-1/account`).flush(account);

    fixture.detectChanges();
    await fixture.whenStable();
    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('600.00');
    expect(text).toContain('Provision');
  });
});
