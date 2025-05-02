using System.Net.Http.Json;
using WorkItems.Shared.Models;

namespace WorkItems.FrontEnd.Services
{
    public class WorkItemService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        public WorkItemService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            //_apiUrl = _configuration["WorkItemService:BaseUrl"];
            _apiUrl = "https://localhost:7198/api/workitems";
        }

        public async Task<List<WorkItem>> GetWorkItemsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<WorkItem>>(_apiUrl);
        }

        public async Task CreateWorkItemAsync(WorkItem item)
        {
            await _httpClient.PostAsJsonAsync(_apiUrl, item);
        }
    }
}
