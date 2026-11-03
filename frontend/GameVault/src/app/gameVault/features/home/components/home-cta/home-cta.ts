import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VaultButton } from '../../../../shared/ui/button/vault-button';

/** Llamada final a la acción: crear cuenta o explorar juegos. */
@Component({
  selector: 'app-home-cta',
  imports: [RouterLink, VaultButton],
  template: `
    <section class="relative overflow-hidden rounded-3xl border border-vault-cyan/20 bg-gradient-to-br from-vault-cyan/15 via-vault-surface to-vault-violet/15 px-6 py-16 text-center sm:px-12">
      <div class="absolute -left-20 -top-20 size-72 rounded-full bg-vault-cyan/20 blur-3xl" aria-hidden="true"></div>
      <div class="absolute -bottom-24 -right-16 size-72 rounded-full bg-vault-violet/20 blur-3xl" aria-hidden="true"></div>

      <div class="relative mx-auto max-w-2xl">
        <h2 class="font-display text-3xl font-bold text-vault-text sm:text-4xl">
          Empieza a construir tu historia como jugador
        </h2>
        <p class="mt-4 text-vault-muted">
          Crea tu cuenta gratis, organiza tu colección y encuentra tu próximo videojuego favorito.
        </p>
        <div class="mt-8 flex flex-wrap justify-center gap-3">
          <a vaultButton size="lg" routerLink="/auth/register">Crear cuenta gratis</a>
          <a vaultButton size="lg" variant="secondary" routerLink="/gamevault/games">Explorar videojuegos</a>
        </div>
        <p class="mt-5 text-xs text-vault-subtle">No necesitas tarjeta de crédito.</p>
      </div>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeCta {}
