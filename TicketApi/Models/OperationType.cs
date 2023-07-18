namespace TicketApi.Models;

public enum OperationType
{
    //1 - Приход 2 - Возврат прихода 3 - Расход 4 - Возврат расход
    /// <summary>
    /// Приход
    /// </summary>
    Income = 1,
    
    /// <summary>
    /// Возврат прихода
    /// </summary>
    RetIncome = 2,
    
    /// <summary>
    /// Расход
    /// </summary>
    Expense = 3,
    
    /// <summary>
    /// Возврат расхода
    /// </summary>
    RetExpense = 4,
}