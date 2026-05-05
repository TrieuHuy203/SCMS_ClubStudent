using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
	// Triển khai các phương thức thao tác với Feedback trong database
	public class FeedbackRepository : IFeedbackRepository
	{
		private readonly SCMSDbContext _context;

		// Inject DbContext qua constructor
		public FeedbackRepository(SCMSDbContext context)
		{
			_context = context;
		}

		// Tạo mới feedback
		public async Task<Feedback> AddAsync(Feedback feedback)
		{
			_context.Feedbacks.Add(feedback);
			await _context.SaveChangesAsync();
			return feedback;
		}

		// Cập nhật feedback
		public async Task<Feedback> UpdateAsync(Feedback feedback)
		{
			_context.Feedbacks.Update(feedback);
			await _context.SaveChangesAsync();
			return feedback;
		}

		// Xóa feedback
		public async Task DeleteAsync(Feedback feedback)
		{
			_context.Feedbacks.Remove(feedback);
			await _context.SaveChangesAsync();
		}

		// Lấy feedback theo id
		public async Task<Feedback?> GetByIdAsync(int feedbackId)
		{
			return await _context.Feedbacks
				.AsNoTracking()
				.Include(f => f.User)
				.Include(f => f.ProcessedByNavigation)
				.FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
		}

		// Lấy feedback theo id và userId để kiểm tra quyền sở hữu
		public async Task<Feedback?> GetByIdAsync(int feedbackId, int userId)
		{
			return await _context.Feedbacks
				.AsNoTracking()
				.Include(f => f.User)
				.Include(f => f.ProcessedByNavigation)
				.FirstOrDefaultAsync(f => f.FeedbackId == feedbackId && f.UserId == userId);
		}

		// Lấy feedback dạng tracked theo id để cập nhật/xóa (admin)
		public async Task<Feedback?> GetByIdForUpdateAsync(int feedbackId)
		{
			return await _context.Feedbacks
				.FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
		}

		// Lấy feedback dạng tracked theo id + userId để cập nhật/xóa (user)
		public async Task<Feedback?> GetByIdForUpdateAsync(int feedbackId, int userId)
		{
			return await _context.Feedbacks
				.FirstOrDefaultAsync(f => f.FeedbackId == feedbackId && f.UserId == userId);
		}

		// Lấy danh sách feedback của user, có tìm kiếm và phân trang
		public async Task<(List<Feedback> Items, int TotalCount)> GetMyFeedbacksAsync(
			int userId,
			FeedbackSearchRequest request)
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var query = _context.Feedbacks
				.AsNoTracking()
				.Where(f => f.UserId == userId);

			if (!string.IsNullOrWhiteSpace(request.Status))
			{
				var normalizedStatus = request.Status.Trim().ToLower();
				query = query.Where(f => f.Status != null && f.Status.ToLower() == normalizedStatus);
			}

			if (!string.IsNullOrWhiteSpace(request.Keyword))
			{
				var keyword = request.Keyword.Trim().ToLower();
				query = query.Where(f => f.Content.ToLower().Contains(keyword));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.Include(f => f.User)
				.Include(f => f.ProcessedByNavigation)
				.OrderByDescending(f => f.CreatedAt)
				.ThenByDescending(f => f.FeedbackId)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Lấy danh sách tất cả feedback, có tìm kiếm và phân trang cho admin
		public async Task<(List<Feedback> Items, int TotalCount)> GetAllFeedbacksAsync(FeedbackSearchRequest request)
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var query = _context.Feedbacks
				.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(request.Status))
			{
				var normalizedStatus = request.Status.Trim().ToLower();
				query = query.Where(f => f.Status != null && f.Status.ToLower() == normalizedStatus);
			}

			if (request.UserId.HasValue)
			{
				query = query.Where(f => f.UserId == request.UserId.Value);
			}

			if (!string.IsNullOrWhiteSpace(request.Keyword))
			{
				var keyword = request.Keyword.Trim().ToLower();
				query = query.Where(f => f.Content.ToLower().Contains(keyword));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.Include(f => f.User)
				.Include(f => f.ProcessedByNavigation)
				.OrderByDescending(f => f.CreatedAt)
				.ThenByDescending(f => f.FeedbackId)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return (items, totalCount);
		}
	}
}
