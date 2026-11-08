using Microsoft.EntityFrameworkCore;
using ServicioProductos.Modelos;

namespace ServicioProductos.Datos;

public class Base_DB : DbContext
{
    public Base_DB(DbContextOptions<Base_DB> opciones)
        : base(opciones) { }

    public DbSet<Producto> Productos { get; set; }
}
