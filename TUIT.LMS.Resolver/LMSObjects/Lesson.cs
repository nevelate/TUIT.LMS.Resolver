namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Lesson
    {
        public string? ThemeTitle { get; set; }
        public int ThemeNumber { get; set; }
        public DateOnly LessonDate { get; set; }
        public LessonType LessonType { get; set; }

        public List<LmsFile> Attachments { get; set; } = null!;
    }    
}
