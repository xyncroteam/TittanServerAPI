namespace wscore.Entities
{
    public class Event
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int TerminalId { get; set; }
        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }
        public EventStatus Status { get; set; }
        public string Date { get; set; }
    }

    public enum EventStatus
    {
        Successful,
        Error,
        OffLine
    }

    public enum EventType
    {
        Deposit,
        DepositCancel,
        OpenDoor,
        OpenCashBox,
        CashBoxRemoved,
        Shock,
        CloseCashBox,
        CashBoxInserted,
        Reboot,
        TimeOffUpdate
    }
}
