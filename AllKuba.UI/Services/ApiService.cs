using AllKuba.UI.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace AllKuba.UI.Services;

public class ApiService(HttpClient http)
{
    private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

    // ── Dashboard ─────────────────────────────────────────────
    public Task<DashboardDto> GetDashboardAsync() =>
        GetAsync<DashboardDto>("dashboard");

    // ── Tipos de Producto ─────────────────────────────────────
    public Task<List<TipoProducto>> GetTiposProductoAsync() =>
        GetAsync<List<TipoProducto>>("tiposproducto");

    public Task<TipoProducto> CrearTipoProductoAsync(CrearTipoProductoRequest req) =>
        PostAsync<TipoProducto>("tiposproducto", req);

    public Task<TipoProducto> EditarTipoProductoAsync(int id, CrearTipoProductoRequest req) =>
        PutAsync<TipoProducto>($"tiposproducto/{id}", req);

    public Task EliminarTipoProductoAsync(int id) =>
        http.DeleteAsync($"tiposproducto/{id}");

    // ── Productos ─────────────────────────────────────────────
    public Task<List<Producto>> GetProductosAsync() =>
        GetAsync<List<Producto>>("productos");

    public Task<Producto> CrearProductoAsync(CrearProductoRequest req) =>
        PostAsync<Producto>("productos", req);

    public Task<Producto> EditarProductoAsync(int id, CrearProductoRequest req) =>
        PutAsync<Producto>($"productos/{id}", req);

    public Task EliminarProductoAsync(int id) =>
        http.DeleteAsync($"productos/{id}");

    public Task<CalcularPrecioResponse> CalcularPrecioAsync(CalcularPrecioRequest req) =>
        PostAsync<CalcularPrecioResponse>("productos/calcular-precio", req);

    // ── Clientes ──────────────────────────────────────────────
    public Task<List<Cliente>> GetClientesAsync() =>
        GetAsync<List<Cliente>>("clientes");

    public Task<Cliente> CrearClienteAsync(CrearClienteRequest req) =>
        PostAsync<Cliente>("clientes", req);

    public Task<Cliente> EditarClienteAsync(int id, CrearClienteRequest req) =>
        PutAsync<Cliente>($"clientes/{id}", req);

    public Task EliminarClienteAsync(int id) =>
        http.DeleteAsync($"clientes/{id}");

    // ── Ventas ────────────────────────────────────────────────
    public Task<List<Venta>> GetVentasAsync(string? estado = null, int? clienteId = null)
    {
        var qs = new List<string>();
        if (!string.IsNullOrEmpty(estado)) qs.Add($"estado={estado}");
        if (clienteId.HasValue) qs.Add($"clienteId={clienteId}");
        var url = qs.Count > 0 ? $"ventas?{string.Join("&", qs)}" : "ventas";
        return GetAsync<List<Venta>>(url);
    }

    public Task<Venta> GetVentaAsync(int id) =>
        GetAsync<Venta>($"ventas/{id}");

    public Task<Venta> CrearVentaAsync(CrearVentaRequest req) =>
        PostAsync<Venta>("ventas", req);

    public async Task CambiarEstadoAsync(int id, string estado) =>
        await http.PatchAsJsonAsync($"ventas/{id}/estado", new CambiarEstadoRequest(estado));

    public Task EliminarVentaAsync(int id) =>
        http.DeleteAsync($"ventas/{id}");

    // ── Helpers ───────────────────────────────────────────────
    private async Task<T> GetAsync<T>(string url)
    {
        var res = await http.GetAsync(url);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<T>(_opts))!;
    }

    private async Task<T> PostAsync<T>(string url, object body)
    {
        var res = await http.PostAsJsonAsync(url, body);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<T>(_opts))!;
    }

    private async Task<T> PutAsync<T>(string url, object body)
    {
        var res = await http.PutAsJsonAsync(url, body);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<T>(_opts))!;
    }
}
