using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext( DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //utilizando el api fluente de entity framework 
            base.OnModelCreating(modelBuilder);
            //con esto se crea la llave compuesta para la entidad AutorLibro usando la llave autor y libro. 
            modelBuilder.Entity<AutorLibro>()
                .HasKey(al => new { al.AutorId, al.LibroId });
        }
        //para crear la tabla de autores en la base de datos 
        public DbSet<Autor> Autores { get; set; }
        //para poder hacer consultas a la tabla libros
        public DbSet<Libro> Libros {  get; set; }

        //para controlar los comentarios de los libros 
        public DbSet<Comentario> Comentarios { get; set; }

        //para romper la relacion de muchos a muchos entre libro y autor se crea esta tabla intermedia 
        public DbSet<AutorLibro> AutoresLibros { get; set; }
    }
}
