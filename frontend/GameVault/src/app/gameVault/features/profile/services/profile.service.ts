import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { AuthSessionService } from '../../../core/services/auth-session.service';
import { SessionUser } from '../../auth/models/login.interfaces';
import { UpdateProfileRequest, UserProfile } from '../models/profile.models';

const API_URL = environment.gameVaultUrl;

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(AuthSessionService);

  getProfile(userName: string): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${API_URL}/users/${encodeURIComponent(userName)}`);
  }

  /** Guarda el perfil y actualiza la sesión, para que el header muestre el nombre nuevo al momento. */
  updateMyProfile(request: UpdateProfileRequest): Observable<SessionUser> {
    return this.http
      .put<SessionUser>(`${API_URL}/me/profile`, request)
      .pipe(tap((user) => this.session.updateUser(user)));
  }
}
