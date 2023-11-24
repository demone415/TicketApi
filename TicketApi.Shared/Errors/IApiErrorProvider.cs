using System.Net;
using Microsoft.Extensions.Logging;

namespace TicketApi.Shared.Errors;

/// <summary>
/// Интерфейс для облегчения создания стандартных объектов ошибок http для бэка/фронта с понятными причинами
/// и уровнем критичности для логирования
/// </summary>
public interface IApiErrorProvider
{
    /// <summary>
    /// Код ошибки для возможности реализации кастомной логики, показа сообщения/экрана/перенаправления/побуждения к конкретному действию пользователя
    /// </summary>
    string Code { get; }

    /// <summary>Описание ошибки</summary>
    string Message { get; }

    /// <summary>Критичность для логирования</summary>
    LogLevel LogLevel { get; }

    /// <summary>HTTP статус</summary>
    HttpStatusCode HttpStatusCode { get; }
}