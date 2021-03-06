//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoScheduler.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Task
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Task()
        {
            this.CalendarEvents = new HashSet<CalendarEvent>();
        }
    
        public int taskID { get; set; }
        public Nullable<int> userID { get; set; }
        public string name { get; set; }
        public string notes { get; set; }
        public Nullable<int> priority { get; set; }
        public Nullable<System.DateTime> dueDate { get; set; }
        public Nullable<int> estCompletionMinutes { get; set; }
        public Nullable<int> categoryID { get; set; }
        public Nullable<bool> isAutoSchedule { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; }
        public virtual User User { get; set; }
    }
}
