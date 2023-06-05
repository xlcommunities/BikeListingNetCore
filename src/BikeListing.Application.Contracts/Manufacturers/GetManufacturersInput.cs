using Volo.Abp.Application.Dtos;
using System;

namespace BikeListing.Manufacturers
{
    public class GetManufacturersInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? Name { get; set; }

        public GetManufacturersInput()
        {

        }
    }
}