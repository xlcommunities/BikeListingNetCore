using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;

using Volo.Abp;

namespace BikeListing.Manufacturers
{
    public class Manufacturer : FullAuditedEntity<Guid>, IHasConcurrencyStamp
    {
        [NotNull]
        public virtual string Name { get; set; }

        public string ConcurrencyStamp { get; set; }

        public Manufacturer()
        {

        }

        public Manufacturer(Guid id, string name)
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
            Id = id;
            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), ManufacturerConsts.NameMaxLength, ManufacturerConsts.NameMinLength);
            Name = name;
        }

    }
}