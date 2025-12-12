import './App.css';
import { MainLayout } from './layout/MainLayout';
import { Navigate, Route, Routes } from 'react-router-dom';
import { WeeklyTipsPage } from './pages/WeeklyTipsPage';
import { EmployeeDetailsPage } from './pages/EmployeeDetailsPage';

function App() {
  return (
    <MainLayout>
      <Routes>
        <Route path="/" element={<WeeklyTipsPage />} />
        <Route path="employees/:id" element={<EmployeeDetailsPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </MainLayout>
  );
}

export default App;
