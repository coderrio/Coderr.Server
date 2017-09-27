using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using codeRR.Server.App.Modules.Triggers.Domain.Actions;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     Uses the IoC container to identify trigger actions
    /// </summary>
    [Component]
    public class ServiceLocatorTriggerActionFactory : ITriggerActionFactory
    {
        private static readonly Dictionary<string, Type> _actionTypes = new Dictionary<string, Type>();
        private readonly IServiceLocator _serviceLocator;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "How on earth could I do that?")]
        static ServiceLocatorTriggerActionFactory()
        {
            LoadTypes(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ServiceLocatorTriggerActionFactory" />.
        /// </summary>
        /// <param name="serviceLocator">IoC container</param>
        /// <exception cref="ArgumentNullException">serviceLocator</exception>
        public ServiceLocatorTriggerActionFactory(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        ///     Create action
        /// </summary>
        /// <param name="actionName">Name of the action to create</param>
        /// <returns>created action</returns>
        /// <exception cref="NotSupportedException">Action is not supported</exception>
        public ITriggerAction Create(string actionName)
        {
            Type type;
            if (!_actionTypes.TryGetValue(actionName, out type))
                throw new NotSupportedException("Do not support action of type " + actionName);

            return (ITriggerAction) _serviceLocator.Resolve(type);
        }

        /// <summary>
        ///     Find and load all trigger actions in the specified assembly
        /// </summary>
        /// <param name="assembly">assembly to search</param>
        public static void LoadTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            var types = from t in assembly.GetTypes()
                where t.GetCustomAttribute<TriggerActionNameAttribute>() != null
                select new {Type = t, Attrbibute = t.GetCustomAttribute<TriggerActionNameAttribute>()};
            foreach (var pair in types)
            {
                _actionTypes.Add(pair.Attrbibute.Name, pair.Type);
            }
        }
    }
}