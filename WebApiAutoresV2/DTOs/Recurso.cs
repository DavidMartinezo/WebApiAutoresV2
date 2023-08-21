namespace WebApiAutoresV2.DTOs
{
    public class Recurso
    {
        //es una ruta raiz que devolvera lo que el usuario puede hacer 
        public List<DatoHATEOAS> Enlaces { get; set; } = new List<DatoHATEOAS>();//por defecto tendra un listado vacio
    }
}
