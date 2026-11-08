# imagen con el sdk de .net para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# copio todo y publico la app
COPY . .
RUN dotnet publish -c Release -o salida

# imagen mas liviana solo para correr la app ya compilada
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/salida .

# el servicio escucha en el puerto 8080 dentro del contenedor
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# arranco el servicio
ENTRYPOINT ["dotnet", "ServicioTransacciones.dll"]
