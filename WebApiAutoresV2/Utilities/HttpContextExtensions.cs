using Microsoft.EntityFrameworkCore;

namespace WebApiAutoresV2.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task IsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queriable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            double cantidad = await queriable.CountAsync();
            //colocando en la cabecera de la respuesta el data de la cantidad de registros disponibles 
            httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());    
        }
    }
}
