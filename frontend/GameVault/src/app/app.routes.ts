import { HomePagePublic } from './gameVault/features/home/pages/public-home/home-page-public';
import { GamePage } from './gameVault/features/games/pages/game-page/game-page';
import { PublicLayout } from './gameVault/layouts/public-layout/public-layout';
import { Routes } from '@angular/router';
import { AuthLayout } from './gameVault/layouts/auth-layout/auth-layout';
import { LoginPage } from './gameVault/features/auth/pages/login-page/login-page';
import { RegisterPage } from './gameVault/features/auth/pages/register-page/register-page';
import { GameDetailsPage } from './gameVault/features/games/pages/game-details-page/game-details-page';

export const routes: Routes = [
  {
    path: 'gamevault',
    component: PublicLayout,

    children: [
      {
        path: 'home',
        component: HomePagePublic,
      },

      {
        path: 'games',
        component: GamePage,
      },

      {
        path: 'gamesdetails/:id',
        component: GameDetailsPage,
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

    children: [
      {
        path: 'login',
        component: LoginPage,
      },

      {
        path: 'register',
        component: RegisterPage,
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
