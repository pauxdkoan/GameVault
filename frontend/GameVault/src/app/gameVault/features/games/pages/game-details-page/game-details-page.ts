import {
  ChangeDetectionStrategy,
  Component,
  computed,
  DestroyRef,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { NgClass } from '@angular/common';
import { GameService } from '../../services/games.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  Company,
  GameDetailsResponse,
  StoreLink,
  WebSite,
} from '../../models/games.interfaces';
import { take } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import {
  faAndroid,
  faAppStoreIos,
  faApple,
  faLinux,
  faPlaystation,
  faSteam,
  faWindows,
  faXbox,
} from '@fortawesome/free-brands-svg-icons';
import {
  faArrowUpRightFromSquare,
  faCalendarDays,
  faCommentDots,
  faDesktop,
  faGamepad,
  faGlobe,
  faLayerGroup,
  faStar,
} from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { GameCompaniesComponent } from '../../components/game-companies/game-companies';
import { GameDiscoverMoreComponent } from '../../components/game-discover-more/game-discover-more';
import { GameDetailsMediaComponent } from '../../components/game-details-media/game-details-media';

type GameDetailsStatus = 'loading' | 'ready' | 'error';

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

interface PlayLink {
  label: string;
  url: string;
}

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
    key: 'linux',
    label: 'Linux',
    patterns: ['linux'],
    icon: faLinux,
    toneClass: 'border-amber-100/35 bg-amber-300/20 text-amber-50',
  },
  {
    key: 'ios',
    label: 'iOS',
    patterns: ['ios', 'iphone', 'ipad'],
    icon: faAppStoreIos,
    toneClass: 'border-sky-100/35 bg-sky-300/20 text-sky-50',
  },
  {
    key: 'android',
    label: 'Android',
    patterns: ['android'],
    icon: faAndroid,
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
    patterns: ['dos', 'computer'],
    icon: faDesktop,
    toneClass: 'border-vault-cyan/35 bg-vault-cyan/20 text-vault-text',
  },
];

