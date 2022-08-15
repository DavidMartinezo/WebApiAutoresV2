namespace WebApiAutoresV2.DTOs
{
    public class LibroDTO
    {
        public int id { get; set; }
        public string Titulo { get; set; }
        public List<ComentarioDTO> Comentarios { get; set; }
    }
}
