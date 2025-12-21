import { Component, computed, effect, signal } from '@angular/core';
import { getCurrentWeekNumber } from '../../shared/utils/weekUtils';
import { TipsApiService } from '../../tips/services/tips-api';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// PrimeNG Imports
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageModule } from 'primeng/message';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { ToolbarModule } from 'primeng/toolbar';
import { TagModule } from 'primeng/tag';
import { ChartModule } from 'primeng/chart';
import { DividerModule } from 'primeng/divider';
import { TipDistributionDto } from '../../models/dtos/tips';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    FormsModule,
    ProgressSpinnerModule,
    MessageModule,
    CardModule,
    TableModule,
    ButtonModule,
    InputNumberModule,
    ToolbarModule,
    TagModule,
    ChartModule,
    DividerModule,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  selectedYear = signal(new Date().getFullYear());
  selectedWeekNumber = signal(getCurrentWeekNumber());

  currentWeekData = signal<TipDistributionDto | null>(null);
  weekData = signal<TipDistributionDto | null>(null);

  loading = signal(false);
  error = signal<string | null>(null);

  displayData = computed<TipDistributionDto | null>(
    () => this.weekData() ?? this.currentWeekData()
  );

  constructor(private tipsApi: TipsApiService) {
    effect(() => {
      const y = this.selectedYear();
      const w = this.selectedWeekNumber();
      this.loadWeek(y, w);
    });
  }

  decrementWeek(): void {
    if (this.selectedWeekNumber() > 1) {
      this.selectedWeekNumber.update((value) => value - 1);
    }
  }

  incrementWeek(): void {
    if (this.selectedWeekNumber() < 53) {
      this.selectedWeekNumber.update((value) => value + 1);
    }
  }

  async loadCurrentWeek() {
    this.loading.set(true);
    this.error.set(null);
    try {
      const data = await this.tipsApi.getCurrentWeekDistribution();
      this.currentWeekData.set(data);

      // Initialize selected week from current week (like your useEffect)
      this.selectedYear.set(data.year);
      this.selectedWeekNumber.set(data.weekNumber);
    } catch (e: any) {
      this.error.set(e?.message ?? 'Failed to load current week');
    } finally {
      this.loading.set(false);
    }
  }

  async loadWeek(year: number, weekNumber: number) {
    this.loading.set(true);
    this.error.set(null);

    try {
      const data = await this.tipsApi.getWeekDistribution({ year, weekNumber });
      this.weekData.set(data);
    } catch (e: any) {
      this.error.set(e?.message ?? 'Failed to load selected week');
    } finally {
      this.loading.set(false);
    }
  }
}
