using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BikeListing.Data;
using Volo.Abp.DependencyInjection;

namespace BikeListing.EntityFrameworkCore;

public class EntityFrameworkCoreBikeListingDbSchemaMigrator
    : IBikeListingDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreBikeListingDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the BikeListingDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<BikeListingDbContext>()
            .Database
            .MigrateAsync();
    }
}
