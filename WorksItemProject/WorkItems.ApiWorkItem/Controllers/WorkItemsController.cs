using Microsoft.AspNetCore.Mvc;
using WorkItems.Shared.Models;
using WorkItems.Shared.Enums;

namespace WorkItems.ApiWorkItem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkItemsController : ControllerBase
    {
        private static List<WorkItem> WorkItems = new();
        private static int CurrentId = 1;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WorkItemsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            if (!WorkItems.Any())
                SeedWorkItems().Wait();
        }

        [HttpGet]
        public ActionResult<List<WorkItem>> Get()
        {
            //return WorkItems.ToList();
            var ordered = WorkItems
                .OrderBy(w => w.AssignedUser?.Username)
                .ThenBy(w => w.DueDate)
                .ThenByDescending(w => w.Relevance)
                .ToList();

            return Ok(ordered);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkItem item)
        {
            item.Id = CurrentId++;
            await AssignWorkItem(item);
            WorkItems.Add(item);
            return Ok(item);
        }

        private async Task AssignWorkItem(WorkItem item)
        {
            string userServiceUrl = _configuration["UserService:BaseUrl"];

            if (string.IsNullOrEmpty(userServiceUrl))
            {
                item.AssignedUser = null;
                return;
            }

            var users = await _httpClient.GetFromJsonAsync<List<User>>(userServiceUrl);

            //obtiene los workitems pendientes
            var pendingCount = WorkItems
                    .Where(w => !w.IsCompleted && w.AssignedUser != null)
                    .GroupBy(w => w.AssignedUser.Username)
                    .ToDictionary(g => g.Key, g => g.Count());

            //obtiene los woritem con prioirida alta
            var highPriorityCount = WorkItems
                .Where(w => !w.IsCompleted && w.AssignedUser != null && w.Relevance == RelevanceLevel.High)
                .GroupBy(w => w.AssignedUser.Username)
                .ToDictionary(g => g.Key, g => g.Count());

            // los usuarios que pueden ser asiganados
            var eligibleUsers = users
                .Where(u => !highPriorityCount.TryGetValue(u.Username, out var count) || count < 3)
                .OrderBy(u => pendingCount.ContainsKey(u.Username) ? pendingCount[u.Username] : 0)
                .ToList();

            foreach (var user in eligibleUsers)
            {
                Console.WriteLine($"Usuario elegible: {user.Username}, HighCount: {(highPriorityCount.ContainsKey(user.Username) ? highPriorityCount[user.Username] : 0)}");
            }

            //sino hay no asigna
            if (!eligibleUsers.Any())
            {
                item.AssignedUser = null;
                return;
            }

            var daysUntilDue = (item.DueDate - DateTime.Today).TotalDays;

            if (daysUntilDue <= 3) //asigna por urgencia
            {
                var user = eligibleUsers.First();
                item.AssignedUser = user;
                user.WorkItems.Add(item);
            }
            else if (item.Relevance == RelevanceLevel.High) //asigna segun la relevancia
            {
                var user = eligibleUsers.First();
                item.AssignedUser = user;
                user.WorkItems.Add(item);
            }
            else //aun no se asigna si no es urgente ni relevante
            {
                //item.AssignedUser = null;
                var user = eligibleUsers.First();
                item.AssignedUser = user;
                user.WorkItems.Add(item);
            }
        }

        private async Task SeedWorkItems()
        {
            string userServiceUrl = _configuration["UserService:BaseUrl"];
            var users = await _httpClient.GetFromJsonAsync<List<User>>(userServiceUrl);

            var user1 = users.FirstOrDefault(u => u.Username == "user1");
            var user2 = users.FirstOrDefault(u => u.Username == "user2");
            var user3 = users.FirstOrDefault(u => u.Username == "user3");

            if (user1 != null)
            {
                var item1 = new WorkItem
                {
                    Id = CurrentId++,
                    Title = "Fix login bug",
                    DueDate = DateTime.Today.AddDays(2),
                    Relevance = RelevanceLevel.High,
                    AssignedUser = user1,
                    IsCompleted = false
                };
                WorkItems.Add(item1);
                //user1.WorkItems.Add(item1);
            }

            if (user2 != null)
            {
                var item2 = new WorkItem
                {
                    Id = CurrentId++,
                    Title = "Design homepage",
                    DueDate = DateTime.Today.AddDays(5),
                    Relevance = RelevanceLevel.Low,
                    AssignedUser = user2,
                    IsCompleted = false
                };
                WorkItems.Add(item2);
                //user2.WorkItems.Add(item2);
            }

            if (user3 != null)
            {
                var item3 = new WorkItem
                {
                    Id = CurrentId++,
                    Title = "Deploy API to production",
                    DueDate = DateTime.Today.AddDays(1),
                    Relevance = RelevanceLevel.High,
                    AssignedUser = user3,
                    IsCompleted = false
                };
                WorkItems.Add(item3);
                //user3.WorkItems.Add(item3);
            }
        }
    }
}
