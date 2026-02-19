using AutoMapper;
using seashore_CRM.Models.Entities;
using seashore_CRM.Models.DTOs;

namespace seashore_CRM.BLL.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Lead, LeadDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.StatusName : null))
                .ForMember(dest => dest.AssignedUserName, opt => opt.MapFrom(src => src.AssignedUser != null ? src.AssignedUser.FullName : null));

            CreateMap<LeadDto, Lead>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedUser, opt => opt.Ignore());

            CreateMap<Lead, Opportunity>()
                .ForMember(dest => dest.LeadId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Stage, opt => opt.MapFrom(src => "New"))
                .ForMember(dest => dest.EstimatedValue, opt => opt.MapFrom(src => src.Budget ?? 0M))
                .ForMember(dest => dest.Probability, opt => opt.MapFrom(src => src.Probability ?? 0))
                .ForMember(dest => dest.ExpectedCloseDate, opt => opt.MapFrom(src => src.DecisionDate));
        }
    }
}
