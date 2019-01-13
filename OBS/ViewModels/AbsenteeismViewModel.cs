using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBS.ViewModels
{
    public class AbsenteeismViewModel
    {
        public int ID { get; set; }

        public string Student { get; set; }
        public string StudentNumber { get; set; }
        public int? StudentId { get; set; }

        public string Status { get; set; }

        public DateTime? Date { get; set; }

        public bool? IsFullDay { get; set; }

    }
}