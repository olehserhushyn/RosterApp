import { AppBar, Box, Container, Toolbar, Typography } from '@mui/material';
import type { ReactNode } from 'react';

interface MainLayoutProps {
  children: ReactNode;
}

export function MainLayout({ children }: MainLayoutProps) {
  return (
    <Box
      sx={{
        minHeight: '100vh',
        bgcolor: 'background.default',
        width: '100%',
        overflowX: 'hidden',
      }}
    >
      <AppBar>
        <Toolbar>
          <Typography variant="h6" component="div">
            Roster App - Weekly Tips
          </Typography>
        </Toolbar>
      </AppBar>

      <Box sx={{ pt: { xs: '56px', sm: '64px' } }}>
        <Container
          maxWidth={false}
          sx={{
            width: '100%',
            px: { xs: 2, sm: 3, md: 4 },
            py: 4,
          }}
        >
          {children}
        </Container>
      </Box>
    </Box>
  );
}
