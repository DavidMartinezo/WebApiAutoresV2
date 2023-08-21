
namespace WebApiAutoresV2.DTOs
{
    public class AutorDTO : Recurso
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        public DateTime FechaPublicacion { get; set; }

    }
}
