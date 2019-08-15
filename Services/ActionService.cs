using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using newapi.Helpers;
using Newtonsoft.Json.Linq;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using wscore.Entities;
using wscore.Helpers;

namespace wscore.Services
{
    public interface IActionService
    {
        DepositReturn Deposit(int terminalId, string number, int total, int userId);
        ActionReturn OpenDoor(int terminalId, int userId);
        ActionReturn OpenDoorTCP(int terminalId, int userId);
        ActionReturn Reboot(int terminalId, int userId);
        TerminalReturn Status(int terminalId, int userId);
        List<TerminalReturn> Terminals(int userId);
        DepositReturn GetDeposit(int DepositId, int userId);
        TerminalReturn UpdateDepositTimeOff(int terminalId, int timeOff, int userId);
        ActionReturn DepositCancel(int DepositId, int TerminalId, int userId);
        void UpdateTerminal(UpdateTerminalReturn updateTerminal);
        TotalAmount getAllTerminalsTotalAmount(int userId);
    }

    public class ActionService : IActionService
    {
        
        public ActionService(IOptions<AppSettings> appSettings)
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

        private List<Terminal> ListTerminal()
        {
            List<Terminal> _listTerminal = new List<Terminal>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Terminal ", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Terminal _terminal = new Terminal();
                        _terminal.Address = reader["Address"].ToString();
                        _terminal.IP = reader["IP"].ToString();
                        _terminal.Description = reader["Description"].ToString();
                        _terminal.TimeOff = int.Parse(reader["timeOff"].ToString());
                        _terminal.TerminalDoor = reader["TerminalDoor"].ToString();
                        _terminal.Name = reader["Name"].ToString();
                        _terminal.CashBoxDoor = reader["CashBoxDoor"].ToString();
                        _terminal.Notes = int.Parse(reader["Notes"].ToString());
                        _terminal.TerminalId = int.Parse(reader["TerminalId"].ToString());
                        _terminal.TotalAmount = int.Parse(reader["TotalAmount"].ToString());
                        _listTerminal.Add(_terminal);
                    }
                }
            }

            return _listTerminal;
        }

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
                        _terminal.Name = reader["Name"].ToString();
                        _terminal.CashBoxDoor = reader["CashBoxDoor"].ToString();
                        _terminal.Notes = int.Parse(reader["Notes"].ToString());
                        _terminal.TerminalId = int.Parse(reader["TerminalId"].ToString());
                        _terminal.TotalAmount = int.Parse(reader["TotalAmount"].ToString());
                    }
                }
            }

            return _terminal;
        }

        private Terminal GetTerminalByName(string name)
        {
            Terminal _terminal = null; ;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Terminal where Name= '" + name.ToString() + "' " , conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _terminal = new Terminal();
                        _terminal.Name = reader["Name"].ToString();
                        _terminal.TerminalId = int.Parse(reader["TerminalId"].ToString());
                    }
                }
            }
            return _terminal;
        }

        public bool isNameUnique(string name)
        {
            var isUnique = GetTerminalByName(name);

            if (isUnique == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Notes GetTerminalNotes(int terminalId)
        {
            Notes _notes = null; ;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from TerminalNotes where TerminalId=" + terminalId.ToString(), conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _notes = new Notes();
                        _notes.Note1= int.Parse(reader["1"].ToString());
                        _notes.Note5 = int.Parse(reader["5"].ToString());
                        _notes.Note10 = int.Parse(reader["10"].ToString());
                        _notes.Note20 = int.Parse(reader["20"].ToString());
                        _notes.Note50 = int.Parse(reader["50"].ToString());
                        _notes.Note100 = int.Parse(reader["100"].ToString());
                    }
                }
            }

            return _notes;
        }

        private Terminal UpdateTerminalTimeOff(Terminal terminal)
        {
            var _terminal = terminal;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Terminal SET timeoff = " + _terminal.TimeOff.ToString() + " WHERE TerminalId = " + _terminal.TerminalId.ToString() + ";", conn);
                cmd.ExecuteNonQuery();
            }

            return _terminal;
        }

        private bool IsOnline(string ip)
        {
            bool ok = true;

            Ping myPing = new Ping();
            PingReply reply = myPing.Send(ip, 1000);
            if (reply.Status == IPStatus.TimedOut)
            {
                ok = false;
            }

            return ok;
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
                MySqlCommand cmd = new MySqlCommand("insert into EventTerminal (TerminalId,EventTypeId,Date) values (" + _event.TerminalId.ToString() + "," + _event.EventTypeId.ToString() + ",'" + _event.Date + "');SELECT LAST_INSERT_ID();", conn);
                cmd.ExecuteNonQuery();
            }

        }

        #endregion

        #endregion

        #region Deposit

        private Deposit DepositInsert(Deposit deposit)
        {
            var _deposit = deposit;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into Deposit (EventId,DepositNumber,Status,Amount) values (" + _deposit.EventId.ToString() + ",'" + _deposit.DepositNumber.ToString() + "','" + (int)_deposit.Status + "'," + _deposit.Amount + ");SELECT LAST_INSERT_ID();", conn);
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

        private Deposit DepositCancel(Deposit deposit)
        {
            var _deposit = deposit;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Deposit SET Status = '" + (int)_deposit.Status + "', UserCancel = 1, DateEnd = NOW() WHERE DepositId = " + _deposit.DepositId.ToString() + ";", conn);
                cmd.ExecuteNonQuery();
            }

            return _deposit;
        }

        private Deposit DepositUpdate(Deposit deposit)
        {
            var _deposit = deposit;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Deposit SET Status = '" + (int)_deposit.Status + "', DateEnd = NOW() WHERE DepositId = " + _deposit.DepositId.ToString() + ";", conn);
                cmd.ExecuteNonQuery();
            }

            return _deposit;
        }

        private Deposit GetDeposit(int DepositId)
        {
            Deposit _deposit = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select d.*, u.UserName, e.TerminalId, u.UserId from Deposit d inner join Event e on d.EventId = e.EventId inner join User u on e.UserId = u.UserId where d.DepositId = " + DepositId.ToString(), conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _deposit = new Deposit();
                        _deposit.DepositId = int.Parse(reader["DepositId"].ToString());
                        _deposit.DepositNumber = reader["DepositNumber"].ToString();
                        _deposit.Status = (DepositStatus)Enum.Parse(typeof(DepositStatus), reader["Status"].ToString());//Enum.Parse(Type.GetType("DepositStatus"), reader["Status"].ToString());
                        _deposit.Amount = int.Parse(reader["Amount"].ToString());
                        _deposit.Date = reader["DateEnd"].ToString();
                        _deposit.UserId = int.Parse(reader["UserId"].ToString());
                        _deposit.UserName = reader["UserName"].ToString();
                        _deposit.TerminalId = int.Parse(reader["TerminalId"].ToString());
                    }
                }
            }

            return _deposit;
        }

        
        private Deposit GetBills(Deposit Deposit)
        {
            
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from DepositBill where DepositId=" + Deposit.DepositId.ToString(), conn);
                var myList = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        myList.Add(reader["Bill"].ToString());
                    }
                }
                Deposit.BillsDetail = myList.ToArray();
            }

            return Deposit;
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

        #endregion

        #endregion

        public ActionReturn Reboot(int terminalId, int userId)
        {
            var _terminal = GetTerminal(terminalId);

            var _event = new Event();

            _event.TerminalId = terminalId;

            if (_terminal != null)
            {

                _event.EventTypeId = 13;
                _event.EventType = EventType.Reboot;
                _event.UserId = userId;
                _event = EventInsert(_event);

                if (IsOnline(_terminal.IP))
                {

                    string _url = "http://" + _terminal.IP + "/reboot.php";

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            var response = client.GetStringAsync(_url).Result;

                        }

                        _event.Status = EventStatus.Successful;
                        _event = EventUpdate(_event);

                    }
                    catch
                    {
                        _event.Status = EventStatus.Error;
                        _event = EventUpdate(_event);

                    }
                }
                else
                {
                    _event.Status = EventStatus.OffLine;
                    _event = EventUpdate(_event);
                }
            }
            else
            {
                _event.Status = EventStatus.Error;
            }

            var _eventReturn = new ActionReturn();
            _eventReturn.TerminalId = _event.TerminalId;
            _eventReturn.EventType = _event.EventType.ToString();
            _eventReturn.Status = _event.Status.ToString();

            return _eventReturn;
        }

        public ActionReturn OpenDoorTCP(int terminalId, int userId)
        {
            var _terminal = GetTerminal(terminalId);

            var _event = new Event();

            _event.TerminalId = terminalId;

            if (_terminal != null)
            {
                _event.EventTypeId = 3;
                _event.EventType = EventType.OpenDoor;
                _event.UserId = userId;
                _event = EventInsert(_event);

                SimpleTcpClient clienttcp;
                clienttcp = new SimpleTcpClient();
                clienttcp.StringEncoder = Encoding.UTF8;
                clienttcp.Connect("192.168.1.5", Convert.ToInt32("8910"));
                clienttcp.WriteLineAndGetReply("hola desde el server", TimeSpan.FromSeconds(3));

                if (IsOnline(_terminal.IP))
                {



                    string _url = "http://" + _terminal.IP + "/open.php";

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            var response = client.GetStringAsync(_url).Result;

                        }

                        _event.Status = EventStatus.Successful;
                        _event = EventUpdate(_event);

                    }
                    catch
                    {
                        _event.Status = EventStatus.Error;
                        _event = EventUpdate(_event);

                    }
                }
                else
                {
                    _event.Status = EventStatus.OffLine;
                    _event = EventUpdate(_event);
                }
            }
            else
            {
                _event.Status = EventStatus.Error;
            }
            var _eventReturn = new ActionReturn();
            _eventReturn.TerminalId = _event.TerminalId;
            _eventReturn.EventType = _event.EventType.ToString();
            _eventReturn.Status = _event.Status.ToString();

            return _eventReturn;
        }

        public ActionReturn OpenDoor(int terminalId, int userId)
        {
            var _terminal = GetTerminal(terminalId);

            var _event = new Event();

            _event.TerminalId = terminalId;

            if (_terminal != null)
            {
                _event.EventTypeId = 3;
                _event.EventType = EventType.OpenDoor;
                _event.UserId = userId;
                _event = EventInsert(_event);

                if (IsOnline(_terminal.IP))
                {

                    string _url = "http://" + _terminal.IP + "/open.php";

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            var response = client.GetStringAsync(_url).Result;

                        }

                        _event.Status = EventStatus.Successful;
                        _event = EventUpdate(_event);

                    }
                    catch
                    {
                        _event.Status = EventStatus.Error;
                        _event = EventUpdate(_event);

                    }
                }
                else
                {
                    _event.Status = EventStatus.OffLine;
                    _event = EventUpdate(_event);
                }
            }
            else
            {
                _event.Status = EventStatus.Error;
            }
            var _eventReturn = new ActionReturn();
            _eventReturn.TerminalId = _event.TerminalId;
            _eventReturn.EventType = _event.EventType.ToString();
            _eventReturn.Status = _event.Status.ToString();

            return _eventReturn;
        }

        public DepositReturn Deposit(int terminalId, string number, int total, int userId)
        {
            var _terminal = GetTerminal(terminalId);

            var _event = new Event();

            _event.TerminalId = terminalId;
            _event.EventTypeId = 1;
            _event.EventType = EventType.Deposit;
            _event.UserId = userId;
            _event = EventInsert(_event);

            var _deposit = new Deposit();
            _deposit.EventId = _event.EventId;
            _deposit.DepositNumber = number;
            _deposit.Amount = total;
            _deposit.TerminalId = terminalId;

            if (_terminal != null)
            {

                if (IsOnline(_terminal.IP))
                {

                    _deposit.Status = DepositStatus.Sending;
                    _deposit = DepositInsert(_deposit);

                    string _url = "http://" + _terminal.IP + "/BasicValidator/Deposit.php?depositid=" + _deposit.DepositId.ToString() + "&total=" + _deposit.Amount.ToString() + "&timeoff=" + _terminal.TimeOff.ToString();

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            var response = client.GetStringAsync(_url).Result;
                            if (response == DepositStatus.Processing.ToString())
                            {
                                _event.Status = EventStatus.Successful;
                                _event = EventUpdate(_event);
                                _deposit.Status = DepositStatus.Processing;
                                _deposit = DepositUpdate(_deposit);
                               
                            }
                            else if (response == DepositStatus.Error.ToString())
                            {
                                _event.Status = EventStatus.Successful;
                                _event = EventUpdate(_event);
                                _deposit.Status = DepositStatus.Error;
                                _deposit = DepositUpdate(_deposit);
                               
                            }
                            else if (response == DepositStatus.Busy.ToString())
                            {
                                _event.Status = EventStatus.Successful;
                                _event = EventUpdate(_event);
                                _deposit.Status = DepositStatus.Busy;
                                _deposit = DepositUpdate(_deposit);
                               
                            }
                        }

                    }
                    catch
                    {
                        _event.Status = EventStatus.Error;
                        _event = EventUpdate(_event);
                        _deposit.Status = DepositStatus.Error;
                        _deposit = DepositUpdate(_deposit);
                       
                    }
                }
                else
                {
                    _event.Status = EventStatus.OffLine;
                    _event = EventUpdate(_event);
                    _deposit.Status = DepositStatus.OffLine;
                    _deposit = DepositUpdate(_deposit);
                }

            }

            DepositReturn _depositReturn = new DepositReturn();
            _depositReturn.Amount = _deposit.Amount;
            _depositReturn.Date = DateTime.Now.ToString();
            _depositReturn.DepositId = _deposit.DepositId;
            _depositReturn.Number = _deposit.DepositNumber;
            _depositReturn.Status = _deposit.Status.ToString();
            _depositReturn.TerminalId = _deposit.TerminalId;

            return _depositReturn;
        }

        public TerminalReturn UpdateDepositTimeOff(int terminalId, int timeOff, int userId)
        {
            TerminalReturn _statusReturn = new TerminalReturn();
            var _terminal = GetTerminal(terminalId);
            var _event = new Event();

            _event.TerminalId = terminalId;

            if (_terminal != null)
            {
                _event.EventTypeId = 24;
                _event.EventType = EventType.TimeOffUpdate;
                _event.UserId = userId;
                _event = EventInsert(_event);

                _terminal.TimeOff = timeOff;

                UpdateTerminalTimeOff(_terminal);

                _statusReturn.TerminalId = _terminal.TerminalId;
                _statusReturn.Name = _terminal.Name;
                _statusReturn.Address = _terminal.Address;
                _statusReturn.Bills = _terminal.Notes;
                _statusReturn.CashBoxDoor = _terminal.CashBoxDoor;
                _statusReturn.Description = _terminal.Description;
                if (IsOnline(_terminal.IP))
                    _statusReturn.Status = Entities.TerminalStatus.Online.ToString();
                else
                    _statusReturn.Status = Entities.TerminalStatus.Offline.ToString();
                _statusReturn.TerminalDoor = _terminal.TerminalDoor;
                _statusReturn.TimeOff = _terminal.TimeOff;

                _event.Status = EventStatus.Successful;
                _event = EventUpdate(_event);
            }
            else
            {
                _statusReturn.TerminalId = terminalId;
                _statusReturn.Status = TerminalStatus.Error.ToString();
            }

            return _statusReturn;
        }

        public List<TerminalReturn> Terminals(int userId)
        {
            var _terminals = ListTerminal();
            List<TerminalReturn> _listReturn = new List<TerminalReturn>();
            if (_terminals != null)
            {
                foreach (Terminal t in _terminals)
                {
                    TerminalReturn r = new TerminalReturn();
                    r.TerminalId = t.TerminalId;
                    r.Name = t.Name;
                    r.Address = t.Address;
                    r.Bills = t.Notes;
                    r.CashBoxDoor = t.CashBoxDoor;
                    r.Description = t.Description;
                   /* if (IsOnline(t.IP))
                        r.Status = Entities.TerminalStatus.Online.ToString();
                    else
                        r.Status = Entities.TerminalStatus.Offline.ToString();*/
                    r.TerminalDoor = t.TerminalDoor;
                    r.TimeOff = t.TimeOff;
                    r.TotalAmount = t.TotalAmount;
                    
                    _listReturn.Add(r);
                }
                
            }
            
            return _listReturn;
        }

        public TerminalReturn Status(int terminalId, int userId)
        {
            var _terminal = GetTerminal(terminalId);
            TerminalReturn _statusReturn = new TerminalReturn();
            if (_terminal != null)
            {
                _statusReturn.TerminalId = _terminal.TerminalId;
                _statusReturn.Name = _terminal.Name;
                _statusReturn.Address = _terminal.Address;
                _statusReturn.Bills = _terminal.Notes;
                _statusReturn.CashBoxDoor = _terminal.CashBoxDoor;
                _statusReturn.Description = _terminal.Description;
                if (IsOnline(_terminal.IP))
                    _statusReturn.Status = Entities.TerminalStatus.Online.ToString();
                else
                    _statusReturn.Status = Entities.TerminalStatus.Offline.ToString();
                _statusReturn.TerminalDoor = _terminal.TerminalDoor;
                _statusReturn.TimeOff = _terminal.TimeOff;
                _statusReturn.TotalAmount = _terminal.TotalAmount;
                _statusReturn.Notes = GetTerminalNotes(_terminal.TerminalId);
            }
            //missing validation when terminal does not exist  shoudl trow and errror.
            else
            {
                _statusReturn.TerminalId = terminalId;
                _statusReturn.Status = TerminalStatus.Offline.ToString();
            }

            return _statusReturn;
        }

        public DepositReturn GetDeposit(int DepositId, int userId)
        {
            var _deposit = GetDeposit(DepositId);
            DepositReturn _depositReturn = new DepositReturn();

            if (_deposit != null)
            {
                _deposit = GetBills(_deposit);

                _depositReturn.Amount = _deposit.Amount;
                _depositReturn.Date = DateTime.Parse(_deposit.Date).ToLocalTime().ToString();
                _depositReturn.DepositId = _deposit.DepositId;
                _depositReturn.Number = _deposit.DepositNumber;
                _depositReturn.Status = _deposit.Status.ToString();
                _depositReturn.TerminalId = _deposit.TerminalId;
                _depositReturn.BillsDetail = _deposit.BillsDetail;
                _depositReturn.UserName = _deposit.UserName;
            }

            return _depositReturn;

        }

        public ActionReturn DepositCancel(int DepositId, int TerminalId, int userId)
        {
            var _deposit = GetDeposit(DepositId);
            DepositReturn _depositReturn = new DepositReturn();
            var _event = new Event();
            _event.TerminalId = TerminalId;
            _event.EventTypeId = 2;
            _event.EventType = EventType.DepositCancel;
            _event.UserId = userId;
            _event = EventInsert(_event);

            if (_deposit != null)
            {
               
                if ((_deposit.UserId == userId) & (_deposit.Status == DepositStatus.Processing) & (_deposit.TerminalId == TerminalId))
                {

                    var _terminal = GetTerminal(_deposit.TerminalId);

                    if (IsOnline(_terminal.IP))
                    {

                        string _url = "http://" + _terminal.IP + "/BasicValidator/DepositCancel.php";

                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                var response = client.GetStringAsync(_url).Result;

                            }

                            _event.Status = EventStatus.Successful;
                            _event = EventUpdate(_event);


                        }
                        catch
                        {
                            _event.Status = EventStatus.Error;
                            _event = EventUpdate(_event);

                        }

                    }
                    else
                    {
                        _event.Status = EventStatus.OffLine;
                        _event = EventUpdate(_event);
                    }

                    /*_deposit = GetBills(_deposit);

                    _depositReturn.Amount = _deposit.Amount;
                    _depositReturn.Date = DateTime.Parse(_deposit.Date).ToLocalTime().ToString();
                    _depositReturn.DepositId = _deposit.DepositId;
                    _depositReturn.Number = _deposit.DepositNumber;
                    _depositReturn.Status = _deposit.Status.ToString();
                    _depositReturn.TerminalId = _deposit.TerminalId;
                    _depositReturn.BillsDetail = _deposit.BillsDetail;
                    _depositReturn.UserName = _deposit.UserName;*/

                }
                else
                {
                    _event.Status = EventStatus.Error;
                    _event = EventUpdate(_event);
                }
            }
            else
            {
                _event.Status = EventStatus.Error;
                _event = EventUpdate(_event);
            }
            

            var _eventReturn = new ActionReturn();
            _eventReturn.TerminalId = _event.TerminalId;
            _eventReturn.EventType = _event.EventType.ToString();
            _eventReturn.Status = _event.Status.ToString();

            return _eventReturn;

        }

        public void UpdateTerminal(UpdateTerminalReturn updateTerminal)
        {
            //check if terminal id exist , check if terminal name does not exist, check if values are empty
            if (updateTerminal == null)
            {
                throw new AppExceptions("Terminal Id is required");
            }
            if (string.IsNullOrEmpty(updateTerminal.Name))
            {
                throw new AppExceptions("Name is required");
            }
            
            else
            {
                var _terminal = GetTerminal(updateTerminal.TerminalId);

                var _updateTerminal = updateTerminal;

                if (_terminal == null)
                {
                    throw new AppExceptions("Terminal not found");
                }

                bool value = isNameUnique(updateTerminal.Name);

                if(updateTerminal.Name != _terminal.Name)
                {
                    if (!value)
                    {
                        throw new AppExceptions("Terminal name already exist");
                    }
                }

                UpdateSQl(_updateTerminal);
            }      
        }

        private void UpdateSQl(UpdateTerminalReturn _updateTerminal)
        {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE Terminal SET Name = '" + _updateTerminal.Name + "' , Address = '" + _updateTerminal.Address + "' , Description= '" + _updateTerminal.Description + "' WHERE TerminalId = " + _updateTerminal.TerminalId.ToString() + ";", conn);
                    cmd.ExecuteNonQuery();
                }
            }

        public TotalAmount getAllTerminalsTotalAmount(int userId)
        {
            TotalAmount totalAmount = new TotalAmount();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select sum(TotalAmount) as TotalAmount from Terminal", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        totalAmount.AllTotalAmount = double.Parse(reader["TotalAmount"].ToString());
                    }
                }
            }
            return totalAmount;
        }
    }
}
