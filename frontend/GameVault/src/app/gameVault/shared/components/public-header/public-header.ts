import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  computed,
  HostListener,
  inject,
  input,
  signal,
} from '@angular/core';
import { NgClass } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { BrandLogo } from '../../ui/brand-logo/brand-logo';
import { AuthSessionService } from '../../../core/services/auth-session.service';
import { AuthService } from '../../../features/auth/services/auth.service';
import { VaultButton } from '../../ui/button/vault-button';
import { filter } from 'rxjs';
import { Icon } from '../../ui/icon/icon';
import { GameSearch } from '../../../features/games/components/game-search/game-search';

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
  imports: [Icon, NgClass, RouterLink, BrandLogo, VaultButton, GameSearch],
  templateUrl: './public-header.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PublicHeaderComponent {
  readonly transparentMode = input(true);

  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly authService = inject(AuthService);
  protected readonly session = inject(AuthSessionService);

  protected readonly isMenuOpen = signal(false);
  protected readonly isExploreOpen = signal(false);
  protected readonly isUserMenuOpen = signal(false);
  protected readonly userMenuId = 'public-user-menu';

  /** Opciones del menú de usuario (escritorio y móvil). "Mi perfil" depende del userName de la sesión. */
  protected readonly userMenuItems = computed(() => [
    { label: 'Mi biblioteca', route: '/gamevault/library' },
    { label: 'Mis listas', route: '/gamevault/lists' },
    { label: 'Mi perfil', route: `/gamevault/u/${this.session.user()?.userName ?? ''}` },
    { label: 'Ajustes', route: '/gamevault/settings' },
  ]);
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

  protected toggleUserMenu(): void {
    this.isUserMenuOpen.update((isOpen) => !isOpen);
    this.isExploreOpen.set(false);
  }

  protected closeHeaderMenus(): void {
    this.closeMenu();
    this.closeExploreMenu();
    this.isUserMenuOpen.set(false);
  }

  protected logout(): void {
    this.closeHeaderMenus();
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/gamevault/home']);
    });
  }

  protected shouldUseSolidHeader(): boolean {
    return (
      !this.transparentMode() ||
      this.isScrolled() ||
      this.isMenuOpen() ||
      this.isExploreOpen() ||
      this.isUserMenuOpen()
    );
  }

  protected headerSurfaceClasses(): string {
    return this.shouldUseSolidHeader()
      ? 'border-vault-border bg-vault-bg/95'
      : // Encima de imágenes (portada del perfil, héroe): una sombra oscura detrás
        // del menú para que el texto siempre se lea.
        'border-transparent bg-gradient-to-b from-vault-bg/90 via-vault-bg/50 to-transparent';
  }
}
