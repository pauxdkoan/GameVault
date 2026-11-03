import {
  CommunityActivity,
  HomeFeature,
  PlayerList,
} from '../models/home-content.model';

/*
 * DATOS DE EJEMPLO
 * ----------------------------------------------------------------
 * Estas secciones del home todavía no tienen endpoint en el backend
 * (reseñas, actividad y listas de usuarios). Mientras tanto se usan
 * estos datos fijos para que el diseño se vea completo.
 *
 * Cuando exista la API, basta con sustituir estas constantes por una
 * llamada en HomeDataService: los componentes no cambian.
 */

export const HOME_FEATURES: HomeFeature[] = [
  {
    title: 'Descubre videojuegos',
    description:
      'Explora juegos por género, plataforma, valoración y fecha de lanzamiento.',
    iconPath: 'M11 4a7 7 0 1 0 0 14 7 7 0 0 0 0-14Zm10 17-4.35-4.35',
  },
  {
    title: 'Organiza tu biblioteca',
    description:
      'Clasifica tus juegos como pendientes, jugando, completados o abandonados.',
    iconPath: 'M4 4h6v6H4zM14 4h6v6h-6zM4 14h6v6H4zM14 14h6v6h-6z',
  },
  {
    title: 'Registra tu progreso',
    description:
      'Guarda tu avance, tiempo jugado, valoración personal y notas.',
    iconPath: 'M4 19h16M7 15l4-4 3 3 5-6',
  },
  {
    title: 'Comparte con la comunidad',
    description:
      'Publica reseñas, crea listas y descubre las recomendaciones de otros jugadores.',
    iconPath:
      'M16 19v-1a4 4 0 0 0-4-4H7a4 4 0 0 0-4 4v1M9.5 10a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7ZM21 19v-1a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75',
  },
];

export const COMMUNITY_ACTIVITY: CommunityActivity[] = [
  {
    id: 'a1',
    userName: 'Mariana Vega',
    initials: 'MV',
    action: 'completó',
    target: 'Kaze no Ronin',
    timeAgo: 'hace 2 h',
    quote: null,
    likes: 48,
    comments: 6,
  },
  {
    id: 'a2',
    userName: 'Diego Salas',
    initials: 'DS',
    action: 'agregó a su lista',
    target: 'Orbit Zero',
    timeAgo: 'hace 5 h',
    quote: null,
    likes: 21,
    comments: 3,
  },
  {
    id: 'a3',
    userName: 'Lucía Bernal',
    initials: 'LB',
    action: 'reseñó',
    target: 'Ashes of Tomorrow',
    timeAgo: 'hace 8 h',
    quote: 'Un final devastador y perfectamente escrito. El mejor ritmo narrativo del año.',
    likes: 132,
    comments: 27,
  },
  {
    id: 'a4',
    userName: 'Tomás Ruiz',
    initials: 'TR',
    action: 'creó la lista',
    target: 'Joyas indie de 2026',
    timeAgo: 'hace 1 d',
    quote: null,
    likes: 76,
    comments: 11,
  },
];

export const PLAYER_LISTS: PlayerList[] = [
  { id: 'l1', title: 'Mejores RPG modernos', author: '@ariko', gameCount: 24, followers: '3,1k' },
  { id: 'l2', title: 'Juegos para disfrutar con amigos', author: '@koba_dev', gameCount: 18, followers: '2,4k' },
  { id: 'l3', title: 'Historias que debes experimentar', author: '@luciab', gameCount: 31, followers: '5,6k' },
  { id: 'l4', title: 'Joyas independientes', author: '@pixelnomad', gameCount: 42, followers: '1,9k' },
  { id: 'l5', title: 'Para completar en un fin de semana', author: '@nova_tr', gameCount: 12, followers: '980' },
  { id: 'l6', title: 'Clásicos que siguen vigentes', author: '@retro_mv', gameCount: 27, followers: '4,2k' },
];
