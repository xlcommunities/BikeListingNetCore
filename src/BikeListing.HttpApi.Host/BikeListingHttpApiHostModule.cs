using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Server.AspNetCore;
using BikeListing.EntityFrameworkCore;
using BikeListing.MultiTenancy;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Public.Web;
using Volo.Abp.Account.Public.Web.Impersonation;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Account;
using Volo.Abp.Account.Public.Web.ExternalProviders;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Microsoft.AspNetCore.Hosting;
using BikeListing.HealthChecks;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;
using Volo.Abp.Swashbuckle;
using Volo.Saas.Host;
using System.Security.Cryptography;

namespace BikeListing;

[DependsOn(
    typeof(BikeListingHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(BikeListingApplicationModule),
    typeof(BikeListingEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpAccountPublicWebImpersonationModule),
    typeof(AbpAccountPublicWebOpenIddictModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule)
    )]
public class BikeListingHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("BikeListing");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(builder =>
            {
                //builder.AddSigningCertificate(GetSigningCertificate(hostingEnvironment, configuration));
                //builder.AddEncryptionCertificate(GetSigningCertificate(hostingEnvironment, configuration));
                builder.AddEncryptionCertificate(LoadCertificate(configuration["AuthServer:EncryptionCertificateThumbprint"]));
                builder.AddSigningCertificate(LoadCertificate(configuration["AuthServer:SigningCertificateThumbprint"]));
                //builder.AddSigningCertificate(GetSigningCertificate(hostingEnvironment, configuration));
                //builder.AddEncryptionCertificate(GetEncryptionCertificate(hostingEnvironment, configuration));
                builder.SetIssuer(new Uri(configuration["AuthServer:Authority"]));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (!Convert.ToBoolean(configuration["App:DisablePII"]))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        if (!Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });
        }

        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureConventionalControllers();
        ConfigureImpersonation(context, configuration);
        ConfigureSwagger(context, configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureCors(context, configuration);
        ConfigureExternalProviders(context);
        ConfigureHealthChecks(context);
    }


    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }

    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddBikeListingHealthChecks();
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.Applications["Angular"].RootUrl = configuration["App:AngularUrl"];
            options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
            options.Applications["Angular"].Urls[AccountUrlNames.EmailConfirmation] = "account/email-confirmation";
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                BasicThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }


    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<BikeListingDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}BikeListing.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<BikeListingDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}BikeListing.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<BikeListingApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}BikeListing.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<BikeListingApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}BikeListing.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(BikeListingApplicationModule).Assembly);
        });
    }

    private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"],
            new Dictionary<string, string>
            {
                    {"BikeListing", "BikeListing API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "BikeListing API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private void ConfigureExternalProviders(ServiceConfigurationContext context)
    {
        context.Services.AddAuthentication()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, _ => { })
            .WithDynamicOptions<GoogleOptions, GoogleHandler>(
                GoogleDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ClientId);
                    options.WithProperty(x => x.ClientSecret, isSecret: true);
                }
            )
            .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
            {
                    //Personal Microsoft accounts as an example.
                    options.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
                options.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
            })
            .WithDynamicOptions<MicrosoftAccountOptions, MicrosoftAccountHandler>(
                MicrosoftAccountDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ClientId);
                    options.WithProperty(x => x.ClientSecret, isSecret: true);
                }
            )
            .AddTwitter(TwitterDefaults.AuthenticationScheme, options => options.RetrieveUserDetails = true)
            .WithDynamicOptions<TwitterOptions, TwitterHandler>(
                TwitterDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ConsumerKey);
                    options.WithProperty(x => x.ConsumerSecret, isSecret: true);
                }
            );
    }

    private void ConfigureImpersonation(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.Configure<AbpAccountOptions>(options =>
        {
            options.TenantAdminUserName = "admin";
            options.ImpersonationTenantPermission = SaasHostPermissions.Tenants.Impersonation;
            options.ImpersonationUserPermission = IdentityPermissions.Users.Impersonation;
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseAbpSecurityHeaders();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();

        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "BikeListing API");

            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }

    //private X509Certificate2 GetSigningCertificate(IWebHostEnvironment hostingEnv, IConfiguration configuration)
    //{
    //    var fileName = "authserver.pfx";
    //    var passPhrase = "2D7AA457-5D33-48D6-936F-C48E5EF468ED";
    //    //var passPhrase = "2D7AA457-5D33-48D6-100F-C48E5EF468ED";
    //    //var passPhrase = "enJJRmkTMQ2qZC*6Dj*s";
    //    var file = Path.Combine(hostingEnv.ContentRootPath, fileName);

    //    if (!File.Exists(file))
    //    {
    //        throw new FileNotFoundException($"1111 Signing Certificate couldn't found: {file}");
    //    }

    //    return new X509Certificate2(file, passPhrase);
    //}

    private X509Certificate2 LoadCertificate(string thumbprint)
    {
        //var bytes = File.ReadAllBytes($"/var/ssl/private/{thumbprint}.p12");
        //return new X509Certificate2(bytes);

        using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
        {
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                        X509FindType.FindByThumbprint,
                                        // Replace below with your certificate's thumbprint
                                        thumbprint,
                                        false);
            // Get the first cert with the thumbprint
            X509Certificate2 cert = certCollection.OfType<X509Certificate2>().FirstOrDefault();

            if (cert is null)
                throw new Exception($"Certificate with thumbprint {thumbprint} was not found");

            // Use certificate
            return cert;

            // Consider to call Dispose() on the certificate after it's being used, avaliable in .NET 4.6 and later
        }
    }

    private X509Certificate2 GetSigningCertificate(IWebHostEnvironment hostingEnv,
                                IConfiguration configuration)
    {
        var fileName = $"cert-signing.pfx";
        var passPhrase = configuration["StringEncryption:DefaultPassPhrase"];
        var file = Path.Combine(hostingEnv.ContentRootPath, fileName);
        if (File.Exists(file))
        {
            var created = File.GetCreationTime(file);
            var days = (DateTime.Now - created).TotalDays;
            if (days > 180)
                File.Delete(file);
            else
                return new X509Certificate2(file, passPhrase,
                             X509KeyStorageFlags.MachineKeySet);
        }

        // file doesn't exist or was deleted because it expired
        using var algorithm = RSA.Create(keySizeInBits: 2048);
        var subject = new X500DistinguishedName("CN=Fabrikam Signing Certificate");
        var request = new CertificateRequest(subject, algorithm,
                            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(
                            X509KeyUsageFlags.DigitalSignature, critical: true));
        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow,
                            DateTimeOffset.UtcNow.AddYears(2));
        File.WriteAllBytes(file, certificate.Export(X509ContentType.Pfx, string.Empty));
        return new X509Certificate2(file, passPhrase,
                            X509KeyStorageFlags.MachineKeySet);
    }

    private X509Certificate2 GetEncryptionCertificate(IWebHostEnvironment hostingEnv,
                                 IConfiguration configuration)
    {
        var fileName = $"cert-encryption.pfx";
        var passPhrase = configuration["StringEncryption:DefaultPassPhrase"];
        var file = Path.Combine(hostingEnv.ContentRootPath, fileName);
        if (File.Exists(file))
        {
            var created = File.GetCreationTime(file);
            var days = (DateTime.Now - created).TotalDays;
            if (days > 180)
                File.Delete(file);
            else
                return new X509Certificate2(file, passPhrase,
                                X509KeyStorageFlags.MachineKeySet);
        }

        // file doesn't exist or was deleted because it expired
        using var algorithm = RSA.Create(keySizeInBits: 2048);
        var subject = new X500DistinguishedName("CN=Fabrikam Encryption Certificate");
        var request = new CertificateRequest(subject, algorithm,
                            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(
                            X509KeyUsageFlags.KeyEncipherment, critical: true));
        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow,
                            DateTimeOffset.UtcNow.AddYears(2));
        File.WriteAllBytes(file, certificate.Export(X509ContentType.Pfx, string.Empty));
        return new X509Certificate2(file, passPhrase, X509KeyStorageFlags.MachineKeySet);
    }
}
