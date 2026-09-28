import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { LeaderboardPage } from './pages/leaderboard-page/leaderboard-page';
import { MatchesPage } from './pages/matches-page/matches-page';
import { PlayerShell } from './pages/player-shell/player-shell';

export const playerRoutes: Routes = [
  {
    path: '',
    component: PlayerShell,
    canActivate: [authGuard],
    children: [
      { path: 'leaderboard', component: LeaderboardPage },
      { path: 'matches', component: MatchesPage },
      { path: '', redirectTo: 'leaderboard', pathMatch: 'full' },
    ],
  },
];
