using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Coderr.Server.Infrastructure.Messaging.Disk.Queue
{
    /// <summary>
    ///     Reads from a disk file using on own internal cache when doing so.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class ReadState<TEntity>
    {
        private const int HeaderLength = 8;
        private const int SizeHeaderLength = 4;
        private const int RecordSeparatorLength = 4;
        private readonly IContentSerializer _contentSerializer;
        private readonly byte[] _recordSeparator;
        private readonly Stream _sourceStream;
        private byte[] _buffer;
        private int _bufferBytesLeft;
        private int _bufferOffset;

        public ReadState(Stream sourceStream, int bufferSize, byte[] recordSeparator,
            IContentSerializer contentSerializer)
        {
            _sourceStream = sourceStream;
            _recordSeparator = recordSeparator;
            _contentSerializer = contentSerializer;
            _buffer = new byte[bufferSize];
        }

        public ReadState(Stream readStream, byte[] buffer)
        {
            _sourceStream = readStream;
            _buffer = buffer;
            _bufferOffset = 0;
            _bufferBytesLeft = 0;
        }

        public async Task AdjustForPositionBug()
        {
            var beforeOffset = _sourceStream.Position;
            var read = await _sourceStream.ReadAsync(_buffer, 0, HeaderLength);
            if (read == 0) return;

            if (read != HeaderLength) Debugger.Break();

            if (_buffer[0] == 1 && _buffer[1] == 3 && _buffer[2] == 3 && _buffer[3] == 7)
            {
                _sourceStream.Position = beforeOffset;
                return;
            }

            _sourceStream.Position = beforeOffset - HeaderLength;
            read = await _sourceStream.ReadAsync(_buffer, 0, 8);
            if (read == 0) return;

            if (_buffer[0] == 1 && _buffer[1] == 3 && _buffer[2] == 3 && _buffer[3] == 7)
            {
                _sourceStream.Position = beforeOffset - HeaderLength;
                return;
            }

            Debugger.Break();

            // All failed, let's sweep.
            _sourceStream.Position = beforeOffset - _buffer.Length / 2;
            var startPosition = _sourceStream.Position;
            await _sourceStream.ReadAsync(_buffer, 0, _buffer.Length);

            var index = 0;
            while (index < _buffer.Length)
            {
                if (_buffer[index + 0] != 1 || _buffer[index + 1] != 3 || _buffer[index + 2] != 3 ||
                    _buffer[index + 3] != 7)
                {
                    index++;
                    continue;
                }

                _sourceStream.Position = startPosition + index;
                return;
            }

            Debugger.Break();
        }

        /// <summary>
        ///     Go through entire file looking for records. Resets state once done.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountRecords()
        {
            var startPos = _sourceStream.Position;
            var recordCount = 0;
            while (true)
            {
                // find record 
                while (true)
                {
                    var gotEnough = await EnsureEnoughBytes(8);
                    if (!gotEnough)
                    {
                        // reset state before returning
                        _sourceStream.Position = startPos;
                        _bufferOffset = 0;
                        _bufferBytesLeft = 0;
                        return recordCount;
                    }


                    var isValidEntry = VerifyRecordSeparator(_buffer, _bufferOffset);
                    if (!isValidEntry)
                    {
                        // move forward one so that we can loop until we find a correct record.
                        _bufferOffset++;
                        _bufferBytesLeft--;
                        continue;
                    }

                    break;
                }

                var recordSize = BitConverter.ToInt32(_buffer, _bufferOffset + RecordSeparatorLength);

                _bufferOffset += HeaderLength;
                _bufferBytesLeft -= HeaderLength;

                var gotBodyBytes = await EnsureEnoughBytes(recordSize);
                if (!gotBodyBytes) throw new InvalidOperationException("Expected to find a complete body. Failed.");

                _bufferOffset += recordSize;
                _bufferBytesLeft -= recordSize;
                recordCount++;
            }
        }

        public bool MustFlushWrite(long lastFlushedWriteIndex)
        {
            return _sourceStream.Position + HeaderLength >= lastFlushedWriteIndex;
        }

        public async Task<Record<TEntity>> ReadRecord()
        {
            var corruptRecordOffset = -1;

            while (true)
            {
                var gotEnough = await EnsureEnoughBytes(HeaderLength);
                if (!gotEnough)
                    return null;

                var isValidEntry = VerifyRecordSeparator(_buffer, _bufferOffset);
                if (!isValidEntry)
                {
                    corruptRecordOffset = (int)_sourceStream.Position - _bufferBytesLeft;
                    // move forward one so that we can loop until we find a correct record.
                    // 
                    _bufferOffset++;
                    _bufferBytesLeft--;
                    continue;
                }

                // yay got a valid record.
                if (corruptRecordOffset > -1)
                {
                    //TODO: Report the record for analysis
                }

                break;
            }

            var recordSize = BitConverter.ToInt32(_buffer, _bufferOffset + RecordSeparatorLength);
            var recordStartPosition = _sourceStream.Position - _bufferBytesLeft;

            _bufferOffset += HeaderLength;
            _bufferBytesLeft -= HeaderLength;

            var gotBodyBytes = await EnsureEnoughBytes(recordSize);
            if (!gotBodyBytes) throw new InvalidOperationException("Expected to find a complete body. Failed.");


            var stream = new BufferStream(_buffer, _bufferOffset, recordSize);
            var entity = (TEntity)await _contentSerializer.DeserializeAsync(stream, recordSize, typeof(TEntity));
            _bufferOffset += recordSize;
            _bufferBytesLeft -= recordSize;
            return new Record<TEntity>(entity, (int)recordStartPosition);
        }

        private async Task<bool> EnsureEnoughBytes(int requestedAmountOfBytes)
        {
            if (_bufferBytesLeft > requestedAmountOfBytes)
                return true;

            var sourceOffset = _bufferOffset;

            if (_bufferOffset + requestedAmountOfBytes > _buffer.Length)
            {
                if (requestedAmountOfBytes > _buffer.Length)
                {
                    var newBuffer = new byte[requestedAmountOfBytes * 2];
                    Buffer.BlockCopy(_buffer, _bufferOffset, newBuffer, 0, _bufferBytesLeft);
                    _buffer = newBuffer;
                }
                else
                {
                    Buffer.BlockCopy(_buffer, _bufferOffset, _buffer, 0, _bufferBytesLeft);
                }

                _bufferOffset = _bufferBytesLeft;
                sourceOffset = 0;
            }

            var bytesLeftToRead = requestedAmountOfBytes - _bufferBytesLeft;
            while (bytesLeftToRead > 0)
            {
                var bytesRead = await _sourceStream.ReadAsync(_buffer, _bufferOffset, _buffer.Length - _bufferOffset);
                if (bytesRead == 0)
                    return false;

                bytesLeftToRead -= bytesRead;
                _bufferOffset += bytesRead;
                _bufferBytesLeft += bytesRead;
            }

            // Since we want to read from that position.
            _bufferOffset = sourceOffset;
            return true;
        }

        private bool VerifyRecordSeparator(byte[] buffer, int offset)
        {
            for (var i = 0; i < _recordSeparator.Length; i++)
            {
                if (_recordSeparator[i] != buffer[offset + i])
                    return false;
            }

            return true;
        }
    }
}