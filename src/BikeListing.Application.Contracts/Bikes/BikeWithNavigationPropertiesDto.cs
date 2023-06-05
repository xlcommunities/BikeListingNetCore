using BikeListing.Manufacturers;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace BikeListing.Bikes
{
    public class BikeWithNavigationPropertiesDto
    {
        public BikeDto Bike { get; set; }

        public ManufacturerDto Manufacturer { get; set; }

    }
}