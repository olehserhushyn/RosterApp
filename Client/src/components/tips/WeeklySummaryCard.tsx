import { Card, CardContent, Typography, Box, useTheme } from '@mui/material';
import { AttachMoney } from '@mui/icons-material';

interface WeeklySummaryCardProps {
  weekNumber: number;
  year: number;
  weekRange: string;
  totalTips: string;
  totalHours: number;
  averagePerHour: number;
  totalEmployees: number;
  currencySymbol: string;
  currencyCode: string;
  isCurrentWeek: boolean;
}

export function WeeklySummaryCard({
  weekNumber,
  year,
  weekRange,
  totalTips,
  totalHours,
  averagePerHour,
  totalEmployees,
  currencySymbol,
  currencyCode,
  isCurrentWeek,
}: WeeklySummaryCardProps) {
  const theme = useTheme();

  return (
    <Card elevation={3} sx={{ height: '100%', borderRadius: 2 }}>
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
          <AttachMoney fontSize="small" color="primary" />
          <Typography variant="overline" color="text.secondary">
            Week {weekNumber}, {year}
          </Typography>
        </Box>

        <Typography variant="h6" color="text.secondary" gutterBottom>
          {weekRange}
        </Typography>

        <Typography variant="h3" sx={{ fontWeight: 700, mb: 3 }}>
          {totalTips}
        </Typography>

        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
          <Typography variant="body2" color="text.secondary">
            Total Hours:
          </Typography>
          <Typography variant="body2" sx={{ fontWeight: 500 }}>
            {totalHours.toFixed(1)}
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
          <Typography variant="body2" color="text.secondary">
            Avg per Hour:
          </Typography>
          <Typography variant="body2" sx={{ fontWeight: 500 }}>
            {currencySymbol}
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

        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
          <Typography variant="body2" color="text.secondary">
            Currency:
          </Typography>
          <Typography variant="body2" sx={{ fontWeight: 500 }}>
            {currencyCode}
          </Typography>
        </Box>

        {/* Status indicator for current/previous weeks */}
        {isCurrentWeek && (
          <Box
            sx={{
              mt: 3,
              p: 1,
              backgroundColor: theme.palette.success.light,
              borderRadius: 1,
              display: 'flex',
              alignItems: 'center',
              gap: 1,
            }}
          >
            <Typography variant="caption" sx={{ color: theme.palette.success.dark }}>
              ✓ Current Week
            </Typography>
          </Box>
        )}
        {!isCurrentWeek && (
          <Box
            sx={{
              mt: 3,
              p: 1,
              backgroundColor: theme.palette.grey[100],
              borderRadius: 1,
              display: 'flex',
              alignItems: 'center',
              gap: 1,
            }}
          >
            <Typography variant="caption" color="text.secondary">
              Historical Week
            </Typography>
          </Box>
        )}
      </CardContent>
    </Card>
  );
}
