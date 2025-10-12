using Common.Enums;

namespace Common.Models
{
    public class FileSystemEntry
    {
        public FileType EntryType { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }
        public ContentType ContentType { get; set; }
    }
}
