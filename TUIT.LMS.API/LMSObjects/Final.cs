﻿using Newtonsoft.Json;
using TUIT.LMS.API.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TUIT.LMS.API.LMSObjects
{
    public class Final
    {
        public string? Subject { get; set; }

        [JsonProperty("subject_id")]
        public int SubjectId { get; set; }

        [JsonProperty("f_grade")]
        public int Grade { get; set; }

        public string? Stream { get; set; }
        public string? Room { get; set; }

        [JsonProperty("date")]
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly ExamDate { get; set; }
        
        [JsonProperty("from")]
        //[JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly StartTime { get; set; }
        
        [JsonProperty("to")]
        //[JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly EndTime { get; set; }

        [JsonProperty("final_limit")]
        public int TimeLimit { get; set; }

        [JsonProperty("final_info")]
        [JsonConverter(typeof(TestPointsConverter))]
        public List<(string?, string?)> TestPoints { get; set; } = null!;
    }
}