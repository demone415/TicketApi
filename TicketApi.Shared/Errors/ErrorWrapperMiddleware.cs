using System.Runtime.ExceptionServices;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Polly.Timeout;
using Refit;

namespace TicketApi.Shared.Errors;

/// <summary>
///     Оборачивает неотловленные исключения в объект <see cref="T:Sravni.Micro.Errors.ErrorModel" />.
/// </summary>
public class ErrorWrapperMiddleware
{
    private readonly ILogger<ErrorWrapperMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>.ctor</summary>
    public ErrorWrapperMiddleware(RequestDelegate next, ILogger<ErrorWrapperMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Тело MW</summary>
    public Task Invoke(HttpContext context)
    {
        ExceptionDispatchInfo edi;
        try
        {
            var task = _next(context);
            return !task.IsCompletedSuccessfully ? Awaited(this, context, task) : Task.CompletedTask;
        }
        catch (Exception ex)
        {
            edi = ExceptionDispatchInfo.Capture(ex);
        }

        return HandleException(context, edi);

        static async Task Awaited(ErrorWrapperMiddleware middleware, HttpContext context, Task task)
        {
            ExceptionDispatchInfo edi = null;
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                edi = ExceptionDispatchInfo.Capture(ex);
            }

            if (edi == null)
            {
                edi = null;
            }
            else
            {
                await middleware.HandleException(context, edi);
                edi = null;
            }
        }
    }

    /// <summary>
    ///     Оборачивает исключение в JSON-объект, устанавливает код ответа
    ///     и возвращает ответ с информацией об исключении.
    /// </summary>
    protected virtual async Task HandleException(HttpContext context, ExceptionDispatchInfo edi)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Не можем обработать исключение, т.к. уже начали писать ответ");
            edi.Throw();
        }

        context.Response.Clear();
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        ClearCacheHeaders(context.Response.Headers);
        var logLevel = LogLevel.Error;
        var sourceException = edi.SourceException;
        Error error;
        switch (sourceException)
        {
            case TimeoutException timeoutException:
                context.Response.StatusCode = 504;
                error = new Error("Timeout", timeoutException.Message);
                break;
            case TimeoutRejectedException rejectedException:
                context.Response.StatusCode = 504;
                error = new Error("Timeout", rejectedException.Message);
                break;
            case ApiException apiException:
                context.Response.StatusCode = (int)apiException.StatusCode;
                error = MakeErrorModel(apiException);
                break;
            case OperationCanceledException _:
                context.Response.StatusCode = 499;
                logLevel = LogLevel.Information;
                error = new Error("RequestAborted", "Request aborted");
                break;
            case IApiErrorProvider apiErrorProvider:
                context.Response.StatusCode = (int)apiErrorProvider.HttpStatusCode;
                logLevel = apiErrorProvider.LogLevel;
                error = new Error(apiErrorProvider.Code, apiErrorProvider.Message);
                break;
            default:
                error = new Error("UnexpectedError", sourceException.GetType().Name + ": " + sourceException.Message);
                break;
        }

        _logger.Log(logLevel, sourceException, "Необработанное исключение: {@Error}", error);
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorModel(error), Default.JsonOptions));
    }

    private static void ClearCacheHeaders(IHeaderDictionary headers)
    {
        headers.CacheControl = (StringValues)"no-cache,no-store";
        headers.Pragma = (StringValues)"no-cache";
        headers.Expires = (StringValues)"-1";
        headers.ETag = new StringValues();
    }

    private static Error MakeErrorModel(ApiException apiException)
    {
        Error error = null;
        if (apiException.HasContent)
            try
            {
                error = JsonSerializer.Deserialize<ErrorModel>(apiException.Content, Default.JsonOptions)?.Error;
            }
            catch (JsonException ex)
            {
            }

        return error ?? new Error(apiException.StatusCode, apiException.Content ?? "");
    }
}