import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HomeHeroSlide } from '../../models/home-content.model';
import { HomeHeroCarouselComponent } from '../hero-carousel/hero-carousel';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { AuthSessionService } from '../../../../core/services/auth-session.service';

/**
 * Portada del home: mensaje principal a la izquierda y
 * carrusel de "Juego del momento" (con la portada en grande) a la derecha.
 * El segundo botón cambia según haya sesión o no.
 */
@Component({
  selector: 'app-hero-section',
  imports: [RouterLink, HomeHeroCarouselComponent, VaultButton],
  templateUrl: './hero-section.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeroSectionComponent {
  protected readonly session = inject(AuthSessionService);

  readonly slides = input.required<HomeHeroSlide[]>();
  readonly loading = input(false);

  /** Primera imagen disponible para usarla de fondo (oscurecida). */
  protected backgroundImage(): string | null {
    return this.slides().find((slide) => slide.backgroundImageUrl)?.backgroundImageUrl ?? null;
  }
}
