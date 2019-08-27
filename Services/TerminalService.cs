using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using wscore.Entities;
using wscore.Helpers;

namespace wscore.Services
{
    public interface ITerminalService
    {
        void DepositTerminal(string deposit);
        void EventTerminal(int terminalId, int eventId);
        bool DepositFromTerminal(DepositFromTerminal deposit);
    }

    public class TerminalService : ITerminalService
    {

        

        public TerminalService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        #region Private

        private readonly AppSettings _appSettings;

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_appSettings.DefaultConnection);
        }

        #region Terminal

        private Terminal GetTerminal(int terminalId)
        {
            Terminal _terminal = null; ;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Terminal where TerminalId=" + terminalId.ToString(), conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _terminal = new Terminal();
                        _terminal.Address = reader["Address"].ToString();
                        _terminal.IP = reader["IP"].ToString();
                        _terminal.Description = reader["Description"].ToString();
                        _terminal.TimeOff = int.Parse(reader["timeOff"].ToString());
                        _terminal.TerminalDoor = reader["TerminalDoor"].ToString();
                        _terminal.CashBoxDoor = reader["CashBoxDoor"].ToString();
                        _terminal.Notes = int.Parse(reader["Notes"].ToString());

                    }
                }
            }

            return _terminal;
        }

        private bool IsOnline(string ip)
        {
            bool ok = true;

            Ping myPing = new Ping();
            PingReply reply = myPing.Send(ip, 1000);
            if (reply.Status == IPStatus.TimedOut) {
                ok = false;
            }

            return ok;
        }

        private void UpdateTerminalNotes(Deposit deposit, int notes)
        {
            if (notes == 0)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE Terminal SET Notes = " + notes.ToString() + " WHERE TerminalId = " + deposit.TerminalId.ToString() + ";", conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                Notes notesList = new Notes();
                notesList.Note1 = 0;
                notesList.Note10 = 0;
                notesList.Note100 = 0;
                notesList.Note20 = 0;
                notesList.Note5 = 0;
                notesList.Note50 = 0;

                foreach (string bill in deposit.BillsDetail)
                {
                    if (bill == "1")
                        notesList.Note1++;
                    if (bill == "10")
                        notesList.Note10++;
                    if (bill == "100")
                        notesList.Note100++;
                    if (bill == "20")
                        notesList.Note20++;
                    if (bill == "5")
                        notesList.Note5++;
                    if (bill == "50")
                        notesList.Note50++;
                }

                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(@"UPDATE Terminal SET Notes = Notes + " + notes.ToString() + ", TotalAmount = TotalAmount + " + deposit.Amount + " WHERE TerminalId = " + deposit.TerminalId.ToString() + ";" +
                        " UPDATE TerminalNotes SET `100` = `100` + " + notesList.Note100.ToString() + ", `50` = `50` + " + notesList.Note50.ToString() + ", `20` = `20` + " + notesList.Note20.ToString() + ", `10` = `10` + " + notesList.Note10.ToString() + ", `5` = `5` + " + notesList.Note5.ToString() + ", `1` = `1` + " + notesList.Note1.ToString() + " WHERE TerminalId = " + deposit.TerminalId.ToString() + ";", conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateTerminalDoor(int terminalId, string door)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Terminal SET TerminalDoor = '" + door + "' WHERE TerminalId = " + terminalId.ToString() + ";", conn);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateCashBoxDoor(int terminalId, string door)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Terminal SET CashBoxDoor = '" + door + "' WHERE TerminalId = " + terminalId.ToString() + ";", conn);
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Event

        private Event EventInsert(Event Event)
        {
            var _event = Event;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into Event (TerminalId,EventTypeId,UserId,Date) values (" + _event.TerminalId.ToString() + "," + _event.EventTypeId.ToString() + "," + _event.UserId.ToString() + ", NOW());SELECT LAST_INSERT_ID();", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _event.EventId = Convert.ToInt32(reader[0]);
                    }
                }
            }

            return _event;
        }

        private Event EventUpdate(Event Event)
        {
            var _event = Event;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Event SET Status = '" + _event.Status.ToString() + "' WHERE EventId = " + _event.EventId.ToString() + ";", conn);
                cmd.ExecuteNonQuery();
            }

            return _event;
        }

        #region Event From Terminal   

        private void EventTerminalInsert(Event Event)
        {
            var _event = Event;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into EventTerminal (TerminalId,EventTypeId,Date) values (" + _event.TerminalId.ToString() + "," + _event.EventTypeId.ToString() + ", NOW());", conn);
                cmd.ExecuteNonQuery();
            }

        }

        #endregion

        #endregion

        #region Deposit

        private Deposit DepositUpdate(Deposit deposit)
        {
            var _deposit = deposit;

            using (MySqlConnection conn = GetConnection())
            {
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Deposit SET Amount = '" + (int)_deposit.Amount + "', Status = '" + (int)_deposit.Status + "', DateEnd = NOW() WHERE DepositId = " + _deposit.DepositId.ToString() + ";", conn);
                cmd.ExecuteNonQuery();
            }

            return _deposit;
        }

        private void DepositBillInsert(Deposit deposit)
        {
            var _deposit = deposit;

            string bills = null;
            int i = 0;
            foreach (string bill in _deposit.BillsDetail)
            {
                if (i == 0)
                {
                    bills = "(" + _deposit.DepositId.ToString() + "," + bill + ")";
                    i = 1;
                }
                else
                {
                    bills += ",(" + _deposit.DepositId.ToString() + "," + bill + ")";
                }
            }
            bills += ";";
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into DepositBill (DepositId,Bill) values " + bills, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _deposit.DepositId = Convert.ToInt32(reader[0]);
                    }
                }
            }
        }

        private DepositFromTerminal DepositInsert(DepositFromTerminal deposit)
        {
            var _deposit = deposit;

            _deposit.Amount = Decimal.ToInt32(Convert.ToDecimal(_deposit.Amount)).ToString();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into Deposit (DepositNumber,Amount, DateEnd,TerminalId,UserId) values ('" + _deposit.DepositNumber.ToString() + "'," + _deposit.Amount + ", NOW()," + _deposit.TerminalId + "," + _deposit.UserId + ");SELECT LAST_INSERT_ID();", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _deposit.DepositId = Convert.ToInt32(reader[0]);
                    }
                }
            }

            return _deposit;
        }

        private void DepositNotesInsert(DepositFromTerminal deposit)
        {

            string strCol = "";
            string strVal = "";
            foreach (var n in deposit.Notes)
            {
                strCol += ",Notes" + n.NoteId;
                strVal += "," + n.Count;
            }
            
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into DepositNotes (DepositId " + strCol + ") values (" + deposit.DepositId + " " + strVal + ") ;", conn);
                cmd.ExecuteNonQuery();
            }
            
            //return _deposit;
        }

        #endregion

        #endregion

        public bool DepositFromTerminal(DepositFromTerminal deposit)
        {
            bool ok = true;

            try
            {
                deposit = DepositInsert(deposit);
                DepositNotesInsert(deposit);
            }
            catch
            {
                ok = false;
            }

            return ok;
        }


        public void DepositTerminal(string deposit)
        {
            string[] _depo = deposit.Split(';');
            string[] _bills = _depo[3].Split('.');
            string bills = "";
            int i = 0;
            int total = 0;
            foreach (string bill in _bills)
            {
                if (i == 0)
                {
                    bills = "\"" + bill + "\"";
                    i = 1;
                }
                else
                {
                    bills += ",\"" + bill + "\"";
                    i++;
                }
                total += Convert.ToInt16(bill);
            }
            string jsonDepo = "{  \"TerminalId\":\"" + _depo[0] + "\",  \"DepositId\" : \"" + _depo[1] + "\",  \"Status\" : \"" + _depo[2] +"\",  \"BillsDetail\" : [" + bills + "] }";
            var _deposit = Newtonsoft.Json.JsonConvert.DeserializeObject<Deposit>(jsonDepo);
            _deposit.Amount = total;
            _deposit = DepositUpdate(_deposit);
            if (i > 0)
            {
                DepositBillInsert(_deposit);
                UpdateTerminalNotes(_deposit, i);
            }
        }

        public void EventTerminal(int terminalId, int eventId)
        {
            var _event = new Event();

            var _depo = new Deposit();
            _depo.TerminalId = terminalId;

            _event.TerminalId = terminalId;
            _event.EventTypeId = eventId;
            
            EventTerminalInsert(_event);

            if (_event.EventTypeId == 3)
                UpdateTerminalDoor(_event.TerminalId, "Open");

            if (_event.EventTypeId == 4)
                UpdateCashBoxDoor(_event.TerminalId, "Open");

            if (_event.EventTypeId == 9)
                UpdateTerminalDoor(_event.TerminalId, "Close");

            if (_event.EventTypeId == 10)
                UpdateCashBoxDoor(_event.TerminalId, "Close");

            if (_event.EventTypeId == 7)
                UpdateCashBoxDoor(_event.TerminalId, "Close");

            if (_event.EventTypeId == 5)
                UpdateTerminalNotes(_depo, 0);

            if (_event.EventTypeId == 8)
                UpdateTerminalNotes(_depo, 0);
        }
                
    }

}
