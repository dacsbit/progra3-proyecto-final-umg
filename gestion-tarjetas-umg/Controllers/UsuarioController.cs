using gestion_tarjetas_umg.Models.Domain;
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

            return StatusCode(StatusCodes.Status200OK, new { IsSuccess = true, pdf = pdfString });
        }

        [HttpGet("login")]
        public IActionResult login(string username, string password)
        {
            (Usuario? lUsuario, bool encontrado) = _memoriaService.tHashUsuarios.Obtener(username);

            if (encontrado && lUsuario != null)
            {
                if (lUsuario.contrasena == password)
                {
                    return Ok(new { IsSuccess = true, token = _seguridadService.generarJwt(lUsuario) });
                }
                else
                {
                    return BadRequest(new { IsSuccess = false, token = "", msg = "Nombre de usuario o contraseña incorrecta" });
                }
            }
            else
            {
                return BadRequest(new { IsSuccess = false, token = "", msg = "Nombre de usuario o contraseña incorrecta" });
            }
        }

    }
}
