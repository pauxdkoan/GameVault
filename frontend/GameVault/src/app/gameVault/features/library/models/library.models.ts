import { GameCardDto } from '../../../shared/models/game-card.model';
import { PagedResult } from '../../../shared/models/api.models';

/** Estados de un juego en la biblioteca (enum GameStatus del backend). */
export type GameStatus = 'Wishlist' | 'Backlog' | 'Playing' | 'Completed' | 'Paused' | 'Dropped';

/** Texto, icono de color y orden de cada estado. Es la única fuente para pintar estados. */
export const LIBRARY_STATUSES: { value: GameStatus; label: string; dotClass: string }[] = [
  { value: 'Playing', label: 'Jugando', dotClass: 'bg-vault-accent' },
  { value: 'Backlog', label: 'Pendiente', dotClass: 'bg-vault-warning' },
  { value: 'Wishlist', label: 'Lo quiero', dotClass: 'bg-vault-text' },
  { value: 'Completed', label: 'Completado', dotClass: 'bg-vault-success' },
  { value: 'Paused', label: 'En pausa', dotClass: 'bg-vault-muted' },
  { value: 'Dropped', label: 'Abandonado', dotClass: 'bg-vault-danger' },
];

export function statusLabel(status: GameStatus): string {
  return LIBRARY_STATUSES.find((item) => item.value === status)?.label ?? status;
}

export function statusDotClass(status: GameStatus): string {
  return LIBRARY_STATUSES.find((item) => item.value === status)?.dotClass ?? 'bg-vault-muted';
}

/** Un juego en mi biblioteca (LibraryEntryDto). */
export interface LibraryEntry {
  game: GameCardDto;
  status: GameStatus;
  progressPercentage: number;
  hoursPlayed: number | null;
  /** De 1 a 10. */
  personalRating: number | null;
  startedAt: string | null;
  completedAt: string | null;
  privateNotes: string | null;
  addedAt: string;
  updatedAt: string | null;
}

/** Respuesta de GET /api/me/library. */
export interface LibraryPage {
  entries: PagedResult<LibraryEntry>;
  statusCounts: Partial<Record<GameStatus, number>>;
}

/** Cuerpo de PUT /api/me/library/{externalId}. */
export interface LibraryEntryRequest {
  status: GameStatus;
  progressPercentage?: number;
  hoursPlayed?: number | null;
  personalRating?: number | null;
  startedAt?: string | null;
  completedAt?: string | null;
  privateNotes?: string | null;
}
