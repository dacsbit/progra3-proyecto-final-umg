using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
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

        public (NodoAvl<T>?, bool) Buscar(T valor)
        {
            return BuscarRecursivo(raiz, valor);
        }

        public (NodoAvl<T>?, bool) BuscarRecursivo(NodoAvl<T>? subRaiz,  T valor)
        {
            if (subRaiz == null) return (null, false);

            if (valor.IgualQue(subRaiz.valor)) return (subRaiz, true);

            if (valor.MenorQue(subRaiz.valor))
            {
                return BuscarRecursivo(subRaiz.izq, valor);
            }
            else
            {
                return BuscarRecursivo(subRaiz.der, valor);
            }
        }

        public bool Existe(T valor)
        {
            (_, bool encontrado) = Buscar(valor);

            return encontrado;
        }

        private NodoAvl<T> MinimoValorNodo(NodoAvl<T> nodo)
        {
            NodoAvl<T> actual = nodo;
            while (actual.izq != null)
                actual = actual.izq;

            return actual;
        }

        public bool Eliminar(T valor)
        {
            bool eliminado;
            (raiz, eliminado) = EliminarRecursivo(raiz, valor);

            return eliminado;
        }

        private (NodoAvl<T>?, bool) EliminarRecursivo(NodoAvl<T>? subRaiz, T valor)
        {
            if (subRaiz == null) return (null, false);

            bool eliminado = false;

            if (valor.MenorQue(subRaiz.valor))
            {
                (subRaiz.izq, eliminado) = EliminarRecursivo(subRaiz.izq, valor);
            }
            else if (valor.MayorQue(subRaiz.valor))
            {
                (subRaiz.der, eliminado) = EliminarRecursivo(subRaiz.der, valor);
            }
            else
            {
                eliminado = true;

                //Caso en donde la sub raiz tiene 1 o ningun hijo
                if (subRaiz.izq == null || subRaiz.der == null)
                {
                    NodoAvl<T>? temp = subRaiz.izq ?? subRaiz.der;
                    subRaiz = temp;
                }
                else
                {
                    //Caso donde el nodo tiene 2 hijos: encontrar el sucesor
                    NodoAvl<T> sucesor = MinimoValorNodo(subRaiz.der!);

                    subRaiz.valor = sucesor.valor;
                    (subRaiz.der, _) = EliminarRecursivo(subRaiz.der, sucesor.valor);
                }
            }

            if (subRaiz == null) return (null, eliminado);

            //tras eliminar se vuelve a calcular la altura en caso de que aun quede al menos 1 nodo
            subRaiz.altura = 1 + Math.Max(Altura(subRaiz.izq), Altura(subRaiz.der));

            //Calculamos el Factor de Balance
            int fb = FactorBalance(subRaiz);

            if(fb > 1 && FactorBalance(subRaiz.izq) >= 0)
            {
                return (RotarDerecha(subRaiz), eliminado);
            }
            else if (fb > 1 && FactorBalance(subRaiz.izq) < 0)
            {
                subRaiz.izq = RotarIzquierda(subRaiz.izq!);
                return (RotarDerecha(subRaiz), eliminado);
            }
            else if (fb < -1 && FactorBalance(subRaiz.der) < 0)
            {
                return (RotarIzquierda(subRaiz), eliminado);
            }
            else if (fb < -1 && FactorBalance(subRaiz.der) >= 0)
            {
                subRaiz.der = RotarDerecha(subRaiz.der!);
                return (RotarIzquierda(subRaiz), eliminado);
            }

            return (subRaiz, eliminado);
        }

        public bool Modificar(T viejo, T nuevo)
        {
            //Si el nuevo valor a ingresar ya existe no se realiza la modificacion para evitar duplicados
            if (Existe(nuevo)) return false;
            
            //Primero se elimina el valor antiguo
            bool eliminado = Eliminar(viejo);

            if (eliminado)
            {
                Insertar(nuevo);
                return true;
            }

            //si el valor no existe no se puede eliminar, por lo tanto no se puede modificar
            return false;
        }

        public List<T> ToList()
        {
            List<T> lista = new();
            InOrden(raiz, lista);
            return lista;
        }

        public void InOrden(NodoAvl<T>? subRaiz, List<T> lista)
        {
            if (subRaiz == null) return;
            InOrden(subRaiz.izq, lista);
            lista.Add(subRaiz.valor);
            InOrden(subRaiz.der, lista);
        }
    }
}
