using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Logging
{
    public class LogAttribute : ActionFilterAttribute
    {
        private readonly ILogger<LogAttribute> _logger;
        public LogAttribute(ILogger<LogAttribute> logger)
        {
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                _logger.LogInformation($"Started: {context.Controller.GetType().Name} : {context.ActionDescriptor.DisplayName}");
            }
            finally
            {

            }
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                _logger.LogInformation($"Completed: {context.Controller.GetType().Name} : {context.ActionDescriptor.DisplayName}");
            }
            finally
            {

            }
        }
    }
}
