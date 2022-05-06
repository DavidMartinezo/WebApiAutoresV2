namespace WebApiAutoresV2.Middleares
{
    public static class LoguearRespuestaHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogguearRespuestaHTTPMiddleware>();
        }
    }
    public class LogguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LogguearRespuestaHTTPMiddleware> logger;

        public LogguearRespuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LogguearRespuestaHTTPMiddleware> logger )
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }
        //para poder invacar esta clase como middleware debe tener un metodo publico invoke or invokeasync este debe enviar un task
        public async Task Invoke(HttpContext context)
        {
            using (var ms= new MemoryStream())
            {
                var cuerpoOriginalRespuesta = context.Response.Body;
                context.Response.Body = ms;
                await siguiente(context);
                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0,SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                context.Response.Body = cuerpoOriginalRespuesta;
                logger.LogInformation(respuesta);   

            }
        }
    }
}
