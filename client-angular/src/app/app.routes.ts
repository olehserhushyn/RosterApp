import { Routes } from '@angular/router';
import { MainLayout } from './shared/layouts/main-layout/main-layout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/components/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'employees',
        loadComponent: () => import('./dashboard/components/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'shifts',
        loadComponent: () => import('./dashboard/components/dashboard').then((m) => m.Dashboard),
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
];
