using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Service.Src.Models
{
    public class UserProgress: BaseModel
    {
        public int UserId { get; set; }

        public int SubjectId { get; set; }
        
    }
}