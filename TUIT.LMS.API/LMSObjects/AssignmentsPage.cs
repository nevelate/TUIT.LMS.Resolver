using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.API.LMSObjects
{
    public class AssignmentsPage
    {
        public float AchievedPoints { get; set; }
        public float MaxPoints { get; set; }
        public float Rating { get; set; }
        public int Grade { get; set; }

        public List<Assignment> Assignments { get; set; } = null!;
    }

    public class Assignment
    {
        public string? Teacher { get; set; }

        public string? TaskName { get; set; }
        public LMSFile? TaskFile { get; set; }

        public DateTime Deadline { get; set; }

        public float? CurrentGrade { get; set; }
        public float MaxGrade { get; set; }

        public int? UploadId { get; set; }
        public LMSFile? UploadedFile { get; set; }
    }
}
