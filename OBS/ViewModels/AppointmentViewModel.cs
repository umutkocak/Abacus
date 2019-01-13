using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBS.ViewModels
{
    public class AppointmentViewModel
    {
        public int ID { get; set; }

        public string Teacher { get; set; }
        public int? TeacherId { get; set; }
        public string Student{ get; set; }
        public int? StudentId { get; set; }

        public string StudentMessage { get; set; }

        public string TeacherMessage { get; set; }

        public DateTime? Date { get; set; }

        public string Time { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}