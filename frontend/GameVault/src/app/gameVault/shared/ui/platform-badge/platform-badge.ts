import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { PlatformFamily } from '../../models/game-card.model';
import { PLATFORM_FAMILIES } from './platform-families';

/**
 * Cuadradito con el logotipo de una familia de plataforma.
 * `title` permite detallar las consolas concretas ("PlayStation 4, PlayStation 5").
 *
 *   <app-platform-badge family="PlayStation" />
 *   <app-platform-badge family="PC" size="lg" title="PC (Microsoft Windows)" />
 */
@Component({
  selector: 'app-platform-badge',
  imports: [FontAwesomeModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <span
      class="grid shrink-0 place-items-center rounded-md border"
      [class]="style().toneClass + (size() === 'lg' ? ' size-11 text-base' : ' size-8 text-sm')"
      [attr.title]="title() || style().label"
      role="img"
      [attr.aria-label]="style().label"
    >
      <fa-icon [icon]="style().icon" [fixedWidth]="true" />
    </span>
  `,
})
export class PlatformBadge {
  readonly family = input.required<PlatformFamily>();
  readonly size = input<'sm' | 'lg'>('sm');
  readonly title = input('');

  protected readonly style = computed(() => PLATFORM_FAMILIES[this.family()] ?? PLATFORM_FAMILIES.Other);
}
