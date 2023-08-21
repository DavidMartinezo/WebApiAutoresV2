using System.ComponentModel.DataAnnotations;
using WebApiAutoresV2.Validaciones;

namespace WebApiAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAtributeTest //debe tener el metodoPrueba+test
    {
        [TestMethod]
        public void ValorNulo_NoDevulveError()
        {
            ///verificar que si envio un dato con minuscula tengo error 
            ///
            //preparacion 
            var PrimeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            var valor = "david";
            //necesitamos un contexto de valicacion 
            var valContext = new ValidationContext(new {Nombre = valor});   

            //ejecucion 
            var resultado = PrimeraLetraMayuscula.GetValidationResult(valor, valContext);
            //verificacion 
            //assert es una clase que permite hacer verificaciones si la verificacion no es satisfactoria arroja error 
            Assert.AreEqual("la primera letra debe ser mayuscula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void ValorConMayusculaNoDevuelveError()
        {
            ///verificar que si envio un dato nulo no obtengo tengo error 
            ///
            //preparacion 
            var PrimeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            string valor = null;
            //necesitamos un contexto de valicacion 
            var valContext = new ValidationContext(new { Nombre = valor });

            //ejecucion 
            var resultado = PrimeraLetraMayuscula.GetValidationResult(valor, valContext);
            //verificacion 
            //assert es una clase que permite hacer verificaciones si la verificacion no es satisfactoria arroja error 
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorNuloNoDevuelveError()
        {
            ///verificar que si envio un dato nulo no obtengo tengo error 
            ///
            //preparacion 
            var PrimeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            string valor = "David";
            //necesitamos un contexto de valicacion 
            var valContext = new ValidationContext(new { Nombre = valor });

            //ejecucion 
            var resultado = PrimeraLetraMayuscula.GetValidationResult(valor, valContext);
            //verificacion 
            //assert es una clase que permite hacer verificaciones si la verificacion no es satisfactoria arroja error 
            Assert.IsNull(resultado);
        }


    }
}