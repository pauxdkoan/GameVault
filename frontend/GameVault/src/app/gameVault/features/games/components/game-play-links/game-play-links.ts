import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { PlatformFamily } from '../../../../shared/models/game-card.model';
import { PlatformBadge } from '../../../../shared/ui/platform-badge/platform-badge';
import { normalizeText } from '../../../../shared/utils/text';
import { Platform, StoreLink, WebSite } from '../../models/games.interfaces';

interface PlayLink {
  label: string;
  url: string;
}

const MAX_LINKS = 4;

/**
 * Tarjeta "Dónde jugar": un logotipo por familia de plataforma y hasta 4 enlaces.
 * Prioriza las tiendas; si no hay ninguna, usa las webs oficiales.
 * Ahora mismo los enlaces están ocultos en la plantilla (ver game-play-links.html);
 * playLinks se deja calculado para volver a mostrarlos sin tocar este archivo.
 */
@Component({
  selector: 'app-game-play-links',
  imports: [PlatformBadge],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'block' },
  templateUrl: './game-play-links.html',
})
export class GamePlayLinks {
  readonly platforms = input<Platform[]>([]);
  readonly stores = input<StoreLink[]>([]);
  readonly webSites = input<WebSite[]>([]);

  /**
   * Una insignia por familia. El título (al pasar el ratón) lista las consolas
   * concretas: "PlayStation 4, PlayStation 5".
   */
  protected readonly platformBadges = computed(() => {
    const byFamily = new Map<PlatformFamily, string[]>();

    for (const platform of this.platforms()) {
      byFamily.set(platform.family, [...(byFamily.get(platform.family) ?? []), platform.name]);
    }

    return Array.from(byFamily, ([family, names]) => ({ family, title: names.join(', ') }));
  });

  protected readonly playLinks = computed<PlayLink[]>(() => {
    const storeLinks = this.stores()
      .filter((store) => hasHttpUrl(store.url))
      .map((store) => ({ label: store.storeName || 'Tienda', url: store.url }));

    if (storeLinks.length > 0) {
      return storeLinks.slice(0, MAX_LINKS);
    }

    return this.webSites()
      .filter((website) => hasHttpUrl(website.url))
      .map((website) => ({ label: websiteLabel(website), url: website.url }))
      .slice(0, MAX_LINKS);
  });
}

function hasHttpUrl(url: string | null | undefined): boolean {
  return typeof url === 'string' && /^https?:\/\//i.test(url);
}

/** Nombre de la web, o su dominio si el nombre falta o es una URL. */
function websiteLabel(website: WebSite): string {
  const name = normalizeText(website.name);

  if (name && !name.startsWith('http')) {
    return name;
  }

  try {
    return new URL(website.url).hostname.replace(/^www\./, '');
  } catch {
    return 'Sitio web';
  }
}
