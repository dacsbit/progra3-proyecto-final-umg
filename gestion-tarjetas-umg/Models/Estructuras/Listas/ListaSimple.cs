namespace gestion_tarjetas_umg.Models.Estructuras.Listas
{
    public class ListaSimple<T>
    {
        public NodoLista<T>? inicio;

        public ListaSimple()
        {
            this.inicio = null;
        }

        public void Agregar(T nuevo)
        {
            inicio = AgregarRecursivo(inicio, nuevo);
        }

        private NodoLista<T> AgregarRecursivo(NodoLista<T>? iterador, T nuevo)
        {
            if (iterador == null) return new NodoLista<T>(nuevo);

            iterador.siguiente = AgregarRecursivo(iterador.siguiente, nuevo);

            return iterador;
        }
    }
}
