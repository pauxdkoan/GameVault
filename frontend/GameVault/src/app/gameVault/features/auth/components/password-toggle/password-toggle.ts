import { Component, input, signal } from '@angular/core';

/**
 * Botón "ojo" para mostrar u ocultar una contraseña.
 *
 * Recibe el <input> mediante una referencia de plantilla y solo
 * cambia su atributo type. No toca el valor ni el formulario.
 *
 * Uso:
 *   <input #pwd type="password" class="vault-input pr-12" ... />
 *   <app-password-toggle [target]="pwd" />
 */
@Component({
  selector: 'app-password-toggle',
  template: `
    <button
      class="absolute inset-y-0 right-0 flex w-12 items-center justify-center rounded-r-xl text-vault-subtle transition hover:text-vault-text focus-visible:text-vault-cyan focus-visible:outline-none cursor-pointer"
      type="button"
      [attr.aria-label]="isVisible() ? 'Ocultar contraseña' : 'Mostrar contraseña'"
      [attr.aria-pressed]="isVisible()"
      (click)="toggle()"
    >
      @if (isVisible()) {
        <!-- ojo tachado -->
        <svg class="size-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="M3 3l18 18" />
          <path d="M10.6 10.6a2 2 0 0 0 2.8 2.8" />
          <path d="M9.9 4.2A10.4 10.4 0 0 1 12 4c5 0 9 4.5 10 8-.4 1.3-1.2 2.7-2.3 4M6.6 6.6C4.5 8 2.9 10 2 12c1 3.5 5 8 10 8 1.7 0 3.3-.5 4.7-1.3" />
        </svg>
      } @else {
        <!-- ojo -->
        <svg class="size-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="M2 12c1-3.5 5-8 10-8s9 4.5 10 8c-1 3.5-5 8-10 8S3 15.5 2 12Z" />
          <circle cx="12" cy="12" r="3" />
        </svg>
      }
    </button>
  `,
})
export class PasswordToggle {
  readonly target = input.required<HTMLInputElement>();

  protected readonly isVisible = signal(false);

  protected toggle(): void {
    this.isVisible.update((visible) => !visible);
    this.target().type = this.isVisible() ? 'text' : 'password';
  }
}
