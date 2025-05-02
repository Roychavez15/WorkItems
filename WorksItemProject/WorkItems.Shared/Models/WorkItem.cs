using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkItems.Shared.Enums;

namespace WorkItems.Shared.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public RelevanceLevel Relevance { get; set; }
        public User? AssignedUser { get; set; }
        public bool IsCompleted { get; set; }
    }
}
