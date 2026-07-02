using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AllKuba.API.Data;
using AllKuba.API.DTOs;
using AllKuba.API.Models;

namespace AllKuba.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var productos = await db.Productos.Include(p => p.TipoProducto).Where(p => p.Activo).ToListAsync();
        return Ok(productos.Select(Map));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await db.Productos.Include(p => p.TipoProducto).FirstOrDefaultAsync(p => p.Id == id);
        return p is null ? NotFound() : Ok(Map(p));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearProductoRequest req)
    {
        var p = new Producto
        {
            Nombre = req.Nombre,
            Descripcion = req.Descripcion,
            TipoProductoId = req.TipoProductoId,
            TieneMedidas = req.TieneMedidas,
            PrecioBase = req.PrecioBase,
            AnchoEstandar = req.AnchoEstandar,
            AltoEstandar = req.AltoEstandar,
            TipoCalculo = req.TipoCalculo,
            PrecioExceso = req.PrecioExceso,
            PrecioCosto = req.PrecioCosto,
            PrecioCostoExceso = req.PrecioCostoExceso,
            PrecioVenta = req.PrecioVenta,
            VariantesJson = JsonSerializer.Serialize(req.Variantes)
        };
        db.Productos.Add(p);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = p.Id }, Map(p));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CrearProductoRequest req)
    {
        var p = await db.Productos.FindAsync(id);
        if (p is null) return NotFound();

        p.Nombre = req.Nombre;
        p.Descripcion = req.Descripcion;
        p.TipoProductoId = req.TipoProductoId;
        p.TieneMedidas = req.TieneMedidas;
        p.PrecioBase = req.PrecioBase;
        p.AnchoEstandar = req.AnchoEstandar;
        p.AltoEstandar = req.AltoEstandar;
        p.TipoCalculo = req.TipoCalculo;
        p.PrecioExceso = req.PrecioExceso;
        p.PrecioCosto = req.PrecioCosto;
        p.PrecioCostoExceso = req.PrecioCostoExceso;
        p.PrecioVenta = req.PrecioVenta;
        p.VariantesJson = JsonSerializer.Serialize(req.Variantes);

        await db.SaveChangesAsync();
        return Ok(Map(p));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await db.Productos.FindAsync(id);
        if (p is null) return NotFound();
        p.Activo = false;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("calcular-precio")]
    public async Task<IActionResult> CalcularPrecio(CalcularPrecioRequest req)
    {
        var p = await db.Productos.FindAsync(req.ProductoId);
        if (p is null) return NotFound();

        var resultado = PrecioCalculator.Calcular(p, req.Ancho, req.Alto, req.VariantesSeleccionadas);
        var subtotal = resultado.PrecioUnitario * req.Cantidad;
        var subtotalCosto = resultado.PrecioCostoUnitario * req.Cantidad;
        return Ok(new CalcularPrecioResponse(
            resultado.PrecioUnitario, resultado.PrecioCostoUnitario,
            subtotal, subtotalCosto, resultado.Detalle));
    }

    private static ProductoDto Map(Producto p)
    {
        var variantes = JsonSerializer.Deserialize<List<VarianteDto>>(p.VariantesJson) ?? [];
        return new(p.Id, p.Nombre, p.Descripcion,
                   p.TipoProductoId, p.TipoProducto?.Nombre,
                   p.TieneMedidas,
                   p.PrecioBase, p.AnchoEstandar, p.AltoEstandar,
                   p.TipoCalculo, p.PrecioExceso,
                   p.PrecioCosto, p.PrecioCostoExceso, p.PrecioVenta,
                   variantes, p.Activo);
    }
}
