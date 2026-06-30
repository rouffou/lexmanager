// DTOs mirroring the backend Contracts assemblies (kept intentionally flat).

export interface PagedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// ─── Identity & CRM ───────────────────────────────────────────────────────────
export type ClientType = 'PhysicalPerson' | 'LegalPerson';

export interface ClientSummary {
  id: string;
  type: ClientType;
  displayName: string;
  email: string;
  createdOnUtc: string;
}

export interface CreateClientRequest {
  type: ClientType;
  email: string;
  phone?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  nationalIdentityNumber?: string | null;
  companyName?: string | null;
  registrationNumber?: string | null;
  legalRepresentative?: string | null;
}

export interface ClientDetail {
  id: string;
  type: string;
  displayName: string;
  email: string;
  phone?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  nationalIdentityNumber?: string | null;
  companyName?: string | null;
  registrationNumber?: string | null;
  legalRepresentative?: string | null;
  createdOnUtc: string;
}

// ─── KYC / LCB-FT (devoir de vigilance), V11 §30 ────────────────────────────────
export type DueDiligenceStatus = 'InProgress' | 'Approved' | 'Rejected';
export type RiskLevel = 'Low' | 'Standard' | 'High';
export type VerificationKind =
  | 'IdentityDocument'
  | 'AddressProof'
  | 'CompanyRegistry'
  | 'BeneficialOwner'
  | 'PepScreening'
  | 'SanctionsScreening';

export interface VerificationCheck {
  kind: VerificationKind;
  reference: string;
  cleared: boolean;
  notes?: string | null;
  recordedOnUtc: string;
}

export interface DueDiligence {
  id: string;
  clientId: string;
  status: DueDiligenceStatus;
  riskLevel: RiskLevel;
  isPoliticallyExposed: boolean;
  complianceScore: number;
  canApprove: boolean;
  requiredChecks: VerificationKind[];
  checks: VerificationCheck[];
  openedOnUtc: string;
  decidedOnUtc?: string | null;
  decisionReason?: string | null;
}

// ─── Case Management ──────────────────────────────────────────────────────────
export interface CaseSummary {
  id: string;
  title: string;
  clientId: string;
  status: string;
  openedOnUtc: string;
}

export interface CaseDetail extends CaseSummary {
  isArchived: boolean;
  jurisdiction?: { courtName: string; generalRegisterNumber: string; judge?: string } | null;
  adverseParties: { name: string; counsel?: string }[];
  closedOnUtc?: string | null;
}

export interface CreateCaseRequest {
  title: string;
  clientId: string;
  courtName?: string | null;
  generalRegisterNumber?: string | null;
  judge?: string | null;
}

// ─── Documents ────────────────────────────────────────────────────────────────
export interface DocumentSummary {
  id: string;
  caseId: string;
  fileName: string;
  category: string;
  currentVersion: number;
  createdOnUtc: string;
}

// ─── Calendar & Time ──────────────────────────────────────────────────────────
export interface CaseTimeSummary {
  caseId: string;
  totalMinutes: number;
  billableMinutes: number;
  entryCount: number;
}

// ─── Billing ──────────────────────────────────────────────────────────────────
export interface BillingDocumentSummary {
  id: string;
  kind: string;
  status: string;
  number?: string | null;
  total: number;
  currency: string;
  dueDateUtc?: string | null;
  createdOnUtc: string;
}

// ─── Billing — CARPA / comptes de tiers (rubriqués), V11 §5 ─────────────────────
export type CarpaTransactionType = 'Deposit' | 'Disbursement';

export interface CarpaTransaction {
  type: CarpaTransactionType;
  amount: number;
  description: string;
  counterparty?: string | null;
  occurredOnUtc: string;
}

export interface CarpaAccount {
  id: string;
  caseId: string;
  clientId: string;
  currency: string;
  balance: number;
  transactions: CarpaTransaction[];
  openedOnUtc: string;
}
