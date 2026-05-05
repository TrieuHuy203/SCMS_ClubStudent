using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    public interface IPostImageRepository
    {
        Task<PostImage> AddAsync(PostImage image);
        Task<List<PostImage>> GetByPostIdAsync(int postId);
        Task<PostImage?> GetByIdAsync(int imageId);

          Task<IEnumerable<PostImageDto>> GetAllAsync(PostImageSearchRequest request);
    Task<PostImageDto> GetDetailAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<PostImageDto>> GetImagesByClubAsync(int clubId, int page, int pageSize);
    }
}