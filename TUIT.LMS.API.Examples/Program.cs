using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUIT.LMS.API.LMSObjects;

namespace TUIT.LMS.API.Examples
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            /*
            JObject jObject = JObject.Parse(data);
            List<JToken> results = jObject["data"].Children().ToList();

            List<Course> courses = new List<Course>();

            foreach (var item in results)
            {
                courses.Add(item.ToObject<Course>());
            }

            JObject jObject = JObject.Parse(data);
            List<JToken> results = jObject["data"].Children().ToList();

            List<Final> finals = new List<Final>();

            foreach (var item in results)
            {
                finals.Add(item.ToObject<Final>());
            }

            JObject jObject = JObject.Parse(data);
            List<JToken> results = jObject["data"].Children().ToList();

            List<Absence> list = new List<Absence>();

            foreach (var item in results)
            {
                list.Add(item.ToObject<Absence>());
            }
            */

            LMSAuthService authService = new LMSAuthService();
            await authService.TryLoginAsync(Secrets.Login, Secrets.Password, Secrets.Token, Secrets.Grecaptcha);
            LMSResolver resolver = new LMSResolver(authService);

            var data = await resolver.GetNewsAsync(2);

            Console.WriteLine("End");
        }
    }
}
