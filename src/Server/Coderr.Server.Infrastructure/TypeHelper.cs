using System;
using System.Linq;
using System.Reflection;

namespace Coderr.Server.Infrastructure
{
    /// <summary>
    ///     Reflection helper.
    /// </summary>
    public class TypeHelper
    {
        /// <summary>
        ///     Create an instance of an assembly type
        /// </summary>
        /// <param name="typeName">Type name, assembly name</param>
        /// <returns>created instance</returns>
        /// <remarks>
        ///     <para>
        ///         Can be used when the type name is not a fully qualified assembly name.
        ///     </para>
        /// </remarks>
        public static object CreateAssemblyObject(string typeName)
        {
            var parts = typeName.Split(',').Select(x => x.Trim()).ToArray();
            var assemblyName = parts[1];
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.Equals(assemblyName));
            if (asm == null)
                throw new InvalidOperationException("Failed to find assembly '" + assemblyName + "'.");
            var type = asm.GetType(parts[0]);
            if (type == null)
                throw new InvalidOperationException("Failed to find type '" + parts[0] + "' in assembly '" +
                                                    assemblyName + "'.");
            return Activator.CreateInstance(type);
        }


        /// <summary>
        ///     Create an instance of an assembly type
        /// </summary>
        /// <param name="typeName">Type name, assembly name</param>
        /// <returns>created instance</returns>
        /// <remarks>
        ///     <para>
        ///         Can be used when the type name is not a fully qualified assembly name.
        ///     </para>
        /// </remarks>
        public static Type CreateAssemblyType(string typeName)
        {
            var parts = typeName.Split(',').Select(x => x.Trim()).ToArray();
            var assemblyName = parts[1];
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.Equals(assemblyName));
            if (asm == null)
                throw new InvalidOperationException("Failed to find assembly '" + assemblyName + "'.");
            var type = asm.GetType(parts[0]);
            if (type == null)
                throw new InvalidOperationException("Failed to find type '" + parts[0] + "' in assembly '" +
                                                    assemblyName + "'.");
            return type;
        }

        /// <summary>
        ///     Create an instance of an assembly type
        /// </summary>
        /// <param name="typeName">Type name, assembly name</param>
        /// <param name="constructorArguments"></param>
        /// <returns>created instance</returns>
        /// <remarks>
        ///     <para>
        ///         Can be used when the type name is not a fully qualified assembly name.
        ///     </para>
        /// </remarks>
        public static object CreateAssemblyObject(string typeName, params object[] constructorArguments)
        {
            var parts = typeName.Split(',').Select(x => x.Trim()).ToArray();
            var assemblyName = parts[1];
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.Equals(assemblyName));
            if (asm == null)
                throw new InvalidOperationException("Failed to find assembly '" + assemblyName + "'.");
            var type = asm.GetType(parts[0]);
            if (type == null)
                throw new InvalidOperationException("Failed to find type '" + parts[0] + "' in assembly '" +
                                                    assemblyName + "'.");

            var constructor = type
                .GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Length == constructorArguments.Length);
            if (constructor == null)
                throw new NotSupportedException(
                    $"Failed to find constructor for {typeName} with arguments [{string.Join(",", constructorArguments)}]");

            return constructor.Invoke(constructorArguments);
        }

    }
}