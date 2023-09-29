using System;
using System.IO;

namespace Coderr.Server.Infrastructure.Messaging.Disk.Queue
{
    /// <summary>
    ///     Wraps an internal buffer.
    /// </summary>
    public class BufferStream : Stream
    {
        private readonly byte[] _buffer;
        private int _count;
        private int _offset;

        public BufferStream(byte[] buffer, int offset, int count)
        {
            _buffer = buffer;
            _offset = offset;
            _count = count;
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = false;
        public override long Length => _count;

        public override long Position
        {
            get => _offset;
            set
            {
                if (value >= _buffer.Length)
                    throw new ArgumentOutOfRangeException("value", "Larger that the internal buffer.");

                _offset = (int)value;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < count)
                throw new ArgumentOutOfRangeException("count", "Larger than the supplied buffer.");
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("count", "Offset+Count is larger than the supplied buffer.");

            var toRead = Math.Min(count, _count);
            if (toRead == 0)
                return 0;

            Buffer.BlockCopy(_buffer, _offset, buffer, offset, toRead);
            _offset += toRead;
            _count -= toRead;
            return toRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}