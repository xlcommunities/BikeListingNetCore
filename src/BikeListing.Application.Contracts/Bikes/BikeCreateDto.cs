using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BikeListing.Bikes
{
    public class BikeCreateDto
    {
        [Required]
        [StringLength(BikeConsts.ModelMaxLength, MinimumLength = BikeConsts.ModelMinLength)]
        public string Model { get; set; }
        public int FrameSize { get; set; }
        public decimal Price { get; set; }
        public Guid ManufacturerId { get; set; }
    }
}