namespace API.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using API.BussinessLogic;
    using API.Interface;
    using Lounge.API;

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
            .AddSingleton<IElgConfigurateHandler, ElgConfigurateHandler>()
            .AddSingleton<IElgStarffHandler, ElgStarffHandler>()
            .AddSingleton<IElgCustomerClassHandler, ElgCustomerClassHandler>()
            .AddSingleton<IElgCustClassConditionHandler, ElgCustClassConditionHandler>()
            .AddSingleton<IElgCustomerHandler, ElgCustomerHandler>()
            .AddSingleton<IElgCustomerInfoHandler, ElgCustomerInfoHandler>()
            .AddSingleton<IElgCustomerTypeHandler, ElgCustomerTypeHandler>()
            .AddSingleton<IElgBookingsHandler, ElgBookingsHandler>()
            .AddSingleton<IElgBookingStatusHandler, ElgBookingStatusHandler>()
            .AddSingleton<IElgLoungeHandler, ElgLoungeHandler>()
            .AddSingleton<IElgCheckinHandler, ElgCheckinHandler>()
            .AddSingleton<IElgBookingCheckoutHandler, ElgBookingCheckoutHandler>()
            .AddSingleton<ISrvQuestionsOptionsHandler, SrvQuestionsOptionsHandler>()
            .AddSingleton<ISrvSurveyQuestionsHandler, SrvSurveyQuestionsHandler>()
            .AddSingleton<ISrvSurveySectionsHandler, SrvSurveySectionsHandler>()
            .AddSingleton<IElgReviewQualityHandler, ElgReviewQualityHandler>()
            .AddSingleton<IElgReportHandler, ElgReportHandler>()
            .AddSingleton<IElgVoucherPosHandler, ElgVoucherPosHandler>()
            .AddSingleton<IElgVoucherMappingHandler, ElgVoucherMappingHandler>()
            .AddSingleton<IElgNotificationHandler, ElgNotificationHandler>()
            .AddSingleton<IElgFaceCustomerHandler, ElgFaceCustomerHandler>()
             .AddSingleton<IRedisService, RedisService>()
            ;
    }
}