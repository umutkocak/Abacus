using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBS.ViewModels
{
    public class ExamViewModel
    {

        public int? ID { get; set; }

        public double? FirstNote { get; set; }

        public double? SecondNote { get; set; }

        public double? ThirdNote { get; set; }

        public double? ProjectNote { get; set; }

        public double? VerbalNote { get; set; }

        public string Class { get; set; }
        public int? ClassId { get; set; }

        public string Student { get; set; }
        public int? StudentId { get; set; }

        public string Lesson { get; set; }
        public int? LessonId { get; set; }

        public int? Period { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}