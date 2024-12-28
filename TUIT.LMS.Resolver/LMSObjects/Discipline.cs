using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class Discipline
    {
        public string? Title { get; set; }
        public string? Semester { get; set; }
        public int CreditCount { get; set; }
        public int? Grade { get; set; }
    }
}
