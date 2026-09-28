import { ChangeDetectionStrategy, Component, input, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-password-input',
  templateUrl: './password-input.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PasswordInput {
  readonly inputId = input.required<string>();
  readonly label = input.required<string>();
  readonly control = input.required<FormControl<string>>();
  readonly placeholder = input<string>('');
  readonly autocomplete = input<string>('current-password');
  readonly errorMessage = input<string | null>(null);

  protected readonly visible = signal(false);

  protected toggleVisibility(): void {
    this.visible.update((value) => !value);
  }
}
