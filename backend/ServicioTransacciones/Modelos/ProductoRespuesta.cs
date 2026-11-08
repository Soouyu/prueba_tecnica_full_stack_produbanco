namespace ServicioTransacciones.Modelos;

// esto es lo que nos devuelve el servicio de productos cuando le preguntamos por un producto
public class ProductoRespuesta
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
    public bool Activo { get; set; }
}
