using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Atko.GDLyra.Utility
{
    internal class ComputeCache<TInput, TResult> : IDictionary<TInput, TResult>
    {
        public int Count => Inner.Count;
        public bool IsReadOnly => false;

        IDictionary<TInput, TResult> Inner { get; } = new Dictionary<TInput, TResult>();

        public ICollection<TInput> Keys
        {
            get
            {
                lock (Inner)
                {
                    return Inner.Keys.ToArray();
                }
            }
        }

        public ICollection<TResult> Values
        {
            get
            {
                lock (Inner)
                {
                    return Inner.Values.ToArray();
                }
            }
        }

        public TResult Access(TInput input, Func<TInput, TResult> compute)
        {
            lock (Inner)
            {
                if (Inner.TryGetValue(input, out var cached))
                {
                    return cached;
                }

                var result = compute(input);
                Inner[input] = result;

                return result;
            }
        }

        public TResult Access(TInput input, Func<TResult> compute)
        {
            return Access(input, (key) => compute());
        }

        public TResult this[TInput key]
        {
            get
            {
                lock (Inner)
                {
                    return Inner[key];
                }
            }
            set
            {
                lock (Inner)
                {
                    Inner[key] = value;
                }
            }
        }

        public void Add(TInput key, TResult value)
        {
            lock (Inner)
            {
                Inner.Add(key, value);
            }
        }

        public bool ContainsKey(TInput key)
        {
            lock (Inner)
            {
                return Inner.ContainsKey(key);
            }
        }

        public bool Remove(TInput key)
        {
            lock (Inner)
            {
                return Inner.Remove(key);
            }
        }

        public bool TryGetValue(TInput key, out TResult value)
        {
            lock (Inner)
            {
                return Inner.TryGetValue(key, out value);
            }
        }

        public void Add(KeyValuePair<TInput, TResult> item)
        {
            lock (Inner)
            {
                Inner.Add(item);
            }
        }

        public void Clear()
        {
            lock (Inner)
            {
                Inner.Clear();
            }
        }

        public bool Contains(KeyValuePair<TInput, TResult> item)
        {
            lock (Inner)
            {
                return Inner.Contains(item);
            }
        }

        public void CopyTo(KeyValuePair<TInput, TResult>[] array, int arrayIndex)
        {
            lock (Inner)
            {
                Inner.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(KeyValuePair<TInput, TResult> item)
        {
            lock (Inner)
            {
                return Inner.Remove(item);
            }
        }


        public IEnumerator<KeyValuePair<TInput, TResult>> GetEnumerator()
        {
            lock (Inner)
            {
                return Inner.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (Inner)
            {
                return GetEnumerator();
            }
        }
    }
}