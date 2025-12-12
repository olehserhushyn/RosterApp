import {
  Card,
  CardContent,
  CircularProgress,
  Grid,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Box,
  Alert,
  useTheme,
} from '@mui/material';
import { AttachMoney } from '@mui/icons-material';
import { useCurrentWeekTipDistribution } from '../api/tips/queries.ts';
import { useNavigate } from 'react-router-dom';

export function WeeklyTipsPage() {
  const { data, isLoading, isError, error } = useCurrentWeekTipDistribution();
  const theme = useTheme();
  const navigate = useNavigate();

  if (isLoading) {
    return (
      <Box
        display="flex"
        flexDirection="column"
        alignItems="center"
        justifyContent="center"
        minHeight="60vh"
        gap={3}
      >
        <CircularProgress size={60} />
        <Typography variant="h6" color="text.secondary">
          Loading tip distribution...
        </Typography>
      </Box>
    );
  }

  if (isError) {
    return (
      <Alert severity="error">
        <Typography variant="h6" gutterBottom>
          Failed to load data
        </Typography>
        <Typography variant="body2">Error: {(error as Error).message}</Typography>
      </Alert>
    );
  }

  if (!data) {
    return (
      <Alert severity="info">
        <Typography variant="h6">No data available</Typography>
        <Typography variant="body2">
          There's no tip distribution data for the current week.
        </Typography>
      </Alert>
    );
  }

  const weekStartDate = new Date(data.weekStartDate).toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
  const totalTipsFormatted = `${data.currencySymbol}${data.totalTips.toFixed(2)}`;
  const averagePerHour = data.totalTips / data.totalHours;
  const totalEmployees = data.employeeShares.length;

  return (
    <Box>
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom sx={{ fontWeight: 700 }}>
          Weekly Tip Distribution
        </Typography>
        <Typography variant="body1" color="text.secondary">
          View how tips are distributed among employees for the current week
        </Typography>
      </Box>

      {/* Main Content - Card and Table */}
      <Grid container spacing={3}>
        {/* Summary Card */}
        <Grid item xs={12} md={4}>
          <Card elevation={3} sx={{ height: '100%', borderRadius: 2 }}>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                <AttachMoney fontSize="small" color="primary" />
                <Typography variant="overline" color="text.secondary">
                  Week {data.weekNumber}, {data.year}
                </Typography>
              </Box>

              <Typography variant="h6" color="text.secondary" gutterBottom>
                {weekStartDate}
              </Typography>

              <Typography variant="h3" sx={{ fontWeight: 700, mb: 3 }}>
                {totalTipsFormatted}
              </Typography>

              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                <Typography variant="body2" color="text.secondary">
                  Total Hours:
                </Typography>
                <Typography variant="body2" sx={{ fontWeight: 500 }}>
                  {data.totalHours.toFixed(1)}
                </Typography>
              </Box>

              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                <Typography variant="body2" color="text.secondary">
                  Avg per Hour:
                </Typography>
                <Typography variant="body2" sx={{ fontWeight: 500 }}>
                  {data.currencySymbol}
                  {averagePerHour.toFixed(2)}
                </Typography>
              </Box>

              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                <Typography variant="body2" color="text.secondary">
                  Employees:
                </Typography>
                <Typography variant="body2" sx={{ fontWeight: 500 }}>
                  {totalEmployees}
                </Typography>
              </Box>

              <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Typography variant="body2" color="text.secondary">
                  Currency:
                </Typography>
                <Typography variant="body2" sx={{ fontWeight: 500 }}>
                  {data.currencyCode}
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Distribution Table */}
        <Grid item xs={12} md={8}>
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
                    {data.employeeShares.map((employee) => {
                      const hourlyRate = employee.shareAmount / employee.hoursWorked;

                      return (
                        <TableRow
                          key={employee.employeeId}
                          onClick={() => navigate(`employees/${employee.employeeId}`)}
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
                              {data.currencySymbol}
                              {hourlyRate.toFixed(2)}
                            </Typography>
                          </TableCell>
                          <TableCell align="right">
                            <Typography sx={{ fontWeight: 600, color: theme.palette.success.dark }}>
                              {data.currencySymbol}
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
                      {data.totalHours.toFixed(1)}
                    </Typography>
                    <Typography variant="body2">
                      <Box component="span" sx={{ fontWeight: 600 }}>
                        Total Distributed:{' '}
                      </Box>
                      {totalTipsFormatted}
                    </Typography>
                  </Box>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
