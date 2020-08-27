using System;

namespace PertEstimationTool.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Assessment Assessments { get; set; }
    }
}
