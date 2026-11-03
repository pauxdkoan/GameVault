import { Component, input } from '@angular/core';

/**
 * Envoltorio de un campo de formulario: etiqueta + input + mensaje.
 *
 * El <input> se pasa como contenido (ng-content), por eso sigue
 * funcionando con formControlName sin lógica extra aquí.
 * Este componente solo se encarga de la presentación.
 *
 * Uso:
 *   <app-form-field label="Email" inputId="email" [error]="emailError()">
 *     <input id="email" class="vault-input" formControlName="email" />
 *   </app-form-field>
 */
@Component({
  selector: 'app-form-field',
  template: `
    <div class="flex flex-col gap-2">
      <div class="flex items-center justify-between gap-3">
        <label
          class="text-sm font-medium text-vault-text"
          [attr.for]="inputId()"
        >
          {{ label() }}
        </label>
        <ng-content select="[fieldAction]" />
      </div>

      <div class="relative">
        <ng-content />
      </div>

      @if (error()) {
        <p class="text-xs font-medium text-vault-danger" role="alert">
          {{ error() }}
        </p>
      } @else if (hint()) {
        <p class="text-xs text-vault-subtle">{{ hint() }}</p>
      }
    </div>
  `,
})
export class FormField {
  readonly label = input.required<string>();
  readonly inputId = input.required<string>();
  readonly error = input<string | null>(null);
  readonly hint = input<string | null>(null);
}
