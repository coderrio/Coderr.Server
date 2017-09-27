using System;

namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     Used to be able to create instances of trigger actions (from when we load configuration data from the data source)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TriggerActionNameAttribute : Attribute
    {
        /// <summary>
        ///     Creates a new instance of <see cref="TriggerActionNameAttribute" />.
        /// </summary>
        /// <param name="name">action name</param>
        /// <exception cref="ArgumentNullException">name</exception>
        public TriggerActionNameAttribute(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
        }

        /// <summary>
        ///     Action name
        /// </summary>
        public string Name { get; private set; }
    }
}