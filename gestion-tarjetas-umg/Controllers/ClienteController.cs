using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.DTO;
using gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL;
using gestion_tarjetas_umg.Models.Responses;
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

        [HttpPost("crear")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 400)]
        public IActionResult CrearCliente([FromBody] List<ClienteDTO> clientesDto)
        {
            if (clientesDto == null || clientesDto.Count == 0) return BadRequest(new Respuesta<string> { IsSuccess = false, Msg = "No se recibio informacion valida", Data = "null" });
            

            foreach (var clienteDTO in clientesDto)
            {
                // Crear cliente
                Cliente cliente = new Cliente
                {
                    nombre = clienteDTO.nombre!,
                    dpi = clienteDTO.dpi,
                    nit = clienteDTO.nit!,
                    telefono = clienteDTO.telefono!,
                    direccion = clienteDTO.direccion!,
                    email = clienteDTO.email!
                };

                // Crear usuario
                Usuario usuario = new Usuario
                {
                    nombreUsuario = clienteDTO.usuario!.nombreUsuario,
                    contrasena = clienteDTO.usuario.contrasena,
                    Cliente = cliente  // Establecer referencia al cliente
                };

                // Establecer la referencia bidireccional
                cliente.Usuario = usuario;

                if(clienteDTO.listaTarjetas != null && clienteDTO.listaTarjetas.Count > 0)
                {
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

                            tarjeta.transacciones.Insertar(transaccion);
                        }

                        cliente.Tarjetas.Agregar(tarjeta);
                    }
                }                

                // Guardar en estructuras de datos
                _memoriaService.tHashUsuarios.Insertar(usuario.nombreUsuario, usuario);
                _memoriaService.arbolClientes.Insertar(cliente);
            }

            return Ok(new Respuesta<string> { IsSuccess = true, Msg = "Datos cargados correctamente", Data = "null" });
        }


        [HttpGet("buscar")]
        [ProducesResponseType(typeof(Respuesta<Cliente>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult BuscarCliente(long dpi)
        {
            Cliente bCliente = new Cliente
            {
                nombre = "",
                dpi = dpi,
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null
            };

            (NodoAvl<Cliente>? nEncontrado, bool encontrado) = _memoriaService.arbolClientes.Buscar(bCliente);

            if (encontrado)
            {
                bCliente = new Cliente
                {
                    nombre = nEncontrado!.valor.nombre,
                    dpi = nEncontrado!.valor.dpi,
                    nit = nEncontrado!.valor.nit,
                    telefono = nEncontrado!.valor.telefono,
                    direccion = nEncontrado!.valor.direccion,
                    email = nEncontrado!.valor.email,
                    Tarjetas = nEncontrado!.valor.Tarjetas,
                    Usuario = new Usuario
                    {
                        claveUnica = nEncontrado!.valor.Usuario!.claveUnica,
                        nombreUsuario = nEncontrado!.valor.Usuario!.nombreUsuario,
                        contrasena = nEncontrado!.valor.Usuario!.contrasena,
                        Cliente = null
                    }
                };
                return Ok(new Respuesta<Cliente> { IsSuccess = true, Data = bCliente });
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [HttpPut("actualizar")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 400)]
        [ProducesResponseType(typeof(Respuesta<ClienteSimpleDTO>), 409)]
        public IActionResult ActualizarCliente(long dpi, [FromBody] ClienteSimpleDTO aCliente)
        {
            Cliente bCliente = new Cliente
            {
                nombre = "",
                dpi = dpi,
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null
            };

            (NodoAvl<Cliente>? eCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(bCliente);

            if (encontrado)
            {
                Cliente nCliente = new Cliente
                {
                    nombre = aCliente.nombre!,
                    dpi = aCliente.dpi,
                    nit = aCliente.nit!,
                    telefono = aCliente.telefono!,
                    direccion = aCliente.direccion!,
                    email = aCliente.email!,
                    Tarjetas = eCliente!.valor.Tarjetas,
                    Usuario = eCliente.valor.Usuario,
                };

                bool actualizado = _memoriaService.arbolClientes.Modificar(eCliente.valor, nCliente);
                if (actualizado)
                {
                    return Ok(new Respuesta<string>{IsSuccess = true, Msg = "Cliente actualizado correctamente", Data = "null" });
                }
                else
                {
                    return Conflict(new Respuesta<string> { IsSuccess = false, Msg = "Error en actualizacion", Data = "null" });
                }
            }
            else
            {
                return BadRequest(new Respuesta<ClienteSimpleDTO> { IsSuccess = false, Msg = "El DPI ingresado no pertenece a ningun cliente", Data = aCliente });
            }
        }

        [HttpDelete("eliminar")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 400)]
        public IActionResult EliminarCliente(long dpi)
        {
            Cliente elCliente = new Cliente
            {
                nombre = "",
                dpi = dpi,
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null
            };

            bool eliminado = _memoriaService.arbolClientes.Eliminar(elCliente);
            if (eliminado) return Ok(new Respuesta<string> {IsSuccess = true, Msg = "Cliente eliminado", Data = "null" });

            return BadRequest(new Respuesta<string> {IsSuccess = false, Msg = "el DPI ingresado no pertenece a ningun cliente", Data = "null" });
        }

        [HttpGet("ReporteTarjetas")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult ReporteTarjetas(long dpi)
        {
            (NodoAvl<Cliente>? nEncontrado, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = dpi,
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null
            });

            if (encontrado)
            {
                byte[] pdfBytes = nEncontrado!.valor.ListadoTarjetas();
                string pdfString = Convert.ToBase64String(pdfBytes);
                return Ok(new Respuesta<string> { IsSuccess = true, Data = pdfString });
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }
    }
}
