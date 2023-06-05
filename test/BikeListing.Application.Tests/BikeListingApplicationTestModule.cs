using Volo.Abp.Modularity;

namespace BikeListing;

[DependsOn(
    typeof(BikeListingApplicationModule),
    typeof(BikeListingDomainTestModule)
    )]
public class BikeListingApplicationTestModule : AbpModule
{

}
