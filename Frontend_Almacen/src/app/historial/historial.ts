import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-historial',
  imports: [FormsModule],
  templateUrl: './historial.html',
  styleUrl: './historial.css'
})
export class Historial implements OnInit {
  // la lista de productos para el dropdown
  productos: any[] = [];

  // producto elegido y los filtros
  productoId: any = null;
  filtroTipo = '';
  filtroFechaInicio = '';
  filtroFechaFin = '';

  // daatos que devuelve el historial
  nombreProducto = '';
  stockProducto = 0;
  transacciones: any[] = [];

  // mensaje de error defaaultt
  mensaje = '';
  esError = false;

  //  para mostrar la tabla solo despues de consultar
  consultado = false;

  constructor(private http: HttpClient) { }

  // al abrir la pantalla se carga los productoss
  ngOnInit() {
    this.cargarProductos();
  }

  // consulta el get a productos para el dropdown
  cargarProductos() {
    this.http.get('http://localhost:5202/api/productos').subscribe((respuesta: any) => {
      this.productos = respuesta.datos;
    });
  }

  //se revisa  el historial del producto con los filtros y sus condiciiones
  consultar() {
    if (!this.productoId) {
      this.esError = true;
      this.mensaje = 'Seleccione un producto';
      return;
    }

    let url = 'http://localhost:5303/api/transacciones/historial/' + this.productoId + '?';

    if (this.filtroTipo) {
      url = url + '&tipo=' + this.filtroTipo;
    }
    if (this.filtroFechaInicio) {
      url = url + '&fechaInicio=' + this.filtroFechaInicio;
    }
    if (this.filtroFechaFin) {
      url = url + '&fechaFin=' + this.filtroFechaFin;
    }

    this.http.get(url).subscribe((respuesta: any) => {
      this.nombreProducto = respuesta.productoNombre;
      this.stockProducto = respuesta.productoStock;
      this.transacciones = respuesta.transacciones;
      this.consultado = true;
      this.esError = false;
      this.mensaje = '';
    });
  }
}
