using gestion_tarjetas_umg.Models.Domain;
using gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL;
using gestion_tarjetas_umg.Models.Estructuras.TablasHash;

namespace gestion_tarjetas_umg.Services
{
    public class MemoriaService
    {
        public TablaHash<string, Usuario> tHashUsuarios = new TablaHash<string, Usuario>(10);
        public ArbolAvl<Cliente> arbolClientes = new ArbolAvl<Cliente>();
    }
}
