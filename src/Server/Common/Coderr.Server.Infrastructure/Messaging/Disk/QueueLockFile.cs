using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coderr.Server.Infrastructure.Messaging.Disk
{
    /// <summary>
    ///     Used to make sure that only one process have access to the queue files to avoid synchronization issues.
    /// </summary>
    public class QueueLockFile
    {
        private readonly string _lockFileName;
        private readonly string _queueDirectory;
        private readonly string _queueName;
        private readonly string _releaseRequestFileName;
        private bool _isOwnedByUs;
        private FileStream _lockFile;
        private Timer _timer;

        public QueueLockFile(string queueDirectory, string queueName)
        {
            _queueDirectory = queueDirectory;
            _queueName = queueName;
            _lockFileName = Path.Combine(_queueDirectory, $"{_queueName}.lock");
            _releaseRequestFileName = Path.Combine(_queueDirectory, $"{_queueName}-release.lock");
        }

        /// <summary>
        ///     Callback used to determine if the queues can be shutdown.
        /// </summary>
        /// <remarks>
        ///     The callback must also shutdown the queues before returning.
        /// </remarks>
        /// <value>
        ///     <c>true</c> = the queues have been shutdown; <c>false</c> = cannot shutdown the queues currently.
        /// </value>
        public Func<Task<bool>> CloseQueueRequested { get; set; }

        /// <summary>
        ///     Last exception during the internal processing.
        /// </summary>
        public Exception LastException { get; set; }


        /// <summary>
        ///     Attempt to create the lock file.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Lock file has already been created (by this or another process).</exception>
        public async Task CreateLockFile(TimeSpan timeout)
        {
            if (!File.Exists(_lockFileName))
            {
                LockQueue();
                return;
            }

            // Try to delete it (app crashed)
            try
            {
                File.Delete(_lockFileName);
                File.Delete(_releaseRequestFileName);
                LockQueue();
                return;
            }
            catch (IOException)
            {
                //a
            }

            File.WriteAllText(_releaseRequestFileName, Base64Encode($"Release {_queueName}, please."));
            var isFree = false;
            var exitAt = DateTime.UtcNow.Add(timeout);
            while (exitAt >= DateTime.UtcNow)
            {
                await Task.Delay(100);
                if (!File.Exists(_lockFileName))
                {
                    isFree = true;
                    break;
                }
            }

            if (!isFree) throw new InvalidOperationException("Failed to allocate queue");

            LockQueue();
        }

        public Task DeleteLockFile()
        {
            if (!_isOwnedByUs) throw new InvalidOperationException("File is not owned by us.");

            _lockFile.Close();
            File.Delete(_lockFileName);
            File.Delete(_releaseRequestFileName);
            _isOwnedByUs = false;
            return Task.CompletedTask;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private void LockQueue()
        {
            if (!Directory.Exists(_queueDirectory)) Directory.CreateDirectory(_queueDirectory);

            File.WriteAllText(_lockFileName, $"Locked since {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            _lockFile = new FileStream(_lockFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            _isOwnedByUs = true;
            _timer = new Timer(OnCheckForReleaseRequests);
            _timer.Change(100, 100);
        }

        /// <summary>
        ///     Check if a new process is requesting to get the files.
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>
        ///     <para>
        ///         IIS application pool recycles will always spin up a new process before shutting down the previous process.
        ///         When writing file based queues that means that the new process can't get access to the files during this
        ///         overlapping period.
        ///     </para>
        ///     <para>
        ///         To solve that, the new process will request that the old process releases/closes the queues before it has been
        ///         completely shutdown. This method
        ///         checks for that request file, checks if it's OK to shutdown and then do so.
        ///     </para>
        /// </remarks>
        private void OnCheckForReleaseRequests(object state)
        {
            var expectedString = Base64Encode($"Release {_queueName}, please.");
            if (!File.Exists(_releaseRequestFileName)) return;

            try
            {
                var actualString = File.ReadAllText(_releaseRequestFileName);
                if (actualString != expectedString)
                {
                    File.Delete(_releaseRequestFileName);
                    return;
                }

                if (CloseQueueRequested == null) return;

                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                var canShutDown = CloseQueueRequested().GetAwaiter().GetResult();
                if (!canShutDown)
                {
                    _timer.Change(100, 100);
                    return;
                }

                // Not owned by us if we have been stopped by another thread.
                if (_isOwnedByUs) DeleteLockFile();
            }
            catch (Exception ex)
            {
                LastException = ex;
            }
        }
    }
}