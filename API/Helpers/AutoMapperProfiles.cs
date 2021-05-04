using AutoMapper;
using API.Dtos;
using System.Linq;
using CoreAndAngular.API.Extensions;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
            .ForMember(
                dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)
                )
                .ForMember(dest=>dest.Age,opt=>opt.MapFrom(src=>src.BirthDate.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto,AppUser>();
        }
    }
}