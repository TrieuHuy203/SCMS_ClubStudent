using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.DAL.Repositories
{
    public class PostImageRepository : IPostImageRepository
    {
        private readonly SCMSDbContext _context;

        public PostImageRepository(SCMSDbContext context)
        {
            _context = context;
        }

        public async Task<PostImage> AddAsync(PostImage image)
        {
            _context.PostImages.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<List<PostImage>> GetByPostIdAsync(int postId)
        {
            return await _context.PostImages
                .Where(img => img.PostId == postId)
                .OrderBy(img => img.SortOrder)
                .ToListAsync();
        }

        public async Task<PostImage?> GetByIdAsync(int imageId)
        {
            return await _context.PostImages.FirstOrDefaultAsync(img => img.ImageId == imageId);
        }

        // public async Task DeleteAsync(int imageId)
        // {
        //     var image = await _context.PostImages.FindAsync(imageId);
        //     if (image != null)
        //     {
        //         _context.PostImages.Remove(image);
        //         await _context.SaveChangesAsync();
        //     }
        // }
    
    

    public async Task<IEnumerable<PostImageDto>> GetAllAsync(PostImageSearchRequest request)
    {
        var query = _context.PostImages.AsQueryable();

        if (request.PostId.HasValue)
            query = query.Where(x => x.PostId == request.PostId.Value);
        // Bỏ filter UserId vì không có trường này

        var result = await query
            .OrderByDescending(x => x.UploadedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new PostImageDto
            {
                Id = x.ImageId,
                PostId = x.PostId,
                ImageUrl = x.ImageUrl,
                // Không có UserId
                CreatedAt = x.UploadedAt
            })
            .ToListAsync();

        return result;
    }

    public async Task<PostImageDto> GetDetailAsync(int id)
    {
        return await _context.PostImages
            .Where(x => x.ImageId == id)
            .Select(x => new PostImageDto
            {
                Id = x.ImageId,
                PostId = x.PostId,
                ImageUrl = x.ImageUrl,
                // Không có UserId
                CreatedAt = x.UploadedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.PostImages.FindAsync(id);
        if (entity == null) return false;
        _context.PostImages.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
   
   
   public async Task<IEnumerable<PostImageDto>> GetImagesByClubAsync(int clubId, int page, int pageSize)
{
    var query = _context.PostImages
        .Include(x => x.Post)
        .Where(x => x.Post != null && x.Post.ClubId == clubId);

    var result = await query
        .OrderByDescending(x => x.UploadedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new PostImageDto
        {
            Id = x.ImageId,
            PostId = x.PostId,
            ImageUrl = x.ImageUrl,
            CreatedAt = x.UploadedAt
        })
        .ToListAsync();

    return result;
}
    }
}