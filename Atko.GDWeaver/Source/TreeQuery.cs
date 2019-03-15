using System;
using Godot;
using NullGuard;

namespace Atko.GDWeaver
{
    struct TreeQuery<T> where T : class
    {
        readonly string Name;
        readonly Predicate<T> Predicate;

        public TreeQuery([AllowNull] string name, [AllowNull] Predicate<T> predicate)
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