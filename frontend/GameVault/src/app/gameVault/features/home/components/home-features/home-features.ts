import { ChangeDetectionStrategy, Component } from '@angular/core';
import { SectionHeader } from '../../../../shared/ui/section-header/section-header';
import { HOME_FEATURES } from '../../data/home-sample-data';

/** Sección "Mucho más que un catálogo": 4 tarjetas con lo que ofrece la app. */
@Component({
  selector: 'app-home-features',
  imports: [SectionHeader],
  templateUrl: './home-features.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeFeatures {
  protected readonly features = HOME_FEATURES;
}
