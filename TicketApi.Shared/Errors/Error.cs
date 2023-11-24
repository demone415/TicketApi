using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TicketApi.Shared.Errors;

/// <summary>
/// Модель ошибки.
/// По мотивам https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#7102-error-condition-responses
/// </summary>
public class Error
{
    private string? _target;

    /// <summary>Код ошибки</summary>
    public string? Code { get; set; }

    /// <summary>
    /// Если ошибка относится к какому-то полю входной модели, то здесь должно быть его имя.
    /// </summary>
    public string? Target
    {
        get => _target;
        set => _target = string.IsNullOrWhiteSpace(value)
            ? null
            : Default.JsonOptions.PropertyNamingPolicy.ConvertName(value);
    }

    /// <summary>Описание ошибки</summary>
    public string? Message { get; set; }

    /// <summary>Значение пришедшее от клиента</summary>
    public string? AttemptedValue { get; set; }

    /// <summary>Если ошибок несколько, то они здесь</summary>
    public List<Error>? Details { get; set; }

    /// <summary>.ctor</summary>
    public Error()
    {
    }

    /// <summary>.ctor</summary>
    /// <param name="code">Код ошибки</param>
    /// <param name="msg">Описание ошибки</param>
    /// <param name="target">Название поля с ошибкой</param>
    public Error(string code, string msg, string? target = null)
    {
        Message = msg;
        Code = code;
        Target = target;
    }

    /// <summary>.ctor</summary>
    /// <param name="status">Статус ответа</param>
    /// <param name="msg">Сообщение об ошибке</param>
    /// <param name="target">Название поля с ошибкой</param>
    public Error(HttpStatusCode status, string msg, string? target = null)
    {
        var str = status switch
        {
            HttpStatusCode.BadRequest => "BadArgument",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.NotFound => "NotFound",
            _ => "UnexpectedError"
        };
        Code = str;
        Message = msg;
        Target = target;
    }

    /// <summary>.ctor</summary>
    /// <param name="modelState">Состояние невалидной модели</param>
    public Error(ModelStateDictionary modelState)
    {
        if (modelState.IsValid)
            return;
        Code = "BadArgument";
        var errorList = new List<Error>();
        foreach (var (key, modelStateEntry) in modelState.Where((Func<KeyValuePair<string, ModelStateEntry>, bool>)(_ =>
                 {
                     var modelStateEntry = _.Value;
                     return modelStateEntry != null && modelStateEntry.ValidationState == ModelValidationState.Invalid;
                 })))
            errorList.Add(new Error
            {
                Target = key,
                Message = string.Join("; ",
                    modelStateEntry.Errors.Select((Func<ModelError, string>)(_ => _.ErrorMessage))),
                AttemptedValue = modelStateEntry.AttemptedValue
            });

        var count = errorList.Count;
        if (count <= 1)
        {
            if (count != 1)
                return;
            Message = errorList[0].Message;
            Target = errorList[0].Target;
            AttemptedValue = errorList[0].AttemptedValue;
        }
        else
        {
            Details = errorList;
            Message = "Ошибки в аргументах";
        }
    }

    /// <summary>Коды ошибок</summary>
    public static class Codes
    {
        /// <summary>Ошибка в аргументах</summary>
        public const string BadArgument = "BadArgument";

        /// <summary>Необработанное исключение</summary>
        public const string UnexpectedError = "UnexpectedError";

        /// <summary>Таймаут</summary>
        public const string Timeout = "Timeout";

        /// <summary>Ресурс не найден</summary>
        public const string NotFound = "NotFound";

        /// <summary>Запрос не авторизован</summary>
        public const string Unauthorized = "Unauthorized";

        /// <summary>Доступ запрещён</summary>
        public const string Forbidden = "Forbidden";

        /// <summary>Запрос отменён</summary>
        public const string Aborted = "RequestAborted";

        /// <summary>Слишком много запросов</summary>
        public const string TooManyRequests = "TooManyRequests";
    }
}