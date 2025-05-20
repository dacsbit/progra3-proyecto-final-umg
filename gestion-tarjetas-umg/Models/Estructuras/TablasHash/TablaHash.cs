namespace gestion_tarjetas_umg.Models.Estructuras.TablasHash
{
    public class TablaHash<K, V>
    {
        private List<ListaHash<K, V>> tabla;
        private int capacidad;

        public TablaHash(int capacidad)
        {
            this.capacidad = capacidad;
            tabla = new List<ListaHash<K, V>>(capacidad);

            //inicializar las listas con listas vacias
            for (int i = 0; i < capacidad; i++)
            {
                tabla.Add(new ListaHash<K, V>());
            }
        }

        private int Hash(K clave)
        {
            return Math.Abs(clave!.GetHashCode()) % capacidad;
        }

        public void Insertar(K clave, V valor)
        {
            int indice = Hash(clave);
            tabla[indice].Insertar(clave, valor);
        }

        public (V?, bool) Obtener(K clave)
        {
            int indice = Hash(clave);
            return tabla[indice].Obtener(clave);
        }

        public bool Eliminar(K clave)
        {
            int indice = Hash(clave);
            return tabla[indice].Eliminar(clave);
        }

        public bool Actualizar(K clave, V nuevo)
        {
            int indice = Hash(clave);
            return tabla[indice].Actualizar(clave, nuevo);
        }

        public List<V> ToList()
        {
            List<V> lista = new List<V>();
            foreach (ListaHash<K, V> listaHash in tabla)
            {
                if(listaHash != null)
                {
                    NodoHash<K, V>? nodoLista = listaHash.inicio;
                    while(nodoLista != null)
                    {
                        lista.Add(nodoLista.valor);
                        nodoLista = nodoLista.siguiente;
                    }
                }
            }
            return lista;
        }
    }
}
