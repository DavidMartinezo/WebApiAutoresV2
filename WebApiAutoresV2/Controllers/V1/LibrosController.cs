using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutoresV2.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2.Controllers.V1
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        //constructor de la clase
        public LibrosController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        //[HttpGet("{id:int}")]   
        //public async Task<ActionResult<Libro>> GetLibros(int id)
        //{
        //    return await context.Libros.Include(x=> x.Autor).FirstOrDefaultAsync(x => x.id == id);

        //}



        #region Gets

        [HttpGet(Name ="ObtenerLibros")]
        public async Task<ActionResult<List<LibroDTO>>> GetAllLibros()
        {
            // var libros = await context.Libros.ToListAsync();
            return mapper.Map<List<LibroDTO>>(await context.Libros.ToListAsync());

        }
        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroConAutorDTO>> Get(int id)
        {
            var libro = await context.Libros
           .Include(libroDb => libroDb.AutoresLibros).ThenInclude(autorLibroDb => autorLibroDb.Autor)
           /*.Include(libroDb => libroDb.Comentarios)*/
           .FirstOrDefaultAsync(libroDb => libroDb.id == id);
            if(libro is null)
            {
                return NotFound();
            }
            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.orden).ToList();
            return mapper.Map<LibroConAutorDTO>(libro);
        }
        #endregion

        #region Post
        [HttpPost(Name ="CrearLibro")]
        public async Task<ActionResult<LibroCreacionDTO>> Post(LibroCreacionDTO LibroCreacionDTO)
        {
            if (LibroCreacionDTO.AutoresIds == null) { return BadRequest("No se puede crear un libro sin autores"); }

            var autoresIds = await context.Autores
                .Where(autorDb => LibroCreacionDTO.AutoresIds
                .Contains(autorDb.Id)).Select(x => x.Id).ToListAsync();
            if (LibroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(LibroCreacionDTO);
            AsignarOrdenAutores(libro);


            context.Add(libro);
            await context.SaveChangesAsync();
            var LibroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.id }, LibroDTO);

        }

        #endregion


        #region Puts

        [HttpPut("{id:int}",Name ="ActualizarLibro")]
        public async Task<ActionResult> Put(LibroCreacionDTO libroCreacionDTO, int id)
        {
            var libroDb = await context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.id == id);
            if (libroDb is null)
            {
                return NotFound();
            }

            libroDb = mapper.Map(libroCreacionDTO, libroDb);

            AsignarOrdenAutores(libroDb);
            await context.SaveChangesAsync();
            return NoContent();
        }
        #endregion
        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].orden = i;
                }
            }

        }
        [HttpPatch("{id:int}", Name = "PatchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPathcDTO> patchDocument)
        {
            if (patchDocument is null) { return BadRequest(); }
            var libroDb = await context.Libros.FirstOrDefaultAsync(x => x.id == id);
            if (libroDb is null) { return NotFound(); }

            var libroDTO = mapper.Map<LibroPathcDTO>(libroDb);
            patchDocument.ApplyTo(libroDTO, ModelState);
            var esValido = TryValidateModel(libroDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(libroDTO, libroDb);

            await context.SaveChangesAsync();
            return NoContent();
        }


        #region Delete
        [HttpDelete("{id:int}",Name ="BorrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await context.Libros.AnyAsync(x => x.id == id);

            if (!existe)
            {
                return NotFound("El registro no existe");
            }
            context.Remove(new Libro() { id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
        #endregion
    }
}
