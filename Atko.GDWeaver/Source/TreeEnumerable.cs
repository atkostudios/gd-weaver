using System;
using System.Collections;
using System.Collections.Generic;
using NullGuard;

namespace Atko.GDWeaver
{
    public struct TreeEnumerable<TElement, TInner, TInnerElement> : IEnumerable<TElement>
        where TElement : class
        where TInner : struct, IEnumerator<TInnerElement>
        where TInnerElement : class
    {
        TInner Inner;
        TreeQuery<TElement> Query;
        int Skip;

        internal TreeEnumerable(TInner inner, int skip = 0, TreeQuery<TElement> query = default(TreeQuery<TElement>))
        {
            Inner = inner;
            Query = query;
            Skip = skip;
        }

        public TreeEnumerable<T,
                QueriableTreeEnumerator<T, QueriableTreeEnumerator<TElement, TInner, TInnerElement>, TElement>, T>
            Every<T>() where T : class
        {
            return Every(new TreeQuery<T>(null, null));
        }

        public TreeEnumerable<T,
                QueriableTreeEnumerator<T, QueriableTreeEnumerator<TElement, TInner, TInnerElement>, TElement>, T>
            Every<T>(string name) where T : class
        {
            return Every(new TreeQuery<T>(name, null));
        }

        public TreeEnumerable<TElement,
            QueriableTreeEnumerator<TElement, QueriableTreeEnumerator<TElement, TInner, TInnerElement>, TElement>,
            TElement> Every(string name)
        {
            return Every<TElement>(name);
        }

        public TreeEnumerable<T,
            QueriableTreeEnumerator<T, QueriableTreeEnumerator<TElement, TInner, TInnerElement>, TElement>, T> Every<T>(
            Predicate<T> predicate) where T : class
        {
            return Every(new TreeQuery<T>(null, predicate));
        }

        public TreeEnumerable<TElement,
            QueriableTreeEnumerator<TElement, QueriableTreeEnumerator<TElement, TInner, TInnerElement>, TElement>,
            TElement> Every(Predicate<TElement> predicate)
        {
            return Every<TElement>(predicate);
        }

        [return: AllowNull]
        public T At<T>() where T : class
        {
            return At(new TreeQuery<T>(null, null));
        }

        [return: AllowNull]
        public T At<T>(string name) where T : class
        {
            return At(new TreeQuery<T>(name, null));
        }

        [return: AllowNull]
        public T At<T>(Predicate<T> predicate) where T : class
        {
            return At(new TreeQuery<T>(null, predicate));
        }

        [return: AllowNull]
        public TElement At(string name)
        {
            return At<TElement>(name);
        }

        [return: AllowNull]
        public TElement At(Predicate<TElement> predicate)
        {
            return At<TElement>(predicate);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return GetTypedEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        QueriableTreeEnumerator<TElement, TInner, TInnerElement> GetTypedEnumerator()
        {
            var enumerator = new QueriableTreeEnumerator<TElement, TInner, TInnerElement>(Inner, Query);
            for (var i = 0; i < Skip; i++)
            {
                enumerator.MoveNext();
            }

            return enumerator;
        }

        TreeEnumerable<TType,
                QueriableTreeEnumerator<TType, QueriableTreeEnumerator<TElement, TInner, TInnerElement>, TElement>,
                TType>
            Every<TType>(TreeQuery<TType> query) where TType : class
        {
            var inner = GetTypedEnumerator();
            var enumerator =
                new QueriableTreeEnumerator<TType, QueriableTreeEnumerator<TElement, TInner, TInnerElement>, TElement>(
                    inner, query);

            return new TreeEnumerable<TType, QueriableTreeEnumerator<TType,
                QueriableTreeEnumerator<TElement, TInner, TInnerElement>, TElement>, TType>(enumerator);
        }

        [return: AllowNull]
        TType At<TType>(TreeQuery<TType> query) where TType : class
        {
            foreach (var element in Every(query))
            {
                return element;
            }

            return null;
        }
    }
}