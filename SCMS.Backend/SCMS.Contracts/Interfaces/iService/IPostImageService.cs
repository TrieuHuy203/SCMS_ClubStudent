using System.Collections.Generic;
using System.Threading.Tasks;
// ...existing code...
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
    public interface IPostImageService
    {
        Task<PostImageResponse> UploadImageAsync(CreatePostImageRequest request);
        Task<List<PostImageResponse>> GetImagesByPostIdAsync(int postId);
        Task DeleteImageAsync(int imageId);
         Task<IEnumerable<PostImageDto>> GetAllAsync(PostImageSearchRequest request);
    Task<PostImageDto> GetDetailAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<PostImageDto>> GetImagesByClubAsync(int clubId, int page, int pageSize);
    }
}