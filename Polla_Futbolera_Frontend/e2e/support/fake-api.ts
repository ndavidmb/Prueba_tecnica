import { Page, Route } from '@playwright/test';

type Role = 'Admin' | 'Player';

interface TeamRecord {
  id: number;
  name: string;
}

interface MatchRecord {
  id: number;
  localTeamId: number;
  visitorTeamId: number;
  localGoals: number | null;
  visitorGoals: number | null;
  status: 1 | 2;
}

interface BetRecord {
  betId: number;
  userId: number;
  matchId: number;
  predictedLocalGoals: number;
  predictedVisitorGoals: number;
}

interface UserRecord {
  id: number;
  name: string;
  email: string;
  password: string;
  role: Role;
}

/**
 * Backend en memoria para los tests de Playwright: intercepta las llamadas a /api/**
 * y reproduce las reglas de negocio reales (regla de puntaje 3/1/0 de Bet.CalculateAndAssignPoints
 * en el backend .NET) para poder ejercitar la app real sin depender de un servidor ni una base de datos.
 */
export class FakeBackend {
  private readonly teams: TeamRecord[] = [
    { id: 1, name: 'River Plate' },
    { id: 2, name: 'Boca Juniors' },
    { id: 3, name: 'Racing Club' },
    { id: 4, name: 'Independiente' },
    { id: 5, name: 'San Lorenzo' },
    { id: 6, name: 'Vélez Sarsfield' },
  ];

  private readonly matches: MatchRecord[] = [
    { id: 1, localTeamId: 1, visitorTeamId: 2, localGoals: null, visitorGoals: null, status: 1 },
    { id: 2, localTeamId: 3, visitorTeamId: 4, localGoals: null, visitorGoals: null, status: 1 },
    { id: 3, localTeamId: 5, visitorTeamId: 6, localGoals: null, visitorGoals: null, status: 1 },
  ];

  private readonly users: UserRecord[] = [
    { id: 1, name: 'Jugador Uno', email: 'player@test.com', password: 'Player123!', role: 'Player' },
    { id: 2, name: 'Administrador', email: 'admin@test.com', password: 'Admin123!', role: 'Admin' },
  ];

  private readonly bets: BetRecord[] = [];
  private nextBetId = 1;

  async install(page: Page): Promise<void> {
    await page.route('**/api/**', (route) => this.dispatch(route));
  }

  private async dispatch(route: Route): Promise<void> {
    const request = route.request();
    const method = request.method();
    const path = new URL(request.url()).pathname;

    if (method === 'POST' && path === '/api/auth/login') return this.handleLogin(route);
    if (method === 'GET' && path === '/api/matches') return this.handleGetMatches(route);
    if (method === 'POST' && path === '/api/bets') return this.handlePlaceBet(route);
    if (method === 'GET' && path === '/api/leaderboard') return this.handleLeaderboard(route);

    const resultMatch = path.match(/^\/api\/admin\/matches\/(\d+)\/result$/);
    if (method === 'PUT' && resultMatch) return this.handleUpdateResult(route, Number(resultMatch[1]));

    const historyMatch = path.match(/^\/api\/leaderboard\/users\/(\d+)\/history$/);
    if (method === 'GET' && historyMatch) return this.handleUserHistory(route, Number(historyMatch[1]));

    return route.fulfill({ status: 404, json: { message: `Ruta no simulada: ${method} ${path}` } });
  }

  private userFromAuthHeader(route: Route): UserRecord | null {
    const header = route.request().headers()['authorization'];
    if (!header?.startsWith('Bearer fake-token-')) return null;
    const userId = Number(header.replace('Bearer fake-token-', ''));
    return this.users.find((u) => u.id === userId) ?? null;
  }

  private teamDto(teamId: number) {
    const team = this.teams.find((t) => t.id === teamId)!;
    return { id: team.id, name: team.name };
  }

  private computePoints(predictedLocal: number, predictedVisitor: number, realLocal: number, realVisitor: number): number {
    if (predictedLocal === realLocal && predictedVisitor === realVisitor) return 3;
    const predictedTrend = Math.sign(predictedLocal - predictedVisitor);
    const realTrend = Math.sign(realLocal - realVisitor);
    return predictedTrend === realTrend ? 1 : 0;
  }

  private pointsForBet(bet: BetRecord): number {
    const match = this.matches.find((m) => m.id === bet.matchId)!;
    if (match.localGoals === null || match.visitorGoals === null) return 0;
    return this.computePoints(bet.predictedLocalGoals, bet.predictedVisitorGoals, match.localGoals, match.visitorGoals);
  }

