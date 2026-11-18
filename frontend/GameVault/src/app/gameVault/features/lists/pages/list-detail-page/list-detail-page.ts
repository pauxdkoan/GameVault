import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { errorMessages } from '../../../../core/http/api-error';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { UserAvatar } from '../../../../shared/ui/user-avatar/user-avatar';
import { formatRelativeDate } from '../../../../shared/utils/date';
import { ListForm } from '../../components/list-form/list-form';
import { GameListDetail, GameListSummary } from '../../models/list.models';
import { ListsService } from '../../services/lists.service';

/**
 * Detalle de una lista (/gamevault/lists/:id).
 * Cualquiera puede ver una lista pública; el dueño además puede editarla,
 * borrarla y quitar juegos.
 */
@Component({
  selector: 'app-list-detail-page',
  imports: [RouterLink, VaultButton, UserAvatar, ListForm],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './list-detail-page.html',
})
export class ListDetailPage implements OnInit {
  private readonly listsService = inject(ListsService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly list = signal<GameListDetail | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly isEditing = signal(false);
  protected readonly confirmDelete = signal(false);
  protected readonly timeAgo = formatRelativeDate;
  /** Juegos cuya portada no cargó: se pinta su inicial en lugar de la imagen rota. */
  protected readonly failedCovers = signal<ReadonlySet<number>>(new Set());

  protected markCoverFailed(externalId: number): void {
    this.failedCovers.update((failed) => new Set(failed).add(externalId));
  }

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      this.load(params.get('id') ?? '');
    });
  }

  protected load(listId: string): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.listsService.getList(listId).subscribe({
      next: (list) => {
        this.list.set(list);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.error.set(errorMessages(error).join(' '));
        this.isLoading.set(false);
      },
    });
  }

  protected onEdited(summary: GameListSummary): void {
    const list = this.list();
    if (list) {
      this.list.set({ ...list, name: summary.name, description: summary.description, isPublic: summary.isPublic });
    }
    this.isEditing.set(false);
  }

  protected removeGame(externalId: number): void {
    const list = this.list();
    if (!list) return;

    this.listsService.removeGame(list.id, externalId).subscribe({
      next: () =>
        this.list.set({ ...list, items: list.items.filter((item) => item.game.externalId !== externalId) }),
      error: (error: unknown) => this.error.set(errorMessages(error).join(' ')),
    });
  }

  protected deleteList(): void {
    const list = this.list();
    if (!list) return;

    this.listsService.delete(list.id).subscribe({
      next: () => this.router.navigate(['/gamevault/lists']),
      error: (error: unknown) => this.error.set(errorMessages(error).join(' ')),
    });
  }
}
