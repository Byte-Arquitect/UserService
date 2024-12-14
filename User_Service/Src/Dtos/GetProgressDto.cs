using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Service.Src.Dtos
{
    public class GetProgressDto
    {
        public string Subject_code { get; set; }
        public string SubjectName { get; set; } = null!;
    }
}
