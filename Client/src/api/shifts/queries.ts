import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import agent from '../agent';
import { EMPLOYEES_QUERY_KEYS } from '../employees/queries';
import type { CreateShiftRequest, ShiftDto, UpdateShiftRequest } from '../../types/dtos/shifts';

export const SHIFTS_QUERY_KEYS = {
  all: ['shifts'] as const,
  employee: (employeeId: number) => ['shifts', 'employee', employeeId] as const,
  detail: (id: number) => ['shifts', id] as const,
};

// export function useEmployeeShifts(employeeId: number) {
//   return useQuery({
//     queryKey: SHIFTS_QUERY_KEYS.employee(employeeId),
//     queryFn: async () => {
//       const response = await agent.get<ShiftDto[]>(`/shifts/employee/${employeeId}`);
//       return response.data;
//     },
//     enabled: !!employeeId,
//   });
// }

export function useCreateShift() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: CreateShiftRequest) => {
      const response = await agent.post<ShiftDto>('/shifts', data);
      return response.data;
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({
        queryKey: SHIFTS_QUERY_KEYS.employee(data.employeeId),
      });
      queryClient.invalidateQueries({
        queryKey: EMPLOYEES_QUERY_KEYS.detail(data.employeeId),
      });
    },
  });
}

export function useUpdateShift() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, data }: { id: number; data: UpdateShiftRequest }) => {
      const response = await agent.put<ShiftDto>(`/shifts/${id}`, data);
      return response.data;
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({
        queryKey: SHIFTS_QUERY_KEYS.employee(data.employeeId),
      });
      queryClient.invalidateQueries({
        queryKey: EMPLOYEES_QUERY_KEYS.detail(data.employeeId),
      });
    },
  });
}

export function useDeleteShift() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: number) => {
      await agent.delete(`/shifts/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: SHIFTS_QUERY_KEYS.all,
      });
      queryClient.invalidateQueries({
        queryKey: EMPLOYEES_QUERY_KEYS.all,
      });
    },
  });
}
