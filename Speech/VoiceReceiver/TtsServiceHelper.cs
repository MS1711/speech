// <copyright file="TtsServiceHelper.cs" company="Microsoft">
// © Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Speech.TtsService.HttpClient
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.Speech.TtsService.HttpClient;
    using System.Threading.Tasks;

    /// <summary>
    /// tts helper 
    /// </summary>
    public class TtsServiceHelper
    {
        /// <summary>
        /// synthesize with service 
        /// </summary>
        /// <param name="serviceHost">host name</param>
        /// <param name="port">port name</param>
        /// <param name="clientId">client id</param>
        /// <param name="ssmlText">ssml input</param>
        /// <param name="format">format string</param>
        /// <param name="output">output stream</param>
        /// <returns>true or false</returns>
        //public static async Task<bool> SynthesizeUsingService(string serviceHost, int port, string clientId, string ssmlText, string format, Stream output)
        public static async Task<bool> SynthesizeUsingService(string serviceHost, int port, string clientId, string ssmlText, string format, MemoryStream output)
        {
            HttpStatusCode responseCode = HttpStatusCode.OK;
            return await SynthesizeUsingService(serviceHost, port, clientId, ssmlText, format, output, responseCode);
        }

        /// <summary>
        /// synthesize with service 
        /// </summary>
        /// <param name="serviceHost">host name</param>
        /// <param name="port">port name</param>
        /// <param name="clientId">client id</param>
        /// <param name="ssmlText">ssml input</param>
        /// <param name="format">format string</param>
        /// <param name="output">output stream</param>
        /// <param name="responseCode">response code</param>
        /// <returns>true or false</returns>
        //public static async Task<bool> SynthesizeUsingService(string serviceHost, int port, string clientId, string ssmlText, string format, Stream output, HttpStatusCode responseCode)
        public static async Task<bool> SynthesizeUsingService(string serviceHost, int port, string clientId, string ssmlText, string format, MemoryStream output, HttpStatusCode responseCode)
        {
            ITtsServiceClient client = new TtsServiceClient(port);

            string requestId = Guid.NewGuid().ToString();
            string audioFormat = format;
            string contentType = "application/ssml+xml";
            int timeOut = 60000;

            DateTime requestStartTime = DateTime.Now;

            TtsServiceResponse serviceResponse = await client.SendSynthRequest(
                clientId,
                requestId,
                serviceHost,
                audioFormat,
                ssmlText,
                contentType,
                timeOut);

            DateTime requestEndTime = DateTime.Now;
            Debug.WriteLine("{0} : Time Taken : {1}ms", DateTime.Now.ToString("o", CultureInfo.InvariantCulture), requestEndTime.Subtract(requestStartTime).Milliseconds);
            Debug.WriteLine("{0} : Response from Server", DateTime.Now.ToString("o", CultureInfo.InvariantCulture));
            Debug.WriteLine("{0} : Content length is {1}", DateTime.Now.ToString("o", CultureInfo.InvariantCulture), serviceResponse.ContentLength);
            Debug.WriteLine("{0} : Content type is {1}", DateTime.Now.ToString("o", CultureInfo.InvariantCulture), serviceResponse.ContentType);

            responseCode = serviceResponse.StatusCode;

            if (serviceResponse.StatusCode == HttpStatusCode.OK)
            {
                Debug.WriteLine("{0} : Response Status Code is OK and StatusDescription is: {1}", DateTime.Now.ToString("o", CultureInfo.InvariantCulture), serviceResponse.ErrorMessage);
                byte[] responseData = serviceResponse.GetResponseData();
                output.Write(responseData, 0, responseData.Length);
                //-------------------------------------------
                output.Seek(0, SeekOrigin.Begin);
                //-------------------------------------------
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
