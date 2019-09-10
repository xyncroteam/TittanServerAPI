
using System;
using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

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
        public List<Note> Notes { get; set; }
    }

    public class DepositFromTerminal
    {
        public int DepositId { get; set; }
        public string UserId { get; set; }
        public string TerminalId { get; set; }
        public string DepositNumber { get; set; }
        public string Amount { get; set; }
        public List<Note> Notes { get; set; }
        public string Date { get; set; }
      
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

    public class DepositRequest
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]//
        public DateTime? EndDate { get; set; }
        public int? TerminalId { get; set; }
    }

    public class DepositNotesRequest
    {
        public int? DepositId { get; set; }
    }

   
    public class Note
    {
        public string NoteId { get; set; }
        public string Count { get; set; }
    }


}
