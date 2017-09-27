using System;

namespace codeRR.Server.App.Modules.Triggers.Domain.Actions
{
    /// <summary>
    ///     used to create trigger actions by using their name
    /// </summary>
    public interface ITriggerActionFactory
    {
        /// <summary>
        ///     Create a trigger
        /// </summary>
        /// <param name="actionName">trigger name</param>
        /// <returns>trigger</returns>
        /// <exception cref="NotSupportedException">no trigger have been mapped for that name.</exception>
        ITriggerAction Create(string actionName);
    }
}