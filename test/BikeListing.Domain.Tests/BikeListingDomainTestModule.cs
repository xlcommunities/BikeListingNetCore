using BikeListing.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace BikeListing;

[DependsOn(
    typeof(BikeListingEntityFrameworkCoreTestModule)
    )]
public class BikeListingDomainTestModule : AbpModule
{

}
