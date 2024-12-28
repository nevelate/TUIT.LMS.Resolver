using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class TableLesson
    {
        private static Regex isLectureRegex = new(@"\d\d\d$");
        private static Regex isLaboratoryRegex = new(@"\-\w\d$");
        private static Regex SubjectRegex = new(@"\)\D+-");
        private static Regex StreamRegex = new(@"\D\D\D\d+-*\w*$");
        private static Regex RoomRegex = new(@"\D-\d+");

        public DateTime StartTime { get; set; }

        public string? Subject { get; set; }
        public string? Stream { get; set; }
        public string? Room { get; set; }

        public DayOfWeek LessonDay { get; set; }
        public TableLessonType TableLessonType { get; set; }

        public LessonType LessonType { get; set; }        
        
        public TableLesson()
        {
            
        }

        [JsonConstructor]
        public TableLesson(string title, int type, DateTime start)
        {
            title = title.Replace("/", "").Replace("\n", "");

            if (isLectureRegex.IsMatch(title)) LessonType = LessonType.Lecture;
            else if (isLaboratoryRegex.IsMatch(title)) LessonType = LessonType.Laboratory;
            else LessonType = LessonType.Practice;
            
            TableLessonType = (TableLessonType)type;
            StartTime = start;
            LessonDay = StartTime.DayOfWeek;                       
            Subject = SubjectRegex.Match(title).Value.TrimStart(')').TrimEnd('-');
            Stream = StreamRegex.Match(title).Value;
            Room = RoomRegex.Match(title).Value;
        }
    }    
}
