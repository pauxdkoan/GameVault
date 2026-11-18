import { PublicLayout } from './gameVault/layouts/public-layout/public-layout';
import { Routes } from '@angular/router';
import { AuthLayout } from './gameVault/layouts/auth-layout/auth-layout';
import { authGuard, guestGuard } from './gameVault/core/guards/auth.guard';

/*
 * Todas las páginas se cargan con loadComponent (carga diferida): el navegador
 * solo descarga el código de una pantalla cuando alguien entra en ella.
 * Los layouts (header, footer) sí van en la carga inicial porque se ven siempre.
 */
export const routes: Routes = [
  {
    path: 'gamevault',
    component: PublicLayout,

    children: [
      {
        path: 'home',
        loadComponent: () =>
          import('./gameVault/features/home/pages/public-home/home-page-public').then((m) => m.HomePagePublic),
      },

      {
        path: 'games',
        loadComponent: () =>
          import('./gameVault/features/games/pages/game-page/game-page').then((m) => m.GamePage),
      },

      {
        path: 'gamesdetails/:id',
        loadComponent: () =>
          import('./gameVault/features/games/pages/game-details-page/game-details-page').then((m) => m.GameDetailsPage),
      },

      // Pantallas de usuario (fase 4).
      {
        path: 'library',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./gameVault/features/library/pages/library-page/library-page').then((m) => m.LibraryPage),
      },

      {
        path: 'lists',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./gameVault/features/lists/pages/my-lists-page/my-lists-page').then((m) => m.MyListsPage),
      },

      // El detalle de una lista es público (si la lista lo es).
      {
        path: 'lists/:id',
        loadComponent: () =>
          import('./gameVault/features/lists/pages/list-detail-page/list-detail-page').then((m) => m.ListDetailPage),
      },

      {
        path: 'u/:userName',
        loadComponent: () =>
          import('./gameVault/features/profile/pages/profile-page/profile-page').then((m) => m.ProfilePage),
      },

      {
        path: 'settings',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./gameVault/features/profile/pages/settings-page/settings-page').then((m) => m.SettingsPage),
      },

      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'home',
      },
    ],
  },

  {
    path: 'auth',
    component: AuthLayout,
    canActivate: [guestGuard],

    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./gameVault/features/auth/pages/login-page/login-page').then((m) => m.LoginPage),
      },

      {
        path: 'register',
        loadComponent: () =>
          import('./gameVault/features/auth/pages/register-page/register-page').then((m) => m.RegisterPage),
      },

      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'login',
      },
    ],
  },

  {
    path: '**',
    redirectTo: 'gamevault',
  },
];
