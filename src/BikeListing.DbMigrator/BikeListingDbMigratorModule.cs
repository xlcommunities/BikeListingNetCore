using BikeListing.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace BikeListing.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(BikeListingEntityFrameworkCoreModule),
    typeof(BikeListingApplicationContractsModule)
)]
public class BikeListingDbMigratorModule : AbpModule
{

}
