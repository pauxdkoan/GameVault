using FluentValidation;

namespace GameVault.Source.Application.Feature.Profile.Commands.UpdateMyProfile
{
    public sealed class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
    {
        public UpdateMyProfileCommandValidator()
        {
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("El nombre visible es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre visible no puede superar los 50 caracteres.");

            RuleFor(x => x.Bio)
                .MaximumLength(300).WithMessage("La biografía no puede superar los 300 caracteres.");

            RuleFor(x => x.Country)
                .MaximumLength(60).WithMessage("El país no puede superar los 60 caracteres.");

            RuleFor(x => x.AvatarUrl)
                .Must(BeHttpUrl).WithMessage("La URL del avatar no es válida.")
                .When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl));

            RuleFor(x => x.BannerUrl)
                .Must(BeHttpUrl).WithMessage("La URL del banner no es válida.")
                .When(x => !string.IsNullOrWhiteSpace(x.BannerUrl));
        }

        /// <summary>Solo http/https: evita valores como "javascript:..." en una etiqueta img.</summary>
        private static bool BeHttpUrl(string? value)
        {
            return value is { Length: <= 500 }
                && Uri.TryCreate(value, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
