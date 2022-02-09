using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace Coderr.Server.Infrastructure.Messaging.Disk.Queue
{
    /// <summary>
    ///     Represents a single file in a queue.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The most efficient way to handle files is to use multiple ones. By doing so we can always do sequential reads
    ///         and writes. Queue can grow without reallocation and we'll delete files once all entries have been read.
    ///     </para>
    /// </remarks>
    public class DiskFile<TEntity> : IDisposable
    {
        private static readonly byte[] RecordSeparator = {1, 3, 3, 7};
        private readonly int _bufferSize = 1000000;
        private readonly SemaphoreSlim _entriesAvailableLock = new SemaphoreSlim(0, int.MaxValue);
        private readonly string _fullPath;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DiskFile<TEntity>));
        private readonly string _queueName;
        private readonly MemoryStream _serializerStream = new MemoryStream();
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);
        private long _flushedWritePosition;
        private bool _isShuttingDown;
        private int _numberOfRecords;
        private ReadState<TEntity> _readState;
        private FileStream _readStream;
        private long _writeFilePosition;
        private FileStream _writeStream;

        public DiskFile(string queueName, string fullPath)
        {
            _queueName = queueName;
            _fullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
        }


        public DiskFile(string queueName, string fullPath, int bufferSize)
        {
            _queueName = queueName;
            _fullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
            _bufferSize = bufferSize;
        }

        /// <summary>
        ///     Number of bytes allocated by this file.
        /// </summary>
        public long CurrentFileSize
        {
            get
            {
                if (_writeStream != null) return _writeStream.Length;
                var info = new FileInfo(_fullPath);
                return info.Length;
            }
        }

        /// <summary>
        ///     Records that have not yet been read.
        /// </summary>
        public int NumberOfAvailableRecords => _numberOfRecords;

        /// <summary>
        ///     Used to serialize queue entries.
        /// </summary>
        public IContentSerializer Serializer { get; set; } = new JsonNetDiskSerializer();


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _readStream?.Dispose();
            _writeStream?.Dispose();
            _entriesAvailableLock.Dispose();
            _writeLock.Dispose();
        }

        /// <summary>
        ///     Close file
        /// </summary>
        /// <returns></returns>
        public Task CloseReadAsync()
        {
            _isShuttingDown = true;
            _entriesAvailableLock.Release();

            if (_readState == null)
                return Task.CompletedTask;

            _readStream.Close();
            _readState = null;
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Close file
        /// </summary>
        /// <returns></returns>
        public async Task CloseWriteAsync()
        {
            _entriesAvailableLock.Release();

            if (_writeStream == null)
                return;

            await _writeStream.FlushAsync();
            _writeStream.Close();
            _writeStream = null;
        }


        public async Task Delete()
        {
            await CloseWriteAsync();
            await CloseReadAsync();
            File.Delete(_fullPath);
        }

        /// <summary>
        ///     Queue an entry
        /// </summary>
        /// <returns>entry if found; otherwise <c>null</c>.</returns>
        public async Task<Record<TEntity>> DequeueAsync(TimeSpan timeout)
        {
            if (_writeStream != null && !await _entriesAvailableLock.WaitAsync(timeout)) return null;


            if (_isShuttingDown) return null;

            if (_writeStream != null && _readState.MustFlushWrite(_flushedWritePosition)) await FlushWriteAsync();

            var record = await _readState.ReadRecord();
            if (record != null)
            {
                Interlocked.Decrement(ref _numberOfRecords);
                Debug($"[Dequeue] We now have {_numberOfRecords} records left.");
            }

            return record;
        }


        /// <summary>
        ///     Enqueue a new item.
        /// </summary>
        /// <param name="item">Item to enqueue</param>
        /// <returns></returns>
        public async Task<int> EnqueueAsync(TEntity item)
        {
            if (_writeStream == null) throw new InvalidOperationException($"File '{_fullPath}' is not open for write.");

            Debug("[Enqueue] acquiring lock");
            await _writeLock.WaitAsync();
            int startPos;
            try
            {
                Debug($"[Enqueue] at position {_writeStream.Position}, record number {_numberOfRecords + 1}.");
                _serializerStream.SetLength(0);
                await Serializer.SerializeAsync(_serializerStream, item);
                _serializerStream.Flush();
                _serializerStream.Position = 0;

                startPos = (int)_writeFilePosition;
                var buf = BitConverter.GetBytes((int)_serializerStream.Length);
                await _writeStream.WriteAsync(RecordSeparator, 0, RecordSeparator.Length);
                await _writeStream.WriteAsync(buf, 0, buf.Length);
                buf = _serializerStream.GetBuffer();
                await _writeStream.WriteAsync(buf, 0, (int)_serializerStream.Length);
                _writeFilePosition += 8 + _serializerStream.Length;

                Interlocked.Increment(ref _numberOfRecords);
                _entriesAvailableLock.Release(1);
            }
            finally
            {
                _writeLock.Release();
            }

            return startPos;
        }

        public async Task FlushWriteAsync()
        {
            await _writeLock.WaitAsync();
            try
            {
                _flushedWritePosition = _writeFilePosition;
                await _writeStream.FlushAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>
        ///     Open the file
        /// </summary>
        /// <param name="startOffset">
        ///     Position to start ready entries from (if we have previously read from this file, we want to
        ///     resume at the given position).
        /// </param>
        /// <param name="openWrite">We are only writing to the last file, is this it?</param>
        /// <returns></returns>
        public async Task OpenAsync(int startOffset = 0, bool openWrite = false)
        {
            _isShuttingDown = false;

            // We need this loop for web applications
            // where the previous instance haven't shutdown completely before the new one
            // is started.
            if (openWrite) await OpenWriteStream();

            await OpenReadStream(startOffset);
        }

        private void Debug(string msg)
        {
            if (_queueName != "ErrorReports")
                return;

            _logger.Debug($"<{_queueName}> {msg}");
        }

        private async Task OpenReadStream(int startOffset)
        {
            _readStream = new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, _bufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            if (startOffset > _readStream.Length)
                throw new InvalidOperationException("Offset cannot be larger than the file.");

            _readState = new ReadState<TEntity>(_readStream, _bufferSize, RecordSeparator, Serializer);

            // Read the last read record.
            if (startOffset > 0)
            {
                _readStream.Position = startOffset;
                await _readState.AdjustForPositionBug();
                await DequeueAsync(TimeSpan.FromSeconds(0));
            }

            Debug("Counting records...");
            _numberOfRecords = await _readState.CountRecords();
            Debug($"... {_numberOfRecords} records ..");
            if (_numberOfRecords > 0) _entriesAvailableLock.Release(_numberOfRecords);
        }

        private async Task OpenWriteStream()
        {
            var attemptsLeft = 10;
            while (attemptsLeft-- > 0)
            {
                try
                {
                    _writeStream = new FileStream(_fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite,
                        _bufferSize,
                        FileOptions.Asynchronous | FileOptions.SequentialScan);
                    break;
                }
                catch
                {
                    await Task.Delay(1000);
                    if (attemptsLeft == 0)
                        throw;
                }
            }
        }
    }
}