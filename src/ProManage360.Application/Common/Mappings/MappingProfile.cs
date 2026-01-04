using AutoMapper;
using ProManage360.Application.Features.Auth.Command.RegisterTenant;
using ProManage360.Domain.Entities;
using ProManage360.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            //// Command → Entity (for creating)
            //CreateMap<RegisterTenantCommand, Tenant>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())  // Generated in handler
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrganizationName))
            //    .ForMember(dest => dest.Domain, opt => opt.MapFrom(src => src.Subdomain.ToLower()))
            //    .ForMember(dest => dest.SubscriptionTier, opt => opt.MapFrom(src => SubscriptionTier.Free)) 
            //    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            //    .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

            //CreateMap<RegisterTenantCommand, User>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.TenantId, opt => opt.Ignore())  // Set in handler
            //    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AdminEmail.ToLower()))
            //    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())  // BCrypt in handler
            //    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            //    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
