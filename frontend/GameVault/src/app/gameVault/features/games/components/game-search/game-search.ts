import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  HostListener,
  computed,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  map,
  Observable,
  of,
  startWith,
  switchMap,
} from 'rxjs';
import { Icon } from '../../../../shared/ui/icon/icon';
import { igdbRatingLabel } from '../../../../shared/utils/rating';
import { formatReleaseYear } from '../../../../shared/utils/date';
import { GameService } from '../../services/games.service';

/** Letras mínimas para empezar a buscar. */
const MIN_LENGTH = 2;
/** Espera tras la última tecla antes de llamar a la API (ms). */
const DEBOUNCE_MS = 300;

/** Una sugerencia ya preparada para pintar. */
interface SearchSuggestion {
  id: number;
  name: string;
  year: string;
  rating: string;
  coverUrl: string | null;
}

/** Estado del desplegable. */
type SearchState =
  | { status: 'idle' }
  | { status: 'loading' }
  | { status: 'error' }
  | { status: 'done'; results: SearchSuggestion[] };

/**
 * Buscador en vivo del header (estilo "typeahead").
 *
 * Cómo funciona, paso a paso:
 *  1. Cada tecla actualiza la señal `query`.
 *  2. `debounceTime(300)` espera a que el usuario deje de escribir 300 ms,
 *     así "zelda" hace 1 petición y no 5.
 *  3. `distinctUntilChanged()` evita repetir la búsqueda si el texto no cambió.
 *  4. Con menos de 2 letras no se busca nada.
 *  5. `switchMap` cancela la petición anterior si llega una nueva: nunca se
 *     pinta una respuesta vieja encima de la buena ("zel" llegando después de "zelda").
 *
 * Teclado: ↓/↑ recorren los resultados, Enter abre el marcado
 * (o el catálogo con la búsqueda completa) y Escape cierra.
 */
@Component({
  selector: 'app-game-search',
  imports: [Icon],
  templateUrl: './game-search.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'relative' },
})
export class GameSearch {
  /** Texto del placeholder (el header y el menú móvil usan el mismo componente). */
  readonly placeholder = input('Buscar juegos');
  /** Avisa al padre cuando se navega a un resultado (el header cierra su menú móvil). */
  readonly navigated = output<void>();

  private readonly gameService = inject(GameService);
  private readonly router = inject(Router);
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  protected readonly listboxId = `game-search-${Math.random().toString(36).slice(2, 8)}`;
  protected readonly minLength = MIN_LENGTH;

  protected readonly query = signal('');
  protected readonly isOpen = signal(false);
  protected readonly activeIndex = signal(-1);

  protected readonly state = toSignal(
    toObservable(this.query).pipe(
      map((text) => text.trim()),
      debounceTime(DEBOUNCE_MS),
      distinctUntilChanged(),
      switchMap((text) => this.search(text)),
    ),
    { initialValue: { status: 'idle' } as SearchState },
  );

  protected readonly results = computed(() => {
    const state = this.state();
    return state.status === 'done' ? state.results : [];
  });

  /** El texto es lo bastante largo como para mostrar el desplegable. */
  protected readonly hasQuery = computed(() => this.query().trim().length >= MIN_LENGTH);

  protected readonly showPanel = computed(() => this.isOpen() && this.hasQuery());

  protected onInput(event: Event): void {
    this.query.set((event.target as HTMLInputElement).value);
    this.activeIndex.set(-1);
    this.isOpen.set(true);
  }

  protected onKeydown(event: KeyboardEvent): void {
    const total = this.results().length;

    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.isOpen.set(true);
        if (total > 0) {
          this.activeIndex.update((index) => (index + 1) % total);
        }
        break;
      case 'ArrowUp':
        event.preventDefault();
        if (total > 0) {
          this.activeIndex.update((index) => (index <= 0 ? total - 1 : index - 1));
        }
        break;
      case 'Enter': {
        event.preventDefault();
        const active = this.results()[this.activeIndex()];
        if (active) {
          this.openGame(active);
        } else {
          this.openAllResults();
        }
        break;
      }
      case 'Escape':
        this.close();
        break;
    }
  }

  protected openGame(game: SearchSuggestion): void {
    this.router.navigate(['/gamevault/gamesdetails', game.id]);
    this.reset();
  }

  /** "Ver todos los resultados": el catálogo con la búsqueda aplicada (?search=). */
  protected openAllResults(): void {
    const text = this.query().trim();
    if (text.length < MIN_LENGTH) {
      return;
    }

    this.router.navigate(['/gamevault/games'], { queryParams: { search: text } });
    this.reset();
  }

  protected optionId(index: number): string {
    return `${this.listboxId}-option-${index}`;
  }

  /** Cierra al hacer clic fuera del buscador. */
  @HostListener('document:click', ['$event'])
  protected onDocumentClick(event: MouseEvent): void {
    if (!this.host.nativeElement.contains(event.target as Node)) {
      this.close();
    }
  }

  private close(): void {
    this.isOpen.set(false);
    this.activeIndex.set(-1);
  }

  private reset(): void {
    this.query.set('');
    this.close();
    this.navigated.emit();
  }

  private search(text: string): Observable<SearchState> {
    if (text.length < MIN_LENGTH) {
      return of({ status: 'idle' });
    }

    return this.gameService.searchGames(text).pipe(
      map((response): SearchState => ({
        status: 'done',
        results: response.games.map((game) => ({
          id: game.externalId,
          name: game.name,
          year: formatReleaseYear(game.released),
          rating: igdbRatingLabel(game.rating),
          coverUrl: game.coverImage,
        })),
      })),
      catchError(() => of<SearchState>({ status: 'error' })),
      startWith<SearchState>({ status: 'loading' }),
    );
  }
}
