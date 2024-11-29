using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUIT.LMS.Resolver.LMSObjects;

namespace TUIT.LMS.Resolver.JsonConverters
{
    public class TableLessonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TableLesson);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            DateTime startTime = DateTime.Parse(jo["StartTime"].ToString());
            string subject = jo["Subject"].ToString();
            string stream = jo["Stream"].ToString();
            string room = jo["Room"].ToString();
            DayOfWeek lessonDay = startTime.DayOfWeek;
            TableLessonType tableLessonType = (TableLessonType)int.Parse(jo["TableLessonType"].ToString());
            LessonType lessonType = (LessonType)int.Parse(jo["LessonType"].ToString());

            return new TableLesson()
            {
                StartTime = startTime,
                Subject = subject,
                Stream = stream,
                Room = room,
                LessonDay = lessonDay,
                TableLessonType = tableLessonType,
                LessonType = lessonType
            };
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
