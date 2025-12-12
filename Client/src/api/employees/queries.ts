import { useQuery } from '@tanstack/react-query';
import agent from '../agent';
import type { EmployeeDetailsDto } from '../../types/dtos/employees';

export const EMPLOYEES_QUERY_KEYS = {
  all: ['employees'] as const,
  detail: (id: number) => ['employees', id] as const,
};

export function useEmployeeDetails(employeeId: number) {
  return useQuery({
    queryKey: EMPLOYEES_QUERY_KEYS.detail(employeeId),
    queryFn: async () => {
      const response = await agent.get<EmployeeDetailsDto>(`/employees/${employeeId}`);
      return response.data;
    },
    enabled: !!employeeId,
  });
}
