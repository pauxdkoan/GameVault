/** Una página de resultados de la API (PagedResultDto del backend). */
export interface PagedResult<T> {
  items: T[];
  count: number;
  page: number;
  pageSize: number;
}

/** Datos públicos mínimos de un usuario (autor de una reseña, dueño de una lista...). */
export interface UserSummary {
  userName: string;
  displayName: string;
  avatarUrl: string | null;
}
