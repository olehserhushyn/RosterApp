import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { TipDistributionDto, TipDistributionRequest } from '../../models/dtos/tips';

@Injectable({
  providedIn: 'root',
})
export class TipsApiService {
  constructor(private http: HttpClient) {}

  private baseUrl = 'https://localhost:44309/api';

  getWeekDistribution(params: TipDistributionRequest) {
    setTimeout(() => {}, 3000);
    return firstValueFrom(
      this.http.get<TipDistributionDto>(
        `${this.baseUrl}/tips/distribution/week/${params.year}/${params.weekNumber}`
      )
    );
  }

  getCurrentWeekDistribution() {
    return firstValueFrom(
      this.http.get<TipDistributionDto>(`${this.baseUrl}/tips/distribution/current`)
    );
  }
}
