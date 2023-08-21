using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using WebApiAutoresV2.DTOs;

namespace WebApiAutoresV2.Controllers.V1
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }
        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {

            /*  datosHeateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("nombreDeLaRuta", new { }),
                descripcion: "QueHace",
                metodo: "GET,POST,PUT,DELETE")); */

            var datosHeateoas = new List<DatoHATEOAS>();
            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");

            if (isAdmin.Succeeded)
            {
                datosHeateoas.Add(new DatoHATEOAS(
                    enlace: Url.Link("CrearAutor", new { }),
                    descripcion: "CrearAutor",
                    metodo: "POST"));

                datosHeateoas.Add(new DatoHATEOAS(
                    enlace: Url.Link("CrearLibro", new { }),
                    descripcion: "CrearLibro",
                    metodo: "POST"));
            }

            datosHeateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("ObtenerRoot", new { }),
                descripcion: "self",
                metodo: "GET"));

            datosHeateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("ObtenerAutores", new { }),
                descripcion: "Get-ListaAutores",
                metodo: "GET")
                );



            return datosHeateoas;
        }
    }
}
