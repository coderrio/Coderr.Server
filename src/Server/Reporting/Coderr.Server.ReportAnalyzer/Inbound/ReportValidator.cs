using System;
using System.Security.Cryptography;
using System.Text;

namespace Coderr.Server.ReportAnalyzer.Inbound
{
    /// <summary>
    ///     Used to make sure that the uploaded report was signed with the correct shared secret.
    /// </summary>
    public class ReportValidator
    {
        /// <summary>
        ///     Validate HTTP body
        /// </summary>
        /// <param name="sharedSecret">Shared secret associated with the AppKey.</param>
        /// <param name="specifiedSignature">Signature that the client have generated.</param>
        /// <param name="body">HTTP body</param>
        /// <returns><c>true</c> if the specifiedSignature was generated with the shared secret; otherwise <c>false</c>.</returns>
        public static bool ValidateBody(string sharedSecret, string specifiedSignature, byte[] body)
        {
            if (sharedSecret == null) throw new ArgumentNullException("sharedSecret");
            if (specifiedSignature == null) throw new ArgumentNullException("specifiedSignature");
            if (body == null) throw new ArgumentNullException("body");

            var hashAlgo = new HMACSHA256(Encoding.UTF8.GetBytes(sharedSecret.ToLower()));
            var hash = hashAlgo.ComputeHash(body);
            var signature = Convert.ToBase64String(hash);

            var hashAlgo1 = new HMACSHA256(Encoding.UTF8.GetBytes(sharedSecret.ToUpper()));
            var hash1 = hashAlgo1.ComputeHash(body);
            var signature1 = Convert.ToBase64String(hash1);


            // uri encoding :(
            specifiedSignature = specifiedSignature.Replace(' ', '+');

            return specifiedSignature.Equals(signature) || specifiedSignature.Equals(signature1);
        }
    }
}