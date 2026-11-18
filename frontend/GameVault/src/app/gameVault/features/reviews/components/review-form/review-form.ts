import { ChangeDetectionStrategy, Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { errorMessages } from '../../../../core/http/api-error';
import { RatingStars } from '../../../../shared/ui/rating-stars/rating-stars';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { Review } from '../../models/review.models';
import { ReviewsService } from '../../services/reviews.service';

const MAX_TITLE = 120;
const MAX_CONTENT = 5000;

/**
 * Formulario para escribir o editar una reseña.
 * Con `review` edita esa reseña; sin ella crea una nueva para `externalId`.
 * Las mismas reglas que el backend (nota 1-10, texto obligatorio, límites)
 * se comprueban aquí para avisar antes de enviar.
 */
@Component({
  selector: 'app-review-form',
  imports: [RatingStars, VaultButton],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  templateUrl: './review-form.html',
})
export class ReviewForm implements OnInit {
  private readonly reviewsService = inject(ReviewsService);

  readonly externalId = input.required<number>();
  readonly review = input<Review | null>(null);
  readonly saved = output<Review>();
  readonly cancelled = output<void>();

  protected readonly title = signal('');
  protected readonly content = signal('');
  protected readonly rating = signal<number | null>(null);
  protected readonly containsSpoilers = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly errors = signal<string[]>([]);
  protected readonly submitted = signal(false);

  protected readonly maxTitle = MAX_TITLE;
  protected readonly maxContent = MAX_CONTENT;

  protected readonly validationError = computed(() => {
    if (!this.rating()) return 'Elige una nota.';
    if (!this.content().trim()) return 'Escribe tu reseña.';
    if (this.content().length > MAX_CONTENT) return `La reseña no puede superar los ${MAX_CONTENT} caracteres.`;
    if (this.title().length > MAX_TITLE) return `El título no puede superar los ${MAX_TITLE} caracteres.`;
    return null;
  });

  ngOnInit(): void {
    const review = this.review();

    if (review) {
      this.title.set(review.title);
      this.content.set(review.content);
      this.rating.set(review.rating);
      this.containsSpoilers.set(review.containsSpoilers);
    }
  }

  protected textValue(event: Event): string {
    return (event.target as HTMLInputElement | HTMLTextAreaElement).value;
  }

  protected submit(event: Event): void {
    event.preventDefault();
    this.submitted.set(true);

    if (this.validationError()) {
      return;
    }

    const request = {
      title: this.title().trim(),
      content: this.content().trim(),
      rating: this.rating()!,
      containsSpoilers: this.containsSpoilers(),
    };
    const review = this.review();
    const save$ = review
      ? this.reviewsService.update(review.id, request)
      : this.reviewsService.create(this.externalId(), request);

    this.isSaving.set(true);
    this.errors.set([]);

    save$.subscribe({
      next: (savedReview) => {
        this.isSaving.set(false);
        this.saved.emit(savedReview);
      },
      error: (error: unknown) => {
        this.isSaving.set(false);
        this.errors.set(errorMessages(error));
      },
    });
  }
}
