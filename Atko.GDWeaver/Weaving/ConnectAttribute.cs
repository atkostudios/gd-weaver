using System;
using NullGuard;

namespace Atko.GDWeaver.Weaving
{
    /// <summary>
    /// Attribute that marks a method as a signal handler.
    ///
    /// When <see cref="Weaver.Weave"/> is called on the current node, if no <see cref="Emitter"/> parameter is
    /// provided, the current node will be used as the source of the signal. If an <see cref="Emitter"/> is provided,
    /// the node contained in the field or property <see cref="Emitter"/> refers to will be used as the source of the
    /// signal.
    ///
    /// Connection of signals occurs after installation of nodes via <see cref="InstallAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ConnectAttribute : Attribute
    {
        /// <summary>
        /// The name of the signal to connect to the method.
        /// </summary>
        public string Signal { get; set; }

        /// <summary>
        /// The name of a field or property containing a reference to a node. If specified, the method will be connected
        /// to the signal on that node rather than the current node.
        /// </summary>
        [AllowNull]
        public string Emitter { get; set; }

        /// <summary>
        /// Bind the method to a signal emitted by the current node.
        /// </summary>
        /// <param name="signal"><see cref="Signal"/></param>
        public ConnectAttribute(string signal)
        {
            Signal = signal;
        }

        /// <summary>
        /// Bind the method to a signal emitted by a specific node field or property on the current node.
        /// </summary>
        /// <summary>
        /// <param name="emitter"><see cref="Emitter"/></param>
        /// <param name="signal"><see cref="Signal"/></param>
        public ConnectAttribute(string emitter, string signal)
        {
            Emitter = emitter;
            Signal = signal;
        }
    }
}