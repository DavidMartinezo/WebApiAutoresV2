namespace WebApiAutoresV2.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1; //propeidad por defecto 1
        private int recordsPorPagina = 10;//es un campo por defecto con 10 
        private readonly int cantidadMaximaPorPagina = 50;//debo indicar el limite de registros a retornar si no la paginacion no sirve 

        public int RecordsPorPagina //esta es la propiedad
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                recordsPorPagina = (value > cantidadMaximaPorPagina) ? cantidadMaximaPorPagina : value;
            }
        } 
    }
}
