import { TestBed } from '@angular/core/testing';
import { BetsApiService } from '../../../infrastructure/api/bets/bets-api.service';
import { BetResult } from '../../../infrastructure/api/bets/bets-api.types';
import { MatchesApiService } from '../../../infrastructure/api/matches/matches-api.service';
import { Match, MatchStatus } from '../../../infrastructure/api/matches/matches-api.types';
import { PlayerMatchesStore } from './matches.store';

function createMatch(id: number): Match {
  return {
    id,
    localTeam: { id: 1, name: 'River Plate' },
    visitorTeam: { id: 2, name: 'Boca Juniors' },
    localGoals: null,
    visitorGoals: null,
    status: MatchStatus.UpcomingMatch,
  };
}

function createBetResult(overrides: Partial<BetResult> = {}): BetResult {
  return {
    betId: 1,
    userId: 1,
    matchId: 1,
    predictedLocalGoals: 2,
    predictedVisitorGoals: 0,
    realLocalGoals: null,
    realVisitorGoals: null,
    pointsEarned: 0,
    isExactMatch: false,
    isTrendMatch: false,
    ...overrides,
  };
}

describe('PlayerMatchesStore', () => {
  let matchesApi: { getAll: ReturnType<typeof vi.fn> };
  let betsApi: { placeBet: ReturnType<typeof vi.fn> };
  let store: PlayerMatchesStore;

  beforeEach(() => {
    matchesApi = { getAll: vi.fn() };
    betsApi = { placeBet: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        { provide: MatchesApiService, useValue: matchesApi },
        { provide: BetsApiService, useValue: betsApi },
      ],
    });

    store = TestBed.inject(PlayerMatchesStore);
  });

  it('empieza sin partidos ni apuestas registradas', () => {
    expect(store.matches()).toEqual([]);
    expect(store.placedBets()).toEqual({});
  });

  it('setup carga los partidos desde la API', async () => {
    const matches = [createMatch(1), createMatch(2)];
    matchesApi.getAll.mockResolvedValue(matches);

    await store.setup();

    expect(store.matches()).toEqual(matches);
  });

  it('placeBet registra la apuesta bajo el id del partido correspondiente', async () => {
    const betResult = createBetResult({ matchId: 5, pointsEarned: 3 });
    betsApi.placeBet.mockResolvedValue(betResult);

    const result = await store.placeBet({ matchId: 5, localGoals: 2, visitorGoals: 0 });

    expect(result).toEqual(betResult);
    expect(store.placedBets()).toEqual({ 5: betResult });
  });

  it('placeBet conserva las apuestas de otros partidos ya registradas', async () => {
    const firstBet = createBetResult({ matchId: 1, betId: 1 });
    const secondBet = createBetResult({ matchId: 2, betId: 2 });
    betsApi.placeBet.mockResolvedValueOnce(firstBet).mockResolvedValueOnce(secondBet);

    await store.placeBet({ matchId: 1, localGoals: 1, visitorGoals: 0 });
    await store.placeBet({ matchId: 2, localGoals: 0, visitorGoals: 0 });

    expect(store.placedBets()).toEqual({ 1: firstBet, 2: secondBet });
  });
});
