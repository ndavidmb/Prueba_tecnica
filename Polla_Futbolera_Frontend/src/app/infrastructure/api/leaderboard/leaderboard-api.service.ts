import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import * as yup from 'yup';
import { environment } from '../../../../environments/environment';
import {
  LeaderboardEntry,
  UserHistory,
  leaderboardListSchema,
  userHistorySchema,
} from './leaderboard-api.types';

@Injectable({ providedIn: 'root' })
export class LeaderboardApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/leaderboard`;

  async getLeaderboard(): Promise<LeaderboardEntry[]> {
    const raw = await firstValueFrom(this.http.get(this.baseUrl));
    return this.mapper(raw, leaderboardListSchema);
  }

  async getUserHistory(userId: number): Promise<UserHistory> {
    const raw = await firstValueFrom(this.http.get(`${this.baseUrl}/users/${userId}/history`));
    return this.mapper(raw, userHistorySchema);
  }

  private mapper<T>(raw: unknown, schema: yup.Schema<T>): T {
    return schema.validateSync(raw, { strict: true, abortEarly: false });
  }
}
