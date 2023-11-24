using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketApi.Models;

namespace TicketApi.Entities;

/// <summary>
/// Заголовок чека
/// </summary>
[Table("ticketheaders")]
public class TicketHeader
{
    /// <summary>
    /// Id чека в моей бд
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }


    /// <summary>
    /// ФПД
    /// </summary>
    [Column("fssign")]
    public string FicsalSign { get; set; }


    /// <summary>
    /// ФД
    /// </summary>
    [Column("fsdoc")]
    public string FicsalDoc { get; set; }


    /// <summary>
    /// ФН
    /// </summary>
    [Column("fsid")]
    public string FsId { get; set; }


    /// <summary>
    /// Дата отправки чека в систему
    /// </summary>
    [Column("tsmp")]
    public DateTimeOffset Tsmp { get; set; }


    /// <summary>
    /// Статус чека в системе
    /// </summary>
    [Column("status")]
    public string Status { get; set; }


    /// <summary>
    /// Кол-во попыток получить данные чека
    /// </summary>
    [Column("fetchtries")]
    public short FetchTries { get; set; }


    /// <summary>
    /// Дата след попытки получить данные чека
    /// </summary>
    [Column("nextfetch")]
    public DateTimeOffset NextFetchDateTime { get; set; }


    /// <summary>
    /// Id чека в сервисе proverkacheka.com
    /// </summary>
    [Column("ticketid")]
    public string TicketId { get; set; }


    /// <summary>
    /// Сумма по чеку в рублях
    /// </summary>
    [Column("ticketsum")]
    public decimal TicketSum { get; set; }


    /// <summary>
    /// ИНН магазина
    /// </summary>
    [Column("shopinn")]
    public string ShopInn { get; set; }


    /// <summary>
    /// Название магазина
    /// </summary>
    [Column("shopname")]
    public string ShopName { get; set; }


    /// <summary>
    /// Адрес магазина
    /// </summary>
    [Column("shopaddress")]
    public string ShopAddress { get; set; }


    /// <summary>
    /// Дата операции
    /// </summary>
    [Column("date")]
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Кассир
    /// </summary>
    [Column("operator")]
    public string Operator { get; set; }

    /// <summary>
    /// Телефон или почта запросившего чек
    /// </summary>
    [Column("buyer")]
    public string BuyerPhoneOrAddress { get; set; }

    /// <summary>
    /// Чек добавлен в базу вручную
    /// </summary>
    [Column("manual")]
    public bool Manual { get; set; }

    /// <summary>
    /// Тип операции
    /// </summary>
    [Column("operationtype")]
    public OperationType OperationType { get; set; }

    /// <summary>
    /// Позиции чека
    /// </summary>
    [Column("lineid")]
    public List<TicketLine> Lines { get; set; }
}