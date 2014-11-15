using System.IO;

namespace AuthWebApi.MediaTypeFormatters
{
    public static class HelperFormatter
    {
        public static byte[] ReadBytes(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }

                return memoryStream.ToArray();
            }
        }

        public static void WriteBytes(byte[] bytes, Stream writeStream)
        {
            if (bytes == null)
                return;

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                memoryStream.CopyTo(writeStream);
            }
        }
    }
}