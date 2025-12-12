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

  // Get current week data for initial load
  const { data: currentWeekData, isLoading: isLoadingCurrentWeek } =
    useCurrentWeekTipDistribution();

  // Get data for the selected week
  const {
    data,
    isLoading: isLoadingWeek,
    isError,
    error,
    refetch,
    isFetching,
  } = useWeekTipDistribution({
    weekNumber: selectedWeekNumber,
    year: selectedYear,
  });

  const isLoading = isLoadingCurrentWeek || isLoadingWeek || isFetching;

  // Initialize with current week when currentWeekData is available
  useEffect(() => {
    if (currentWeekData) {
      setSelectedYear(currentWeekData.year);
      setSelectedWeekNumber(currentWeekData.weekNumber);
    }
  }, [currentWeekData]);

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
    const currentYear = new Date().getFullYear();
    const currentWeek = getCurrentWeekNumber();
    const maxWeeksInYear = getWeeksInYear(selectedYear);

    // Only allow navigation to future weeks if we're in current year and week
    if (selectedYear === currentYear && selectedWeekNumber === currentWeek) {
      return; // Can't go to future
    }

    if (selectedWeekNumber < maxWeeksInYear) {
      setSelectedWeekNumber(selectedWeekNumber + 1);
    } else {
      // Go to first week of next year
      setSelectedYear(selectedYear + 1);
      setSelectedWeekNumber(1);
    }
  };

  const handleWeekChange = (weekOffset: number) => {
    const currentWeek = getCurrentWeekNumber();
    const currentYear = new Date().getFullYear();

    if (selectedYear === currentYear) {
      // For current year, calculate week number from offset
      const weekNumber = weekOffset === 0 ? currentWeek : currentWeek - weekOffset;
      setSelectedWeekNumber(Math.max(1, weekNumber));
    } else {
      // For other years, calculate week number from offset
      const maxWeeks = getWeeksInYear(selectedYear);
      const weekNumber = maxWeeks - weekOffset + 1;
      setSelectedWeekNumber(Math.max(1, weekNumber));
    }
  };

  const handleYearChange = (year: number) => {
    const currentYear = new Date().getFullYear();
    const currentWeek = getCurrentWeekNumber();

    setSelectedYear(year);

    // When changing year, set appropriate week
    if (year === currentYear) {
      setSelectedWeekNumber(currentWeek);
    } else if (year < currentYear) {
      // For past years, set to last week
      setSelectedWeekNumber(getWeeksInYear(year));
    } else {
      // For future years, set to first week
      setSelectedWeekNumber(1);
    }
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

  // Use the data from the selected week, fall back to currentWeekData if needed
  const displayData = data || currentWeekData;

  if (isLoading && !displayData) {
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

  if (!displayData) {
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

  // Calculate week range
  const startDate = new Date(displayData.weekStartDate);
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
  const totalTipsFormatted = `${displayData.currencySymbol}${displayData.totalTips.toFixed(2)}`;
  const averagePerHour = displayData.totalTips / displayData.totalHours;
  const totalEmployees = displayData.employeeShares.length;

  // Calculate the week offset for the WeekNavigation component
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

  // Generate available years
  const getAvailableYears = () => {
    const currentYear = new Date().getFullYear();
    const years = [];
    for (let i = currentYear - 5; i <= currentYear; i++) {
      years.push(i);
    }
    return years;
  };

  const isCurrentWeek =
    selectedYear === new Date().getFullYear() && selectedWeekNumber === getCurrentWeekNumber();

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
            weekNumber={displayData.weekNumber}
            year={displayData.year}
            weekRange={weekRange}
            totalTips={totalTipsFormatted}
            totalHours={displayData.totalHours}
            averagePerHour={averagePerHour}
            totalEmployees={totalEmployees}
            currencySymbol={displayData.currencySymbol}
            currencyCode={displayData.currencyCode}
            isCurrentWeek={isCurrentWeek}
          />
        </Grid>

        <Grid item xs={12} md={8}>
          <EmployeeDistributionTable
            employeeShares={displayData.employeeShares}
            totalHours={displayData.totalHours}
            totalTips={totalTipsFormatted}
            totalEmployees={totalEmployees}
            currencySymbol={displayData.currencySymbol}
            onEmployeeClick={handleEmployeeClick}
          />
        </Grid>
      </Grid>
    </Box>
  );
}
