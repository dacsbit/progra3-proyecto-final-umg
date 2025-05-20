using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.DTO;
using System.Text.Json;

namespace gestion_tarjetas_umg.Services
{
    public class DataLoaderService
    {
        public readonly MemoriaService _memoriaService;

        public DataLoaderService(MemoriaService memoriaService)
        {
            _memoriaService = memoriaService;
        }

        public void CargarDatosIniciales(string jsonFilePath)
        {
            // Leer el archivo JSON
            string jsonString = File.ReadAllText(jsonFilePath);

            // Desserializar a DTOs
            List<ClienteDTO> clientesDTO = JsonSerializer.Deserialize<List<ClienteDTO>>(jsonString);

            foreach (var clienteDTO in clientesDTO)
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
        }
    }
}
