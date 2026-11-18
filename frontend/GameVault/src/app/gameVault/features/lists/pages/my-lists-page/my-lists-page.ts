import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { errorMessages } from '../../../../core/http/api-error';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { PageHeader } from '../../../../shared/ui/page-header/page-header';
import { ListCard } from '../../components/list-card/list-card';
import { ListForm } from '../../components/list-form/list-form';
import { GameListSummary } from '../../models/list.models';
import { ListsService } from '../../services/lists.service';

/**
 * "Mis listas" (/gamevault/lists, requiere sesión).
 * Con ?nueva=1 (enlace del botón "Añadir a una lista") abre directamente el formulario.
 */
@Component({
  selector: 'app-my-lists-page',
  imports: [PageHeader, ListCard, ListForm, VaultButton],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './my-lists-page.html',
})
export class MyListsPage implements OnInit {
  private readonly listsService = inject(ListsService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly lists = signal<GameListSummary[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly formOpen = signal(false);

  ngOnInit(): void {
    this.formOpen.set(this.route.snapshot.queryParamMap.get('nueva') === '1');
    this.load();
  }

  protected load(): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.listsService.getMyLists().subscribe({
      next: (lists) => {
        this.lists.set(lists);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.error.set(errorMessages(error).join(' '));
        this.isLoading.set(false);
      },
    });
  }

  /** Al crear una lista vamos directo a ella. */
  protected onCreated(list: GameListSummary): void {
    this.router.navigate(['/gamevault/lists', list.id]);
  }

  protected closeForm(): void {
    this.formOpen.set(false);
    // Quitamos ?nueva=1 para que al recargar no vuelva a abrirse.
    this.router.navigate([], { queryParams: {}, replaceUrl: true });
  }
}
