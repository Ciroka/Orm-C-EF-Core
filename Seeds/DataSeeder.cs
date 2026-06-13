namespace PracticoOrm.Seeds;

using System.Text.Json;
using PracticoOrm;
using PracticoOrm.Models;

public class DataSeeder
{
    public static async Task SeedAsync (AppDbContext context)
    {
        if (!context.PuntoDeVentas.Any()) await SeedEntidad<PuntoDeVenta>("Seeds/puntosDeVenta.json", context);
        if (!context.Mostradores.Any()) await SeedMostradores(context);
        if (!context.Ingredientes.Any()) await SeedEntidad<Ingrediente>("Seeds/ingredientes.json", context);
        if (!context.TipoProductos.Any()) await SeedEntidad<TipoProducto>("Seeds/tipoProductos.json", context);
        if (!context.Recetas.Any()) await SeedEntidad<Receta>("Seeds/recetas.json", context);
        if (!context.DetalleRecetas.Any()) await SeedDetallesRecetas(context);
        if (!context.Productos.Any()) await SeedProductos(context);
        if (!context.Ventas.Any()) await SeedVentas(context);
        if (!context.DetalleVentas.Any()) await SeedDetallesVentas(context);
    }
    
    public static async Task SeedEntidad<T>(string rutaArchivo, AppDbContext context) where T: class
    {
        var json = await File.ReadAllTextAsync(rutaArchivo);
        var datos = JsonSerializer.Deserialize<List<T>>(json);
        await context.Set<T>().AddRangeAsync(datos!);
        await context.SaveChangesAsync();
    }

    public static async Task SeedMostradores(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/mostradores.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);

        foreach (var dato in datos!)
        {
            var puntoDeVentaNombre = dato.GetProperty("PuntoDeVentaNombre").GetString();
            var puntoDeVentaId = context.PuntoDeVentas.First(p => p.Nombre == puntoDeVentaNombre).PuntoDeVentaId;

            var mostrador = new Mostrador
            {
              Nombre = dato.GetProperty("Nombre").GetString()!,
              PuntoDeVentaId = puntoDeVentaId  
            };         

            await context.Mostradores.AddAsync(mostrador);
        }
        await context.SaveChangesAsync();
    }

    public static async Task SeedVentas(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/ventas.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);

        foreach (var dato in datos!)
        {
            var mostradorNombre = dato.GetProperty("MostradorNombre").GetString();
            var mostradorId = context.Mostradores.First(m => m.Nombre == mostradorNombre).MostradorId;

            var venta = new Venta
            {
                FechaDeVenta = dato.GetProperty("FechaDeVenta").GetDateTime(),
                MostradorId = mostradorId
            };
            await context.Ventas.AddAsync(venta);
        }
        await context.SaveChangesAsync();
    }


    public static async Task SeedDetallesVentas(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/detallesVenta.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);
        var ventas = context.Ventas.ToList();

        foreach (var dato in datos!)
        {
            var ventaIndex = dato.GetProperty("VentaIndex").GetInt16();
            var venta = ventas[ventaIndex];
            var ventaId = venta.VentaId;

            var productoNombre = dato.GetProperty("ProductoNombre").GetString();
            var productoId = context.Productos.First(p => p.Nombre == productoNombre).ProductoId;

            var detalleVenta = new DetalleVenta
            {
             VentaId = ventaId,
             ProductoId = productoId,
             Cantidad = dato.GetProperty("Cantidad").GetInt16()   
            };
            await context.AddAsync(detalleVenta);
        }
        await context.SaveChangesAsync();
    }


    public static async Task SeedProductos(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/productos.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);

        foreach (var dato in datos!)
        {
            var recetaNombre = dato.GetProperty("RecetaNombre").GetString();
            var recetaId = context.Recetas.First(r => r.Nombre == recetaNombre).RecetaId;

            var tipoProductoNombre = dato.GetProperty("TipoProductoNombre").GetString();
            var tipoProductoId = context.TipoProductos.First(tp => tp.Nombre == tipoProductoNombre).TipoProductoId;

            var producto = new Producto
            {
                Nombre = dato.GetProperty("Nombre").GetString()!,
                RecetaId = recetaId,
                TipoProductoId = tipoProductoId,
                PorcentajeDeGanancia = dato.GetProperty("PorcentajeDeGanancia").GetDecimal(),
                Descripcion = dato.GetProperty("Descripcion").GetString()
            };
            await context.AddAsync(producto); 
        }
        await context.SaveChangesAsync();
    }


    public static async Task SeedDetallesRecetas(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/detallesReceta.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);
        var recetas = context.Recetas.ToList();

        foreach (var dato in datos!)
        {
            var recetaIndex = dato.GetProperty("RecetaIndex").GetInt16();
            var receta = recetas[recetaIndex];
            var recetaID = receta.RecetaId;

            var ingredienteNombre = dato.GetProperty("IngredienteNombre").GetString();
            var ingredienteId = context.Ingredientes.First(i => i.Nombre == ingredienteNombre).IngredienteId;

            var detallesReceta = new DetalleReceta
            {
                RecetaId = recetaID,
                IngredienteId = ingredienteId,
                Cantidad = dato.GetProperty("Cantidad").GetDecimal()
            };
            await context.AddAsync(detallesReceta);
        }   
        await context.SaveChangesAsync();
    }
}