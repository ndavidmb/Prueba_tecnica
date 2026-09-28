import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, authResponseSchema } from './auth-api.types';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  async login(payload: LoginRequest): Promise<AuthResponse> {
    const raw = await firstValueFrom(this.http.post(`${this.baseUrl}/login`, payload));
    return this.mapper(raw);
  }

  async register(payload: RegisterRequest): Promise<AuthResponse> {
    const raw = await firstValueFrom(this.http.post(`${this.baseUrl}/register`, payload));
    return this.mapper(raw);
  }

  private mapper(raw: unknown): AuthResponse {
    return authResponseSchema.validateSync(raw, { strict: true, abortEarly: false });
  }
}
