using System.ComponentModel.DataAnnotations;
using WebApiAutoresV2.Validaciones;

namespace WebApiAutoresV2.DTOs
{
    public class LibroCreacionDTO
    {
        [PrimeraLetraMayusculaAtribute]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no puede ser más de {1}")]
        public string Titulo { get; set; }
    }
}
