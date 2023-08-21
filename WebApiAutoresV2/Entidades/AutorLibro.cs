﻿namespace WebApiAutoresV2.Entidades
{
    public class AutorLibro
    {
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public int orden { get; set; }
        //propiedades de navegacion 
        public Libro Libro { get; set; }
        public Autor Autor { get; set; }

    }
}
