import { TestBed } from '@angular/core/testing';
import { AuthApiService } from '../../../infrastructure/api/auth/auth-api.service';
import { AuthResponse } from '../../../infrastructure/api/auth/auth-api.types';
import { AuthStore } from './auth.store';

const AUTH_STORAGE_KEY = 'auth.user';

function createAuthResponse(overrides: Partial<AuthResponse> = {}): AuthResponse {
  return { token: 'token-abc', email: 'player@test.com', role: 'Player', name: 'Jugador Uno', ...overrides };
}

describe('AuthStore', () => {
  let authApi: { login: ReturnType<typeof vi.fn>; register: ReturnType<typeof vi.fn> };
  let store: AuthStore;

  beforeEach(() => {
    localStorage.clear();
    authApi = { login: vi.fn(), register: vi.fn() };

    TestBed.configureTestingModule({
      providers: [{ provide: AuthApiService, useValue: authApi }],
    });

    store = TestBed.inject(AuthStore);
  });

  it('no hay usuario autenticado antes de llamar a setup o login', () => {
    expect(store.isAuthenticated()).toBe(false);
    expect(store.role()).toBeNull();
  });

  it('login persiste el usuario y lo expone como autenticado', async () => {
    const response = createAuthResponse({ role: 'Admin' });
    authApi.login.mockResolvedValue(response);

    await store.login({ email: response.email, password: 'secret' });

    expect(store.isAuthenticated()).toBe(true);
    expect(store.role()).toBe('Admin');
    expect(store.getToken()).toBe(response.token);
    expect(JSON.parse(localStorage.getItem(AUTH_STORAGE_KEY)!)).toEqual(response);
  });

  it('setup recupera la sesión guardada en localStorage', () => {
    const response = createAuthResponse();
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(response));

    store.setup();

    expect(store.user()).toEqual(response);
  });

  it('setup descarta datos corruptos en localStorage sin lanzar errores', () => {
    localStorage.setItem(AUTH_STORAGE_KEY, '{not-json');

    expect(() => store.setup()).not.toThrow();
    expect(store.isAuthenticated()).toBe(false);
    expect(localStorage.getItem(AUTH_STORAGE_KEY)).toBeNull();
  });

  it('logout limpia el usuario y el localStorage', async () => {
    authApi.login.mockResolvedValue(createAuthResponse());
    await store.login({ email: 'player@test.com', password: 'secret' });

    store.logout();

    expect(store.isAuthenticated()).toBe(false);
    expect(store.getToken()).toBeNull();
    expect(localStorage.getItem(AUTH_STORAGE_KEY)).toBeNull();
  });
});
