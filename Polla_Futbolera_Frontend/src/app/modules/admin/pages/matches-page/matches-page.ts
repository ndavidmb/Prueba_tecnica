import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { UpdateMatchResultRequest } from '../../../../infrastructure/api/matches/matches-api.types';
import { MatchRow } from '../../components/match-row/match-row';
import { MatchesStore } from '../../stores/matches.store';
import { AuthStore } from '../../../auth/stores/auth.store';
import { Router } from '@angular/router';

@Component({
  imports: [MatchRow],
  selector: 'app-matches-page',
  templateUrl: './matches-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MatchesPage implements OnInit {
  private readonly matchesStore = inject(MatchesStore);
  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);

  protected readonly matches = this.matchesStore.matches;
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly savingId = signal<number | null>(null);

  async ngOnInit(): Promise<void> {
    this.loading.set(true);
    this.errorMessage.set(null);

    try {
      await this.matchesStore.setup();
    } catch {
      this.errorMessage.set('No se pudieron cargar los partidos.');
    } finally {
      this.loading.set(false);
    }
  }

  protected logout(): void {
    this.authStore.logout();
    this.router.navigateByUrl('/auth/login');
  }

  protected async onSave(id: number, payload: UpdateMatchResultRequest): Promise<void> {
    this.savingId.set(id);
    this.errorMessage.set(null);

    try {
      await this.matchesStore.updateResult(id, payload);
    } catch {
      this.errorMessage.set('No se pudo actualizar el resultado del partido.');
    } finally {
      this.savingId.set(null);
    }
  }
}
