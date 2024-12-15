using System.ComponentModel.DataAnnotations;
using User_Service.Src.Common.Constants;
using User_Service.Src.DataAnnotations;

namespace User_Service.Src.DTOs.Auth
{
    public class RegisterStudentDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string FirstLastName { get; set; } = null!;

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string SecondLastName { get; set; } = null!;

        [Required]
        [Rut]
        public string RUT { get; set; } = null!;

        [Required]
        [UCNEmailAddress("El correo electronico debe ser con dominio de la universidad UCN")]
        public string Email { get; set; } = null!;

        [Required]
        public string CareerName { get; set; } = null!;

        [Required]
        [StringLength(16, MinimumLength = 10)]
        [RegularExpression(
            RegularExpressions.PasswordValidation,
            ErrorMessage = "La contraseña debe de tener un numero y una letra como minimo y tener una longitud de 10 a 16 caracteres"
        )]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password))]
        public string RepeatedPassword { get; set; } = null!;
    }
}
