using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class DetalleVenta {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DetalleVentaId {get; set;}

    public int Cantidad {get; set;}

    public int VentaId {get; set;}
    public Venta Venta {get; set;} = null!;
    
    public int ProductoId {get; set;}
    public Producto Producto {get; set;} = null!;
}