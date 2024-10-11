using AutoMapper;
using TeaTimeDemo.DTOs;
using TeaTimeDemo.Models;

namespace TeaTimeDemo.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Survey, SurveyDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.IsPublished ? "是" : "否"))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime.HasValue ? src.CreateTime.Value.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty))
                .ForMember(dest => dest.CompleteTime, opt => opt.MapFrom(src => src.CompleteTime.HasValue ? src.CompleteTime.Value.ToString("yyyy/MM/dd HH:mm:ss") : "未完成"));
        }
    }
}
