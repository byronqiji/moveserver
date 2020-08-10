using log4net;
using log4net.Repository;

namespace DBMoveServer.LogUtility
{
    public class LoggerFactory
    {
        public static ILoggerRepository Repository { get; private set; }

        static LoggerFactory()
        {
            Repository = LogManager.GetRepository(typeof(LoggerFactory).Assembly);
        }

        public static ILog CreateLogger(string loggerName)
        {
            return LogManager.GetLogger(Repository.Name, loggerName);
        }
    }
}
