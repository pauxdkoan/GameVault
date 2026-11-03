import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { GameService } from '../../../games/services/games.service';
import { ShowcaseCover } from '../../models/login.interfaces';

/** Cuántas portadas se piden a la API para el mural. */
const COVERS_TO_LOAD = 18;
/** Número de columnas del mural. */
const COLUMN_COUNT = 4;

/**
 * Panel decorativo de las pantallas de login y registro.
 *
 * Muestra un mural de portadas que se desplaza lentamente
 * (heredero de las "tarjetas flotantes" del diseño original)
 * y un mensaje de bienvenida encima.
 *
 * Si la API no responde, se pintan bloques de color para que
 * el panel nunca se vea vacío.
 */
@Component({
  selector: 'app-auth-showcase',
  templateUrl: './auth-showcase.html',
  styleUrl: './auth-showcase.css',
})
export class AuthShowcase implements OnInit {
  private readonly gameService = inject(GameService);

  private readonly covers = signal<ShowcaseCover[]>([]);

  /** Reparte las portadas en columnas: 0,3,6... / 1,4,7... / 2,5,8... */
  protected readonly columns = computed(() => {
    const covers = this.covers();
    return Array.from({ length: COLUMN_COUNT }, (_, column) =>
      covers.filter((_, index) => index % COLUMN_COUNT === column),
    );
  });

  protected readonly hasCovers = computed(() => this.covers().length > 0);

  /** Bloques de relleno cuando no hay portadas (6 por columna). */
  protected readonly placeholders = Array.from({ length: 6 }, (_, i) => i);

  protected readonly highlights = [
    'Descubre miles de juegos',
    'Organiza tu biblioteca',
    'Comparte reseñas con la comunidad',
  ];

  ngOnInit(): void {
    this.gameService.getGames({ page: 1, pageSize: COVERS_TO_LOAD }).subscribe({
      next: (response) => {
        const covers = response.games
          .filter((game) => !!game.coverImage)
          .map(
            (game): ShowcaseCover => ({
              id: game.externalId,
              name: game.name,
              cover: game.coverImage!,
            }),
          );

        this.covers.set(covers);
      },
      // Si falla nos quedamos con los bloques de relleno; no hace falta avisar.
      error: () => this.covers.set([]),
    });
  }
}
