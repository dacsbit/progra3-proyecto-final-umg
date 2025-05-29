using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.DTO;
using gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL;
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

        public IActionResult CrearCliente([FromBody] List<ClienteDTO> clientesDto)
        {
            if (clientesDto == null || clientesDto.Count == 0) return BadRequest("No se recibio informacion valida");
            

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

            return Ok("Datos cargados correctamente");
        }

        [HttpGet("buscar")]
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
                return Ok(bCliente);
            }
            else
            {
                return Ok("Cliente no encontrado");
            }
        }

        [HttpPut("actualizar")]
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
                    return Ok(new { msg = "Cliente actualizado correctamente", data = "null" });
                }
                else
                {
                    return Ok(new { msg = "Error en actalizacion", data = "null" });
                }
            }
            else
            {
                return BadRequest(new { msg = "El DPI ingresado no pertenece a ningun cliente", data = aCliente });
            }
        }

        [HttpDelete("eliminar")]
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
            if (eliminado) return Ok(new { msg = "Cliente eliminado" });

            return BadRequest(new { msg = "el dpi ingresado no pertenece a ningun cliente" });
        }
    }
}
