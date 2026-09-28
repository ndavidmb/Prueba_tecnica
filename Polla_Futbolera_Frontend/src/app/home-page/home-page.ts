import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthStore } from '../modules/auth/stores/auth.store';

@Component({
  imports: [RouterLink],
  selector: 'app-home-page',
  templateUrl: './home-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage {
  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);

  protected readonly user = this.authStore.user;

  protected logout(): void {
    this.authStore.logout();
    this.router.navigateByUrl('/auth/login');
  }
}
