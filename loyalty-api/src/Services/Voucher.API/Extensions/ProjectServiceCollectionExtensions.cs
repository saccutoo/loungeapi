namespace API.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Voucher.API.BussinessLogic;
    using Voucher.API.Interface;

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
            .AddSingleton<IEbankVoucherHandler, EbankVoucherHandler>()
            .AddSingleton<IVucIssueBatchHandler, VucIssueBatchHandler>()
            .AddSingleton<IVucAppliedChannelHandler, VucAppliedChannelHandler>()
            .AddSingleton<IVucVoucherStatusHandler, VucVoucherStatusHandler>()
            .AddSingleton<IVucVoucherTypeHandler, VucVoucherTypeHandler>()
            .AddSingleton<IVucVoucherHandler, VucVoucherHandler>()
            .AddSingleton<IVucVoucherAmtConditionsHandler, VucVoucherAmtConditionsHandler>()
            .AddSingleton<IVucIssueBatchHandler, VucIssueBatchHandler>()
            .AddSingleton<IVucTransTypeHandler, VucTransTypeHandler>()
            .AddSingleton<IVucCustomerHandler, VucCustomerHandler>()
            .AddSingleton<IVucMapVoucherCust, VucMapVoucherCustHandler>();
    }
}