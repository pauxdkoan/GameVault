import { GameCardDto } from '../models/game-card.model';
import { toGameCard, toGameCards } from './game-card.mapper';

/** Tarjeta de la API con valores normales; cada prueba cambia solo lo que le interesa. */
function dto(overrides: Partial<GameCardDto> = {}): GameCardDto {
  return {
    externalId: 1942,
    slug: 'the-witcher-3',
    name: 'The Witcher 3',
    summary: 'Un brujo busca a su hija adoptiva.',
    released: '2015-05-19',
    coverImage: 'https://img/cover.jpg',
    screenshots: ['https://img/shot1.jpg', 'https://img/shot2.jpg'],
    trailerVideo: null,
    rating: 4.6,
    metacritic: 93,
    genres: ['RPG', 'Aventura'],
    platforms: ['PC', 'PlayStation'],
    ...overrides,
  };
}

describe('toGameCard', () => {
  it('copia los datos principales', () => {
    const card = toGameCard(dto());

    expect(card.id).toBe('1942');
    expect(card.externalId).toBe(1942);
    expect(card.name).toBe('The Witcher 3');
    expect(card.coverImageUrl).toBe('https://img/cover.jpg');
    expect(card.screenshotUrl).toBe('https://img/shot1.jpg');
    expect(card.rating).toBe(4.6);
    expect(card.platforms).toEqual(['PC', 'PlayStation']);
  });

  it('usa textos de respaldo cuando faltan nombre, slug o géneros', () => {
    const card = toGameCard(dto({ name: '', slug: '', genres: [] }));

    expect(card.name).toBe('Juego sin nombre');
    expect(card.slug).toBe('game-1942');
    expect(card.genres).toEqual(['Sin género']);
  });

  it('muestra como máximo 3 géneros', () => {
    const card = toGameCard(dto({ genres: ['A', 'B', 'C', 'D'] }));

    expect(card.genres).toEqual(['A', 'B', 'C']);
  });

  it('usa la primera captura si no hay portada, y la portada si no hay capturas', () => {
    expect(toGameCard(dto({ coverImage: null })).coverImageUrl).toBe('https://img/shot1.jpg');

    const withoutShots = toGameCard(dto({ screenshots: [] }));
    expect(withoutShots.screenshotUrl).toBe('https://img/cover.jpg');
    expect(withoutShots.screenshotUrls).toEqual(['https://img/cover.jpg']);
  });

  it('quita familias de plataforma repetidas', () => {
    const card = toGameCard(dto({ platforms: ['PC', 'PlayStation', 'PC'] }));

    expect(card.platforms).toEqual(['PC', 'PlayStation']);
  });

  it('pone nota 0 y fecha por confirmar cuando no vienen', () => {
    const card = toGameCard(dto({ rating: null, released: null }));

    expect(card.rating).toBe(0);
    expect(card.releaseLabel).toBe('Fecha por confirmar');
  });
});

describe('toGameCards', () => {
  it('devuelve una lista vacía si la API no manda juegos', () => {
    expect(toGameCards(null)).toEqual([]);
    expect(toGameCards(undefined)).toEqual([]);
  });

  it('mapea cada juego de la lista', () => {
    expect(toGameCards([dto(), dto({ externalId: 7 })]).map((card) => card.id)).toEqual(['1942', '7']);
  });
});
