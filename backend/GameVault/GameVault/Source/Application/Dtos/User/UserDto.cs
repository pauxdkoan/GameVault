namespace GameVault.Source.Application.Dtos.User
{
    public class UserDto
    {
        public string UserName {  get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? Country { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
