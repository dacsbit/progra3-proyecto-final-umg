namespace gestion_tarjetas_umg.Models.Estructuras.TablasHash
{
    public class NodoHash<K, V>
    {
        public K clave;
        public V valor;
        public NodoHash<K, V>? siguiente;

        public NodoHash(K clave, V valor)
        {
            this.clave = clave;
            this.valor = valor;
            this.siguiente = null;
        }
    }
}
