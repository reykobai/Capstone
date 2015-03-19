//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectCollaborationSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Task
    {
        public Task()
        {
            this.Comments = new HashSet<Comment>();
            this.Subtasks = new HashSet<Subtask>();
            this.Histories = new HashSet<History>();
        }
    
        public int TaskID { get; set; }
        public string TaskTitle { get; set; }
        public System.DateTime DueDate { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStatus { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public string TaskPriority { get; set; }
        public int InformationID { get; set; }
        public int ProjectID { get; set; }
        public string FileUploaded { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Information Information { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<Subtask> Subtasks { get; set; }
        public virtual ICollection<History> Histories { get; set; }
    }
}
