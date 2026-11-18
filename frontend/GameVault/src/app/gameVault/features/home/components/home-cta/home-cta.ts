import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionService } from '../../../../core/services/auth-session.service';
import { VaultButton } from '../../../../shared/ui/button/vault-button';

/**
 * Cierre del home para quien todavía no tiene cuenta.
 * Con la sesión iniciada no se pinta: ya no hay nada que vender.
 */
@Component({
  selector: 'app-home-cta',
  imports: [RouterLink, VaultButton],
  template: `
    @if (!session.isLoggedIn()) {
      <section class="vault-card flex flex-col gap-6 p-8 sm:p-10 lg:flex-row lg:items-center lg:justify-between">
        <div class="max-w-xl">
          <h2 class="text-4xl text-vault-text sm:text-5xl">¿Qué estás jugando ahora?</h2>
          <p class="mt-3 text-vault-muted">
            Crea una cuenta, apúntalo en tu biblioteca y deja tu nota cuando lo termines.
          </p>
        </div>
        <div class="flex flex-wrap gap-3">
          <a vaultButton size="lg" routerLink="/auth/register">Crear cuenta</a>
          <a vaultButton size="lg" variant="secondary" routerLink="/gamevault/games">Explorar juegos</a>
        </div>
      </section>
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeCta {
  protected readonly session = inject(AuthSessionService);
}
