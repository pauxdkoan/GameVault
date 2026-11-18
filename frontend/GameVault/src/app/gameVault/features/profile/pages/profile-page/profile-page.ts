import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { errorMessages } from '../../../../core/http/api-error';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { UserAvatar } from '../../../../shared/ui/user-avatar/user-avatar';
import { ListCard } from '../../../lists/components/list-card/list-card';
import { ReviewCard } from '../../../reviews/components/review-card/review-card';
import { ProfileStats } from '../../components/profile-stats/profile-stats';
import { UserProfile } from '../../models/profile.models';
import { ProfileService } from '../../services/profile.service';

/**
 * Perfil público (/gamevault/u/:userName): portada, avatar, bio, contadores,
 * últimas reseñas y listas públicas. Si es mi perfil, muestra "Editar perfil".
 */
@Component({
  selector: 'app-profile-page',
  imports: [RouterLink, VaultButton, UserAvatar, ListCard, ReviewCard, ProfileStats],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './profile-page.html',
})
export class ProfilePage implements OnInit {
  private readonly profileService = inject(ProfileService);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly profile = signal<UserProfile | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);

  private readonly memberSinceFormatter = new Intl.DateTimeFormat('es', { month: 'long', year: 'numeric' });

  ngOnInit(): void {
    // Igual que en el detalle de juego: al pasar de un perfil a otro Angular reutiliza el componente.
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      this.load(params.get('userName') ?? '');
    });
  }

  protected load(userName: string): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.profileService.getProfile(userName).subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.profile.set(null);
        this.error.set(errorMessages(error).join(' '));
        this.isLoading.set(false);
      },
    });
  }

  protected memberSince(value: string | null): string {
    return value ? `Miembro desde ${this.memberSinceFormatter.format(new Date(value))}` : '';
  }

  /** Al borrar una reseña desde mi propio perfil, la quitamos de la lista. */
  protected removeReview(reviewId: string): void {
    const profile = this.profile();
    if (profile) {
      this.profile.set({ ...profile, recentReviews: profile.recentReviews.filter((review) => review.id !== reviewId) });
    }
  }
}
