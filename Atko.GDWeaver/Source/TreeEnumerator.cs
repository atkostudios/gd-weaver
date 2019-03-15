using System.Collections;
using System.Collections.Generic;
using Godot;
using NullGuard;

namespace Atko.GDWeaver
{
    public struct TreeEnumerator : IEnumerator<Node>
    {
        static readonly Pool<Stack<Node>> Stacks = new Pool<Stack<Node>>();
        static readonly Pool<Queue<Node>> Queues = new Pool<Queue<Node>>();

        [AllowNull]
        public Node Current { get; private set; }

        [AllowNull]
        object IEnumerator.Current => Current;

        readonly TreeEnumerationMode Mode;
        readonly Node Root;
        readonly Queue<Node> Queue;
        readonly Stack<Node> Stack;

        int Index;
        Node Next;

        internal TreeEnumerator(Node root, TreeEnumerationMode mode)
        {
            Mode = mode;
            Root = root;
            Stack = null;
            Queue = null;
            Index = -1;
            Current = null;
            Next = null;

            switch (Mode)
            {
                case TreeEnumerationMode.DescendBF:
                    Queue = Queues.Get();
                    Queue.Clear();
                    Queue.Enqueue(Root);
                    break;
                case TreeEnumerationMode.DescendDF:
                    Stack = Stacks.Get();
                    Stack.Clear();
                    Stack.Push(Root);
                    break;
                case TreeEnumerationMode.Children:
                    break;
                default:
                    Next = root;
                    break;
            }
        }

        public bool MoveNext()
        {
            switch (Mode)
            {
                case TreeEnumerationMode.DescendBF:
                    return MoveNextDescendBF();
                case TreeEnumerationMode.DescendDF:
                    return MoveNextDescendDF();
                case TreeEnumerationMode.Children:
                    return MoveNextChildren();
                default:
                    return MoveNextAscend();
            }
        }

        public void Reset()
        {
            switch (Mode)
            {
                case TreeEnumerationMode.DescendBF:
                    ResetDescendBF();
                    break;
                case TreeEnumerationMode.DescendDF:
                    ResetDescendDF();
                    break;
                case TreeEnumerationMode.Children:
                    ResetChildren();
                    break;
                default:
                    ResetAscend();
                    break;
            }
        }

        public void Dispose()
        {
            switch (Mode)
            {
                case TreeEnumerationMode.DescendBF:
                    DisposeDescendBF();
                    break;
                case TreeEnumerationMode.DescendDF:
                    DisposeDescendDF();
                    break;
                case TreeEnumerationMode.Children:
                    DisposeChildren();
                    break;
                default:
                    DisposeAscend();
                    break;
            }
        }

        bool MoveNextDescendBF()
        {
            if (Queue.Count == 0)
            {
                return false;
            }

            Current = Queue.Dequeue();
            if (Current != null)
            {
                var count = Current.GetChildCount();
                for (var i = 0; i < count; i++)
                {
                    Queue.Enqueue(Current.GetChild(i));
                }
            }

            return true;
        }

        void ResetDescendBF()
        {
            Queue.Clear();
            Queue.Enqueue(Root);
            Current = null;
        }

        void DisposeDescendBF()
        {
            Queue.Clear();
            Queues.Return(Queue);
        }

        bool MoveNextDescendDF()
        {
            if (Stack.Count == 0)
            {
                return false;
            }

            Current = Stack.Pop();
            if (Current != null)
            {
                var count = Current.GetChildCount();
                for (var i = count - 1; i >= 0; i--)
                {
                    Stack.Push(Current.GetChild(i));
                }
            }

            return true;
        }

        void ResetDescendDF()
        {
            Stack.Clear();
            Stack.Push(Root);
            Current = null;
        }

        void DisposeDescendDF()
        {
            Stack.Clear();
            Stacks.Return(Stack);
        }

        bool MoveNextChildren()
        {
            Index++;
            var running = Index < Root.GetChildCount();
            Current = running ? Root.GetChild(Index) : null;
            return running;
        }

        void ResetChildren()
        {
            Index = -1;
        }

        void DisposeChildren()
        { }

        bool MoveNextAscend()
        {
            if (Current == Next || Next == null)
            {
                return false;
            }

            Current = Next;
            Next = Next.GetParent();

            return true;
        }

        void ResetAscend()
        {
            Current = null;
            Next = Root;
        }

        void DisposeAscend()
        { }
    }
}