namespace TicketApi.Models;

public enum ResultCodes : short
{
    CheckInvalid = 0,
    Success = 1,
    NoInformation = 2,
    RequestNumberExceeded = 3,
    DailyRequestNumberExceeded = 253,
    SaveUnsuccessful = 254,
    SomethingWentWrong = 255,
}