import { Component, computed, CUSTOM_ELEMENTS_SCHEMA, inject, input, output, signal } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { RouterLink } from '@angular/router';
import { PlatformBadge } from '../../ui/platform-badge/platform-badge';
import { GameCard } from '../../models/game-card.model';
import { ensureSwiperRegistered } from '../../utils/swiper';
import { igdbRatingLabel } from '../../utils/rating';


/** Cuántos logotipos de plataforma caben en la tarjeta; el resto se resume en "+N". */
const PLATFORM_ICON_LIMIT = 3;

@Component({
  selector: 'app-game-card-preview',
  imports: [PlatformBadge, RouterLink],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './game-card-preview.html',
})
export class GameCardPreview {
  constructor() {
    ensureSwiperRegistered();
  }

  private readonly sanitizer = inject(DomSanitizer);

  readonly game = input.required<GameCard>();

  readonly variant = input<'default' | 'compact'>('default');

  protected readonly previewImageFailed = signal(false);

  protected readonly isCompact = computed(() => this.variant() === 'compact');

  /** Nota de IGDB mostrada sobre 10 (ver shared/utils/rating.ts). */
  protected readonly ratingLabel = computed(() => igdbRatingLabel(this.game().rating));

  protected readonly summaryPreview = computed(() => {
    const summary = this.game().summary?.replace(/\s+/g, ' ').trim();

    return summary || 'Sin resumen disponible.';
  });

  protected readonly genreBadges = computed(() => {
    const genres = this.game().genres.filter((genre) => genre.trim() !== '');

    return genres.length > 0 ? genres.slice(0, 3) : ['Sin género'];
  });

  /** Familias de plataforma que se pintan como logotipo. */
  protected readonly platformBadges = computed(() =>
    this.game().platforms.slice(0, PLATFORM_ICON_LIMIT),
  );

  protected readonly extraPlatformCount = computed(() =>
    Math.max(this.game().platforms.length - PLATFORM_ICON_LIMIT, 0),
  );

  protected readonly previewImages = computed(() => {
    const screenshots = this.game()
      .screenshotUrls.map((screenshot) => screenshot.trim())
      .filter((screenshot) => screenshot !== '');

    if (screenshots.length > 0) {
      return screenshots;
    }

    return this.game().screenshotUrl ? [this.game().screenshotUrl] : [];
  });

  protected readonly trailerEmbedUrl = computed<SafeResourceUrl | null>(() => {
    const embedUrl = this.buildTrailerEmbedUrl(this.game().trailerVideoUrl);

    return embedUrl
      ? this.sanitizer.bypassSecurityTrustResourceUrl(embedUrl)
      : null;
  });

  protected readonly previewSlideCount = computed(() => {
    const trailerCount = this.trailerEmbedUrl() ? 1 : 0;

    return trailerCount + this.previewImages().length;
  });

  protected readonly hasPreviewMedia = computed(
    () => this.previewSlideCount() > 0,
  );

  protected markPreviewImageAsFailed(): void {
    this.previewImageFailed.set(true);
  }

  private buildTrailerEmbedUrl(url: string | null): string | null {
    if (!url) {
      return null;
    }

    const videoId = this.getYoutubeVideoId(url);

    if (!videoId) {
      return null;
    }

    const params = new URLSearchParams({
      autoplay: '1',
      mute: '1',
      playsinline: '1',
      controls: '0',
      rel: '0',
      modestbranding: '1',
    });

    return `https://www.youtube.com/embed/${videoId}?${params.toString()}`;
  }

  private getYoutubeVideoId(url: string): string | null {
    try {
      const parsedUrl = new URL(url);

      const hostname = parsedUrl.hostname.replace(/^www\./, '');

      if (hostname === 'youtu.be') {
        return parsedUrl.pathname.split('/').filter(Boolean)[0] ?? null;
      }

      if (hostname.endsWith('youtube.com')) {
        if (parsedUrl.pathname.startsWith('/embed/')) {
          return parsedUrl.pathname.split('/').filter(Boolean)[1] ?? null;
        }

        return parsedUrl.searchParams.get('v');
      }
    } catch {
      return null;
    }

    return null;
  }
}
