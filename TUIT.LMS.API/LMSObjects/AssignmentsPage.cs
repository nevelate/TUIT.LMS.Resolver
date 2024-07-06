using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.API.LMSObjects
{
    public class AssignmentsPage
    {
        public int AchievedPoints { get; set; }
        public int MaxPoints { get; set; }
        public int Rating { get; set; }
        public int Grade { get; set; }

        public List<Assignment> Assignments { get; set; } = null!;
    }

    public class Assignment
    {
        public string? Teacher { get; set; }

        public string? TaskName { get; set; }
        public string? TaskUrl { get; set; }

        public DateTime Deadline { get; set; }

        public int? CurrentGrade { get; set; }
        public int MaxGrade { get; set; }

        public int? UploadId { get; set; }
        public string? UploadedFileName { get; set; }
        public string? UploadedFileUrl { get; set; }
    }
}
