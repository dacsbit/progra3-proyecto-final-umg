using Microsoft.AspNetCore.Http.HttpResults;

namespace gestion_tarjetas_umg.Models.Estructuras.Listas
{
    public class Pila<T>
    {
        public NodoLista<T>? cima;

        public Pila()
        {
            this.cima = null;
        }

        public void Push(T nuevo)
        {
            if (cima == null)
            {
                cima = new NodoLista<T>(nuevo);
            }
            else
            {
                NodoLista<T> nuevoNodo = new NodoLista<T>(nuevo);
                nuevoNodo.siguiente = cima;
                cima = nuevoNodo;
            }
        }

        public NodoLista<T>? Pop()
        {
            if (cima == null) return null;
            
            NodoLista<T> nodoRetorno = cima;
            cima = cima.siguiente;
            return nodoRetorno;
        }
    }
}
