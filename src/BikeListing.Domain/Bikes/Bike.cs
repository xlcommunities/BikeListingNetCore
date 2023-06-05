using BikeListing.Manufacturers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace BikeListing.Bikes
{
    public class Bike : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Model { get; set; }

        public virtual int FrameSize { get; set; }

        public virtual decimal Price { get; set; }
        public Guid ManufacturerId { get; set; }

        public Bike()
        {

        }

        public Bike(Guid id, Guid manufacturerId, string model, int frameSize, decimal price)
        {

            Id = id;
            Check.NotNull(model, nameof(model));
            Check.Length(model, nameof(model), BikeConsts.ModelMaxLength, BikeConsts.ModelMinLength);
            Model = model;
            FrameSize = frameSize;
            Price = price;
            ManufacturerId = manufacturerId;
        }

    }
}