public interface ILogManager
{
    public void LogInfo(string message);
    public void LogError(string message);
    public void LogError(Exception ex, string msg);
}