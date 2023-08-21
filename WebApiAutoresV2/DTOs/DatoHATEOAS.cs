namespace WebApiAutoresV2.DTOs
{
    public class DatoHATEOAS
    {
        public string Enlace { get; private set; }
        public string Description { get; private set; }
        public string Metodo { get; private set; }

        public DatoHATEOAS(string enlace, string descripcion, string metodo)
        {
            Enlace = enlace;
            Description = descripcion;
            Metodo = metodo;
        }
    }
}
