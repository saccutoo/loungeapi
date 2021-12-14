using API.BussinessLogic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace VoucherUrbox.Jobs.Bussiness
{
    public class SyncSVToPortalHostedServices : IHostedService
    {

        private int executionCount = 0;
        private readonly ILogger<SyncSVToPortalHostedServices> _logger;

        private readonly ICustomerHandler _iCustomerHandler;
        private Timer _timer;
        private bool isRunning = false;

        //private IConfigurationRoot _configRoot;
        public SyncSVToPortalHostedServices(ILogger<SyncSVToPortalHostedServices> logger, ICustomerHandler iCustomerHandler)
        {
            _logger = logger;
            _iCustomerHandler = iCustomerHandler;
            //_configRoot = configRoot;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SyncSVToPortalHostedServices running.");
            var timeInterval = Helpers.GetConfig("SVConfig:TimmerInterval");
            _logger.LogInformation(
                "SyncSVToPortalHostedServices timeInterval: {timeInterval}", timeInterval);
            var timeIntervalInt = 0;
            int.TryParse(timeInterval, out timeIntervalInt);
            if(timeIntervalInt > 1)
            {
                _timer = new Timer(async o => await DoWorkAsync(o), null, TimeSpan.Zero,
                TimeSpan.FromSeconds(timeIntervalInt));
            }
            

            return Task.CompletedTask;
        }

        private async Task DoWorkAsync(object state)
        {
            var count = Interlocked.Increment(ref executionCount);
            if (isRunning)
            {
                return;
            }
            var isStopConfig = Helpers.GetConfig("SVConfig:IsStop");
            if (isStopConfig == "STOP")
            {
                _logger.LogInformation("SyncOOSToPortalHostedServices configured stop");
                return;
            }
            isRunning = true;
            
            try
            {
                await _iCustomerHandler.SyncFromSmartVistaToPortal();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"SyncOOSToPortalHostedServices Exception");
                isRunning = false;
            }
            isRunning = false;
            _logger.LogInformation(
                "SyncSVToPortalHostedServices is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SyncSVToPortalHostedServices is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
