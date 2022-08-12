using System.ComponentModel.DataAnnotations;
namespace WebApiAutoresV2.Entidades
{
    public class Libro
    {
        public int id { get; set; }
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no puede ser más de {1}")]
        public string Titulo { get; set; }
        public int AutorId { get; set; }
    }
}
