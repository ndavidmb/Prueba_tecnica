import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import * as yup from 'yup';
import { environment } from '../../../../environments/environment';
import {
  Match,
  MatchResultUpdate,
  UpdateMatchResultRequest,
  matchListSchema,
  matchResultUpdateSchema,
} from './matches-api.types';

@Injectable({ providedIn: 'root' })
export class MatchesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  async getAll(): Promise<Match[]> {
    const raw = await firstValueFrom(this.http.get(`${this.baseUrl}/matches`));
    return this.mapper(raw, matchListSchema);
  }

  async updateResult(id: number, payload: UpdateMatchResultRequest): Promise<MatchResultUpdate> {
    const raw = await firstValueFrom(
      this.http.put(`${this.baseUrl}/admin/matches/${id}/result`, payload),
    );
    return this.mapper(raw, matchResultUpdateSchema);
  }

  private mapper<T>(raw: unknown, schema: yup.Schema<T>): T {
    return schema.validateSync(raw, { strict: true, abortEarly: false });
  }
}
