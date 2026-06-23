using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class Producto
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductoId {get; set;}

    [MaxLength(100)]
    public string Nombre {get; set;} = null!;

    [MaxLength(100)]
    public string? Descripcion {get; set;}

    [Column(TypeName = "decimal(10,2)")]
    public decimal PorcentajeDeGanancia {get; set;}
    
    public int RecetaId {get; set;}
    public Receta Receta {get; set;} = null!;

    public int TipoProductoId {get; set;}
    public TipoProducto TipoProducto {get; set;} = null!;
    
    public ICollection<DetalleVenta> DetalleVentas {get; set;} = [];

    public override string ToString() => Nombre;
}