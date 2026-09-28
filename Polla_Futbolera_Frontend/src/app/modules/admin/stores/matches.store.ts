import { Injectable, inject, signal } from '@angular/core';
import { MatchesApiService } from '../../../infrastructure/api/matches/matches-api.service';
import { Match, UpdateMatchResultRequest } from '../../../infrastructure/api/matches/matches-api.types';

@Injectable({ providedIn: 'root' })
export class MatchesStore {
  private readonly matchesApi = inject(MatchesApiService);

  private readonly _matches = signal<Match[]>([]);
  readonly matches = this._matches.asReadonly();

  async setup(): Promise<void> {
    const matches = await this.matchesApi.getAll();
    this._matches.set(matches);
  }

  async updateResult(id: number, payload: UpdateMatchResultRequest): Promise<void> {
    const updated = await this.matchesApi.updateResult(id, payload);
    this._matches.update((matches) =>
      matches.map((match) =>
        match.id === id
          ? { ...match, localGoals: updated.localGoals, visitorGoals: updated.visitorGoals, status: updated.status }
          : match,
      ),
    );
  }
}
