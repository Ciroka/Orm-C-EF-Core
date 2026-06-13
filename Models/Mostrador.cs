using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class Mostrador
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MostradorId {get; set;}


    [MaxLength(100)]
    public string Nombre {get; set;} = null!;

    public int PuntoDeVentaId {get; set;}

    public PuntoDeVenta PuntoDeVenta{get; set;} = null!;

    public ICollection<Venta> Ventas {get; set;} = [];
}