import { Injectable, inject, signal } from '@angular/core';
import { BetsApiService } from '../../../infrastructure/api/bets/bets-api.service';
import { BetResult, CreateBetRequest } from '../../../infrastructure/api/bets/bets-api.types';
import { MatchesApiService } from '../../../infrastructure/api/matches/matches-api.service';
import { Match } from '../../../infrastructure/api/matches/matches-api.types';

@Injectable({ providedIn: 'root' })
export class PlayerMatchesStore {
  private readonly matchesApi = inject(MatchesApiService);
  private readonly betsApi = inject(BetsApiService);

  private readonly _matches = signal<Match[]>([]);
  private readonly _placedBets = signal<Record<number, BetResult>>({});

  readonly matches = this._matches.asReadonly();
  readonly placedBets = this._placedBets.asReadonly();

  async setup(): Promise<void> {
    this._matches.set(await this.matchesApi.getAll());
  }

  async placeBet(payload: CreateBetRequest): Promise<BetResult> {
    const result = await this.betsApi.placeBet(payload);
    this._placedBets.update((bets) => ({ ...bets, [payload.matchId]: result }));
    return result;
  }
}
