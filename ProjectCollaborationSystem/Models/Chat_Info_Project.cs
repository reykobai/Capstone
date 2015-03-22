using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCollaborationSystem.Models
{
    public class Chat_Info_Project
    {
        public Information info = new Information();
        public Chat chat = new Chat();
        public Project project = new Project();

        List<Information> infos = new List<Information>();
        List<Chat> chats = new List<Chat>();
        List<Project> projects = new List<Project>();
    }
}