import { ChangeDetectionStrategy, Component, computed, input, linkedSignal, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Match, MatchStatus, UpdateMatchResultRequest } from '../../../../infrastructure/api/matches/matches-api.types';

@Component({
  imports: [FormsModule],
  selector: 'app-match-row',
  templateUrl: './match-row.html',
  styleUrl: './match-row.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MatchRow {
  readonly match = input.required<Match>();
  readonly saving = input(false);

  readonly save = output<UpdateMatchResultRequest>();

  protected readonly editable = computed(() => this.match().status === MatchStatus.UpcomingMatch);

  protected readonly localGoals = linkedSignal(() => this.match().localGoals ?? 0);
  protected readonly visitorGoals = linkedSignal(() => this.match().visitorGoals ?? 0);

  protected readonly canSave = computed(
    () => this.editable() && !this.saving() && this.localGoals() >= 0 && this.visitorGoals() >= 0,
  );

  protected onSave(): void {
    if (!this.canSave()) return;

    this.save.emit({ localGoals: this.localGoals(), visitorGoals: this.visitorGoals() });
  }
}
