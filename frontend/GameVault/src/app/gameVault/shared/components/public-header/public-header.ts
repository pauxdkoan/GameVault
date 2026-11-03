import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  HostListener,
  inject,
  input,
  signal,
} from '@angular/core';
import { NgClass } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { BrandLogo } from '../../ui/brand-logo/brand-logo';
import { VaultButton } from '../../ui/button/vault-button';
import { filter } from 'rxjs';

interface HeaderNavItem {
  label: string;
  route: string;
  fragment?: string;
}

interface ExploreMenuItem {
  label: string;
  description: string;
  route: string;
  fragment?: string;
}

@Component({
  selector: 'app-public-header',
  imports: [NgClass, RouterLink, BrandLogo, VaultButton],
  templateUrl: './public-header.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PublicHeaderComponent {
  readonly transparentMode = input(true);

  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly isMenuOpen = signal(false);
  protected readonly isExploreOpen = signal(false);
  protected readonly isScrolled = signal(false);
  protected readonly mobileMenuId = 'public-mobile-menu';
  protected readonly exploreMenuId = 'public-explore-menu';
  /** Enlaces principales (a la derecha de "Explorar"). */
  protected readonly navItems: HeaderNavItem[] = [
    { label: 'Juegos', route: '/gamevault/games' },
    { label: 'Comunidad', route: '/gamevault/home', fragment: 'community' },
  ];

  /** Opciones del desplegable "Explorar" (secciones del home). */
  protected readonly exploreItems: ExploreMenuItem[] = [
    {
      label: 'Todos los juegos',
      description: 'Catálogo completo con filtros',
      route: '/gamevault/games',
    },
    {
      label: 'Tendencias',
      description: 'Lo que más se está jugando esta semana',
      route: '/gamevault/home',
      fragment: 'trending',
    },
    {
      label: 'Próximos lanzamientos',
      description: 'Los estrenos que vienen en camino',
      route: '/gamevault/home',
      fragment: 'upcoming',
    },
    {
      label: 'Listas de jugadores',
      description: 'Colecciones curadas por la comunidad',
      route: '/gamevault/home',
      fragment: 'lists',
    },
  ];

  constructor() {
    this.router.events
      .pipe(
        filter(
          (event): event is NavigationEnd => event instanceof NavigationEnd,
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => this.closeHeaderMenus());
  }

  @HostListener('window:scroll')
  protected updateScrollState(): void {
    this.isScrolled.set(window.scrollY > 24);
  }

  @HostListener('document:keydown.escape')
  protected closeMenuFromEscape(): void {
    this.closeHeaderMenus();
  }

  protected toggleMenu(): void {
    this.isMenuOpen.update((isOpen) => {
      const nextIsOpen = !isOpen;

      if (nextIsOpen) {
        this.isExploreOpen.set(false);
      }

      return nextIsOpen;
    });
  }

  protected toggleExploreMenu(): void {
    this.isExploreOpen.update((isOpen) => !isOpen);
  }

  protected closeMenu(): void {
    this.isMenuOpen.set(false);
  }

  protected closeExploreMenu(): void {
    this.isExploreOpen.set(false);
  }

  protected closeHeaderMenus(): void {
    this.closeMenu();
    this.closeExploreMenu();
  }

  protected shouldUseSolidHeader(): boolean {
    return (
      !this.transparentMode() ||
      this.isScrolled() ||
      this.isMenuOpen() ||
      this.isExploreOpen()
    );
  }

  protected headerSurfaceClasses(): string {
    return this.shouldUseSolidHeader()
      ? 'border-vault-border/70 bg-vault-bg/80 backdrop-blur-xl'
      : 'border-transparent bg-transparent';
  }
}
