using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUIT.LMS.Resolver.LMSObjects;

namespace TUIT.LMS.Resolver.Examples
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            LMSAuthService authService = new LMSAuthService();
            try
            {
                await authService.LoginAsync(Secrets.Login, Secrets.Password, Secrets.Token, Secrets.Grecaptcha);
            }
            catch(Exception e)
            {
                Console.WriteLine("ex");
                Console.WriteLine(e.Message);
            }
            LMSResolver resolver = new LMSResolver(authService);

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
