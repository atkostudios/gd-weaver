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
            var methods = type.Methods().Where((current) => !current.IsStatic);

            foreach (var method in methods)
            {
                var attributes = method.Attributes<ConnectAttribute>();
                foreach (var attribute in attributes)
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