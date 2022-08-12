using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApiAutoresV2.DTOs;
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
        private readonly IMapper mapper;

        public AutoresController(ApplicationDBContext context,
                                     ILogger<AutoresController> logger, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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
        [HttpGet]
        public async Task<List<Autor>> get()
        {

            return await context.Autores.ToListAsync();
        }

        [HttpGet("id:int")]
        public async Task<ActionResult<AutorDTO>> PrimerAutor(int id)
        {
            var autor = await context.Autores.FirstAsync(autorDb => autorDb.Id == id);
            if (autor == null)
            {
                return BadRequest("El registro no existe");
            }
            return mapper.Map<AutorDTO>(autor);
        }

        [HttpGet("{nombre}")]
        public async Task<List<AutorDTO>> PrimerAutor(string nombre)
        {
            var autores = await context.Autores.Where(autorBd => autorBd.Nombre.Contains(nombre)).ToListAsync();


            return mapper.Map<List<AutorDTO>>(autores);
        }
        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO AutorCreacionDTO)
        {
            //validaciones por controlador 
            var existeAutorConNombre = await context.Autores.AnyAsync(x => x.Nombre == AutorCreacionDTO.Nombre);
            if (!existeAutorConNombre)
            {
                var autor = mapper.Map<Autor>(AutorCreacionDTO);
                context.Add(autor);
                await context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest($"Existe Autor con nombre {AutorCreacionDTO.Nombre}");
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
