using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApiAutoresV2.DTOs;
using WebApiAutoresV2.Servicios;

namespace WebApiAutoresV2.Controllers.V1
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector =dataProtectionProvider.CreateProtector("unValorUnicoYQuizasSecreto");
        }

        [HttpGet("hash/{textoPlano}")]
        public ActionResult Gethash(string textoPlano)
        {
            //teniendo la salt se puede comparar de esta forma
            //var salt = Convert.FromBase64String("YATfxMBHTjzpgj4PknE7eA==");
            //var resultado1 = hashService.Hash(textoPlano,salt);
            //var resultado2 = hashService.Hash(textoPlano, salt);
          //ejemplo sin la salt, genera valores distintos 
            var resultado1 = hashService.Hash(textoPlano);
            var resultado2 = hashService.Hash(textoPlano);
            return Ok(new
            {
                textoplano=textoPlano,
                hash1 = resultado1,
                hash2 = resultado2
            });
        }

        [HttpGet("encriptar")]
        public ActionResult Encriptar()
        {
            //si se desea limitar el tiempo para descifrar se usaria esta linea y se sustituyen dataProtector por esta nueva var
            var protectorTiempoLimitado = dataProtector.ToTimeLimitedDataProtector();
            var textoPlano = "David Martinez";
            var textoCifrado = dataProtector.Protect(textoPlano);
            //var textoCifrado = protectorTiempoLimitado.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
            var textoDesencriptado = dataProtector.Unprotect(textoCifrado);
            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDesencritado = textoDesencriptado
            });
        }

        [HttpPost("registrar",Name ="RegistrarUsuario")] //api/cuentas/registrar 
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email
            };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }
        [HttpPost("login",Name ="LogearUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Loging(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Password,isPersistent:false,lockoutOnFailure:false);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("RenovarToken",Name ="RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task< ActionResult<RespuestaAutenticacion>> Renovar()
        {
            var emailClaim =  HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = email
            };
            return await ConstruirToken(credencialesUsuario);
        }


        [HttpPost("AddAdmin",Name ="HacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            if (editarAdminDTO == null) { return BadRequest(); }
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            if (usuario == null)
            {
                return BadRequest();
            }
            await userManager.AddClaimAsync(usuario, new Claim("isAdmin", "1"));
            return NoContent();
        }
        [HttpPost("RemovedAdmin",Name ="RemoverAdmin")]
        public async Task<ActionResult> RemovedAdmin(EditarAdminDTO editarAdminDTO)
        {
            if (editarAdminDTO == null) { return BadRequest(); }
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            if (usuario == null)
            {
                return BadRequest();
            }
            await userManager.RemoveClaimAsync(usuario, new Claim("isAdmin", "1"));
            return NoContent();
        }
        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            //es un par de llave y valor estos se anaden al token estos no son secretos 
            //por ende no se colocan datos sencitivos 
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email),
                new Claim("lo que yo quiera", "otro valor"),

            };
            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDb = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDb);

            //construyendo el JWT
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1); //one year for testing purposes 

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            //se retorna la respuesta 
            return new RespuestaAutenticacion()
            {
                //aunque la expiracion va como parte del token es de buen gusto colocarlo explicitamente 
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }
    }
}
