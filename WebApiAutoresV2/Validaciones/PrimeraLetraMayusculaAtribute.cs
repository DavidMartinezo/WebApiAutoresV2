using System.ComponentModel.DataAnnotations;//para que funcione el ValidationAttribute

namespace WebApiAutoresV2.Validaciones
{
    public class PrimeraLetraMayusculaAtribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
           var primeraLetra=value.ToString()[0].ToString();
            if(primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("la primera letra debe ser mayuscula");
            }
            return ValidationResult.Success;
        }
    }
}
