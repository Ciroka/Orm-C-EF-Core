using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PracticoOrm.Models;


public class Ingrediente
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IngredienteId {get; set;}

    [MaxLength(100)]
    public string Nombre {get; set;} = null!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Costo{get; set;}

    public ICollection<DetalleReceta> DetalleRecetas {get; set;} = [];

    public override string ToString() => Nombre;
}