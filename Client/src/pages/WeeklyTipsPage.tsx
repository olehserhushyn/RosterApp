import { Grid, Box, Typography, CircularProgress, Alert, Button } from '@mui/material';
import { useCurrentWeekTipDistribution, useWeekTipDistribution } from '../api/tips/queries.ts';
import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { WeekNavigation } from '../components/tips/WeekNavigation.tsx';
import { WeeklySummaryCard } from '../components/tips/WeeklySummaryCard.tsx';
import { EmployeeDistributionTable } from '../components/tips/EmployeeDistributionTable.tsx';
import { getCurrentWeekNumber, getWeeksInYear } from '../utils/weekUtils';

export function WeeklyTipsPage() {
  const [selectedYear, setSelectedYear] = useState<number>(new Date().getFullYear());
  const [selectedWeekNumber, setSelectedWeekNumber] = useState<number>(getCurrentWeekNumber());
  const navigate = useNavigate();

  const { data: currentWeekData } = useCurrentWeekTipDistribution();

  const { data, isLoading, isError, error, refetch } = useWeekTipDistribution({
    weekNumber: selectedWeekNumber,
    year: selectedYear,
  });

  useEffect(() => {
    if (currentWeekData && !data) {
      setSelectedYear(currentWeekData.year);
      setSelectedWeekNumber(currentWeekData.weekNumber);
    }
  }, [currentWeekData, data]);

  const handlePreviousWeek = () => {
    if (selectedWeekNumber > 1) {
      setSelectedWeekNumber(selectedWeekNumber - 1);
    } else {
      // Go to last week of previous year
      setSelectedYear(selectedYear - 1);
      setSelectedWeekNumber(getWeeksInYear(selectedYear - 1));
    }
  };

  const handleNextWeek = () => {
    const maxWeeksInYear = getWeeksInYear(selectedYear);
    if (selectedWeekNumber < maxWeeksInYear) {
      setSelectedWeekNumber(selectedWeekNumber + 1);
    } else {
      setSelectedYear(selectedYear + 1);
      setSelectedWeekNumber(1);
    }
  };

  const handleWeekChange = (weekOffset: number) => {
    const currentWeek = getCurrentWeekNumber();
    const currentYear = new Date().getFullYear();

    if (selectedYear === currentYear) {
      const weekNumber = weekOffset === 0 ? currentWeek : currentWeek - weekOffset;
      setSelectedWeekNumber(weekNumber);
    } else {
      const maxWeeks = getWeeksInYear(selectedYear);
      const weekNumber = maxWeeks - weekOffset + 1;
      setSelectedWeekNumber(weekNumber);
    }
  };

  const handleYearChange = (year: number) => {
    setSelectedYear(year);
    setSelectedWeekNumber(year === new Date().getFullYear() ? getCurrentWeekNumber() : 1);
  };

  const handleCurrentWeek = () => {
    const currentWeek = getCurrentWeekNumber();
    const currentYear = new Date().getFullYear();
    setSelectedYear(currentYear);
    setSelectedWeekNumber(currentWeek);
  };

  const handleEmployeeClick = (employeeId: number) => {
    navigate(`employees/${employeeId}`);
  };

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
        <Button onClick={() => refetch()} sx={{ mt: 1 }}>
          Retry
        </Button>
      </Alert>
    );
  }

  if (!data) {
    return (
      <Alert severity="info">
        <Typography variant="h6">No data available</Typography>
        <Typography variant="body2">
          There's no tip distribution data for the selected week.
        </Typography>
        <Button onClick={handleCurrentWeek} sx={{ mt: 1 }}>
          Go to Current Week
        </Button>
      </Alert>
    );
  }

  const startDate = new Date(data.weekStartDate);
  const endDate = new Date(startDate);
  endDate.setDate(startDate.getDate() + 6);

  const formatDate = (date: Date) => {
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  };

  const weekRange = `${formatDate(startDate)} - ${formatDate(endDate)}`;
  const totalTipsFormatted = `${data.currencySymbol}${data.totalTips.toFixed(2)}`;
  const averagePerHour = data.totalTips / data.totalHours;
  const totalEmployees = data.employeeShares.length;

  const calculateWeekOffset = () => {
    const currentWeek = getCurrentWeekNumber();
    const currentYear = new Date().getFullYear();

    if (selectedYear === currentYear) {
      return selectedWeekNumber === currentWeek ? 0 : currentWeek - selectedWeekNumber;
    } else {
      const maxWeeksInSelectedYear = getWeeksInYear(selectedYear);
      return maxWeeksInSelectedYear - selectedWeekNumber + 1;
    }
  };

  const getAvailableYears = () => {
    const currentYear = new Date().getFullYear();
    const years = [];
    for (let i = currentYear - 5; i <= currentYear + 1; i++) {
      years.push(i);
    }
    return years;
  };

  return (
    <Box>
      <Box sx={{ mb: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
            Weekly Tip Distribution
          </Typography>
        </Box>

        <WeekNavigation
          weekRange={weekRange}
          weekNumber={selectedWeekNumber}
          year={selectedYear}
          selectedWeekOffset={calculateWeekOffset()}
          selectedYear={selectedYear}
          onPreviousWeek={handlePreviousWeek}
          onNextWeek={handleNextWeek}
          onWeekChange={handleWeekChange}
          onYearChange={handleYearChange}
          onCurrentWeek={handleCurrentWeek}
          isLoading={isLoading}
          totalWeeksInYear={getWeeksInYear(selectedYear)}
          currentWeekNumber={getCurrentWeekNumber()}
          currentYear={new Date().getFullYear()}
          availableYears={getAvailableYears()}
        />

        <Typography variant="body1" color="text.secondary" sx={{ mt: 2 }}>
          View how tips are distributed among employees for the selected week
        </Typography>
      </Box>

      <Grid container spacing={3}>
        <Grid item xs={12} md={4}>
          <WeeklySummaryCard
            weekNumber={data.weekNumber}
            year={data.year}
            weekRange={weekRange}
            totalTips={totalTipsFormatted}
            totalHours={data.totalHours}
            averagePerHour={averagePerHour}
            totalEmployees={totalEmployees}
            currencySymbol={data.currencySymbol}
            currencyCode={data.currencyCode}
            isCurrentWeek={
              selectedYear === new Date().getFullYear() &&
              selectedWeekNumber === getCurrentWeekNumber()
            }
          />
        </Grid>

        <Grid item xs={12} md={8}>
          <EmployeeDistributionTable
            employeeShares={data.employeeShares}
            totalHours={data.totalHours}
            totalTips={totalTipsFormatted}
            totalEmployees={totalEmployees}
            currencySymbol={data.currencySymbol}
            onEmployeeClick={handleEmployeeClick}
          />
        </Grid>
      </Grid>
    </Box>
  );
}
