﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Web;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Threading;
using Newtonsoft.Json;

namespace VoiceReceiver
{
    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }


    /*
    * {
        "version": "3.0",
        "header": {
            "status": "success",
            "scenario": "smd",
            "name": "北京今天天气如何.",
            "lexical": "北京 今天天气如何",
            "properties": {
                "requestid": "0692ff9a-2cf0-4652-8f5e-b301ff7e4128",
                "HIGHCONF": "1"
            }
        },
        "results": [
        {
            "scenario": "smd",
            "name": "北京今天天气如何.",
            "lexical": "北京 今天天气如何",
            "confidence": "0.9605375",
            "properties": {
            "HIGHCONF": "1"
            }
        }
        ]
    }
    */
    public class SRResult
    {
        public class Header
        {
            public string Status { get; set; }
            public string Name { get; set; }
        }
        
        public class Result
        {
            public string Name { get; set; }
        }

        public Header header { get; set; }
        public List<Result> results { get; set; }

        public bool IsSuccess
        {
            get
            {
                return header.Status.Equals("success");
            }
        }

        public string Best
        {
            get
            {
                if (results.Count == 0)
                {
                    return "";
                }
                else
                {
                    return results[0].Name;
                }
            }
        }
    }

    /*
     * This class demonstrates how to get a valid O-auth token from
     * Azure Data Market.
     */
    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private string clientId;
        private string clientSecret;
        private string request;
        private AdmAccessToken token;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            /*
             * If clientid or client secret has special characters, encode before sending request
             */
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope={2}",
                                          HttpUtility.UrlEncode(clientId),
                                          HttpUtility.UrlEncode(clientSecret),
                                          HttpUtility.UrlEncode("https://speech.platform.bing.com"));

            this.token = HttpPost(DatamarketAccessUri, this.request);

            // renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }

        public AdmAccessToken GetAccessToken()
        {
            return this.token;
        }

        private void RenewAccessToken()
        {
            AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, this.request);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.token = newAccessToken;
            Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}",
                              this.clientId,
                              this.token.access_token));
        }

        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }

    public class SpeechUtil
    {
        private static string URL = "https://speech.platform.bing.com/recognize";

        public static Task<SRResult> GetResultAsync(string wavFile)
        {
            return Task.Run<SRResult>(() => { 
                return GetResult(wavFile);
            });
        }

        public static SRResult GetResult(string wavFile)
        {
            if (string.IsNullOrEmpty(wavFile))
            {
                return null;
            }

            AdmAccessToken admToken;
            string headerValue;

            /*
             * Note: Create an App using the ADM portal by signing up for an account at https://datamarket.azure.com/home and then go to https://datamarket.azure.com/developer/applications and click register.  Put in some unique clientId and a client secret will be generated for you.  
             */
            AdmAuthentication admAuth = new AdmAuthentication("TestSR", "0qAB53n4hxjBtNFyno8TMWTzjGXYtgGwXRH9/AfJsFE=");

            string requestUri = URL.Trim(new char[] { '/', '?' });

            /* URI Params. Refer to the README file for more information. */
            requestUri += @"?scenarios=smd";                                  // websearch is the other main option.
            requestUri += @"&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";     // You must use this ID.
            requestUri += @"&locale=zh-CN";                                   // We support several other languages.  Refer to README file.
            requestUri += @"&device.os=WindowsOS";
            requestUri += @"&version=3.0";
            requestUri += @"&format=json";
            requestUri += @"&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3";
            requestUri += @"&requestid=" + Guid.NewGuid().ToString();

            string host = @"speech.platform.bing.com";
            string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";

            /*
             * Input your own audio file or use read from a microphone stream directly.
             */
            string audioFile = wavFile;
            string responseString;
            FileStream fs = null;

            try
            {
                admToken = admAuth.GetAccessToken();
                Console.WriteLine("ADM Token: {0}\n", admToken.access_token);

                /*
                 * Create a header with the access_token property of the returned token
                 */
                headerValue = "Bearer " + admToken.access_token;

                Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);

                HttpWebRequest request = null;
                request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.SendChunked = true;
                request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version11;
                request.Host = host;
                request.ContentType = contentType;
                request.Headers["Authorization"] = headerValue;

                using (fs = new FileStream(audioFile, FileMode.Open, FileAccess.Read))
                {

                    /*
                     * Open a request stream and write 1024 byte chunks in the stream one at a time.
                     */
                    byte[] buffer = null;
                    int bytesRead = 0;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        /*
                         * Read 1024 raw bytes from the input audio file.
                         */
                        buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }

                        // Flush
                        requestStream.Flush();
                    }

                    using (WebResponse response = request.GetResponse())
                    {   
                        Console.WriteLine(((HttpWebResponse)response).StatusCode);

                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            responseString = sr.ReadToEnd();
                            SRResult res = JsonConvert.DeserializeObject<SRResult>(responseString);
                            return res;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
