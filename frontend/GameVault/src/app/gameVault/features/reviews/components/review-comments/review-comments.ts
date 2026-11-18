import { ChangeDetectionStrategy, Component, inject, input, OnInit, output, signal } from '@angular/core';
import { AuthSessionService } from '../../../../core/services/auth-session.service';
import { errorMessages } from '../../../../core/http/api-error';
import { UserAvatar } from '../../../../shared/ui/user-avatar/user-avatar';
import { formatRelativeDate } from '../../../../shared/utils/date';
import { ReviewComment } from '../../models/review.models';
import { ReviewsService } from '../../services/reviews.service';

const MAX_COMMENT = 1000;

/** Comentarios de una reseña. Se crea al abrir los comentarios, así que carga en ngOnInit. */
@Component({
  selector: 'app-review-comments',
  imports: [UserAvatar],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  templateUrl: './review-comments.html',
})
export class ReviewComments implements OnInit {
  private readonly reviewsService = inject(ReviewsService);
  protected readonly session = inject(AuthSessionService);

  readonly reviewId = input.required<string>();
  /** Avisa al padre de cuántos comentarios hay ahora, para actualizar el contador. */
  readonly countChange = output<number>();

  protected readonly comments = signal<ReviewComment[] | null>(null);
  protected readonly draft = signal('');
  protected readonly isSending = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly maxComment = MAX_COMMENT;
  protected readonly timeAgo = formatRelativeDate;

  ngOnInit(): void {
    this.reviewsService.getComments(this.reviewId()).subscribe({
      next: (page) => this.comments.set(page.items),
      error: (error: unknown) => {
        this.comments.set([]);
        this.error.set(errorMessages(error).join(' '));
      },
    });
  }

  protected setDraft(event: Event): void {
    this.draft.set((event.target as HTMLTextAreaElement).value);
  }

  protected send(event: Event): void {
    event.preventDefault();
    const content = this.draft().trim();

    if (!content || content.length > MAX_COMMENT) {
      return;
    }

    this.isSending.set(true);
    this.error.set(null);

    this.reviewsService.addComment(this.reviewId(), content).subscribe({
      next: (comment) => {
        this.comments.update((list) => [...(list ?? []), comment]);
        this.countChange.emit(this.comments()!.length);
        this.draft.set('');
        this.isSending.set(false);
      },
      error: (error: unknown) => {
        this.isSending.set(false);
        this.error.set(errorMessages(error).join(' '));
      },
    });
  }

  protected remove(comment: ReviewComment): void {
    this.reviewsService.deleteComment(comment.id).subscribe({
      next: () => {
        this.comments.update((list) => (list ?? []).filter((item) => item.id !== comment.id));
        this.countChange.emit(this.comments()!.length);
      },
      error: (error: unknown) => this.error.set(errorMessages(error).join(' ')),
    });
  }
}
