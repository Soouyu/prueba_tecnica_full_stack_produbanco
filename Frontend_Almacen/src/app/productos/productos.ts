import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-productos',
  imports: [FormsModule],
  templateUrl: './productos.html',
  styleUrl: './productos.css'
})
export class Productos implements OnInit {
  // creo la lista de productos que viene del backend un arreglo
  productos: any[] = [];

  // objeto para el formulario creaar o editar
  producto: any = {
    nombre: '',
    descripcion: '',
    categoria: '',
    precio: 0,
    imagen: '',
    stock: 0
  };

  // mensaje vacio para llenar con uun mensaje de error o exito
  mensaje = '';

  // me dice si el mensaje es un error
  esError = false;

  // filtros de busqueda
  filtroNombre = '';
  filtroCategoria = '';

  // paginacion
  pagina = 1;
  tamanoPagina = 3;
  total = 0;

  constructor(private http: HttpClient) { }

  // se ejecuta solo cuando se abre la pantalla
  ngOnInit() {
    this.cargarProductos();
  }

  // llamo al backend de productos con los filtros y la pagina
  cargarProductos() {
    let url = 'http://localhost:5202/api/productos?pagina=' + this.pagina + '&tamanoPagina=' + this.tamanoPagina;

    if (this.filtroNombre) {
      url = url + '&nombre=' + this.filtroNombre;
    }
    if (this.filtroCategoria) {
      url = url + '&categoria=' + this.filtroCategoria;
    }

    this.http.get(url).subscribe((respuesta: any) => {
      this.productos = respuesta.datos;
      this.total = respuesta.total;
    });
  }

  // cuando busco vuelvo a la pagina 1
  buscar() {
    this.pagina = 1;
    this.cargarProductos();
  }

  // limpio los filtros y recargo
  limpiarFiltros() {
    this.filtroNombre = '';
    this.filtroCategoria = '';
    this.pagina = 1;
    this.cargarProductos();
  }

  // pagina anterior
  anterior() {
    if (this.pagina > 1) {
      this.pagina = this.pagina - 1;
      this.cargarProductos();
    }
  }

  // pagina siguiente
  siguiente() {
    if (this.pagina * this.tamanoPagina < this.total) {
      this.pagina = this.pagina + 1;
      this.cargarProductos();
    }
  }

  // si el producto tiene id lo edita sino lo creea
  guardar() {
    // valido los campos obligatorios antes de enviar
    if (!this.producto.nombre || !this.producto.categoria) {
      this.esError = true;
      this.mensaje = 'El nombre y la categoria son obligatorios';
      return;
    }
    if (this.producto.precio <= 0) {
      this.esError = true;
      this.mensaje = 'El precio debe ser mayor a 0';
      return;
    }
    if (this.producto.stock < 0) {
      this.esError = true;
      this.mensaje = 'El stock no puede ser negativo';
      return;
    }

    this.esError = false;

    if (this.producto.id) {
      // peticion put si tiene id
      this.http.put('http://localhost:5202/api/productos/' + this.producto.id, this.producto).subscribe(() => {
        this.mensaje = 'Producto editado correctamente';
        this.limpiar();
        this.cargarProductos();
      });
    } else {
      // petion post si falla
      this.http.post('http://localhost:5202/api/productos', this.producto).subscribe(() => {
        this.mensaje = 'Producto creado correctamente';
        this.limpiar();
        this.cargarProductos();
      });
    }
  }

  // se cargan los datos de  producto en el formulario para editarlo
  editar(p: any) {
    this.producto = { ...p };
  }

  // fuuncion para eliminar un producto por su id y se vuelve a cargaar a la listaa
  eliminar(id: number) {
    if (confirm('quieres eliminar este producto?')) {
      this.http.delete('http://localhost:5202/api/productos/' + id).subscribe(() => {
        this.esError = false;
        this.mensaje = 'Producto eliminado correctamente';
        this.cargarProductos();
      });
    }
  }

  // dejo el formulario vacio
  limpiar() {
    this.producto = {
      nombre: '',
      descripcion: '',
      categoria: '',
      precio: 0,
      imagen: '',
      stock: 0
    };
  }
}
