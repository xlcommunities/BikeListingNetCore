using Volo.Abp.Settings;

namespace BikeListing.Settings;

public class BikeListingSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(BikeListingSettings.MySetting1));
    }
}
