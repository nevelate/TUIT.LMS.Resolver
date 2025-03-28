﻿using Newtonsoft.Json;
using TUIT.LMS.Resolver.JsonConverters;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Final
    {
        public string? Subject { get; set; }

        [JsonProperty("subject_id")]
        public int SubjectId { get; set; }

        [JsonProperty("f_grade")]
        [JsonConverter(typeof(FinalGradeConverter))]
        public int? Grade { get; set; }

        public string? Stream { get; set; }
        public string? Room { get; set; }

        [JsonProperty("date")]
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly ExamDate { get; set; }
        
        [JsonProperty("from")]        
        public TimeOnly StartTime { get; set; }
        
        [JsonProperty("to")]        
        public TimeOnly EndTime { get; set; }

        [JsonProperty("final_limit")]
        public int TimeLimit { get; set; }

        [JsonProperty("final_info")]
        [JsonConverter(typeof(TestPointsConverter))]
        public List<TestPoint> TestPoints { get; set; }
    }    
}
