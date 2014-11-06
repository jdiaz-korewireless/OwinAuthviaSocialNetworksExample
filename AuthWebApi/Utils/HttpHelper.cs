using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuthWebApi.Utils
{    
    public static class HttpHelper
    {
        public static HttpClient CreateHttpClient()
        {
            var decompressHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            return new HttpClient(decompressHandler);
        }

        public static async Task<TResponse> GetResponseAsync<TResponse>(string uri, params JsonConverter[] jsonConverters)
        {
            using (var client = HttpHelper.CreateHttpClient())
            {
                var response = await client.GetStringAsync(uri);
                return JsonConvert.DeserializeObject<TResponse>(response, jsonConverters);
            }
        }

        public static Task<TResponse> PostFormRequestAsync<TResponse>(string uri, string formValues, params JsonConverter[] jsonConverters)
        {
            return SendRequestAsync<TResponse>(HttpMethod.Post, uri, formValues, jsonConverters);
        }

        public static Task<TResponse> DeleteAsync<TResponse>(string uri, string formValues, params JsonConverter[] jsonConverters)
        {
            return SendRequestAsync<TResponse>(HttpMethod.Delete, uri, formValues, jsonConverters);
        }

        private static async Task<TResponse> SendRequestAsync<TResponse>(HttpMethod httpMethod, string uri, string formValues, params JsonConverter[] jsonConverters)
        {
            using (var client = HttpHelper.CreateHttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                //Build request
                request.RequestUri = new Uri(uri);
                request.Method = httpMethod;

                request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(formValues));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                using (HttpResponseMessage response = client.SendAsync(request, cancellationTokenSource.Token).Result)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TResponse>(responseContent, jsonConverters);
                }
            }
        }
    }
}
