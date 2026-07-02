using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AllKuba.API.Data;
using AllKuba.API.DTOs;
using AllKuba.API.Models;

namespace AllKuba.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiposProductoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tipos = await db.TiposProducto.Where(t => t.Activo).OrderBy(t => t.Nombre).ToListAsync();
        return Ok(tipos.Select(Map));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var t = await db.TiposProducto.FindAsync(id);
        return t is null ? NotFound() : Ok(Map(t));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearTipoProductoRequest req)
    {
        var tipo = new TipoProducto
        {
            Nombre = req.Nombre,
            Descripcion = req.Descripcion,
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
        db.TiposProducto.Add(tipo);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = tipo.Id }, Map(tipo));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CrearTipoProductoRequest req)
    {
        var tipo = await db.TiposProducto.FindAsync(id);
        if (tipo is null) return NotFound();

        tipo.Nombre = req.Nombre;
        tipo.Descripcion = req.Descripcion;
        tipo.TieneMedidas = req.TieneMedidas;
        tipo.PrecioBase = req.PrecioBase;
        tipo.AnchoEstandar = req.AnchoEstandar;
        tipo.AltoEstandar = req.AltoEstandar;
        tipo.TipoCalculo = req.TipoCalculo;
        tipo.PrecioExceso = req.PrecioExceso;
        tipo.PrecioCosto = req.PrecioCosto;
        tipo.PrecioCostoExceso = req.PrecioCostoExceso;
        tipo.PrecioVenta = req.PrecioVenta;
        tipo.VariantesJson = JsonSerializer.Serialize(req.Variantes);

        // Sincronizar todos los productos de este tipo
        var productos = await db.Productos
            .Where(p => p.TipoProductoId == id && p.Activo)
            .ToListAsync();

        foreach (var p in productos)
        {
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
        }

        await db.SaveChangesAsync();
        return Ok(Map(tipo));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tipo = await db.TiposProducto.FindAsync(id);
        if (tipo is null) return NotFound();
        tipo.Activo = false;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static TipoProductoDto Map(TipoProducto t)
    {
        var variantes = JsonSerializer.Deserialize<List<VarianteDto>>(t.VariantesJson) ?? [];
        return new(t.Id, t.Nombre, t.Descripcion, t.TieneMedidas,
                   t.PrecioBase, t.AnchoEstandar, t.AltoEstandar,
                   t.TipoCalculo, t.PrecioExceso,
                   t.PrecioCosto, t.PrecioCostoExceso, t.PrecioVenta, variantes);
    }
}