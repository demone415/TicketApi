using System.Globalization;
using Newtonsoft.Json;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Models;
using TicketApi.Repositories;

// ReSharper disable All

namespace TicketApi.Service.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IClassifierService _classifierService;
    private readonly IProverkaCheckaService _proverkaCheckaService;
    private readonly CultureInfo _enUs = new("en-US");

    public TicketService(ITicketRepository ticketRepository, IProverkaCheckaService proverkaCheckaService, IClassifierService classifierService)
    {
        _ticketRepository = ticketRepository;
        _proverkaCheckaService = proverkaCheckaService;
        _classifierService = classifierService;
    }

    public async Task<TicketDataResult> GetTicketData(string qrString)
    {
        var qrData = GetQrData(qrString);
        if (qrData == null)
        {
            return new TicketDataResult() { ResultCode = ResultCodes.CheckInvalid };
        }

        var checkResult = await _proverkaCheckaService.GetTicketDataFromQrString(qrString);
        var result = CreateTicketDataResult(qrData, checkResult);
        return result;
    }

    public async Task<TicketDataResult> GetTicketData(TicketHeader header)
    {
        var qrData = GetQrData(header);
        var checkResult = await _proverkaCheckaService.GetTicketDataFromQrData(qrData);
        var result = CreateTicketDataResult(header, checkResult);
        return result;
    }

    public async Task<TicketHeader> ClassifyTicket(TicketHeader header)
    {
        return await _classifierService.ClassifyTicket(header);
    }

    public async Task<AutoTicketResult> ProcessQrAuto(string qrstring)
    {
        var result = new AutoTicketResult();
        var ticketResult = await GetTicketData(qrstring);
        result.ResultCode = ticketResult.ResultCode;
        if (
                ticketResult.ResultCode == ResultCodes.NoInformation 
             || ticketResult.ResultCode == ResultCodes.DailyRequestNumberExceeded
            )
        {
            result.SaveSuccessful = await _ticketRepository.SaveTicket(ticketResult.Header);
        }
        if (ticketResult.ResultCode == ResultCodes.Success)
        {
            var categorizedTicket = await ClassifyTicket(ticketResult.Header);
            result.SaveSuccessful = await _ticketRepository.SaveTicket(ticketResult.Header);
        }

        return result;
    }

    private static TicketDataResult CreateTicketDataResult(TicketHeader header, CheckResult checkResult)
    {
        var result = new TicketDataResult();
        result.Header = header;
        switch (checkResult.Code)
        {
            case ResultCodes.NoInformation:
            case ResultCodes.DailyRequestNumberExceeded:
                result.Header.FetchTries += 1;
                result.Header.NextFetchDateTime = GetNextFetchTime(result.Header.FetchTries);
                break;
            case ResultCodes.Success:
                var check = JsonConvert.DeserializeObject<Check>(checkResult.Data.ToString());
                result.Header = FillHeaderFromCheck(header, check);
                break;
        }

        return result;
    }

    private static DateTimeOffset GetNextFetchTime(short fetchTries)
    {
        /*
            1-ый запрос - сейчас, условно время T.
            2-ой запрос - T + 6 часов
            3-ий запрос - T + 1 день
            4-ый запрос - T + 3 дня
            5-ый запрос - T + 10 дней.
         */
        switch (fetchTries)
        {
            case 1: return DateTimeOffset.UtcNow.AddHours(6);
            case 2: return DateTimeOffset.UtcNow.AddHours(18);
            case 3: return DateTimeOffset.UtcNow.AddDays(2);
            case 4: return DateTimeOffset.UtcNow.AddDays(7);
            default: return DateTimeOffset.MaxValue.ToUniversalTime();
        }
    }


    private static TicketDataResult CreateTicketDataResult(QrData? qrData, CheckResult checkResult)
    {
        var result = new TicketDataResult();
        result.ResultCode = checkResult.Code;
        switch (checkResult.Code)
        {
            case ResultCodes.NoInformation:
            case ResultCodes.DailyRequestNumberExceeded:
                result.Header = CreateHeaderFromQrData(qrData);
                break;
            case ResultCodes.Success:
                var check = JsonConvert.DeserializeObject<Check>(checkResult.Data.ToString());
                result.Header = CreateHeaderFromCheck(check);
                break;
        }

        return result;
    }
    
    private static TicketHeader CreateHeaderFromCheck(Check check)
    {
        var lines = new List<TicketLine>(check.Json.Items.Count);
        var data = check.Json;
        var header = new TicketHeader()
        {
            Tsmp = DateTimeOffset.UtcNow,
            TicketId = data.Metadata.Id.ToString(),
            TicketSum = data.TotalSum / 100,
            ShopInn = data.UserInn.Trim(),
            ShopName = data.RetailPlace,
            ShopAddress = data.RetailPlaceAddress,
            FicsalDoc = data.FiscalDocumentNumber.ToString(),
            FicsalSign = data.FiscalSign.ToString(),
            FsId = data.FiscalDriveNumber,
            Date = data.DateTime.ToUniversalTime(),
            Operator = data.Operator?.Trim(),
            BuyerPhoneOrAddress = data.BuyerPhoneOrAddress,
            Manual = false,
            Status = HeaderStatuses.Success,
            FetchTries = 1,
            NextFetchDateTime = DateTimeOffset.MinValue.ToUniversalTime(),
        };
        lines.AddRange(data.Items.Select(item => new TicketLine
        {
            Name = item.Name,
            Quantity = item.Quantity,
            Price = item.Price,
            Cost = item.Price * item.Quantity,
            Category1 = null,
            Category2 = null,
            Category3 = null,
            Tags = null,
            IsEssential = false,
            PaymentType = item.PaymentType,
            ProductType = item.ProductType,
            RawProductCode = item.ProductCodeNew?.Gs1M.RawProductCode,
            ProductIdType = item.ProductCodeNew?.Gs1M.ProductIdType.ToString(),
        }));
        header.Lines = lines;
        return header;
    }
    
    private static TicketHeader FillHeaderFromCheck(TicketHeader header, Check check)
    {
        var lines = new List<TicketLine>(check.Json.Items.Count);
        var data = check.Json;
        header.Tsmp = DateTimeOffset.UtcNow;
        header.TicketId = data.Metadata.Id.ToString();
        header.TicketSum = data.TotalSum / 100;
        header.ShopInn = data.UserInn.Trim();
        header.ShopName = data.RetailPlace;
        header.ShopAddress = data.RetailPlaceAddress;
        header.Operator = data.Operator?.Trim();
        header.BuyerPhoneOrAddress = data.BuyerPhoneOrAddress;
        header.Status = HeaderStatuses.Success;
        header.FetchTries += 1;
        lines.AddRange(data.Items.Select(item => new TicketLine
        {
            Name = item.Name,
            Quantity = item.Quantity,
            Price = item.Price,
            Cost = item.Price * item.Quantity,
            Category1 = null,
            Category2 = null,
            Category3 = null,
            Tags = null,
            IsEssential = false,
            PaymentType = item.PaymentType,
            ProductType = item.ProductType,
            RawProductCode = item.ProductCodeNew?.Gs1M.RawProductCode,
            ProductIdType = item.ProductCodeNew?.Gs1M.ProductIdType.ToString(),
        }));
        header.Lines = lines;
        
        return header;
    }
    
    private static TicketHeader CreateHeaderFromQrData(QrData? qrData)
    {
        return new TicketHeader
        {
            OperationType = qrData.Value.OperationType,
            TicketSum = qrData.Value.Sum,
            FicsalSign = qrData.Value.FiscalSign,
            FicsalDoc = qrData.Value.FiscalDocument,
            FsId = qrData.Value.FiscalNumber,
            Date = qrData.Value.Date.ToUniversalTime(),
            Tsmp = DateTimeOffset.UtcNow,
            NextFetchDateTime = DateTimeOffset.UtcNow.AddHours(6),
            Status = HeaderStatuses.InQueue,
            Manual = false,
            FetchTries = 1,
        };
    }

    private static QrData GetQrData(TicketHeader header)
    {
        var model = new QrData
        {
            Date = header.Date.Date,
            Sum = header.TicketSum,
            FiscalNumber = header.FsId,
            FiscalDocument = header.FicsalDoc,
            FiscalSign = header.FicsalSign,
            OperationType = header.OperationType,
        };
        
        return model;
    }
    
    private QrData? GetQrData(string qrRaw)
    {
        // t=20230226T1329&s=1162.26&fn=9960440301861117&i=54314&fp=2498462644&n=1
        var arr = qrRaw.Split("&").ToList();
        if (arr.Count != 6)
        {
            return null;
        }

        if (!arr.All(_ => _.Contains('=')))
        {
            return null;
        }

        for (var index = 0; index < arr.Count; index++)
        {
            arr[index] = arr[index][(arr[index].IndexOf("=", StringComparison.InvariantCultureIgnoreCase) + 1)..];
        }

        var isDateTimeParsed = DateTime.TryParseExact(arr[0], "yyyyMMddTHHmm", _enUs, DateTimeStyles.None, out var dt);
        if (!isDateTimeParsed)
        {
            return null;
        }

        var isSumParsed = decimal.TryParse(arr[1], _enUs, out var sum);
        if (!isSumParsed)
        {
            return null;
        }
        
        var isOpTypeParsed = short.TryParse(arr[5], _enUs, out var opType);
        if (!isOpTypeParsed)
        {
            return null;
        }
        
        // t=20230226T1329&s=1162.26&fn=9960440301861117&i=54314&fp=2498462644&n=1
        var model = new QrData
        {
            Date = dt,
            Sum = sum,
            FiscalNumber = arr[2],
            FiscalDocument = arr[3],
            FiscalSign = arr[4],
            OperationType = (OperationType)opType,
        };


        return model;
    }
}