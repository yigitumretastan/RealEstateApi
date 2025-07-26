using AutoMapper;
using Real_Estate_Api.DTOs;
using Real_Estate_Api.Models;

namespace Real_Estate_Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mapping
            CreateMap<RegisterDto, User>();
            CreateMap<LoginDto, User>();

            // Listing mapping
            CreateMap<CreateListingDto, Listing>();
            CreateMap<UpdateListingDto, Listing>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Optional: Entity to DTO dönüşümleri (eğer DTO return edeceksen)
            // CreateMap<Listing, ListingDto>();
        }
    }
}
