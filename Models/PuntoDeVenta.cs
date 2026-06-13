using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class PuntoDeVenta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PuntoDeVentaId {get; set;}

    [MaxLength(100)]
    public string Nombre {get; set;} = null!;

    public ICollection<Mostrador> Mostradores {get; set;} = [];

    public override string ToString ()=> Nombre;
}