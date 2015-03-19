using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCollaborationSystem.Models
{
    public class History_Information
    {

        public List<History> History { get; set; }
        public List<Information> Information { get; set; }

        public History history { get; set; }
        public Information information { get; set; }

    }
}