import { ChangeDetectionStrategy, Component, computed, input, linkedSignal, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateBetRequest, BetResult } from '../../../../infrastructure/api/bets/bets-api.types';
import { Match, MatchStatus } from '../../../../infrastructure/api/matches/matches-api.types';

@Component({
  imports: [FormsModule],
  selector: 'app-match-bet-row',
  templateUrl: './match-bet-row.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MatchBetRow {
  readonly match = input.required<Match>();
  readonly placedBet = input<BetResult | null>(null);
  readonly saving = input(false);
  readonly errorMessage = input<string | null>(null);

  readonly placeBet = output<CreateBetRequest>();

  protected readonly isUpcoming = computed(() => this.match().status === MatchStatus.UpcomingMatch);
  protected readonly canBet = computed(() => this.isUpcoming() && !this.placedBet());

  protected readonly localGoals = linkedSignal(() => 0);
  protected readonly visitorGoals = linkedSignal(() => 0);

  protected readonly canSubmit = computed(
    () => this.canBet() && !this.saving() && this.localGoals() >= 0 && this.visitorGoals() >= 0,
  );

  protected onSubmit(): void {
    if (!this.canSubmit()) return;

    this.placeBet.emit({
      matchId: this.match().id,
      localGoals: this.localGoals(),
      visitorGoals: this.visitorGoals(),
    });
  }
}
