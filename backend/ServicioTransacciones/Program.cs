using Microsoft.EntityFrameworkCore;
using ServicioTransacciones.Datos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// registrar la base de datos con postgesql y ef core
builder.Services.AddDbContext<Base_DB>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConexionPostgres"))
);

// HttpCLiente para llamar al microservicio de productos
builder.Services.AddHttpClient();

// se dan permisos al cors para no tener error con el front
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy(
        "PermitirFront",
        politica =>
        {
            politica.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("PermitirFront");

app.UseAuthorization();

app.MapControllers();

app.Run();
