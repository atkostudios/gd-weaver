using System.Linq;
using Atko.Mirra.Images;
using Godot;

namespace Atko.GDWeaver.Weaving
{
    static class Connector
    {
        public static void Run(Node node)
        {
            var type = node.GetType().Image();

            foreach (var method in type.Methods().Instance())
            {
                foreach (var attribute in method.Attributes<ConnectAttribute>())
                {
                    var signal = attribute.Signal;
                    var emitter = attribute.Emitter == null
                        ? node
                        : type.Accessor(attribute.Emitter)?.Get(node) as Node;

                    if (emitter != null)
                    {
                        if (emitter.IsConnected(signal, node, method.ShortName))
                        {
                            continue;
                        }

                        emitter.Connect(signal, node, method.ShortName);
                    }
                }
            }
        }
    }
}