using System.IO;

namespace Infrastructure
{
    public static class StreamExtensions
    {
        public static byte[] ReadToArray(this Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
