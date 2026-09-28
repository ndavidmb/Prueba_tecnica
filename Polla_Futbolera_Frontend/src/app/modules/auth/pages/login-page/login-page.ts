import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PasswordInput } from '../../components/password-input/password-input';
import { AuthStore } from '../../stores/auth.store';

interface LoginForm {
  email: FormControl<string>;
  password: FormControl<string>;
}

@Component({
  imports: [ReactiveFormsModule, RouterLink, PasswordInput],
  selector: 'app-login-page',
  templateUrl: './login-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPage {
  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);

  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly form = new FormGroup<LoginForm>({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  protected async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    try {
      const { email, password } = this.form.getRawValue();
      await this.authStore.login({ email, password });
      if (this.authStore.role() === 'Admin') {
        await this.router.navigateByUrl('/admin/matches');
      } else {
        await this.router.navigateByUrl('/player');
      }
    } catch {
      this.errorMessage.set('Credenciales inválidas.');
    } finally {
      this.loading.set(false);
    }
  }
}
