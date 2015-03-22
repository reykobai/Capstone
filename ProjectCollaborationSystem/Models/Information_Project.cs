using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ProjectCollaborationSystem.Models
{
    public class Information_Project
    {
     
        public Information info { get; set; }
        public Project project { get; set; }
        public Subtask subtask { get; set; }
        public List<Subtask> subtasking { get; set; }
        public Task tasking { get; set; }
        public List<Task> task { get; set; }
        public List<Information> information { get; set; }
        public List<Comment_Information> cinfo { get; set; }

        public List<Task_information> taskinfo { get; set; }
        public List<Subtask_Information> subinfo { get; set; }

        public List<Team_Information> teaminfo { get; set; }

        public List<Project> projects { get; set; }
        public List<Team> teams { get; set; }
        public Team teaming { get; set; }

        public List<History> historys { get; set; }

        public List<History_Information> Hist_inf { get; set; }
        public List<Chat_Info_Project> cip = new List<Chat_Info_Project>();
    }
}