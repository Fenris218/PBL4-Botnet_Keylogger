namespace Common.Models
{
    public class FileChunk
    {
        //vị trí của chunk trong file gốc (byte thứ bao nhiêu)
        public long Offset { get; set; }
        //dữ liệu của chunk
        public byte[] Data { get; set; }
    }
}
