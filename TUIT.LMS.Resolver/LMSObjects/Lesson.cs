using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Lesson
    {
        public string? ThemeTitle { get; set; }
        public int ThemeNumber { get; set; }
        public DateOnly LessonDate { get; set; }
        public LessonType LessonType { get; set; }

        public List<LMSFile> Attachments { get; set; } = null!;
    }    
}
