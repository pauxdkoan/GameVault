import { ChangeDetectionStrategy, Component, inject, input, linkedSignal, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthSessionService } from '../../../../core/services/auth-session.service';
import { errorMessages } from '../../../../core/http/api-error';
import { GameStatus, LIBRARY_STATUSES, LibraryEntry, statusDotClass, statusLabel } from '../../models/library.models';
import { LibraryService } from '../../services/library.service';
import { Icon } from '../../../../shared/ui/icon/icon';

/**
 * Botón "Añadir a mi biblioteca" del detalle de un juego.
 *
 * - Sin sesión: lleva al login y vuelve a este juego después.
 * - Sin el juego en la biblioteca: abre la lista de estados para añadirlo.
 * - Con el juego: muestra su estado y permite cambiarlo o quitarlo.
 */
@Component({
  selector: 'app-library-button',
  imports: [Icon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './library-button.html',
  host: { '(document:keydown.escape)': 'isOpen.set(false)' },
})
export class LibraryButton {
  private readonly libraryService = inject(LibraryService);
  private readonly session = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly externalId = input.required<number>();
  /** El estado que trae el detalle del juego (myLibraryEntry). */
  readonly initialEntry = input<LibraryEntry | null>(null);

  /** Se reinicia solo si cambia el juego (linkedSignal), y se actualiza al guardar. */
  protected readonly entry = linkedSignal(() => this.initialEntry());
  protected readonly isOpen = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly statuses = LIBRARY_STATUSES;
  protected readonly statusLabel = statusLabel;
  protected readonly statusDotClass = statusDotClass;
  protected readonly menuId = `library-menu-${Math.random().toString(36).slice(2, 8)}`;

  protected toggle(): void {
    if (!this.session.isLoggedIn()) {
      this.router.navigate(['/auth/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    this.isOpen.update((open) => !open);
  }

  protected choose(status: GameStatus): void {
    const current = this.entry();
    this.isOpen.set(false);

    if (current?.status === status) {
      return;
    }

    // Al cambiar de estado se conservan el progreso, las horas y la nota.
    this.save(() =>
      this.libraryService.saveEntry(this.externalId(), {
        status,
        progressPercentage: current?.progressPercentage ?? 0,
        hoursPlayed: current?.hoursPlayed ?? null,
        personalRating: current?.personalRating ?? null,
        startedAt: current?.startedAt ?? null,
        completedAt: current?.completedAt ?? null,
        privateNotes: current?.privateNotes ?? null,
      }),
    );
  }

  protected remove(): void {
    this.isOpen.set(false);
    this.isSaving.set(true);

    this.libraryService.removeEntry(this.externalId()).subscribe({
      next: () => {
        this.entry.set(null);
        this.isSaving.set(false);
      },
      error: (error: unknown) => this.fail(error),
    });
  }

  private save(request: () => ReturnType<LibraryService['saveEntry']>): void {
    this.isSaving.set(true);
    this.error.set(null);

    request().subscribe({
      next: (entry) => {
        this.entry.set(entry);
        this.isSaving.set(false);
      },
      error: (error: unknown) => this.fail(error),
    });
  }

  private fail(error: unknown): void {
    this.isSaving.set(false);
    this.error.set(errorMessages(error).join(' '));
  }
}
