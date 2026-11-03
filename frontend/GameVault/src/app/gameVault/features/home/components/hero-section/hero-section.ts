import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HomeHeroSlide } from '../../models/home-content.model';
import { HomeHeroCarouselComponent } from '../hero-carousel/hero-carousel';
import { VaultButton } from '../../../../shared/ui/button/vault-button';

/**
 * Portada del home: mensaje principal a la izquierda y
 * carrusel de "Juego del momento" a la derecha.
 */
@Component({
  selector: 'app-hero-section',
  imports: [RouterLink, HomeHeroCarouselComponent, VaultButton],
  templateUrl: './hero-section.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeroSectionComponent {
  readonly slides = input.required<HomeHeroSlide[]>();
  readonly loading = input(false);

  /** Primera imagen disponible para usarla de fondo difuminado. */
  protected backgroundImage(): string | null {
    return this.slides().find((slide) => slide.backgroundImageUrl)?.backgroundImageUrl ?? null;
  }

  protected readonly highlights = [
    'Miles de juegos por descubrir',
    'Biblioteca personalizada',
    'Reseñas y listas de la comunidad',
  ];
}
