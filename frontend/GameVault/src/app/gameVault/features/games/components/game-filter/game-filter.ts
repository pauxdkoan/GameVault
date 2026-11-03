import {
  ChangeDetectionStrategy,
  Component,
  computed,
  output,
  signal,
} from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faArrowRight,
  faCheck,
  faChevronDown,
  faMagnifyingGlass,
  faXmark,
} from '@fortawesome/free-solid-svg-icons';
import { GameFilters, GameSort } from '../../models/games.interfaces';

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
  imports: [FontAwesomeModule],
  templateUrl: './game-filter.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GameFilter {
  readonly filtersChange = output<GameFilters>();

  protected readonly searchIcon = faMagnifyingGlass;
  protected readonly arrowIcon = faArrowRight;
  protected readonly chevronIcon = faChevronDown;
  protected readonly checkIcon = faCheck;
  protected readonly removeIcon = faXmark;

  protected readonly search = signal('');
  protected readonly activeFilters = signal<GameFilters>({ sort: 'Popular' });
  protected readonly openGroup = signal<FilterKey | null>(null);
  protected readonly selectedChips = computed(() => this.buildSelectedChips());

  protected readonly filterGroups: FilterGroup[] = [
    {
      title: 'Plataforma',
      key: 'platformIds',
      options: [
        { label: 'PC', value: 6 },
        { label: 'PlayStation 5', value: 167 },
        { label: 'Xbox Series', value: 169 },
        { label: 'Nintendo Switch', value: 130 },
        { label: 'Android', value: 34 },
      ],
    },
    {
      title: 'Genero',
      key: 'genreIds',
      options: [
        { label: 'Aventura', value: 31 },
        { label: 'RPG', value: 12 },
        { label: 'Indie', value: 32 },
        { label: 'Estrategia', value: 15 },
        { label: 'Shooter', value: 5 },
      ],
    },
    {
      title: 'Orden',
      key: 'sort',
      options: [
        { label: 'Popularidad', value: 'Popular' },
        { label: 'Rating', value: 'Rating' },
        { label: 'Lanzamiento', value: 'ReleaseDate' },
      ],
    },
  ];

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
        label: `Busqueda: ${filters.search}`,
      });
    }

    this.addMultiSelectChips(chips, 'platformIds', 'Plataforma');
    this.addMultiSelectChips(chips, 'genreIds', 'Genero');

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

  private findOptionLabel(key: FilterKey, value: number | GameSort): string {
    const group = this.filterGroups.find((filterGroup) => filterGroup.key === key);
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
