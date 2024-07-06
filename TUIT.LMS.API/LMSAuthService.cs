using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace TUIT.LMS.API
{
    public class LMSAuthService
    {
        public HttpClient HttpClient { get; private set; }
        private HttpClientHandler _httpClientHandler;

        private CookieContainer _cookieContainer;

        private static readonly Dictionary<string, string> headers;

        private const string PostRequestFormat = "_token={0}&login={1}&password={2}&g-recaptcha-response={3}";

        static LMSAuthService()
        {
            headers = new()
                {
                    {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"},
                    {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36"},
                    {"Origin", "https://lms.tuit.uz"},
                    {"sec-ch-ua", "\"Not/A)Brand\";v=\"8\", \"Chromium\";v=\"126\", \"Google Chrome\";v=\"126\""},
                    {"sec-ch-ua-mobile", "?0"},
                    {"sec-ch-ua-platform", "\"Windows\""},
                    {"Sec-Fetch-Dest", "document"},
                    {"Sec-Fetch-Mode", "navigate"},
                    {"Sec-Fetch-Site", "same-origin"},
                    {"Sec-Fetch-User", "?1"},
                    {"Upgrade-Insecure-Requests", "1"},
                    {"Host", "lms.tuit.uz"},
                };
        }

        public LMSAuthService()
        {
            _cookieContainer = new CookieContainer();
            _httpClientHandler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
            };
            HttpClient = new HttpClient(_httpClientHandler);
        }

        public async Task<bool> TryLoginAsync(string login, string password, string token, string grecaptcha)
        {
            var content = new StringContent(string.Format(PostRequestFormat, token, login, password, grecaptcha));
            content.Headers.ContentType = new("application/x-www-form-urlencoded");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://lms.tuit.uz/auth/login")
            {
                Content = content,
            };

            foreach (var pair in headers)
            {
                request.Headers.Add(pair.Key, pair.Value);
            }

            var response = await HttpClient.SendAsync(request);
            var responseAsString = await response.Content.ReadAsStringAsync();

            return responseAsString.Contains("Dashboard");
        }

        public bool CheckIfNeededReLoginAsync()
        {
            return _cookieContainer.GetCookies(new Uri("https://lms.tuit.uz")).Any(c => c.Expired);
        }
    }
}
