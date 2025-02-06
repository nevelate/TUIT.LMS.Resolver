using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TUIT.LMS.Resolver.JsonConverters;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Course
    {

        private static readonly Regex isLectureRegex = new(@"\d\d\d$");
        private static readonly Regex isLaboratoryRegex = new(@"\-\w\d$");

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
                if (isLectureRegex.IsMatch(stream)) LessonTypes.Add(LessonType.Lecture);
                else if (isLaboratoryRegex.IsMatch(stream)) LessonTypes.Add(LessonType.Laboratory);
                else LessonTypes.Add(LessonType.Practice);
            }
        }
    }
}
