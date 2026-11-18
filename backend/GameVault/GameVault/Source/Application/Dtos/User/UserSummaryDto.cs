namespace GameVault.Source.Application.Dtos.User
{
    /// <summary>Datos públicos mínimos de un usuario (autor de una reseña, dueño de una lista...).</summary>
    public sealed class UserSummaryDto
    {
        public string UserName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }
    }
}
