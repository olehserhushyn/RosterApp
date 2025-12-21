import { Component } from '@angular/core';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    ToolbarModule,
    ButtonModule,
    MenubarModule,
    RouterOutlet, // <-- Provides the <router-outlet> tag
    RouterModule, // <-- Provides the routerLink directive
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayout {
  navItems = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: '/dashboard',
    },
    {
      label: 'Employees',
      icon: 'pi pi-users',
      routerLink: '/employees',
    },
    {
      label: 'Shifts',
      icon: 'pi pi-calendar',
      routerLink: '/shifts',
    },
  ];
}
