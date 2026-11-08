import { Routes } from '@angular/router';
import { Productos } from './productos/productos';
import { Transacciones } from './transacciones/transacciones';

export const routes: Routes = [
  { path: '', component: Productos },
  { path: 'transacciones', component: Transacciones }
];
