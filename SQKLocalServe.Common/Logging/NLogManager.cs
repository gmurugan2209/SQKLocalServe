using NLog;

public sealed class NLogManager : ILogManager
{
    private readonly Logger _logger;

    public NLogManager()
    {
        _logger = LogManager.GetCurrentClassLogger();
    }

    public void LogInfo(string message) => _logger.Info(message);
    public void LogError(string message) => _logger.Error(message);
    public void LogError(Exception ex, string msg) => _logger.Error(ex, msg);
}