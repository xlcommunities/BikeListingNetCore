using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BikeListing.Manufacturers
{
    public class ManufacturerCreateDto
    {
        [Required]
        [StringLength(ManufacturerConsts.NameMaxLength, MinimumLength = ManufacturerConsts.NameMinLength)]
        public string Name { get; set; }
    }
}