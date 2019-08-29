
using System;
using System.ComponentModel.DataAnnotations;

namespace wscore.Entities
{
    public class ActionReturn
    {
        public string UserName { get; set; }
        public int TerminalId { get; set; }
        public string EventType { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }

    public class StatusReturn
    {
        public int TerminalId { get; set; }
        public string Status { get; set; }
        public string Door { get; set; }
        public string CashBoxDoor { get; set; }
        public string Reader { get; set; }

    }

    public class DepositReturn
    {
        public string UserName { get; set; }
        public int TerminalId { get; set; }
        public int DepositId { get; set; }
        public string Number { get; set; }
        public int Amount { get; set; }
        public string[] BillsDetail { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }

    }

    public class TerminalReturn
    {
        public int TerminalId { get; set; }
        public string Status { get; set; }
        public string TerminalDoor { get; set; }
        public string CashBoxDoor { get; set; }
        public int Bills { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public int TimeOff { get; set; }
        public int TotalAmount { get; set; }
        public Notes Notes { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
    }
    public class TerminalIdsReturn
    {
        public int TerminalId { get; set; }
        public string Name { get; set; }
    }

    public class UpdateTerminalReturn
    {
        // [Required]
        public int TerminalId { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        // [Required]
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
    }

    public class TotalAmount
    {
        public double AllTotalAmount { get; set; }
        public int totalTerminals { get; set; }
        public double TotalDeposit { get; set; }
    }

    public class TerminalsList
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime LastComunication { get; set; }
        public double percentageTerminal { get; set; }
    }

    public class TerminalsCapacity
    {
        public int TotalSystemCapacityNotes { get; set; }
        public int currentNotes { get; set; }
        public double percentageNotes { get; set; }
    }

    public class DepositListReturn
    {
        public string TerminalName { get; set; }
        public string TerminalAddress { get; set; }
        public int DepositId { get; set; }
        public int DepositNumber { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public Notes DepositNotes { get; set; }
        public string UserNameDeposit { get; set; }
    }


}
