using AutoMapper;
using API.Dtos;
using System.Linq;
using CoreAndAngular.API.Extensions;
using API.Entities;
using System;

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
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.BirthDate.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<AppUser, LikeDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.BirthDate.CalculateAge()));

            CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderPhotoUrl, opt =>
               opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))

            .ForMember(dest => dest.RecipientPhotoUrl, opt =>
               opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
            //    CreateMap<DateTime,DateTime>().ConvertUsing(d=>DateTime.SpecifyKind(d,DateTimeKind.Utc));
        }
    }
}