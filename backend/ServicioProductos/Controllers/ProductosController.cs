using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioProductos.Datos;
using ServicioProductos.Modelos;

namespace ServicioProductos.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly Base_DB _base;

    public ProductosController(Base_DB db)
    {
        _base = db;
    }

    // mostrar con filtros dinamicos y la paginacion

    [HttpGet]
    public async Task<IActionResult> Mostrar(
        string nombre = null,
        string categoria = null,
        int pagina = 1,
        int tamanoPagina = 10
    )
    {
        var consulta = _base.Productos.Where(p => p.Activo);
        // solo si tiene algo un nombre, paso todo a minusculas para que no importe mayuscula o minuscula
        if (!string.IsNullOrEmpty(nombre))
            consulta = consulta.Where(p => p.Nombre.ToLower().Contains(nombre.ToLower()));

        // solo si tiene una categoria
        if (!string.IsNullOrEmpty(categoria))
            consulta = consulta.Where(p => p.Categoria == categoria);

        var total = await consulta.CountAsync();

        var productos = await consulta
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return Ok(
            new
            {
                total,
                pagina,
                tamanoPagina,
                datos = productos,
            }
        );
    }

    //obetener 1 por id
    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var producto = await _base.Productos.FindAsync(id);

        if (producto == null)
        {
            return NotFound(new { mensaje = "Productos no encontrado :( " });
        }
        return Ok(producto);
    }

    //crear un producto
    [HttpPost]
    public async Task<IActionResult> Insertar(Producto productos)
    {
        productos.Activo = true;
        productos.FechaCreacion = DateTime.Now;

        _base.Productos.Add(productos);
        await _base.SaveChangesAsync();

        return Ok(new { mensaje = "Producto creado correctamente", productos });
    }

    // editar un producto
    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, Producto datos)
    {
        var producto = await _base.Productos.FindAsync(id);

        if (producto == null)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }
        producto.Nombre = datos.Nombre;
        producto.Descripcion = datos.Descripcion;
        producto.Categoria = datos.Categoria;
        producto.Precio = datos.Precio;
        producto.Imagen = datos.Imagen;
        producto.Stock = datos.Stock;

        await _base.SaveChangesAsync();

        return Ok(new { mensaje = "Productos editado correctamente ", producto });
    }

    //eliminar por id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _base.Productos.FindAsync(id);

        if (producto == null)
        {
            return NotFound(new { mensaje = "Producto no encontrado :(" });
        }

        producto.Activo = false;
        await _base.SaveChangesAsync();

        return Ok(new { mensaje = "Producto eliminado correctamente" });
    }

    // ajustar el stock (lo usa el servicio de transacciones)
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> AjustarStock(int id, [FromBody] int cantidad)
    {
        var producto = await _base.Productos.FindAsync(id);

        if (producto == null)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }

        int nuevoStock = producto.Stock + cantidad;
        if (nuevoStock < 0)
        {
            return BadRequest(new { mensaje = "Stock insuficiente" });
        }

        producto.Stock = nuevoStock;
        await _base.SaveChangesAsync();

        return Ok(new { mensaje = "Stock actualizado", stock = producto.Stock });
    }
}
