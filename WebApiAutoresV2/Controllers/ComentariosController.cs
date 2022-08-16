using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutoresV2.DTOs;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO ComentarioCreacionDTO)
        {
            var existe = await context.Libros.AnyAsync(libro => libro.id == libroId);
            if (!existe)
            {
                return BadRequest($"No existe el libro de id: {libroId}");
            }
            var comentario = mapper.Map<Comentario>(ComentarioCreacionDTO);
            comentario.LibroId= libroId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existe = await context.Libros.AnyAsync(libro => libro.id == libroId);
            if (!existe)
            {
                return BadRequest($"No existe el libro de id: {libroId}");
            }
            var comentarios = await context.Comentarios
                .Where(libroDb => libroDb.Id == libroId).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }
    }
}
