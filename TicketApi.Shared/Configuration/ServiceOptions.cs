using Microsoft.AspNetCore.Mvc;

namespace TicketApi.Shared.Configuration;

public sealed class ServiceOptions
{
    /// <summary>
    /// Имя сервиса
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание сервиса для сваггера
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Версия API по умолчанию
    /// </summary>
    public string DefaultVersion { get; set; }

    /// <summary>
    /// Парсит указанную в конфигурации версию сервиса и возвращает её.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Если указанная версия невалидна</exception>
    public ApiVersion GetDefaultApiVersion()
    {
        if (string.IsNullOrEmpty(DefaultVersion))
            return new ApiVersion(1, 0);
        var strArray = DefaultVersion.Split('.');
        int result1;
        int result2;
        if (strArray.Length != 2 || !int.TryParse(strArray[0], out result1) || !int.TryParse(strArray[1], out result2))
            throw new InvalidOperationException($"Не удалось распарсить версию сервиса: `{DefaultVersion}`");
        return new ApiVersion(result1, result2);
    }
    
    public ServiceOptions()
    {
        DefaultVersion = "1.0";
        Description = "";
    }
}