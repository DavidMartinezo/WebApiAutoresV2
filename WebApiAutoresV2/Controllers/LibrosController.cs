using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutoresV2.DTOs;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController :ControllerBase
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
        [HttpPost()]
        public async Task<ActionResult<LibroCreacionDTO>> Post(LibroCreacionDTO LibroCreacionDTO)
        {
            //var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            //if (!existeAutor)
            //{
            //    return BadRequest($"No existe el autor de id: {libro.AutorId}");
            //}
            var libro = mapper.Map<Libro>(LibroCreacionDTO);
            context.Add(libro);
            await context.SaveChangesAsync();  
            return Ok();   

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTO>> Get(int id)
        {
            var libro = await context.Libros
                .Include(libroDb => libroDb.Comentarios)
                .FirstOrDefaultAsync(libroDb => libroDb.id == id);
            return mapper.Map<LibroDTO>(libro); 
        }
    }
}
