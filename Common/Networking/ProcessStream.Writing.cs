using Common.Utilities;
using System.Buffers.Binary;
using System.Text;

namespace Common.Networking
{
    public partial class ProcessStream
    {
        public void WriteByte(sbyte value)
        {
            BaseStream.WriteByte((byte)value);
        }

        public async Task WriteByteAsync(sbyte value)
        {
            await WriteUnsignedByteAsync((byte)value);
        }

        public void WriteUnsignedByte(byte value)
        {
            BaseStream.WriteByte(value);
        }

        public async Task WriteUnsignedByteAsync(byte value)
        {
            await WriteAsync([value]);
        }

        public void WriteBoolean(bool value)
        {
            BaseStream.WriteByte((byte)(value ? 0x01 : 0x00));
        }

        public async Task WriteBooleanAsync(bool value)
        {
            await WriteByteAsync((sbyte)(value ? 0x01 : 0x00));
        }

        public void WriteUnsignedShort(ushort value)
        {
            Span<byte> span = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(span, value);
            BaseStream.Write(span);
        }

        public async Task WriteUnsignedShortAsync(ushort value)
        {
            using var write = new RentedArray<byte>(sizeof(ushort));
            BinaryPrimitives.WriteUInt16BigEndian(write, value);
            await WriteAsync(write);
        }

        public void WriteShort(short value)
        {
            Span<byte> span = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(span, value);
            BaseStream.Write(span);
        }

        public async Task WriteShortAsync(short value)
        {
            using var write = new RentedArray<byte>(sizeof(short));
            BinaryPrimitives.WriteInt16BigEndian(write, value);
            await WriteAsync(write);
        }

        public void WriteInt(int value)
        {
            Span<byte> span = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(span, value);
            BaseStream.Write(span);
        }

        public async Task WriteIntAsync(int value)
        {
            using var write = new RentedArray<byte>(sizeof(int));
            BinaryPrimitives.WriteInt32BigEndian(write, value);
            await WriteAsync(write);
        }

        public void WriteLong(long value)
        {
            Span<byte> span = stackalloc byte[8];
            BinaryPrimitives.WriteInt64BigEndian(span, value);
            BaseStream.Write(span);
        }

        public async Task WriteLongAsync(long value)
        {
            using var write = new RentedArray<byte>(sizeof(long));
            BinaryPrimitives.WriteInt64BigEndian(write, value);
            await WriteAsync(write);
        }

        public void WriteFloat(float value)
        {
            Span<byte> span = stackalloc byte[4];
            BinaryPrimitives.WriteSingleBigEndian(span, value);
            BaseStream.Write(span);
        }

        public async Task WriteFloatAsync(float value)
        {
            using var write = new RentedArray<byte>(sizeof(float));
            BinaryPrimitives.WriteSingleBigEndian(write, value);
            await WriteAsync(write);
        }

        public void WriteDouble(double value)
        {
            Span<byte> span = stackalloc byte[8];
            BinaryPrimitives.WriteDoubleBigEndian(span, value);
            BaseStream.Write(span);
        }

        public async Task WriteDoubleAsync(double value)
        {
            using var write = new RentedArray<byte>(sizeof(double));
            BinaryPrimitives.WriteDoubleBigEndian(write, value);
            await WriteAsync(write);
        }

        public void WriteString(string value, int maxLength = short.MaxValue)
        {
            System.Diagnostics.Debug.Assert(value.Length <= maxLength);

            using var bytes = new RentedArray<byte>(Encoding.UTF8.GetByteCount(value));
            Encoding.UTF8.GetBytes(value, bytes.Span);
            WriteInt(bytes.Length);
            Write(bytes);
        }

        public void WriteNullableString(string? value, int maxLength = short.MaxValue)
        {
            if (value is null)
                return;

            System.Diagnostics.Debug.Assert(value.Length <= maxLength);

            using var bytes = new RentedArray<byte>(Encoding.UTF8.GetByteCount(value));
            Encoding.UTF8.GetBytes(value, bytes.Span);
            WriteInt(bytes.Length);
            Write(bytes);
        }

        public async Task WriteStringAsync(string value, int maxLength = short.MaxValue)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value.Length > maxLength)
                throw new ArgumentException($"string ({value.Length}) exceeded maximum length ({maxLength})", nameof(value));

            using var bytes = new RentedArray<byte>(Encoding.UTF8.GetByteCount(value));
            Encoding.UTF8.GetBytes(value, bytes.Span);
            await WriteIntAsync(bytes.Length);
            await WriteAsync(bytes);
        }

        public void WriteLongArray(long[] values)
        {
            Span<byte> buffer = stackalloc byte[8];
            for (int i = 0; i < values.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer, values[i]);
                BaseStream.Write(buffer);
            }
        }

        public async Task WriteLongArrayAsync(long[] values)
        {
            foreach (var value in values)
                await WriteLongAsync(value);
        }

        public async Task WriteLongArrayAsync(ulong[] values)
        {
            foreach (var value in values)
                await WriteLongAsync((long)value);
        }

        public void WriteByteArray(byte[] values)
        {
            WriteInt(values.Length);
            BaseStream.Write(values);
        }
    }
}
