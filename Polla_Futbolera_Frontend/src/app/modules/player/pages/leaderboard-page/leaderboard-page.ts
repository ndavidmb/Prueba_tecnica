import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { LeaderboardTable } from '../../components/leaderboard-table/leaderboard-table';
import { UserHistoryPanel } from '../../components/user-history-panel/user-history-panel';
import { LeaderboardStore } from '../../stores/leaderboard.store';

@Component({
  imports: [LeaderboardTable, UserHistoryPanel],
  selector: 'app-leaderboard-page',
  templateUrl: './leaderboard-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeaderboardPage implements OnInit {
  private readonly leaderboardStore = inject(LeaderboardStore);

  protected readonly entries = this.leaderboardStore.entries;
  protected readonly selectedUserHistory = this.leaderboardStore.selectedUserHistory;

  protected readonly loading = signal(false);
  protected readonly historyLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly selectedUserId = signal<number | null>(null);

  async ngOnInit(): Promise<void> {
    this.loading.set(true);
    try {
      await this.leaderboardStore.setup();
    } catch {
      this.errorMessage.set('No se pudo cargar el leaderboard.');
    } finally {
      this.loading.set(false);
    }
  }

  protected async onSelectUser(userId: number): Promise<void> {
    if (this.selectedUserId() === userId) {
      this.onCloseHistory();
      return;
    }

    this.selectedUserId.set(userId);
    this.historyLoading.set(true);

    try {
      await this.leaderboardStore.loadUserHistory(userId);
    } catch {
      this.errorMessage.set('No se pudo cargar el historial del usuario.');
    } finally {
      this.historyLoading.set(false);
    }
  }

  protected onCloseHistory(): void {
    this.selectedUserId.set(null);
    this.leaderboardStore.clearSelectedUserHistory();
  }
}
