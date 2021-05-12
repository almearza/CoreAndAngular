using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using API.Entities;
namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DContext context, IMapper mapper)
        {
            _mapper = mapper;
            this._context = context;
        }
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.AppUsers.Include(m => m.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.AppUsers.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.AppUsers.Include(m => m.Photos).FirstOrDefaultAsync(m => m.UserName == username.ToLower());
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<MemberDto> GetMemberByUserNameAsync(string username)
        {
            return await _context.AppUsers
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(m => m.Username == username.ToLower());
        }

        public async Task<PageList<MemberDto>> GetMembersAsync(UserPrams userPrams)
        {
            var query =  _context.AppUsers.AsQueryable();
            query = query.Where(u=>u.UserName!=userPrams.CurrentUsername);
            query = query.Where(u=>u.Gender==userPrams.Gender);

            var minDob = DateTime.Today.AddYears(-userPrams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userPrams.MinAge);
            query=query.Where(u=>u.BirthDate>=minDob && u.BirthDate<=maxDob);

            query = userPrams.OrderBy switch{
                "created"=>query.OrderByDescending(u=>u.Created),
                _=>query.OrderByDescending(u=>u.LastActive)
            };
            // .AsNoTracking();

            return await PageList<MemberDto>.CreateAsync(query
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking()
            ,userPrams.PageNumber,userPrams.PageSize);
        }
    }
}