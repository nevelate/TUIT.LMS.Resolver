using Newtonsoft.Json;
using System.Text.RegularExpressions;
using TUIT.LMS.Resolver.JsonConverters;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Course
    {

        private static readonly Regex IsLectureRegex = new(@"\d\d\d$");
        private static readonly Regex IsLaboratoryRegex = new(@"\-\w\d$");

        public int Id { get; set; }

        public string? Subject { get; set; }

        [JsonProperty("subject_id")]
        public int SubjectId { get; set; }

        [JsonProperty("attendance")]
        public int AbsenceCount { get; set; }
        
        public List<string> Streams { get; set; } = null!;

        public List<LessonType> LessonTypes { get; set; } = null!;

        [JsonConverter(typeof(StringToListConverter))]
        public List<string> Teachers { get; set; } = null!;

        [JsonProperty("failed")]
        public bool IsFailed { get; set; }

        public Course()
        {
            
        }

        [JsonConstructor]
        public Course(string streams)
        {
            Streams = streams.Split("###").ToList();

            LessonTypes = new List<LessonType>();

            foreach (var stream in Streams) 
            {
                if (IsLectureRegex.IsMatch(stream)) LessonTypes.Add(LessonType.Lecture);
                else if (IsLaboratoryRegex.IsMatch(stream)) LessonTypes.Add(LessonType.Laboratory);
                else LessonTypes.Add(LessonType.Practice);
            }
        }
    }
}
