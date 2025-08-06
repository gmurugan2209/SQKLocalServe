using NLog;
using ILogger = NLog.ILogger;

namespace SQKLocalServe.Common.Logging;

public interface ILogManager
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogError(Exception ex, string message = "");
}

public class NLogManager : ILogManager
{
    private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

    public void LogInfo(string message)
    {
        logger.Info(message);
    }

    public void LogWarning(string message)
    {
        logger.Warn(message);
    }

    public void LogError(string message)
    {
        logger.Error(message);
    }

    public void LogError(Exception ex, string message = "")
    {
        logger.Error(ex, message);
    }
}
