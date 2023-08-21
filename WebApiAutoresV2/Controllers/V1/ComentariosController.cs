using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutoresV2.DTOs;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2.Controllers.V1
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDBContext context, 
            IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }


        #region Gets
        [HttpGet(Name ="ObtenerComentariosPorLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existe = await context.Libros.AnyAsync(libro => libro.id == libroId);
            if (!existe)
            {
                return BadRequest($"No existe el libro de id: {libroId}");
            }
            var comentarios = await context.Comentarios
                .Where(comentarioDb => comentarioDb.LibroId == libroId).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetComentarioPorID(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDb => comentarioDb.Id == id);
            if (comentario == null) { return NotFound(); }
            return mapper.Map<ComentarioDTO>(comentario);
        }
        #endregion

        #region Posts
        [HttpPost(Name ="CrearComentario")]
        [AllowAnonymous]//endpoint q puede ser usado por usuarios logeados y obtener sus claims y por no logeados tambien 
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO ComentarioCreacionDTO)
        {
            //este valor vendra nulo por que el claim email por un mapeo automatico tiene algo extra se debe limpiar
            //en el startup JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //video 83
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario= await userManager.FindByEmailAsync(email); //devuelve un identity user
            var usuarioId = usuario.Id;
            var existe = await context.Libros.AnyAsync(libro => libro.id == libroId);
            if (!existe)
            {
                return BadRequest($"No existe el libro de id: {libroId}");
            }
            var comentario = mapper.Map<Comentario>(ComentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);

        }
        #endregion
        #region Put
        [HttpPut("{id:int}",Name ="ActualizarComentario")]
        public async Task<ActionResult> UpdateComentario(ComentarioCreacionDTO comentarioCreacionDTO, int libroId, int id)
        {
            var existe = await context.Libros.AnyAsync(libro => libro.id == libroId);
            if (!existe)
            {
                return NotFound($"No existe el libro de id: {libroId}");
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDb => comentarioDb.Id == id);
            if(!existeComentario)
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();
        }

        #endregion

    }
}
