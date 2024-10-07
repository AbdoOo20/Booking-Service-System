import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadChildren: () => import('./pages/pages.routes').then(p => p.routes)
    },
    {
        path: 'landing',
        loadComponent: () => import('./pages/landing/landing.component').then(c => c.LandingComponent),
    },
    {
        path: 'lock-screen',
        loadComponent: () => import('./pages/lock-screen/lock-screen.component').then(c => c.LockScreenComponent)
    },
    {
        path: '**',
        loadComponent: () => import('./pages/not-found/not-found.component').then(c => c.NotFoundComponent)
    }
];
