import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { AuthSessionService } from '../../../../core/services/auth-session.service';
import { RouterLink } from '@angular/router';
import { GameCard } from '../../../../shared/models/game-card.model';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { LIBRARY_STATUSES } from '../../../library/models/library.models';

/** Estados de ejemplo para las tres portadas de muestra. */
const PREVIEW_STATES = [
  { label: 'Jugando', progress: 64, dotClass: 'bg-vault-accent', fallbackName: 'Nova Requiem' },
  { label: 'Completado', progress: 100, dotClass: 'bg-vault-success', fallbackName: 'Ashes of Tomorrow' },
  { label: 'Pendiente', progress: 0, dotClass: 'bg-vault-warning', fallbackName: 'Lumen Hollow' },
];

/**
 * Sección "Cada juego, en su estante": texto a la izquierda y tres portadas
 * reales (tendencias) con un estado de ejemplo a la derecha.
 * Los estados y el progreso son de ejemplo y la sección lo dice en pantalla.
 * Con sesión, el botón lleva a la biblioteca real (/gamevault/library).
 */
@Component({
  selector: 'app-home-library-preview',
  imports: [RouterLink, VaultButton],
  templateUrl: './home-library-preview.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeLibraryPreview {
  protected readonly session = inject(AuthSessionService);

  readonly games = input.required<GameCard[]>();

  /** Los seis estados reales de la biblioteca (mismos textos y colores que en /library). */
  protected readonly statuses = LIBRARY_STATUSES;

  /**
   * Une cada estado de ejemplo con un juego real.
   * Si la API no devolvió juegos, se usa un nombre de ejemplo.
   */
  protected readonly items = computed(() =>
    PREVIEW_STATES.map((state, index) => {
      // Se saltan los 3 primeros para no repetir las portadas de la fila de tendencias.
      const game = this.games()[index + 3] ?? this.games()[index];
      return {
        id: game?.id ?? state.fallbackName,
        name: game?.name ?? state.fallbackName,
        cover: game?.coverImageUrl ?? null,
        label: state.label,
        progress: state.progress,
        dotClass: state.dotClass,
      };
    }),
  );
}
