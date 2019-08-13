
namespace wscore.Entities
{
    public class Terminal
    {
        public int TerminalId { get; set; }
        public string Name { get; set; }
        public string TerminalDoor { get; set; }
        public string CashBoxDoor { get; set; }
        public int Notes { get; set; }
        public int TotalAmount { get; set; }
        public int TimeOff { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string IP { get; set; }
        public TerminalStatus Status { get; set; }
    }

    public enum TerminalStatus
    {
        Online,
        Offline,
        Error
    }
    public class Notes
    {
        public int Note100 { get; set; }
        public int Note50 { get; set; }
        public int Note20 { get; set; }
        public int Note10 { get; set; }
        public int Note5 { get; set; }
        public int Note1 { get; set; }

    }
}