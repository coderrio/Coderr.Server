using System;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters.Normalizers
{
    /// <summary>
    ///     Divides memory usage into ranges,
    /// </summary>
    public static class MemoryNormalizer
    {
        /// <summary>
        ///     Divide using a predefined segmentation size.
        /// </summary>
        /// <param name="memoryInMb">Memory usage</param>
        /// <param name="segmentSize">segment size in Mb</param>
        /// <returns>memory usage like "Range 10-50Mb"</returns>
        public static string Divide(string memoryInMb, float segmentSize)
        {
            int value;
            return !int.TryParse(memoryInMb, out value)
                ? null
                : Divide(value, segmentSize);
        }

        /// <summary>
        ///     Divide using a predefined segmentation size.
        /// </summary>
        /// <param name="memoryInMb">Memory usage</param>
        /// <param name="segmentSize">segment size in Mb</param>
        /// <returns>memory usage like "Range 10-50Mb"</returns>
        public static string Divide(int memoryInMb, float segmentSize)
        {
            if (memoryInMb < 10)
                return "Range 0-10 Mb";
            if (memoryInMb < 50)
                return "Range 10-50 Mb";
            if (memoryInMb < 100)
                return "Range 50-100 Mb";
            if (memoryInMb > 2000)
                return "> 2Gb";

            var lowerPart = (long) Math.Floor(memoryInMb/segmentSize)*segmentSize;
            return "Range " + lowerPart + "-" + (lowerPart + segmentSize) + " Mb";
        }
    }
}