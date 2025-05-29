using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.DTO;
using gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL;
using gestion_tarjetas_umg.Models.Estructuras.TablasHash;

namespace gestion_tarjetas_umg.Services
{
    public class MemoriaService
    {
        public TablaHash<string, Usuario> tHashUsuarios = new TablaHash<string, Usuario>(10);
        public ArbolAvl<Cliente> arbolClientes = new ArbolAvl<Cliente>();

        public Tarjeta? BuscarTarjetaEnSistema(CobroPagoDTO cobro)
        {
            foreach (Cliente cliente in arbolClientes.ToList())
            {
                var tarjeta = cliente.Tarjetas.Buscar(new Tarjeta
                {
                    numeroTarjeta = cobro.numTarjeta,
                    cvv = cobro.cvv,
                    mesExp = cobro.mesExp,
                    anioExp = cobro.anioExp,
                    nombreTarjeta = "",
                    red = "",
                    pin = 0,
                })?.valor;

                if (tarjeta != null)
                    return tarjeta;
            }

            return null;
        }
    }
}
