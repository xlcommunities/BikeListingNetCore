using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace BikeListing.Data;

/* This is used if database provider does't define
 * IBikeListingDbSchemaMigrator implementation.
 */
public class NullBikeListingDbSchemaMigrator : IBikeListingDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
