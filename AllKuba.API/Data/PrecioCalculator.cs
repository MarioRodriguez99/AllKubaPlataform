using System.Text.Json;
using AllKuba.API.DTOs;
using AllKuba.API.Models;

namespace AllKuba.API.Data;

public static class PrecioCalculator
{
    public record Resultado(
        decimal PrecioUnitario,
        decimal PrecioCostoUnitario,
        string Detalle
    );

    public static Resultado Calcular(Producto p, decimal ancho, decimal alto, Dictionary<string, string>? variantes = null)
    {
        decimal precioBase;
        decimal costoBase;
        string detalle;

        if (!p.TieneMedidas)
        {
            // Producto sin medidas — precio fijo
            precioBase = p.PrecioVenta > 0 ? p.PrecioVenta : p.PrecioBase;
            costoBase = p.PrecioCosto;
            detalle = "Precio fijo";
        }
        else
        {
            bool superaEstandar = ancho > p.AnchoEstandar || alto > p.AltoEstandar;

            if (!superaEstandar || p.TipoCalculo == "ninguno")
            {
                precioBase = p.PrecioBase;
                costoBase = p.PrecioCosto;
                detalle = "Precio base (medida estándar)";
            }
            else if (p.TipoCalculo == "m2")
            {
                decimal m2 = ancho * alto;
                precioBase = m2 * p.PrecioExceso;
                costoBase = m2 * p.PrecioCostoExceso;
                detalle = $"{ancho} × {alto} = {m2:F2} m² × ${p.PrecioExceso}/m²";
            }
            else // ml
            {
                precioBase = ancho * p.PrecioExceso;
                costoBase = ancho * p.PrecioCostoExceso;
                detalle = $"{ancho} ml × ${p.PrecioExceso}/ml";
            }
        }

        // Sumar precio extra de variantes
        decimal extraVariantes = 0;
        if (variantes != null && variantes.Any())
        {
            try
            {
                var variantesProducto = JsonSerializer.Deserialize<List<VarianteDto>>(p.VariantesJson);
                if (variantesProducto != null)
                {
                    foreach (var (tipo, opcionElegida) in variantes)
                    {
                        var varianteDef = variantesProducto.FirstOrDefault(v => v.Tipo == tipo);
                        if (varianteDef != null)
                        {
                            var opcion = varianteDef.Opciones.FirstOrDefault(o => o.Nombre == opcionElegida);
                            if (opcion != null && opcion.PrecioExtra > 0)
                                extraVariantes += opcion.PrecioExtra;
                        }
                    }
                }
            }
            catch { }
        }

        return new(precioBase + extraVariantes, costoBase, detalle);
    }
}
