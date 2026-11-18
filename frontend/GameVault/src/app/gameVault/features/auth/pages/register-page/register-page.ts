import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';

import { AuthService } from '../../services/auth.service';
import { ErrorMessages, getControlError } from '../../utils/form-errors';
import { AuthFormHeader } from '../../components/auth-form-header/auth-form-header';
import { FormAlert } from '../../components/form-alert/form-alert';
import { PasswordToggle } from '../../components/password-toggle/password-toggle';
import { FormField } from '../../../../shared/ui/form-field/form-field';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { errorMessages } from '../../../../core/http/api-error';

type RegisterField =
  | 'name'
  | 'userName'
  | 'email'
  | 'password'
  | 'confirmPassword';

/** Mensajes de error de cada campo del registro. */
const REGISTER_MESSAGES: Record<RegisterField, ErrorMessages> = {
  name: { required: 'Escribe tu nombre.' },
  userName: {
    required: 'Elige un nombre de usuario.',
    minlength: 'Debe tener al menos 4 caracteres.',
  },
  email: {
    required: 'Escribe tu email.',
    email: 'Ese email no parece válido.',
  },
  password: {
    required: 'Crea una contraseña.',
    minlength: 'Debe tener al menos 8 caracteres.',
  },
  confirmPassword: {
    required: 'Repite la contraseña.',
    passwordMismatch: 'Las contraseñas no coinciden.',
  },
};

/** Valida que "contraseña" y "confirmar contraseña" sean iguales. */
function matchPasswords(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password');
  const confirm = group.get('confirmPassword');

  if (!password || !confirm || !confirm.value) {
    return null;
  }

  if (password.value !== confirm.value) {
    confirm.setErrors({ ...confirm.errors, passwordMismatch: true });
    return { passwordMismatch: true };
  }

  if (confirm.hasError('passwordMismatch')) {
    const { passwordMismatch, ...rest } = confirm.errors ?? {};
    confirm.setErrors(Object.keys(rest).length ? rest : null);
  }

  return null;
}

@Component({
  selector: 'app-register-page',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    AuthFormHeader,
    FormAlert,
    PasswordToggle,
    FormField,
    VaultButton,
  ],
  templateUrl: './register-page.html',
})
export class RegisterPage {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  protected readonly isLoading = signal(false);
  protected readonly apiErrors = signal<string[]>([]);

  protected readonly registerForm = this.fb.nonNullable.group(
    {
      name: ['', [Validators.required]],
      userName: ['', [Validators.required, Validators.minLength(4)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: [matchPasswords] },
  );

  protected fieldError(field: RegisterField): string | null {
    return getControlError(
      this.registerForm.controls[field],
      REGISTER_MESSAGES[field],
    );
  }

  protected onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.apiErrors.set([]);

    this.authService.register(this.registerForm.getRawValue()).subscribe({
      next: () => {
        this.isLoading.set(false);
        // Tras registrarse, lo mandamos a iniciar sesión
        this.router.navigate(['/auth/login']);
      },
      error: (error: unknown) => {
        this.isLoading.set(false);
        this.apiErrors.set(errorMessages(error));
      },
    });
  }
}
