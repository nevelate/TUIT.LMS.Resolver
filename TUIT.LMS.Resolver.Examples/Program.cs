using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUIT.LMS.Resolver;
using TUIT.LMS.Resolver.LMSObjects;

namespace TUIT.LMS.Resolver.Examples
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            LmsAuthService authService = new LmsAuthService();
            try
            {
                await authService.LoginAsync(Secrets.Login, Secrets.Password, Secrets.Token, Secrets.Grecaptcha);
            }
            catch(Exception e)
            {
                Console.WriteLine("ex");
                Console.WriteLine(e.Message);
            }
            LmsResolver resolver = new LmsResolver(authService);

            var information = await resolver.GetInformationAsync();

            PrintInformation(information);
        }

        static void PrintInformation(Information information)
        {
            Console.WriteLine($"{information.FullName} - {information.BirthDate}");
            Console.WriteLine($"{information.Specialization} - {information.StudyLanguage} - {information.TypeOfStudy}");
            Console.WriteLine(information.Group);
        }
    }
}
