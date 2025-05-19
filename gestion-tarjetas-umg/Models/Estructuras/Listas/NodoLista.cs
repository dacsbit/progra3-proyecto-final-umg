namespace gestion_tarjetas_umg.Models.Estructuras.Listas
{
    public class NodoLista<T>
    {
        public NodoLista<T>? siguiente;
        public T valor;

        public NodoLista(T valor)
        { 
            this.valor = valor;
            this.siguiente = null;
        }

    }
}
