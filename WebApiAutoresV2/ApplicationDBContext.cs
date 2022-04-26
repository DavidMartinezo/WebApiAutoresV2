using Microsoft.EntityFrameworkCore;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext( DbContextOptions options) : base(options)
        {

        }
        //para crear la tabla de autores en la base de datos 
        public DbSet<Autor> Autores { get; set; }
        //para poder hacer consultas a la tabla libros
        public DbSet<Libro> Libros {  get; set; }
    }
}
