using System.Text;
using SQKLocalServe.Common.Logging;

namespace SQKLocalServe_1.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogManager _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogManager logger)
    {
        _next   = next   ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        // 1️⃣  --- Log REQUEST --------------------------------------------------
        string requestText = await FormatRequestAsync(context.Request);
        _logger.LogInfo($"HTTP {context.Request.Method} {context.Request.Path} - Request: {requestText}");

        // 2️⃣  --- Capture RESPONSE -------------------------------------------
        Stream originalBody = context.Response.Body;
        await using var responseBuffer = new MemoryStream();
        context.Response.Body = responseBuffer;

        try
        {
            await _next(context);               // ─▶ other middleware / MVC

            // 3️⃣  --- Log RESPONSE ------------------------------------------
            string responseText = await FormatResponseAsync(responseBuffer, context.Response.ContentType);
            _logger.LogInfo($"HTTP {context.Request.Method} {context.Request.Path} - Response: {responseText}");

            // 4️⃣  --- Flush captured body back to the client ----------------
            responseBuffer.Position = 0;
            await responseBuffer.CopyToAsync(originalBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unhandled exception while processing {context.Request.Method} {context.Request.Path}");
            throw;                               // keep the exception semantics intact
        }
        finally
        {
            context.Response.Body = originalBody; // always restore
        }
    }

    /*─────────────────────────  helpers  ─────────────────────────*/

    private static async Task<string> FormatRequestAsync(HttpRequest request)
    {
        request.EnableBuffering();

        await using var buffer = new MemoryStream();
        await request.Body.CopyToAsync(buffer);
        string body = Encoding.UTF8.GetString(buffer.ToArray());

        request.Body.Position = 0;               // let MVC read it again
        return body;
    }

    private static async Task<string> FormatResponseAsync(Stream responseStream, string? contentType)
    {
        if (!responseStream.CanSeek)
        {
            // rare edge-case: swap into a seek-able stream
            await using var copy = new MemoryStream();
            await responseStream.CopyToAsync(copy);
            responseStream = copy;
        }

        responseStream.Position = 0;
        var encoding = TryGetEncoding(contentType) ?? Encoding.UTF8;

        using var reader = new StreamReader(responseStream, encoding, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        string text = await reader.ReadToEndAsync();
        responseStream.Position = 0;

        return text;
    }

    private static Encoding? TryGetEncoding(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType)) return null;

        var charset = contentType.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(p => p.Trim())
                                 .FirstOrDefault(p => p.StartsWith("charset=", StringComparison.OrdinalIgnoreCase))
                                 ?.Substring("charset=".Length);

        return string.IsNullOrWhiteSpace(charset) ? null : Encoding.GetEncoding(charset);
    }
}