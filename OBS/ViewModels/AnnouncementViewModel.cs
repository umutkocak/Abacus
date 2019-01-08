using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBS.ViewModels
{
    public class AnnouncementViewModel
    {

        public int ID { get; set; }

        public string Title { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public bool? AllUser { get; set; }

        public DateTime? Date { get; set; }

        public int? StudentId { get; set; }

        public int? ClassId { get; set; }

        public int? LessonId { get; set; }
        public int? CreatedByID { get; set; }
        public string CreatedBy { get; set; }
    }

    public class AnnouncementListViewModel
    {

        public int ID { get; set; }

        public string Title { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public bool? AllUser { get; set; }

        public DateTime? Date { get; set; }

        public string Student { get; set; }
        public int? StudentId { get; set; }

        public string Class { get; set; }
        public int? ClassId { get; set; }

        public string Lesson { get; set; }
        public int? LessonId { get; set; }
        public int? CreatedByID { get; set; }
        public string CreatedBy { get; set; }
    }
}