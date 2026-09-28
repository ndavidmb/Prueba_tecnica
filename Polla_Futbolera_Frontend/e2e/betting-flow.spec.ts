import { Locator, expect, test } from '@playwright/test';
import { FakeBackend } from './support/fake-api';

const PLAYER = { email: 'player@test.com', password: 'Player123!' };
const ADMIN = { email: 'admin@test.com', password: 'Admin123!' };

async function login(page: import('@playwright/test').Page, credentials: { email: string; password: string }) {
  // Tras un logout, el formulario de login vuelve a montarse vía navegación interna del
  // router (sin recarga de página) y hay una breve ventana (~150-300ms) en la que Angular
  // resincroniza la vista con un FormControl todavía vacío, borrando lo recién tecleado.
  // Se espera a que ese ciclo termine antes de completar el formulario.
  await page.waitForTimeout(500);
  await page.getByLabel('Email').fill(credentials.email);
  await page.getByLabel('Contraseña', { exact: true }).fill(credentials.password);
  await page.getByRole('button', { name: 'Ingresar' }).click();
}

async function logout(page: import('@playwright/test').Page) {
  await page.getByRole('button', { name: 'Cerrar sesión' }).click();
  await expect(page).toHaveURL(/\/auth\/login$/);
}

async function placeBet(row: Locator, localGoals: number, visitorGoals: number) {
  const inputs = row.locator('input[type="number"]');
  await inputs.nth(0).fill(String(localGoals));
  await inputs.nth(1).fill(String(visitorGoals));
  await row.getByRole('button', { name: 'Apostar' }).click();
  await expect(row).toContainText('Tu apuesta');
}

async function setResult(row: Locator, localGoals: number, visitorGoals: number) {
  const inputs = row.locator('input[type="number"]');
  await inputs.nth(0).fill(String(localGoals));
  await inputs.nth(1).fill(String(visitorGoals));
  await row.getByRole('button', { name: 'Guardar' }).click();
  await expect(row).toContainText('Finalizado');
}

test.describe('Ciclo completo de apuestas y puntaje', () => {
  test('el jugador acumula 4 puntos (0 + 3 + 1) luego de que el admin carga los resultados', async ({ page }) => {
    const backend = new FakeBackend();
    await backend.install(page);

    await test.step('Player inicia sesión y aposta en los 3 partidos', async () => {
      await page.goto('/auth/login');
      await login(page, PLAYER);
      await expect(page).toHaveURL(/\/player\/leaderboard$/);

      await page.getByRole('link', { name: 'Partidos' }).click();
      await expect(page).toHaveURL(/\/player\/matches$/);

      const matchRows = page.locator('app-match-bet-row');
      await expect(matchRows).toHaveCount(3);

      // Partido 1: marcador y ganador incorrectos (predicción 2-0, resultado real será 0-2) -> 0 puntos
      await placeBet(matchRows.nth(0), 2, 0);
      // Partido 2: marcador exacto (predicción 1-0, resultado real será 1-0) -> 3 puntos
      await placeBet(matchRows.nth(1), 1, 0);
      // Partido 3: ganador correcto, marcador incorrecto (predicción 2-0, resultado real será 3-1) -> 1 punto
      await placeBet(matchRows.nth(2), 2, 0);
    });

    await test.step('Admin carga los resultados reales de los 3 partidos', async () => {
      await logout(page);
      await login(page, ADMIN);
      await expect(page).toHaveURL(/\/admin\/matches$/);

      const adminRows = page.locator('app-match-row');
      await expect(adminRows).toHaveCount(3);

      await setResult(adminRows.nth(0), 0, 2);
      await setResult(adminRows.nth(1), 1, 0);
      await setResult(adminRows.nth(2), 3, 1);
    });

    await test.step('Player vuelve a entrar y ve 4 puntos totales con el detalle correcto', async () => {
      await logout(page);
      await login(page, PLAYER);
      await expect(page).toHaveURL(/\/player\/leaderboard$/);

      const ownRow = page.locator('app-leaderboard-table tbody tr', { hasText: 'Jugador Uno' });
      await expect(ownRow).toBeVisible();
      // Columnas: Posición, Nombre, Puntos, Apuestas
      await expect(ownRow.locator('td').nth(2)).toHaveText('4');
      await expect(ownRow.locator('td').nth(3)).toHaveText('3');

      await ownRow.click();

      const historyPanel = page.locator('app-user-history-panel');
      await expect(historyPanel.getByText('Puntos totales: 4')).toBeVisible();

      const historyRows = historyPanel.locator('tbody tr');
      await expect(historyRows).toHaveCount(3);
      // Puntos: columna 4 (índice 3) de cada fila del historial
      await expect(historyRows.nth(0).locator('td').nth(3)).toHaveText('0');
      await expect(historyRows.nth(1).locator('td').nth(3)).toHaveText('3');
      await expect(historyRows.nth(2).locator('td').nth(3)).toHaveText('1');
    });
  });
});
