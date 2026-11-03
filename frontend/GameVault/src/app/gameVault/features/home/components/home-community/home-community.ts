import { ChangeDetectionStrategy, Component } from '@angular/core';
import { SectionHeader } from '../../../../shared/ui/section-header/section-header';
import { COMMUNITY_ACTIVITY } from '../../data/home-sample-data';

/**
 * "Descubre lo que juega la comunidad".
 * Usa datos de ejemplo hasta que el backend tenga reseñas y actividad.
 */
@Component({
  selector: 'app-home-community',
  imports: [SectionHeader],
  templateUrl: './home-community.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeCommunity {
  protected readonly activities = COMMUNITY_ACTIVITY;

  /** Alterna colores de avatar para que no sean todos iguales. */
  protected avatarClasses(index: number): string {
    return index % 2 === 0
      ? 'bg-vault-cyan/15 text-vault-cyan'
      : 'bg-vault-violet/15 text-vault-violet';
  }
}
