import { igdbRatingLabel } from './rating';

describe('igdbRatingLabel', () => {
  it('pasa la nota de IGDB (0-5) a la escala de 10', () => {
    expect(igdbRatingLabel(4.3)).toBe('8.6');
    expect(igdbRatingLabel(5)).toBe('10.0');
  });

  it('sin nota muestra S/N', () => {
    expect(igdbRatingLabel(0)).toBe('S/N');
    expect(igdbRatingLabel(null)).toBe('S/N');
    expect(igdbRatingLabel(undefined)).toBe('S/N');
  });
});
