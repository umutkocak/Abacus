using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBS.ViewModels
{

    public class ClassViewModel
    {
        public int ID { get; set; }
        public string ClassName { get; set; }
        public int? TeacherId { get; set; }
        public string Teacher { get; set; }

        public List<LessonsViewModel> Lessons { get; set; }
        public List<StudentsViewModel> Students { get; set; }

    }

    public class LessonsViewModel
    {
        public int? ID { get; set; }
        public string LessonName { get; set; }
    }

    public class StudentsViewModel
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string TCNumber { get; set; }
        public string FullName { get; set; }
        public string StudentNumber { get; set; }
        public DateTime? Birthday { get; set; }
    }

    public class StudentViewModel
    {
        public StudentsViewModel Student { get; set; }
        public int? ClassId { get; set; }
        public string Class { get; set; }
    }

 

}