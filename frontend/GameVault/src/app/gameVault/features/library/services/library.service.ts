import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { GameStatus, LibraryEntry, LibraryEntryRequest, LibraryPage } from '../models/library.models';

const API_URL = `${environment.gameVaultUrl}/me/library`;

/** Peticiones de "Mi biblioteca". Todas necesitan sesión (el token lo pone authInterceptor). */
@Injectable({ providedIn: 'root' })
export class LibraryService {
  private readonly http = inject(HttpClient);

  getLibrary(status: GameStatus | null, page = 1, pageSize = 24): Observable<LibraryPage> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);

    if (status) {
      params = params.set('status', status);
    }

    return this.http.get<LibraryPage>(API_URL, { params });
  }

  /** Añade el juego o actualiza su estado (el backend decide cuál de las dos). */
  saveEntry(externalId: number, request: LibraryEntryRequest): Observable<LibraryEntry> {
    return this.http.put<LibraryEntry>(`${API_URL}/${externalId}`, request);
  }

  removeEntry(externalId: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/${externalId}`);
  }
}
