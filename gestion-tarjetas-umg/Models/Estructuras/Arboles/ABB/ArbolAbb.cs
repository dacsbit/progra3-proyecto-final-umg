using gestion_tarjetas_umg.Models.Interfaces;

namespace gestion_tarjetas_umg.Models.Estructuras.Arboles.ABB
{
    public class ArbolAbb<T> where T : Comparador<T>
    {
        public NodoAbb<T>? raiz;

        public ArbolAbb()
        {
            this.raiz = null;
        }

        public void Insertar(T nuevo)
        {
            raiz = InsertarRecursivo(raiz, nuevo);
        }

        private NodoAbb<T> InsertarRecursivo(NodoAbb<T>? subRaiz, T nuevo)
        {
            if (subRaiz ==  null) return new NodoAbb<T>(nuevo);

            if (nuevo.MenorQue(subRaiz.valor))
            {
                subRaiz.izq = InsertarRecursivo(subRaiz.izq, nuevo);
            }
            else if (nuevo.MayorQue(subRaiz.valor))
            {
                subRaiz.der = InsertarRecursivo(subRaiz.der, nuevo);
            }
            else
            {
                return subRaiz;
            }

            return subRaiz;
        }
    }
}
