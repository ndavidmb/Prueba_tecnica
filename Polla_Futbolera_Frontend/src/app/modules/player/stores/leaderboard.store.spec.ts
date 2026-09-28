import { TestBed } from '@angular/core/testing';
import { LeaderboardApiService } from '../../../infrastructure/api/leaderboard/leaderboard-api.service';
import { LeaderboardEntry, UserHistory } from '../../../infrastructure/api/leaderboard/leaderboard-api.types';
import { LeaderboardStore } from './leaderboard.store';

function createEntry(overrides: Partial<LeaderboardEntry>): LeaderboardEntry {
  return { userId: 1, userName: 'Jugador Uno', totalPoints: 0, totalBets: 0, rankPosition: 1, ...overrides };
}

describe('LeaderboardStore', () => {
  let leaderboardApi: { getLeaderboard: ReturnType<typeof vi.fn>; getUserHistory: ReturnType<typeof vi.fn> };
  let store: LeaderboardStore;

  beforeEach(() => {
    leaderboardApi = { getLeaderboard: vi.fn(), getUserHistory: vi.fn() };

    TestBed.configureTestingModule({
      providers: [{ provide: LeaderboardApiService, useValue: leaderboardApi }],
    });

    store = TestBed.inject(LeaderboardStore);
  });

  it('entries ordena las posiciones ascendentemente sin importar el orden recibido', async () => {
    leaderboardApi.getLeaderboard.mockResolvedValue([
      createEntry({ userId: 2, rankPosition: 2 }),
      createEntry({ userId: 1, rankPosition: 1 }),
    ]);

    await store.setup();

    expect(store.entries().map((entry) => entry.userId)).toEqual([1, 2]);
  });

  it('loadUserHistory expone el historial del usuario seleccionado', async () => {
    const history: UserHistory = { userId: 1, userName: 'Jugador Uno', totalPoints: 4, bets: [] };
    leaderboardApi.getUserHistory.mockResolvedValue(history);

    await store.loadUserHistory(1);

    expect(store.selectedUserHistory()).toEqual(history);
    expect(leaderboardApi.getUserHistory).toHaveBeenCalledWith(1);
  });

  it('clearSelectedUserHistory limpia el historial seleccionado', async () => {
    leaderboardApi.getUserHistory.mockResolvedValue({ userId: 1, userName: 'Jugador Uno', totalPoints: 4, bets: [] });
    await store.loadUserHistory(1);

    store.clearSelectedUserHistory();

    expect(store.selectedUserHistory()).toBeNull();
  });
});
