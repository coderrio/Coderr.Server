using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Infrastructure.Messaging.Disk;
using Coderr.Server.Infrastructure.Messaging.Disk.Queue;
using FluentAssertions;
using Xunit;

namespace Coderr.Server.Infrastructure.Tests.Messaging.Disk.Queue
{
    public class DiskFileTests : IDisposable
    {
        private readonly DiskFile<string> _sut;
        private readonly string _fullPath;

        public DiskFileTests()
        {
            var directory = Path.GetTempPath();
            var file = Path.GetTempFileName();
            _fullPath = Path.Combine(directory, file);
            _sut = new DiskFile<string>("MyQueeue", _fullPath);

        }

        [Fact]
        public async Task Should_be_able_to_wait_on_record_directly()
        {
            await _sut.OpenAsync();
            var t2 = _sut.DequeueAsync(TimeSpan.FromSeconds(1)).ContinueWith(MyAction);

            await _sut.EnqueueAsync("HEllo wold");

            await t2;
        }

        [Fact]
        public async Task Should_ignore_invalid_record_in_the_middle()
        {
            await _sut.OpenAsync();
            await _sut.EnqueueAsync("Hello world");
            await _sut.CloseReadAsync();
            using (var stream = File.OpenWrite(_fullPath))
            {
                stream.Seek(0, SeekOrigin.End);
                var buf = new byte[20];
                await stream.WriteAsync(buf, 0, buf.Length);
            }
            await _sut.OpenAsync();
            await _sut.EnqueueAsync("Hello world2");
            var record1 = await _sut.DequeueAsync(TimeSpan.Zero);
            var record2 = await _sut.DequeueAsync(TimeSpan.Zero);



            _sut.NumberOfAvailableRecords.Should().Be(0);
            record1.Entity.Should().Be("Hello world");
            record2.Entity.Should().Be("Hello world2");
        }

        private void MyAction(Task<Record<string>> obj)
        {
            
        }

        public void Dispose()
        {
            _sut?.Dispose();
            File.Delete(_fullPath);
        }
    }
}
