import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';

type StarFill = 'full' | 'half' | 'empty';

/**
 * Nota de 1 a 10 pintada como 5 estrellas con medias (7 = 3 estrellas y media).
 *
 * - Solo lectura:  <app-rating-stars [value]="review.rating" />
 * - Editable:      <app-rating-stars [value]="rating()" editable (valueChange)="rating.set($event)" />
 *
 * En modo editable cada estrella tiene dos mitades clicables: la izquierda vale
 * un número impar (1, 3, 5...) y la derecha uno par (2, 4, 6...).
 */
@Component({
  selector: 'app-rating-stars',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div
      class="inline-flex items-center gap-0.5"
      [attr.role]="editable() ? 'radiogroup' : 'img'"
      [attr.aria-label]="ariaLabel()"
      (mouseleave)="hover.set(null)"
    >
      @for (fill of stars(); track $index; let i = $index) {
        <span class="relative inline-block" [style.width.px]="size()" [style.height.px]="size()">
          <svg class="absolute inset-0 text-vault-border" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
            <path [attr.d]="starPath" />
          </svg>
          @if (fill !== 'empty') {
            <svg
              class="absolute inset-0 text-vault-warning"
              viewBox="0 0 24 24"
              fill="currentColor"
              aria-hidden="true"
              [style.clip-path]="fill === 'half' ? 'inset(0 50% 0 0)' : null"
            >
              <path [attr.d]="starPath" />
            </svg>
          }
          @if (editable()) {
            <button
              class="absolute inset-y-0 left-0 w-1/2 cursor-pointer"
              type="button"
              role="radio"
              [attr.aria-checked]="value() === i * 2 + 1"
              [attr.aria-label]="i * 2 + 1 + ' de 10'"
              (mouseenter)="hover.set(i * 2 + 1)"
              (click)="valueChange.emit(i * 2 + 1)"
            ></button>
            <button
              class="absolute inset-y-0 right-0 w-1/2 cursor-pointer"
              type="button"
              role="radio"
              [attr.aria-checked]="value() === i * 2 + 2"
              [attr.aria-label]="i * 2 + 2 + ' de 10'"
              (mouseenter)="hover.set(i * 2 + 2)"
              (click)="valueChange.emit(i * 2 + 2)"
            ></button>
          }
        </span>
      }
    </div>
  `,
})
export class RatingStars {
  /** Nota de 1 a 10 (o null si no hay). */
  readonly value = input<number | null>(null);
  readonly editable = input(false);
  readonly size = input(16);
  readonly valueChange = output<number>();

  protected readonly hover = signal<number | null>(null);

  protected readonly starPath =
    'M12 2.5l2.9 6 6.6.9-4.8 4.6 1.2 6.5L12 17.4l-5.9 3.1 1.2-6.5L2.5 9.4l6.6-.9L12 2.5z';

  protected readonly stars = computed<StarFill[]>(() => {
    const shown = this.hover() ?? this.value() ?? 0;

    return [1, 2, 3, 4, 5].map((star) => {
      const points = shown - (star - 1) * 2;
      return points >= 2 ? 'full' : points === 1 ? 'half' : 'empty';
    });
  });

  protected readonly ariaLabel = computed(() =>
    this.value() ? `Nota: ${this.value()} de 10` : 'Sin nota',
  );
}
