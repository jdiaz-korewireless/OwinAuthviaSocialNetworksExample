using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AuthWebApi.MediaTypeFormatters
{
    public class BinaryMediaTypeFormatter : MediaTypeFormatter
    {
        private static Type SupportedDataType = typeof(byte[]);
        private static string SupportedContentType = "application/octet-stream";

        public BinaryMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(SupportedContentType));
        }

        public override bool CanReadType(Type type)
        {
            return type == SupportedDataType;
        }

        public override bool CanWriteType(Type type)
        {
            return type == SupportedDataType;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskSource = new TaskCompletionSource<object>();
            try
            {
                taskSource.SetResult(HelperFormatter.ReadBytes(readStream));
            }
            catch (Exception e)
            {
                taskSource.SetException(e);
            }
            return taskSource.Task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var taskSource = new TaskCompletionSource<object>();
            try
            {
                HelperFormatter.WriteBytes((byte[])value, writeStream);
                taskSource.SetResult(null);
            }
            catch (Exception e)
            {
                taskSource.SetException(e);
            }
            return taskSource.Task;
        }
    }
}