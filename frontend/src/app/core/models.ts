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
