import * as yup from 'yup';

export type Role = 'Admin' | 'Player';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  role: Role;
  name: string;
}

export const authResponseSchema: yup.ObjectSchema<AuthResponse> = yup.object({
  token: yup.string().required(),
  email: yup.string().email().required(),
  role: yup.mixed<Role>().oneOf(['Admin', 'Player']).required(),
  name: yup.string().required(),
});
