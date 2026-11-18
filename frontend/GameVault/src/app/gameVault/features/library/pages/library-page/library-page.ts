import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessages } from '../../../../core/http/api-error';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { PageHeader } from '../../../../shared/ui/page-header/page-header';
import { LibraryEntryCard } from '../../components/library-entry-card/library-entry-card';
import { GameStatus, LIBRARY_STATUSES, LibraryEntry } from '../../models/library.models';
import { LibraryService } from '../../services/library.service';

const PAGE_SIZE = 24;

/**
 * Página "Mi biblioteca" (/gamevault/library, requiere sesión).
 * Pestañas por estado con su contador; "Todos" muestra la biblioteca completa.
 */
@Component({
  selector: 'app-library-page',
  imports: [PageHeader, LibraryEntryCard, VaultButton, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './library-page.html',
})
export class LibraryPage implements OnInit {
  private readonly libraryService = inject(LibraryService);

  protected readonly statuses = LIBRARY_STATUSES;
  protected readonly selectedStatus = signal<GameStatus | null>(null);
  protected readonly entries = signal<LibraryEntry[]>([]);
  protected readonly counts = signal<Partial<Record<GameStatus, number>>>({});
  protected readonly total = signal(0);
  protected readonly page = signal(1);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);

  /** Total de toda la biblioteca, sumando los contadores de cada estado. */
  protected readonly libraryTotal = computed(() =>
    Object.values(this.counts()).reduce((sum, count) => sum + (count ?? 0), 0),
  );
  protected readonly hasMore = computed(() => this.page() * PAGE_SIZE < this.total());

  ngOnInit(): void {
    this.load(1);
  }

  protected selectStatus(status: GameStatus | null): void {
    this.selectedStatus.set(status);
    this.load(1);
  }

  protected load(page: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.libraryService.getLibrary(this.selectedStatus(), page, PAGE_SIZE).subscribe({
      next: (response) => {
        const items = response.entries.items;
        this.entries.set(page === 1 ? items : [...this.entries(), ...items]);
        this.counts.set(response.statusCounts);
        this.total.set(response.entries.count);
        this.page.set(page);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.error.set(errorMessages(error).join(' '));
        this.isLoading.set(false);
      },
    });
  }

  /**
   * Tras editar o quitar recargamos la primera página: los contadores cambian
   * y el juego puede haber salido de la pestaña actual.
   */
  protected refresh(): void {
    this.load(1);
  }
}
