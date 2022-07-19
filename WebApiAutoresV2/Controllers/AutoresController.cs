using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApiAutoresV2.Entidades;
using WebApiAutoresV2.Filtros;
using WebApiAutoresV2.Servicios;

namespace WebApiAutoresV2.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IServicio servicio;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioTransient servicioTransient;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(ApplicationDBContext context, 
                                 IServicio servicio, 
                                 ServicioSingleton servicioSingleton,
                                 ServicioScoped servicioScoped, 
                                 ServicioTransient servicioTransient,
                                 ILogger<AutoresController> logger)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioSingleton = servicioSingleton;
            this.servicioScoped = servicioScoped;
            this.servicioTransient = servicioTransient;
            this.logger = logger;
        }
       [HttpGet("GUID")]
       [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObtenerGuids()
        {
            return Ok( new{
                AutoresControllerTransient = servicioTransient.guid,
                ServicioA_transient=servicio.ObtenerTransient(),
                AutorescontrollerScoped= servicioScoped.guid,
                ServicioA_Scoped=servicio.ObtenerScoped(),
                AutoresControllerSingleton = servicioSingleton.guid,
                ServicioA_SingleTon=servicio.ObtenerSingleton()
            });

        }
        [HttpGet("/HEADER")]
        
        public string Header([FromHeader] string valor, [FromQuery] string nombre) {

            return valor;

        }
        [HttpGet("/BODY")]
        public string Body([FromQuery] string valor)
        {

            return valor;

        }
        [HttpGet("/FromForm")]
        public string FromForm([FromForm] string valor)
        {

            return valor;

        }
        [HttpGet]
        [HttpGet("/listado")] //--> listado como ruta.
        public async Task<ActionResult<List<Autor>>> Get()
        {
            throw new NotImplementedException();
            logger.LogInformation("estamos obteniendo los autores");
            servicio.realizarTarea();
            //return new List<Autor>() { 
            //                            new Autor() { Id = 1, Nombre = "David" },
            //                            new Autor() {  Id = 2, Nombre ="Sharlen"}
            //
            //  };
            return await context.Autores.Include(x => x.libros).ToListAsync();
        }
        [HttpGet("primer")]
        public async Task<ActionResult<Autor>> PrimerAutor()
        {
            return await context.Autores.FirstOrDefaultAsync();
        }
        [HttpGet("id:int")]
        public async Task<ActionResult<Autor>> PrimerAutor(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return BadRequest("El registro no existe");
            }
            return await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> PrimerAutor(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor==null) 
            {
                return BadRequest("El registro no existe");
            }
            return autor;
        }
        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            //validaciones por controlador 
            var existeAutorConNombre= await context.Autores.AnyAsync(x => x.Nombre==autor.Nombre);  
            if (!existeAutorConNombre)
            {
                context.Add(autor);
                await context.SaveChangesAsync();
                return Ok();
            }else
            {
                return BadRequest($"Existe Autor con nombre {autor.Nombre}");
            }
          
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id no coincide");
            }
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound("El registro no existe");
            }
            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("id:int")]
        public async Task<ActionResult> Delete(int id)
        {
            //any signfica si existe alguno con el id que se provee
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound("El registro no existe");
            }
            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
