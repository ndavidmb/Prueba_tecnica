import { Routes } from '@angular/router';
import { guestGuard } from '../../core/guards/auth.guard';
import { LoginPage } from './pages/login-page/login-page';
import { RegisterPage } from './pages/register-page/register-page';

export const authRoutes: Routes = [
  { path: 'login', component: LoginPage, canActivate: [guestGuard] },
  { path: 'register', component: RegisterPage, canActivate: [guestGuard] },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
