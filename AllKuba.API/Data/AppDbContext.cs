using Microsoft.EntityFrameworkCore;
using AllKuba.API.Models;

namespace AllKuba.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TipoProducto> TiposProducto => Set<TipoProducto>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<LineaVenta> LineasVenta => Set<LineaVenta>();
}
