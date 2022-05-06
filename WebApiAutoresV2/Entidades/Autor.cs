using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutoresV2.Validaciones;

namespace WebApiAutoresV2.Entidades
{
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }
       // [PrimeraLetraMayusculaAtribute]
        public string Nombre { get; set; }
        public List<Libro> libros { get; set; }
        [NotMapped]
        //[CreditCard]
        public string Tarjeta { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula",
                        new string[] { nameof(Nombre) });
                }
            }
        }

     

    }
}
