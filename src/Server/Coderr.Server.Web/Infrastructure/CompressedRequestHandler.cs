using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace codeRR.Server.Web.Infrastructure
{
    public class CompressedRequestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (IsRequetCompressed(request))
                request.Content = DecompressRequestContent(request);

            return base.SendAsync(request, cancellationToken);
        }

        private HttpContent DecompressRequestContent(HttpRequestMessage request)
        {
            // Read in the input stream, then decompress in to the output stream.
            // Doing this asynchronously, but not really required at this point
            // since we end up waiting on it right after this.
            Stream outputStream = new MemoryStream();
            Task task = request.Content.ReadAsStreamAsync()
                .ContinueWith(t =>
                {
                    var inputStream = t.Result;
                    var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);

                    gzipStream.CopyTo(outputStream);
                    gzipStream.Dispose();

                    outputStream.Seek(0, SeekOrigin.Begin);
                });

            // Wait for inputstream and decompression to complete. Would be nice
            // to not block here and work asynchronous when ready instead, but I couldn't 
            // figure out how to do it in context of a DelegatingHandler.
            task.Wait();

            // Save the original content
            var origContent = request.Content;

            // Replace request content with the newly decompressed stream
            HttpContent newContent = new StreamContent(outputStream);

            // Copy all headers from original content in to new one
            foreach (var header in origContent.Headers)
                newContent.Headers.Add(header.Key, header.Value);

            return newContent;
        }

        private bool IsRequetCompressed(HttpRequestMessage request)
        {
            if (request.Content.Headers.ContentEncoding != null &&
                request.Content.Headers.ContentEncoding.Contains("gzip"))
                return true;

            return false;
        }
    }
}