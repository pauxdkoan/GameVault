import { formatRelativeDate, formatReleaseYear, parseDate } from './date';

describe('date utils', () => {
  it('parseDate trata como UTC las fechas del backend que llegan sin "Z"', () => {
    expect(parseDate('2026-11-04T16:00:00')?.toISOString()).toBe('2026-11-04T16:00:00.000Z');
    expect(parseDate('2026-11-04T16:00:00Z')?.toISOString()).toBe('2026-11-04T16:00:00.000Z');
  });

  it('parseDate devuelve null con valores vacíos o inválidos', () => {
    expect(parseDate(null)).toBeNull();
    expect(parseDate('')).toBeNull();
    expect(parseDate('no es una fecha')).toBeNull();
  });

  it('formatReleaseYear devuelve el año o el texto por confirmar', () => {
    expect(formatReleaseYear('2015-05-19')).toBe('2015');
    expect(formatReleaseYear(null)).toBe('Fecha por confirmar');
  });

  it('formatRelativeDate escribe "hace 5 minutos" y "ahora mismo"', () => {
    const fiveMinutesAgo = new Date(Date.now() - 5 * 60 * 1000).toISOString();

    expect(formatRelativeDate(fiveMinutesAgo)).toBe('hace 5 minutos');
    expect(formatRelativeDate(new Date())).toBe('ahora mismo');
    expect(formatRelativeDate(null)).toBe('');
  });
});
