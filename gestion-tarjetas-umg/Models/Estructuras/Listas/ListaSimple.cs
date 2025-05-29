using gestion_tarjetas_umg.Models.Interfaces;

namespace gestion_tarjetas_umg.Models.Estructuras.Listas
{
    public class ListaSimple<T> where T : Comparador<T>
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

        public NodoLista<T>? Buscar(T valor)
        {
            return BuscarRecursivo(inicio, valor);
        }

        private NodoLista<T>? BuscarRecursivo(NodoLista<T>? indice, T valor)
        {
            if (indice == null) return null;

            if (valor.IgualQue(indice.valor)) return indice;

            return BuscarRecursivo(indice.siguiente, valor);
        }

        public List<T> ToList()
        {
            NodoLista<T>? iterador = inicio;
            List<T> lista = new List<T>();

            while (iterador != null)
            {
                lista.Add(iterador.valor);
                iterador = iterador.siguiente;
            }

            return lista;
        }
    }
}
