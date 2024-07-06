using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUIT.LMS.API.JsonConverters;

namespace TUIT.LMS.API.LMSObjects
{
    public class Course
    {
        public int Id { get; set; }

        public string? Subject { get; set; }

        [JsonProperty("subject_id")]
        public int SubjectId { get; set; }

        [JsonProperty("attendance")]
        public int AbsenceCount { get; set; }

        [JsonConverter(typeof(StringToListConverter))]
        public List<string> Streams { get; set; } = null!;

        [JsonConverter(typeof(StringToListConverter))]
        public List<string> Teachers { get; set; } = null!;

        [JsonProperty("failed")]
        public bool IsFailed { get; set; }
    }
}
