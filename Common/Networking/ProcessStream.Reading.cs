using Common.Utilities;
using System.Buffers.Binary;
using System.Text;

namespace Common.Networking
{
    public partial class ProcessStream
    {

        public sbyte ReadSignedByte() => (sbyte)this.ReadUnsignedByte();//chuyển tù byte sang sbyte (-128 đến 127)

        public async Task<sbyte> ReadByteAsync() => (sbyte)await this.ReadUnsignedByteAsync();

        public byte ReadUnsignedByte()
        {
            Span<byte> buffer = stackalloc byte[1];
            BaseStream.ReadExactly(buffer);
            return buffer[0];
        }

        public async Task<byte> ReadUnsignedByteAsync()
        {
            var buffer = new byte[1];
            await this.ReadAsync(buffer);
            return buffer[0];
        }

        public bool ReadBoolean()
        {
            return ReadUnsignedByte() == 0x01;
        }

        public async Task<bool> ReadBooleanAsync()
        {
            var value = (int)await this.ReadByteAsync();
            return value switch
            {
                0x00 => false,
                0x01 => true,
                _ => throw new ArgumentOutOfRangeException("Byte returned by stream is out of range (0x00 or 0x01)",
                    nameof(BaseStream))
            };
        }

        //ushort là unsigned 16-bit integer (0 → 65535).
        public ushort ReadUnsignedShort()
        {
            Span<byte> buffer = stackalloc byte[2];
            this.ReadExactly(buffer);
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }

        public async Task<ushort> ReadUnsignedShortAsync()
        {
            var buffer = new byte[2];
            await this.ReadAsync(buffer);
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }

        //short (signed 16-bit) integer (-32,768 → 32,767).
        public short ReadShort()
        {
            Span<byte> buffer = stackalloc byte[2];
            this.ReadExactly(buffer);
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }

        public async Task<short> ReadShortAsync()
        {
            using var buffer = new RentedArray<byte>(sizeof(short));
            await this.ReadExactlyAsync(buffer);
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }

        //int (signed 32-bit) integer (-2,147,483,648 → 2,147,483,647).
        public int ReadInt()
        {
            Span<byte> buffer = stackalloc byte[4];
            this.ReadExactly(buffer);
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }

        public async Task<int> ReadIntAsync()
        {
            using var buffer = new RentedArray<byte>(sizeof(int));
            await this.ReadExactlyAsync(buffer);
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }

        //long (Int64, signed).
        public long ReadLong()
        {
            Span<byte> buffer = stackalloc byte[8];
            this.ReadExactly(buffer);
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }

        public async Task<long> ReadLongAsync()
        {
            using var buffer = new RentedArray<byte>(sizeof(long));
            await this.ReadExactlyAsync(buffer);
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }

        public ulong ReadUnsignedLong()
        {
            Span<byte> buffer = stackalloc byte[8];
            this.ReadExactly(buffer);
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }

        public async Task<ulong> ReadUnsignedLongAsync()
        {
            using var buffer = new RentedArray<byte>(sizeof(ulong));
            await this.ReadExactlyAsync(buffer);
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }

        public float ReadFloat()
        {
            Span<byte> buffer = stackalloc byte[4];
            this.ReadExactly(buffer);
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }

        public async Task<float> ReadFloatAsync()
        {
            using var buffer = new RentedArray<byte>(sizeof(float));
            await this.ReadExactlyAsync(buffer);
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }

        public double ReadDouble()
        {
            Span<byte> buffer = stackalloc byte[8];
            this.ReadExactly(buffer);
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }

        public async Task<double> ReadDoubleAsync()
        {
            using var buffer = new RentedArray<byte>(sizeof(double));
            await this.ReadExactlyAsync(buffer);
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }

        public string ReadString(int maxLength = 32767)
        {
            var length = ReadInt();
            var buffer = new byte[length];
            this.ReadExactly(buffer);

            var value = Encoding.UTF8.GetString(buffer);
            if (maxLength > 0 && value.Length > maxLength)
            {
                throw new ArgumentException($"string ({value.Length}) exceeded maximum length ({maxLength})", nameof(value));
            }
            return value;
        }

        public async Task<string> ReadStringAsync(int maxLength = 32767)
        {
            var length = await this.ReadIntAsync();
            using var buffer = new RentedArray<byte>(length);
            if (BitConverter.IsLittleEndian)
            {
                buffer.Span.Reverse();
            }
            await this.ReadExactlyAsync(buffer);

            var value = Encoding.UTF8.GetString(buffer);
            if (maxLength > 0 && value.Length > maxLength)
            {
                throw new ArgumentException($"string ({value.Length}) exceeded maximum length ({maxLength})", nameof(maxLength));
            }
            return value;
        }

        public byte[] ReadUInt8Array(int length = 0)
        {
            if (length == 0)
                length = ReadInt();

            var result = new byte[length];
            if (length == 0)
                return result;

            int n = length;
            while (true)
            {
                n -= Read(result, length - n, n);
                if (n == 0)
                    break;
            }
            return result;
        }

        public async Task<byte[]> ReadUInt8ArrayAsync(int length = 0)
        {
            if (length == 0)
                length = await this.ReadIntAsync();

            var result = new byte[length];
            if (length == 0)
                return result;

            int n = length;
            while (true)
            {
                n -= await this.ReadAsync(result, length - n, n);
                if (n == 0)
                    break;
            }
            return result;
        }

        public async Task<byte> ReadUInt8Async()
        {
            int value = await this.ReadByteAsync();
            if (value == -1)
                throw new EndOfStreamException();
            return (byte)value;
        }

        public byte[] ReadByteArray()
        {
            var length = ReadInt();
            return ReadUInt8Array(length);
        }
    }
}
