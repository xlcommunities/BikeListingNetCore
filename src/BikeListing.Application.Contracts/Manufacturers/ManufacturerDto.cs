using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace BikeListing.Manufacturers
{
    public class ManufacturerDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}