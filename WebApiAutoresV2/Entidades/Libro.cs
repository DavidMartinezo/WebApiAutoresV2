using System.ComponentModel.DataAnnotations;
using WebApiAutoresV2.Validaciones;

namespace WebApiAutoresV2.Entidades
{
    public class Libro
    {
        public int id { get; set; }
        [Required]
        [PrimeraLetraMayusculaAtribute]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no puede ser más de {1}")]
        public string Titulo { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
