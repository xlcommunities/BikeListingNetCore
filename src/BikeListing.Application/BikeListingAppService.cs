using BikeListing.Localization;
using Volo.Abp.Application.Services;

namespace BikeListing;

/* Inherit your application services from this class.
 */
public abstract class BikeListingAppService : ApplicationService
{
    protected BikeListingAppService()
    {
        LocalizationResource = typeof(BikeListingResource);
    }
}
