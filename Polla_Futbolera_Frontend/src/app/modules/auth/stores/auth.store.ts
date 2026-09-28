import { Injectable, computed, inject, signal } from '@angular/core';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  Role,
} from '../../../infrastructure/api/auth/auth-api.types';
import { AuthApiService } from '../../../infrastructure/api/auth/auth-api.service';

export type AuthenticatedUser = AuthResponse;

const AUTH_STORAGE_KEY = 'auth.user';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly authApi = inject(AuthApiService);

  private readonly _user = signal<AuthenticatedUser | null>(null);

  readonly user = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);
  readonly role = computed<Role | null>(() => this._user()?.role ?? null);

  setup(): void {
    const stored = localStorage.getItem(AUTH_STORAGE_KEY);
    if (!stored) return;

    try {
      this._user.set(JSON.parse(stored) as AuthenticatedUser);
    } catch {
      localStorage.removeItem(AUTH_STORAGE_KEY);
    }
  }

  async login(payload: LoginRequest): Promise<void> {
    const response = await this.authApi.login(payload);
    this.persist(response);
  }

  async register(payload: RegisterRequest): Promise<void> {
    const response = await this.authApi.register(payload);
    this.persist(response);
  }

  logout(): void {
    this._user.set(null);
    localStorage.removeItem(AUTH_STORAGE_KEY);
  }

  getToken(): string | null {
    return this._user()?.token ?? null;
  }

  private persist(user: AuthenticatedUser): void {
    this._user.set(user);
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(user));
  }
}
