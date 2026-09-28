import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { UserHistory } from '../../../../infrastructure/api/leaderboard/leaderboard-api.types';

@Component({
  imports: [],
  selector: 'app-user-history-panel',
  templateUrl: './user-history-panel.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserHistoryPanel {
  readonly history = input.required<UserHistory | null>();
  readonly loading = input(false);

  readonly close = output<void>();
}
