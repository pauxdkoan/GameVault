using GameVault.Source.Application.Dtos.Reviews;
using GameVault.Source.Domain.Entities;

namespace GameVault.Source.Application.Mappings
{
    internal static class CommentMapper
    {
        /// <summary>Requiere ReviewComment.ApplicationUser cargado.</summary>
        public static CommentDto ToDto(ReviewComment comment, Guid? currentUserId)
        {
            return new CommentDto
            {
                Id = comment.Id,
                ReviewId = comment.ReviewId,
                Author = GameEntityMapper.ToUserSummary(comment.ApplicationUser),
                Content = comment.Content,
                IsMine = currentUserId == comment.ApplicationUserId,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}
