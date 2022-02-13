using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Coderr.Server.WebSite.Infrastructure
{
    public class PendingRequestTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private static long _numberOfRequests;

        public PendingRequestTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public static long NumberOfRequests => _numberOfRequests;

        public async Task Invoke(HttpContext httpContext)
        {
            Interlocked.Increment(ref _numberOfRequests);
            try
            {
                await _next(httpContext);
            }
            finally
            {
                Interlocked.Decrement(ref _numberOfRequests);
            }
        }
    }
}