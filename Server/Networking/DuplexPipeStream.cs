using System.Buffers;
using System.IO.Pipelines;

namespace Server.Networking
{
    public sealed class DuplexPipeStream : Stream
    {
        private readonly PipeReader _input;
        private readonly PipeWriter _output;
        private readonly bool _throwOnCancelled;
        private volatile bool _cancelCalled;

        public DuplexPipeStream(IDuplexPipe pipe, bool throwOnCancelled = false)
        {
            _input = pipe.Input;
            _output = pipe.Output;
            _throwOnCancelled = throwOnCancelled;
        }

        public void CancelPendingRead()
        {
            _cancelCalled = true;
            _input.CancelPendingRead();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsyncInternal(new Memory<byte>(buffer, offset, count), default).Result;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
        {
            return ReadAsyncInternal(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
        {
            return ReadAsyncInternal(destination, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

        public async override Task WriteAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer != null)
            {
                _output.Write(new ReadOnlySpan<byte>(buffer, offset, count));
            }

            await _output.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public async override ValueTask WriteAsync(ReadOnlyMemory<byte> source,
            CancellationToken cancellationToken = default)
        {
            _output.Write(source.Span);
            await _output.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public override void Flush()
        {
            FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return WriteAsync(null, 0, 0, cancellationToken);
        }

        private async ValueTask<int> ReadAsyncInternal(Memory<byte> destination, CancellationToken cancellationToken)
        {
            while (true)
            {
                var result = await _input.ReadAsync(cancellationToken).ConfigureAwait(false);
                var readableBuffer = result.Buffer;
                try
                {
                    if (_throwOnCancelled && result.IsCanceled && _cancelCalled)
                    {
                        _cancelCalled = false;
                        throw new OperationCanceledException();
                    }

                    if (!readableBuffer.IsEmpty)
                    {
                        var count = (int)Math.Min(readableBuffer.Length, destination.Length);
                        readableBuffer = readableBuffer.Slice(0, count);
                        readableBuffer.CopyTo(destination.Span);
                        return count;
                    }

                    if (result.IsCompleted)
                    {
                        return 0;
                    }
                }
                finally
                {
                    _input.AdvanceTo(readableBuffer.End, readableBuffer.End);
                }
            }
        }
    }
}
