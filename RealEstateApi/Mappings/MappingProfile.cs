using AutoMapper;
using RealEstateApi.DTOs;
using RealEstateApi.Models;

namespace RealEstateApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, User>();
            CreateMap<LoginDto, User>();

            CreateMap<CreateListingDto, Listing>();
            CreateMap<UpdateListingDto, Listing>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
