using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApiAutoresV2.Validaciones
{
    public class EmailValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(value.ToString());
            return match.Success ? ValidationResult.Success : new ValidationResult("Email format incorrect");

        }
    }
}