  private async handleLogin(route: Route): Promise<void> {
    const body = route.request().postDataJSON() as { email: string; password: string };
    const user = this.users.find((u) => u.email === body.email && u.password === body.password);

    if (!user) {
      return route.fulfill({ status: 401, json: { message: 'Credenciales inválidas.' } });
    }

    return route.fulfill({
      status: 200,
      json: { token: `fake-token-${user.id}`, email: user.email, role: user.role, name: user.name },
    });
  }

  private async handleGetMatches(route: Route): Promise<void> {
    const dto = this.matches.map((match) => ({
      id: match.id,
      localTeam: this.teamDto(match.localTeamId),
      visitorTeam: this.teamDto(match.visitorTeamId),
      localGoals: match.localGoals,
      visitorGoals: match.visitorGoals,
      status: match.status,
    }));

    return route.fulfill({ status: 200, json: dto });
  }

  private async handlePlaceBet(route: Route): Promise<void> {
    const user = this.userFromAuthHeader(route);
    if (!user) return route.fulfill({ status: 401, json: { message: 'No autenticado.' } });

    const body = route.request().postDataJSON() as {
      matchId: number;
      localGoals: number;
      visitorGoals: number;
    };
    const match = this.matches.find((m) => m.id === body.matchId);
    if (!match) return route.fulfill({ status: 404, json: { message: 'El partido no existe.' } });

    const bet: BetRecord = {
      betId: this.nextBetId++,
      userId: user.id,
      matchId: body.matchId,
      predictedLocalGoals: body.localGoals,
      predictedVisitorGoals: body.visitorGoals,
    };
    this.bets.push(bet);

    const pointsEarned = this.pointsForBet(bet);

    return route.fulfill({
      status: 201,
      json: {
        betId: bet.betId,
        userId: bet.userId,
        matchId: bet.matchId,
        predictedLocalGoals: bet.predictedLocalGoals,
        predictedVisitorGoals: bet.predictedVisitorGoals,
        realLocalGoals: match.localGoals,
        realVisitorGoals: match.visitorGoals,
        pointsEarned,
        isExactMatch: pointsEarned === 3,
        isTrendMatch: pointsEarned === 1,
      },
    });
  }

  private async handleUpdateResult(route: Route, matchId: number): Promise<void> {
    const match = this.matches.find((m) => m.id === matchId);
    if (!match) return route.fulfill({ status: 400, json: { message: 'El partido no existe.' } });

    const body = route.request().postDataJSON() as { localGoals: number; visitorGoals: number };
    match.localGoals = body.localGoals;
    match.visitorGoals = body.visitorGoals;
    match.status = 2;

    return route.fulfill({
      status: 200,
      json: { id: match.id, localGoals: match.localGoals, visitorGoals: match.visitorGoals, status: match.status },
    });
  }

  private async handleLeaderboard(route: Route): Promise<void> {
    const entries = this.users
      .filter((user) => user.role === 'Player')
      .map((player) => {
        const playerBets = this.bets.filter((bet) => bet.userId === player.id);
        const totalPoints = playerBets.reduce((sum, bet) => sum + this.pointsForBet(bet), 0);
        return { userId: player.id, userName: player.name, totalPoints, totalBets: playerBets.length };
      })
      .sort((a, b) => b.totalPoints - a.totalPoints)
      .map((entry, index) => ({ ...entry, rankPosition: index + 1 }));

    return route.fulfill({ status: 200, json: entries });
  }

  private async handleUserHistory(route: Route, userId: number): Promise<void> {
    const user = this.users.find((u) => u.id === userId);
    if (!user) return route.fulfill({ status: 404, json: { message: 'El usuario no existe.' } });

    const bets = this.bets
      .filter((bet) => bet.userId === userId)
      .map((bet) => {
        const match = this.matches.find((m) => m.id === bet.matchId)!;
        return {
          betId: bet.betId,
          matchId: bet.matchId,
          localTeamName: this.teamDto(match.localTeamId).name,
          visitorTeamName: this.teamDto(match.visitorTeamId).name,
          predictedLocalGoals: bet.predictedLocalGoals,
          predictedVisitorGoals: bet.predictedVisitorGoals,
          realLocalGoals: match.localGoals,
          realVisitorGoals: match.visitorGoals,
          pointsEarned: this.pointsForBet(bet),
        };
      });
    const totalPoints = bets.reduce((sum, bet) => sum + bet.pointsEarned, 0);

    return route.fulfill({ status: 200, json: { userId: user.id, userName: user.name, totalPoints, bets } });
  }
}
