using GameVault.Source.Application.Dtos.User;

namespace GameVault.Source.Application.Dtos.Reviews
{
    public sealed class CommentDto
    {
        public Guid Id { get; set; }

        public Guid ReviewId { get; set; }

        public UserSummaryDto Author { get; set; } = new();

        public string Content { get; set; } = string.Empty;

        public bool IsMine { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
