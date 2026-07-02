namespace AllKuba.UI.Models;

// ── Variantes ─────────────────────────────────────────────────
public class OpcionVariante
{
    public string Nombre { get; set; } = "";
    public decimal PrecioExtra { get; set; } = 0;
}

public class VarianteDefinicion
{
    public string Tipo { get; set; } = "";
    public List<OpcionVariante> Opciones { get; set; } = [];
}

public class VarianteSeleccion
{
    public string Tipo { get; set; } = "";
    public List<OpcionVariante> Opciones { get; set; } = [];
    public string Seleccion { get; set; } = "";
}

public class Variante
{
    public string Tipo { get; set; } = "";
    public List<OpcionVariante> Opciones { get; set; } = [];
}

// ── Tipo Producto ─────────────────────────────────────────────
public class TipoProducto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public bool TieneMedidas { get; set; } = true;
    public decimal PrecioBase { get; set; }
    public decimal AnchoEstandar { get; set; }
    public decimal AltoEstandar { get; set; }
    public string TipoCalculo { get; set; } = "m2";
    public decimal PrecioExceso { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal PrecioCostoExceso { get; set; }
    public decimal PrecioVenta { get; set; }
    public List<VarianteDefinicion> Variantes { get; set; } = [];

    public string TipoCalculoLabel => TipoCalculo switch
    {
        "m2" => "Por m²",
        "ml" => "Por metro lineal",
        _ => "Precio fijo"
    };
}

public record CrearTipoProductoRequest(
    string Nombre, string Descripcion, bool TieneMedidas,
    decimal PrecioBase, decimal AnchoEstandar, decimal AltoEstandar,
    string TipoCalculo, decimal PrecioExceso,
    decimal PrecioCosto, decimal PrecioCostoExceso, decimal PrecioVenta,
    List<VarianteDefinicion> Variantes
);

// ── Producto ─────────────────────────────────────────────────
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int? TipoProductoId { get; set; }
    public string? TipoProductoNombre { get; set; }
    public bool TieneMedidas { get; set; } = true;
    public decimal PrecioBase { get; set; }
    public decimal AnchoEstandar { get; set; }
    public decimal AltoEstandar { get; set; }
    public string TipoCalculo { get; set; } = "ninguno";
    public decimal PrecioExceso { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal PrecioCostoExceso { get; set; }
    public decimal PrecioVenta { get; set; }
    public List<VarianteDefinicion> Variantes { get; set; } = [];
    public bool Activo { get; set; } = true;

    public string TipoCalculoLabel => TipoCalculo switch
    {
        "m2" => "Por m²",
        "ml" => "Por metro lineal",
        _ => "Precio fijo"
    };
}

public record CrearProductoRequest(
    string Nombre, string Descripcion,
    int? TipoProductoId, bool TieneMedidas,
    decimal PrecioBase, decimal AnchoEstandar, decimal AltoEstandar,
    string TipoCalculo, decimal PrecioExceso,
    decimal PrecioCosto, decimal PrecioCostoExceso, decimal PrecioVenta,
    List<VarianteDefinicion> Variantes
);

public record CalcularPrecioRequest(int ProductoId, decimal Ancho, decimal Alto, int Cantidad, Dictionary<string, string> VariantesSeleccionadas);
public record CalcularPrecioResponse(decimal PrecioUnitario, decimal PrecioCostoUnitario, decimal Subtotal, decimal SubtotalCosto, string Detalle);

// ── Cliente ───────────────────────────────────────────────────
public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public List<string> Telefonos { get; set; } = [];
    public string Email { get; set; } = "";
    public string Direccion { get; set; } = "";

    public string TelefonosDisplay => Telefonos.Any() ? string.Join(" / ", Telefonos) : "—";
}

public record CrearClienteRequest(string Nombre, List<string> Telefonos, string Email, string Direccion);

// ── Venta ─────────────────────────────────────────────────────
public class LineaVenta
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = "";
    public decimal Ancho { get; set; }
    public decimal Alto { get; set; }
    public int Cantidad { get; set; }
    public Dictionary<string, string> VarianteSeleccionada { get; set; } = [];
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioCostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public decimal SubtotalCosto { get; set; }
    public string Notas { get; set; } = "";

    public string MedidasDisplay => Ancho > 0 && Alto > 0 ? $"{Ancho}m × {Alto}m" : "—";
    public string VariantesDisplay => VarianteSeleccionada.Any()
        ? string.Join(", ", VarianteSeleccionada
            .Where(kv => !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{kv.Key}: {kv.Value}"))
        : "—";
}

public class Venta
{
    public int Id { get; set; }
    public string Numero { get; set; } = "";
    public string Estado { get; set; } = "";
    public Cliente Cliente { get; set; } = new();
    public List<LineaVenta> Lineas { get; set; } = [];
    public decimal Total { get; set; }
    public decimal TotalCosto { get; set; }
    public string Observaciones { get; set; } = "";
    public DateTime CreadoEn { get; set; }

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
public class DashboardDto
{
    public int TotalVentas { get; set; }
    public int VentasEntregadas { get; set; }
    public int VentasPendientes { get; set; }
    public int VentasEnProduccion { get; set; }
    public decimal IngresoTotal { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal GananciaTotal { get; set; }
    public decimal MargenPromedio { get; set; }
    public List<VentaPorMesDto> VentasPorMes { get; set; } = [];
    public List<ProductoTopDto> ProductosTop { get; set; } = [];
}

public class VentaPorMesDto { public string Mes { get; set; } = ""; public decimal Ingresos { get; set; } public decimal Costos { get; set; } public decimal Ganancia { get; set; } }
public class ProductoTopDto { public string Nombre { get; set; } = ""; public int Cantidad { get; set; } public decimal Ingresos { get; set; } public decimal Ganancia { get; set; } }

// ── NuevaVenta helpers ────────────────────────────────────────
public class LineaPreview
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = "";
    public bool TieneMedidas { get; set; } = true;
    public decimal Ancho { get; set; }
    public decimal Alto { get; set; }
    public int Cantidad { get; set; }
    public Dictionary<string, string> VarianteSeleccionada { get; set; } = [];
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioCostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public decimal SubtotalCosto { get; set; }
    public string Notas { get; set; } = "";

    public string MedidasDisplay => TieneMedidas && Ancho > 0 ? $"{Ancho}m × {Alto}m" : "—";
    public string VariantesDisplay => VarianteSeleccionada.Any()
        ? string.Join(", ", VarianteSeleccionada
            .Where(kv => !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{kv.Key}: {kv.Value}"))
        : "—";
}