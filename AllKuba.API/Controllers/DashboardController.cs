using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllKuba.API.Data;
using AllKuba.API.DTOs;

namespace AllKuba.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ventas = await db.Ventas
            .Include(v => v.Lineas).ThenInclude(l => l.Producto)
            .Where(v => v.Estado != "cancelado")
            .ToListAsync();

        var total = ventas.Sum(v => v.Total);
        var costo = ventas.Sum(v => v.TotalCosto);
        var ganancia = total - costo;
        var margen = total > 0 ? (ganancia / total) * 100 : 0;

        // Ventas por mes (últimos 6 meses)
        var porMes = ventas
            .GroupBy(v => new { v.CreadoEn.Year, v.CreadoEn.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .TakeLast(6)
            .Select(g => new VentaPorMesDto(
                $"{g.Key.Month:D2}/{g.Key.Year}",
                g.Sum(v => v.Total),
                g.Sum(v => v.TotalCosto),
                g.Sum(v => v.Total) - g.Sum(v => v.TotalCosto)
            )).ToList();

        // Productos más vendidos
        var productosTop = ventas
            .SelectMany(v => v.Lineas)
            .GroupBy(l => l.NombreProducto)
            .Select(g => new ProductoTopDto(
                g.Key,
                g.Sum(l => l.Cantidad),
                g.Sum(l => l.Subtotal),
                g.Sum(l => l.Subtotal) - g.Sum(l => l.SubtotalCosto)
            ))
            .OrderByDescending(p => p.Ingresos)
            .Take(5)
            .ToList();

        return Ok(new DashboardDto(
            ventas.Count,
            ventas.Count(v => v.Estado == "entregado"),
            ventas.Count(v => v.Estado == "pendiente"),
            ventas.Count(v => v.Estado == "en_produccion"),
            total, costo, ganancia, margen,
            porMes, productosTop
        ));
    }
}
