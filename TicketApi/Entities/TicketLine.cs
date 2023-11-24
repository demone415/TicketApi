using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketApi.Entities;

/// <summary>
/// Позиция чека
/// </summary>
[Table("ticketlines")]
public class TicketLine
{
    /// <summary>
    /// Id позиции в моей базе
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// Кол-во
    /// </summary>
    [Column("quantity")]
    public decimal Quantity { get; set; }

    /// <summary>
    /// Цена
    /// </summary>
    [Column("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// Стоимость
    /// </summary>
    [Column("cost")]
    public decimal Cost { get; set; }

    /// <summary>
    /// Категория 1
    /// </summary>
    [Column("category1")]
    public string Category1 { get; set; }

    /// <summary>
    /// Категория 2
    /// </summary>
    [Column("category2")]
    public string Category2 { get; set; }

    /// <summary>
    /// Категория 3
    /// </summary>
    [Column("category3")]
    public string Category3 { get; set; }

    /// <summary>
    /// Теги через запятую
    /// </summary>
    [Column("tags")]
    public string Tags { get; set; }

    /// <summary>
    /// Важная позиция
    /// </summary>
    [Column("isessential")]
    public bool IsEssential { get; set; }

    /// <summary>
    /// Способ оплаты
    /// </summary>
    [Column("paymenttype")]
    public decimal? PaymentType { get; set; }

    /// <summary>
    /// Тип продукта
    /// </summary>
    [Column("producttype")]
    public decimal? ProductType { get; set; }

    /// <summary>
    /// Тип продукта строкой
    /// </summary>
    [Column("rawproductcode")]
    public string RawProductCode { get; set; }

    /// <summary>
    /// Id типа продукта строкой
    /// </summary>
    [Column("productidtype")]
    public string ProductIdType { get; set; }
}