using Common.Utilities;
using System.Text;

namespace Common.Helper
{
    public static class FileHelper
    {
        private static readonly char[] IllegalPathChars = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).ToArray();

        public static bool HasIllegalCharacters(string path)
        {
            return path.Any(c => IllegalPathChars.Contains(c));
        }

        public static string GetRandomFilename(int length, string extension = "")
        {
            return string.Concat(StringHelper.GetRandomString(length), extension);
        }

        public static string GetTempFilePath(string extension)
        {
            string tempFilePath;
            do
            {
                tempFilePath = Path.Combine(Path.GetTempPath(), GetRandomFilename(12, extension));
            } while (File.Exists(tempFilePath));

            return tempFilePath;
        }

        public static void WriteLogFile(string filename, string appendText)
        {
            appendText = ReadLogFile(filename) + appendText;

            using (FileStream fStream = File.Open(filename, FileMode.Create, FileAccess.Write))
            {
                byte[] data = Encoding.UTF8.GetBytes(appendText);
                fStream.Seek(0, SeekOrigin.Begin);
                fStream.Write(data, 0, data.Length);
            }
        }

        public static string ReadLogFile(string filename)
        {
            return File.Exists(filename) ? Encoding.UTF8.GetString(File.ReadAllBytes(filename)) : string.Empty;
        }
    }
}
