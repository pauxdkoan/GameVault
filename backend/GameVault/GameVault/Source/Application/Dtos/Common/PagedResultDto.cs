namespace GameVault.Source.Application.Dtos.Common
{
    /// <summary>Una página de resultados: los elementos y el total para paginar.</summary>
    public sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = [];

        public int Count { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
