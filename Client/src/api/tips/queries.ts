import { useQuery } from '@tanstack/react-query';
import agent from '../agent';
import type { TipDistributionDto, TipDistributionRequest } from '../../types/dtos/tips';

export const TIPS_QUERY_KEYS = {
  currentWeek: ['tips', 'current-week'] as const,
  week: (params: TipDistributionRequest) => ['tips', 'week', params] as const,
};

export function useCurrentWeekTipDistribution() {
  return useQuery({
    queryKey: TIPS_QUERY_KEYS.currentWeek,
    queryFn: async () => {
      const response = await agent.get<TipDistributionDto>('/tips/distribution/current');
      return response.data;
    },
  });
}

export function useWeekTipDistribution(params: TipDistributionRequest) {
  return useQuery({
    queryKey: TIPS_QUERY_KEYS.week(params),
    queryFn: async () => {
      const response = await agent.get<TipDistributionDto>(
        `/tips/distribution/week/${params.year}/${params.weekNumber}`
      );
      return response.data;
    },
    enabled: !!params.year && !!params.weekNumber,
    retry: 3,
  });
}
