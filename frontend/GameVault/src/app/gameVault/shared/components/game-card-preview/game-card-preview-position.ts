export type GameCardPreviewSide = 'auto' | 'left' | 'right';

export interface GameCardPreviewPosition {
  top: number;
  left: number;
}

export interface GameCardPreviewPositionOptions {
  width?: number;
  estimatedHeight?: number;
  gap?: number;
  viewportPadding?: number;
  preferredSide?: GameCardPreviewSide;
}

export const DEFAULT_GAME_CARD_PREVIEW_WIDTH = 320;
export const DEFAULT_GAME_CARD_PREVIEW_ESTIMATED_HEIGHT = 420;
export const DEFAULT_GAME_CARD_PREVIEW_GAP = 8;
export const DEFAULT_GAME_CARD_PREVIEW_VIEWPORT_PADDING = 16;

export function calculateGameCardPreviewPosition(
  origin: HTMLElement,
  options: GameCardPreviewPositionOptions = {},
): GameCardPreviewPosition {
  const width = options.width ?? DEFAULT_GAME_CARD_PREVIEW_WIDTH;
  const estimatedHeight =
    options.estimatedHeight ?? DEFAULT_GAME_CARD_PREVIEW_ESTIMATED_HEIGHT;
  const gap = options.gap ?? DEFAULT_GAME_CARD_PREVIEW_GAP;
  const viewportPadding =
    options.viewportPadding ?? DEFAULT_GAME_CARD_PREVIEW_VIEWPORT_PADDING;
  const preferredSide = options.preferredSide ?? 'auto';
  const rect = origin.getBoundingClientRect();
  const viewportWidth = window.innerWidth;
  const viewportHeight = window.innerHeight;

  const rightLeft = rect.right + gap;
  const leftLeft = rect.left - width - gap;
  const rightFits = rightLeft + width <= viewportWidth - viewportPadding;
  const leftFits = leftLeft >= viewportPadding;

  let left =
    preferredSide === 'left'
      ? leftLeft
      : preferredSide === 'right'
        ? rightLeft
        : rightFits || !leftFits
          ? rightLeft
          : leftLeft;

  if (preferredSide === 'left' && !leftFits && rightFits) {
    left = rightLeft;
  }

  if (preferredSide === 'right' && !rightFits && leftFits) {
    left = leftLeft;
  }

  const maxLeft = Math.max(
    viewportPadding,
    viewportWidth - width - viewportPadding,
  );
  const maxTop = Math.max(
    viewportPadding,
    viewportHeight - estimatedHeight - viewportPadding,
  );

  return {
    top: clamp(rect.top, viewportPadding, maxTop),
    left: clamp(left, viewportPadding, maxLeft),
  };
}

function clamp(value: number, min: number, max: number): number {
  return Math.min(Math.max(value, min), max);
}
