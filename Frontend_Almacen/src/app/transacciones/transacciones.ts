import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-transacciones',
  imports: [FormsModule],
  templateUrl: './transacciones.html',
  styleUrl: './transacciones.css'
})
export class Transacciones implements OnInit {
  // se crea la listaa de transacciones que viene del backend
  transacciones: any[] = [];

  // la lista de productos para el dropdown
  productos: any[] = [];

  // objeto del formulaario para registrar una compra o venta
  transaccion: any = {
    tipo: '',
    productoId: null,
    cantidad: 0,
    precioUnitario: 0,
    detalle: ''
  };

  // mensaje de exito o error
  mensaje = '';

  // me dice si el mensaje es un error (para pintarlo en rojo)
  esError = false;

  // filtros de busqueda
  filtroTipo = '';
  filtroFechaInicio = '';
  filtroFechaFin = '';

  // paginacion
  pagina = 1;
  tamanoPagina = 3;
  total = 0;

  constructor(private http: HttpClient) { }

  // cuando carga la  pantallaa  se carga las transacciones y los productos
  ngOnInit() {
    this.cargarTransacciones();
    this.cargarProductos();
  }

  // se consulta al backend las transacciones con los filtros y la pagina
  cargarTransacciones() {
    let url = 'http://localhost:5303/api/transacciones?pagina=' + this.pagina + '&tamanoPagina=' + this.tamanoPagina;

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
      this.transacciones = respuesta.datos;
      this.total = respuesta.total;
    });
  }

  // cuando se busca se vuelve a la pagina 1
  buscar() {
    this.pagina = 1;
    this.cargarTransacciones();
  }

  // funcion para limpiar los filtros y recarga
  limpiarFiltros() {
    this.filtroTipo = '';
    this.filtroFechaInicio = '';
    this.filtroFechaFin = '';
    this.pagina = 1;
    this.cargarTransacciones();
  }

  // pagina anterior
  anterior() {
    if (this.pagina > 1) {
      this.pagina = this.pagina - 1;
      this.cargarTransacciones();
    }
  }

  // pagina siguiente
  siguiente() {
    if (this.pagina * this.tamanoPagina < this.total) {
      this.pagina = this.pagina + 1;
      this.cargarTransacciones();
    }
  }

  // consulta get con los productos para el dropdown
  cargarProductos() {
    this.http.get('http://localhost:5202/api/productos').subscribe((respuesta: any) => {
      this.productos = respuesta.datos;
    });
  }

  // registro la compra o venta
  guardar() {
    // valido los campos obligatorios antes de enviar
    if (!this.transaccion.tipo || !this.transaccion.productoId) {
      this.esError = true;
      this.mensaje = 'El tipo y el producto son obligatorios';
      return;
    }
    if (this.transaccion.cantidad <= 0) {
      this.esError = true;
      this.mensaje = 'La cantidad debe ser mayor a 0';
      return;
    }
    if (this.transaccion.precioUnitario <= 0) {
      this.esError = true;
      this.mensaje = 'El precio unitario debe ser mayor a 0';
      return;
    }

    this.http.post('http://localhost:5303/api/transacciones', this.transaccion).subscribe({
      next: () => {
        this.esError = false;
        this.mensaje = 'Transaccion registrada correctamente';
        this.limpiar();
        this.cargarTransacciones();
      },
      error: (e) => {
        // muestro el mensaje de error que manda el backend si no hay stock u otro caso
        this.esError = true;
        this.mensaje = e.error.mensaje;
      }
    });
  }

  // dejo el formulario vacio
  limpiar() {
    this.transaccion = {
      tipo: '',
      productoId: null,
      cantidad: 0,
      precioUnitario: 0,
      detalle: ''
    };
  }
}
