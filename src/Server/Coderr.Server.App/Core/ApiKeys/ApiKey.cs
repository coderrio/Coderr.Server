using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using codeRR.Server.Infrastructure.Security;

namespace codeRR.Server.App.Core.ApiKeys
{
    /// <summary>
    ///     A generated API key which can be used to call codeRR´s HTTP api.
    /// </summary>
    public class ApiKey
    {
        private List<Claim> _claims = new List<Claim>();

        /// <summary>
        ///     Application that will be using this key
        /// </summary>
        public string ApplicationName { get; set; }


        /// <summary>
        ///     Claims associated with this key
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Typically contains <see cref="CoderrClaims.Application" /> to identity which applications the key can access.
        ///     </para>
        /// </remarks>
        public Claim[] Claims { get { return _claims.ToArray(); } private set { _claims = new List<Claim>(value); } }

        /// <summary>
        ///     When this key was generated
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     AccountId that generated this key
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        ///     Api key
        /// </summary>
        public string GeneratedKey { get; set; }

        /// <summary>
        ///     PK
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        ///     Used when generating signatures.
        /// </summary>
        public string SharedSecret { get; set; }

        /// <summary>
        ///     Add an application that this ApiKey can be used for.
        /// </summary>
        /// <param name="applicationId">application id</param>
        public void Add(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");

            _claims.Add(new Claim(CoderrClaims.Application, applicationId.ToString(), ClaimValueTypes.Integer32));
        }

        /// <summary>
        ///     Validate a given signature using the HTTP body.
        /// </summary>
        /// <param name="specifiedSignature">Signature passed from the client</param>
        /// <param name="body">HTTP body (i.e. the data that the signature was generated on)</param>
        /// <returns><c>true</c> if the signature was generated using the shared secret; otherwise <c>false</c>.</returns>
        public bool ValidateSignature(string specifiedSignature, byte[] body)
        {
            var hashAlgo = new HMACSHA256(Encoding.UTF8.GetBytes(SharedSecret.ToLower()));
            var hash = hashAlgo.ComputeHash(body);
            var signature = Convert.ToBase64String(hash);

            var hashAlgo1 = new HMACSHA256(Encoding.UTF8.GetBytes(SharedSecret.ToUpper()));
            var hash1 = hashAlgo1.ComputeHash(body);
            var signature1 = Convert.ToBase64String(hash1);

            return specifiedSignature.Equals(signature) || specifiedSignature.Equals(signature1);
        }
    }
}