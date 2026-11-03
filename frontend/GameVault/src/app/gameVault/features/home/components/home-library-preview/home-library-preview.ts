import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GameCard } from '../../../../shared/models/game-card.model';
import { VaultButton } from '../../../../shared/ui/button/vault-button';

/** Estados de ejemplo para la vista previa de la biblioteca. */
const PREVIEW_STATES = [
  { label: 'Jugando', progress: 64, bar: 'bg-vault-gradient', fallbackName: 'Nova Requiem' },
  { label: 'Completado', progress: 100, bar: 'bg-vault-violet', fallbackName: 'Ashes of Tomorrow' },
  { label: 'Pendiente', progress: 12, bar: 'bg-vault-cyan', fallbackName: 'Lumen Hollow' },
];

/**
 * Sección "Tu biblioteca. Tus reglas.": texto a la izquierda y
 * una maqueta de la biblioteca a la derecha con 3 juegos reales.
 * Los estados y el progreso son de ejemplo hasta que exista la biblioteca.
 */
@Component({
  selector: 'app-home-library-preview',
  imports: [RouterLink, VaultButton],
  templateUrl: './home-library-preview.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeLibraryPreview {
  readonly games = input.required<GameCard[]>();

  protected readonly tabs = ['Todos', 'Jugando', 'Pendientes', 'Completados'];

  protected readonly benefits = [
    'Estados personalizados',
    'Seguimiento de progreso',
    'Valoraciones personales',
    'Listas y colecciones',
  ];

  /**
   * Une cada estado de ejemplo con un juego real.
   * Si la API no devolvió juegos, se usa un nombre de ejemplo.
   */
  protected readonly items = computed(() =>
    PREVIEW_STATES.map((state, index) => {
      const game = this.games()[index];
      return {
        id: game?.id ?? state.fallbackName,
        name: game?.name ?? state.fallbackName,
        cover: game?.coverImageUrl ?? null,
        rating: game?.rating ?? 0,
        label: state.label,
        progress: state.progress,
        bar: state.bar,
      };
    }),
  );
}
