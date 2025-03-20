using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var jo = JObject.Load(reader);
            var startTime = DateTime.Parse(jo["StartTime"].ToString());
            var subject = jo["Subject"].ToString();
            var stream = jo["Stream"].ToString();
            var room = jo["Room"].ToString();
            var lessonDay = startTime.DayOfWeek;
            var tableLessonType = (TableLessonType)int.Parse(jo["TableLessonType"].ToString());
            var lessonType = (LessonType)int.Parse(jo["LessonType"].ToString());

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
