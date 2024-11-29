using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUIT.LMS.Resolver.JsonConverters;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Absence
    {
        public string? Subject { get; set; }

        [JsonProperty("subject_id")]
        public int SubjectId { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly Date { get; set; }

        [JsonProperty("type")]
        public string? LessonType { get; set; }

        [JsonProperty("calendar")]
        public string? ThemeTitle { get; set; }

        [JsonProperty("theme_number")]
        public int ThemeNumber { get; set; }
    }
}