@Component({
  selector: 'app-game-details-page',
  imports: [
    FontAwesomeModule,
    GameCompaniesComponent,
    GameDetailsMediaComponent,
    GameDiscoverMoreComponent,
    NgClass,
    RouterLink,
  ],
  templateUrl: './game-details-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GameDetailsPage implements OnInit {
  
  private readonly gameService = inject(GameService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly status = signal<GameDetailsStatus>('loading');
  protected readonly gameDetails = signal<GameDetailsResponse | null>(null);
  protected readonly errorMessage = signal<unknown | null>(null);

  protected readonly icons = {
    arrowUpRight: faArrowUpRightFromSquare,
    calendar: faCalendarDays,
    comments: faCommentDots,
    gamepad: faGamepad,
    layers: faLayerGroup,
    star: faStar,
  };

  protected readonly isLoading = computed(() => this.status() === 'loading');
  protected readonly displayErrorMessage = computed(() => {
    const error = this.errorMessage();

    if (Array.isArray(error)) {
      return error.flat().filter(Boolean).join(' ');
    }

    if (typeof error === 'string' && error.trim() !== '') {
      return error;
    }

    return 'Ocurre un error inesperado intente nuevamente.';
  });
  protected readonly heroBackgroundImage = computed(() => {
    const imageUrl = this.heroImageUrl();

    if (!imageUrl) {
      return 'linear-gradient(135deg, rgba(6, 17, 36, 0.98), rgba(7, 19, 39, 0.74))';
    }

    return `linear-gradient(90deg, rgba(6, 17, 36, 0.96) 0%, rgba(6, 17, 36, 0.7) 36%, rgba(6, 17, 36, 0.24) 100%), linear-gradient(180deg, rgba(6, 17, 36, 0.04) 0%, rgba(6, 17, 36, 0.86) 100%), url("${imageUrl}")`;
  });
  protected readonly heroImageUrl = computed(() => {
    const game = this.gameDetails();

    return game?.screenshots?.[0] ?? game?.coverImage ?? null;
  });
  protected readonly releaseYear = computed(() => {
    const releaseDate = this.gameDetails()?.released;

    if (!releaseDate) {
      return 'Fecha por confirmar';
    }

    const date = new Date(releaseDate);

    return Number.isNaN(date.getTime())
      ? 'Fecha por confirmar'
      : date.getFullYear().toString();
  });
  protected readonly releaseLabel = computed(() =>
    this.formatRelease(this.gameDetails()?.released ?? null),
  );
  protected readonly ratingLabel = computed(() => {
    const rating = this.gameDetails()?.rating;

    return typeof rating === 'number' && rating > 0 ? rating.toFixed(1) : 'S/R';
  });
  protected readonly metacriticLabel = computed(() => {
    const metacritic = this.gameDetails()?.metacritic;

    return typeof metacritic === 'number' && metacritic > 0
      ? metacritic.toString()
      : 'N/A';
  });
  protected readonly primaryDeveloper = computed(() => {
    const developers = this.companyNames(
      this.gameDetails()?.companies ?? [],
      'developer',
    );

    return developers || 'Desarrollador por confirmar';
  });
  protected readonly genreChips = computed(() =>
    this.uniqueValues(this.gameDetails()?.genres ?? []).slice(0, 5),
  );
  protected readonly detailChips = computed(() => {
    const game = this.gameDetails();

    return [
      ...this.uniqueValues(game?.gameModes ?? []),
      ...this.uniqueValues(game?.themes ?? []),
    ];
  });
  protected readonly aboutParagraphs = computed(() => {
    const game = this.gameDetails();
    const summary = this.normalizeText(game?.summary);
    const storyLine = this.normalizeText(game?.storyLine);
    const paragraphs = summary ? [summary] : [];

    if (
      storyLine &&
      storyLine.toLocaleLowerCase() !== summary.toLocaleLowerCase()
    ) {
      paragraphs.push(storyLine);
    }

    return paragraphs.length > 0
      ? paragraphs
      : ['Aun no hay sinopsis disponible para este juego.'];
  });
  protected readonly platformBadges = computed(() =>
    this.groupPlatformBadges(this.gameDetails()?.platforms ?? []),
  );
  protected readonly visiblePlatformBadges = computed(() =>
    this.platformBadges().slice(0, 8),
  );
  protected readonly playLinks = computed<PlayLink[]>(() => {
    const game = this.gameDetails();
    const storeLinks = (game?.stores ?? [])
      .filter((store) => this.hasUrl(store.url))
      .map((store) => ({
        label: store.storeName || 'Tienda',
        url: store.url,
      }));

    if (storeLinks.length > 0) {
      return storeLinks.slice(0, 4);
    }

    return (game?.webSites ?? [])
      .filter((website) => this.hasUrl(website.url))
      .map((website) => ({
        label: this.websiteLabel(website),
        url: website.url,
      }))
      .slice(0, 4);
  });
  protected readonly quickFacts = computed(() => {
    const game = this.gameDetails();

    return [
      {
        label: 'Lanzamiento',
        value: this.releaseLabel(),
        icon: this.icons.calendar,
      },
      {
        label: 'Tipo',
        value: this.normalizeText(game?.gameType) || 'Juego',
        icon: this.icons.layers,
      },
      {
        label: 'IGDB',
        value: this.ratingLabel(),
        icon: this.icons.star,
      },
      {
        label: 'Metacritic',
        value: this.metacriticLabel(),
        icon: this.icons.gamepad,
      },
    ];
  });

  ngOnInit(): void {
    // Escuchamos los cambios del :id. Al navegar a un juego similar o a una
    // expansión, Angular reutiliza este componente y ngOnInit no se repite,
    // así que leer solo el snapshot dejaba los datos del juego anterior.
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((params) => {
        this.getGameDetails(Number(params.get('id')));
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });
  }

  protected retryLoad(): void {
    const gameId = Number(this.route.snapshot.paramMap.get('id'));

    this.getGameDetails(gameId);
  }

  private getGameDetails(gameId: number): void {
    this.status.set('loading');
    this.errorMessage.set(null);

    if (!gameId) {
      this.errorMessage.set(
        'Ocurrio un error al cargar los detalles del juego...',
      );
      setTimeout(() => {
        this.status.set('error');
        this.router.navigate(['/gamevault/games']);
      }, 2000);
      return;
    }

    this.gameService
      .getGameDetails({ id: gameId })
      .pipe(take(1))
      .subscribe({
        next: (response) => {
          this.gameDetails.set(response);
          this.status.set('ready');
        },
        error: (error) => {
          this.errorMessage.set(error);
          this.status.set('error');
        },
      });
  }

  private companyNames(
    companies: Company[],
    role: 'developer' | 'publisher',
  ): string {
    const names = companies
      .filter((company) =>
        role === 'developer' ? company.isDeveloper : company.isPublisher,
      )
      .map((company) => company.name)
      .filter((name) => name.trim() !== '');

    return this.uniqueValues(names).join(', ');
  }

  private formatRelease(released: Date | string | null): string {
    if (released === null) {
      return 'Fecha por confirmar';
    }

    const releaseDate = new Date(released);

    if (Number.isNaN(releaseDate.getTime())) {
      return 'Fecha por confirmar';
    }

    return new Intl.DateTimeFormat('es', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    }).format(releaseDate);
  }

  private groupPlatformBadges(platforms: string[]): PlatformBadge[] {
    const badgesByFamily = new Map<string, PlatformBadge>();

    // Agrupa generaciones separadas por familia para mantener el panel compacto.
    for (const platform of this.uniqueValues(platforms)) {
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

  private websiteLabel(website: WebSite): string {
    const name = this.normalizeText(website.name);

    if (name && !name.startsWith('http')) {
      return name;
    }

    try {
      return new URL(website.url).hostname.replace(/^www\./, '');
    } catch {
      return 'Sitio web';
    }
  }

  private hasUrl(
    url: StoreLink['url'] | WebSite['url'] | null | undefined,
  ): boolean {
    return typeof url === 'string' && /^https?:\/\//i.test(url);
  }

  private uniqueValues(values: string[]): string[] {
    return Array.from(
      new Map(
        values
          .map((value) => value.trim())
          .filter((value) => value !== '')
          .map((value) => [value.toLocaleLowerCase(), value]),
      ).values(),
    );
  }

  private normalizeText(value: string | null | undefined): string {
    return value?.replace(/\s+/g, ' ').trim() ?? '';
  }
}
