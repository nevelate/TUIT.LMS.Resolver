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
}
