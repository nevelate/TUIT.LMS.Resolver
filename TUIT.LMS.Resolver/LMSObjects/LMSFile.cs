namespace TUIT.LMS.Resolver.LMSObjects
{
    public class LmsFile
    {
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }

        public LmsFile()
        {

        }

        public LmsFile(string fileName, string fileUrl)
        {
            FileName = fileName;
            FileUrl = fileUrl;
        }
    }
}
