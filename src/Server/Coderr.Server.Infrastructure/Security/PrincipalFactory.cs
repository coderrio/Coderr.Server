using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace codeRR.Server.Infrastructure.Security
{
    /// <summary>
    ///     Creates the security principal that is used by the system
    /// </summary>
    public class PrincipalFactory : IPrincipalFactory
    {
        private static IPrincipalFactory _instance = new PrincipalFactory();

        public static void Configure(string principalFactoryType)
        {
            if (principalFactoryType == null) throw new ArgumentNullException(nameof(principalFactoryType));
            _instance = (IPrincipalFactory)TypeHelper.CreateAssemblyObject(principalFactoryType);
        }

        Task<ClaimsPrincipal> IPrincipalFactory.CreateAsync(PrincipalFactoryContext context)
        {
            return CreatePrincipalAsync(context);
        }

        /// <summary>
        ///     Create a new principal
        /// </summary>
        /// <param name="context">information that should be stored in the created principal</param>
        /// <returns>created principal</returns>
        public static Task<ClaimsPrincipal> CreateAsync(PrincipalFactoryContext context)
        {
            return _instance.CreateAsync(context);
        }

        protected virtual Task<ClaimsPrincipal> CreatePrincipalAsync(PrincipalFactoryContext context)
        {
            var claims = new List<Claim>();
            if (context.Claims != null)
                claims.AddRange(context.Claims);
            if (claims.All(x => x.Type != ClaimTypes.NameIdentifier))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, context.AccountId.ToString(), ClaimValueTypes.String));
            if (claims.All(x => x.Type != ClaimTypes.Name))
                claims.Add(new Claim(ClaimTypes.Name, context.UserName, ClaimValueTypes.String));

            if (context.Roles != null)
                claims.AddRange(context.Roles.Select(role => new Claim(ClaimTypes.Role, role, ClaimValueTypes.String)));

            var identity = new ClaimsIdentity(claims, context.AuthenticationType, ClaimTypes.Name, ClaimTypes.Role);
            return Task.FromResult(new ClaimsPrincipal(identity));
        }
    }
}