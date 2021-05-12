using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : ApiBaseController
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }
        [HttpPost("{username}")]//the username i wanna to like
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var sourceUser = await _likesRepository.GetUserWithLikesAsync(sourceUserId);
            if (sourceUser.UserName == username) return BadRequest("you can not liked your self");

            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            if (likedUser == null) return BadRequest("no user to like");

            var userLike = await _likesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("you already liked this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("failed to like user");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikeParams likeParams)
        {
            likeParams.userId=User.GetUserId();
            var likes =  await _likesRepository.GetUserLikes(likeParams);
            Response.AddPaginationHeader(likes.CurrentPage,likes.TotalPages,likes.TotalCount,likes.PageSize);
            return Ok(likes);
        }
    }
}