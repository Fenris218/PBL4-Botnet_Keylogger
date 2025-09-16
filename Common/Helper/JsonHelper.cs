using System.Runtime.Serialization.Json;
using System.Text;

namespace Common.Helper
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(Stream stream)
        {
            var s = new DataContractJsonSerializer(typeof(T));
            return (T)s.ReadObject(stream);
        }
    }
}
