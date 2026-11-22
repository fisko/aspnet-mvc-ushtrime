using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UniversityMvcClient.Services
{
    public static class ApiClient
    {
        // Një HttpClient i vetëm për gjithë aplikacionin
        private static readonly HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5050/")
        };

        // GET: kthen objektin, ose null nëse API kthen gabim (p.sh. 404)
        public static async Task<T> GetAsync<T>(string url)
        {
            var response = await Client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return default(T);
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        // POST: dërgon objektin si JSON
        public static async Task<bool> PostAsync<T>(string url, T model)
        {
            var response = await Client.PostAsync(url, ToJson(model));
            return response.IsSuccessStatusCode;
        }

        // PUT: përditëson objektin
        public static async Task<bool> PutAsync<T>(string url, T model)
        {
            var response = await Client.PutAsync(url, ToJson(model));
            return response.IsSuccessStatusCode;
        }

        // DELETE: fshin objektin
        public static async Task<bool> DeleteAsync(string url)
        {
            var response = await Client.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        private static StringContent ToJson<T>(T model)
        {
            return new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        }
    }
}
