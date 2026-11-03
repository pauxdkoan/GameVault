import { NgClass } from '@angular/common';
import { Component, computed, CUSTOM_ELEMENTS_SCHEMA, inject, input, output, signal } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faApple, faPlaystation, faSteam, faWindows, faXbox } from '@fortawesome/free-brands-svg-icons';
import { faDesktop, faGamepad, faGlobe, faMobileScreen, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { GameCard } from '../../models/game-card.model';


interface PlatformBadge {
  key: string;
  label: string;
  title: string;
  icon: IconDefinition;
  toneClass: string;
}

interface PlatformIconRule {
  key: string;
  label: string;
  patterns: string[];
  icon: IconDefinition;
  toneClass: string;
}

const PLATFORM_ICON_LIMIT = 3;

const DEFAULT_PLATFORM_TONE = 'border-white/15 bg-white/10 text-zinc-100';

const PLATFORM_ICON_RULES: PlatformIconRule[] = [
  {
    key: 'playstation',
    label: 'PlayStation',
    patterns: ['playstation', 'ps5', 'ps4', 'ps3', 'ps2', 'ps1', 'psp', 'vita'],
    icon: faPlaystation,
    toneClass: 'border-blue-200/35 bg-blue-500/20 text-blue-50',
  },
  {
    key: 'xbox',
    label: 'Xbox',
    patterns: ['xbox'],
    icon: faXbox,
    toneClass: 'border-emerald-200/35 bg-emerald-400/20 text-emerald-50',
  },
  {
    key: 'windows',
    label: 'PC / Windows',
    patterns: ['pc', 'windows'],
    icon: faWindows,
    toneClass: 'border-vault-cyan/35 bg-vault-cyan/20 text-vault-text',
  },
  {
    key: 'apple',
    label: 'Apple',
    patterns: ['mac', 'os x'],
    icon: faApple,
    toneClass: 'border-zinc-100/35 bg-zinc-100/15 text-zinc-50',
  },
  {
    key: 'mobile',
    label: 'Mobile',
    patterns: ['ios', 'iphone', 'ipad', 'android', 'mobile'],
    icon: faMobileScreen,
    toneClass: 'border-lime-100/35 bg-lime-300/20 text-lime-50',
  },
  {
    key: 'steam',
    label: 'Steam',
    patterns: ['steam'],
    icon: faSteam,
    toneClass: 'border-sky-100/35 bg-sky-400/20 text-sky-50',
  },
  {
    key: 'web',
    label: 'Web',
    patterns: ['web', 'browser'],
    icon: faGlobe,
    toneClass: 'border-sky-100/35 bg-sky-400/20 text-sky-50',
  },
  {
    key: 'nintendo',
    label: 'Nintendo',
    patterns: [
      'switch',
      'nintendo',
      'wii',
      'game boy',
      'gameboy',
      'gamecube',
      'game cube',
      'nes',
      'snes',
      '3ds',
    ],
    icon: faGamepad,
    toneClass: 'border-red-100/40 bg-red-500/20 text-red-50',
  },
  {
    key: 'desktop',
    label: 'Desktop',
    patterns: ['dos', 'computer', 'linux'],
    icon: faDesktop,
    toneClass: 'border-vault-cyan/35 bg-vault-cyan/20 text-vault-text',
  },
];


@Component({
  selector: 'app-game-card-preview',
  imports: [FontAwesomeModule, NgClass, RouterLink],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './game-card-preview.html',
})
export class GameCardPreview {
  private readonly sanitizer = inject(DomSanitizer);

  readonly game = input.required<GameCard>();

  readonly variant = input<'default' | 'compact'>('default');

  protected readonly previewImageFailed = signal(false);

  protected readonly isCompact = computed(() => this.variant() === 'compact');

  protected readonly ratingLabel = computed(() =>
    this.game().rating > 0 ? this.game().rating.toFixed(1) : 'S/R',
  );

  protected readonly summaryPreview = computed(() => {
    const summary = this.game().summary?.replace(/\s+/g, ' ').trim();

    return summary || 'Sin resumen disponible.';
  });

  protected readonly genreBadges = computed(() => {
    const genres = this.game().genres.filter((genre) => genre.trim() !== '');

    return genres.length > 0 ? genres.slice(0, 3) : ['Sin género'];
  });

  protected readonly platformLabels = computed(() => {
    const platforms = this.game()
      .platformLabel.map((platform) => platform.trim())
      .filter((platform) => platform !== '');

    return Array.from(
      new Map(
        platforms.map((platform) => [platform.toLowerCase(), platform]),
      ).values(),
    );
  });

  protected readonly platformBadges = computed<PlatformBadge[]>(() => {
    const platforms = this.platformLabels();

    if (platforms.length === 0) {
      return [
        {
          key: 'unknown',
          label: 'Plataforma por confirmar',
          title: 'Plataforma por confirmar',
          icon: faGamepad,
          toneClass: DEFAULT_PLATFORM_TONE,
        },
      ];
    }

    return this.groupPlatformBadges(platforms).slice(0, PLATFORM_ICON_LIMIT);
  });

  protected readonly extraPlatformCount = computed(() =>
    Math.max(
      this.groupPlatformBadges(this.platformLabels()).length -
        PLATFORM_ICON_LIMIT,
      0,
    ),
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

  private mapPlatformToBadge(platform: string): PlatformBadge {
    const normalizedPlatform = platform.toLowerCase();

    const rule = PLATFORM_ICON_RULES.find(({ patterns }) =>
      patterns.some((pattern) => normalizedPlatform.includes(pattern)),
    );

    return {
      key: rule?.key ?? normalizedPlatform,
      label: rule?.label ?? platform,
      title: platform,
      icon: rule?.icon ?? faGamepad,
      toneClass: rule?.toneClass ?? DEFAULT_PLATFORM_TONE,
    };
  }

  private groupPlatformBadges(platforms: string[]): PlatformBadge[] {
    const badgesByFamily = new Map<string, PlatformBadge>();

    for (const platform of platforms) {
      const badge = this.mapPlatformToBadge(platform);

      const currentBadge = badgesByFamily.get(badge.key);

      if (!currentBadge) {
        badgesByFamily.set(badge.key, badge);

        continue;
      }

      badgesByFamily.set(badge.key, {
        ...currentBadge,
        title: `${currentBadge.title}, ${platform}`,
      });
    }

    return Array.from(badgesByFamily.values());
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
