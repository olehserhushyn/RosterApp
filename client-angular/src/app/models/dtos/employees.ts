import type { ShiftDto } from './shifts';

export interface EmployeeDetailsDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  createdAt: string;
  totalWeeklyHours: number;
  shifts: ShiftDto[];
}

export interface EmployeeDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  createdAt: string;
}
