import { register } from 'swiper/element/bundle';

let registered = false;

/**
 * Registra <swiper-container> y <swiper-slide> la primera vez que se necesitan.
 *
 * Antes se registraba en main.ts, y Swiper (unos 100 kB) entraba en la carga
 * inicial aunque la página no tuviera carruseles (login, biblioteca...).
 * Ahora cada componente con carrusel llama a esta función en su constructor,
 * y Swiper viaja en el trozo de código diferido de esas páginas.
 * El flag evita registrar dos veces (el navegador lanzaría un error).
 */
export function ensureSwiperRegistered(): void {
  if (!registered) {
    register();
    registered = true;
  }
}
