import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BrandLogo } from '../../ui/brand-logo/brand-logo';

interface FooterLink {
  label: string;
  route: string;
  fragment?: string;
}

interface FooterColumn {
  title: string;
  links: FooterLink[];
}

@Component({
  selector: 'app-public-footer',
  imports: [RouterLink, BrandLogo],
  templateUrl: './public-footer.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PublicFooterComponent {
  protected readonly currentYear = new Date().getFullYear();

  /** Columnas de enlaces. Para añadir un enlace, solo se edita esta lista. */
  protected readonly columns: FooterColumn[] = [
    {
      title: 'Producto',
      links: [
        { label: 'Explorar juegos', route: '/gamevault/games' },
        { label: 'Tendencias', route: '/gamevault/home', fragment: 'trending' },
        { label: 'Lanzamientos', route: '/gamevault/home', fragment: 'upcoming' },
        { label: 'Biblioteca', route: '/gamevault/home', fragment: 'library' },
      ],
    },
    {
      title: 'Comunidad',
      links: [
        { label: 'Actividad', route: '/gamevault/home', fragment: 'community' },
        { label: 'Listas de jugadores', route: '/gamevault/home', fragment: 'lists' },
        { label: 'Crear cuenta', route: '/auth/register' },
      ],
    },
  ];

}
