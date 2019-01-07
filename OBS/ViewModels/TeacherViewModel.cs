using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBS.ViewModels
{
    public class TeacherViewModel
    {
        public int ID { get; set; }
        public string  TCNumber { get; set; }
        public string FullName{ get; set; }
        public DateTime? BirthDay { get; set; }
        public int? ProfessionId { get; set; }
        public string Profession{ get; set; }
        public string Picture { get; set; }
    }
}