using GameVault.Source.Application.Feature.Reviews.Commands.CreateReview;
using GameVault.Source.Application.Feature.Reviews.Commands.UpdateReview;

namespace GameVault.Tests.Validators
{
    /// <summary>Reglas de crear (POST) y editar (PUT) una reseña. Comparten ReviewContentRules.</summary>
    public class ReviewValidatorsTests
    {
        private readonly CreateReviewCommandValidator _create = new();
        private readonly UpdateReviewCommandValidator _update = new();

        private static CreateReviewCommand ValidCreate() =>
            new(UserId: Guid.NewGuid(), ExternalId: 1942, Title: "Obra maestra", Content: "Me encantó.", Rating: 9, ContainsSpoilers: false);

        private static UpdateReviewCommand ValidUpdate() =>
            new(UserId: Guid.NewGuid(), ReviewId: Guid.NewGuid(), Title: "", Content: "Lo rejugué.", Rating: 8, ContainsSpoilers: true);

        [Fact]
        public void Acepta_reseñas_validas_y_el_titulo_vacio()
        {
            Assert.True(_create.Validate(ValidCreate()).IsValid);
            Assert.True(_update.Validate(ValidUpdate()).IsValid);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void Acepta_los_extremos_de_la_nota(int rating)
        {
            Assert.True(_create.Validate(ValidCreate() with { Rating = rating }).IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public void Rechaza_nota_fuera_de_1_a_10(int rating)
        {
            var create = _create.Validate(ValidCreate() with { Rating = rating });
            var update = _update.Validate(ValidUpdate() with { Rating = rating });

            Assert.Contains(create.Errors, e => e.PropertyName == nameof(CreateReviewCommand.Rating));
            Assert.Contains(update.Errors, e => e.PropertyName == nameof(UpdateReviewCommand.Rating));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Rechaza_reseña_sin_texto(string content)
        {
            var result = _create.Validate(ValidCreate() with { Content = content });

            Assert.Contains(result.Errors, e => e.ErrorMessage == "Escribe tu reseña.");
        }

        [Fact]
        public void Rechaza_textos_demasiado_largos()
        {
            var result = _update.Validate(ValidUpdate() with { Title = new string('t', 121), Content = new string('c', 5001) });

            Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateReviewCommand.Title));
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateReviewCommand.Content));
        }

        [Fact]
        public void Crear_rechaza_un_juego_no_valido()
        {
            var result = _create.Validate(ValidCreate() with { ExternalId = 0 });

            Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateReviewCommand.ExternalId));
        }
    }
}
