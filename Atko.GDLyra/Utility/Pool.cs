using System.Collections.Generic;

namespace Atko.GDLyra.Utility
{
    internal class Pool<T> where T : new()
    {
        Stack<T> Stack { get; } = new Stack<T>();

        public T Get()
        {
            lock (Stack)
            {
                if (Stack.Count == 0)
                {
                    return new T();
                }

                return Stack.Pop();
            }
        }

        public void Return(T obj)
        {
            lock (Stack)
            {
                Stack.Push(obj);
            }
        }
    }
}