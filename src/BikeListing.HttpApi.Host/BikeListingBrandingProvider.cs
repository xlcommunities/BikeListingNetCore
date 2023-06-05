using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace BikeListing;

[Dependency(ReplaceServices = true)]
public class BikeListingBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "BikeListing";
}
