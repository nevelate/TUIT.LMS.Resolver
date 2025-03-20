using Newtonsoft.Json;
using System.Globalization;

namespace TUIT.LMS.Resolver.JsonConverters
{
    internal class DateOnlyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateOnly);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var data = reader.Value as string;
            var date = DateOnly.Parse(data, new CultureInfo("ru-RU"));
            return date;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
