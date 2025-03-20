namespace TUIT.LMS.Resolver.LMSObjects
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
        public LmsFile? TaskFile { get; set; }

        public DateTime Deadline { get; set; }

        public float? CurrentGrade { get; set; }
        public float MaxGrade { get; set; }

        public int? UploadId { get; set; }
        public LmsFile? UploadedFile { get; set; }

        public bool IsFailed { get; set; }
    }
}
