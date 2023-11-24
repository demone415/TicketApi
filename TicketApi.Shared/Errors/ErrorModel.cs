namespace TicketApi.Shared.Errors;

public class ErrorModel
{
    /// <summary>
    /// Объект с информацией об ошибках
    /// </summary>
    public Error Error { get; set; }

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="error">Объект с информацией об ошибках</param>
    public ErrorModel(Error error)
    {
        Error = error;
    }

    /// <summary>
    /// Для десериализации
    /// </summary>
    public ErrorModel()
    {
    }
}