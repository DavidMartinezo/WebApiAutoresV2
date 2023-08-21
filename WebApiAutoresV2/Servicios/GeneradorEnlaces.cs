using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using WebApiAutoresV2.DTOs;

namespace WebApiAutoresV2.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }
        private async Task<bool> EsAdmin()
        {
            var httpContex = httpContextAccessor.HttpContext;
            var resultado = await authorizationService.AuthorizeAsync(httpContex.User, "isAdmin");
            return resultado.Succeeded;
        }
        //metodo para generar la url 
        private IUrlHelper ConstruirUrlHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        #region GenerardorDeEnlacesHEATEOAS

        /// <summary>
        /// la url se contruye usando una factoria 
        /// </summary>
        /// <param name="autorDTO"></param>
        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var isAdmin = await EsAdmin();
            var url = ConstruirUrlHelper();
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: url.Link("obtenerAutor", new { id = autorDTO.Id }),
                descripcion: "Self",
                metodo: "GET"));

            if (isAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                              enlace: url.Link("actualizarAutor", new { id = autorDTO.Id }),
                              descripcion: "Autor-Actualizar",
                              metodo: "PUT"));
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: url.Link("borrarAutor", new { id = autorDTO.Id }),
                    descripcion: "Actor-Borrar",
                    metodo: "DELETE"));
            }

        }
        #endregion
    }
}
