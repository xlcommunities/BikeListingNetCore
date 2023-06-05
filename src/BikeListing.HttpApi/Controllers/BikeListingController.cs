using BikeListing.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace BikeListing.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class BikeListingController : AbpControllerBase
{
    protected BikeListingController()
    {
        LocalizationResource = typeof(BikeListingResource);
    }
}
