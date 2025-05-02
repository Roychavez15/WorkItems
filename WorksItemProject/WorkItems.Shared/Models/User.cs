using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WorkItems.Shared.Enums;

namespace WorkItems.Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }

        [JsonIgnore]
        public List<WorkItem> WorkItems { get; set; } = new();

        public int PendingItems => WorkItems.Count(w => !w.IsCompleted);
        public int CompletedItems => WorkItems.Count(w => w.IsCompleted);
        public bool IsSaturated => WorkItems.Count(w => w.Relevance == RelevanceLevel.High && !w.IsCompleted) > 3;
    }
}
