using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ProjectCollaborationSystem.Models
{
    public class Team_Information
    {

        public Information inform { get; set; }
        public List<Information> infoList { get; set; }
        public Team teams { get; set; }
        public List<Team> teamList { get; set; }

        public Project project { get; set; }
        public List<Project> projectList { get; set; }

        public Task tasks { get; set; }

    }
}