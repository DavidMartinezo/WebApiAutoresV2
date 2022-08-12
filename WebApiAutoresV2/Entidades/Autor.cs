using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutoresV2.Validaciones;

namespace WebApiAutoresV2.Entidades
{
    public class Autor 
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:120 , ErrorMessage = "El campo {0} no debe ser más de {1} carácteres")]
        [PrimeraLetraMayusculaAtribute]
        public string Nombre { get; set; }
        }
}

