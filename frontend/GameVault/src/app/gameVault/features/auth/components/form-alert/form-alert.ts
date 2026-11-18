import { Component, input } from '@angular/core';

/**
 * Caja de error que muestra los mensajes devueltos por la API.
 * Si la lista está vacía no se pinta nada.
 */
@Component({
  selector: 'app-form-alert',
  template: `
    @if (messages().length > 0) {
      <div
        class="flex gap-3 rounded-md border border-vault-danger/30 bg-vault-danger/10 p-4 text-sm text-vault-text"
        role="alert"
      >
        <span
          class="mt-0.5 grid size-5 shrink-0 place-items-center rounded-full bg-vault-danger/20 text-xs font-bold text-vault-danger"
          aria-hidden="true"
          >!</span
        >
        <ul class="flex flex-col gap-1">
          @for (message of messages(); track $index) {
            <li>{{ message }}</li>
          }
        </ul>
      </div>
    }
  `,
})
export class FormAlert {
  readonly messages = input<string[]>([]);
}
