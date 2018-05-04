//-----------------------------------------------------------------------------
//
// THIS CODE-SAMPLE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
// EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//
// This sample is not supported under any Microsoft standard support program 
// or service. The script is provided AS IS without warranty of any kind. 
// Microsoft further disclaims all implied warranties including, without 
// limitation, any implied warranties of merchantability or of fitness for a 
// particular purpose. The entire risk arising out of the use or performance
// of the sample and documentation remains with you. In no event shall 
// Microsoft, its authors, or anyone else involved in the creation, 
// production, or delivery of the script be liable for any damages whatsoever 
// (including, without limitation, damages for loss of business profits, 
// business interruption, loss of business information, or other pecuniary 
// loss) arising out of ' the use of or inability to use the sample or 
// documentation, even if Microsoft has been advised of the possibility of 
// such damages.
//
// Copyright (c) 2015-2016 Microsoft GmbH
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace CreateServiceEndpoint
{
    /// <summary>
    /// Helper methods for HttpClient
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// PostAsJsonAsync
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUrl"></param>
        /// <param name="model"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model)
        {
            return await PostAsJsonAsync(client, requestUrl, model, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// PostAsJsonAsync
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUrl"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PostAsync(requestUrl, stringContent, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// PutAsJsonAsync
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUrl"></param>
        /// <param name="model"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model)
        {
            return await PutAsJsonAsync(client, requestUrl, model, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// PutAsJsonAsync
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUrl"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PutAsync(requestUrl, stringContent, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// PatchAsJsonAsync
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUrl"></param>
        /// <param name="model"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PatchAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model)
        {
            return await PatchAsJsonAsync(client, requestUrl, model, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// PatchAsJsonAsync
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUrl"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PatchAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PatchAsync(requestUrl, stringContent, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// PatchAsync
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(client.BaseAddress + requestUri),
                Content = content,
            };

            return client.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Throw an exception if the response code is not success
        /// </summary>
        /// <param name="response"></param>
        /// <param name="excludes"></param>
        public static void EnsureHttpSuccess(this HttpResponseMessage response, params HttpStatusCode[] excludes)
        {
            if(!response.IsSuccessStatusCode && excludes.All(x => x != response.StatusCode))
            {
                response.Content.LoadIntoBufferAsync().Wait();
                throw new HttpException((int)response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
