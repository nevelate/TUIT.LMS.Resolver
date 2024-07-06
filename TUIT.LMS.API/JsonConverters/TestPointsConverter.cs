using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.API.JsonConverters
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

            List<(string?, string?)> list = new List<(string?, string?)> ();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(new(row["number"].ToString(), row["point"].ToString()));
            }

            return list;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
