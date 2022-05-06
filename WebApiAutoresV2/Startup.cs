using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutoresV2.Filtros;
using WebApiAutoresV2.Middleares;
using WebApiAutoresV2.Servicios;

namespace WebApiAutoresV2;
    public class Startup
    {
    //constructor de la clase
    public Startup(IConfiguration configuration)
    {
        Configuration=configuration;
    }
    //propiedad configuration 
    public IConfiguration Configuration {  get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
       //para evitar la referencia circular en las clasess libro y autor
        services.AddControllers( opciones =>
        {
            opciones.Filters.Add(typeof(FiltroDeExepcion));
        }).AddJsonOptions(x => 
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddTransient<IServicio, ServicioA>();

        services.AddTransient<ServicioTransient>();
        services.AddScoped<ServicioScoped>();
        services.AddSingleton<ServicioSingleton>();
        services.AddTransient<MiFiltroDeAccion>();



        services.AddDbContext < ApplicationDBContext> (options=>
        options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
        services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WebApiAutoresV2", Version = "v1" });
});
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        /*tuberias middleware*/
        //app.UseMiddleware<LogguearRespuestaHTTPMiddleware>();
        app.UseLoguearRespuestaHTTP();
        app.Map("/ruta1", app =>
        {
            app.Run(async contexto =>
            {
                await contexto.Response.WriteAsync("interceptando la tuberia");

            });
        });
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutoresV2 v1"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

