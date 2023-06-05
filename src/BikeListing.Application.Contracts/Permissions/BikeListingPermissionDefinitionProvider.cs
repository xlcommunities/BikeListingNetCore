using BikeListing.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace BikeListing.Permissions;

public class BikeListingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(BikeListingPermissions.GroupName);

        myGroup.AddPermission(BikeListingPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(BikeListingPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(BikeListingPermissions.MyPermission1, L("Permission:MyPermission1"));

        var manufacturerPermission = myGroup.AddPermission(BikeListingPermissions.Manufacturers.Default, L("Permission:Manufacturers"));
        manufacturerPermission.AddChild(BikeListingPermissions.Manufacturers.Create, L("Permission:Create"));
        manufacturerPermission.AddChild(BikeListingPermissions.Manufacturers.Edit, L("Permission:Edit"));
        manufacturerPermission.AddChild(BikeListingPermissions.Manufacturers.Delete, L("Permission:Delete"));

        var bikePermission = myGroup.AddPermission(BikeListingPermissions.Bikes.Default, L("Permission:Bikes"));
        bikePermission.AddChild(BikeListingPermissions.Bikes.Create, L("Permission:Create"));
        bikePermission.AddChild(BikeListingPermissions.Bikes.Edit, L("Permission:Edit"));
        bikePermission.AddChild(BikeListingPermissions.Bikes.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BikeListingResource>(name);
    }
}