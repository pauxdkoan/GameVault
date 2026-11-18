import { ChangeDetectionStrategy, Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionService } from '../../../../core/services/auth-session.service';
import { errorMessages } from '../../../../core/http/api-error';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { RatingStars } from '../../../../shared/ui/rating-stars/rating-stars';
import { Review } from '../../models/review.models';
import { ReviewsService } from '../../services/reviews.service';
import { ReviewCard } from '../review-card/review-card';
import { ReviewForm } from '../review-form/review-form';

const PAGE_SIZE = 10;

/**
 * Sección "Reseñas" de la ficha de un juego.
 *
 * Arriba: nota media de la comunidad y mi reseña (o el botón para escribirla).
 * Abajo: las reseñas del resto, de 10 en 10 con "Cargar más".
 * Mi reseña se quita de la lista general para no salir dos veces.
 */
@Component({
  selector: 'app-game-reviews',
  imports: [ReviewCard, ReviewForm, RatingStars, VaultButton, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  templateUrl: './game-reviews.html',
})
export class GameReviews implements OnInit {
  private readonly reviewsService = inject(ReviewsService);
  protected readonly session = inject(AuthSessionService);

  readonly externalId = input.required<number>();
  readonly gameName = input('');

  protected readonly reviews = signal<Review[]>([]);
  protected readonly myReview = signal<Review | null>(null);
  protected readonly averageRating = signal<number | null>(null);
  protected readonly reviewCount = signal(0);
  protected readonly total = signal(0);
  protected readonly page = signal(1);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);
  /** true mientras el formulario (crear o editar) está abierto. */
  protected readonly formOpen = signal(false);

  protected readonly hasMore = computed(() => this.page() * PAGE_SIZE < this.total());

  ngOnInit(): void {
    this.load(1);
  }

  protected load(page: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.reviewsService.getGameReviews(this.externalId(), page, PAGE_SIZE).subscribe({
      next: (response) => {
        const mine = response.myReview;
        const others = response.reviews.items.filter((review) => review.id !== mine?.id);

        this.reviews.set(page === 1 ? others : [...this.reviews(), ...others]);
        this.myReview.set(mine);
        this.averageRating.set(response.averageRating);
        this.reviewCount.set(response.reviewCount);
        this.total.set(response.reviews.count);
        this.page.set(page);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.error.set(errorMessages(error).join(' '));
        this.isLoading.set(false);
      },
    });
  }

  /** Al guardar recargamos la primera página: cambia la media y el número de reseñas. */
  protected onSaved(): void {
    this.formOpen.set(false);
    this.load(1);
  }

  protected onDeleted(): void {
    this.myReview.set(null);
    this.load(1);
  }
}
