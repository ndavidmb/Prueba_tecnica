import { TestBed } from '@angular/core/testing';
import { MatchesApiService } from '../../../infrastructure/api/matches/matches-api.service';
import { Match, MatchStatus } from '../../../infrastructure/api/matches/matches-api.types';
import { MatchesStore } from './matches.store';

function createMatch(id: number, overrides: Partial<Match> = {}): Match {
  return {
    id,
    localTeam: { id: 1, name: 'River Plate' },
    visitorTeam: { id: 2, name: 'Boca Juniors' },
    localGoals: null,
    visitorGoals: null,
    status: MatchStatus.UpcomingMatch,
    ...overrides,
  };
}

describe('MatchesStore (admin)', () => {
  let matchesApi: { getAll: ReturnType<typeof vi.fn>; updateResult: ReturnType<typeof vi.fn> };
  let store: MatchesStore;

  beforeEach(() => {
    matchesApi = { getAll: vi.fn(), updateResult: vi.fn() };

    TestBed.configureTestingModule({
      providers: [{ provide: MatchesApiService, useValue: matchesApi }],
    });

    store = TestBed.inject(MatchesStore);
  });

  it('setup carga todos los partidos', async () => {
    const matches = [createMatch(1), createMatch(2)];
    matchesApi.getAll.mockResolvedValue(matches);

    await store.setup();

    expect(store.matches()).toEqual(matches);
  });

  it('updateResult actualiza únicamente el partido afectado y deja los demás intactos', async () => {
    matchesApi.getAll.mockResolvedValue([createMatch(1), createMatch(2)]);
    await store.setup();

    matchesApi.updateResult.mockResolvedValue({
      id: 1,
      localGoals: 2,
      visitorGoals: 1,
      status: MatchStatus.FullTime,
    });

    await store.updateResult(1, { localGoals: 2, visitorGoals: 1 });

    const [updated, untouched] = store.matches();
    expect(updated).toMatchObject({ id: 1, localGoals: 2, visitorGoals: 1, status: MatchStatus.FullTime });
    expect(untouched).toEqual(createMatch(2));
  });
});
