import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

/**
 * Logo de GameVault. Se usa en el header, el footer y las
 * pantallas de autenticación para que siempre sea idéntico.
 */
@Component({
  selector: 'app-brand-logo',
  imports: [RouterLink],
  template: `
    <a
      class="inline-flex items-center gap-2.5 rounded-md outline-none focus-visible:ring-2 focus-visible:ring-vault-accent"
      routerLink="/gamevault/home"
      aria-label="GameVault, ir al inicio"
    >
      <span
        class="grid size-9 place-items-center rounded-md bg-vault-accent font-display text-base font-black text-vault-bg"
        aria-hidden="true"
      >
        GV
      </span>
      @if (showName()) {
        <span class="font-display text-xl font-extrabold uppercase tracking-wide text-vault-text">GameVault</span>
      }
    </a>
  `,
})
export class BrandLogo {
  readonly showName = input(true);
}
