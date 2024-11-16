using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace User_Service.Src.Models
{
    public class User : BaseModel
    {
         [StringLength(250)]
        public string Name { get; set; } = null!;

        [StringLength(250)]
        public string FirstLastName { get; set; } = null!;

        [StringLength(250)]
        public string SecondLastName { get; set; } = null!;

        [StringLength(250)]
        public string RUT { get; set; } = null!;

        [StringLength(250)]
        public string Email { get; set; } = null!;
        public bool IsEnabled { get; set; } = true;
        public int CareerId { get; set; }
        public int RoleId { get; set; }
    }
}