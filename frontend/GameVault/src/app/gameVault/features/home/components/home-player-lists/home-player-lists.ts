import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { SectionHeader } from '../../../../shared/ui/section-header/section-header';
import { ListCard } from '../../../lists/components/list-card/list-card';
import { GameListSummary } from '../../../lists/models/list.models';
import { ListsService } from '../../../lists/services/lists.service';

/**
 * "Listas creadas por jugadores": las últimas listas públicas con juegos (GET /api/lists).
 * Reutiliza la misma tarjeta que "Mis listas" y el perfil.
 */
@Component({
  selector: 'app-home-player-lists',
  imports: [SectionHeader, ListCard, RouterLink, VaultButton],
  templateUrl: './home-player-lists.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePlayerLists implements OnInit {
  private readonly listsService = inject(ListsService);

  protected readonly lists = signal<GameListSummary[]>([]);
  protected readonly status = signal<'loading' | 'ready' | 'error'>('loading');

  ngOnInit(): void {
    this.listsService.getPublicLists(1, 6).subscribe({
      next: (page) => {
        this.lists.set(page.items);
        this.status.set('ready');
      },
      error: () => this.status.set('error'),
    });
  }
}
