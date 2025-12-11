import { useQuery } from '@tanstack/react-query';
import agent from '../agent';
import type { TipDistributionDto } from '../../types/dtos/tips';

export const TIPS_QUERY_KEYS = {
  currentWeek: ['tips', 'current-week'] as const,
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
