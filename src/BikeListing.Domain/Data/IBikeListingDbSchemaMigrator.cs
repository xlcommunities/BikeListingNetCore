using System.Threading.Tasks;

namespace BikeListing.Data;

public interface IBikeListingDbSchemaMigrator
{
    Task MigrateAsync();
}
