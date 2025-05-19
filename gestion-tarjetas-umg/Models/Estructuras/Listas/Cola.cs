namespace gestion_tarjetas_umg.Models.Estructuras.Listas
{
    public class Cola<T>
    {
        public NodoLista<T>? frente;
        public NodoLista<T>? final;

        public Cola()
        {
            this.frente = null;
            this.final = null;
        }

        public void Encolar(T nuevo)
        {
            NodoLista<T> nuevoNodo = new NodoLista<T>(nuevo);
            if (frente == null)
            {
                frente = nuevoNodo;
                final = nuevoNodo;
            }
            else
            {
                final!.siguiente = nuevoNodo;
                final = nuevoNodo;
            }
        }

        public NodoLista<T>? Desencolar()
        {
            if (frente == null)
            {
                return null;
            }
            else
            {
                NodoLista<T> nodoRetorno = frente;
                frente = frente.siguiente;

                return nodoRetorno;
            }
        }
    }
}
