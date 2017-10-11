using System.Collections.Generic;
using codeRR.Client.ContextProviders;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;

namespace codeRR.Server.Web.Infrastructure
{
    public class CustomerInfoProvider : IContextInfoProvider
    {
        private readonly string _contactEmail;
        private readonly string _installationId;

        public CustomerInfoProvider(string contactEmail, string installationId)
        {
            _contactEmail = contactEmail;
            _installationId = installationId;
        }

        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var dict = new Dictionary<string, string>
            {
                {"InstallationId", _installationId},
                {"ContactEmail", _contactEmail}
            };
            return new ContextCollectionDTO("CustomerInfo", dict);
        }

        public string Name
        {
            get { return "CustomerInfo"; }
        }
    }
}