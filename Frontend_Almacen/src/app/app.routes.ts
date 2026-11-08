import { Routes } from '@angular/router';
import { Productos } from './productos/productos';
import { Transacciones } from './transacciones/transacciones';
import { Historial } from './historial/historial';

export const routes: Routes = [
  { path: '', component: Productos },
  { path: 'transacciones', component: Transacciones },
  { path: 'historial', component: Historial }
];
