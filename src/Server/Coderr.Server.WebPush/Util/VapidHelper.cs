using System;
using System.Collections.Generic;
using Coderr.Server.WebPush.Model;
using Org.BouncyCastle.Crypto.Parameters;

namespace Coderr.Server.WebPush.Util
{
    public static class VapidHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

        /// <summary>
        ///     Generate vapid keys
        /// </summary>
        public static VapidDetails GenerateVapidKeys(string subject)
        {
            var keys = ECKeyHelper.GenerateKeys();
            var publicKey = ((ECPublicKeyParameters)keys.Public).Q.GetEncoded(false);
            var privateKey = ((ECPrivateKeyParameters)keys.Private).D.ToByteArrayUnsigned();


            var publicKeyBase64 = UrlBase64Helper.Encode(publicKey);
            var privateKeyBase64 = UrlBase64Helper.Encode(ByteArrayPadLeft(privateKey, 32));

            return new VapidDetails(subject, publicKeyBase64, privateKeyBase64);
        }

        /// <summary>
        ///     This method takes the required VAPID parameters and returns the required
        ///     header to be added to a Web Push Protocol Request.
        /// </summary>
        /// <param name="audience">This must be the origin of the push service.</param>
        /// <param name="details"></param>
        /// <returns>A dictionary of header key/value pairs.</returns>
        public static VapidHttpHeaders GetHttpHeaders(string audience, VapidDetails details)
        {
            if (details == null) throw new ArgumentNullException(nameof(details));
            ValidateAudience(audience);

            var decodedPrivateKey = UrlBase64Helper.Decode(details.PrivateKey);


            var header = new Dictionary<string, object> {{"typ", "JWT"}, {"alg", "ES256"}};

            var jwtPayload = new Dictionary<string, object>
            {
                {"aud", audience},
                {"exp", details.Expiration}, 
                {"sub", details.Subject}
            };

            var signingKey = ECKeyHelper.GetPrivateKey(decodedPrivateKey);

            var signer = new JwsSigner(signingKey);
            var token = signer.GenerateSignature(header, jwtPayload);

            return new VapidHttpHeaders
            {
                CryptoKey = "p256ecdsa=" + details.PublicKey,
                Authorization = "WebPush " + token
            };
        }

        public static void ValidateAudience(string audience)
        {
            if (string.IsNullOrEmpty(audience))
                throw new ArgumentException(@"No audience could be generated for VAPID.");

            if (audience.Length == 0)
                throw new ArgumentException(
                    @"The audience value must be a string containing the origin of a push service. " + audience);

            if (!Uri.IsWellFormedUriString(audience, UriKind.Absolute))
                throw new ArgumentException(@"VAPID audience is not a url.");
        }


        private static byte[] ByteArrayPadLeft(byte[] src, int size)
        {
            var dst = new byte[size];
            var startAt = dst.Length - src.Length;
            Array.Copy(src, 0, dst, startAt, src.Length);
            return dst;
        }

        
    }
}