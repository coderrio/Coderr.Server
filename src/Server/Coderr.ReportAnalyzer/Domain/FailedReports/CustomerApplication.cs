using System;
using System.Security.Cryptography;
using System.Text;

namespace codeRR.ReportAnalyzer.Domain.FailedReports
{
    /// <summary>
    ///     Small wrapper around an application
    /// </summary>
    public class CustomerApplication
    {
        /// <summary>
        ///     Shared secret for the application
        /// </summary>
        public string SharedSecret { get; set; }

        /// <summary>
        ///     Validate received report body
        /// </summary>
        /// <param name="specifiedSignature">signature provided in the HTTP request</param>
        /// <param name="body">Compressed body</param>
        /// <returns><c>true</c> if HMAC authentication is OK; otherwise <c>false</c>.</returns>
        public bool ValidateBody(string specifiedSignature, byte[] body)
        {
            var hashAlgo = new HMACSHA256(Encoding.UTF8.GetBytes(SharedSecret));
            var hash = hashAlgo.ComputeHash(body);
            var signature = Convert.ToBase64String(hash);

            // uri encoding :(
            specifiedSignature = specifiedSignature.Replace(' ', '+');

            return specifiedSignature.Equals(signature);
        }
    }
}