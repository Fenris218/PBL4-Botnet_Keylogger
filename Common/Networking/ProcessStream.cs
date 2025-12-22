namespace Common.Networking
{
    //wrapper Stream để thêm các tính năng như thread-safe và quản lý bộ nhớ.
    public partial class ProcessStream : Stream
    {
        private bool disposed;

        public Stream BaseStream { get; set; } //streamm thực tế

        // *** SemaphoreSlim: Đảm bảo thread-safety ***
        // Chỉ cho phép 1 thread đọc/ghi stream tại một thời điểm
        // Điều này quan trọng vì nhiều packet handler có thể cố gắng truy cập stream đồng thời
        // SemaphoreSlim(1, 1) = binary semaphore (giống mutex nhưng nhẹ hơn)
        public SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1); //để đảm bảo thread-safe khi đọc/ghi, tránh nhiều thread truy cập cùng lúc

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public ProcessStream()
        {

            this.BaseStream = new MemoryStream(); //khởi tạo với MemoryStream mặc định
        }

        public ProcessStream(Stream stream)
        {
            this.BaseStream = stream;//khởi tạo với stream được cung cấp
        }

        public ProcessStream(byte[] data)
        {
            this.BaseStream = new MemoryStream(data);//khởi tạo với mảng byte
        }

        public override void Flush() => this.BaseStream.Flush();//đảm bảo dữ liệu được ghi vào stream

        public override int Read(byte[] buffer, int offset, int count) => this.BaseStream.Read(buffer, offset, count);

        //đọc dữ liệu 1 phần rồi ghi vào buffer
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            try
            {
                var read = await BaseStream.ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

                return read;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //đọc toàn bộ dữ liệu vào buffer
        public virtual async Task<int> ReadAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            try
            {
                var read = await this.BaseStream.ReadAsync(buffer, cancellationToken);

                return read;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await BaseStream.WriteAsync(buffer.AsMemory(offset, count), cancellationToken);
        }

        public virtual async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            await this.BaseStream.WriteAsync(buffer, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.BaseStream.Write(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) => this.BaseStream.Seek(offset, origin);

        public override void SetLength(long value) => this.BaseStream.SetLength(value);

        // chuyển toàn bộ nội dung stream thành mảng byte
        public byte[] ToArray()
        {
            this.Position = 0;
            var buffer = new byte[this.Length];
            for (var totalBytesCopied = 0; totalBytesCopied < this.Length;)
                totalBytesCopied += this.Read(buffer, totalBytesCopied, Convert.ToInt32(this.Length) - totalBytesCopied);
            return buffer;
        }

        //giải phóng umanaged resources
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                this.BaseStream.Dispose();
                this.Lock.Dispose();
            }

            this.disposed = true;
        }
    }
}
