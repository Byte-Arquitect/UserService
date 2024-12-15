using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Service.Src.Models
{
    public class Subject
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public int Credits { get; set; }
        public int Semester { get; set; }
    }

    public class SubjectsResponse
    {
        public List<Subject> Subjects { get; set; }
    }
}
