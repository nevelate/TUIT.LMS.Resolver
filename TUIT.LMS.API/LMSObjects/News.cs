using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.API.LMSObjects
{
    public class News
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly NewsDate { get; set; }
    }
}
