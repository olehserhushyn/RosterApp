import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  Alert,
  CircularProgress,
  Typography,
  Grid,
} from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { TimePicker } from '@mui/x-date-pickers/TimePicker';
import type { ShiftDto, CreateShiftRequest, UpdateShiftRequest } from '../../types/dtos/shifts';

interface ShiftDialogProps {
  open: boolean;
  mode: 'create' | 'edit';
  shift: ShiftDto | null;
  onSubmit: (data: CreateShiftRequest | UpdateShiftRequest) => Promise<void>;
  isLoading: boolean;
  error: Error | null;
  onClose: () => void;
}

export function ShiftDialog({
  open,
  mode,
  shift,
  onSubmit,
  isLoading,
  error,
  onClose,
}: ShiftDialogProps) {
  const [date, setDate] = useState<Date | null>(new Date());
  const [startTime, setStartTime] = useState<Date | null>(new Date(new Date().setHours(9, 0, 0)));
  const [endTime, setEndTime] = useState<Date | null>(new Date(new Date().setHours(17, 0, 0)));
  const [notes, setNotes] = useState('');

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (shift && mode === 'edit') {
      setDate(new Date(shift.date));
      setStartTime(new Date(`2000-01-01T${shift.startTime}`));
      setEndTime(new Date(`2000-01-01T${shift.endTime}`));
      setNotes(shift.notes || '');
    } else {
      resetForm();
    }
  }, [shift, mode, open]);

  const resetForm = () => {
    setDate(new Date());
    setStartTime(new Date(new Date().setHours(9, 0, 0)));
    setEndTime(new Date(new Date().setHours(17, 0, 0)));
    setNotes('');
    setErrors({});
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!date) {
      newErrors.date = 'Date is required';
    }

    if (!startTime) {
      newErrors.startTime = 'Start time is required';
    }

    if (!endTime) {
      newErrors.endTime = 'End time is required';
    }

    if (startTime && endTime && startTime >= endTime) {
      newErrors.endTime = 'End time must be after start time';
    }

    if (startTime && endTime) {
      const diffMs = endTime.getTime() - startTime.getTime();
      const diffHours = diffMs / (1000 * 60 * 60);
      if (diffHours > 24) {
        newErrors.endTime = 'Shift cannot exceed 24 hours';
      }
      if (diffHours <= 0) {
        newErrors.endTime = 'Shift must have positive duration';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const formatDateToApi = (date: Date) => {
    return date.toISOString().split('T')[0];
  };

  const formatTimeToApi = (date: Date) => {
    const timeString = date.toTimeString().split(' ')[0];
    // Ensure format is HH:mm:ss
    return timeString.includes(':') ? timeString : `${timeString}:00`;
  };

  const handleSubmit = async () => {
    if (!validateForm() || !date || !startTime || !endTime) {
      return;
    }

    const formData = {
      date: formatDateToApi(date),
      startTime: formatTimeToApi(startTime),
      endTime: formatTimeToApi(endTime),
      notes: notes || undefined,
    };

    try {
      await onSubmit(formData);
    } catch (err) {
      // Error is handled by parent component
    }
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const calculateHours = () => {
    if (!startTime || !endTime) return 0;
    const diffMs = endTime.getTime() - startTime.getTime();
    return diffMs / (1000 * 60 * 60);
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{mode === 'create' ? 'Add New Shift' : 'Edit Shift'}</DialogTitle>

      <DialogContent>
        <Box sx={{ pt: 2 }}>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error.message}
            </Alert>
          )}

          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <DatePicker
                  label="Date"
                  value={date}
                  onChange={setDate}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      error: !!errors.date,
                      helperText: errors.date,
                    },
                  }}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TimePicker
                  label="Start Time"
                  value={startTime}
                  onChange={setStartTime}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      error: !!errors.startTime,
                      helperText: errors.startTime,
                    },
                  }}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TimePicker
                  label="End Time"
                  value={endTime}
                  onChange={setEndTime}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      error: !!errors.endTime,
                      helperText: errors.endTime,
                    },
                  }}
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Notes (Optional)"
                  multiline
                  rows={3}
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  placeholder="Add any notes about this shift..."
                />
              </Grid>

              <Grid item xs={12}>
                <Box
                  sx={{
                    p: 2,
                    bgcolor: 'grey.50',
                    borderRadius: 1,
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                  }}
                >
                  <Typography variant="body2">Total Hours:</Typography>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>
                    {calculateHours().toFixed(1)} hours
                  </Typography>
                </Box>
              </Grid>
            </Grid>
          </LocalizationProvider>
        </Box>
      </DialogContent>

      <DialogActions>
        <Button onClick={handleClose} disabled={isLoading}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={isLoading}
          startIcon={isLoading && <CircularProgress size={20} />}
        >
          {isLoading ? 'Saving...' : mode === 'create' ? 'Add Shift' : 'Update Shift'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
