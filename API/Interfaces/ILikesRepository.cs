using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        public Task<UserLike> GetUserLikeAsync(int sourceUserId,int likedUserId);
        public Task<AppUser>GetUserWithLikesAsync(int userId);
        public Task<PageList<LikeDto>> GetUserLikes(LikeParams likeParams);
    }
}