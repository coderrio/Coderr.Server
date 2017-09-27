using System;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.App.Modules.Geolocation
{
    /// <summary>
    ///     Geographic location of where an error report originated from.
    /// </summary>
    public class ErrorOrigin
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ErrorOrigin" />.
        /// </summary>
        /// <param name="ipAddress">IP address that we received the report from</param>
        /// <param name="longitude">Longitude that the IP lookup service returned.</param>
        /// <param name="latitude">Latitude that the IP lookup service returned.</param>
        public ErrorOrigin(string ipAddress, double longitude, double latitude)
        {
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            IpAddress = ipAddress;
            Longitude = longitude;
            Latitude = latitude;
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
        ///     Name of countrt
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        ///     IP address that we received the report from
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip")]
        public string IpAddress { get; private set; }

        /// <summary>Longitude that the IP lookup service returned.</summary>
        public double Latitude { get; private set; }

        /// <summary>Latitude that the IP lookup service returned.</summary>
        public double Longitude { get; private set; }

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