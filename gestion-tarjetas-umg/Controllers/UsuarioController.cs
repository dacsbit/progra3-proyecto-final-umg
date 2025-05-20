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

        public UsuarioController(MemoriaService memoriaService)
        {
            _memoriaService = memoriaService;
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

    }
}
