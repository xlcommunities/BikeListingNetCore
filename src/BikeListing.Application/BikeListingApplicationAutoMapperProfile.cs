using BikeListing.Bikes;
using System;
using BikeListing.Shared;
using Volo.Abp.AutoMapper;
using BikeListing.Manufacturers;
using AutoMapper;

namespace BikeListing;

public class BikeListingApplicationAutoMapperProfile : Profile
{
    public BikeListingApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerExcelDto>();

        CreateMap<Bike, BikeDto>();
        CreateMap<Bike, BikeExcelDto>();
        CreateMap<BikeWithNavigationProperties, BikeWithNavigationPropertiesDto>();
        CreateMap<Manufacturer, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
    }
}