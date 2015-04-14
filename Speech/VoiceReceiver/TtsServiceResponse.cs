// <copyright file="TtsServiceResponse.cs" company="Microsoft">
// © Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Speech.TtsService.HttpClient
{
    using System;
    using System.IO;
    using System.Net;    

    /// <summary>
    /// Class to hold HttpResponse data from TtsService request.
    /// </summary>
    public class TtsServiceResponse
    {
        /// <summary>
        /// Holds http response data as array of bytes.
        /// </summary>
        private byte[] responseData;

        /// <summary>
        /// Initializes new instance of TtsServiceResponse when WebHttpResponse is null, in some cases (like HttpTimeout)
        /// </summary>
        /// <param name="statusCode">httpStatus code</param>
        /// <param name="errorMessage">http error message</param>
        public TtsServiceResponse(HttpStatusCode statusCode, string errorMessage) 
            : this(statusCode, errorMessage, 0, null, string.Empty)
        {            
        }

        /// <summary>
        /// Initializes new instance of TtsServiceResponse (this is useful for unittesting)
        /// </summary>
        /// <param name="statusCode">httpStatus code</param>
        /// <param name="errorMessage">http error message</param>
        /// <param name="contentLength">content length</param>
        /// <param name="data">content data</param>
        /// <param name="contentType">content type</param>
        public TtsServiceResponse(HttpStatusCode statusCode, string errorMessage, long contentLength, byte[] data, string contentType)
        {
            this.StatusCode = statusCode;
            this.ErrorMessage = errorMessage;
            this.ContentLength = contentLength;
            this.responseData = data;
            this.ContentType = contentType;
        }

        /// <summary>
        /// Initializes new instance of TtsServiceResponse and converts HttpWebResponse 
        /// to TtsServiceResponse
        /// </summary>
        /// <param name="response">HttpWebResponse object to read http response data from</param>
        public TtsServiceResponse(HttpWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            this.StatusCode = response.StatusCode;
            this.ErrorMessage = response.StatusDescription;

            if (response.ContentLength > 0)
            {
                byte[] data = null;
                using (Stream responseStream = response.GetResponseStream())
                {                   
                    data = StreamToBytes(responseStream);                    
                }

                if (data != null)
                {
                    if ((long)data.Length == response.ContentLength)
                    {
                        this.responseData = data;
                        this.ContentLength = response.ContentLength;
                        this.ContentType = response.ContentType;
                    }
                    else
                    {
                        throw new ArgumentException("HttpWebResponse content data length does not match the http response.Contentlength.");
                    }
                }
                else
                {
                    throw new ArgumentException("data cannot be null when http response.Contentlength > 0");
                }
            }
            else
            {
                this.responseData = null;
                this.ContentLength = 0;
                this.ContentType = string.Empty;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="TtsServiceResponse"/> class from being created.
        /// </summary>
        private TtsServiceResponse()
        {
        }

        /// <summary>
        /// Gets and sets HttpResponse StatusCode
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets and sets Http Response Error message
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Gets and sets Http Response Content Length.
        /// </summary>
        public long ContentLength { get; private set; }

        /// <summary>
        /// Gets and sets Http Response Content Length.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets Http Response data in bytes
        /// </summary>
        /// <returns> array of response data bytes</returns>
        public byte[] GetResponseData()
        {
            // Need to return a clone of the array so that consumers
            // of this library cannot change its contents.
            return (this.responseData != null) ? ((byte[])this.responseData.Clone()) : null;
        }

        /// <summary>
        /// Converts a stream to a byte array
        /// </summary>
        /// <param name="input">The stream to convert</param>
        /// <returns>The stream as a byte array</returns>
        private static byte[] StreamToBytes(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
