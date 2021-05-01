using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUserNameAsync(string username);
        Task<MemberDto> GetMemberByUserNameAsync(string username);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
    }
}