using System;
using System.Diagnostics.CodeAnalysis;

namespace Coderr.Server.Domain.Modules.ErrorOrigins
{
    /// <summary>
    ///     Geographic location of where an error report originated from.
    /// </summary>
    public class ErrorOrigin
    {
        public const double EmptyLatitude = 91;
        public const double EmptyLongitude = 181;

        /// <summary>
        ///     Creates a new instance of <see cref="ErrorOrigin" />.
        /// </summary>
        /// <param name="ipAddress">IP address that we received the report from</param>
        /// <param name="longitude">Longitude that the IP lookup service returned.</param>
        /// <param name="latitude">Latitude that the IP lookup service returned.</param>
        public ErrorOrigin(string ipAddress, double longitude, double latitude)
        {
            if (string.IsNullOrEmpty(ipAddress) && longitude <= 0 && latitude <= 0)
                throw new ArgumentException("Either IPAddress or long/lat must be specified.");

            IpAddress = ipAddress;
            Longitude = longitude;
            Latitude = latitude;
        }

        public ErrorOrigin(string ipAddress)
        {
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            Longitude = EmptyLongitude;
            Latitude = EmptyLatitude;
        }

        /// <summary>
        ///     For the mapper.
        /// </summary>
        protected ErrorOrigin()
        {
        }

        /// <summary>
        ///     City reported by the lookup service.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     Country code (top domain)
        /// </summary>
        public string CountryCode { get; set; }


        /// <summary>
        ///     Name of country.
        /// </summary>
        public string CountryName { get; set; }

        public int Id { get; set; }

        /// <summary>
        ///     IP address that we received the report from
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip")]
        public string IpAddress { get; private set; }

        /// <summary>Longitude that the IP lookup service returned.</summary>
        /// <remarks>
        ///     91 if not specified
        /// </remarks>
        public double Latitude { get; set; }

        /// <summary>
        ///     Latitude that the IP lookup service returned.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         181 if not specified.
        ///     </para>
        /// </remarks>
        public double Longitude { get; set; }

        /// <summary>
        ///     TODO: WTF IS THIS?!
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        ///     Country name
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        ///     Zip / postal code.
        /// </summary>
        public string ZipCode { get; set; }
    }
}