using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AllKuba.API.Data;
using AllKuba.API.DTOs;
using AllKuba.API.Models;

namespace AllKuba.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? estado, [FromQuery] int? clienteId)
    {
        var q = db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Lineas).ThenInclude(l => l.Producto)
            .AsQueryable();

        if (!string.IsNullOrEmpty(estado)) q = q.Where(v => v.Estado == estado);
        if (clienteId.HasValue) q = q.Where(v => v.ClienteId == clienteId);

        var ventas = await q.OrderByDescending(v => v.CreadoEn).ToListAsync();
        return Ok(ventas.Select(Map));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var v = await db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Lineas).ThenInclude(l => l.Producto)
            .FirstOrDefaultAsync(v => v.Id == id);

        return v is null ? NotFound() : Ok(Map(v));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearVentaRequest req)
    {
        var cliente = await db.Clientes.FindAsync(req.ClienteId);
        if (cliente is null) return BadRequest("Cliente no encontrado");

        var anio = DateTime.Now.Year;
        var count = await db.Ventas.CountAsync(v => v.CreadoEn.Year == anio) + 1;
        var numero = $"PRES-{anio}-{count:D3}";

        var venta = new Venta
        {
            ClienteId = req.ClienteId,
            Numero = numero,
            Observaciones = req.Observaciones,
            Estado = "pendiente"
        };

        foreach (var lineaReq in req.Lineas)
        {
            var producto = await db.Productos.FindAsync(lineaReq.ProductoId);
            if (producto is null) return BadRequest($"Producto {lineaReq.ProductoId} no encontrado");

            // El calculador devuelve tanto precio de venta como precio de costo
            var calculo = PrecioCalculator.Calcular(producto, lineaReq.Ancho, lineaReq.Alto);
            var subtotal = calculo.PrecioUnitario * lineaReq.Cantidad;
            var subtotalCosto = calculo.PrecioCostoUnitario * lineaReq.Cantidad;

            venta.Lineas.Add(new LineaVenta
            {
                ProductoId = lineaReq.ProductoId,
                NombreProducto = producto.Nombre,
                Ancho = lineaReq.Ancho,
                Alto = lineaReq.Alto,
                Cantidad = lineaReq.Cantidad,
                VarianteSeleccionadaJson = JsonSerializer.Serialize(lineaReq.VarianteSeleccionada),
                PrecioUnitario = calculo.PrecioUnitario,
                PrecioCostoUnitario = calculo.PrecioCostoUnitario,
                Subtotal = subtotal,
                SubtotalCosto = subtotalCosto,
                Notas = lineaReq.Notas
            });
        }

        venta.Total = venta.Lineas.Sum(l => l.Subtotal);
        venta.TotalCosto = venta.Lineas.Sum(l => l.SubtotalCosto);

        db.Ventas.Add(venta);
        await db.SaveChangesAsync();

        var created = await db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Lineas).ThenInclude(l => l.Producto)
            .FirstAsync(v => v.Id == venta.Id);

        return CreatedAtAction(nameof(Get), new { id = venta.Id }, Map(created));
    }

    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoRequest req)
    {
        var estados = new[] { "pendiente", "aprobado", "en_produccion", "entregado", "cancelado" };
        if (!estados.Contains(req.Estado)) return BadRequest("Estado inválido");

        var v = await db.Ventas.FindAsync(id);
        if (v is null) return NotFound();

        v.Estado = req.Estado;
        v.ActualizadoEn = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(new { v.Estado });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var v = await db.Ventas.Include(v => v.Lineas).FirstOrDefaultAsync(v => v.Id == id);
        if (v is null) return NotFound();
        db.Ventas.Remove(v);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static VentaDto Map(Venta v) => new(
    v.Id, v.Numero, v.Estado,
    new ClienteDto(
        v.Cliente.Id,
        v.Cliente.Nombre,
        JsonSerializer.Deserialize<List<string>>(v.Cliente.TelefonosJson) ?? [],
        v.Cliente.Email,
        v.Cliente.Direccion),
        [.. v.Lineas.Select(l => new LineaVentaDto(
            l.Id, l.ProductoId, l.NombreProducto,
            l.Ancho, l.Alto, l.Cantidad,
            JsonSerializer.Deserialize<Dictionary<string, string>>(l.VarianteSeleccionadaJson) ?? [],
            l.PrecioUnitario, l.PrecioCostoUnitario,
            l.Subtotal, l.SubtotalCosto, l.Notas
          ))],
                v.Total, v.TotalCosto, v.Observaciones, v.CreadoEn
        );
}
