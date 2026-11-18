import {
  ChangeDetectionStrategy,
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  input,
  signal,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { HomeHeroSlide } from '../../models/home-content.model';
import { ensureSwiperRegistered } from '../../../../shared/utils/swiper';
import { igdbRatingLabel } from '../../../../shared/utils/rating';

@Component({
  selector: 'app-home-hero-carousel',
  imports: [RouterLink],
  templateUrl: './hero-carousel.html',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeHeroCarouselComponent {
  constructor() {
    ensureSwiperRegistered();
  }

  readonly slides = input.required<HomeHeroSlide[]>();
  readonly loading = input(false);

  protected readonly failedImages = signal<ReadonlySet<string>>(new Set());

  protected markImageAsFailed(slideId: string): void {
    this.failedImages.update((failedImages) => {
      const nextFailedImages = new Set(failedImages);
      nextFailedImages.add(slideId);
      return nextFailedImages;
    });
  }

  /** Nota de IGDB mostrada sobre 10. */
  protected ratingLabel(rating: number | null): string {
    return igdbRatingLabel(rating);
  }

  protected hasImageFailed(slideId: string): boolean {
    return this.failedImages().has(slideId);
  }
}
