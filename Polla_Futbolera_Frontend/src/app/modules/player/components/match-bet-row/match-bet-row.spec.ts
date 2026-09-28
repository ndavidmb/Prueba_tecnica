import { TestBed } from '@angular/core/testing';
import { BetResult, CreateBetRequest } from '../../../../infrastructure/api/bets/bets-api.types';
import { Match, MatchStatus } from '../../../../infrastructure/api/matches/matches-api.types';
import { MatchBetRow } from './match-bet-row';

function createMatch(overrides: Partial<Match> = {}): Match {
  return {
    id: 1,
    localTeam: { id: 1, name: 'River Plate' },
    visitorTeam: { id: 2, name: 'Boca Juniors' },
    localGoals: null,
    visitorGoals: null,
    status: MatchStatus.UpcomingMatch,
    ...overrides,
  };
}

function createBetResult(overrides: Partial<BetResult> = {}): BetResult {
  return {
    betId: 1,
    userId: 1,
    matchId: 1,
    predictedLocalGoals: 1,
    predictedVisitorGoals: 0,
    realLocalGoals: null,
    realVisitorGoals: null,
    pointsEarned: 0,
    isExactMatch: false,
    isTrendMatch: false,
    ...overrides,
  };
}

describe('MatchBetRow', () => {
  function createComponent() {
    const fixture = TestBed.createComponent(MatchBetRow);
    fixture.componentRef.setInput('match', createMatch());
    return fixture;
  }

  it('permite apostar en un partido próximo sin apuesta previa', () => {
    const fixture = createComponent();
    fixture.detectChanges();

    expect(fixture.componentInstance['canSubmit']()).toBe(true);
  });

  it('no permite apostar si el partido ya tiene una apuesta registrada', () => {
    const fixture = createComponent();
    fixture.componentRef.setInput('placedBet', createBetResult());
    fixture.detectChanges();

    expect(fixture.componentInstance['canSubmit']()).toBe(false);
  });

  it('no permite apostar en un partido finalizado', () => {
    const fixture = createComponent();
    fixture.componentRef.setInput('match', createMatch({ status: MatchStatus.FullTime, localGoals: 1, visitorGoals: 0 }));
    fixture.detectChanges();

    expect(fixture.componentInstance['canSubmit']()).toBe(false);
  });

  it('no permite apostar mientras se está guardando', () => {
    const fixture = createComponent();
    fixture.componentRef.setInput('saving', true);
    fixture.detectChanges();

    expect(fixture.componentInstance['canSubmit']()).toBe(false);
  });

  it('emite placeBet con el id del partido y el marcador cargado', () => {
    const fixture = createComponent();
    fixture.detectChanges();

    const emitted: CreateBetRequest[] = [];
    fixture.componentInstance.placeBet.subscribe((payload) => emitted.push(payload));

    fixture.componentInstance['localGoals'].set(2);
    fixture.componentInstance['visitorGoals'].set(1);
    fixture.componentInstance['onSubmit']();

    expect(emitted).toEqual([{ matchId: 1, localGoals: 2, visitorGoals: 1 }]);
  });
});
