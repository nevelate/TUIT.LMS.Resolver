﻿using Newtonsoft.Json;

namespace TUIT.LMS.Resolver.JsonConverters
{
    internal class StringToListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var data = reader.Value as string;
            List<string> values = data.Split("###").ToList();
            return values;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
