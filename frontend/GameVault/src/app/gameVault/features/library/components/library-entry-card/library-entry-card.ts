import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessages } from '../../../../core/http/api-error';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { RatingStars } from '../../../../shared/ui/rating-stars/rating-stars';
import { GameStatus, LIBRARY_STATUSES, LibraryEntry, statusDotClass, statusLabel } from '../../models/library.models';
import { LibraryService } from '../../services/library.service';

/**
 * Un juego de "Mi biblioteca": portada, estado, progreso, nota y horas.
 * "Editar" abre un pequeño formulario en la propia tarjeta.
 * Al guardar o quitar avisa a la página (saved / removed) para que recargue los contadores.
 */
@Component({
  selector: 'app-library-entry-card',
  imports: [RouterLink, RatingStars, VaultButton],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  templateUrl: './library-entry-card.html',
})
export class LibraryEntryCard {
  private readonly libraryService = inject(LibraryService);

  readonly entry = input.required<LibraryEntry>();
  readonly saved = output<LibraryEntry>();
  readonly removed = output<void>();

  protected readonly statuses = LIBRARY_STATUSES;
  protected readonly statusLabel = statusLabel;
  protected readonly statusDotClass = statusDotClass;

  /** La portada no cargó: se pinta la inicial del juego. */
  protected readonly coverFailed = signal(false);
  protected readonly isEditing = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly error = signal<string | null>(null);

  // Valores del formulario de edición (se rellenan al abrirlo).
  protected readonly status = signal<GameStatus>('Backlog');
  protected readonly progress = signal(0);
  protected readonly rating = signal<number | null>(null);
  protected readonly hours = signal<number | null>(null);

  protected startEdit(): void {
    const entry = this.entry();
    this.status.set(entry.status);
    this.progress.set(entry.progressPercentage);
    this.rating.set(entry.personalRating);
    this.hours.set(entry.hoursPlayed);
    this.error.set(null);
    this.isEditing.set(true);
  }

  protected numberValue(event: Event): number | null {
    const value = (event.target as HTMLInputElement).value;
    return value === '' ? null : Number(value);
  }

  protected save(): void {
    const entry = this.entry();
    // Al marcarlo como completado, el progreso pasa a 100 % (lo normal y lo que espera el perfil).
    const progress = this.status() === 'Completed' ? 100 : this.progress();

    this.isSaving.set(true);
    this.libraryService
      .saveEntry(entry.game.externalId, {
        status: this.status(),
        progressPercentage: progress,
        personalRating: this.rating(),
        hoursPlayed: this.hours(),
        startedAt: entry.startedAt,
        completedAt: entry.completedAt,
        privateNotes: entry.privateNotes,
      })
      .subscribe({
        next: (updated) => {
          this.isSaving.set(false);
          this.isEditing.set(false);
          this.saved.emit(updated);
        },
        error: (error: unknown) => {
          this.isSaving.set(false);
          this.error.set(errorMessages(error).join(' '));
        },
      });
  }

  protected remove(): void {
    this.isSaving.set(true);
    this.libraryService.removeEntry(this.entry().game.externalId).subscribe({
      next: () => this.removed.emit(),
      error: (error: unknown) => {
        this.isSaving.set(false);
        this.error.set(errorMessages(error).join(' '));
      },
    });
  }
}
