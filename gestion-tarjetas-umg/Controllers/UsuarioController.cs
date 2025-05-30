using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.DTO;
using gestion_tarjetas_umg.Models.Responses;
using gestion_tarjetas_umg.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestion_tarjetas_umg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly MemoriaService _memoriaService;
        private readonly SeguridadService _seguridadService;

        public UsuarioController(MemoriaService memoriaService, SeguridadService seguridadService)
        {
            _memoriaService = memoriaService;
            _seguridadService = seguridadService;
        }

        [HttpGet("listado")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        public IActionResult listadoUsuarios()
        {
            List<Usuario> listaUsuarios = _memoriaService.tHashUsuarios.ToList();
            Usuario dummy = new Usuario
            {
                nombreUsuario = "dummy",
                contrasena = "dummy",
                Cliente = null,
            };
            
            byte[] pdfBytes = dummy.GenerarPdfDesdeLista(listaUsuarios);

            string pdfString =  Convert.ToBase64String(pdfBytes);

            return Ok(new Respuesta<string>{ IsSuccess = true, Data = pdfString });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 400)]
        public IActionResult login([FromBody] LoginDTO login)
        {
            (Usuario? lUsuario, bool encontrado) = _memoriaService.tHashUsuarios.Obtener(login.username);

            if (encontrado && lUsuario != null)
            {
                if (lUsuario.contrasena == login.password)
                {
                    return Ok(new Respuesta<string>{ IsSuccess = true, Data = _seguridadService.generarJwt(lUsuario), Msg="Inicio de sesion correcto" });
                }
                else
                {
                    return BadRequest(new Respuesta<string> { IsSuccess = false, Data = "", Msg = "Nombre de usuario o contraseña incorrecta" });
                }
            }
            else
            {
                return BadRequest(new Respuesta<string> { IsSuccess = false, Data = "", Msg = "Nombre de usuario o contraseña incorrecta" });
            }
        }

    }
}
