using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebApiAutoresV2.DTOs
{
    public class ResultadoHash
    {
        public string Hash { get; set; }
        public Byte[] Sal { get; set; }
    }
}
