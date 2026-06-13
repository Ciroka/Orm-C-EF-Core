using System.ComponentModel.DataAnnotations.Schema;
using PracticoOrm.Models;

namespace PracticoOrm.Models;

public class DetalleReceta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DetalleRecetaId {get; set;}

    [Column(TypeName = "decimal(10,2)")]
    public decimal Cantidad {get; set;}

    public int RecetaId {get; set;}

    public Receta Receta {get; set;} = null!;

    public int IngredienteId {get; set;}

    public Ingrediente Ingrediente {get; set;} = null!;
}