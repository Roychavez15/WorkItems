using System.Net.Http.Json;
using WorkItems.Shared.Models;

namespace WorkItems.FrontEnd.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        public UserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            //_apiUrl = _configuration["UserService:BaseUrl"];
            _apiUrl = "https://localhost:7186/api/users";
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>(_apiUrl);
        }

        public async Task CreateUserAsync(User user)
        {
            await _httpClient.PostAsJsonAsync(_apiUrl, user);
        }
    }
}
