import { Component, input } from '@angular/core';

/** Título y subtítulo que aparecen encima de cada formulario de auth. */
@Component({
  selector: 'app-auth-form-header',
  template: `
    <header class="flex flex-col gap-2">
      <h1 class="text-5xl text-vault-text">
        {{ title() }}
      </h1>
      <p class="text-sm leading-6 text-vault-muted sm:text-base">
        {{ subtitle() }}
      </p>
    </header>
  `,
})
export class AuthFormHeader {
  readonly title = input.required<string>();
  readonly subtitle = input.required<string>();
}
