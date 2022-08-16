using AutoMapper;
using WebApiAutoresV2.DTOs;
using WebApiAutoresV2.Entidades;

namespace WebApiAutoresV2.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorConLibroDTO>()
                .ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibro));
            CreateMap<LibroCreacionDTO, Libro>()
                //se crea una regla especifica para el atributo de autores libros de la entidad libro;
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroConAutorDTO>()
                .ForMember(LibroDTO => LibroDTO.Autores, opciones => opciones.MapFrom(MapLibroDtoAutores));
            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
        }


        private List<LibroDTO> MapAutorDTOLibro(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();
            if (autor.AutoresLibros == null) { return resultado; }
            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }
            return resultado;
        }
        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO LibroCreacionDTO, Libro Libro)
        {
            var resultado = new List<AutorLibro>();
            if (LibroCreacionDTO.AutoresIds == null) { return resultado; }
            foreach (var autorId in LibroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }
            return resultado;

        }

        private List<AutorDTO> MapLibroDtoAutores(Libro Libro, LibroDTO LibroDTO)
        {
            var resultado = new List<AutorDTO>();

            if (Libro.AutoresLibros == null) { return resultado; }
            foreach (var autorLibro in Libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }
            return resultado;
        }
    }
}