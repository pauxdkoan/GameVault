import { Component, OnDestroy, computed, input, output, signal } from '@angular/core';
import { GameCard } from '../../models/game-card.model';
import { GameCardPreview } from '../game-card-preview/game-card-preview';
import {
  DEFAULT_GAME_CARD_PREVIEW_WIDTH,
  GameCardPreviewPosition,
  GameCardPreviewSide,
  calculateGameCardPreviewPosition,
} from '../game-card-preview/game-card-preview-position';
import { igdbRatingLabel } from '../../utils/rating';

type GameCardPreviewMode = 'internal' | 'external' | 'none';

@Component({
  selector: 'app-game-card',
  imports: [GameCardPreview],
  templateUrl: './game-card.html',
})
export class GameCardComponent implements OnDestroy {
  readonly game = input.required<GameCard>();

  readonly variant = input<'default' | 'compact'>('default');
  readonly previewMode = input<GameCardPreviewMode>('internal');
  readonly previewSide = input<GameCardPreviewSide>('auto');
  readonly previewWidth = input(DEFAULT_GAME_CARD_PREVIEW_WIDTH);

  readonly previewOpen = output<HTMLElement>();
  readonly previewClose = output<void>();

  private closePreviewTimeout: ReturnType<typeof setTimeout> | null = null;

  protected readonly hasImageError = signal(false);
  protected readonly activePreviewPosition =
    signal<GameCardPreviewPosition | null>(null);

  protected readonly isCompact = computed(() => this.variant() === 'compact');

  /** Nota de IGDB mostrada sobre 10 (ver shared/utils/rating.ts). */
  protected readonly ratingLabel = computed(() => igdbRatingLabel(this.game().rating));

  ngOnDestroy(): void {
    this.cancelClosePreview();
  }

  protected openPreview(event: Event): void {
    if (this.previewMode() === 'none') {
      return;
    }

    this.cancelClosePreview();

    const element = event.currentTarget as HTMLElement;

    if (this.previewMode() === 'external') {
      this.previewOpen.emit(element);

      return;
    }

    this.activePreviewPosition.set(
      calculateGameCardPreviewPosition(element, {
        preferredSide: this.previewSide(),
        width: this.previewWidth(),
      }),
    );
  }

  protected closePreview(): void {
    if (this.previewMode() === 'external') {
      this.previewClose.emit();

      return;
    }

    this.cancelClosePreview();

    this.closePreviewTimeout = setTimeout(() => {
      this.activePreviewPosition.set(null);
      this.closePreviewTimeout = null;
    }, 150);
  }

  protected cancelClosePreview(): void {
    if (!this.closePreviewTimeout) {
      return;
    }

    clearTimeout(this.closePreviewTimeout);
    this.closePreviewTimeout = null;
  }

  protected markImageAsFailed(): void {
    this.hasImageError.set(true);
  }
}
