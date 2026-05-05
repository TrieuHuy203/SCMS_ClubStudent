using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
	// Triển khai các phương thức thao tác với Post trong database
	public class PostRepository : IPostRepository
	{
		private readonly SCMSDbContext _context;

		// Inject DbContext qua constructor
		public PostRepository(SCMSDbContext context)
		{
			_context = context;
		}

		// Tạo mới bài viết
		public async Task<Post> AddAsync(Post post)
		{
			_context.Posts.Add(post);
			await _context.SaveChangesAsync();
			return post;
		}

		// Cập nhật bài viết
		public async Task<Post> UpdateAsync(Post post)
		{
			_context.Posts.Update(post);
			await _context.SaveChangesAsync();
			return post;
		}

		// Xóa bài viết
		public async Task DeleteAsync(Post post)
		{
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();
		}

		// Lấy bài viết theo id
		public async Task<Post?> GetByIdAsync(int postId)
		{
			return await _context.Posts
				.AsNoTracking()
				.Include(p => p.Club)
				.Include(p => p.Event)
				.Include(p => p.User)
				.FirstOrDefaultAsync(p => p.PostId == postId);
		}

		// Lấy bài viết theo id và userId để kiểm tra quyền sở hữu
		public async Task<Post?> GetByIdAsync(int postId, int userId)
		{
			return await _context.Posts
				.AsNoTracking()
				.Include(p => p.Club)
				.Include(p => p.Event)
				.Include(p => p.User)
				.FirstOrDefaultAsync(p => p.PostId == postId && p.UserId == userId);
		}

		// Lấy bài viết dạng tracked theo id để cập nhật/xóa (admin)
		public async Task<Post?> GetByIdForUpdateAsync(int postId)
		{
			return await _context.Posts
				.FirstOrDefaultAsync(p => p.PostId == postId);
		}

		// Lấy bài viết dạng tracked theo id + userId để cập nhật/xóa (user)
		public async Task<Post?> GetByIdForUpdateAsync(int postId, int userId)
		{
			return await _context.Posts
				.FirstOrDefaultAsync(p => p.PostId == postId && p.UserId == userId);
		}

		// Lấy danh sách bài viết của user, có tìm kiếm và phân trang
		public async Task<(List<Post> Items, int TotalCount)> GetMyPostsAsync(
			int userId,
			PostSearchRequest request)
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var query = _context.Posts
				.AsNoTracking()
				.Include(p => p.Club)
				.Include(p => p.Event)
				.Include(p => p.User)
				.Where(p => p.UserId == userId);

			// Lọc theo trạng thái nếu có
			if (!string.IsNullOrWhiteSpace(request.Status))
			{
				var normalizedStatus = request.Status.Trim().ToLower();
				query = query.Where(p => p.Status != null && p.Status.ToLower() == normalizedStatus);
			}

			// Lọc theo loại bài viết nếu có
			if (!string.IsNullOrWhiteSpace(request.PostType))
			{
				var normalizedPostType = request.PostType.Trim().ToLower();
				query = query.Where(p => p.PostType != null && p.PostType.ToLower() == normalizedPostType);
			}

			// Lọc theo CLB nếu có
			if (request.ClubId.HasValue)
			{
				query = query.Where(p => p.ClubId == request.ClubId.Value);
			}

			// Lọc theo sự kiện nếu có
			if (request.EventId.HasValue)
			{
				query = query.Where(p => p.EventId == request.EventId.Value);
			}

			// Tìm kiếm theo từ khóa (tiêu đề, nội dung)
			if (!string.IsNullOrWhiteSpace(request.Keyword))
			{
				var keyword = request.Keyword.Trim().ToLower();
				query = query.Where(p =>
					p.Title.ToLower().Contains(keyword) ||
					p.Content.ToLower().Contains(keyword));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(p => p.CreatedAt)
				.ThenByDescending(p => p.PostId)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Lấy danh sách bài viết của các CLB user đã tham gia
		public async Task<(List<Post> Items, int TotalCount)> GetMyClubPostsAsync(
			IEnumerable<int> clubIds,
			PostSearchRequest request)
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var normalizedClubIds = clubIds?.Distinct().ToList() ?? new List<int>();
			if (normalizedClubIds.Count == 0)
			{
				return (new List<Post>(), 0);
			}

			var query = _context.Posts
				.AsNoTracking()
				.Include(p => p.Club)
				.Include(p => p.Event)
				.Include(p => p.User)
				.Where(p => p.ClubId.HasValue && normalizedClubIds.Contains(p.ClubId.Value));

			if (!string.IsNullOrWhiteSpace(request.Status))
			{
				var normalizedStatus = request.Status.Trim().ToLower();
				query = query.Where(p => p.Status != null && p.Status.ToLower() == normalizedStatus);
			}

			if (!string.IsNullOrWhiteSpace(request.PostType))
			{
				var normalizedPostType = request.PostType.Trim().ToLower();
				query = query.Where(p => p.PostType != null && p.PostType.ToLower() == normalizedPostType);
			}

			if (request.ClubId.HasValue)
			{
				query = query.Where(p => p.ClubId == request.ClubId.Value);
			}

			if (request.EventId.HasValue)
			{
				query = query.Where(p => p.EventId == request.EventId.Value);
			}

			if (!string.IsNullOrWhiteSpace(request.Keyword))
			{
				var keyword = request.Keyword.Trim().ToLower();
				query = query.Where(p =>
					p.Title.ToLower().Contains(keyword) ||
					p.Content.ToLower().Contains(keyword) ||
					(p.Club != null && p.Club.ClubName.ToLower().Contains(keyword)) ||
					(p.Event != null && p.Event.EventName.ToLower().Contains(keyword)));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(p => p.CreatedAt)
				.ThenByDescending(p => p.PostId)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Lấy danh sách bài viết công khai toàn hệ thống (chỉ Approved)
		public async Task<(List<Post> Items, int TotalCount)> GetPublicPostsAsync(PostSearchRequest request)
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var query = _context.Posts
				.AsNoTracking()
				.Include(p => p.Club)
				.Include(p => p.Event)
				.Include(p => p.User)
				.Where(p => p.Status != null && p.Status.ToLower() == "approved")
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(request.PostType))
			{
				var normalizedPostType = request.PostType.Trim().ToLower();
				query = query.Where(p => p.PostType != null && p.PostType.ToLower() == normalizedPostType);
			}

			// Nếu không truyền ClubId và EventId thì chỉ lấy bài viết chung (ClubId == null && EventId == null)
			if (!request.ClubId.HasValue && !request.EventId.HasValue)
			{
				query = query.Where(p => p.ClubId == null && p.EventId == null);
			}
			else
			{
				if (request.ClubId.HasValue)
				{
					query = query.Where(p => p.ClubId == request.ClubId.Value);
				}
				if (request.EventId.HasValue)
				{
					query = query.Where(p => p.EventId == request.EventId.Value);
				}
			}

			if (!string.IsNullOrWhiteSpace(request.Keyword))
			{
				var keyword = request.Keyword.Trim().ToLower();
				query = query.Where(p =>
					p.Title.ToLower().Contains(keyword) ||
					p.Content.ToLower().Contains(keyword) ||
					(p.User.FullName != null && p.User.FullName.ToLower().Contains(keyword)) ||
					(p.User.Email != null && p.User.Email.ToLower().Contains(keyword)) ||
					(p.Club != null && p.Club.ClubName.ToLower().Contains(keyword)) ||
					(p.Event != null && p.Event.EventName.ToLower().Contains(keyword)));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(p => p.CreatedAt)
				.ThenByDescending(p => p.PostId)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Lấy danh sách tất cả bài viết, có tìm kiếm và phân trang cho admin
		public async Task<(List<Post> Items, int TotalCount)> GetAllPostsAsync(PostSearchRequest request)
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var query = _context.Posts
				.AsNoTracking()
				.Include(p => p.Club)
				.Include(p => p.Event)
				.Include(p => p.User)
				.AsQueryable();

			// Lọc theo trạng thái nếu có
			if (!string.IsNullOrWhiteSpace(request.Status))
			{
				var normalizedStatus = request.Status.Trim().ToLower();
				query = query.Where(p => p.Status != null && p.Status.ToLower() == normalizedStatus);
			}

			// Lọc theo loại bài viết nếu có
			if (!string.IsNullOrWhiteSpace(request.PostType))
			{
				var normalizedPostType = request.PostType.Trim().ToLower();
				query = query.Where(p => p.PostType != null && p.PostType.ToLower() == normalizedPostType);
			}

			// Lọc theo CLB nếu có
			if (request.ClubId.HasValue)
			{
				query = query.Where(p => p.ClubId == request.ClubId.Value);
			}

			// Lọc theo sự kiện nếu có
			if (request.EventId.HasValue)
			{
				query = query.Where(p => p.EventId == request.EventId.Value);
			}

			// Tìm kiếm theo từ khóa (tiêu đề, nội dung, tên user, email, tên CLB, tên sự kiện)
			if (!string.IsNullOrWhiteSpace(request.Keyword))
			{
				var keyword = request.Keyword.Trim().ToLower();
				query = query.Where(p =>
					p.Title.ToLower().Contains(keyword) ||
					p.Content.ToLower().Contains(keyword) ||
					(p.User.FullName != null && p.User.FullName.ToLower().Contains(keyword)) ||
					(p.User.Email != null && p.User.Email.ToLower().Contains(keyword)) ||
					(p.Club != null && p.Club.ClubName.ToLower().Contains(keyword)) ||
					(p.Event != null && p.Event.EventName.ToLower().Contains(keyword)));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(p => p.CreatedAt)
				.ThenByDescending(p => p.PostId)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Lấy bản ghi like theo post + user
		public async Task<Like?> GetPostLikeAsync(int postId, int userId)
		{
			return await _context.Likes
				.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId && x.CommentId == null);
		}

		// Thêm like cho bài viết
		public async Task<Like> AddPostLikeAsync(Like like)
		{
			_context.Likes.Add(like);
			await _context.SaveChangesAsync();
			return like;
		}

		// Xóa like khỏi bài viết
		public async Task RemovePostLikeAsync(Like like)
		{
			_context.Likes.Remove(like);
			await _context.SaveChangesAsync();
		}

		// Đếm tổng like của bài viết
		public async Task<int> CountPostLikesAsync(int postId)
		{
			return await _context.Likes.CountAsync(x => x.PostId == postId && x.CommentId == null);
		}

		// Kiểm tra user đã báo cáo bài viết chưa (chặn report trùng)
		public async Task<bool> HasUserReportedPostAsync(int postId, int userId)
		{
			return await _context.PostReports.AnyAsync(x => x.PostId == postId && x.UserId == userId);
		}

		// Lưu báo cáo bài viết
		public async Task<PostReport> AddPostReportAsync(PostReport report)
		{
			_context.PostReports.Add(report);
			await _context.SaveChangesAsync();
			return report;
		
		}

// Lấy danh sách bài viết user đã like (có phân trang, lọc)
		public async Task<(List<Post> Items, int TotalCount)> GetLikedPostsAsync(int userId, PostSearchRequest request)
		{
			var query = _context.Likes
				.Where(like => like.UserId == userId && like.CommentId == null)
				.Join(_context.Posts, like => like.PostId, post => post.PostId, (like, post) => post)
				.AsQueryable();

			// Lọc theo từ khóa nếu có
			if (!string.IsNullOrWhiteSpace(request.Keyword))
				query = query.Where(p => p.Title.Contains(request.Keyword) || p.Content.Contains(request.Keyword));

			// Lọc theo trạng thái nếu có
			if (!string.IsNullOrWhiteSpace(request.Status))
				query = query.Where(p => p.Status == request.Status);

			// Lọc theo loại bài viết nếu có
			if (!string.IsNullOrWhiteSpace(request.PostType))
				query = query.Where(p => p.PostType == request.PostType);

			// Lọc theo CLB nếu có
			if (request.ClubId.HasValue)
				query = query.Where(p => p.ClubId == request.ClubId.Value);

			// Lọc theo sự kiện nếu có
			if (request.EventId.HasValue)
				query = query.Where(p => p.EventId == request.EventId.Value);

			int totalCount = await query.CountAsync();
			int skip = (request.Page - 1) * request.PageSize;
			var items = await query
				.OrderByDescending(p => p.CreatedAt)
				.Skip(skip)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}
	}
}
