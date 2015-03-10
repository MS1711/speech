namespace LiebaoAp.Common.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpUtility
    {
        public static string Post(string requestUrl, string requestText)
        {
            return PostAsync(requestUrl, requestText).Result;
        }

        public static async Task<string> PostAsync(string requestUrl, string requestText)
        {
            var traceId = Guid.NewGuid().ToString("N");
            Trace.TraceInformation(
                "HttpUtility.Post(traceId:'{0}', requestUrl:'{1}', requestText:'{2}')",
                traceId,
                requestUrl,
                requestText);

            var encodedText = EncodingUtility.Encode(requestText);
            Trace.TraceInformation("HttpUtility.Post(traceId:'{0}', encodedText:'{1}')", traceId, encodedText);

            using (var httpClient = new HttpClient())
            {
                var httpContent = new StringContent(encodedText);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = httpContent };

                var response = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Trace.TraceInformation("HttpUtility.Post(traceId:'{0}', responseText:'{1}')", traceId, responseText);

                var decodedResponseText = EncodingUtility.Decode(responseText);
                Trace.TraceInformation(
                    "HttpUtility.Post(traceId:'{0}', decodedResponseText:'{1}')",
                    traceId,
                    decodedResponseText);

                return decodedResponseText;
            }
        }
    }
}
