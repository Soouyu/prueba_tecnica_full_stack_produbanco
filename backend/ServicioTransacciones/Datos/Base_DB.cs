using Microsoft.EntityFrameworkCore;
using ServicioTransacciones.Modelos;

namespace ServicioTransacciones.Datos;

// capa que se encarga de la conexion de la base de datos
public class Base_DB : DbContext
{
    public Base_DB(DbContextOptions<Base_DB> opciones)
        : base(opciones) { }

    public DbSet<Transaccion> Transacciones { get; set; }
}
