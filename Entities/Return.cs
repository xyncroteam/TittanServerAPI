
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
    }

    public class UpdateTerminalReturn
    {
       // [Required]
        public int TerminalId { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
       // [Required]
        public string Name { get; set; }
    }

    public class TotalAmount
    {
        public double AllTotalAmount { get; set; }
    }


}
