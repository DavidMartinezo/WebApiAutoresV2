using Microsoft.AspNetCore.Components.Web;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiAutoresV2.Utilities
{
    public class AgregarParametroHATEOAS : IOperationFilter
    {

        /// <summary>
        /// este metodo se hace necesario si se esta usando Swagger y se debe agregar en el startup SwaggerDoc
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //filtrando para incluir solamente en los metodos get 
            if(context.ApiDescription.HttpMethod != "GET")
            {
                return;
            }
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }
            operation.Parameters.Add(new OpenApiParameter
                {
                Name = "IncludeHATEOAS",
                In= ParameterLocation.Header,
                Required = false

            });
        }
    }
}
