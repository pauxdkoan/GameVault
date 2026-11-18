import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { Company } from '../../models/games.interfaces';

@Component({
  selector: 'app-game-companies',
  imports: [],
  templateUrl: './game-companies.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GameCompaniesComponent {
  readonly companies = input<Company[]>([]);

  protected readonly visibleCompanies = computed(() =>
    this.companies().filter((company) => company.name.trim() !== ''),
  );
  protected readonly developers = computed(() =>
    this.visibleCompanies().filter((company) => company.isDeveloper),
  );
  protected readonly publishers = computed(() =>
    this.visibleCompanies().filter((company) => company.isPublisher),
  );
  protected readonly otherCompanies = computed(() =>
    this.visibleCompanies().filter(
      (company) => !company.isDeveloper && !company.isPublisher,
    ),
  );
  protected readonly hasCompanies = computed(
    () => this.visibleCompanies().length > 0,
  );
}
