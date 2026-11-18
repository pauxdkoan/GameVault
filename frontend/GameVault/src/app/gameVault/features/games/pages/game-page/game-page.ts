import {
  ChangeDetectionStrategy,
  Component,
  HostListener,
  OnDestroy,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { Subscription, take } from 'rxjs';
import { GameCardComponent } from '../../../../shared/components/game-card/game-card';
import { GameCard } from '../../../../shared/models/game-card.model';
import { toGameCards } from '../../../../shared/mappers/game-card.mapper';
import { GameFilters, GetGamesResponse } from '../../models/games.interfaces';
import { GameFilter } from '../../components/game-filter/game-filter';
import { GameService } from '../../services/games.service';

type GamePageStatus = 'loading' | 'ready' | 'empty' | 'error';

@Component({
  selector: 'app-game-page',
  imports: [GameCardComponent, GameFilter],
  templateUrl: './game-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GamePage implements OnInit, OnDestroy {
  private readonly gameService = inject(GameService);
  private readonly pageSize = 20;
  private readonly nextPage = signal(1);
  private gamesSubscription?: Subscription;

  protected readonly games = signal<GameCard[]>([]);
  protected readonly activeFilters = signal<GameFilters>({ sort: 'Popular' });
  protected readonly status = signal<GamePageStatus>('loading');
  protected readonly isLoading = signal(false);
  protected readonly hasMoreGames = signal(true);
  protected readonly totalGames = signal(0);
  protected readonly skeletonCards = Array.from(
    { length: 12 },
    (_, index) => index + 1,
  );

  protected readonly isFirstLoad = computed(
    () => this.isLoading() && this.games().length === 0,
  );
  protected readonly isLoadingMore = computed(
    () => this.isLoading() && this.games().length > 0,
  );
  protected readonly canLoadMore = computed(
    () => this.hasMoreGames() && !this.isLoading(),
  );
  protected readonly loadedGamesLabel = computed(
    () => `${this.games().length} juegos cargados`,
  );
  protected readonly totalGamesLabel = computed(() =>
    this.totalGames() > 0
      ? `${this.totalGames()} juegos disponibles`
      : 'Catalogo de juegos',
  );

  ngOnInit(): void {
    this.loadNextPage();
  }

  ngOnDestroy(): void {
    this.gamesSubscription?.unsubscribe();
  }

  @HostListener('window:scroll')
  protected onWindowScroll(): void {
    if (!this.canLoadMore() || typeof window === 'undefined') {
      return;
    }

    const scrollTop = window.scrollY;
    const clientHeight = window.innerHeight;
    const scrollHeight = document.documentElement.scrollHeight;
    const isNearBottom = scrollTop + clientHeight + 350 >= scrollHeight;

    if (isNearBottom) {
      this.loadNextPage();
    }
  }

  protected retryLoad(): void {
    this.resetGames();
    this.loadNextPage();
  }

  protected onFiltersChange(filters: GameFilters): void {
    this.gamesSubscription?.unsubscribe();
    this.activeFilters.set(filters);
    this.resetGames();
    this.loadNextPage();
  }

  protected loadNextPage(): void {
    if (!this.canLoadMore()) {
      return;
    }

    this.isLoading.set(true);
    const filters = this.activeFilters();

    this.gamesSubscription = this.gameService
      .getGames({
        page: this.nextPage(),
        pageSize: this.pageSize,
        ...filters,
      })
      .pipe(take(1))
      .subscribe({
        next: (response) => this.appendGames(response),
        error: () => {
          this.status.set(this.games().length > 0 ? 'ready' : 'error');
          this.isLoading.set(false);
        },
      });
  }

  private appendGames(response: GetGamesResponse): void {
    const currentGamesCount = this.games().length;
    const newGames = toGameCards(response.games);

    this.games.update((currentGames) => [...currentGames, ...newGames]);
    this.totalGames.set(response.count ?? currentGamesCount + newGames.length);
    this.nextPage.update((currentPage) => currentPage + 1);
    this.hasMoreGames.set(this.checkHasMoreGames(response, newGames.length));
    this.status.set(this.games().length > 0 ? 'ready' : 'empty');
    this.isLoading.set(false);
  }

  private resetGames(): void {
    this.games.set([]);
    this.totalGames.set(0);
    this.nextPage.set(1);
    this.hasMoreGames.set(true);
    this.isLoading.set(false);
    this.status.set('loading');
  }

  private checkHasMoreGames(
    response: GetGamesResponse,
    newGamesCount: number,
  ): boolean {
    if (newGamesCount === 0) {
      return false;
    }

    if (response.count !== undefined) {
      return this.games().length < response.count;
    }

    return newGamesCount === this.pageSize;
  }
}
