using AutoMapper;
using API.Dtos;
namespace API.Helpers
{
    public class AutoMapperProfiles:Profile
    {
       public AutoMapperProfiles()
       {
           CreateMap<AppUser,MemberDto>();
           CreateMap<Photo,PhotoDto>();
       }
    }
}