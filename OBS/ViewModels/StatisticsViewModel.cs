using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBS.ViewModels
{
    public class StatisticsViewModel
    {
        public int? StudentCount { get; set; }
        public int? TeacherCount { get; set; }
        public int? LessonCount { get; set; }
        public int? ClassCount { get; set; }
        public int? AnnouncementCount { get; set; }
        public int? HomeWorkCount { get; set; }
    }
}