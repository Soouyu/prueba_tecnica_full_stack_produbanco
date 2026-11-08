using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioTransacciones.Datos;
using ServicioTransacciones.Modelos;

namespace ServicioTransacciones.Controllers;

//controlador principal de la apii
[ApiController]
[Route("api/transacciones")]
public class TransaccionesController : ControllerBase
{
    private readonly Base_DB _base;
    private readonly HttpClient _http;
    private readonly string _urlProductos;

    public TransaccionesController(Base_DB db, IHttpClientFactory fabrica, IConfiguration config)
    {
        _base = db;
        _http = fabrica.CreateClient();
        _urlProductos = config["ServicioProductos:Url"];
    }

    // mostrar con filtros dinamicos
    [HttpGet]
    public async Task<IActionResult> Mostrar(
        int? productoId = null,
        string tipo = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        int pagina = 1,
        int tamanoPagina = 10
    )
    {
        var consulta = _base.Transacciones.AsQueryable();

        // solo si mandan un producto
        if (productoId != null)
            consulta = consulta.Where(t => t.ProductoId == productoId);

        // solo si mandan un tipo
        if (!string.IsNullOrEmpty(tipo))
            consulta = consulta.Where(t => t.Tipo == tipo);

        // solo si mandan fecha de inicio
        if (fechaInicio != null)
            consulta = consulta.Where(t => t.Fecha >= fechaInicio);

        // solo si mandan fecha de fin
        if (fechaFin != null)
            consulta = consulta.Where(t => t.Fecha <= fechaFin);

        var total = await consulta.CountAsync();

        var transacciones = await consulta
            .OrderByDescending(t => t.Fecha)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return Ok(
            new
            {
                total,
                pagina,
                tamanoPagina,
                datos = transacciones,
            }
        );
    }

    // obtener por id uno
    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var transaccion = await _base.Transacciones.FindAsync(id);

        if (transaccion == null)
        {
            return NotFound(new { mensaje = "Transaccion no encontrada :(" });
        }
        return Ok(transaccion);
    }

    // crear una transaccion
    [HttpPost]
    public async Task<IActionResult> Insertar(Transaccion transaccion)
    {
        // solo acepto compra o venta
        if (transaccion.Tipo != "COMPRA" && transaccion.Tipo != "VENTA")
        {
            return BadRequest(new { mensaje = "El tipo debe ser COMPRA o VENTA" });
        }

        if (transaccion.Cantidad <= 0)
        {
            return BadRequest(new { mensaje = "La cantidad debe ser mayor a 0" });
        }

        // valido el servicio de productos si el producto existe
        var respuesta = await _http.GetAsync(
            $"{_urlProductos}/api/productos/{transaccion.ProductoId}"
        );

        if (!respuesta.IsSuccessStatusCode)
        {
            return BadRequest(new { mensaje = "El producto no existe" });
        }

        var producto = await respuesta.Content.ReadFromJsonAsync<ProductoRespuesta>();

        // si es venta valido que haya stock suficiente antes
        if (transaccion.Tipo == "VENTA" && transaccion.Cantidad > producto.Stock)
        {
            return BadRequest(new { mensaje = "Stock insuficiente para la venta" });
        }

        // calculo el total y pongo la fecha
        transaccion.PrecioTotal = transaccion.Cantidad * transaccion.PrecioUnitario;
        transaccion.Fecha = DateTime.Now;

        // ajusto el stock segun la operacion
        int ajuste = transaccion.Tipo == "VENTA" ? -transaccion.Cantidad : transaccion.Cantidad;
        await _http.PutAsJsonAsync(
            $"{_urlProductos}/api/productos/{transaccion.ProductoId}/stock",
            ajuste
        );

        // guardo la transaccion
        _base.Transacciones.Add(transaccion);
        await _base.SaveChangesAsync();

        return Ok(new { mensaje = "Transaccion registrada correctamente", transaccion });
    }

    // editar una transaccion
    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, Transaccion datos)
    {
        var transaccion = await _base.Transacciones.FindAsync(id);

        if (transaccion == null)
        {
            return NotFound(new { mensaje = "Transaccion no encontrada" });
        }

        transaccion.Detalle = datos.Detalle;

        await _base.SaveChangesAsync();

        return Ok(new { mensaje = "Transaccion editada correctamente", transaccion });
    }

    // eliminar una transaccion por id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var transaccion = await _base.Transacciones.FindAsync(id);

        if (transaccion == null)
        {
            return NotFound(new { mensaje = "Transaccion no encontrada :(" });
        }

        _base.Transacciones.Remove(transaccion);
        await _base.SaveChangesAsync();

        return Ok(new { mensaje = "Transaccion eliminada correctamente" });
    }

    // historial de un producto con su nombre y stock
    [HttpGet("historial/{productoId}")]
    public async Task<IActionResult> Historial(
        int productoId,
        string tipo = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null
    )
    {
        // se consulta la data del producto de otro servicio
        var respuesta = await _http.GetAsync($"{_urlProductos}/api/productos/{productoId}");

        if (!respuesta.IsSuccessStatusCode)
        {
            return BadRequest(new { mensaje = "El producto no existe" });
        }

        var producto = await respuesta.Content.ReadFromJsonAsync<ProductoRespuesta>();

        // consulta a las transacciones de ese producto con los filtros
        var consulta = _base.Transacciones.Where(t => t.ProductoId == productoId);

        if (!string.IsNullOrEmpty(tipo))
            consulta = consulta.Where(t => t.Tipo == tipo);

        if (fechaInicio != null)
            consulta = consulta.Where(t => t.Fecha >= fechaInicio);

        if (fechaFin != null)
            consulta = consulta.Where(t => t.Fecha <= fechaFin);

        var transacciones = await consulta.OrderByDescending(t => t.Fecha).ToListAsync();

        return Ok(
            new
            {
                productoNombre = producto.Nombre,
                productoStock = producto.Stock,
                transacciones,
            }
        );
    }
}
