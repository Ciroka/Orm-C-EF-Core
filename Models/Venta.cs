using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class Venta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VentaId {get; set;}

    public DateTime FechaDeVenta {get; set;} = DateTime.UtcNow;

    public int MostradorId {get; set;}
    
    public Mostrador Mostrador {get; set;} = null!;

    public ICollection<DetalleVenta> DetalleVentas {get; set;} = [];
}