import './App.css';
import { MainLayout } from './layout/MainLayout';
import { Navigate, Route, Routes } from 'react-router-dom';
import { WeeklyTipsPage } from './pages/WeeklyTipsPage';

function App() {
  return (
    <MainLayout>
      <Routes>
        <Route path="/" element={<WeeklyTipsPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </MainLayout>
  );
}

export default App;
