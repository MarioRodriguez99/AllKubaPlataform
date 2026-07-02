namespace AllKuba.API.Models;

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
    public string VariantesJson { get; set; } = "[]";
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    public ICollection<Producto> Productos { get; set; } = [];
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int? TipoProductoId { get; set; }
    public TipoProducto? TipoProducto { get; set; }
    public bool TieneMedidas { get; set; } = true;
    public decimal PrecioBase { get; set; }
    public decimal AnchoEstandar { get; set; }
    public decimal AltoEstandar { get; set; }
    public string TipoCalculo { get; set; } = "ninguno";
    public decimal PrecioExceso { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal PrecioCostoExceso { get; set; }
    public decimal PrecioVenta { get; set; }
    public string VariantesJson { get; set; } = "[]";
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    public ICollection<LineaVenta> LineasVenta { get; set; } = [];
}

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string TelefonosJson { get; set; } = "[]";
    public string Email { get; set; } = "";
    public string Direccion { get; set; } = "";
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    public ICollection<Venta> Ventas { get; set; } = [];
}

public class Venta
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public string Numero { get; set; } = "";
    public string Observaciones { get; set; } = "";
    public string Estado { get; set; } = "pendiente";
    public decimal Total { get; set; }
    public decimal TotalCosto { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

    public ICollection<LineaVenta> Lineas { get; set; } = [];
}

public class LineaVenta
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public Venta Venta { get; set; } = null!;
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public string NombreProducto { get; set; } = "";
    public decimal Ancho { get; set; }
    public decimal Alto { get; set; }
    public int Cantidad { get; set; } = 1;
    public string VarianteSeleccionadaJson { get; set; } = "{}";
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioCostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public decimal SubtotalCosto { get; set; }
    public string Notas { get; set; } = "";
}