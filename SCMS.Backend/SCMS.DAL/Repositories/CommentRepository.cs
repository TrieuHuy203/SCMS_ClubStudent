using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.DTOs.Search;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
	// Triển khai các phương thức thao tác với Comment trong database
	public class CommentRepository : ICommentRepository
	{
		private readonly SCMSDbContext _context;

		// Inject DbContext qua constructor
		public CommentRepository(SCMSDbContext context)
		{
			_context = context;
		}

		// Tạo mới bình luận
		public async Task<Comment> AddAsync(Comment comment)
		{
			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();
			return comment;
		}

		// Cập nhật bình luận
		public async Task<Comment> UpdateAsync(Comment comment)
		{
			_context.Comments.Update(comment);
			await _context.SaveChangesAsync();
			return comment;
		}

		// Xóa bình luận (hard delete)
		public async Task DeleteAsync(Comment comment)
		{
			_context.Comments.Remove(comment);
			await _context.SaveChangesAsync();
		}

		// Lấy bình luận theo id
		public async Task<Comment?> GetByIdAsync(int commentId)
		{
			return await _context.Comments
				.AsNoTracking()
				.Include(c => c.User)
				.Include(c => c.Post)
				.Include(c => c.Likes)
				.FirstOrDefaultAsync(c => c.CommentId == commentId);
		}

		// Lấy bình luận theo id và userId để kiểm tra quyền sở hữu
		public async Task<Comment?> GetByIdAsync(int commentId, int userId)
		{
			return await _context.Comments
				.AsNoTracking()
				.Include(c => c.User)
				.Include(c => c.Post)
				.Include(c => c.Likes)
				.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);
		}

		// Lấy bình luận dạng tracked theo id để cập nhật/xóa (admin)
		public async Task<Comment?> GetByIdForUpdateAsync(int commentId)
		{
			return await _context.Comments
				.FirstOrDefaultAsync(c => c.CommentId == commentId);
		}

		// Lấy bình luận dạng tracked theo id + userId để cập nhật/xóa (user)
		public async Task<Comment?> GetByIdForUpdateAsync(int commentId, int userId)
		{
			return await _context.Comments
				.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);
		}

		// Lấy danh sách bình luận của user, có tìm kiếm và phân trang
		public async Task<(List<Comment> Items, int TotalCount)> GetMyCommentsAsync(
			int userId,
			CommentSearchRequest request)
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var query = _context.Comments
				.AsNoTracking()
				.Where(c => c.UserId == userId);

			// Lọc theo PostId nếu có
			if (request.PostId.HasValue)
			{
				query = query.Where(c => c.PostId == request.PostId.Value);
			}

			// Tìm kiếm theo từ khóa (nội dung bình luận)
			if (!string.IsNullOrWhiteSpace(request.Keyword))
			{
				var keyword = request.Keyword.Trim().ToLower();
				query = query.Where(c =>
					c.Content.ToLower().Contains(keyword));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.Include(c => c.User)
				.Include(c => c.Post)
				.Include(c => c.Likes)
				.OrderByDescending(c => c.CreatedAt)
				.ThenByDescending(c => c.CommentId)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Lấy danh sách tất cả bình luận, có tìm kiếm và phân trang cho admin
		public async Task<(List<Comment> Items, int TotalCount)> GetAllCommentsAsync(CommentSearchRequest request)
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var query = _context.Comments
				.AsNoTracking();

			// Lọc theo PostId nếu có
			if (request.PostId.HasValue)
			{
				query = query.Where(c => c.PostId == request.PostId.Value);
			}
			// Lọc theo ClubId nếu có
				if (request.ClubId.HasValue)
				{
					query = query.Where(c => c.Post != null && c.Post.ClubId == request.ClubId.Value);
				}

			// Lọc theo UserId nếu có
			if (request.UserId.HasValue)
			{
				query = query.Where(c => c.UserId == request.UserId.Value);
			}

			// Tìm kiếm theo từ khóa (nội dung bình luận)
			if (!string.IsNullOrWhiteSpace(request.Keyword))
			{
				var keyword = request.Keyword.Trim().ToLower();
				query = query.Where(c =>
					c.Content.ToLower().Contains(keyword));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.Include(c => c.User)
				.Include(c => c.Post)
				.Include(c => c.Likes)
				.OrderByDescending(c => c.CreatedAt)
				.ThenByDescending(c => c.CommentId)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Lấy like của user trên bình luận cụ thể
		public async Task<Like?> GetCommentLikeAsync(int commentId, int userId)
		{
			return await _context.Likes
				.FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId && x.PostId == null);
		}

		// Thêm like cho bình luận
		public async Task<Like> AddCommentLikeAsync(Like like)
		{
			_context.Likes.Add(like);
			await _context.SaveChangesAsync();
			return like;
		}

		// Xóa like khỏi bình luận
		public async Task RemoveCommentLikeAsync(Like like)
		{
			_context.Likes.Remove(like);
			await _context.SaveChangesAsync();
		}

		// Đếm tổng like của bình luận
		public async Task<int> CountCommentLikesAsync(int commentId)
		{
			return await _context.Likes.CountAsync(x => x.CommentId == commentId && x.PostId == null);
		}
	}
}
