using GameVault.Source.Application.Enums;

namespace GameVault.Source.Application.Dtos.Game.AdditionalDtos
{
    /// <summary>Plataforma concreta (ej: "PlayStation 5") y su familia.</summary>
    public sealed class PlatformDto
    {
        public string Name { get; set; } = string.Empty;

        public PlatformFamily Family { get; set; }
    }
}
