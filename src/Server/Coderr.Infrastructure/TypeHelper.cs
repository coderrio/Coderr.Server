using System;
using System.Linq;

namespace codeRR.Server.Infrastructure
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
        /// <returns>created onstance</returns>
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
    }
}