using Newtonsoft.Json;

namespace TicketApi.Models;

public sealed class Check
{
    [JsonProperty("json")]
    public Json Json { get; set; }

    [JsonProperty("html")]
    public string Html { get; set; }
}

public sealed class Json
{
    [JsonProperty("buyerPhoneOrAddress")]
    public string BuyerPhoneOrAddress { get; set; }
    
    [JsonProperty("messageFiscalSign")]
    public double MessageFiscalSign { get; set; }

    [JsonProperty("code")]
    public long Code { get; set; }

    [JsonProperty("fiscalDriveNumber")]
    public string FiscalDriveNumber { get; set; }

    [JsonProperty("kktRegId")]
    public string KktRegId { get; set; }

    [JsonProperty("userInn")]
    public string UserInn { get; set; }

    [JsonProperty("fiscalDocumentNumber")]
    public long FiscalDocumentNumber { get; set; }

    [JsonProperty("dateTime")]
    public DateTimeOffset DateTime { get; set; }

    [JsonProperty("fiscalSign")]
    public long FiscalSign { get; set; }

    [JsonProperty("shiftNumber")]
    public long ShiftNumber { get; set; }

    [JsonProperty("requestNumber")]
    public long RequestNumber { get; set; }

    [JsonProperty("operationType")]
    public long OperationType { get; set; }

    [JsonProperty("totalSum")]
    public long TotalSum { get; set; }

    [JsonProperty("operator")]
    public string Operator { get; set; }

    [JsonProperty("items")]
    public List<Item> Items { get; set; }

    [JsonProperty("nds18")]
    public long Nds18 { get; set; }

    [JsonProperty("nds10")]
    public long Nds10 { get; set; }

    [JsonProperty("user")]
    public string User { get; set; }

    [JsonProperty("retailPlaceAddress")]
    public string RetailPlaceAddress { get; set; }

    [JsonProperty("retailPlace")]
    public string RetailPlace { get; set; }

    [JsonProperty("appliedTaxationType")]
    public long AppliedTaxationType { get; set; }

    [JsonProperty("cashTotalSum")]
    public long CashTotalSum { get; set; }

    [JsonProperty("ecashTotalSum")]
    public long EcashTotalSum { get; set; }

    [JsonProperty("prepaidSum")]
    public long PrepaidSum { get; set; }

    [JsonProperty("creditSum")]
    public long CreditSum { get; set; }

    [JsonProperty("provisionSum")]
    public long ProvisionSum { get; set; }

    [JsonProperty("fiscalDocumentFormatVer")]
    public long FiscalDocumentFormatVer { get; set; }

    [JsonProperty("fnsUrl")]
    public string FnsUrl { get; set; }

    [JsonProperty("sellerAddress")]
    public string SellerAddress { get; set; }

    [JsonProperty("metadata")]
    public Metadata Metadata { get; set; }
}

public sealed class Item
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("price")]
    public long Price { get; set; }

    [JsonProperty("quantity")]
    public long Quantity { get; set; }

    [JsonProperty("itemsQuantityMeasure")]
    public long ItemsQuantityMeasure { get; set; }

    [JsonProperty("sum")]
    public long Sum { get; set; }

    [JsonProperty("nds")]
    public long Nds { get; set; }

    [JsonProperty("ndsSum")]
    public long NdsSum { get; set; }

    [JsonProperty("paymentType")]
    public long PaymentType { get; set; }

    [JsonProperty("productType")]
    public long ProductType { get; set; }

    [JsonProperty("productCodeNew")]
    public ProductCodeNew ProductCodeNew { get; set; }

    [JsonProperty("labelCodeProcesMode")]
    public long LabelCodeProcesMode { get; set; }

    [JsonProperty("checkingProdInformationResult")]
    public long CheckingProdInformationResult { get; set; }
}

public sealed class ProductCodeNew
{
    [JsonProperty("gs1m")]
    public Gs1M Gs1M { get; set; }
}

public sealed class Gs1M
{
    [JsonProperty("rawProductCode")]
    public string RawProductCode { get; set; }

    [JsonProperty("productIdType")]
    public long ProductIdType { get; set; }

    [JsonProperty("gtin")]
    public string Gtin { get; set; }

    [JsonProperty("sernum")]
    public string Sernum { get; set; }
}

public sealed class Metadata
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("ofdId")]
    public string OfdId { get; set; }

    [JsonProperty("receiveDate")]
    public DateTimeOffset ReceiveDate { get; set; }

    [JsonProperty("subtype")]
    public string Subtype { get; set; }

    [JsonProperty("address")]
    public string Address { get; set; }
}
