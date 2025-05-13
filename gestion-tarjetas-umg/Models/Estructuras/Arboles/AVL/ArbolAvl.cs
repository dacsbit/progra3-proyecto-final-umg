using gestion_tarjetas_umg.Models.Interfaces;

namespace gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL
{
    public class ArbolAvl<T> where T : Comparador<T>
    {
        public NodoAvl<T>? raiz;

        public ArbolAvl()
        {
            this.raiz = null;
        }

        public int Altura(NodoAvl<T>? nodo)
        {
            return nodo != null ? nodo.altura : 0;
        }

        public int FactorBalance(NodoAvl<T>? nodo)
        {
            return nodo != null ? Altura(nodo.izq) - Altura(nodo.der) : 0;
        }

        public void Insertar(T valor)
        {
            raiz = InsertarRecursivo(raiz, valor);
        }

        private NodoAvl<T> RotarDerecha(NodoAvl<T> y)
        {
            NodoAvl<T> x = y.izq!;       // x será la nueva raíz local del subárbol
            NodoAvl<T>? T2 = x.der;      // T2 es el subárbol derecho de x, que se mueve

            x.der = y;                   // y se convierte en hijo derecho de x
            y.izq = T2;                  // T2 se convierte en hijo izquierdo de y

            // Actualizamos alturas
            y.altura = 1 + Math.Max(Altura(y.izq), Altura(y.der));
            x.altura = 1 + Math.Max(Altura(x.izq), Altura(x.der));

            return x;                    // x es la nueva raíz del subárbol
        }

        private NodoAvl<T> RotarIzquierda(NodoAvl<T> y)
        {
            NodoAvl<T> x = y.der!;       // x será la nueva raíz local del subárbol
            NodoAvl<T>? T2 = x.izq;      // T2 es el subárbol izquierdo de x, que se mueve

            x.izq = y;                   // y se convierte en hijo izquierdo de x
            y.der = T2;                  // T2 se convierte en hijo derecho de y

            //Actualizamos alturas
            y.altura = 1 + Math.Max(Altura(y.izq), Altura(y.der));
            x.altura = 1 + Math.Max(Altura(x.izq), Altura(x.der));

            return x;                    // x es la nueva raíz del subárbol
        }

        private NodoAvl<T> InsertarRecursivo(NodoAvl<T>? subRaiz, T valor)
        {
            if (subRaiz == null)
            {
                return new NodoAvl<T>(valor);
            }
            else if (valor.MenorQue(subRaiz.valor))
            {
                subRaiz.izq = InsertarRecursivo(subRaiz.izq, valor);
            }
            else if (valor.MayorQue(subRaiz.valor))
            {
                subRaiz.der = InsertarRecursivo(subRaiz.der, valor);
            }
            else
            {
                return subRaiz;
            }

            subRaiz.altura = 1 + Math.Max(Altura(subRaiz.izq), Altura(subRaiz.der));
            int fb = FactorBalance(subRaiz);

            if (fb > 1 && valor.MenorQue(subRaiz.izq!.valor)) // Caso 1: izquierda - izquierda
            {
                return RotarDerecha(subRaiz);
            }else if (fb < -1 && valor.MayorQue(subRaiz.der!.valor)) // Caso 2: derecha - derecha
            {
                return RotarIzquierda(subRaiz);
            }else if (fb > 1 && valor.MayorQue(subRaiz.izq!.valor))
            {
                subRaiz.izq = RotarIzquierda(subRaiz.izq);
                return RotarDerecha(subRaiz);
            }else if (fb < -1 && valor.MenorQue(subRaiz.der!.valor))
            {
                subRaiz.der = RotarDerecha(subRaiz.der);
                return RotarIzquierda(subRaiz);
            }

            return subRaiz;
        }
    }
}
