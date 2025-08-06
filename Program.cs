// Configure NLog
var logger = NLog.LogManager.Setup()
    .LoadConfigurationFromFile("nlog.config")
    .GetCurrentClassLogger();

try
{
    logger.Info("Starting application");

    // Initialize NLog
    var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
    logger.Info("Application Starting Up");

    try
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure NLog for ASP.NET Core
        builder.Host.UseNLog();

    // Configure NLog for ASP.NET Core
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
