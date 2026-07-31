using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Auth
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; }= string.Empty;

        [Required(ErrorMessage = "El nombre de usuario obligatorio.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Email {  get; set; }= string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]

        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confimar la contraseña.")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]

        public string ConfirmPassword { get; set; } = string.Empty;



    }
}
