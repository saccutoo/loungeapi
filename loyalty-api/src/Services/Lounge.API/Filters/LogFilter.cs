using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace API.Filters
{
    public class LogFilter : IActionFilter
    {

        private readonly ILogger<LogFilter> _logger;
        public LogFilter(ILogger<LogFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                //var response = context.HttpContext.Response;
                //var responseBody = response.Body;
                //var bodyString = JsonConvert.SerializeObject(responseBody);
                var contextMethod = context.HttpContext.Request.Method;
                _logger.LogInformation("API END Method:" + contextMethod.ToString());
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "OnActionExecuted Error");
            }
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var contextPath = context.HttpContext.Request.Path;
                var contextMethod = context.HttpContext.Request.Method;
                _logger.LogInformation("API START path: " + contextPath.ToString());
                _logger.LogInformation("API START Method: " + contextMethod.ToString());

                var headers = context.HttpContext.Request.Headers;
                var headerString = JsonConvert.SerializeObject(headers);
                _logger.LogInformation("API START Headers: " + headerString);

                var param = context.HttpContext.Request.Query;
                var paramString = JsonConvert.SerializeObject(param);
                _logger.LogInformation("API START params: " + paramString);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "OnActionExecuting Error");
            }

            
        }
    }
}
