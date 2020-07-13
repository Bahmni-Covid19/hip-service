using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public class OpenMrsClient
    {
        private readonly HttpClient httpClient;
        private readonly OpenMrsConfiguration configuration;

        public OpenMrsClient(HttpClient httpClient, OpenMrsConfiguration openmrsConfiguration)
        {
            this.httpClient = httpClient;
            configuration = openmrsConfiguration;
        }

        public async Task<String> GetAsync(string openmrsUrl)
        {
            var authToken = Encoding.ASCII.GetBytes($"{configuration.Username}:{configuration.Password}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            var response = await httpClient.GetAsync(configuration.Url + openmrsUrl);

            return await response.Content.ReadAsStringAsync();
        }
    }
}