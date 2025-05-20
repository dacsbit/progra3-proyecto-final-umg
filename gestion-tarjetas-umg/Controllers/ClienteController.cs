using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.DTO;
using gestion_tarjetas_umg.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestion_tarjetas_umg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly MemoriaService _memoriaService;


        public ClienteController(MemoriaService memoriaService)
        {
            _memoriaService = memoriaService;
        }

        [HttpPost("cargaInicial")]

        public IActionResult CargaInicial([FromBody] List<ClienteDTO> clientesDto)
        {
            if (clientesDto == null || clientesDto.Count == 0) return BadRequest("No se recibio informacion valida");
            

            foreach (var clienteDTO in clientesDto)
            {
                // Crear cliente
                Cliente cliente = new Cliente
                {
                    nombre = clienteDTO.nombre,
                    dpi = clienteDTO.dpi,
                    nit = clienteDTO.nit,
                    telefono = clienteDTO.telefono,
                    direccion = clienteDTO.direccion,
                    email = clienteDTO.email
                };

                // Crear usuario
                Usuario usuario = new Usuario
                {
                    nombreUsuario = clienteDTO.usuario.nombreUsuario,
                    contrasena = clienteDTO.usuario.contrasena,
                    Cliente = cliente  // Establecer referencia al cliente
                };

                // Establecer la referencia bidireccional
                cliente.Usuario = usuario;

                // Procesar tarjetas (igual que antes)
                foreach (var tarjetaDTO in clienteDTO.listaTarjetas)
                {
                    Tarjeta tarjeta = new Tarjeta
                    {
                        numeroTarjeta = tarjetaDTO.numeroTarjeta,
                        cvv = tarjetaDTO.cvv,
                        mesExp = tarjetaDTO.mesExp,
                        anioExp = tarjetaDTO.anioExp,
                        nombreTarjeta = tarjetaDTO.nombreTarjeta,
                        red = tarjetaDTO.red,
                        pin = tarjetaDTO.pin
                    };

                    foreach (var transaccionDTO in tarjetaDTO.transacciones)
                    {
                        Transaccion transaccion = new Transaccion
                        {
                            id = transaccionDTO.id,
                            referencia = transaccionDTO.referencia,
                            montoCredito = transaccionDTO.montoCredito,
                            montoDebito = transaccionDTO.montoDebito,
                            fecha = transaccionDTO.fecha,
                            descripcion = transaccionDTO.descripcion
                        };

                        tarjeta.transacciones.Agregar(transaccion);
                    }

                    cliente.Tarjetas.Add(tarjeta);
                }

                // Guardar en estructuras de datos
                _memoriaService.tHashUsuarios.Insertar(usuario.claveUnica, usuario);
                _memoriaService.arbolClientes.Insertar(cliente);
            }

            return Ok("Datos cargados correctamente");
        }
    }
}
