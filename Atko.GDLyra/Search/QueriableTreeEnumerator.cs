using System.Collections;
using System.Collections.Generic;
using Godot;

namespace Atko.GDLyra.Search
{
    public struct QueriableTreeEnumerator<TElement, TInner, TInnerElement> : IEnumerator<TElement>
        where TInner : struct, IEnumerator<TInnerElement>
        where TElement : class
        where TInnerElement : class
    {
        public TElement Current { get; private set; }
        object IEnumerator.Current => Current;

        TInner Inner;
        TreeQuery<TElement> Query;

        internal QueriableTreeEnumerator(TInner inner, TreeQuery<TElement> query)
        {
            Inner = inner;
            Query = query;
            Current = inner.Current as TElement;
        }

        public bool MoveNext()
        {
            while (Inner.MoveNext())
            {
                var casted = Inner.Current as TElement;
                if (casted != null)
                {
                    if (Query.Matches(Inner.Current as Node))
                    {
                        Current = casted;
                        return true;
                    }
                }
            }

            return false;
        }

        public void Reset()
        {
            Inner.Reset();
            Current = null;
        }

        public void Dispose()
        {
            Inner.Dispose();
        }
    }
}