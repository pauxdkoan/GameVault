using GameVault.Source.Application.Feature.Library.Commands.UpsertLibraryEntry;
using GameVault.Source.Domain.Enums;

namespace GameVault.Tests.Validators
{
    /// <summary>Reglas de PUT /api/me/library/{externalId}.</summary>
    public class UpsertLibraryEntryCommandValidatorTests
    {
        private readonly UpsertLibraryEntryCommandValidator _validator = new();

        private static UpsertLibraryEntryCommand ValidCommand() =>
            new(UserId: Guid.NewGuid(), ExternalId: 1942, Status: GameStatus.Playing, ProgressPercentage: 40);

        [Fact]
        public void Acepta_una_entrada_normal()
        {
            Assert.True(_validator.Validate(ValidCommand()).IsValid);
        }

        [Fact]
        public void Acepta_los_campos_opcionales_vacios()
        {
            var command = ValidCommand() with { HoursPlayed = null, PersonalRating = null, StartedAt = null, CompletedAt = null, PrivateNotes = null };

            Assert.True(_validator.Validate(command).IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Rechaza_un_juego_no_valido(int externalId)
        {
            AssertInvalid(ValidCommand() with { ExternalId = externalId }, nameof(UpsertLibraryEntryCommand.ExternalId));
        }

        [Fact]
        public void Rechaza_un_estado_que_no_existe()
        {
            AssertInvalid(ValidCommand() with { Status = (GameStatus)99 }, nameof(UpsertLibraryEntryCommand.Status));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Rechaza_progreso_fuera_de_0_a_100(int progress)
        {
            AssertInvalid(ValidCommand() with { ProgressPercentage = progress }, nameof(UpsertLibraryEntryCommand.ProgressPercentage));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public void Rechaza_nota_fuera_de_1_a_10(int rating)
        {
            AssertInvalid(ValidCommand() with { PersonalRating = rating }, nameof(UpsertLibraryEntryCommand.PersonalRating));
        }

        [Fact]
        public void Rechaza_horas_negativas()
        {
            AssertInvalid(ValidCommand() with { HoursPlayed = -1 }, nameof(UpsertLibraryEntryCommand.HoursPlayed));
        }

        [Fact]
        public void Rechaza_terminar_antes_de_empezar()
        {
            var command = ValidCommand() with
            {
                StartedAt = new DateOnly(2026, 5, 10),
                CompletedAt = new DateOnly(2026, 5, 1),
            };

            AssertInvalid(command, nameof(UpsertLibraryEntryCommand.CompletedAt));
        }

        [Fact]
        public void Rechaza_notas_privadas_demasiado_largas()
        {
            AssertInvalid(ValidCommand() with { PrivateNotes = new string('a', 2001) }, nameof(UpsertLibraryEntryCommand.PrivateNotes));
        }

        private void AssertInvalid(UpsertLibraryEntryCommand command, string property)
        {
            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == property);
        }
    }
}
