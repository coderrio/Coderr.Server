using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Portable.Licensing;
using Portable.Licensing.Validation;

namespace Coderr.Server.Web.Infrastructure.Misc
{
    public class License
    {
        private static SecureString _publicKey;
        public static int Count = 25000;
        private static Portable.Licensing.License _license;

        private static readonly GeneralValidationFailure UnlicensedApplications = new GeneralValidationFailure
        {
            HowToResolve = "Delete unlicensed applications or buy additional applications",
            Message = "You have created more applications than your license allows."
        };

        static License()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "coderr.lic");
            _license = Portable.Licensing.License.Load(file);

            ValidationErrors = _license.Validate()  
                .ExpirationDate()  
                .And()  
                .Signature(_publicKey.ToString())  
                .And()
                .AssertThat(x=>x.Quantity <= CreatedApplicationCount, UnlicensedApplications)
                .AssertValidLicense();
        }

        public static int CreatedApplicationCount { get; set; }

        public static IEnumerable<IValidationFailure> ValidationErrors { get; private set; }

        public static int ApplicationCount => _license.Quantity;

        public static string LicensedTo => _license.Customer.Company;

        public static DateTime Expires => _license.Expiration;

        
    }
}
