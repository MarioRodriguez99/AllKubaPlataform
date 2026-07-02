namespace AllKuba.API.DTOs;

// ── Variantes ─────────────────────────────────────────────────
public record OpcionVarianteDto(string Nombre, decimal PrecioExtra);
public record VarianteDto(string Tipo, List<OpcionVarianteDto> Opciones);

// ── Tipos de Producto ─────────────────────────────────────────
public record TipoProductoDto(
    int Id,
    string Nombre,
    string Descripcion,
    bool TieneMedidas,
    decimal PrecioBase,
    decimal AnchoEstandar,
    decimal AltoEstandar,
    string TipoCalculo,
    decimal PrecioExceso,
    decimal PrecioCosto,
    decimal PrecioCostoExceso,
    decimal PrecioVenta,
    List<VarianteDto> Variantes
)
{
    public string TipoCalculoLabel => TipoCalculo switch
    {
        "m2" => "Por m²",
        "ml" => "Por metro lineal",
        _ => "Precio fijo"
    };
}

public record CrearTipoProductoRequest(
    string Nombre,
    string Descripcion,
    bool TieneMedidas,
    decimal PrecioBase,
    decimal AnchoEstandar,
    decimal AltoEstandar,
    string TipoCalculo,
    decimal PrecioExceso,
    decimal PrecioCosto,
    decimal PrecioCostoExceso,
    decimal PrecioVenta,
    List<VarianteDto> Variantes
);

// ── Productos ─────────────────────────────────────────────────
public record ProductoDto(
    int Id,
    string Nombre,
    string Descripcion,
    int? TipoProductoId,
    string? TipoProductoNombre,
    bool TieneMedidas,
    decimal PrecioBase,
    decimal AnchoEstandar,
    decimal AltoEstandar,
    string TipoCalculo,
    decimal PrecioExceso,
    decimal PrecioCosto,
    decimal PrecioCostoExceso,
    decimal PrecioVenta,
    List<VarianteDto> Variantes,
    bool Activo
)
{
    public string TipoCalculoLabel => TipoCalculo switch
    {
        "m2" => "Por m²",
        "ml" => "Por metro lineal",
        _ => "Precio fijo"
    };
}

public record CrearProductoRequest(
    string Nombre,
    string Descripcion,
    int? TipoProductoId,
    bool TieneMedidas,
    decimal PrecioBase,
    decimal AnchoEstandar,
    decimal AltoEstandar,
    string TipoCalculo,
    decimal PrecioExceso,
    decimal PrecioCosto,
    decimal PrecioCostoExceso,
    decimal PrecioVenta,
    List<VarianteDto> Variantes
);

public record CalcularPrecioRequest(int ProductoId, decimal Ancho, decimal Alto, int Cantidad, Dictionary<string, string> VariantesSeleccionadas);
public record CalcularPrecioResponse(decimal PrecioUnitario, decimal PrecioCostoUnitario, decimal Subtotal, decimal SubtotalCosto, string Detalle);

// ── Clientes ──────────────────────────────────────────────────
public record ClienteDto(int Id, string Nombre, List<string> Telefonos, string Email, string Direccion)
{
    public string TelefonosDisplay => Telefonos.Any() ? string.Join(" / ", Telefonos) : "—";
}

public record CrearClienteRequest(string Nombre, List<string> Telefonos, string Email, string Direccion);

// ── Ventas ────────────────────────────────────────────────────
public record LineaVentaDto(
    int Id, int ProductoId, string NombreProducto,
    decimal Ancho, decimal Alto, int Cantidad,
    Dictionary<string, string> VarianteSeleccionada,
    decimal PrecioUnitario, decimal PrecioCostoUnitario,
    decimal Subtotal, decimal SubtotalCosto, string Notas
)
{
    public string MedidasDisplay => Ancho > 0 && Alto > 0 ? $"{Ancho}m × {Alto}m" : "—";
    public string VariantesDisplay => VarianteSeleccionada.Any()
        ? string.Join(", ", VarianteSeleccionada
            .Where(kv => !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{kv.Key}: {kv.Value}"))
        : "—";
}

public record VentaDto(
    int Id, string Numero, string Estado,
    ClienteDto Cliente, List<LineaVentaDto> Lineas,
    decimal Total, decimal TotalCosto,
    string Observaciones, DateTime CreadoEn
)
{
    public string EstadoLabel => Estado switch
    {
        "pendiente" => "Pendiente",
        "aprobado" => "Aprobado",
        "en_produccion" => "En producción",
        "entregado" => "Entregado",
        "cancelado" => "Cancelado",
        _ => Estado
    };
    public string FechaDisplay => CreadoEn.ToLocalTime().ToString("dd/MM/yyyy");
    public decimal Ganancia => Total - TotalCosto;
}

public record CrearLineaRequest(
    int ProductoId, decimal Ancho, decimal Alto, int Cantidad,
    Dictionary<string, string> VarianteSeleccionada, string Notas
);
public record CrearVentaRequest(int ClienteId, string Observaciones, List<CrearLineaRequest> Lineas);
public record CambiarEstadoRequest(string Estado);

// ── Dashboard ─────────────────────────────────────────────────
public record DashboardDto(
    int TotalVentas, int VentasEntregadas, int VentasPendientes, int VentasEnProduccion,
    decimal IngresoTotal, decimal CostoTotal, decimal GananciaTotal, decimal MargenPromedio,
    List<VentaPorMesDto> VentasPorMes, List<ProductoTopDto> ProductosTop
);
public record VentaPorMesDto(string Mes, decimal Ingresos, decimal Costos, decimal Ganancia);
public record ProductoTopDto(string Nombre, int Cantidad, decimal Ingresos, decimal Ganancia);
