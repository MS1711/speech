// <copyright file="TtsServiceClient.cs" company="Microsoft">
// © Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Speech.TtsService.HttpClient
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to send TtsService Http request.
    /// </summary>
    public class TtsServiceClient : ITtsServiceClient
    {
        /// <summary>
        /// The port
        /// </summary>
        private int port;

        /// <summary>
        /// Initializes a default instance of the TtsServiceClient class
        /// </summary>
        public TtsServiceClient()
            : this(80)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TtsServiceClient class with provided params.
        /// </summary>
        /// <param name="port">http port</param>
        public TtsServiceClient(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// Sends Ping GET HttpRequest to the TtsServive
        /// </summary>
        /// <param name="hostName">host name</param>
        /// <param name="timeout">request timeout in msec</param>
        /// <returns>TtsServiceResponse object</returns>
        public async Task<TtsServiceResponse> SendPingRequest(string hostName, int timeout)
        {
            if (string.IsNullOrEmpty(hostName))
            {
                throw new ArgumentNullException("hostName");
            }

            //Trace.TraceInformation("TtsServiceClient.SendPingRequest: Sending TTS ping request to hostName={0}" + hostName);

            return await SendHttpRequest(hostName, this.port, "ping", "GET", null, null, null, null, timeout);
        }

        /// <summary>
        /// Sends Synth POST HttpRequest to TtsService.
        /// </summary>
        /// <param name="clientId">Id of the client sending the request</param>
        /// <param name="hostName">host name</param>
        /// <param name="outputFormat">audio outputformat</param>
        /// <param name="ssmlInput">SSML input text to synthesize</param>
        /// <param name="timeout">request timeout in msec</param>
        /// <returns>TtsServiceResponse object</returns>
        public async Task<TtsServiceResponse> SendSynthRequest(string clientId, string hostName, string outputFormat, string ssmlInput, int timeout)
        {
            return await SendSynthRequest(clientId, Guid.NewGuid().ToString(), hostName, outputFormat, ssmlInput, "application/ssml+xml", timeout);
        }

        /// <summary>
        /// Sends Synth POST HttpRequest to TtsService.
        /// </summary>
        /// <param name="clientId">Id of the client sending the request</param>
        /// <param name="requestId">The request id.</param>
        /// <param name="hostName">host name</param>
        /// <param name="outputFormat">audio outputformat</param>
        /// <param name="inputText">input text to synthesize</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="timeout">request timeout in msec</param>
        /// <returns>
        /// TtsServiceResponse object
        /// </returns>
        public async Task<TtsServiceResponse> SendSynthRequest(string clientId, string requestId, string hostName, string outputFormat, string inputText, string contentType, int timeout)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId");
            }

            if (string.IsNullOrEmpty(requestId))
            {
                throw new ArgumentNullException("requestId");
            }

            if (string.IsNullOrEmpty(hostName))
            {
                throw new ArgumentNullException("hostName");
            }

            if (string.IsNullOrEmpty(outputFormat))
            {
                throw new ArgumentNullException("outputFormat");
            }

            if (string.IsNullOrEmpty(inputText))
            {
                throw new ArgumentNullException("inputText");
            }

            Dictionary<string, string> queryParam = new Dictionary<string, string>();
            Dictionary<string, string> headers = new Dictionary<string, string>();

            // just add output format to both querystring and request headers
            queryParam.Add("outputformat", outputFormat);
            headers.Add("X-MICROSOFT-OutputFormat", outputFormat);

            // Add the headers required for instrumentation logging
            headers.Add("X-FD-ClientID", clientId);
            headers.Add("X-FD-ImpressionGUID", requestId);

            // Get post body as bytes.
            byte[] postBody = Encoding.UTF8.GetBytes(inputText);

            //Trace.TraceInformation("TtsServiceClient.SendPingRequest: Sending TTS synth request to hostname={0} with outputformat={1} inputText=[{2}], reqTimeout={3}", hostName, outputFormat, inputText, timeout);
            
            return await SendHttpRequest(hostName, this.port, "synthesize", "POST", contentType, queryParam, headers, postBody, timeout);
        }

        /// <summary>
        /// Send Http Request
        /// </summary>
        /// <param name="hostName">http host name</param>
        /// <param name="port">http port</param>
        /// <param name="servicePath">service path</param>
        /// <param name="method">http method</param>
        /// <param name="contentType">http content-type</param>
        /// <param name="queryParamCollection"> query param collection</param>
        /// <param name="headersCollection">headers collection</param>
        /// <param name="body">The post body to include</param>
        /// <param name="timeout">request timeout in msec</param>
        /// <returns>TtsService response object</returns>
        private static async Task<TtsServiceResponse> SendHttpRequest(string hostName, int port, string servicePath, string method, string contentType, Dictionary<string, string> queryParamCollection, Dictionary<string, string> headersCollection, byte[] body, int timeout)
        {
            if (hostName == null)
            {
                throw new ArgumentNullException("hostName");
            }

            if (servicePath == null)
            {
                throw new ArgumentNullException("servicePath");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (method.Equals("POST") && (body == null || contentType == null))
            {
                throw new ArgumentException("Either postbody or content-type is null");
            }

            string queryParams = string.Empty;
            if (queryParamCollection != null && queryParamCollection.Count > 0)
            {
                string value = null;

                foreach (string param in queryParamCollection.Keys)
                {
                    value = queryParamCollection[param];

                    // TODO: make value as URLEncoded.
                    queryParams += param + "=" + value + "&";
                }
            }

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            //WebResponse response = null;
            TtsServiceResponse serviceResponse = null;
            try
            {
                string requestUri = String.Format(CultureInfo.InvariantCulture, "http://{0}:{1}/{2}?{3}", hostName, port, servicePath, queryParams);

                request = (HttpWebRequest)WebRequest.Create(requestUri);
                request.Method = method;
                request.ContinueTimeout = timeout;

                if (headersCollection != null && headersCollection.Count > 0)
                {
                    foreach (string key in headersCollection.Keys)
                    {
                        //request.Headers.Add(key, headersCollection[key]);
                        request.Headers[key] = headersCollection[key];
                    }
                }

                if (method.Equals("POST"))
                {
                    request.ContentType = contentType;
                    //request.ContentLength = body.Length;

                    //Stream requestStream = request.GetRequestStream();
                    Stream requestStream = await request.GetRequestStreamAsync();
                    requestStream.Write(body, 0, body.Length);
                    //requestStream.Close();
                    requestStream.Flush();
                }

                //response = (HttpWebResponse)request.GetResponse();
                response = (HttpWebResponse)await request.GetResponseAsync();
                serviceResponse = new TtsServiceResponse(response);
            }
            catch (WebException webExcp)
            {
                WebExceptionStatus status = webExcp.Status;
                response = (HttpWebResponse)webExcp.Response;
                //Trace.TraceError("HttpClient.SendHttpRequest: WebException has been caught while trying to send Http request : WebExceptionStatus={0}, Exception={0}", status.ToString(), webExcp.ToString());

                if (status == WebExceptionStatus.SendFailure)
                {
                    serviceResponse = new TtsServiceResponse(HttpStatusCode.RequestTimeout, HttpStatusCode.RequestTimeout.ToString());
                }
                else
                {
                    // If NoResponse is received, then throw the exception
                    if (response == null)
                    {
                        string msg = string.Format(CultureInfo.InvariantCulture, "HttpClient.SendHttpRequest: WebException has been caught while trying to send Http request with WebExceptionStatus={0}", status.ToString());
                        throw new WebException(msg);
                    }
                    else
                    {
                        serviceResponse = new TtsServiceResponse(response);
                        //Trace.TraceError("HttpClient.SendHttpRequest: WebException has been caught, received Http Response with statusCode={0}", serviceResponse.StatusCode.ToString());
                    }
                }
            }

            return serviceResponse;
        }
    }
}
