namespace GameVault.Source.Application.Dtos.Game.AdditionalDtos
{
    public class InvolvedCompaniesDto
    {

        public string Name { get; set; } = string.Empty;

        public string? LogoImage {  get; set; } =string.Empty;


        public bool IsDeveloper {  get; set; }
        public bool IsPublisher {  get; set; }
    }
}
