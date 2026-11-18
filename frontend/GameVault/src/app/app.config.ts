import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './gameVault/core/interceptors/auth.interceptor';
import { errorInterceptor } from './gameVault/core/interceptors/error.interceptor';
import { AuthService } from './gameVault/features/auth/services/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    // Orden importante: errorInterceptor primero para que reciba la respuesta
    // al final, cuando authInterceptor ya intentó renovar el token ante un 401.
    provideHttpClient(withInterceptors([errorInterceptor, authInterceptor])),
    // Al arrancar, si hay sesión guardada, refresca los datos del usuario.
    // No bloquea la carga: la app se pinta y el usuario se actualiza después.
    provideAppInitializer(() => {
      inject(AuthService).loadCurrentUser().subscribe();
    }),
    provideRouter(
      routes,
      withInMemoryScrolling({
        anchorScrolling: 'enabled',
        scrollPositionRestoration: 'enabled',
      }),
    ),
  ],
};
