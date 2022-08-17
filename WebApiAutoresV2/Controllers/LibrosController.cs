using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutoresV2.DTOs;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2.Controllers
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

        [HttpGet]
        public async Task<ActionResult<List<LibroDTO>>> GetAllLibros()
        {
            // var libros = await context.Libros.ToListAsync();
            return mapper.Map<List<LibroDTO>>(await context.Libros.ToListAsync());

        }
        [HttpGet("{id:int}", Name ="ObtenerLibro")]
        public async Task<ActionResult<LibroConAutorDTO>> Get(int id)
        {
                 var libro = await context.Libros
                .Include(libroDb => libroDb.AutoresLibros).ThenInclude(autorLibroDb => autorLibroDb.Autor)
                /*.Include(libroDb => libroDb.Comentarios)*/
                .FirstOrDefaultAsync(libroDb => libroDb.id == id);
            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.orden).ToList();
            return mapper.Map<LibroConAutorDTO>(libro);
        }

        [HttpPost()]
        public async Task<ActionResult<LibroCreacionDTO>> Post(LibroCreacionDTO LibroCreacionDTO)
        {
            if (LibroCreacionDTO.AutoresIds==null) { return BadRequest("No se puede crear un libro sin autores"); }

            var autoresIds = await context.Autores
                .Where(autorDb => LibroCreacionDTO.AutoresIds
                .Contains(autorDb.Id)).Select(x => x.Id).ToListAsync();
            if (LibroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(LibroCreacionDTO);

            if(libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].orden = i;
                }
            }
            context.Add(libro);
            await context.SaveChangesAsync();
            var LibroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new {id = libro.id}, LibroDTO);

        }

    }
}
