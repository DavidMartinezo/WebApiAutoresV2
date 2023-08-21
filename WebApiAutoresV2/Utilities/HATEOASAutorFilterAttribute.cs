using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutoresV2.DTOs;
using WebApiAutoresV2.Servicios;

namespace WebApiAutoresV2.Utilities
{
    public class HATEOASAutorFilterAttribute : HATEOASFilterAttribute
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);
            if (!debeIncluir)
            {
                await next();
            }
            var resultado = context.Result as ObjectResult;
            var autorDTO = resultado.Value as AutorDTO;
            if (autorDTO is null)
            {
                //verificar si es un listado 
                var autoresDTO = resultado.Value as List<AutorDTO> ??
                    throw new ArgumentNullException("Se esperaba una instancia de AutorDTO o Listado de AutorDTO");
                //leemos cada elemento y pasamos el objecto resultante autor 
                autoresDTO.ForEach(async autor => await generadorEnlaces.GenerarEnlaces(autor));
                resultado.Value = autoresDTO;

            }
            else
            {
                //generando los enlaces para ello creamos un servicio reutilizando la logica del metodo generarEnlaces
                await generadorEnlaces.GenerarEnlaces(autorDTO);
            }

            await next();


        }
    }
}
