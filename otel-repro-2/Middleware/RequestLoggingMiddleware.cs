using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _log;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        Stream originalResponseBodyAsStream = SubstituteResponseBodyStream(httpContext.Response);

        using (httpContext.Response.Body)
        {
            try
            {
                _log.LogDebug(await GetRequestAsString(httpContext.Request));

                await _next.Invoke(httpContext);
            }
            finally
            {
                _log.LogDebug(await GetResponseAsString(httpContext.Response));

                await RestoreResponseBodyStream(httpContext.Response, originalResponseBodyAsStream);
            }
        }
    }

    private static Stream SubstituteResponseBodyStream(HttpResponse httpResponse)
    {
        Stream originalResponseBody = httpResponse.Body;

        httpResponse.Body = new MemoryStream();
        return originalResponseBody;
    }

    private async Task RestoreResponseBodyStream(HttpResponse httpResponse, Stream originalResponseBodyAsStream)
    {
        httpResponse.Body.Position = 0;

        await httpResponse.Body.CopyToAsync(originalResponseBodyAsStream);

        httpResponse.Body = originalResponseBodyAsStream;
    }

    private async Task<string> GetRequestAsString(HttpRequest httpRequest)
    {
        var message = new StringBuilder()
            .AppendLine("HTTP REQUEST:")
            .AppendLine("=============")
            .AppendLine(GetRequestEndpoint(httpRequest))
            .AppendLine(GetHeaders(httpRequest.Headers))
            .AppendLine(await GetRequestBodyAsString(httpRequest))
            .ToString().Trim();

        return message;
    }

    private async Task<string> GetRequestBodyAsString(HttpRequest httpRequest)
    {
        using (var requestBodyReader = new StreamReader(httpRequest.Body))
        {
            string httpRequestBodyAsString = await requestBodyReader.ReadToEndAsync();

            var httpRequestBodyAsBytes = Encoding.UTF8.GetBytes(httpRequestBodyAsString);

            httpRequest.Body = new MemoryStream(httpRequestBodyAsBytes);

            return httpRequestBodyAsString;
        }
    }

    private async Task<string> GetResponseAsString(HttpResponse httpResponse)
    {
        var message = new StringBuilder()
            .AppendLine("HTTP RESPONSE:")
            .AppendLine("==============")
            .AppendLine(GetRequestEndpoint(httpResponse.HttpContext.Request))
            .AppendLine(GetHeaders(httpResponse.Headers))
            .AppendLine(await GetResponseBodyAsString(httpResponse))
            .ToString().Trim();

        return message;
    }

    private Task<string> GetResponseBodyAsString(HttpResponse httpResponse)
    {
        httpResponse.Body.Position = 0;

        return GetBodyAsString(httpResponse.Body);
    }

    private string GetRequestEndpoint(HttpRequest httpRequest)
    {
        return $"{httpRequest.Method} {httpRequest.Path}{httpRequest.QueryString} {httpRequest.Protocol}";
    }

    private string GetHeaders(IHeaderDictionary headers)
    {
        var messageBuilder = new StringBuilder();

        headers.OrderBy(h => h.Key).ToList().ForEach(
            header =>
                messageBuilder
                    .AppendLine($"{header.Key}: {header.Value}")
        );

        return messageBuilder.ToString().Trim();
    }

    private async Task<string> GetBodyAsString(Stream body)
    {
        return await new StreamReader(body).ReadToEndAsync();
    }
}