using Microsoft.EntityFrameworkCore;
using PracticoOrm.Models;

namespace PracticoOrm;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

    public DbSet<DetalleReceta> DetalleRecetas => Set<DetalleReceta>();
    public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();
    public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
    public DbSet<Mostrador> Mostradores => Set<Mostrador>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<PuntoDeVenta> PuntoDeVentas => Set<PuntoDeVenta>();
    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<TipoProducto> TipoProductos => Set<TipoProducto>(); 
    public DbSet<Venta> Ventas => Set<Venta>();
    
}

