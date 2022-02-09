
namespace Coderr.Server.WebPush.Util
{
    public class EncryptionResult
    {
        public byte[] PublicKey { get; set; }
        public byte[] Payload { get; set; }
        public byte[] Salt { get; set; }

        public string Base64EncodePublicKey()
        {
            return UrlBase64Helper.Encode(PublicKey);
        }

        public string Base64EncodeSalt()
        {
            return UrlBase64Helper.Encode(Salt);
        }
    }
}