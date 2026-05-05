// ...existing code...
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
// ...existing code...
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;

namespace SCMS.BusinessService.Services
{
    public class PostImageService : IPostImageService
    {
        private readonly IPostImageRepository _postImageRepository;
        // ...existing code...

        public PostImageService(IPostImageRepository postImageRepository)
        {
            _postImageRepository = postImageRepository;
        }

        public async Task<PostImageResponse> UploadImageAsync(CreatePostImageRequest request)
        {
            // Logic này chỉ nhận thông tin ảnh đã được upload (ImageUrl, Caption, ...)
            var postImage = new PostImage
            {
                PostId = request.PostId,
                ImageUrl = request.ImageUrl, // ImageUrl sẽ được controller truyền vào sau khi upload file
                Caption = request.Caption,
                SortOrder = request.SortOrder,
                UploadedAt = DateTime.UtcNow
            };

            var saved = await _postImageRepository.AddAsync(postImage);

            return new PostImageResponse
            {
                ImageId = saved.ImageId,
                PostId = saved.PostId,
                ImageUrl = saved.ImageUrl,
                Caption = saved.Caption,
                SortOrder = saved.SortOrder,
                UploadedAt = saved.UploadedAt
            };
        }

        public async Task<List<PostImageResponse>> GetImagesByPostIdAsync(int postId)
        {
            var images = await _postImageRepository.GetByPostIdAsync(postId);
            return images.Select(img => new PostImageResponse
            {
                ImageId = img.ImageId,
                PostId = img.PostId,
                ImageUrl = img.ImageUrl,
                Caption = img.Caption,
                SortOrder = img.SortOrder,
                UploadedAt = img.UploadedAt
            }).ToList();
        }

        public async Task DeleteImageAsync(int imageId)
        {
            await _postImageRepository.DeleteAsync(imageId);
        }
  
   public async Task<IEnumerable<PostImageDto>> GetAllAsync(PostImageSearchRequest request)
    {
        return await _postImageRepository.GetAllAsync(request);
    }

    public async Task<PostImageDto> GetDetailAsync(int id)
    {
        return await _postImageRepository.GetDetailAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _postImageRepository.DeleteAsync(id);
    }
  
    public async Task<IEnumerable<PostImageDto>> GetImagesByClubAsync(int clubId, int page, int pageSize)
{
    return await _postImageRepository.GetImagesByClubAsync(clubId, page, pageSize);
}
    }
}