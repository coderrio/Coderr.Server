using System;

namespace codeRR.Server.App.Modules.Messaging.Templating
{
    /// <summary>
    ///     Look at the size of that thing.
    /// </summary>
    public class Size
    {
        /// <summary>
        ///     Creates a new instance of <see cref="Size" />.
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <exception cref="ArgumentOutOfRangeException">something is not 1 or larger</exception>
        public Size(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            if (height <= 0) throw new ArgumentOutOfRangeException("height");
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Height in some more arbitrary unit
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     Weight in some arbitrary unit
        /// </summary>
        public int Width { get; set; }
    }
}