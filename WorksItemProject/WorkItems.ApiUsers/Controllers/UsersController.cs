using Microsoft.AspNetCore.Mvc;
using WorkItems.Shared.Enums;
using WorkItems.Shared.Models;

namespace WorkItems.ApiUsers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static List<User> Users = new();

        //constructor para incializar usarios por defecto para ejemplos
        public UsersController()
        {            
            if (Users.Count == 0) //eviatr que se dupliquen
            {
                Users.AddRange(new List<User>
                {
                    new User {Id=1, Username = "user1", FullName = "User 1" },
                    new User { Id=2, Username = "user2", FullName = "User 2" },
                    new User { Id=3, Username = "user3", FullName = "User 3" }
                });
            }
        }

        // GET api/users
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(Users);
        }

        //si se desea obtner las estadistcas del usaurio
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            return Ok(Users.Select(u => new {
                u.Username,
                PendingItems = u.WorkItems.Count(w => !w.IsCompleted),
                IsSaturated = u.WorkItems.Count(w => w.Relevance == RelevanceLevel.High && !w.IsCompleted) > 3
            }));
        }

        // POST api/users
        [HttpPost]
        public IActionResult Create(User user)
        {
            if (Users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Username already exists.");
            }

            Users.Add(user);
            return Ok(user);
        }

        // PUT api/users/{username}
        [HttpPut("{username}")]
        public IActionResult Update(string username, User updatedUser)
        {
            var user = Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return NotFound("User not found.");
            }
            user.FullName = updatedUser.FullName;
            return Ok(user);
        }

        // DELETE api/users/{username}
        [HttpDelete("{username}")]
        public IActionResult Delete(string username)
        {
            var user = Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return NotFound("User not found.");
            }

            Users.Remove(user);
            return NoContent();
        }

        public static List<User> GetUsers() => Users;
    }
}
