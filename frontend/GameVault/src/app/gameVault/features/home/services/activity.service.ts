import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { PagedResult } from '../../../shared/models/api.models';
import { Activity } from '../models/activity.models';

/** Actividad reciente de la comunidad, para el home. */
@Injectable({ providedIn: 'root' })
export class ActivityService {
  private readonly http = inject(HttpClient);

  getActivity(page = 1, pageSize = 6): Observable<PagedResult<Activity>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<Activity>>(`${environment.gameVaultUrl}/activity`, { params });
  }
}
