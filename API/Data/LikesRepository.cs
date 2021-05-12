using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using API.Helpers;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DContext _context;
        private readonly IMapper _mapper;
        public LikesRepository(DContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }
        public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int likedUserId)
        {
            return await _context.UserLikes
            .FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PageList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            IQueryable<LikeDto> query = null;
            switch (likeParams.predicate)
            {
                case "liked":
                    query =  _context.UserLikes
                    .Where(u => u.SourceUserId ==likeParams.userId)
                    .Select(u => u.LikedUser).ProjectTo<LikeDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();
                    break;
                default://likedby
                    query = _context.UserLikes
                     .Where(u => u.LikedUserId ==likeParams.userId)
                     .Select(u => u.SourceUser).ProjectTo<LikeDto>(_mapper.ConfigurationProvider)
                     .AsQueryable();
                     break;
            }
            return await PageList<LikeDto>.CreateAsync(query,likeParams.PageNumber,likeParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikesAsync(int userId)
        {
            return await _context.AppUsers
            .Include(u => u.LikedUsers)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}