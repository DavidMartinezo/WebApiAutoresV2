using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Test.PruebasUnitarias.Mocks;
using WebApiAutoresV2.Controllers.V1;

namespace WebApiAutores.Test.PruebasUnitarias.RootControllerTest
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task ValidarHATEOASAdmin()
        {
            //deveria devolver cuatro links 
            //preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);//necesitamos un mock
            rootController.Url = new UrlHelperMock();
            //ejecucion
            var resultado = await rootController.Get();
            //verificacion
            Assert.AreEqual(4, resultado.Value?.Count());


        }
        [TestMethod]
        public async Task ValidarHATEOASNoAdmin()
        {
            //deveria devolver 2 links 
            //preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);//necesitamos un mock
            rootController.Url = new UrlHelperMock();
            //ejecucion
            var resultado = await rootController.Get();
            //verificacion
            Assert.AreEqual(2, resultado.Value?.Count());


        }


        [TestMethod]
        public async Task ValidarHATEOASNoAdminUsandoMOQ()
        {
            //deveria devolver 2 links 
            //preparacion
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(Task.FromResult(AuthorizationResult.Failed()));
            //para el otro metodo que usa el policy name 

            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
              It.IsAny<ClaimsPrincipal>(),
              It.IsAny<object>(),
              It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));
            var rootController = new RootController(mockAuthorizationService.Object);//necesitamos un mock
                                                                                     //seteamos el mockde la IurlHelper
            var mockUrlHelper = new Mock<IUrlHelper>();
            // mockUrlHelper.Setup(x => x.Link("ObtenerAutores", new {})).Returns();//puedo colocar lo que quiero que se retorne 
            mockUrlHelper.Setup(x => x.Link(
                 It.IsAny<string>(),
                It.IsAny<object>()
                )).Returns(string.Empty);
            rootController.Url = mockUrlHelper.Object;
            //ejecucion
            var resultado = await rootController.Get();
            //verificacion
            Assert.AreEqual(2, resultado.Value?.Count());


        }
    }
}
