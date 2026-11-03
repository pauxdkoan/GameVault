import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { take } from 'rxjs';
import { HeroSectionComponent } from '../../components/hero-section/hero-section';
import { HomeGameSectionComponent } from '../../components/home-game-section/home-game-section';
import { HomeUpcomingList } from '../../components/home-upcoming-list/home-upcoming-list';
import { HomeFeatures } from '../../components/home-features/home-features';
import { HomeLibraryPreview } from '../../components/home-library-preview/home-library-preview';
import { HomeCommunity } from '../../components/home-community/home-community';
import { HomePlayerLists } from '../../components/home-player-lists/home-player-lists';
import { HomeCta } from '../../components/home-cta/home-cta';
import { HomeDataService } from '../../services/home-data.service';
import { HomeContent } from '../../models/home-content.model';

type HomePageStatus = 'loading' | 'ready' | 'error';

const EMPTY_HOME_CONTENT: HomeContent = {
  heroSlides: [],
  trendingGames: [],
  upcomingGames: [],
  discoverGames: [],
};

/**
 * Página principal pública.
 *
 * Solo hace dos cosas:
 *  1. Pide el contenido al HomeDataService.
 *  2. Reparte los datos a cada sección (cada sección es un componente).
 *
 * Las secciones que dependen de la API (tendencias, lanzamientos) muestran
 * un esqueleto mientras cargan y un aviso si falla. Las secciones
 * informativas (features, comunidad, CTA) se ven siempre.
 */
@Component({
  selector: 'app-home-page-public',
  imports: [
    HeroSectionComponent,
    HomeGameSectionComponent,
    HomeUpcomingList,
    HomeFeatures,
    HomeLibraryPreview,
    HomeCommunity,
    HomePlayerLists,
    HomeCta,
  ],
  templateUrl: './home-page-public.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePagePublic implements OnInit {
  private readonly homeDataService = inject(HomeDataService);

  protected readonly content = signal<HomeContent>(EMPTY_HOME_CONTENT);
  protected readonly status = signal<HomePageStatus>('loading');

  protected readonly isLoading = computed(() => this.status() === 'loading');
  protected readonly skeletonCards = [1, 2, 3, 4, 5, 6];

  ngOnInit(): void {
    this.loadHomeContent();
  }

  protected retryLoad(): void {
    this.loadHomeContent();
  }

  private loadHomeContent(): void {
    this.status.set('loading');

    this.homeDataService
      .getHomeContent()
      .pipe(take(1))
      .subscribe({
        next: (content) => {
          this.content.set(content);
          this.status.set('ready');
        },
        error: () => {
          this.content.set(EMPTY_HOME_CONTENT);
          this.status.set('error');
        },
      });
  }
}
