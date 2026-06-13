using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class TipoProducto
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TipoProductoId {get; set;}

    [MaxLength(100)]
    public string Nombre {get; set;} = null!;

    public ICollection<Producto> Productos {get; set;} = [];

    public override string ToString() => Nombre;
}