namespace gestion_tarjetas_umg.Models.Estructuras.TablasHash
{
    public class ListaHash<K, V>
    {
        public NodoHash<K, V>? inicio;

        public ListaHash()
        {
            this.inicio = null;
        }

        public void Insertar(K clave, V valor)
        {
            NodoHash<K, V> nuevo = new NodoHash<K, V>(clave, valor);
            nuevo.siguiente = inicio;
            inicio = nuevo;
        }

        public (V?, bool) Obtener(K clave)
        {
            NodoHash<K, V>? actual = inicio;
            if (actual == null) return (default, false);

            while (actual != null)
            {
                if (actual.clave!.Equals(clave)) return (actual.valor, true);
                actual = actual.siguiente;
            }
            return (default, false);
        }

        public bool Eliminar(K clave)
        {
            NodoHash<K, V>? actual = inicio;
            NodoHash<K, V>? anterior = null;

            while (actual != null)
            {
                if (actual.clave!.Equals(clave))
                {
                    if (anterior == null)
                    {
                        inicio = actual.siguiente;
                    }
                    else
                    {
                        anterior.siguiente = actual.siguiente;
                    }
                    return true;
                }
                anterior = actual;
                actual = actual.siguiente;
            }
            return false;
        }

        public bool Actualizar(K clave, V nuevo)
        {
            NodoHash<K, V>? actual = inicio;
            while (actual != null)
            {
                if (actual.clave!.Equals(clave))
                {
                    actual.valor = nuevo;
                    return true;
                }

                actual = actual.siguiente;
            }

            return false;
        }
    }
}
