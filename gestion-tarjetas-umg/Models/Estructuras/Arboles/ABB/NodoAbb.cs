namespace gestion_tarjetas_umg.Models.Estructuras.Arboles.ABB
{
    public class NodoAbb<T>
    {
        public NodoAbb<T>? izq;
        public NodoAbb<T>? der;
        public T valor;

        public NodoAbb(T valor)
        {
            this.valor = valor;
            this.izq = null;
            this.der = null;
        }
    }
}
