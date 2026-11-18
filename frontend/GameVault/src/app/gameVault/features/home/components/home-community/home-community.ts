import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { RatingStars } from '../../../../shared/ui/rating-stars/rating-stars';
import { SectionHeader } from '../../../../shared/ui/section-header/section-header';
import { UserAvatar } from '../../../../shared/ui/user-avatar/user-avatar';
import { formatRelativeDate } from '../../../../shared/utils/date';
import { Activity } from '../../models/activity.models';
import { ActivityService } from '../../services/activity.service';
import { Icon } from '../../../../shared/ui/icon/icon';

/**
 * "Descubre lo que juega la comunidad": últimas reseñas, juegos completados
 * y listas nuevas (GET /api/activity). Carga sus propios datos, así si la
 * API falla solo esta sección muestra el aviso.
 */
@Component({
  selector: 'app-home-community',
  imports: [Icon, SectionHeader, RouterLink, UserAvatar, RatingStars, VaultButton],
  templateUrl: './home-community.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeCommunity implements OnInit {
  private readonly activityService = inject(ActivityService);

  protected readonly activities = signal<Activity[]>([]);
  protected readonly status = signal<'loading' | 'ready' | 'error'>('loading');
  protected readonly timeAgo = formatRelativeDate;

  ngOnInit(): void {
    this.activityService.getActivity(1, 6).subscribe({
      next: (page) => {
        this.activities.set(page.items);
        this.status.set('ready');
      },
      error: () => this.status.set('error'),
    });
  }

  /** Verbo de cada tipo de evento: "reseñó", "completó", "creó la lista". */
  protected actionLabel(activity: Activity): string {
    switch (activity.type) {
      case 'Review':
        return 'reseñó';
      case 'Completed':
        return 'completó';
      default:
        return 'creó la lista';
    }
  }
}
