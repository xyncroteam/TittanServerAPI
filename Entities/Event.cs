using System;

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

    public class EventTCP
    {
        public int TerminalId { get; set; }
        public string Event { get; set; }
        public int UserId { get; set; }
        public int Code { get; set; }
    }

    public enum EventStatus
    {
        Successful,
        Error,
        OffLine,
        Busy
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
        TimeOffUpdate,
        Reset
    }
}
