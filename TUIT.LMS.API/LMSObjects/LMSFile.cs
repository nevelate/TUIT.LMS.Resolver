using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.Resolver.LMSObjects
{
    public class LMSFile
    {
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }

        public LMSFile()
        {

        }

        public LMSFile(string fileName, string fileUrl)
        {
            FileName = fileName;
            FileUrl = fileUrl;
        }
    }
}
