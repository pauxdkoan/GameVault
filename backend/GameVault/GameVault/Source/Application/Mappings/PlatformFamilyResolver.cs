using GameVault.Source.Application.Dtos.Game.AdditionalDtos;
using GameVault.Source.Application.Dtos.Igdb.AdttionalDtos;
using GameVault.Source.Application.Enums;

namespace GameVault.Source.Application.Mappings
{
    /// <summary>
    /// Decide a qué familia pertenece una plataforma de IGDB a partir de su nombre.
    /// Antes estas reglas estaban dentro de GameMapper y devolvían texto;
    /// ahora devuelven un enum y se reutilizan en tarjetas y detalle.
    /// </summary>
    internal static class PlatformFamilyResolver
    {
        // El orden importa: "PlayStation VR" debe caer en PlayStation, no en VR.
        private static readonly (PlatformFamily Family, string[] Keywords)[] Rules =
        [
            (PlatformFamily.PC, ["Windows", "PC", "Linux", "Mac"]),
            (PlatformFamily.PlayStation, ["PlayStation"]),
            (PlatformFamily.Xbox, ["Xbox"]),
            (PlatformFamily.Nintendo, ["Nintendo", "Switch", "Wii", "Game Boy"]),
            (PlatformFamily.Mobile, ["Android", "iOS"]),
            (PlatformFamily.VR, ["VR", "Oculus", "Quest"]),
        ];

        public static PlatformFamily Resolve(string? platformName)
        {
            if (string.IsNullOrWhiteSpace(platformName))
            {
                return PlatformFamily.Other;
            }

            foreach (var (family, keywords) in Rules)
            {
                if (keywords.Any(keyword =>
                        platformName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                {
                    return family;
                }
            }

            return PlatformFamily.Other;
        }

        /// <summary>Familias sin repetir y sin "Other", para los iconos de una tarjeta.</summary>
        public static List<PlatformFamily> ToFamilies(IEnumerable<IgdbPlatformDto>? platforms)
        {
            if (platforms is null)
            {
                return [];
            }

            return platforms
                .Select(platform => Resolve(platform.Name))
                .Where(family => family != PlatformFamily.Other)
                .Distinct()
                .ToList();
        }

        /// <summary>Cada plataforma con su nombre completo y su familia, para el detalle.</summary>
        public static List<PlatformDto> ToPlatforms(IEnumerable<IgdbPlatformDto>? platforms)
        {
            if (platforms is null)
            {
                return [];
            }

            return platforms
                .Where(platform => !string.IsNullOrWhiteSpace(platform.Name))
                .Select(platform => new PlatformDto
                {
                    Name = platform.Name,
                    Family = Resolve(platform.Name)
                })
                .ToList();
        }
    }
}
