namespace GameVault.Source.Infrastructure.ExternalServices.Igdb
{
    internal sealed record IgdbPageQuery(
        string ResultsQuery,
        string CountQuery);
}
