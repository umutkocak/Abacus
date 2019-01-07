using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBS.ViewModels
{
    public class UserViewModel
    {
        public int? ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? TeacherId { get; set; }

        public string Teacher { get; set; }
        public string TeacherImage { get; set; }
        public string Password { get; set; }
        public int? StudentId { get; set; }
        public string Student { get; set; }
        public string StudentImage { get; set; }


    }
}