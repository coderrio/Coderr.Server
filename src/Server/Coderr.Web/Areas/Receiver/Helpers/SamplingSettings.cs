using System.Threading;

namespace codeRR.Server.Web.Areas.Receiver.Helpers
{
    public class SamplingSetting
    {
        private int _currentCount;
        public string AppKey { get; set; }
        public int Count { get; set; }

        /// <summary>
        ///     Accept all until the given count is reached (then ignore one). <c>false</c> means ignore all but the given count
        ///     index.
        /// </summary>
        public bool Inclusive { get; set; }

        public bool CanAccept()
        {
            var value = Interlocked.Increment(ref _currentCount);

            if (Inclusive)
            {
                var canAccept = value < Count;
                if (canAccept)
                    return true;

                _currentCount = 0;
                return false;
            }

            else
            {
                var canAccept = value >= Count;
                if (!canAccept)
                    return false;

                _currentCount = 0;
                return true;
            }
        }
    }
}