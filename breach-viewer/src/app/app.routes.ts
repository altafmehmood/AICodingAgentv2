import { Routes } from '@angular/router';
import { BreachListComponent } from './components/breach-list/breach-list.component';

export const routes: Routes = [
  { path: '', component: BreachListComponent },
  { path: 'breaches', component: BreachListComponent },
  { path: '**', redirectTo: '' }
];
