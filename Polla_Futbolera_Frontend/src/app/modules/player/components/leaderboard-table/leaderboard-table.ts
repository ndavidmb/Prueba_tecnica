import { ChangeDetectionStrategy, Component, inject, input, output } from '@angular/core';
import { LeaderboardEntry } from '../../../../infrastructure/api/leaderboard/leaderboard-api.types';
import { AuthStore } from '../../../auth/stores/auth.store';

@Component({
  imports: [],
  selector: 'app-leaderboard-table',
  templateUrl: './leaderboard-table.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeaderboardTable {
  private readonly authStore = inject(AuthStore);
  readonly entries = input.required<LeaderboardEntry[]>();
  readonly selectedUserId = input<number | null>(null);

  protected readonly user = this.authStore.user;

  readonly selectUser = output<number>();
}
