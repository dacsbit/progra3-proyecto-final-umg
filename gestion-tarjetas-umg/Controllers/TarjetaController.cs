using System.Runtime.Intrinsics.Arm;
using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.DTO;
using gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL;
using gestion_tarjetas_umg.Models.Estructuras.Listas;
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
        public IActionResult PagarTarjeta([FromBody] CobroPagoDTO PagoDTO)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized("No se encontro informacion del cliente");

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
                    return Ok(new { IsSuccess = true, Msg = "Pago recibido correctamente" });
                }
                else
                {
                    return NotFound(new { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada" });
                }
            }
            else
            {
                return NotFound(new { IsSuccess = false, Msg = "Cliente no encontrado" });
            }
        }

        [HttpPost("Cobrar")]
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
                return Ok(new { IsSuccess = true, Msg = "Cobro realizado correctamente" });
            }
            else
            {
                return NotFound(new { IsSuccess = false, Msg = "Cobro no realizado. Revise los datos de tarjeta" });
            }
        }

        [Authorize]
        [HttpGet("ConsultarMovimientos")]
        public IActionResult ConsultarMovimientos([FromBody] ReporteriaTarjetaDTO reporteriaTarjeta)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized("No se encontro informacion del cliente");

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
                        return BadRequest(new { IsSuccess = false, Msg = "revise el rango de fecha solicitado" });
                    
                    byte[] pdfBytes = tarjetaCliente.valor.GenerarMovimientos(reporteriaTarjeta.fechaInicio, reporteriaTarjeta.fechaFinal);
                    string pdfString = Convert.ToBase64String(pdfBytes);
                    return Ok(new {IsSuccess = true, pdf = pdfString });
                }
                else
                {
                    return NotFound(new { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada" });
                }
            }
            else
            {
                return NotFound(new { IsSuccess = false, Msg = "Cliente no encontrado" });
            }
        }

        [Authorize]
        [HttpGet("RenovarTarjeta")]
        public IActionResult RenovarTarjeta([FromBody] TarjetaConsultaDTO tarjetaConsultaDTO)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized("No se encontro informacion del cliente");

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
                        return Ok(new { IsSuccess = false, Msg = "Solo se puede renovar una tarjeta expirada", Data = "null" });
                    tarjetaCliente.valor.activa = false;

                    Tarjeta tarjetaRenovacion = new Tarjeta
                    {
                        nombreTarjeta = $"{nodoCliente.valor.nombre}",
                    };

                    nodoCliente.valor.Tarjetas.Agregar(tarjetaRenovacion);

                    return Ok(new { IsSuccess = true, Msg = "tarjeta renovada correctamente", Data = tarjetaRenovacion });
                }
                else
                {
                    return NotFound(new { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada", Data = "null" });
                }
            }
            else
            {
                return NotFound(new { IsSuccess = false, Msg = "Cliente no encontrado", Data = "null" });
            }
        }

        [Authorize]
        [HttpPut("CambioPin")]
        public IActionResult CambioPin([FromBody] CambioPinDTO cambioPinDTO)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized("No se encontro informacion del cliente");

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

                    return Ok(new { IsSuccess = true, Msg = "PIN actualizado correctamente" });
                }
                else
                {
                    return NotFound(new { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada" });
                }
            }
            else
            {
                return NotFound(new { IsSuccess = false, Msg = "Cliente no encontrado" });
            }
        }

        [Authorize]
        [HttpPut("BloquearTarjeta")]
        public IActionResult BloquearTarjeta([FromBody] TarjetaConsultaDTO tarjetaConsulta)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized("No se encontro informacion del cliente");

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

                    return Ok(new { IsSuccess = true, Msg = "Bloqueo de tarjeta exitoso" });
                }
                else
                {
                    return NotFound(new { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada" });
                }
            }
            else
            {
                return NotFound(new { IsSuccess = false, Msg = "Cliente no encontrado" });
            }
        }

        [Authorize]
        [HttpPut("DesbloqueoTarjeta")]
        public IActionResult DesbloqueoTarjeta([FromBody] TarjetaConsultaDTO tarjetaConsulta)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized("No se encontro informacion del cliente");

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

                    return Ok(new { IsSuccess = true, Msg = "Bloqueo de tarjeta exitoso" });
                }
                else
                {
                    return NotFound(new { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada" });
                }
            }
            else
            {
                return NotFound(new { IsSuccess = false, Msg = "Cliente no encontrado" });
            }
        }

        [Authorize]
        [HttpPut("AumentarLimiteCredito")]
        public IActionResult AumentarLimiteCredito(AumentarLimiteCreditoDTO tarjetaConsulta)
        {
            var claveUnicaUsuario = User.FindFirst("claveUnica")?.Value;
            var dpiCliente = User.FindFirst("dpi")?.Value;

            if (dpiCliente == null) return Unauthorized("No se encontro informacion del cliente");

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

                    return Ok(new { IsSuccess = true, Msg = "Bloqueo de tarjeta exitoso" });
                }
                else
                {
                    return NotFound(new { IsSuccess = false, Msg = "Tarjeta ingresada no encontrada" });
                }
            }
            else
            {
                return NotFound(new { IsSuccess = false, Msg = "Cliente no encontrado" });
            }
        }
    }
}
