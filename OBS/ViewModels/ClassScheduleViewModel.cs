using System.Collections.Generic;

namespace OBS.ViewModels
{
    public class ClassScheduleViewModel
    {
        public int ID { get; set; }
        public int? ClassId { get; set; }
        public string Class { get; set; }
      
        public ScheduleViewModel Schedule { get; set; }

    }

    public class ScheduleViewModel
    {
        public int? LessonId { get; set; }
        public string Lesson { get; set; }
        public string Day { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Color { get; set; }
    }
}