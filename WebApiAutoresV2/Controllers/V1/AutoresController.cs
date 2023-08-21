using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutoresV2.DTOs;
using WebApiAutoresV2.Entidades;
using WebApiAutoresV2.Filtros;
using WebApiAutoresV2.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiAutoresV2.Utilities;

namespace WebApiAutoresV2.Controllers.V1 //cambiamos el namespace
{
    [ApiController]
    [Route("api/autores")]
    //[Route("api/V1/autores")]
    [ApiConventionType(typeof(DefaultApiConventions))]//indicando las convenciones que quiero indicar no undocumented
    [CabeceraPresente("x-version","1")] //[CabeceraPresente("cualquier valor deseado para cabecera","valor")] 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        //private readonly IConfiguration configuration;

        public AutoresController(ApplicationDBContext context,
                                      IMapper mapper, IAuthorizationService authorizationService
                                      /* IConfiguration configuration*/)
        {
            this.context = context;
            // this.logger = logger;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
            //this.configuration = configuration;
        }
        [HttpGet("/HEADER")]

        public string Header([FromHeader] string valor, [FromQuery] string nombre)
        {

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


        #region Gets


        [HttpGet(Name = "obtenerAutores")] //api/autores
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryAble = context.Autores.AsQueryable();//representa un tipo al que se le puede hacer queries 
            await HttpContext.IsertarParametrosPaginacionEnCabecera(queryAble);//pasamos esta variable al metodo de extension que creamos
            var autores = await queryAble.OrderBy(a => a.Nombre).Paginar(paginacionDTO).ToListAsync(); 
            return mapper.Map<List<AutorDTO>>(autores);
           
        }

        [HttpGet("{id:int}", Name = "obtenerAutor")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorConLibroDTO>> GetAutor(int id)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existeAutor) { return BadRequest($"El autor con Id {id} no existe"); }
            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");
            var autor = await context.Autores
                .Include(autorDb => autorDb.AutoresLibros)
                .ThenInclude(autorLibroDb => autorLibroDb.Libro)
                .FirstAsync(autorDb => autorDb.Id == id);
            if (autor == null)
            {
                return BadRequest("El registro no existe");
            }
            var dto = mapper.Map<AutorConLibroDTO>(autor);
           // GenerarEnlaces(dto, isAdmin.Succeeded);
            return dto;
        }

        [HttpGet("{nombre}", Name = "obtenerAutoresPorNombre")]
        public async Task<List<AutorDTO>> PrimerAutor(string nombre)
        {
            var autores = await context.Autores.Where(autorBd =>
            autorBd.Nombre.Contains(nombre)).ToListAsync();


            return mapper.Map<List<AutorDTO>>(autores);
        }
        [HttpGet("GetPrimerAutor")]
        public async Task<AutorDTO> GetPrimerAutor([FromHeader] string nombre, [FromQuery] string apellido)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).FirstOrDefaultAsync();


            return mapper.Map<AutorDTO>(autores);
        }

        [HttpGet("/listadoAutores")]
        [AllowAnonymous]
        public async Task<List<AutorDTO>> GetAutoresAsync()
        {

            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }
        #endregion

        #region Post
        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post(AutorCreacionDTO AutorCreacionDTO)
        {
            //validaciones por controlador 
            var existeAutorConNombre = await context.Autores.AnyAsync(x => x.Nombre == AutorCreacionDTO.Nombre);
            if (!existeAutorConNombre)
            {
                var autor = mapper.Map<Autor>(AutorCreacionDTO);
                context.Add(autor);
                await context.SaveChangesAsync();
                var autorDto = mapper.Map<AutorDTO>(autor);
                return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDto);
            }
            else
            {
                return BadRequest($"Existe Autor con nombre {AutorCreacionDTO.Nombre}");
            }


        }
        #endregion
        #region PUT
        [HttpPut("{id:int}", Name = "actualizarAutor")] // api/autores/1 
        public async Task<ActionResult> Put(int id, AutorCreacionDTO autorCreacionDTO)
        {
            //actualizar de forma completa 

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound("El registro no existe");
            }
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        #endregion


        /// <summary>
        /// borra un autor
        /// </summary>
        /// <param name="id">id del autor a borrar.</param>
        /// <returns></returns>
        #region Delete
        [HttpDelete("{id:int}", Name = "borrarAutor")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound("El registro no existe");
            }
            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
        #endregion


       
    }
}
