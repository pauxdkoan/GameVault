import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  input,
  output,
  signal,
  untracked,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of } from 'rxjs';
import { Icon } from '../../../../shared/ui/icon/icon';
import {
  GameFilters,
  GameFiltersResponse,
  GameSort,
} from '../../models/games.interfaces';
import { GameService } from '../../services/games.service';

type FilterKey = 'genreIds' | 'platformIds' | 'sort';
type ChipKey = FilterKey | 'search';

interface FilterOption {
  label: string;
  value: number | GameSort;
}

interface FilterGroup {
  title: string;
  key: FilterKey;
  options: FilterOption[];
}

interface FilterChip {
  label: string;
  key: ChipKey;
  value?: number | GameSort;
}

@Component({
  selector: 'app-game-filter',
  imports: [Icon],
  templateUrl: './game-filter.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GameFilter {
  readonly filtersChange = output<GameFilters>();
  /** Búsqueda que llega desde la URL (?search=), p. ej. desde el buscador del header. */
  readonly initialSearch = input<string | undefined>(undefined);

  protected readonly search = signal('');
  protected readonly activeFilters = signal<GameFilters>({ sort: 'Popular' });
  protected readonly openGroup = signal<FilterKey | null>(null);
  protected readonly selectedChips = computed(() => this.buildSelectedChips());

  /**
   * Grupos de filtros. Las opciones vienen de GET /api/games/filters
   * (antes los ids de IGDB estaban escritos a mano aquí).
   * Si la petición falla, se muestran sin opciones en vez de romper el catálogo.
   */
  protected readonly filterGroups = toSignal(
    inject(GameService)
      .getFilters()
      .pipe(
        map((filters) => this.toFilterGroups(filters)),
        catchError(() => of<FilterGroup[]>([])),
      ),
    { initialValue: [] as FilterGroup[] },
  );

  constructor() {
    // Si la página recibe ?search= en la URL, la caja y el chip lo reflejan
    // (la página es quien vuelve a cargar los juegos).
    effect(() => {
      const search = this.initialSearch()?.trim() || undefined;
      untracked(() => {
        this.search.set(search ?? '');
        this.activeFilters.update((filters) => ({ ...filters, search }));
      });
    });
  }

  protected onSearchInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.search.set(input.value);
  }

  protected applySearch(event?: Event): void {
    event?.preventDefault();
    this.openGroup.set(null);

    this.emitFilters({
      ...this.activeFilters(),
      search: this.cleanSearch(),
    });
  }

  protected selectOption(group: FilterGroup, option: FilterOption): void {
    const nextFilters: GameFilters = {
      ...this.activeFilters(),
      search: this.cleanSearch(),
    };

    if (group.key === 'sort') {
      nextFilters.sort = option.value as GameSort;
      this.openGroup.set(null);
    } else {
      nextFilters[group.key] = this.toggleSelectedId(
        nextFilters[group.key],
        option.value as number,
      );
    }

    this.emitFilters(nextFilters);
  }

  protected toggleFilterGroup(group: FilterGroup): void {
    this.openGroup.update((currentGroup) =>
      currentGroup === group.key ? null : group.key,
    );
  }

  protected isGroupOpen(group: FilterGroup): boolean {
    return this.openGroup() === group.key;
  }

  protected removeChip(chip: FilterChip): void {
    const nextFilters: GameFilters = {
      ...this.activeFilters(),
      search: this.cleanSearch(),
    };

    if (chip.key === 'search') {
      this.search.set('');
      nextFilters.search = undefined;
    } else if (chip.key === 'sort') {
      nextFilters.sort = 'Popular';
    } else {
      nextFilters[chip.key] = this.removeSelectedId(
        nextFilters[chip.key],
        chip.value as number,
      );
    }

    this.emitFilters(nextFilters);
  }

  protected isActiveOption(group: FilterGroup, option: FilterOption): boolean {
    const filters = this.activeFilters();

    if (group.key === 'sort') {
      return filters.sort === option.value;
    }

    return (filters[group.key] ?? []).includes(option.value as number);
  }

  protected selectedCount(group: FilterGroup): number {
    const filters = this.activeFilters();

    if (group.key === 'sort') {
      return filters.sort ? 1 : 0;
    }

    return filters[group.key]?.length ?? 0;
  }

  private emitFilters(filters: GameFilters): void {
    const nextFilters = this.normalizeFilters(filters);
    this.activeFilters.set(nextFilters);
    this.filtersChange.emit(nextFilters);
  }

  private buildSelectedChips(): FilterChip[] {
    const filters = this.activeFilters();
    const chips: FilterChip[] = [];

    if (filters.search) {
      chips.push({
        key: 'search',
        label: `Búsqueda: ${filters.search}`,
      });
    }

    this.addMultiSelectChips(chips, 'platformIds', 'Plataforma');
    this.addMultiSelectChips(chips, 'genreIds', 'Género');

    if (filters.sort) {
      chips.push({
        key: 'sort',
        label: `Orden: ${this.findOptionLabel('sort', filters.sort)}`,
        value: filters.sort,
      });
    }

    return chips;
  }

  private addMultiSelectChips(
    chips: FilterChip[],
    key: 'genreIds' | 'platformIds',
    label: string,
  ): void {
    for (const value of this.activeFilters()[key] ?? []) {
      chips.push({
        key,
        label: `${label}: ${this.findOptionLabel(key, value)}`,
        value,
      });
    }
  }

  private toFilterGroups(filters: GameFiltersResponse): FilterGroup[] {
    return [
      {
        title: 'Plataforma',
        key: 'platformIds',
        options: filters.platforms.map(({ id, label }) => ({ label, value: id })),
      },
      {
        title: 'Género',
        key: 'genreIds',
        options: filters.genres.map(({ id, label }) => ({ label, value: id })),
      },
      {
        title: 'Orden',
        key: 'sort',
        options: filters.sorts.map(({ value, label }) => ({ label, value })),
      },
    ];
  }

  private findOptionLabel(key: FilterKey, value: number | GameSort): string {
    const group = this.filterGroups().find((filterGroup) => filterGroup.key === key);
    return (
      group?.options.find((option) => option.value === value)?.label ??
      value.toString()
    );
  }

  private cleanSearch(): string | undefined {
    const search = this.search().trim();
    return search.length > 0 ? search : undefined;
  }

  private normalizeFilters(filters: GameFilters): GameFilters {
    return {
      search: filters.search,
      genreIds: this.normalizeIds(filters.genreIds),
      platformIds: this.normalizeIds(filters.platformIds),
      sort: filters.sort,
    };
  }

  private normalizeIds(ids?: number[]): number[] | undefined {
    const normalizedIds = [...new Set(ids ?? [])].filter((id) => id > 0);
    return normalizedIds.length > 0 ? normalizedIds : undefined;
  }

  private toggleSelectedId(ids: number[] | undefined, value: number): number[] | undefined {
    const selectedIds = ids ?? [];

    if (selectedIds.includes(value)) {
      return this.removeSelectedId(selectedIds, value);
    }

    return [...selectedIds, value];
  }

  private removeSelectedId(ids: number[] | undefined, value: number): number[] | undefined {
    const nextIds = (ids ?? []).filter((id) => id !== value);
    return nextIds.length > 0 ? nextIds : undefined;
  }
}
