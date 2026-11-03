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
      class="inline-flex items-center gap-2.5 rounded-lg outline-none focus-visible:ring-2 focus-visible:ring-vault-cyan"
      routerLink="/gamevault/home"
      aria-label="GameVault, ir al inicio"
    >
      <span
        class="grid size-9 place-items-center rounded-xl bg-vault-gradient font-display text-sm font-bold text-vault-bg shadow-vault-glow"
        aria-hidden="true"
      >
        GV
      </span>
      @if (showName()) {
        <span class="font-display text-lg font-bold text-vault-text">
          Game<span class="text-vault-gradient">Vault</span>
        </span>
      }
    </a>
  `,
})
export class BrandLogo {
  readonly showName = input(true);
}
