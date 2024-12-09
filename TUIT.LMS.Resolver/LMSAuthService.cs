using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace TUIT.LMS.Resolver
{
    public class LMSAuthService
    {
        private HttpClient _httpClient;
        private HttpClientHandler _httpClientHandler;

        private CookieContainer _cookieContainer;

        private HtmlParser _htmlParser;

        private readonly Dictionary<string, string> loginRequestHeaders;

        private const string PostRequestFormat = "_token={0}&login={1}&password={2}&g-recaptcha-response={3}";

        public event Action? LoginRequested;

        public LMSAuthService()
        {
            _cookieContainer = new CookieContainer();
            _httpClientHandler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
            };
            _httpClient = new HttpClient(_httpClientHandler);

            _htmlParser = new HtmlParser();

            loginRequestHeaders = new()
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

        public async Task<bool> TryLoginAsync(string login, string password, string token, string grecaptcha)
        {
            var content = new StringContent(string.Format(PostRequestFormat, token, login, password, grecaptcha));
            content.Headers.ContentType = new("application/x-www-form-urlencoded");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://lms.tuit.uz/auth/login")
            {
                Content = content,
            };

            foreach (var pair in loginRequestHeaders)
            {
                request.Headers.Add(pair.Key, pair.Value);
            }

            var response = await _httpClient.SendAsync(request);
            var responseAsString = await response.Content.ReadAsStringAsync();

            return responseAsString.Contains("Dashboard");
        }

        public void LogOut()
        {
            _httpClientHandler.Dispose();
            _httpClient.Dispose();

            _cookieContainer = new CookieContainer();

            _httpClientHandler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
            };
            _httpClient = new HttpClient(_httpClientHandler);
        }

        public async Task CheckIfNeededReLogin()
        {
            var response = await _httpClient.GetStringAsync("https://lms.tuit.uz");
            if (!response.Contains("Dashboard")) LoginRequested?.Invoke();
        }

        public async Task<IDocument> GetHTMLAsync(string? requestUri)
        {            
            var responseAsString = await _httpClient.GetStringAsync(requestUri);
            return await _htmlParser.ParseDocumentAsync(responseAsString);
        }

        public async Task<string> GetStringAsync(string? requestUri)
        {
            return await _httpClient.GetStringAsync(requestUri);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _httpClient.SendAsync(request);
        }
    }
}
