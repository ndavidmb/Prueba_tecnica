import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthStore } from '../../../auth/stores/auth.store';

@Component({
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  selector: 'app-player-shell',
  templateUrl: './player-shell.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlayerShell {
  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);

  protected readonly user = this.authStore.user;

  protected logout(): void {
    this.authStore.logout();
    this.router.navigateByUrl('/auth/login');
  }
}
