using Common.Models;
using System.Collections;

namespace Common.IO
{
    //FileSplit có thể được duyệt (foreach) như một tập hợp các phần tử kiểu FileChunk.
    public class FileSplit : IEnumerable<FileChunk>, IDisposable
    {
        //kích thước tối đa 1 chunk là 8KB
        public readonly int MaxChunkSize = 8 * 1024;
        public string FilePath => _fileStream.Name;
        public long FileSize => _fileStream.Length;
        private readonly FileStream _fileStream;

        public FileSplit(string filePath, FileAccess fileAccess)
        {
            switch (fileAccess)
            {
                case FileAccess.Read:
                    _fileStream = File.OpenRead(filePath);
                    break;
                case FileAccess.Write:
                    _fileStream = File.OpenWrite(filePath);
                    break;
                default:
                    throw new ArgumentException($"{nameof(fileAccess)} must be either Read or Write.");
            }
        }
        //ghi 1 chunk vào file
        public void WriteChunk(FileChunk chunk)

        {
            //ban đầu con trỏ chỉ offset = 0, nên dòng này sẽ di chuyển con trỏ về offset tương ứng (tính từ begin=0) của chunk để ghi file
            _fileStream.Seek(chunk.Offset, SeekOrigin.Begin);
            //0 ở đây là vị trí bắt đầu trong mảng byte[]
            _fileStream.Write(chunk.Data, 0, chunk.Data.Length);
        }

        //đọc 1 chunk từ vị trí chỉ định
        public FileChunk ReadChunk(long offset)
        {
            _fileStream.Seek(offset, SeekOrigin.Begin);

            long chunkSize = _fileStream.Length - _fileStream.Position < MaxChunkSize
                ? _fileStream.Length - _fileStream.Position
                : MaxChunkSize;

            var chunkData = new byte[chunkSize];
            _fileStream.Read(chunkData, 0, chunkData.Length);

            return new FileChunk
            {
                Data = chunkData,
                Offset = _fileStream.Position - chunkData.Length
            };
        }

        //để FileSplit có thể được duyệt (foreach) như một tập hợp các phần tử kiểu FileChunk, thì phải có phương thức GetEnumerator
        public IEnumerator<FileChunk> GetEnumerator()
        {
            for (long currentChunk = 0; currentChunk <= _fileStream.Length / MaxChunkSize; currentChunk++)
            {//yield return trả từng Chunk ra ngoài mỗi vòng lặp, chứ không cần load toàn bộ file một lúc.
                yield return ReadChunk(currentChunk * MaxChunkSize);
            }
        }

        //Hiểu đơn giản là cầu nối đảm bảo tính tương thích giữa IEnumerable<T> và IEnumerable.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Dispose để dọn tài nguyên khi enumerator không còn sử dụng nữa.    
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // khỏi gọi finalizer nữa
        }

        protected virtual void Dispose(bool disposing)
        {
            //nếu true thì dọn managed resources
            if (disposing)
            {
                _fileStream.Dispose();
            }
        }
    }
}
