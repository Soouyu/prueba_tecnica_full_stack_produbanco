using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioProductos.Modelos;

[Table("productos")]
public class Producto
{
    [Column("id")]
    public int Id { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; }

    [Column("categoria")]
    public string Categoria { get; set; }

    [Column("precio")]
    public decimal Precio { get; set; }

    [Column("imagen")]
    public string Imagen { get; set; }

    [Column("stock")]
    public int Stock { get; set; }

    [Column("activo")]
    public bool Activo { get; set; }

    [Column("fecha_creacion", TypeName = "timestamp without time zone")]
    public DateTime FechaCreacion { get; set; }
}
