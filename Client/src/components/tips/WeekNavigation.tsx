import {
  Box,
  Typography,
  IconButton,
  Button,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  useTheme,
  Tooltip,
  Divider,
} from '@mui/material';
import {
  ChevronLeft,
  ChevronRight,
  CalendarToday,
  Today,
  ExpandMore,
  ExpandLess,
} from '@mui/icons-material';
import { useState, useEffect } from 'react';
import { getCurrentWeekNumber, getWeeksInYear } from '../../utils/weekUtils';

interface WeekNavigationProps {
  weekRange: string;
  weekNumber: number;
  year: number;
  selectedWeekOffset: number;
  selectedYear: number;
  onPreviousWeek: () => void;
  onNextWeek: () => void;
  onWeekChange: (weekOffset: number) => void;
  onYearChange: (year: number) => void;
  onCurrentWeek: () => void;
  isLoading: boolean;
  totalWeeksInYear?: number;
  currentWeekNumber?: number;
  currentYear?: number;
  availableYears?: number[];
}

export function WeekNavigation({
  weekRange,
  selectedWeekOffset,
  selectedYear,
  onPreviousWeek,
  onNextWeek,
  onWeekChange,
  onYearChange,
  onCurrentWeek,
  isLoading,
  currentWeekNumber,
  currentYear,
  availableYears,
}: WeekNavigationProps) {
  const theme = useTheme();
  const [weeksInYear, setWeeksInYear] = useState<Array<{ value: number; label: string }>>([]);
  const [years, setYears] = useState<number[]>([]);

  const currentWeek = currentWeekNumber || getCurrentWeekNumber();
  const now = new Date();
  const currentYearValue = currentYear || now.getFullYear();

  const maxWeeksInSelectedYear = getWeeksInYear(selectedYear);
  const isCurrentYear = selectedYear === currentYearValue;

  useEffect(() => {
    if (availableYears && availableYears.length > 0) {
      setYears(availableYears);
    } else {
      const yearsList = [];
      for (let y = currentYearValue - 5; y <= currentYearValue + 1; y++) {
        yearsList.push(y);
      }
      setYears(yearsList);
    }
  }, [availableYears, currentYearValue]);

  useEffect(() => {
    const weekOptions = [];
    const maxWeeks = isCurrentYear ? currentWeek : maxWeeksInSelectedYear;

    if (isCurrentYear) {
      weekOptions.push({
        value: 0,
        label: `Current Week (Week ${currentWeek})`,
      });
      for (let week = currentWeek - 1; week >= 1; week--) {
        weekOptions.push({
          value: currentWeek - week,
          label: `Week ${week}, ${selectedYear}`,
        });
      }
    } else {
      // For past/future years, add all weeks in descending order
      for (let week = maxWeeks; week >= 1; week--) {
        const offset = isCurrentYear ? currentWeek - week : maxWeeks - week + 1;
        weekOptions.push({
          value: offset,
          label: `Week ${week}, ${selectedYear}`,
        });
      }
    }

    setWeeksInYear(weekOptions);
  }, [selectedYear, currentWeek, isCurrentYear, maxWeeksInSelectedYear]);

  const handleWeekChange = (event: any) => {
    onWeekChange(parseInt(event.target.value));
  };

  const handleYearChange = (event: any) => {
    const newYear = parseInt(event.target.value);
    onYearChange(newYear);
  };

  const getSelectedWeekLabel = () => {
    if (selectedWeekOffset === 0 && isCurrentYear) {
      return `Week ${currentWeek}, ${selectedYear}`;
    }

    let selectedWeek: number;
    if (isCurrentYear) {
      selectedWeek = currentWeek - selectedWeekOffset;
    } else {
      selectedWeek = maxWeeksInSelectedYear - selectedWeekOffset + 1;
    }

    return `Week ${selectedWeek}, ${selectedYear}`;
  };

  const canGoNextWeek = selectedWeekOffset > 0;

  const canGoPreviousWeek =
    selectedWeekOffset < (isCurrentYear ? currentWeek - 1 : maxWeeksInSelectedYear - 1);

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        backgroundColor: theme.palette.grey[50],
        p: 2,
        borderRadius: 1,
        border: `1px solid ${theme.palette.divider}`,
        mb: 3,
      }}
    >
      {/* Left Navigation */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <Tooltip title="Previous week">
          <span>
            <IconButton
              onClick={onPreviousWeek}
              disabled={!canGoPreviousWeek || isLoading}
              size="small"
            >
              <ChevronLeft />
            </IconButton>
          </span>
        </Tooltip>

        <Tooltip title="Go to current week">
          <span>
            <Button
              variant={selectedWeekOffset === 0 && isCurrentYear ? 'contained' : 'outlined'}
              size="small"
              onClick={onCurrentWeek}
              disabled={(selectedWeekOffset === 0 && isCurrentYear) || isLoading}
              startIcon={<Today fontSize="small" />}
            >
              Now
            </Button>
          </span>
        </Tooltip>

        <Tooltip title="Next week">
          <span>
            <IconButton onClick={onNextWeek} disabled={!canGoNextWeek || isLoading} size="small">
              <ChevronRight />
            </IconButton>
          </span>
        </Tooltip>
      </Box>

      {/* Center Display */}
      <Box sx={{ textAlign: 'center', minWidth: 300 }}>
        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          {weekRange}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {getSelectedWeekLabel()}
          {selectedWeekOffset === 0 && isCurrentYear && (
            <Box
              component="span"
              sx={{
                ml: 1,
                px: 1,
                py: 0.25,
                backgroundColor: theme.palette.success.light,
                borderRadius: 1,
                fontSize: '0.75rem',
                color: theme.palette.success.dark,
              }}
            >
              Current
            </Box>
          )}
          {!isCurrentYear && (
            <Box
              component="span"
              sx={{
                ml: 1,
                px: 1,
                py: 0.25,
                backgroundColor: theme.palette.grey[300],
                borderRadius: 1,
                fontSize: '0.75rem',
                color: theme.palette.grey[700],
              }}
            >
              {selectedYear < currentYearValue ? 'Past Year' : 'Future Year'}
            </Box>
          )}
        </Typography>

        {/* Week progress indicator - only show for current year */}
        {isCurrentYear && (
          <Box sx={{ mt: 1, display: 'flex', alignItems: 'center', gap: 1 }}>
            <Box
              sx={{
                flexGrow: 1,
                height: 4,
                backgroundColor: theme.palette.grey[300],
                borderRadius: 2,
              }}
            >
              <Box
                sx={{
                  width: `${
                    ((selectedWeekOffset === 0 ? currentWeek : currentWeek - selectedWeekOffset) /
                      maxWeeksInSelectedYear) *
                    100
                  }%`,
                  height: '100%',
                  backgroundColor:
                    selectedWeekOffset === 0 ? theme.palette.primary.main : theme.palette.grey[500],
                  borderRadius: 2,
                }}
              />
            </Box>
            <Typography variant="caption" color="text.secondary">
              Week {selectedWeekOffset === 0 ? currentWeek : currentWeek - selectedWeekOffset} of{' '}
              {maxWeeksInSelectedYear}
            </Typography>
          </Box>
        )}
      </Box>

      {/* Right Week and Year Selector */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <CalendarToday fontSize="small" color="action" />

          {/* Year Selector */}
          <FormControl size="small" sx={{ minWidth: 120 }}>
            <InputLabel>Year</InputLabel>
            <Select
              value={selectedYear}
              onChange={handleYearChange}
              label="Year"
              disabled={isLoading}
            >
              {years.map((yearOption) => (
                <MenuItem
                  key={yearOption}
                  value={yearOption}
                  sx={{
                    fontWeight: yearOption === currentYearValue ? 600 : 400,
                    backgroundColor:
                      yearOption === currentYearValue
                        ? theme.palette.action.selected
                        : 'transparent',
                  }}
                >
                  {yearOption}
                  {yearOption === currentYearValue && ' (Current)'}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <Divider orientation="vertical" flexItem sx={{ mx: 1 }} />

          {/* Week Selector */}
          <FormControl size="small" sx={{ minWidth: 200 }}>
            <InputLabel>Week</InputLabel>
            <Select
              value={selectedWeekOffset}
              onChange={handleWeekChange}
              label="Week"
              disabled={isLoading}
              MenuProps={{
                PaperProps: {
                  style: {
                    maxHeight: 300,
                  },
                },
              }}
            >
              {weeksInYear.map((option) => (
                <MenuItem
                  key={option.value}
                  value={option.value}
                  sx={{
                    fontWeight: option.value === 0 && isCurrentYear ? 600 : 400,
                    backgroundColor:
                      option.value === 0 && isCurrentYear
                        ? theme.palette.action.selected
                        : 'transparent',
                  }}
                >
                  {option.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Box>
      </Box>
    </Box>
  );
}
