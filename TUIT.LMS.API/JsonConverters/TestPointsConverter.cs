using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUIT.LMS.Resolver.LMSObjects;

namespace TUIT.LMS.Resolver.JsonConverters
{
    internal class TestPointsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<(string?, string?)>);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            string data = reader.Value as string;
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(data);

            List<TestPoint> list = new List<TestPoint> ();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(new()
                {
                    TestNumber = (int)(long)row["number"],
                    Point = (int)(long)row["point"]
                });
            }

            return list;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
