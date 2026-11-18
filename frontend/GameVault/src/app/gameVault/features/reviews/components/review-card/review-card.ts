import { ChangeDetectionStrategy, Component, inject, input, linkedSignal, output, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthSessionService } from '../../../../core/services/auth-session.service';
import { errorMessages } from '../../../../core/http/api-error';
import { RatingStars } from '../../../../shared/ui/rating-stars/rating-stars';
import { UserAvatar } from '../../../../shared/ui/user-avatar/user-avatar';
import { formatRelativeDate } from '../../../../shared/utils/date';
import { Review } from '../../models/review.models';
import { ReviewsService } from '../../services/reviews.service';
import { ReviewComments } from '../review-comments/review-comments';
import { Icon } from '../../../../shared/ui/icon/icon';

/**
 * Una reseña: autor, nota, texto (oculto si tiene spoilers), me gusta y comentarios.
 * `showGame` añade la portada y el nombre del juego (para el perfil).
 */
@Component({
  selector: 'app-review-card',
  imports: [Icon, RatingStars, UserAvatar, ReviewComments, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  templateUrl: './review-card.html',
})
export class ReviewCard {
  private readonly reviewsService = inject(ReviewsService);
  private readonly session = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly review = input.required<Review>();
  readonly showGame = input(false);
  readonly edit = output<void>();
  readonly deleted = output<void>();

  /** Copia local para actualizar me gusta y comentarios sin recargar todo. */
  protected readonly current = linkedSignal(() => this.review());
  /** La miniatura del juego no cargó: se pinta su inicial. */
  protected readonly coverFailed = signal(false);
  protected readonly spoilerVisible = signal(false);
  protected readonly commentsOpen = signal(false);
  protected readonly confirmDelete = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly timeAgo = formatRelativeDate;

  protected toggleLike(): void {
    const review = this.current();

    if (!this.session.isLoggedIn()) {
      this.router.navigate(['/auth/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    const request$ = review.likedByMe
      ? this.reviewsService.unlike(review.id)
      : this.reviewsService.like(review.id);

    request$.subscribe({
      next: (likeCount) =>
        this.current.set({ ...this.current(), likeCount, likedByMe: !review.likedByMe }),
      error: (error: unknown) => this.error.set(errorMessages(error).join(' ')),
    });
  }

  protected updateCommentCount(commentCount: number): void {
    this.current.set({ ...this.current(), commentCount });
  }

  protected remove(): void {
    this.reviewsService.delete(this.current().id).subscribe({
      next: () => this.deleted.emit(),
      error: (error: unknown) => this.error.set(errorMessages(error).join(' ')),
    });
  }
}
