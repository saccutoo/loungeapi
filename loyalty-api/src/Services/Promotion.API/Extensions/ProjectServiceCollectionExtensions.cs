namespace API.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using API.BussinessLogic;
    using API.Interface;

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
            .AddSingleton<IPrmProductHandler, PrmProductHandler>()
            .AddSingleton<IPrmCostAccountHandler, PrmCostAccountHandler>()
            .AddSingleton<IPrmProductHandler, PrmProductHandler>()
            .AddSingleton<IPrmPromotionHandler, PrmPromotionHandler>()
            .AddSingleton<IPrmProductInstanceHandler, PrmProductInstanceHandler>()
            .AddSingleton<ICriteriaConditionHandler, CriteriaConditionHandler>()
            .AddSingleton<IStatusConfigHandler, StatusConfigHandler>()
            .AddSingleton<IPrmTransactionLogHandler, PrmTransactionLogHandler>()
            ;
    }
}