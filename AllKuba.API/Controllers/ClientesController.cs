using AllKuba.API.Data;
using AllKuba.API.DTOs;
using AllKuba.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AllKuba.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await db.Clientes.ToListAsync();
        return Ok(clientes.Select(Map));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var c = await db.Clientes.FindAsync(id);
        return c is null ? NotFound() : Ok(Map(c));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearClienteRequest req)
    {
        var c = new Cliente
        {
            Nombre = req.Nombre,
            TelefonosJson = JsonSerializer.Serialize(req.Telefonos),
            Email = req.Email,
            Direccion = req.Direccion
        };
        db.Clientes.Add(c);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = c.Id }, Map(c));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CrearClienteRequest req)
    {
        var c = await db.Clientes.FindAsync(id);
        if (c is null) return NotFound();
        c.Nombre = req.Nombre;
        c.TelefonosJson = JsonSerializer.Serialize(req.Telefonos);
        c.Email = req.Email;
        c.Direccion = req.Direccion;
        await db.SaveChangesAsync();
        return Ok(Map(c));
    }

    private static ClienteDto Map(Cliente c)
    {
        var telefonos = JsonSerializer.Deserialize<List<string>>(c.TelefonosJson) ?? [];
        return new(c.Id, c.Nombre, telefonos, c.Email, c.Direccion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await db.Clientes.FindAsync(id);
        if (c is null) return NotFound();
        db.Clientes.Remove(c);
        await db.SaveChangesAsync();
        return NoContent();
    }

}
