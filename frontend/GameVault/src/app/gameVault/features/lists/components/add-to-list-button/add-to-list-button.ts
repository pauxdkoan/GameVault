import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthSessionService } from '../../../../core/services/auth-session.service';
import { errorMessages } from '../../../../core/http/api-error';
import { GameListSummary } from '../../models/list.models';
import { ListsService } from '../../services/lists.service';
import { Icon } from '../../../../shared/ui/icon/icon';

/**
 * Botón "Añadir a una lista" del detalle de un juego.
 * Carga mis listas al abrirlo (no antes) y avisa del resultado debajo.
 */
@Component({
  selector: 'app-add-to-list-button',
  imports: [Icon, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './add-to-list-button.html',
  host: { '(document:keydown.escape)': 'isOpen.set(false)' },
})
export class AddToListButton {
  private readonly listsService = inject(ListsService);
  private readonly session = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly externalId = input.required<number>();

  protected readonly isOpen = signal(false);
  protected readonly lists = signal<GameListSummary[] | null>(null);
  protected readonly message = signal<{ text: string; isError: boolean } | null>(null);
  protected readonly busyListId = signal<string | null>(null);

  protected toggle(): void {
    if (!this.session.isLoggedIn()) {
      this.router.navigate(['/auth/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    this.isOpen.update((open) => !open);
    this.message.set(null);

    if (this.isOpen() && this.lists() === null) {
      this.listsService.getMyLists().subscribe({
        next: (lists) => this.lists.set(lists),
        error: (error: unknown) => this.showError(error),
      });
    }
  }

  protected add(list: GameListSummary): void {
    this.busyListId.set(list.id);

    this.listsService.addGame(list.id, this.externalId()).subscribe({
      next: () => {
        this.busyListId.set(null);
        this.isOpen.set(false);
        this.message.set({ text: `Añadido a "${list.name}".`, isError: false });
      },
      error: (error: unknown) => {
        this.busyListId.set(null);
        this.showError(error);
      },
    });
  }

  private showError(error: unknown): void {
    this.message.set({ text: errorMessages(error).join(' '), isError: true });
  }
}
