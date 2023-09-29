using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using log4net;

namespace Coderr.Server.Infrastructure.Messaging.Disk.Queue
{
    public class DiskQueue<TEntity> : IDisposable
    {
        private readonly DequeueTracking _dequeueTracking;
        private readonly LinkedList<DiskFile<TEntity>> _files = new LinkedList<DiskFile<TEntity>>();
        private readonly string _queueDirectory;
        private readonly string _queueName;
        private bool _isShuttingDown;
        private DateTime _lastQueueFileSizeCheck;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DiskQueue<>));

        public DiskQueue(string queueDirectory, string queueName)
        {
            _queueDirectory = queueDirectory ?? throw new ArgumentNullException(nameof(queueDirectory));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));

            // Create a sub directory to make sure that there are no queue collisions
            _queueDirectory = Path.Combine(_queueDirectory, _queueName);

            _dequeueTracking = new DequeueTracking(queueDirectory, queueName);
        }

        /// <summary>
        ///     Flush to disk after each IO operation.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Hurts performance but increases reliability. Turn on if you want to make sure that everything exists if the
        ///         application crashes etc. Turn of if you do batch writes (flush yourself after the batch write).
        ///     </para>
        /// </remarks>
        public bool AutoFlush { get; set; }

        /// <summary>
        ///     Number of items
        /// </summary>
        public int Count
        {
            get { return _files.Sum(x => x.NumberOfAvailableRecords); }
        }

        /// <summary>
        ///     Create a new file once this limit has been reached.
        /// </summary>
        /// <value>
        ///     Default is 100MB.
        /// </value>
        public int MaximumFileSize { get; set; } = 100000000;

        /// <summary>
        ///     Reduce number of file size checks to improve IO performance.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Checking file size to determine if a new file should be created hurts performance in high IO system. Activating
        ///         this settings will let queue files grow a bit more before creating a new file, but will improve the overall IO
        ///         performance.
        ///     </para>
        /// </remarks>
        public bool ReduceQueueFileSizeChecks { get; set; }

        /// <summary>
        ///     Serializer used for the contents that you enqueue.
        /// </summary>
        /// <value>
        ///     Built in serializer is <c>System.Text.Json</c>
        /// </value>
        public IContentSerializer Serializer { get; set; } = new JsonNetDiskSerializer();

        public void Dispose()
        {
            _dequeueTracking.Dispose();
        }

        /// <summary>
        ///     Flush everything and close all queue files.
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            _isShuttingDown = true;
            await FlushAsync();
            foreach (var file in _files)
            {
                await file.CloseWriteAsync();
                await file.CloseReadAsync();
            }

            _files.Clear();

            await _dequeueTracking.CloseAsync();
        }

        /// <summary>
        ///     Dequeue an entry.
        /// </summary>
        /// <returns>Dequeued entry if any; otherwise <c>null</c>.</returns>
        public async Task<TEntity> DequeueAsync()
        {
            if (_isShuttingDown) return default;

            if (_files.Count == 0)
                throw new InvalidOperationException("Open queue first.");

            while (true)
            {
                var file = _files.First.Value;
                var record = await file.DequeueAsync(TimeSpan.FromSeconds(10));
                if (_isShuttingDown)
                    // since we don't update the position,
                    // we can safely return null;
                    return default;

                if (record != null)
                {
                    _logger.Debug($"{_queueName} is dequeing {((Message)(object)record.Entity).Body}");

                    // Just read the last record.
                    if (file.NumberOfAvailableRecords == 0 && _files.Count > 1)
                    {
                        await MoveToNextQueueFile();
                    }
                    else
                    {
                        await _dequeueTracking.WriteReadRecord(record.RecordOffset);
                        if (AutoFlush)
                            await _dequeueTracking.FlushAsync();
                    }

                    return record.Entity;
                }

                // Only file left means that we are still writing to it.
                if (_files.Count == 1) return default;

                // We are done with this file.
                await MoveToNextQueueFile();
            }
        }

        /// <summary>
        ///     Enqueue a new entry.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task EnqueueAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (_files.Count == 0)
                throw new InvalidOperationException("Open queue first.");

            var file = _files.Last.Value;
            await file.EnqueueAsync(entity);

            var checkFileSize = !ReduceQueueFileSizeChecks;
            if (ReduceQueueFileSizeChecks && DateTime.UtcNow.Subtract(_lastQueueFileSizeCheck).TotalMilliseconds > 500)
            {
                checkFileSize = true;
                _lastQueueFileSizeCheck = DateTime.UtcNow;
            }

            _logger.Debug($"{_queueName} is storing {((Message)(object)entity).Body}");
            if (checkFileSize && file.CurrentFileSize > MaximumFileSize)
            {
                await file.FlushWriteAsync();
                await file.CloseWriteAsync();
                file = await OpenNewFile();
                _files.AddLast(file);
            }
            else if (AutoFlush)
            {
                await file.FlushWriteAsync();
            }
        }

        /// <summary>
        ///     Flush everything from disk.
        /// </summary>
        /// <returns></returns>
        public async Task FlushAsync()
        {
            await _files.Last.Value.FlushWriteAsync();
            await _dequeueTracking.FlushAsync();
        }

        /// <summary>
        ///     Open first queue file.
        /// </summary>
        /// <returns></returns>
        public async Task OpenAsync(TimeSpan timeout)
        {
            if (!Directory.Exists(_queueDirectory)) Directory.CreateDirectory(_queueDirectory);

            _logger.Debug($"[{_queueName}] Opening queue..");

            await _dequeueTracking.OpenAsync(timeout);

            var lastRecordPosition = _dequeueTracking.LastReadRecord;
            _lastQueueFileSizeCheck = DateTime.UtcNow;

            _logger.Debug($"[{_queueName}] Loading files..");
            await LoadFiles(lastRecordPosition);
            if (_files.Count == 0)
            {
                var file = await OpenNewFile();
                _files.AddLast(file);
            }
        }

        private async Task LoadFiles(int lastReadRecordPosition)
        {
            var files = Directory
                .GetFiles(_queueDirectory, _queueName + "_*.data")
                .OrderBy(x => x)
                .ToList();

            var lastIndex = files.Count - 1;
            for (var index = 0; index < files.Count; index++)
            {
                var file = files[index];

                var queueFile = new DiskFile<TEntity>(_queueName, file);
                if (lastReadRecordPosition >= 0)
                {
                    if (queueFile.CurrentFileSize < lastReadRecordPosition)
                    {
                        _logger.Error(
                            $"Last record {lastReadRecordPosition} is larger than the file size {queueFile.CurrentFileSize}, we'll ignore this file: " +
                            file);
                        await queueFile.CloseWriteAsync();
                        await queueFile.CloseReadAsync();
                        await queueFile.Delete();
                        continue;
                    }

                    await queueFile.OpenAsync(lastReadRecordPosition, index == lastIndex);
                    lastReadRecordPosition = -1;
                }
                else
                {
                    await queueFile.OpenAsync(0, index == lastIndex);
                }

                _files.AddLast(queueFile);
            }
        }

        private async Task MoveToNextQueueFile()
        {
            _logger.Info("Moving to next file");

            await _dequeueTracking.ResetAsync();
            var file = _files.First.Value;
            await file.CloseReadAsync();
            await file.Delete();
            _files.RemoveFirst();
        }

        private async Task<DiskFile<TEntity>> OpenNewFile()
        {
            var name = Path.Combine(_queueDirectory,
                $"{_queueName}_{DateTime.UtcNow:yyyyMMdHHmmssfff}.data");
            var file = new DiskFile<TEntity>(_queueName, name) {Serializer = Serializer};
            await file.OpenAsync(0, true);
            return file;
        }
    }
}