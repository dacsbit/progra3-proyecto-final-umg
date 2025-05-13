namespace gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL
{
    public class NodoAvl<T>
    {
        public T valor;
        public NodoAvl<T>? izq;
        public NodoAvl<T>? der;
        public int altura;

        public NodoAvl(T valor)
        {
            this.valor = valor;
            this.altura = 1;
        }
    }
}
