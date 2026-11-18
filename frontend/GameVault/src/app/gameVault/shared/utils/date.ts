/** Texto cuando el juego no tiene fecha o la fecha no es válida. */
export const UNKNOWN_RELEASE_LABEL = 'Fecha por confirmar';

const releaseFormatter = new Intl.DateTimeFormat('es', {
  day: 'numeric',
  month: 'short',
  year: 'numeric',
});

/** Convierte lo que llegue de la API en Date, o null si no es una fecha válida. */
export function parseDate(value: Date | string | null | undefined): Date | null {
  if (value === null || value === undefined || value === '') {
    return null;
  }

  // El backend guarda las fechas en UTC pero las envía sin "Z" (ej: "2026-11-04T16:00:00").
  // Sin la Z, JavaScript las leería como hora local y "hace 5 minutos" saldría mal.
  const text =
    typeof value === 'string' && /T\d{2}:\d{2}/.test(value) && !/(Z|[+-]\d{2}:\d{2})$/.test(value)
      ? `${value}Z`
      : value;

  const date = new Date(text);
  return Number.isNaN(date.getTime()) ? null : date;
}

/** Fecha de lanzamiento legible: "12 mar 2025" o "Fecha por confirmar". */
export function formatReleaseDate(value: Date | string | null | undefined): string {
  const date = parseDate(value);
  return date ? releaseFormatter.format(date) : UNKNOWN_RELEASE_LABEL;
}

/** Solo el año: "2025" o "Fecha por confirmar". */
export function formatReleaseYear(value: Date | string | null | undefined): string {
  const date = parseDate(value);
  return date ? date.getFullYear().toString() : UNKNOWN_RELEASE_LABEL;
}

const relativeFormatter = new Intl.RelativeTimeFormat('es', { numeric: 'auto' });

/** "hace 5 minutos", "ayer", "hace 3 meses"... para reseñas, comentarios y actividad. */
export function formatRelativeDate(value: Date | string | null | undefined): string {
  const date = parseDate(value);

  if (!date) {
    return '';
  }

  const seconds = Math.round((date.getTime() - Date.now()) / 1000);
  const steps: [Intl.RelativeTimeFormatUnit, number][] = [
    ['year', 31536000],
    ['month', 2592000],
    ['week', 604800],
    ['day', 86400],
    ['hour', 3600],
    ['minute', 60],
  ];

  for (const [unit, unitSeconds] of steps) {
    if (Math.abs(seconds) >= unitSeconds) {
      return relativeFormatter.format(Math.round(seconds / unitSeconds), unit);
    }
  }

  return 'ahora mismo';
}
