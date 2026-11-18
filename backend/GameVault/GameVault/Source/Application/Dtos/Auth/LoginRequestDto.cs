using System.ComponentModel.DataAnnotations;

namespace GameVault.Source.Application.Dtos.Auth
{
    public class LoginRequestDto
    {
        [Required (ErrorMessage ="El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Email {  get; set; }= string.Empty;

        [Required (ErrorMessage ="La contraseña es obligatorio.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;

    }
}
