using System;
using System.Text;

namespace codeRR.Server.Api.Client
{
    internal static class StringBuilderExtensions
    {
        public static void AppendUrlEncoded(this StringBuilder sb, string name, string value)
        {
            if (sb.Length != 0)
                sb.Append("&");
            sb.Append(Uri.EscapeDataString(name));
            sb.Append("=");
            sb.Append(Uri.EscapeDataString(value));
        }
    }
}