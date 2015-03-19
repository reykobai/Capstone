using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ProjectCollaborationSystem.Models
{
    public class Task_information
    {
       
        public List<Task> task { get; set; }
        public List<Subtask> subtask { get; set; }
        public List<Information> informations { get; set; }

        public List<Project> proj { get; set; }
        public List<Project> proje { get; set; }
        public List<Project> projec { get; set; }

        public List<Task> tas { get; set; }
        public List<Task> ta { get; set; }
        public List<Task> t { get; set; }

        public List<Subtask> subtas { get; set; }
        public List<Subtask> subta { get; set; }
        public List<Subtask> subt { get; set; }

        public Information info { get; set; }
        public Task tasked { get; set; }

        public Subtask subtasked { get; set; }
      
    }
}