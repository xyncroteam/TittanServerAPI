
namespace wscore.Entities
{
    public class Deposit
    {
        public int DepositId { get; set; }
        public int EventId { get; set; }
        public int TerminalId { get; set; }
        public string DepositNumber { get; set; }
        public int Amount { get; set; }
        public string[] BillsDetail { get; set; }
        public string Date { get; set; }
        public DepositStatus Status { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }

    public enum DepositStatus
    {
        Sending,
        Processing,
        Completed,
        Canceled,
        OffLine,
        Busy,
        Error
    }


}
