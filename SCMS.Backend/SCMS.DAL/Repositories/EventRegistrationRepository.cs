using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
	public class EventRegistrationRepository : IEventRegistrationRepository
	{
		private readonly SCMSDbContext _context;

		public EventRegistrationRepository(SCMSDbContext context)
		{
			_context = context;
		}

		public async Task<EventRegistration> AddAsync(EventRegistration entity)
		{
			await _context.EventRegistrations.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<EventRegistration> UpdateAsync(EventRegistration entity)
		{
			_context.EventRegistrations.Update(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<Event?> GetEventByIdAsync(int eventId)
		{
			return await _context.Events
				.AsNoTracking()
				.Include(e => e.Club)
				.FirstOrDefaultAsync(e => e.EventId == eventId && !e.IsDeleted);
		}

		public async Task<EventRegistration?> GetByUserAndEventAsync(int userId, int eventId)
		{
			return await _context.EventRegistrations
				.Include(r => r.Event)
				.ThenInclude(e => e.Club)
				.FirstOrDefaultAsync(r => r.UserId == userId && r.EventId == eventId);
		}

		public async Task<EventRegistration?> GetByIdAsync(int eventRegistrationId, int userId)
		{
			return await _context.EventRegistrations
				.AsNoTracking()
				.Include(r => r.Event)
				.ThenInclude(e => e.Club)
				.FirstOrDefaultAsync(r => r.EventRegistrationId == eventRegistrationId && r.UserId == userId);
		}

		public async Task<EventRegistration?> GetByIdForAdminAsync(int eventRegistrationId)
		{
			return await _context.EventRegistrations
				.AsNoTracking()
				.Include(r => r.Event)
				.ThenInclude(e => e.Club)
				.Include(r => r.User)
				.FirstOrDefaultAsync(r => r.EventRegistrationId == eventRegistrationId);
		}

		public async Task<bool> IsUserApprovedMemberOfClubAsync(int userId, int clubId)
		{
			return await _context.Memberships
				.AnyAsync(m => m.UserId == userId
					&& m.ClubId == clubId
					&& m.Status != null
					&& m.Status.ToLower() == "approved");
		}

		public async Task<(List<EventRegistration> Items, int TotalCount)> GetMyRegistrationsAsync(
			int userId,
			string? keyword,
			string? registrationStatus,
			int page,
			int pageSize)
		{
			if (page < 1) page = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.EventRegistrations
				.AsNoTracking()
				.Include(r => r.Event)
				.ThenInclude(e => e.Club)
				.Where(r => r.UserId == userId)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				var normalizedKeyword = keyword.Trim().ToLower();
				query = query.Where(r => r.Event.EventName.ToLower().Contains(normalizedKeyword));
			}

			if (!string.IsNullOrWhiteSpace(registrationStatus))
			{
				var normalizedStatus = registrationStatus.Trim().ToLower();
				query = query.Where(r => r.RegistrationStatus != null && r.RegistrationStatus.ToLower() == normalizedStatus);
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(r => r.Event.EventTime)
				.ThenByDescending(r => r.EventRegistrationId)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public async Task<(List<EventRegistration> Items, int TotalCount)> GetRegistrationsByEventAsync(
			int eventId,
			string? keyword,
			string? registrationStatus,
			int page,
			int pageSize)
		{
			if (page < 1) page = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.EventRegistrations
				.AsNoTracking()
				.Include(r => r.Event)
				.Include(r => r.User)
				.Where(r => r.EventId == eventId)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				var normalizedKeyword = keyword.Trim().ToLower();
				query = query.Where(r =>
					(r.User.FullName != null && r.User.FullName.ToLower().Contains(normalizedKeyword)) ||
					(r.User.Email != null && r.User.Email.ToLower().Contains(normalizedKeyword)) ||
					(r.User.Phone != null && r.User.Phone.ToLower().Contains(normalizedKeyword)));
			}

			if (!string.IsNullOrWhiteSpace(registrationStatus))
			{
				var normalizedStatus = registrationStatus.Trim().ToLower();
				query = query.Where(r => r.RegistrationStatus != null && r.RegistrationStatus.ToLower() == normalizedStatus);
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(r => r.EventRegistrationId)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public async Task<(List<EventRegistration> Items, int TotalCount)> GetAllRegistrationsAsync(
			string? keyword,
			string? registrationStatus,
			int page,
			int pageSize)
		{
			if (page < 1) page = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.EventRegistrations
				.AsNoTracking()
				.Include(r => r.Event)
				.Include(r => r.User)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				var normalizedKeyword = keyword.Trim().ToLower();
				query = query.Where(r =>
					(r.Event.EventName != null && r.Event.EventName.ToLower().Contains(normalizedKeyword)) ||
					(r.User.FullName != null && r.User.FullName.ToLower().Contains(normalizedKeyword)) ||
					(r.User.Email != null && r.User.Email.ToLower().Contains(normalizedKeyword)) ||
					(r.User.Phone != null && r.User.Phone.ToLower().Contains(normalizedKeyword)));
			}

			if (!string.IsNullOrWhiteSpace(registrationStatus))
			{
				var normalizedStatus = registrationStatus.Trim().ToLower();
				query = query.Where(r => r.RegistrationStatus != null && r.RegistrationStatus.ToLower() == normalizedStatus);
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(r => r.Event.EventTime)
				.ThenByDescending(r => r.EventRegistrationId)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public async Task<int> CountRegisteredByEventAsync(int eventId)
		{
			return await _context.EventRegistrations
				.AsNoTracking()
				.CountAsync(r =>
					r.EventId == eventId
					&& r.RegistrationStatus != null
					&& r.RegistrationStatus.ToLower() == "registered");
		}

		public async Task<List<EventRegistration>> GetRegistrationsForExportByEventAsync(int eventId)
		{
			return await _context.EventRegistrations
				.AsNoTracking()
				.Include(r => r.Event)
				.Include(r => r.User)
				.Where(r => r.EventId == eventId)
				.OrderByDescending(r => r.EventRegistrationId)
				.ToListAsync();
		}

		public async Task SyncEventParticipantCountAsync(int eventId)
		{
			var registeredCount = await _context.EventRegistrations
				.CountAsync(r =>
					r.EventId == eventId
					&& r.RegistrationStatus != null
					&& r.RegistrationStatus.ToLower() == "registered");

			var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId && !e.IsDeleted);
			if (ev == null)
			{
				return;
			}

			ev.ParticipantCount = registeredCount;
			await _context.SaveChangesAsync();
		}
	}
}
