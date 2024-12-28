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
            await authService.TryLoginAsync(Secrets.Login, Secrets.Password, Secrets.Token, Secrets.Grecaptcha);
            LMSResolver resolver = new LMSResolver(authService);

            var data = await resolver.GetInformationAsync();

            Console.WriteLine("End");
        }
    }
}
