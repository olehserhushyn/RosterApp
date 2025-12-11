export interface EmployeeTipShareDto {
  employeeId: number;
  employeeName: string;
  hoursWorked: number;
  shareAmount: number;
  sharePercentage: number;
}

export interface TipDistributionDto {
  weekNumber: number;
  year: number;
  weekStartDate: string;
  totalTips: number;
  currencyCode: string;
  currencySymbol: string;
  totalHours: number;
  employeeShares: EmployeeTipShareDto[];
}
