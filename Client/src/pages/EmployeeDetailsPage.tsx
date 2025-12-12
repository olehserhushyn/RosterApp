import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Grid,
  Card,
  CardContent,
  Button,
  Alert,
  CircularProgress,
  Stack,
  useTheme,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Chip,
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  AccessTime as TimeIcon,
  Event as EventIcon,
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
} from '@mui/icons-material';
import type { ShiftDto } from '../types/dtos/shifts';
import { useEmployeeDetails } from '../api/employees/queries';
import { ShiftDialog } from '../components/shifts/ShiftActionModalComponent';

export function EmployeeDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const theme = useTheme();
  const employeeId = parseInt(id || '0');

  const [openDialog, setOpenDialog] = useState(false);
  const [selectedShift, setSelectedShift] = useState<ShiftDto | null>(null);
  const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');

  const {
    data: employee,
    isLoading: isLoadingEmployee,
    error: employeeError,
  } = useEmployeeDetails(employeeId);

  const deleteShiftMutation = useDeleteShift();
  const createShiftMutation = useCreateShift();
  const updateShiftMutation = useUpdateShift();

  const handleAddShift = () => {
    setSelectedShift(null);
    setDialogMode('create');
    setOpenDialog(true);
  };

  const handleEditShift = (shift: ShiftDto) => {
    setSelectedShift(shift);
    setDialogMode('edit');
    setOpenDialog(true);
  };

  const handleDeleteShift = (shiftId: number) => {
    if (window.confirm('Are you sure you want to delete this shift?')) {
      deleteShiftMutation.mutate(shiftId);
    }
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedShift(null);
  };

  const handleShiftSubmit = async (data: any) => {
    if (dialogMode === 'create') {
      await createShiftMutation.mutateAsync({
        ...data,
        employeeId,
      });
    } else if (selectedShift) {
      await updateShiftMutation.mutateAsync({
        id: selectedShift.id,
        data,
      });
    }
    handleCloseDialog();
  };

  const formatTime = (timeString: string) => {
    try {
      return new Date(`2000-01-01T${timeString}`).toLocaleTimeString('en-US', {
        hour: 'numeric',
        minute: '2-digit',
        hour12: true,
      });
    } catch {
      return timeString;
    }
  };

  const formatDate = (dateString: string) => {
    try {
      return new Date(dateString).toLocaleDateString('en-US', {
        weekday: 'short',
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      });
    } catch {
      return dateString;
    }
  };

  if (isLoadingEmployee) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (employeeError || !employee) {
    return (
      <Alert severity="error" sx={{ mt: 2 }}>
        Failed to load employee details: {(employeeError as Error)?.message || 'Employee not found'}
      </Alert>
    );
  }

  const shifts = employee.shifts || [];

  return (
    <Box>
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(-1)} sx={{ mb: 2 }}>
          Back
        </Button>
        <Typography variant="h4" component="h1" gutterBottom sx={{ fontWeight: 700 }}>
          {employee.firstName} {employee.lastName}
        </Typography>
        <Typography variant="body1" color="text.secondary">
          {employee.email}
        </Typography>
      </Box>

      {/* Stats Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} md={4}>
          <Card elevation={2} sx={{ borderRadius: 2 }}>
            <CardContent>
              <Stack direction="row" alignItems="center" spacing={2}>
                <Box
                  sx={{
                    p: 1.5,
                    borderRadius: '50%',
                    backgroundColor: theme.palette.primary.light,
                    color: theme.palette.primary.contrastText,
                  }}
                >
                  <TimeIcon />
                </Box>
                <Box>
                  <Typography variant="body2" color="text.secondary">
                    This Week's Hours
                  </Typography>
                  <Typography variant="h4" sx={{ fontWeight: 700 }}>
                    {employee.totalWeeklyHours?.toFixed(1)}
                  </Typography>
                </Box>
              </Stack>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card elevation={2} sx={{ borderRadius: 2 }}>
            <CardContent>
              <Stack direction="row" alignItems="center" spacing={2}>
                <Box
                  sx={{
                    p: 1.5,
                    borderRadius: '50%',
                    backgroundColor: theme.palette.success.light,
                    color: theme.palette.success.contrastText,
                  }}
                >
                  <EventIcon />
                </Box>
                <Box>
                  <Typography variant="body2" color="text.secondary">
                    Employee Since
                  </Typography>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>
                    {new Date(employee.createdAt).toLocaleDateString()}
                  </Typography>
                </Box>
              </Stack>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card elevation={2} sx={{ borderRadius: 2 }}>
            <CardContent>
              <Stack direction="row" alignItems="center" spacing={2}>
                <Box
                  sx={{
                    p: 1.5,
                    borderRadius: '50%',
                    backgroundColor: theme.palette.secondary.light,
                    color: theme.palette.secondary.contrastText,
                  }}
                >
                  <AccessTimeIcon />
                </Box>
                <Box>
                  <Typography variant="body2" color="text.secondary">
                    Total Shifts
                  </Typography>
                  <Typography variant="h4" sx={{ fontWeight: 700 }}>
                    {shifts.length}
                  </Typography>
                </Box>
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Shifts Section */}
      <Paper elevation={2} sx={{ borderRadius: 2, overflow: 'hidden' }}>
        <Box
          sx={{
            p: 3,
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            borderBottom: `1px solid ${theme.palette.divider}`,
          }}
        >
          <Box>
            <Typography variant="h6" sx={{ fontWeight: 600 }}>
              Shifts
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Manage employee shifts for the current week
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleAddShift}
            disabled={createShiftMutation.isPending}
          >
            Add Shift
          </Button>
        </Box>

        {shifts.length === 0 ? (
          <Box sx={{ p: 4, textAlign: 'center' }}>
            <Typography variant="body1" color="text.secondary" gutterBottom>
              No shifts found for this week
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Add a shift to get started
            </Typography>
          </Box>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow sx={{ backgroundColor: theme.palette.grey[50] }}>
                  <TableCell sx={{ fontWeight: 600 }}>Date</TableCell>
                  <TableCell sx={{ fontWeight: 600 }}>Start Time</TableCell>
                  <TableCell sx={{ fontWeight: 600 }}>End Time</TableCell>
                  <TableCell sx={{ fontWeight: 600 }}>Hours</TableCell>
                  <TableCell sx={{ fontWeight: 600 }}>Notes</TableCell>
                  <TableCell sx={{ fontWeight: 600 }} align="right">
                    Actions
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {shifts.map((shift) => (
                  <TableRow key={shift.id} hover sx={{ '&:last-child td': { borderBottom: 0 } }}>
                    <TableCell>
                      <Typography variant="body1">{formatDate(shift.date)}</Typography>
                    </TableCell>
                    <TableCell>
                      <Chip
                        icon={<TimeIcon fontSize="small" />}
                        label={formatTime(shift.startTime)}
                        size="small"
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>
                      <Chip
                        icon={<TimeIcon fontSize="small" />}
                        label={formatTime(shift.endTime)}
                        size="small"
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>
                      <Typography
                        sx={{
                          fontWeight: 600,
                          color: theme.palette.primary.main,
                        }}
                      >
                        {shift.hoursWorked.toFixed(1)}h
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography
                        variant="body2"
                        color="text.secondary"
                        sx={{
                          maxWidth: 200,
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                          whiteSpace: 'nowrap',
                        }}
                      >
                        {shift.notes || '-'}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Stack direction="row" spacing={1} justifyContent="flex-end">
                        <IconButton
                          size="small"
                          color="primary"
                          onClick={() => handleEditShift(shift)}
                          disabled={updateShiftMutation.isPending}
                        >
                          <EditIcon fontSize="small" />
                        </IconButton>
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => handleDeleteShift(shift.id)}
                          disabled={deleteShiftMutation.isPending}
                        >
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Stack>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>

      {/* Shift Dialog */}
      <ShiftDialog
        open={openDialog}
        mode={dialogMode}
        shift={selectedShift}
        onSubmit={handleShiftSubmit}
        isLoading={createShiftMutation.isPending || updateShiftMutation.isPending}
        error={createShiftMutation.error || updateShiftMutation.error}
        onClose={handleCloseDialog}
      />
    </Box>
  );
}

// Import the missing icon
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { useCreateShift, useDeleteShift, useUpdateShift } from '../api/shifts/queries';
