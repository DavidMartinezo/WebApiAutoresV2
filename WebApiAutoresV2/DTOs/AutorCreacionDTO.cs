
using System.ComponentModel.DataAnnotations;
using WebApiAutoresV2.Validaciones;

namespace WebApiAutoresV2.DTOs
{
    public class AutorCreacionDTO 
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe ser más de {1} carácteres")]
        [PrimeraLetraMayusculaAtribute]
        public string Nombre { get; set; }
    }
}
