using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace BikeListing.Manufacturers
{
    public class ManufacturerUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        [StringLength(ManufacturerConsts.NameMaxLength, MinimumLength = ManufacturerConsts.NameMinLength)]
        public string Name { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}