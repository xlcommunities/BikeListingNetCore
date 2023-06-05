using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace BikeListing.Bikes
{
    public class BikeUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        [StringLength(BikeConsts.ModelMaxLength, MinimumLength = BikeConsts.ModelMinLength)]
        public string Model { get; set; }
        public int FrameSize { get; set; }
        public decimal Price { get; set; }
        public Guid ManufacturerId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}