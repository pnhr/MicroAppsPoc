using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PS.Master.Logging.Database;
using PS.Master.Logging.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Logging
{
    public class DbLogger : ILogger
    {
        private readonly string name;
        private readonly Func<DbLoggerConfiguration> getCurrentConfig;
        private readonly IServiceProvider serviceProvider;
        private readonly AppLoggingDbContext loggerDbContext;

        public DbLogger(string name, IConfiguration configuration, Func<DbLoggerConfiguration> getCurrentConfig)
        {
            this.name = name;
            this.getCurrentConfig = getCurrentConfig;
            loggerDbContext = new AppLoggingDbContext(configuration);
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return getCurrentConfig().LogLevel.Contains(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                if (!IsEnabled(logLevel))
                    return;

                string? message = formatter(state, exception);
                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Information:
                    case LogLevel.Warning:
                    case LogLevel.Critical:
                        LogMessage(message, logLevel);
                        break;
                    case LogLevel.Error:
                        LogError(message, exception);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        private void LogMessage(string message, LogLevel logLevel)
        {
            try
            {
                ActivityLog activityLog = new ActivityLog();
                activityLog.Message = message;
                activityLog.LogLevelId = (int)logLevel;
                activityLog.UrlOrModule = "-";
                activityLog.LogDateTime = DateTime.Now;
                activityLog.UserId = "CORP\\e999999";

                loggerDbContext.ActivityLogs.Add(activityLog);
                loggerDbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
        }
        private void LogCritical(string message, Exception? exception = null)
        {
            try
            {
                ActivityLog activityLog = new ActivityLog();
                activityLog.Message = message;
                activityLog.LogLevelId = (int)LogLevel.Critical;
                activityLog.UrlOrModule = "-";
                activityLog.LogDateTime = DateTime.Now;
                activityLog.UserId = "CORP\\e999999";

                loggerDbContext.ActivityLogs.Add(activityLog);
                loggerDbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
        }

        private void LogError(string message, Exception? exception)
        {
            try
            {
                ErrorLog errorLog = new ErrorLog();
                errorLog.Message = message;
                errorLog.LogLevelId = (int)LogLevel.Error;
                errorLog.ErrorDateTime = DateTime.Now;
                errorLog.UrlOrModule = "-";

                if (exception != null)
                {
                    errorLog.ClassName = exception.TargetSite?.DeclaringType?.FullName;
                    errorLog.MethodName = "";
                    errorLog.StackTrace = exception.StackTrace ?? "-";
                    errorLog.ErrorType = exception.GetType()?.FullName ?? "-";
                }


                loggerDbContext.ErrorLogs.Add(errorLog);
                loggerDbContext.SaveChanges();

                if (exception?.InnerException != null)
                    LogError(exception.InnerException.Message, exception.InnerException);
            }
            catch(Exception ex)
            {

            }
            finally
            {
            }
        }
    }
}
