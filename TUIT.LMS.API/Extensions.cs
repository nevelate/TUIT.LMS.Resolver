using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TUIT.LMS.API
{
    internal static class Extensions
    {
        private static HtmlParser _parser;

        static Extensions()
        {
            _parser = new HtmlParser();
        }

        public static async Task<IDocument> GetHTMLAsync(this HttpClient httpClient, string url)
        {
            var responseAsString = await httpClient.GetStringAsync(url);
            return await _parser.ParseDocumentAsync(responseAsString);
        }

        public static string RemoveUpToColonAndTrim(this string s)
        {
            int colonIndex = s.IndexOf(":");
            s = s.Remove(0, colonIndex + 1);
            return s.Trim('\n', ' ', '\t');
        }
    }
}
