using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Assignment
    {
        public string? Teacher { get; set; }

        public string? TaskName { get; set; }
        public LmsFile? TaskFile { get; set; }

        public DateTime Deadline { get; set; }

        public float? CurrentGrade { get; set; }
        public float MaxGrade { get; set; }

        public int? UploadId { get; set; }
        public LmsFile? UploadedFile { get; set; }

        public bool IsFailed { get; set; }
    }
}
