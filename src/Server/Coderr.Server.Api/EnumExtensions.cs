using System;

namespace codeRR.Server.Api.Core
{
    /// <summary>
    ///     Extensions making it easier to work with enums
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        ///     Convert from one enum type to another
        /// </summary>
        /// <typeparam name="TTo">Type to convert to</typeparam>
        /// <param name="source">source</param>
        /// <returns>Converted enum value</returns>
        /// <remarks>
        ///     <para>
        ///         Does the conversion by translating the value to a string and then parsing it. That chocie was made
        ///         since the same value might exist in both enums by representing different fields.
        ///     </para>
        /// </remarks>
        /// <exception cref="FormatException">Source enum value was not found in the target type.</exception>
        public static TTo ConvertEnum<TTo>(this Enum source) where TTo : struct
        {
            var str = source.ToString();
            TTo result;
            if (!Enum.TryParse(str, true, out result))
            {
                throw new FormatException(
                    string.Format("Cannot convert enum of type '{0}' with value '{1}' to enum type '{2}'.",
                        source.GetType().FullName, source, typeof(TTo).FullName));
            }

            return result;
        }
    }
}