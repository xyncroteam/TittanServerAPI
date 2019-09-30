using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newapi.Helpers;
using wscore.Entities;
using wscore.Helpers;
using wscore.Services;

namespace wscore.Controllers
{
    [Produces("application/json")]
    [Route("action")]
    public class ActionController : Controller
    {
        private IActionService _actionService;
        private ILoginService _loginService;

        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First().Value);
        }

        protected string GetUserName()
        {
            return this.User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
        }

        public ActionController(IActionService actionService, ILoginService loginService)
        {
            _actionService = actionService;
            _loginService = loginService;
        }

        #region DemoVersion

        [AllowAnonymous]
        [HttpPost("opendoorDemo")]
        public IActionResult OpenDoorDemo([FromBody]Event eventParam)
        {

            ActionReturn _eventReturn = new ActionReturn();
            _eventReturn.EventType = "OpenDoor";
            _eventReturn.Status = "OnLine";
            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();

            return Ok(_eventReturn);
        }
        [AllowAnonymous]
        [HttpPost("rebootDemo")]
        public IActionResult RebootDemo([FromBody]Event eventParam)
        {
            //var _eventReturn = _actionService.Reboot(eventParam.TerminalId, GetUserId());

            ActionReturn _eventReturn = new ActionReturn();

            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();

            return Ok(_eventReturn);
        }

        [AllowAnonymous]
        [HttpPost("getstatusDemo")]
        public IActionResult TerminalStatusDemo([FromBody]TerminalReturn statusParam)
        {
            // var _statusReturn = _actionService.Status(statusParam.TerminalId, GetUserId());

            TerminalReturn _statusReturn = new TerminalReturn();

            if (statusParam.TerminalId == 99)
            {

                //_statusReturn = _actionService.Status(statusParam.TerminalId, GetUserId()); ;
                //return Ok(_statusReturn);

                Notes notes = new Notes();

                notes.Note1 = 100;
                notes.Note10 = 57;
                notes.Note100 = 50;
                notes.Note20 = 36;
                notes.Note5 = 20;
                notes.Note50 = 20;

                _statusReturn.TerminalId = 99;
                _statusReturn.Status = "OnLine";
                _statusReturn.TerminalDoor = "Close";
                _statusReturn.Bills = 283;
                _statusReturn.Name = "Chevron Sunrise";
                _statusReturn.Description = "12607 W Sunrise Blvd, Fort Lauderdale, FL 33323";
                _statusReturn.TotalAmount = 7490;//_statusReturn.TotalAmount;
                _statusReturn.Notes = notes;

            }

            if (statusParam.TerminalId == 2)
            {
                Notes notes = new Notes();

                notes.Note1 = 183;
                notes.Note10 = 57;
                notes.Note100 = 150;
                notes.Note20 = 40;
                notes.Note5 = 220;
                notes.Note50 = 78;

                _statusReturn.TerminalId = 2;
                _statusReturn.Status = "OffLine";
                _statusReturn.TerminalDoor = "Close";
                _statusReturn.Bills = 728;
                _statusReturn.Name = "Panera Bread";
                _statusReturn.Description = "10001 Cleary Blvd, Plantation, FL 33324";
                _statusReturn.TotalAmount = 21553;
                _statusReturn.Notes = notes;

            }

            if (statusParam.TerminalId == 3)
            {
                Notes notes = new Notes();


                notes.Note100 = 1082;

                notes.Note50 = 557;

                _statusReturn.TerminalId = 3;
                //_statusReturn.Status = "OnLine";
                _statusReturn.TerminalDoor = "Close";
                _statusReturn.Bills = 1639;
                _statusReturn.Name = "Publix Super Market";
                _statusReturn.Description = "13700 W State Rd 84, Davie, FL 33325";
                _statusReturn.TotalAmount = 136050;
                _statusReturn.Notes = notes;

            }

            if (statusParam.TerminalId == 4)
            {
                Notes notes = new Notes();


                notes.Note20 = 1822;



                _statusReturn.TerminalId = 4;
                _statusReturn.Status = "OnLine";
                _statusReturn.TerminalDoor = "Close";
                _statusReturn.Bills = 1822;
                _statusReturn.Name = "CVS Weston";
                _statusReturn.Description = "1120 Weston Rd, Weston, FL 33326";
                _statusReturn.TotalAmount = 36440;
                _statusReturn.Notes = notes;

            }

            if (statusParam.TerminalId == 5)
            {
                Notes notes = new Notes();


                notes.Note1 = 857;
                notes.Note5 = 159;


                _statusReturn.TerminalId = 5;
                _statusReturn.Status = "OnLine";
                _statusReturn.TerminalDoor = "Open";
                _statusReturn.Bills = 1016;
                _statusReturn.Name = "Subway";
                _statusReturn.Description = "13606 W State Rd 84, Davie, FL 33325";
                _statusReturn.TotalAmount = 1652;
                _statusReturn.Notes = notes;

            }

            return Ok(_statusReturn);
        }
        [AllowAnonymous]
        [HttpPost("listTerminalsDemo")]
        public IActionResult ListTerminalsDemo()
        {
            //var _statusReturn = _actionService.Terminals(GetUserId()); ;

            List<TerminalReturn> _statusReturn = new List<TerminalReturn>();

            TerminalReturn _terminal1 = new TerminalReturn();

            //_terminal1 = _actionService.Status(99, GetUserId()); ;

            _terminal1.TerminalId = 99;
            _terminal1.Status = "OnLine";
            _terminal1.TerminalDoor = "Close";
            _terminal1.Bills = 283;
            _terminal1.Name = "Chevron Sunrise";
            _terminal1.Description = "12607 W Sunrise Blvd, Fort Lauderdale, FL 33323";
            _terminal1.TotalAmount = 7490;

            TerminalReturn _terminal2 = new TerminalReturn();

            _terminal2.TerminalId = 2;
            _terminal2.Status = "OffLine";
            _terminal2.TerminalDoor = "Close";
            _terminal2.Bills = 728;
            _terminal2.Name = "Panera Bread";
            _terminal2.Description = "10001 Cleary Blvd, Plantation, FL 33324";
            _terminal2.TotalAmount = 21553;


            TerminalReturn _terminal3 = new TerminalReturn();

            _terminal3.TerminalId = 3;
            _terminal3.Status = "OnLine";
            _terminal3.TerminalDoor = "Close";
            _terminal3.Bills = 1639;
            _terminal3.Name = "Publix Super Market";
            _terminal3.Description = "13700 W State Rd 84, Davie, FL 33325";
            _terminal3.TotalAmount = 136050;


            TerminalReturn _terminal4 = new TerminalReturn();

            _terminal4.TerminalId = 4;
            _terminal4.Status = "OnLine";
            _terminal4.TerminalDoor = "Close";
            _terminal4.Bills = 1822;
            _terminal4.Name = "CVS Weston";
            _terminal4.Description = "1120 Weston Rd, Weston, FL 33326";
            _terminal4.TotalAmount = 36440;


            TerminalReturn _terminal5 = new TerminalReturn();

            _terminal5.TerminalId = 5;
            _terminal5.Status = "OnLine";
            _terminal5.TerminalDoor = "Open";
            _terminal5.Bills = 1016;
            _terminal5.Name = "Subway";
            _terminal5.Description = "13606 W State Rd 84, Davie, FL 33325";
            _terminal5.TotalAmount = 1652;

            _statusReturn.Add(_terminal1);
            _statusReturn.Add(_terminal2);
            _statusReturn.Add(_terminal3);
            _statusReturn.Add(_terminal4);
            _statusReturn.Add(_terminal5);

            return Ok(_statusReturn);
        }

        #endregion

        [Authorize(Roles = "Admin")]
        [HttpPost("opendoortcp")]
        public IActionResult OpenDoorTCP([FromBody]EventTCP eventParam)
        {
            eventParam.UserId = GetUserId();
            var _eventReturn = _actionService.OpenDoorTCP(eventParam);
            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();

            return Ok(_eventReturn);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost("opendoor")]
        public IActionResult OpenDoor([FromBody]Event eventParam)
        {
            var _eventReturn = _actionService.OpenDoor(eventParam.TerminalId, GetUserId());
            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();

            return Ok(_eventReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("resettcp")]
        public IActionResult ResetTCP([FromBody]EventTCP eventParam)
        {
            eventParam.UserId = GetUserId();
            var _eventReturn = _actionService.ResetTCP(eventParam);
            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();

            return Ok(_eventReturn);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("reboot")]
        public IActionResult Reboot([FromBody]Event eventParam)
        {
            var _eventReturn = _actionService.Reboot(eventParam.TerminalId, GetUserId());
            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();

            return Ok(_eventReturn);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("updatetimeoffdeposit")]
        public IActionResult UpdateDepositTimeOff([FromBody]TerminalReturn eventParam)
        {
            var _eventReturn = _actionService.UpdateDepositTimeOff(eventParam.TerminalId, eventParam.TimeOff, GetUserId());

            return Ok(_eventReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("deposittcp")]
        public IActionResult DepositTCP([FromBody]EventTCP eventParam)
        {
            eventParam.UserId = GetUserId();
            var _eventReturn = _actionService.DepositTCP(eventParam);
            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();

            return Ok(_eventReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody]Deposit depositParam)
        {
            var _eventReturn = _actionService.Deposit(depositParam.TerminalId, depositParam.DepositNumber, depositParam.Amount, GetUserId());
            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();

            return Ok(_eventReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("getstatus")]
        public IActionResult TerminalStatus([FromBody]TerminalReturn statusParam)
        {
            var _statusReturn = _actionService.Status(statusParam.TerminalId); ;
            return Ok(_statusReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("listTerminals")]
        public IActionResult ListTerminals()
        {
            var _statusReturn = _actionService.Terminals(); ;
            return Ok(_statusReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("getdeposit")]
        public IActionResult GetDeposit([FromBody]Deposit depositParam)
        {
            var _depositReturn = _actionService.GetDeposit(depositParam.DepositId, GetUserId()); ;
            return Ok(_depositReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("canceldeposit")]
        public IActionResult DepositCancel([FromBody]Deposit depositParam)
        {
            var _eventReturn = _actionService.DepositCancel(depositParam.DepositId, depositParam.TerminalId, GetUserId());
            _eventReturn.Date = DateTime.Now.ToString();
            _eventReturn.UserName = GetUserName();
            return Ok(_eventReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("updateTerminal")]
        public IActionResult UpdateTerminal([FromBody]UpdateTerminalReturn updateTerminal)
        {
            try
            {
                _actionService.UpdateTerminal(updateTerminal);
                return Ok("Data updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = "Admin,User")]
        [HttpPost("deposits")]
        public IActionResult GetDetposits([FromBody]ReportRequest depositParam)
        {
            try
            {
                if (depositParam == null)
                {
                    throw new AppExceptions("Date invalid ");
                }
                var _statusReturn = _actionService.getDeposits(depositParam);
                return Ok(_statusReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("terminaldeposits")]
        public IActionResult GetTerminalDeposits([FromBody]TerminalRequest depositParam)
        {
            try
            {
                if (depositParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _statusReturn = _actionService.getDepositsByTerminal(depositParam.TerminalId);
                return Ok(_statusReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("depositNotes")]
        public IActionResult GetDepositNotes([FromBody]DepositNotesRequest depositParam)
        {
            try
            {
                if (depositParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _depositReturn = _actionService.DepositNotes(depositParam.DepositId);
                return Ok(_depositReturn);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("withdrawNotes")]
        public IActionResult GetWithdrawNotes([FromBody]WithdrawNotesRquest withdrawParam)
        {
            try
            {
                if (withdrawParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _withdrawReturn = _actionService.WithdrawNotes(withdrawParam.EventId);
                return Ok(_withdrawReturn);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("getTerminalsIds")]
        public IActionResult GetAllTerminalsIds()
        {
            var _statusReturn = _actionService.GetTerminalsIds();
            return Ok(_statusReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("asignUserToTerminal")]
        public IActionResult AsignUserToTerminal([FromBody]TerminalUserRequest requestParam)
        {
            try
            {
                if (requestParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                if (requestParam.TerminalId != null && requestParam.UserId != null)
                {
                    _actionService.asignUserToTerminal(requestParam);
                    return Ok("User asigned to terminal");
                }
                else
                {
                    throw new AppExceptions("Terminal or User Can not be empty");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("unasignUserFromTerminal")]
        public IActionResult UnasignUserFromTerminal([FromBody]TerminalUserRequest requestParam)
        {
            try
            {
                if (requestParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                if (requestParam.TerminalId != null && requestParam.UserId != null)
                {
                    _actionService.unasignUserFromTerminal(requestParam);
                    return Ok("User been unasign from terminal");
                }
                else
                {
                    throw new AppExceptions("Terminal or User Can not be empty");
                }                    
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = "Admin,User")]
        [HttpPost("getAllTerminalUsers")]
        public IActionResult GetAllTerminalUsers([FromBody]TerminalRequest requestParam)
        {
            try
            {
                if (requestParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _usersReturn = _actionService.getAllTerminalUsers(requestParam.TerminalId);
                return Ok(_usersReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("withdraw")]
        public IActionResult GetWithdraws([FromBody]ReportRequest withdrawParam)
        {
            try
            {
                if (withdrawParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _statusReturn = _actionService.getWidthraws(withdrawParam);
                return Ok(_statusReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("terminalOnline")]
        public IActionResult IsTerminalOnline([FromBody]TerminalRequest requestParam)
        {
            try
            {
                if (requestParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _statusReturn = _actionService.isTerminalOnline(requestParam.TerminalId);
                return Ok(_statusReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        #region DashBoard functions

        [Authorize(Roles = "Admin,User")]
        [HttpPost("allTerminalsTotalAmount")]
        public IActionResult GetAllTerminaslTotalAmount()
        {
            var _allTotalTerminaslAmountReturn = _actionService.getAllTerminalsTotalAmount();
            return Ok(_allTotalTerminaslAmountReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("totalDeposit")]
        public IActionResult GetAllTotalDeposit()
        {
            var _allTotalTerminaslAmountReturn = _actionService.getAllTotalDeposit();
            return Ok(_allTotalTerminaslAmountReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("totalWithdraw")]
        public IActionResult GetAllTotalWidthraws()
        {
            var _totalTerminalsWithdrawAmountReturn = _actionService.getAllTotalWithdraw();
            return Ok(_totalTerminalsWithdrawAmountReturn);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("offlineterminals")]
        public IActionResult GetAllOfflineTerminals()
        {
            var _allOfflineTerminals = _actionService.getAllOfflineTerminals();
            return Ok(_allOfflineTerminals);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("terminalscapacity")]
        public IActionResult GetTerminalsCapacity()
        {
            var _terminalsCapacity = _actionService.getTerminalsCapacity();
            return Ok(_terminalsCapacity);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("terminalspercentage")]
        public IActionResult GetAllTerminalsPercentage()
        {
            var _terminalsCapacity = _actionService.getAllTerminalsPercentage();
            return Ok(_terminalsCapacity);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("terminalbillcapacity")]
        public IActionResult GetTerminalBillCapacity([FromBody]TerminalRequest terminalIdparam)
        {
            try
            {
                var capacityBills = _actionService.GetTerminalCapacityBills(terminalIdparam.TerminalId);
                return Ok(capacityBills);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("totalsystembills")]
        public IActionResult GetTotalTerminalBills([FromBody]TerminalRequest terminalIdparam)
        {
            try
            {
                var totalsystemBills = _actionService.getTotalTerminalBills(terminalIdparam.TerminalId);
                return Ok(totalsystemBills);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("cahboxbills")]
        public IActionResult GetCashBoxNumberBills([FromBody]CashBoxRequest cashBoxparam)
        {
            try
            {
                var cashBoxBills = _actionService.GetCashBoxBills(cashBoxparam);
                return Ok(cashBoxBills);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("terminalevents")]
        public IActionResult GetTerminalEvents([FromBody]EventRequest eventparam)
        {
            try
            {
                if (eventparam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _statusReturn = _actionService.getEventsByTerminal(eventparam);
                return Ok(_statusReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin,User")]
        [HttpPost("latestterminalevents")]
        public IActionResult GetLatestEventsByTerminal([FromBody]TerminalRequest depositParam)
        {
            try
            {
                if (depositParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _statusReturn = _actionService.getEventsByTerminal(depositParam.TerminalId);
                return Ok(_statusReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        #endregion
    }
}

