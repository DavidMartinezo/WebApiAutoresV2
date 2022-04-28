using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutoresV2.Validaciones;

namespace WebApiAutoresV2.Entidades
{
    public class Autor 
    {
        public int Id { get; set; }
        [PrimeraLetraMayusculaAtribute]
        public string Nombre {  get; set; }
        public List<Libro> libros {  get; set; }  
        [NotMapped]
        [CreditCard]
        public string Tarjeta {  get; set;}

     
    }
}
