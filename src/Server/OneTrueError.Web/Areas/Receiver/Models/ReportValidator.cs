using System;
using System.Security.Cryptography;
using System.Text;

namespace OneTrueError.Web.Areas.Receiver.Models
{
    public class ReportValidator
    {
        public static bool ValidateBody(string sharedSecret, string specifiedSignature, byte[] body)
        {
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