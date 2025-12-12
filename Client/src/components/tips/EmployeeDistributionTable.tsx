import {
  Card,
  CardContent,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Box,
  useTheme,
} from '@mui/material';
import type { EmployeeTipShareDto } from '../../types/dtos/tips';

interface EmployeeDistributionTableProps {
  employeeShares: EmployeeTipShareDto[];
  totalHours: number;
  totalTips: string;
  totalEmployees: number;
  currencySymbol: string;
  onEmployeeClick: (employeeId: number) => void;
}

export function EmployeeDistributionTable({
  employeeShares,
  totalHours,
  totalTips,
  totalEmployees,
  currencySymbol,
  onEmployeeClick,
}: EmployeeDistributionTableProps) {
  const theme = useTheme();

  return (
    <Card elevation={2} sx={{ borderRadius: 2, overflow: 'hidden' }}>
      <CardContent sx={{ p: 0 }}>
        <Box sx={{ p: 3, pb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Employee Distribution
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Tip shares based on hours worked
          </Typography>
        </Box>

        <TableContainer component={Paper} elevation={0}>
          <Table>
            <TableHead>
              <TableRow sx={{ backgroundColor: theme.palette.grey[50] }}>
                <TableCell sx={{ fontWeight: 600 }}>Employee</TableCell>
                <TableCell align="right" sx={{ fontWeight: 600 }}>
                  Hours
                </TableCell>
                <TableCell align="right" sx={{ fontWeight: 600 }}>
                  Hourly Rate
                </TableCell>
                <TableCell align="right" sx={{ fontWeight: 600 }}>
                  Tip Share
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {employeeShares.map((employee) => {
                const hourlyRate = employee.shareAmount / employee.hoursWorked;

                return (
                  <TableRow
                    key={employee.employeeId}
                    onClick={() => onEmployeeClick(employee.employeeId)}
                    hover
                    sx={{ '&:last-child td': { borderBottom: 0 }, cursor: 'pointer' }}
                  >
                    <TableCell>
                      <Box>
                        <Typography variant="body1" sx={{ fontWeight: 500 }}>
                          {employee.employeeName}
                        </Typography>
                      </Box>
                    </TableCell>
                    <TableCell align="right">
                      <Typography sx={{ fontWeight: 500 }}>
                        {employee.hoursWorked.toFixed(1)}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Typography sx={{ fontWeight: 500 }}>
                        {currencySymbol}
                        {hourlyRate.toFixed(2)}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Typography sx={{ fontWeight: 600, color: theme.palette.success.dark }}>
                        {currencySymbol}
                        {employee.shareAmount.toFixed(2)}
                      </Typography>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>

        {/* Table Footer */}
        <Box
          sx={{
            p: 2,
            backgroundColor: theme.palette.grey[50],
            borderTop: `1px solid ${theme.palette.divider}`,
          }}
        >
          <Box display="flex" justifyContent="space-between" alignItems="center">
            <Typography variant="body2" color="text.secondary">
              {totalEmployees} employees
            </Typography>
            <Box display="flex" gap={3}>
              <Typography variant="body2">
                <Box component="span" sx={{ fontWeight: 600 }}>
                  Total Hours:{' '}
                </Box>
                {totalHours.toFixed(1)}
              </Typography>
              <Typography variant="body2">
                <Box component="span" sx={{ fontWeight: 600 }}>
                  Total Distributed:{' '}
                </Box>
                {totalTips}
              </Typography>
            </Box>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
