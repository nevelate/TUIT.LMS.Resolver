﻿namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Information
    {
        public string? FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? Gender { get; set; }

        public string? StudentNumber { get; set; }
        public string? Group { get; set; }
        public string? Tutor { get; set; }

        public string? Address { get; set; }
        public string? TemporaryAddress { get; set; }

        public string? Base64Photo { get; set; }

        public string? Specialization { get; set; }
        public string? StudyLanguage { get; set; }
        public string? Degree { get; set; }
        public string? TypeOfStudy { get; set; }
        public int Year { get; set; }

        public string? Stipend { get; set; }
    }
}
