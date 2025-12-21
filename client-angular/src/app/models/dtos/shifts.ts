export interface ShiftDto {
  id: number;
  employeeId: number;
  date: string;
  startTime: string;
  endTime: string;
  notes: string | null;
  hoursWorked: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateShiftRequest {
  employeeId: number;
  date: string;
  startTime: string;
  endTime: string;
  notes?: string;
}

export interface UpdateShiftRequest {
  date: string;
  startTime: string;
  endTime: string;
  notes?: string;
}
