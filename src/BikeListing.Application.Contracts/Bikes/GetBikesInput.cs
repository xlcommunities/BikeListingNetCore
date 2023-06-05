using Volo.Abp.Application.Dtos;
using System;

namespace BikeListing.Bikes
{
    public class GetBikesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? Model { get; set; }
        public int? FrameSizeMin { get; set; }
        public int? FrameSizeMax { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public Guid? ManufacturerId { get; set; }

        public GetBikesInput()
        {

        }
    }
}