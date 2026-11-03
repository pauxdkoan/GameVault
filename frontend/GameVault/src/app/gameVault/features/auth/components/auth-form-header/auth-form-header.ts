import { Component, input } from '@angular/core';

/** Título y subtítulo que aparecen encima de cada formulario de auth. */
@Component({
  selector: 'app-auth-form-header',
  template: `
    <header class="flex flex-col gap-2">
      <span
        class="inline-flex w-fit items-center gap-2 rounded-full border border-vault-cyan/25 bg-vault-cyan/10 px-3 py-1 text-xs font-semibold text-vault-cyan-soft"
      >
        <span class="size-1.5 rounded-full bg-vault-cyan" aria-hidden="true"></span>
        {{ eyebrow() }}
      </span>
      <h1 class="mt-2 font-display text-3xl font-bold text-vault-text sm:text-4xl">
        {{ title() }}
      </h1>
      <p class="text-sm leading-6 text-vault-muted sm:text-base">
        {{ subtitle() }}
      </p>
    </header>
  `,
})
export class AuthFormHeader {
  readonly eyebrow = input.required<string>();
  readonly title = input.required<string>();
  readonly subtitle = input.required<string>();
}
