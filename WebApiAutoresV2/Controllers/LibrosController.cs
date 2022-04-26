using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController :ControllerBase
    {
        private readonly ApplicationDBContext context;
        //constructor de la clase
        public LibrosController(ApplicationDBContext context)
        {
            this.context = context;
        }
        [HttpGet("{id:int}")]   
        public async Task<ActionResult<Libro>> GetLibros(int id)
        {
            return await context.Libros.Include(x=> x.Autor).FirstOrDefaultAsync(x => x.id == id);

        }

        [HttpGet]
        public async Task<ActionResult<List<Libro>>> GetAllLibros()
        {
            return await context.Libros.ToListAsync();

        }
        [HttpPost()]
        public async Task<ActionResult<Libro>> Post(Libro libro)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if (!existeAutor)
            {
                return BadRequest($"No existe el autor de id: {libro.AutorId}");
            }
            context.Add(libro);
            await context.SaveChangesAsync();  
            return Ok();   

        }
    }
}
