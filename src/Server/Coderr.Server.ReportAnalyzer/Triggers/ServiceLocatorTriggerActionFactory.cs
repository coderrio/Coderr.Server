using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.Triggers.Actions;
using Coderr.Server.ReportAnalyzer.Triggers.Handlers.Actions;
using DotNetCqs.DependencyInjection;
using Coderr.Server.Abstractions.Boot;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Triggers
{
    /// <summary>
    ///     Uses the IoC container to identify trigger actions
    /// </summary>
    [ContainerService]
    public class ServiceLocatorTriggerActionFactory : ITriggerActionFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<string, Type> _actionTypes = new Dictionary<string, Type>();

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "How on earth could I do that?")]
        static ServiceLocatorTriggerActionFactory()
        {
            LoadTypes(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ServiceLocatorTriggerActionFactory" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">serviceLocator</exception>
        public ServiceLocatorTriggerActionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

            return (ITriggerAction) _serviceProvider.GetService(type);
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