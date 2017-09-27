using System.IO;
using System.IO.Compression;
using System.Text;

namespace codeRR.Server.ReportAnalyzer.Domain.Reports
{
    /// <summary>
    ///     Decompresses report from GZIP compression
    /// </summary>
    public class ReportDecompressor
    {
        /// <summary>
        ///     Deflate a compressed error report in JSON format
        /// </summary>
        /// <param name="errorReport">Compressed JSON errorReport</param>
        /// <returns>JSON string decompressed</returns>
        public string Deflate(byte[] errorReport)
        {
            //owned and disposed by decompressor
            var zipStream = new MemoryStream(errorReport);

            using (var deflateStream = new MemoryStream())
            {
                using (var decompressor = new GZipStream(zipStream, CompressionMode.Decompress))
                {
                    decompressor.CopyTo(deflateStream);
                    deflateStream.Position = 0;
                    var buffer = new byte[deflateStream.Length];
                    deflateStream.Read(buffer, 0, (int) deflateStream.Length);
                    var strBuffer = Encoding.UTF8.GetString(buffer);
                    return strBuffer;
                }
            }
        }
    }
}