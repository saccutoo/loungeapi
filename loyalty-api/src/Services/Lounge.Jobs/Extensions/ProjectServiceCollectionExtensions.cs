namespace API.Extensions
{
    using global::API.BussinessLogic;
    using Microsoft.Extensions.DependencyInjection;
    using VoucherUrbox.Jobs.Bussiness;

    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods add project services.
    /// </summary>
    /// <remarks>
    /// AddSingleton - Only one instance is ever created and returned.
    /// AddScoped - A new instance is created and returned for each request/response cycle.
    /// AddTransient - A new instance is created and returned each time.
    /// </remarks>
    public static class ProjectServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services) => services            
            .AddSingleton<ICustomerHandler, CustomerHandler>()
            .AddHostedService<SyncSVToPortalHostedServices>()
            .AddHostedService<SyncOOSToPortalHostedServices>()
            ;

    }
}