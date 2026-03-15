using AutoMapper;
using seashore_CRM.Models.Entities;
using seashore_CRM.BLL.DTOs;
using System.Linq;

namespace seashore_CRM.BLL.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Lead, LeadDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.StatusName : null))
                .ForMember(dest => dest.AssignedUserName, opt => opt.MapFrom(src => src.AssignedUser != null ? src.AssignedUser.FullName : null))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<LeadDto, Lead>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedUser, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore());

            // Map product dto to LeadItem when saving (LineTotal computed in controller/service)
            CreateMap<LeadProductDto, LeadItem>()
                // Do not map ProductId automatically when null. Service should create product first for free-entry items.
                .ForMember(dest => dest.ProductId, opt => opt.Condition(src => src.ProductId.HasValue))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.TaxPercentage, opt => opt.MapFrom(src => src.TaxPercentage))
                .ForMember(dest => dest.LineTotal, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
