import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { BrandLogo } from '../../shared/ui/brand-logo/brand-logo';
import { AuthShowcase } from '../../features/auth/components/auth-showcase/auth-showcase';
import { Icon } from '../../shared/ui/icon/icon';

/**
 * Layout de las pantallas de autenticación.
 *
 * Pantalla dividida en dos:
 *  - Izquierda: logo + formulario (login o registro vía router-outlet).
 *  - Derecha: panel decorativo con portadas (solo en pantallas grandes).
 *
 * No contiene lógica: solo organiza la página.
 */
@Component({
  selector: 'app-auth-layout',
  imports: [RouterOutlet, RouterLink, BrandLogo, AuthShowcase, Icon],
  templateUrl: './auth-layout.html',
})
export class AuthLayout {
  protected readonly currentYear = new Date().getFullYear();
}
