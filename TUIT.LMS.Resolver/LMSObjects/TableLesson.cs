using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class TableLesson
    {
        private static readonly Regex IsLectureRegex = new(@"\d\d\d$");
        private static readonly Regex IsLaboratoryRegex = new(@"\-\w\d$");
        private static readonly Regex SubjectRegex = new(@"\)[^-]+\-");
        private static readonly Regex StreamRegex = new(@"\D\D\D\d+-*\w*$");
        private static readonly Regex RoomRegex = new(@"\D-\d+");

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

            if (IsLectureRegex.IsMatch(title)) LessonType = LessonType.Lecture;
            else if (IsLaboratoryRegex.IsMatch(title)) LessonType = LessonType.Laboratory;
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
