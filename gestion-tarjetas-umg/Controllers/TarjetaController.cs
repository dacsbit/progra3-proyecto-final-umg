using System.Runtime.Intrinsics.Arm;
using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.DTO;
using gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL;
using gestion_tarjetas_umg.Models.Estructuras.Listas;
using gestion_tarjetas_umg.Models.Responses;
using gestion_tarjetas_umg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestion_tarjetas_umg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class TarjetaController : ControllerBase
    {
        private readonly MemoriaService _memoriaService;

        public TarjetaController(MemoriaService memoriaService)
        {
            _memoriaService = memoriaService;
        }

        [Authorize]
        [HttpPost("Pagar")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 401)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult PagarTarjeta([FromBody] CobroPagoDTO PagoDTO)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized(new Respuesta<string> { IsSuccess = false, Msg = "No se encontro informacion del cliente", Data = "null" });

            (NodoAvl<Cliente>? nodoCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = long.Parse(dpiCliente),
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null

            });

            if (nodoCliente != null)
            {
                NodoLista<Tarjeta>? tarjetaCliente = nodoCliente.valor.Tarjetas.Buscar(new Tarjeta
                {
                    numeroTarjeta = PagoDTO.numTarjeta,
                    cvv = PagoDTO.cvv,
                    mesExp = PagoDTO.mesExp,
                    anioExp = PagoDTO.anioExp,
                    nombreTarjeta = "",
                    red = "",
                    pin = 0,
                });

                if (tarjetaCliente != null)
                {
                    Transaccion transaccionPago = new Transaccion
                    {
                        id = Guid.NewGuid().ToString(),
                        referencia = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                        montoCredito = PagoDTO.monto,
                        montoDebito = 0.00,
                        fecha = DateTime.UtcNow,
                        descripcion = "Pago de tarjeta"
                    };

                    tarjetaCliente.valor.transacciones.Insertar(transaccionPago);
                    return Ok(new Respuesta<string> { IsSuccess = true, Msg = "Pago recibido correctamente", Data = "null" });
                }
                else
                {
                    return NotFound(new Respuesta<string>{ IsSuccess = false, Msg = "Tarjeta ingresada no encontrada", Data = "null" });
                }
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [HttpPost("Cobrar")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult CobrarTarjeta([FromBody] CobroPagoDTO cobroDTO)
        {
            Tarjeta? tarjeta = _memoriaService.BuscarTarjetaEnSistema(cobroDTO);
            if (tarjeta != null)
            {
                Transaccion transaccionCobro = new Transaccion
                {
                    id = Guid.NewGuid().ToString(),
                    referencia = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                    montoCredito = 0.00,
                    montoDebito = cobroDTO.monto,
                    fecha = DateTime.UtcNow,
                    descripcion = "Pago de tarjeta"
                };

                tarjeta.transacciones.Insertar(transaccionCobro);
                return Ok(new Respuesta<string> { IsSuccess = true, Msg = "Cobro realizado correctamente", Data = "null" });
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cobro no realizado. Revise los datos de tarjeta", Data = "null" });
            }
        }

        [Authorize]
        [HttpPost("ConsultarMovimientos")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 400)]
        [ProducesResponseType(typeof(Respuesta<string>), 401)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult ConsultarMovimientos([FromBody] ReporteriaTarjetaDTO reporteriaTarjeta)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized(new Respuesta<string> { IsSuccess = false, Msg = "No se encontro informacion del cliente", Data = "null" });

            (NodoAvl<Cliente>? nodoCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = long.Parse(dpiCliente),
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null

            });

            if(nodoCliente != null)
            {
                NodoLista<Tarjeta>? tarjetaCliente = nodoCliente.valor.Tarjetas.Buscar(new Tarjeta
                {
                    numeroTarjeta = reporteriaTarjeta.numTarjeta,
                    cvv = reporteriaTarjeta.cvv,
                    mesExp = reporteriaTarjeta.mesExp,
                    anioExp = reporteriaTarjeta.anioExp,
                    nombreTarjeta = "",
                    red = "",
                    pin = 0,
                });

                if (tarjetaCliente != null)
                {
                    if (reporteriaTarjeta.fechaInicio > reporteriaTarjeta.fechaFinal) 
                        return BadRequest(new Respuesta<string> { IsSuccess = false, Msg = "revise el rango de fecha solicitado", Data = "null" });
                    
                    byte[] pdfBytes = tarjetaCliente.valor.GenerarMovimientos(reporteriaTarjeta.fechaInicio, reporteriaTarjeta.fechaFinal);
                    string pdfString = Convert.ToBase64String(pdfBytes);
                    return Ok(new Respuesta<string> {IsSuccess = true, Data = pdfString });
                }
                else
                {
                    return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada", Data = "null" });
                }
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [Authorize]
        [HttpPost("RenovarTarjeta")]
        [ProducesResponseType(typeof(Respuesta<Tarjeta>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 400)]
        [ProducesResponseType(typeof(Respuesta<string>), 401)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult RenovarTarjeta([FromBody] TarjetaConsultaDTO tarjetaConsultaDTO)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized(new Respuesta<string>{ IsSuccess = false, Msg = "No se encontro informacion del cliente", Data = "null" });

            (NodoAvl<Cliente>? nodoCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = long.Parse(dpiCliente),
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null

            });

            if (nodoCliente != null)
            {
                NodoLista<Tarjeta>? tarjetaCliente = nodoCliente.valor.Tarjetas.Buscar(new Tarjeta
                {
                    numeroTarjeta = tarjetaConsultaDTO.numTarjeta,
                    cvv = tarjetaConsultaDTO.cvv,
                    mesExp = tarjetaConsultaDTO.mesExp,
                    anioExp = tarjetaConsultaDTO.anioExp,
                    nombreTarjeta = "",
                    red = "",
                    pin = 0,
                });

                if (tarjetaCliente != null)
                {
                    int diaFinalMes = DateTime.DaysInMonth(tarjetaCliente.valor.anioExp, tarjetaCliente.valor.mesExp);
                    DateTime fechaExp = new(tarjetaCliente.valor.anioExp, tarjetaCliente.valor.mesExp, diaFinalMes);
                    if (DateTime.UtcNow.Date > fechaExp) 
                        return BadRequest(new Respuesta<string>{ IsSuccess = false, Msg = "Solo se puede renovar una tarjeta expirada", Data = "null" });
                    tarjetaCliente.valor.activa = false;

                    Tarjeta tarjetaRenovacion = new Tarjeta
                    {
                        nombreTarjeta = $"{nodoCliente.valor.nombre}",
                    };

                    nodoCliente.valor.Tarjetas.Agregar(tarjetaRenovacion);

                    return Ok(new Respuesta<Tarjeta>{ IsSuccess = true, Msg = "tarjeta renovada correctamente", Data = tarjetaRenovacion });
                }
                else
                {
                    return NotFound(new Respuesta<string>{ IsSuccess = false, Msg = "Tarjeta ingresada no encontrada", Data = "null" });
                }
            }
            else
            {
                return NotFound(new Respuesta<string>{ IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [Authorize]
        [HttpPut("CambioPin")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 401)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult CambioPin([FromBody] CambioPinDTO cambioPinDTO)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized(new Respuesta<string> { IsSuccess = false, Msg = "No se encontro informacion del cliente", Data = "null" });

            (NodoAvl<Cliente>? nodoCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = long.Parse(dpiCliente),
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null

            });

            if (nodoCliente != null)
            {
                NodoLista<Tarjeta>? tarjetaCliente = nodoCliente.valor.Tarjetas.Buscar(new Tarjeta
                {
                    numeroTarjeta = cambioPinDTO.numTarjeta,
                    cvv = cambioPinDTO.cvv,
                    mesExp = cambioPinDTO.mesExp,
                    anioExp = cambioPinDTO.anioExp,
                    nombreTarjeta = "",
                    red = "",
                    pin = 0,
                });

                if (tarjetaCliente != null)
                {
                    tarjetaCliente.valor.pin = cambioPinDTO.nuevoPin;

                    return Ok(new Respuesta<string> { IsSuccess = true, Msg = "PIN actualizado correctamente", Data = "null" });
                }
                else
                {
                    return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada", Data = "null" });
                }
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [Authorize]
        [HttpPut("BloquearTarjeta")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 401)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult BloquearTarjeta([FromBody] TarjetaConsultaDTO tarjetaConsulta)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized(new Respuesta<string> { IsSuccess = false, Msg = "No se encontro informacion del cliente", Data = "null" });

            (NodoAvl<Cliente>? nodoCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = long.Parse(dpiCliente),
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null

            });

            if (nodoCliente != null)
            {
                NodoLista<Tarjeta>? tarjetaCliente = nodoCliente.valor.Tarjetas.Buscar(new Tarjeta
                {
                    numeroTarjeta = tarjetaConsulta.numTarjeta,
                    cvv = tarjetaConsulta.cvv,
                    mesExp = tarjetaConsulta.mesExp,
                    anioExp = tarjetaConsulta.anioExp,
                    nombreTarjeta = "",
                    red = "",
                    pin = 0,
                });

                if (tarjetaCliente != null)
                {
                    tarjetaCliente.valor.bloqueada = true;

                    return Ok(new Respuesta<string> { IsSuccess = true, Msg = "Bloqueo de tarjeta exitoso", Data = "null" });
                }
                else
                {
                    return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada", Data = "null" });
                }
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [Authorize]
        [HttpPut("DesbloqueoTarjeta")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 401)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult DesbloqueoTarjeta([FromBody] TarjetaConsultaDTO tarjetaConsulta)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized(new Respuesta<string> { IsSuccess = false, Msg = "No se encontro informacion del cliente", Data = "null" });

            (NodoAvl<Cliente>? nodoCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = long.Parse(dpiCliente),
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null

            });

            if (nodoCliente != null)
            {
                NodoLista<Tarjeta>? tarjetaCliente = nodoCliente.valor.Tarjetas.Buscar(new Tarjeta
                {
                    numeroTarjeta = tarjetaConsulta.numTarjeta,
                    cvv = tarjetaConsulta.cvv,
                    mesExp = tarjetaConsulta.mesExp,
                    anioExp = tarjetaConsulta.anioExp,
                    nombreTarjeta = "",
                    red = "",
                    pin = 0,
                });

                if (tarjetaCliente != null)
                {
                    tarjetaCliente.valor.bloqueada = false;

                    return Ok(new Respuesta<string> { IsSuccess = true, Msg = "Bloqueo de tarjeta exitoso", Data = "null" });
                }
                else
                {
                    return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada", Data = "null" });
                }
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [Authorize]
        [HttpPut("AumentarLimiteCredito")]
        [ProducesResponseType(typeof(Respuesta<string>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 401)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult AumentarLimiteCredito(AumentarLimiteCreditoDTO tarjetaConsulta)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized(new Respuesta<string> { IsSuccess = false, Msg = "No se encontro informacion del cliente", Data = "null" });

            (NodoAvl<Cliente>? nodoCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = long.Parse(dpiCliente),
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null

            });

            if (nodoCliente != null)
            {
                NodoLista<Tarjeta>? tarjetaCliente = nodoCliente.valor.Tarjetas.Buscar(new Tarjeta
                {
                    numeroTarjeta = tarjetaConsulta.numTarjeta,
                    cvv = tarjetaConsulta.cvv,
                    mesExp = tarjetaConsulta.mesExp,
                    anioExp = tarjetaConsulta.anioExp,
                    nombreTarjeta = "",
                    red = "",
                    pin = 0,
                });

                if (tarjetaCliente != null)
                {
                    tarjetaCliente.valor.limiteCredito = tarjetaConsulta.MontoAumento;

                    return Ok(new Respuesta<string> { IsSuccess = true, Msg = "Bloqueo de tarjeta exitoso", Data = "null" });
                }
                else
                {
                    return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada", Data = "null" });
                }
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [Authorize]
        [HttpGet("SolicitarTarjeta")]
        [ProducesResponseType(typeof(Respuesta<Tarjeta>), 200)]
        [ProducesResponseType(typeof(Respuesta<string>), 400)]
        [ProducesResponseType(typeof(Respuesta<string>), 404)]
        public IActionResult SolicitarTarjeta()
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized(new Respuesta<string> { IsSuccess = false, Msg = "No se encontro informacion del cliente", Data = "null" });

            (NodoAvl<Cliente>? nodoCliente, bool encontrado) = _memoriaService.arbolClientes.Buscar(new Cliente
            {
                nombre = "",
                dpi = long.Parse(dpiCliente),
                nit = "",
                telefono = "",
                direccion = "",
                email = "",
                Usuario = null

            });

            if (nodoCliente != null)
            {
                Tarjeta tarjetaNueva = new Tarjeta
                {
                    nombreTarjeta = $"{nodoCliente.valor.nombre}",
                };

                nodoCliente.valor.Tarjetas.Agregar(tarjetaNueva);

                return Ok(new Respuesta<Tarjeta> { IsSuccess = true, Msg = "Nueva tarjeta registrada. Recuerde que debe desbloquearla para usarla", Data = tarjetaNueva });
            }
            else
            {
                return NotFound(new Respuesta<string> { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }
    }
}
