using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CreateServiceEndpoint
{
    internal class VstsTasks
    {
        private const string VstsPat = "YOUR-VSTS-PAT";   // this should come from secure configuration

        /// <summary>
        /// Gets the service endpoint identifier of the specified service endpoint.
        /// </summary>
        /// <param name="vstsBaseUrl">The VSTS account base URL.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="serviceEndpointName">The service endpoint name</param>
        /// <returns>The id if successful, empty guid otherwise</returns>
        public static async Task<Guid?> GetServiceEndpointId(string vstsBaseUrl, Guid projectId, string serviceEndpointName)
        {
            var client = CreateVstsHttpClient(vstsBaseUrl, VstsPat);

            var url = $"defaultcollection/{projectId}/_apis/distributedtask/serviceendpoints?api-version=1.0";
            var response = await client.GetAsync(url).ConfigureAwait(false);
            response.EnsureHttpSuccess();

            var data = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            if (data["value"].Select(x => (string)x.SelectToken("name")).Any(n => n == serviceEndpointName))
                return
                    data["value"].Select(
                            x => new { name = (string)x.SelectToken("name"), id = (Guid)x.SelectToken("id") })
                        .FirstOrDefault(x => x.name == serviceEndpointName).id;
            else
                return null;
        }

        /// <summary>
        /// Checks if the specified service endpoint already exists.
        /// </summary>
        /// <param name="vstsBaseUrl">The VSTS account base URL.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="serviceEndpointName"></param>
        /// <returns>True if exists, false otherwise</returns>
        public static async Task<bool> ServiceEndpointExists(string vstsBaseUrl, Guid projectId, string serviceEndpointName)
        {
            return await GetServiceEndpointId(vstsBaseUrl, projectId, serviceEndpointName).ConfigureAwait(false) != null;
        }

        public static async Task DeleteServiceEndpoint(string vstsBaseUrl, Guid projectId, Guid sepId)
        {
            var client = CreateVstsHttpClient(vstsBaseUrl, VstsPat);

            var url =
                $"defaultcollection/{projectId}/_apis/serviceendpoint/endpoints/{sepId}?api-version=4.1-preview.1";
            var response = await client.DeleteAsync(url).ConfigureAwait(false);
            response.EnsureHttpSuccess();

        }

        /// <summary>
        /// Creates the specified AppCenter service endpoint.
        /// </summary>
        /// <param name="vstsBaseUrl">The VSTS account base URL.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="serviceEndpointName">The service endpoint name</param>
        /// <param name="appCenterApiToken">The access token for AppCenter</param>
        /// <returns>The id of the created endpoint</returns>
        public static async Task<Guid> CreateAppCenterServiceEndpoint(string vstsBaseUrl, Guid projectId, string serviceEndpointName, string appCenterApiToken)
        {
            var client = CreateVstsHttpClient(vstsBaseUrl, VstsPat);

            var sepId = Guid.NewGuid();

            var data = new
            {
                id = sepId,
                description = string.Empty,
                administratorsGroup = (string)null,
                name = serviceEndpointName,
                type = "vsmobilecenter",
                url = "https://api.mobile.azure.com/v0.1",
                readersGroup = (string)null,
                groupScopeId = (string)null,
                authorization = new
                {
                    parameters = new
                    {
                        apitoken = appCenterApiToken
                    },
                    scheme = "Token",
                }
            };

            var url = $"defaultcollection/{projectId}/_apis/distributedtask/serviceendpoints/{sepId}?api-version=2.2-preview.1";
            var response = await client.PostAsJsonAsync(url, data).ConfigureAwait(false);
            response.EnsureHttpSuccess();
            dynamic result = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            return Guid.Parse(result.id.ToString());
        }

        /// <summary>
        /// Creates the specified service endpoint.
        /// </summary>
        /// <param name="vstsBaseUrl">The VSTS account base URL.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="serviceEndpointName">The service endpoint name</param>
        /// <param name="sonarQubeServerUrl">The Url of the selected SonarQube server.</param>
        /// <param name="sqUserName">SonarQube user name</param>
        /// <param name="sqPassword">SonarQube password</param>
        /// <returns>The id of the created endpoint</returns>
        public static async Task<Guid> CreateSonarQubeServiceEndpoint(
            string vstsBaseUrl, Guid projectId, string serviceEndpointName, string sonarQubeServerUrl, string sqUserName, string sqPassword)
        {
            var client = CreateVstsHttpClient(vstsBaseUrl, VstsPat);

            var sepId = Guid.NewGuid();

            var data = new
            {
                id = sepId,
                description = string.Empty,
                administratorsGroup = (string)null,
                authorization = new
                {
                    parameters = new
                    {
                        username = sqUserName,
                        password = sqPassword,
                    },
                    scheme = "UsernamePassword",
                },
                createdBy = (string)null,
                name = serviceEndpointName,
                type = "generic",
                url = sonarQubeServerUrl,
                readersGroup = (string)null,
                groupScopeId = (string)null
            };

            var url = $"defaultcollection/{projectId}/_apis/distributedtask/serviceendpoints/{sepId}?api-version=2.2-preview.1";
            var response = await client.PostAsJsonAsync(url, data).ConfigureAwait(false);
            response.EnsureHttpSuccess();
            dynamic result = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            return Guid.Parse(result.id.ToString());
        }

        /// <summary>
        /// Gets the identifier of the specified VSTS project.
        /// </summary>
        /// <param name="vstsBaseUrl">The VSTS account base URL.</param>
        /// <param name="name">The name of the project.</param>
        /// <returns></returns>
        public static async Task<Guid> GetId(string vstsBaseUrl, string name)
        {
            var client = CreateVstsHttpClient(vstsBaseUrl, VstsPat);

            var response = await client.GetAsync("defaultcollection/_apis/projects/" + name).ConfigureAwait(false);

            response.EnsureHttpSuccess();

            dynamic project = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            var retval = Guid.Parse((string)project.id);
            return retval;
        }

        /// <summary>
        /// Creates the VSTS HTTP client with predefined URL and credentials for project related API access.
        /// </summary>
        /// <param name="vstsBaseUrl">The VSTS base URL.</param>
        /// <param name="vstsPat">VSTS personal access token</param>
        /// <param name="acceptedMediaType">Type of the accepted media.</param>
        /// <returns>
        /// The configured <see cref="HttpClient" /> object.
        /// </returns>
        public static HttpClient CreateVstsHttpClient(string vstsBaseUrl, string vstsPat, string acceptedMediaType = "application/json")
        {
            var c = new HttpClient { BaseAddress = new Uri(vstsBaseUrl) };

            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{vstsPat}")));
            c.DefaultRequestHeaders.Accept.Clear();
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedMediaType));

            return c;
        }
    }
}
