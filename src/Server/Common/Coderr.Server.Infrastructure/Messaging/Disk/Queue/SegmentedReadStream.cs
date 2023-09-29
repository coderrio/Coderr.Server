using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace Coderr.Server.Infrastructure.Messaging.Disk.Queue
{
    /// <summary>
    ///     A stream with a shorter length than the actual stream.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Some serializer can't use a segment from a stream. Therefore we need to fake the length of a stream to make
    ///         sure that the serializer doesn't try to parse too much.
    ///     </para>
    /// </remarks>
    public class SegmentedReadStream : Stream
    {
        private readonly Stream _inner;
        private int _bytesLeftToRead;
        private ILog _logger = LogManager.GetLogger(typeof(SegmentedReadStream));
        private int _segmentSize;

        public SegmentedReadStream(Stream inner, int segmentSize)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _segmentSize = segmentSize;
        }

        /// <inheritdoc />
        public override bool CanRead => _inner.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => _inner.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => _inner.CanWrite;

        /// <inheritdoc />
        public override long Length => _segmentSize;

        /// <inheritdoc />
        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        /// <inheritdoc />
        public override void Flush()
        {
            _inner.Flush();
        }

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _inner.FlushAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_bytesLeftToRead < count) count = _bytesLeftToRead;

            if (count == 0)
                return 0;

            var read = _inner.Read(buffer, offset, count);
            _bytesLeftToRead -= read;
            return read;
        }

        /// <inheritdoc />
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (offset + count > _segmentSize)
                count = _segmentSize - offset;

            return _inner.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">
        ///     Not supported, control the size through <see cref="SetSegmentSize" />
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Set the fake limit.
        /// </summary>
        /// <param name="recordSize"></param>
        public void SetSegmentSize(int recordSize)
        {
            _segmentSize = recordSize;
            _bytesLeftToRead = _segmentSize;
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}