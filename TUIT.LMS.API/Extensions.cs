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
        public static string RemoveUpToColonAndTrim(this string s)
        {
            int colonIndex = s.IndexOf(":");
            s = s.Remove(0, colonIndex + 1);
            return s.Trim('\n', ' ', '\t');
        }

        public static int? ParseOrReturnNull(this string s)
        {
            int result = 0;
            if (int.TryParse(s, out result))
            {
                return result;
            }
            return null;
        }
    }
}
