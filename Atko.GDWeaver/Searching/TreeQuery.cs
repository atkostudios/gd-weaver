using System;
using Godot;

namespace Atko.GDWeaver.Searching
{
    public struct TreeQuery<T> where T : class
    {
        readonly string Name;
        readonly Predicate<T> Predicate;

        public TreeQuery(string name, Predicate<T> predicate)
        {
            Name = name;
            Predicate = predicate;
        }

        public bool Matches(Node node)
        {
            var casted = node as T;
            if (casted == null)
            {
                return false;
            }

            return (Name == null || node.Name == Name) && (Predicate == null || Predicate(casted));
        }
    }
}