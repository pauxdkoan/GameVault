/**
 * Notas de los juegos.
 *
 * El backend devuelve la nota de IGDB en una escala de 0 a 5, pero las reseñas
 * de GameVault van de 1 a 10. Para no mezclar escalas en pantalla, TODAS las
 * notas se muestran sobre 10. Esto solo cambia cómo se pinta el número:
 * el dato que llega de la API no se toca.
 */

/** Nota de IGDB (0-5) lista para mostrar sobre 10, p. ej. 4.3 → "8.6". Sin nota → "S/N". */
export function igdbRatingLabel(rating: number | null | undefined): string {
  return typeof rating === 'number' && rating > 0 ? (rating * 2).toFixed(1) : 'S/N';
}
