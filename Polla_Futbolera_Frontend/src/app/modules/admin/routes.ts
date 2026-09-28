import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';
import { MatchesPage } from './pages/matches-page/matches-page';

export const adminRoutes: Routes = [
  { path: 'matches', component: MatchesPage, canActivate: [roleGuard(['Admin'])] },
  { path: '', redirectTo: 'matches', pathMatch: 'full' },
];
