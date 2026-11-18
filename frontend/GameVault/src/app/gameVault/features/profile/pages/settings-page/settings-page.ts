import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthSessionService } from '../../../../core/services/auth-session.service';
import { errorMessages } from '../../../../core/http/api-error';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { FormField } from '../../../../shared/ui/form-field/form-field';
import { PageHeader } from '../../../../shared/ui/page-header/page-header';
import { UserAvatar } from '../../../../shared/ui/user-avatar/user-avatar';
import { ProfileService } from '../../services/profile.service';

/** Solo http(s), igual que el validador del backend. */
const HTTP_URL_PATTERN = /^https?:\/\/\S+$/i;

/**
 * Ajustes (/gamevault/settings, requiere sesión): editar nombre visible, bio,
 * país, avatar y portada. Usa formularios reactivos como el login y el registro.
 */
@Component({
  selector: 'app-settings-page',
  imports: [ReactiveFormsModule, RouterLink, VaultButton, FormField, PageHeader, UserAvatar],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './settings-page.html',
})
export class SettingsPage implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly profileService = inject(ProfileService);
  protected readonly session = inject(AuthSessionService);

  protected readonly form = this.fb.group({
    displayName: ['', [Validators.required, Validators.maxLength(50)]],
    bio: ['', Validators.maxLength(300)],
    country: ['', Validators.maxLength(60)],
    avatarUrl: ['', [Validators.maxLength(500), Validators.pattern(HTTP_URL_PATTERN)]],
    bannerUrl: ['', [Validators.maxLength(500), Validators.pattern(HTTP_URL_PATTERN)]],
  });

  protected readonly isSaving = signal(false);
  protected readonly saved = signal(false);
  protected readonly errors = signal<string[]>([]);

  ngOnInit(): void {
    const user = this.session.user();
    if (user) {
      this.form.setValue({
        displayName: user.displayName,
        bio: user.bio ?? '',
        country: user.country ?? '',
        avatarUrl: user.avatarUrl ?? '',
        bannerUrl: user.bannerUrl ?? '',
      });
    }
  }

  /** Mensaje de error de un campo, solo cuando el usuario ya lo ha tocado. */
  protected fieldError(name: keyof typeof this.form.controls): string | null {
    const control = this.form.controls[name];
    if (!control.touched || control.valid) return null;
    if (control.hasError('required')) return 'Este campo es obligatorio.';
    if (control.hasError('maxlength')) return `Máximo ${control.getError('maxlength').requiredLength} caracteres.`;
    if (control.hasError('pattern')) return 'Debe ser una URL que empiece por http:// o https://';
    return null;
  }

  protected submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    const value = this.form.getRawValue();
    const orNull = (text: string) => text.trim() || null;

    this.isSaving.set(true);
    this.saved.set(false);
    this.errors.set([]);

    this.profileService
      .updateMyProfile({
        displayName: value.displayName.trim(),
        bio: orNull(value.bio),
        country: orNull(value.country),
        avatarUrl: orNull(value.avatarUrl),
        bannerUrl: orNull(value.bannerUrl),
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.saved.set(true);
          this.form.markAsPristine();
        },
        error: (error: unknown) => {
          this.isSaving.set(false);
          this.errors.set(errorMessages(error));
        },
      });
  }
}
