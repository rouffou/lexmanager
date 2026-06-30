import { Injectable, signal } from '@angular/core';

const TOKEN_KEY = 'lex.token';

/// Minimal token holder for the OAuth2/OIDC bearer flow. A full implementation would
/// redirect to the IdP; here we store/clear the JWT and expose it to the interceptor.
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly token = signal<string | null>(localStorage.getItem(TOKEN_KEY));

  readonly isAuthenticated = signal<boolean>(!!localStorage.getItem(TOKEN_KEY));

  getToken(): string | null {
    return this.token();
  }

  setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
    this.token.set(token);
    this.isAuthenticated.set(true);
  }

  clear(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.token.set(null);
    this.isAuthenticated.set(false);
  }
}
