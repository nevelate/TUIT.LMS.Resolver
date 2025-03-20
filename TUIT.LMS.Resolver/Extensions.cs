namespace TUIT.LMS.Resolver
{
    internal static class Extensions
    {
        public static string RemoveUpToColonAndTrim(this string s)
        {
            var colonIndex = s.IndexOf(":");
            s = s.Remove(0, colonIndex + 1);
            return s.Trim('\n', ' ', '\t');
        }

        public static string RemoveFileExtension(this string s)
        {
            var dotIndex = s.LastIndexOf('.');
            return s.Remove(dotIndex);
        }

        public static float? ParseOrReturnNull(this string s)
        {
            float result = 0;
            if (float.TryParse(s.Replace('.', ','), out result))
            {
                return result;
            }
            return null;
        }
    }
}
