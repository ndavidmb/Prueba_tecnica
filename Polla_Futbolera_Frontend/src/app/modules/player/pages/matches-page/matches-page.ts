import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CreateBetRequest } from '../../../../infrastructure/api/bets/bets-api.types';
import { MatchBetRow } from '../../components/match-bet-row/match-bet-row';
import { PlayerMatchesStore } from '../../stores/matches.store';

@Component({
  imports: [MatchBetRow],
  selector: 'app-matches-page',
  templateUrl: './matches-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MatchesPage implements OnInit {
  private readonly matchesStore = inject(PlayerMatchesStore);

  protected readonly matches = this.matchesStore.matches;
  protected readonly placedBets = this.matchesStore.placedBets;

  protected readonly loading = signal(false);
  protected readonly loadErrorMessage = signal<string | null>(null);
  protected readonly savingMatchId = signal<number | null>(null);
  protected readonly errorsByMatchId = signal<Record<number, string>>({});

  async ngOnInit(): Promise<void> {
    this.loading.set(true);
    try {
      await this.matchesStore.setup();
    } catch {
      this.loadErrorMessage.set('No se pudieron cargar los partidos.');
    } finally {
      this.loading.set(false);
    }
  }

  protected async onPlaceBet(matchId: number, payload: CreateBetRequest): Promise<void> {
    this.savingMatchId.set(matchId);
    this.errorsByMatchId.update(({ [matchId]: _removed, ...rest }) => rest);

    try {
      await this.matchesStore.placeBet(payload);
    } catch (error) {
      this.errorsByMatchId.update((errors) => ({ ...errors, [matchId]: this.extractErrorMessage(error) }));
    } finally {
      this.savingMatchId.set(null);
    }
  }

  private extractErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse && typeof error.error?.message === 'string') {
      return error.error.message;
    }
    return 'No se pudo registrar la apuesta.';
  }
}
