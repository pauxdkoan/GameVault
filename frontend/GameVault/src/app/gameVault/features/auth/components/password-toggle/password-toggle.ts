import { Component, input, signal } from '@angular/core';
import { Icon } from '../../../../shared/ui/icon/icon';

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
  imports: [Icon],
  template: `
    <button
      class="absolute inset-y-0 right-0 flex w-12 items-center justify-center rounded-r-md text-vault-subtle transition hover:text-vault-text focus-visible:text-vault-accent focus-visible:outline-none cursor-pointer"
      type="button"
      [attr.aria-label]="isVisible() ? 'Ocultar contraseña' : 'Mostrar contraseña'"
      [attr.aria-pressed]="isVisible()"
      (click)="toggle()"
    >
      @if (isVisible()) {
        <app-icon name="eye-off" class="size-5" [strokeWidth]="1.8" />
      } @else {
        <app-icon name="eye" class="size-5" [strokeWidth]="1.8" />
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
