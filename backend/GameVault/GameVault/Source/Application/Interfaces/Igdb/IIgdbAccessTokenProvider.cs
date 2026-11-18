namespace GameVault.Source.Application.Interfaces.Igdb
{
    public interface IIgdbAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
    }
}
