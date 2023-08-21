using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutoresV2.Filtros;
using WebApiAutoresV2.Middleares;
using WebApiAutoresV2.Servicios;
using WebApiAutoresV2.Utilities;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApiAutoresV2;
public class Startup
{
    //constructor de la clase
    public Startup(IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        Configuration = configuration;
    }
    //propiedad configuration 
    public IConfiguration Configuration { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        //para el hash 
        services.AddTransient<HashService>();
        //para evitar la referencia circular en las clasess libro y autor
        services.AddControllers(opciones =>
       {
           opciones.Filters.Add(typeof(FiltroDeExepcion));
           //agregamos el filtro para agrupar controladores 
           opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
       }).AddJsonOptions(x =>
       x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

        //services.AddHostedService<EscribirEnArchivo>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llaveJwt"])),
                ClockSkew = TimeSpan.Zero

            });
        // services.AddResponseCaching();//para utilizar cache en nuesro proyecto agregar en Configure and configure services
        services.AddDbContext<ApplicationDBContext>(options =>
     options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
        services.AddSwaggerGen(c =>
          {
              c.SwaggerDoc("V1", new OpenApiInfo
              {
                  Title = "WebApiAutoresV",
                  Version = "V1",
                  Description = "Api para trabajar con autores y libros",
                  Contact = new
                  OpenApiContact
                  {
                      Email = "davidmartinezosorio@gmial.com",
                      Name = "David Martinez",
                      Url = new Uri("https://www.miurl.com")
                  },
                  License=new OpenApiLicense
                  {
                      Name="MIT"
                  }
              }); 
              c.SwaggerDoc("V2", new() { Title = "WebApiAutoresV2", Version = "V2" });
              //agreagando el parametro IncludeHATEOAS
              c.OperationFilter<AgregarParametroHATEOAS>();
              //agregando parametro para version 
              c.OperationFilter<AgregarParametroXversion>();
              
              //se agrega esto para configurar Swagger con JWT
              c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
              {
                  Name = "Authorization",
                  Type = SecuritySchemeType.ApiKey,
                  Scheme = "Bearer",
                  BearerFormat = "JWT",
                  In = ParameterLocation.Header
              });
              // se agrega esto para configurar Swagger con JWT
              c.AddSecurityRequirement(new OpenApiSecurityRequirement
              {
                  {
                  new OpenApiSecurityScheme
                  {
                      Reference = new OpenApiReference
                      {
                          Type = ReferenceType.SecurityScheme,
                          Id= "Bearer"
                      }
                  },
                  new string[]{}
                  }
              });
              //hasta aqui para configurar Swagger
              //para incluir comentarios en el xml de swagger /// <summary>
              var archivoXMl = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
              var rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXMl);
              c.IncludeXmlComments(rutaXml);
          });
        services.AddAutoMapper(typeof(Startup));
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();
        services.AddAuthorization(opciones =>
        {
            opciones.AddPolicy(
                "IsAdmin", politica => politica.RequireClaim("isAdmin"));
            opciones.AddPolicy(
                "EsVendedor", politica => politica.RequireClaim("esVendedor"));
        });
        services.AddDataProtection();
        services.AddCors(opciones =>
        {
            opciones.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("").AllowAnyMethod().AllowAnyHeader()
                .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
                //para permitir headers se debe indicar  .WithExposedHeaders()

            });
        });

        //registrnado los servicios de HATEOAS 
        services.AddTransient<GeneradorEnlaces>();
        services.AddTransient<HATEOASAutorFilterAttribute>();
        services.AddSingleton<IActionContextAccessor,  ActionContextAccessor>();
        //agregando la configuracion para appInsights 
        services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);

    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        /*tuberias middleware*/
        // app.UseMiddleware<LogguearRespuestaHTTPMiddleware>(); //esta es la misma que la de abajo pero sin pasar la clase LogguearRespuestaHTTPMiddleware
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
          
        }
        app.UseSwagger();
        app.UseSwaggerUI(c => {
            c.SwaggerEndpoint("/swagger/V1/swagger.json", "WebApiAutoresV2 V1");
            c.SwaggerEndpoint("/swagger/V2/swagger.json", "WebApiAutoresV2 V2");

        });

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        //app.UseResponseCaching();
        app.UseAuthorization(); //asegurarse que tenga este para hacer uso de autorizathion  antes de mappear todos los controladoers en use UseEndpoints

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

