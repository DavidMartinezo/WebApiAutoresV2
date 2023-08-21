using System.Net.NetworkInformation;
using WebApiAutoresV2.DTOs;

namespace WebApiAutoresV2.Utilities
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queriable, PaginacionDTO paginacionDTO)
        {
            return queriable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina)
                .Take(paginacionDTO.RecordsPorPagina);
        }

    }
}
