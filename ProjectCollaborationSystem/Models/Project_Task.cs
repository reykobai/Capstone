using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ProjectCollaborationSystem.Models
{
    public class Project_Task
    {
        public List<Project> projects { get; set; }
        public List<Task> tasks { get; set; }

        public List<Subtask> subtasks { get; set; }
      
    }
}