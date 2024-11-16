using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Service.Src.Dtos
{
    public class RegisterResponseDto
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;

        public string FirstLastName { get; set; } = null!;

        public string SecondLastName { get; set; } = null!;

        public string RUT { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string RoleName { get; set; } = null!;

        public string CareerName { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}