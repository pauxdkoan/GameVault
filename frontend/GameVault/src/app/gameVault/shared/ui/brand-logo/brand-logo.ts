import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

/**
 * Logo de GameVault (marca SVG + nombre). Se usa en el header, el footer y las
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
      <!-- Marca: la puerta de una cámara acorazada (bisagras + puerta redonda)
           cuya rueda de apertura es una cruceta de mando. "Vault" + "Game". -->
      <svg class="size-9 shrink-0" viewBox="0 0 32 32" aria-hidden="true">
        <rect width="32" height="32" rx="7" class="fill-vault-accent" />
        <rect x="2.5" y="8.5" width="3" height="5" rx="1" class="fill-vault-bg" />
        <rect x="2.5" y="18.5" width="3" height="5" rx="1" class="fill-vault-bg" />
        <circle cx="17.5" cy="16" r="10.5" class="fill-vault-bg" />
        <path
          d="M15.4 9.2h4.2v4.7h4.7v4.2h-4.7v4.7h-4.2v-4.7h-4.7v-4.2h4.7z"
          class="fill-vault-accent stroke-vault-accent"
          stroke-width="0.8"
          stroke-linejoin="round"
        />
        <circle cx="17.5" cy="16" r="1.5" class="fill-vault-bg" />
      </svg>
      @if (showName()) {
        <span class="font-display text-xl font-extrabold uppercase tracking-wide text-vault-text">GameVault</span>
      }
    </a>
  `,
})
export class BrandLogo {
  readonly showName = input(true);
}
