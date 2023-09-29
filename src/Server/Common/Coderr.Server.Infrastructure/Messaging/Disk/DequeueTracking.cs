using System;
using System.IO;
using System.Threading.Tasks;
using log4net;

namespace Coderr.Server.Infrastructure.Messaging.Disk
{
    /// <summary>
    ///     Keeps track of which entries we have dequeued in the first queue file.
    /// </summary>
    public class DequeueTracking : IDisposable
    {
        private readonly byte[] _buffer = new byte[65535];
        private readonly string _fullPath;
        private readonly string _queueName;
        private FileStream _fileStream;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DequeueTracking));

        public DequeueTracking(string queueDirectory, string queueName)
        {
            if (queueDirectory == null) throw new ArgumentNullException(nameof(queueDirectory));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            _fullPath = Path.Combine(queueDirectory, queueName + ".meta");
        }

        /// <summary>
        ///     Last record that we dequeued (i.e. we should be dequeuing the entry after the specified one).
        /// </summary>
        public int LastReadRecord { get; private set; } = -1;

        public void Dispose()
        {
            _fileStream?.Dispose();
        }

        public async Task CloseAsync()
        {
            await _fileStream.FlushAsync();
            _fileStream.Close();
        }

        public async Task FlushAsync()
        {
            await _fileStream.FlushAsync();
        }

        public async Task OpenAsync(TimeSpan timeout)
        {
            _logger.Debug($"[{_queueName}] Opening meta file...");

            // We need this loop for web applications
            // where the previous instance haven't shutdown completely before the new one
            // is started.
            var attemptsLeft = timeout.TotalSeconds;
            while (attemptsLeft-- > 0)
            {
                try
                {
                    _fileStream = new FileStream(_fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read,
                        8192,
                        FileOptions.SequentialScan | FileOptions.Asynchronous);
                    _logger.Debug($"[{_queueName}] Opened file successfully...");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Debug($"[{_queueName}] Failed, will try again in 1 second ({ex.Message})...");
                    await Task.Delay(1000);
                    if (attemptsLeft == 0)
                        //_logger.Fatal($"[{_queueName}] Failed to open metafile ({ex.Message})...");
                        throw new InvalidOperationException("Failed to open meta file 10 times.", ex);
                }
            }

            _logger.Debug($"[{_queueName}] Reading records...");
            while (true)
            {
                var typeOfRecord = _fileStream.ReadByte();
                if (typeOfRecord == -1)
                    break;

                if (typeOfRecord == 1)
                {
                    var readBytes = await _fileStream.ReadAsync(_buffer, 0, 4);
                    if (readBytes == 0)
                        break;

                    if (readBytes != 4)
                        throw new InvalidOperationException("Corrupt meta file, uh oh.");

                    LastReadRecord = BitConverter.ToInt32(_buffer, 0);
                }
            }

            _logger.Debug($"[{_queueName}] last read record: " + LastReadRecord);
        }

        /// <summary>
        ///     Reset file, since we are starting to read from a new queue file.
        /// </summary>
        /// <returns></returns>
        public async Task ResetAsync()
        {
            _fileStream.SetLength(0);
            LastReadRecord = -1;
            await _fileStream.FlushAsync();
        }

        /// <summary>
        ///     Store where the next message to read is located.
        /// </summary>
        /// <param name="filePosition">Should be the position to the record separator and not the start of the message.</param>
        /// <returns></returns>
        public async Task WriteReadRecord(int filePosition)
        {
            //if (_fileName != fileName)
            //{
            //    _buffer[0] = (byte)Encoding.UTF8.GetBytes(fileName, 0, fileName.Length, _buffer, 1);
            //    _fileName = fileName;
            //    var count = 1 + _buffer[0];
            //    await _fileStream.WriteAsync(_buffer, 0, count);
            //}
            _fileStream.WriteByte(1);
            var buf = BitConverter.GetBytes(filePosition);
            await _fileStream.WriteAsync(buf, 0, buf.Length);
        }
    }

    public enum MetaFileRecordType
    {
        OpenFile,
        Dequeue
    }
}