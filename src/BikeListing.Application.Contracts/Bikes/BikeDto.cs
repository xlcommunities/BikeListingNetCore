using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace BikeListing.Bikes
{
    public class BikeDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Model { get; set; }
        public int FrameSize { get; set; }
        public decimal Price { get; set; }
        public Guid ManufacturerId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}