using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Reflection.Metadata;

namespace WebApiAutoresV2.Utilities
{
    public class CabeceraPresenteAttribute : Attribute, IActionConstraint //el utltimo permite terner logica por enponit para decidir cual se va a ejecutar 
    {
        private readonly string cabecera;
        private readonly string valor;

        public int Order => 0;//throw new NotImplementedException();


        public CabeceraPresenteAttribute(string cabecera, string valor)
        {
            this.cabecera = cabecera;
            this.valor = valor;
        }
        public bool Accept(ActionConstraintContext context)
        {
            //logica para determinar si la cabecera tiene la cabecera y el valor indicada usaremos el enpoint 
            var cabeceras = context.RouteContext.HttpContext.Request.Headers;// throw new NotImplementedException();
            if(!cabeceras.ContainsKey(cabecera))
            {
                return false;
            }
            return string.Equals(cabeceras[cabecera],valor, StringComparison.OrdinalIgnoreCase);
        }
    }
}
