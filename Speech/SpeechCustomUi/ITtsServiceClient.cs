// <copyright file="ITtsServiceClient.cs" company="Microsoft">
// © Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Speech.TtsService.HttpClient
{
    using System.Threading;
    using System.Threading.Tasks;
    /// <summary>
    /// Interface for sending TtsService Http requests
    /// </summary>
    public interface ITtsServiceClient
    {
        /// <summary>
        /// Sends Ping GET HttpRequest to the TtsServive
        /// </summary>
        /// <param name="hostName">host name</param>
        /// <param name="timeout">request timeout in msec</param>
        /// <returns>TtsServiceResponse object</returns>
        Task<TtsServiceResponse> SendPingRequest(string hostName, int timeout);

        /// <summary>
        /// Sends Synth POST HttpRequest to TtsService.
        /// </summary>
        /// <param name="clientId">Id of the client sending the request</param>
        /// <param name="hostName">host name</param>
        /// <param name="outputFormat">audio outputformat</param>
        /// <param name="ssmlInput">SSML input text to synthesize</param>
        /// <param name="timeout">request timeout in msec</param>
        /// <returns>
        /// TtsServiceResponse object
        /// </returns>
        Task<TtsServiceResponse> SendSynthRequest(string clientId, string hostName, string outputFormat, string ssmlInput, int timeout);

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
        Task<TtsServiceResponse> SendSynthRequest(string clientId, string requestId, string hostName, string outputFormat, string inputText, string contentType, int timeout);
    }
}
