import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthSessionService } from '../services/auth-session.service';

/**
 * Protege las rutas privadas (biblioteca, perfil...).
 * Sin sesión, manda al login y recuerda a dónde quería ir el usuario.
 *
 * Uso en las rutas:  { path: 'library', canActivate: [authGuard], ... }
 */
export const authGuard: CanActivateFn = (_route, state) => {
  const session = inject(AuthSessionService);
  const router = inject(Router);

  return session.isLoggedIn()
    ? true
    : router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: state.url },
      });
};

/**
 * Lo contrario: si ya hay sesión, no tiene sentido ver login o registro.
 */
export const guestGuard: CanActivateFn = () => {
  const session = inject(AuthSessionService);
  const router = inject(Router);

  return session.isLoggedIn() ? router.createUrlTree(['/gamevault/home']) : true;
};
