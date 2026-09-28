import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BetResult, CreateBetRequest, betResultSchema } from './bets-api.types';

@Injectable({ providedIn: 'root' })
export class BetsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/bets`;

  async placeBet(payload: CreateBetRequest): Promise<BetResult> {
    const raw = await firstValueFrom(this.http.post(this.baseUrl, payload));
    return this.mapper(raw);
  }

  private mapper(raw: unknown): BetResult {
    return betResultSchema.validateSync(raw, { strict: true, abortEarly: false });
  }
}
