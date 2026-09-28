import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { HomePage } from './home-page/home-page';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./modules/auth/routes').then((m) => m.authRoutes),
  },
  {
    path: 'admin',
    loadChildren: () => import('./modules/admin/routes').then((m) => m.adminRoutes),
  },
  {
    path: 'player',
    loadChildren: () => import('./modules/player/routes').then((m) => m.playerRoutes),
  },
  { path: '', component: HomePage, canActivate: [authGuard] },
  { path: '**', redirectTo: '' },
];
