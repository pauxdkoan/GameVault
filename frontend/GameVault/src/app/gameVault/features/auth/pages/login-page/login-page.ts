import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthService } from '../../services/auth.service';
import { getControlError } from '../../utils/form-errors';
import { AuthFormHeader } from '../../components/auth-form-header/auth-form-header';
import { FormAlert } from '../../components/form-alert/form-alert';
import { PasswordToggle } from '../../components/password-toggle/password-toggle';
import { FormField } from '../../../../shared/ui/form-field/form-field';
import { VaultButton } from '../../../../shared/ui/button/vault-button';

@Component({
  selector: 'app-login-page',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    AuthFormHeader,
    FormAlert,
    PasswordToggle,
    FormField,
    VaultButton,
  ],
  templateUrl: './login-page.html',
})
export class LoginPage {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  protected readonly isLoading = signal(false);
  protected readonly apiErrors = signal<string[]>([]);

  protected readonly loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  /** Texto de error de un campo (o null si está bien). */
  protected fieldError(field: 'email' | 'password'): string | null {
    const messages = {
      email: {
        required: 'Escribe tu email.',
        email: 'Ese email no parece válido.',
      },
      password: { required: 'Escribe tu contraseña.' },
    };

    return getControlError(this.loginForm.controls[field], messages[field]);
  }

  protected onSubmit(): void {
    // Marca todo como tocado para que se vean los errores al pulsar "Entrar"
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.apiErrors.set([]);

    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/gamevault']);
      },
      error: (messages: string[]) => {
        this.isLoading.set(false);
        this.apiErrors.set(messages);
      },
    });
  }
}
