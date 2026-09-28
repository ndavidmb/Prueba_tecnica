import { Injectable, computed, inject, signal } from '@angular/core';
import { LeaderboardApiService } from '../../../infrastructure/api/leaderboard/leaderboard-api.service';
import { LeaderboardEntry, UserHistory } from '../../../infrastructure/api/leaderboard/leaderboard-api.types';

@Injectable({ providedIn: 'root' })
export class LeaderboardStore {
  private readonly leaderboardApi = inject(LeaderboardApiService);

  private readonly _entries = signal<LeaderboardEntry[]>([]);
  private readonly _selectedUserHistory = signal<UserHistory | null>(null);

  readonly entries = computed(() => [...this._entries()].sort((a, b) => a.rankPosition - b.rankPosition));
  readonly selectedUserHistory = this._selectedUserHistory.asReadonly();

  async setup(): Promise<void> {
    this._entries.set(await this.leaderboardApi.getLeaderboard());
  }

  async loadUserHistory(userId: number): Promise<void> {
    this._selectedUserHistory.set(await this.leaderboardApi.getUserHistory(userId));
  }

  clearSelectedUserHistory(): void {
    this._selectedUserHistory.set(null);
  }
}
