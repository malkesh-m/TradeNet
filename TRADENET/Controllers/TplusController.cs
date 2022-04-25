using DevExtreme.AspNet.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using TRADENET.Const;
using TRADENET.Models;


namespace TRADENET.Controllers
{
    //[Route(Name = "Tplus")]
    public class TplusController : Controller
    {
        LibraryModel mylib = new LibraryModel();
        UtilityModel myutil = new UtilityModel();

        // GET: Tplus
        // Called From Menu
        //Start #region Menu

        //Declare all your call from menus here only strictly 

        [Authorize]
        public ActionResult DashBoard()

        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                UtilityModel myutil = new UtilityModel();
                UtilityDBModel util = new UtilityDBModel();
                LibraryModel MyLib = new LibraryModel();
                modUserDetailsModel ud = new modUserDetailsModel();
                //string user1 = myutil.gstrUserCd();
                //UserMasterModel user = ud.GetUserDetails(user1);
                util.run_menuQuery();



                string strMaxBillDate = util.GetMaxBillDate();
                if (strMaxBillDate == "" || strMaxBillDate == null)
                {
                    strMaxBillDate = DateTime.Now.ToString("dd/MM/yyyy");
                }
                ViewBag.MaxBillDate = strMaxBillDate;

                string strMaxTrxDate = util.GetMaxTrxDate();
                ViewBag.MaxTradeDate = strMaxTrxDate;

                modDashBoard dashboard = new modDashBoard();

                this.Session["USERMENU"] = dashboard.GetMenuDetails();

                DataTable dt = dashboard.GetMenuDetails("H0000000");
                modDashBoardUser dbuser = new modDashBoardUser
                {
                    blnShowDashBoard = false,
                    blnShowBillbtn = false,
                    blnShowBrokerageChart = false,
                    blnShowCharts = false,
                    blnShowClientbtn = false,
                    blnShowCreditorsChart = false,
                    blnShowDebtorsChart = false,
                    blnShowRiskMgbtn = false,
                    blnShowRMSChart = false,
                    blnShowSearchbtn = false,
                    blnShowTradesbtn = false
                };

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dRow in dt.Rows)
                    {
                        if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0000000")
                        {
                            dbuser.blnShowDashBoard = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0100000")
                        {
                            dbuser.blnShowClientbtn = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0200000")
                        {
                            dbuser.blnShowBillbtn = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0300000")
                        {
                            dbuser.blnShowTradesbtn = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0400000")
                        {
                            dbuser.blnShowRiskMgbtn = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0500000")
                        {
                            dbuser.blnShowSearchbtn = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0600000")
                        {
                            dbuser.blnShowCharts = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0601000")
                        {
                            dbuser.blnShowBrokerageChart = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0602000")
                        {
                            dbuser.blnShowRMSChart = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0603000")
                        {
                            dbuser.blnShowDebtorsChart = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                        else if (dRow["wmn_modulecd"].ToString().ToUpper() == "H0604000")
                        {
                            dbuser.blnShowCreditorsChart = dRow["wmn_visibleyn"].ToString().Trim() == "1";
                        }
                    }
                }
                DataTable dstemp = new DataTable();
                string strsql = "";
                strsql = "select TNC_OptCode, (cast(TNC_MaxRptDays AS varchar(30)) ";
                strsql = strsql + " + ',' + cast(TNC_FutureYn AS varchar(30))+ ',' + cast(TNC_AfterDt as varchar(30)) + ',' + cast(TNC_PastYn as varchar(30)) + ',' + cast(TNC_Filler1 as varchar(30)) + ',' + cast(TNC_Filler2 as varchar(30))) as STR_parm";
                strsql = strsql + " from tradenetcontrol";
                dstemp = MyLib.OpenDataTable(strsql);
                string strApp = "";
                for (int j = 0; j < dstemp.Rows.Count; j++)
                {
                    strApp = dstemp.Rows[j]["TNC_OptCode"].ToString();
                    this.Session[strApp] = dstemp.Rows[j]["STR_parm"];

                }
                myutil.LoginAccessOld();
                //myutil.LoginAccessOld();
                HttpContext.Application["CMSCHEDULE"] = mylib.fnGetSysParam("CMSCHEDULE");
                if (util.commexTemp_conn("Commex") != null)
                {
                    util.getallwebPeram();
                    util.getCompanyDetail();
                }
                //this.Session["DashBoardRights"] = dbuser;
                return View(dbuser);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }


        public ActionResult ReceiptEntry(bool fromDashBoard = false, string Code = "", Boolean img = false)
        {
            ClearUnwantedSessions();
            ViewBag.Receiptfromdashboard = fromDashBoard;
            LibraryModel mylib = new LibraryModel();
            UtilityDBModel myutil = new UtilityDBModel();
            UtilityModel util = new UtilityModel();
            DataTable dt = null;
            SqlConnection objConn = new SqlConnection();
            // string objConn = "";
            string strsqlOther = "";
            ViewBag.Msg = img;
            ViewBag.Viewimg = false;
            // string StrValidConn = myutil.CheckConnection("Tplusimages");
            string StrValidConn = "0";
            string msg = "";

            string strGroup = "select LA_grouping from LoginAccess Where LA_UserId ='" + util.gstrUserCd() + "' and LA_grouping in ('A','B') Order by LA_grouping";
            DataTable dtLogin = mylib.OpenDataTable(strGroup);
            if (dtLogin.Rows.Count == 0)
            {
                TempData["BankAlert"] = "You have no Banks assigned to you, Cannot Proceed \nContact Head Office";
                return RedirectToAction("Dashboard");
            }

            string strrsRight = "";
            strrsRight = this.Session["216"].ToString();

            string[] rsRight = strrsRight.Split(',');

            ViewBag.Viewimg = false;
            // string StrValidConn = myutil.CheckConnection("Tplusimages");
            ViewBag.gstrToday = DateTime.Now.ToString("yyyyMMdd");
            ViewBag.strRightFutureYn = rsRight[1];
            ViewBag.strRightPastYn = rsRight[3];
            ViewBag.strRightAfterDt = rsRight[2];
            string strGetTMinus2BillDate = myutil.GetMinusT2BillDate();

            string date1 = util.dtos(strGetTMinus2BillDate);


            DateTime date2 = new DateTime();
            date2 = util.stod(date1);
            if (date1 != "")
            {
                date2 = util.stod(date1);
            }
            else
            {
                date2 = util.stod(DateTime.Now.ToString());
            }


            int nodays = (date2 - DateTime.Now).Days;


            ViewBag.T2BillDate = nodays;

            strsqlOther = "select Op_product from other_products where op_product='Tplusimages' and OP_Status='A' ";
            //if (objConn.State == ConnectionState.Closed)
            //{
            //    objConn.Open();
            //}
            dt = mylib.OpenDataTable(strsqlOther);
            if (dt.Rows.Count > 0)

            {
                objConn = myutil.SlipScanTemp_conn("Tplusimages");
                if (objConn == null)
                {
                    ViewBag.Msg = true;
                    ViewBag.Viewimg = false;
                }
                else
                {
                    ViewBag.Msg = false;
                    ViewBag.Viewimg = true;
                }
            }
            else
            {
                ViewBag.Msg = false;
                ViewBag.Viewimg = false;
                // msg = "Unable to Connect Tplusimages";
            }


            if (fromDashBoard)
            {

                string strName = mylib.fnFireQuery("Client_master", "cm_name", "cm_cd", Code, false);
                modReceiptPayment recpay = new modReceiptPayment();
                IEnumerable<JsonComboModel> micr = recpay.FillMICR(Code);

                ReceiptTableModel rec = new ReceiptTableModel()
                {
                    rc_clientcd = Code,
                    cm_name = strName.Trim(),
                    GetAllMicr = micr
                };
                return View(rec);
            }
            else
            {
                return View();
            }



        }

        [Authorize]
        public ActionResult LedgerView()
        {
            ClearUnwantedSessions();

            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                UtilityDBModel util = new UtilityDBModel();
                string strGetT2BillDate = util.GetT2BillDate();
                ViewBag.T2BillDate = strGetT2BillDate;
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        [Authorize]
        public ActionResult LedgerViewCom()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult GetClientDetails(string criteria, string SearchBy)
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                modClientInformationModel cm = new modClientInformationModel();
                ModelState.Clear();
                var freeze = cm.GetClientMaster(criteria, SearchBy);
                return Json(freeze, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult GetClientDetailsLedger(string criteria, string SearchBy)
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                modClientInformationModel cm = new modClientInformationModel();
                ModelState.Clear();
                var freeze = cm.GetClientMasterLedger(criteria, SearchBy);
                return Json(freeze, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }
        public ActionResult GetSecurityDetails(string criteria, string SearchBy)
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                modClientInformationModel cm = new modClientInformationModel();
                ModelState.Clear();
                var freeze = cm.GetClientMasterWithSecurity(criteria, SearchBy);
                return Json(freeze, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }


        public ActionResult continuosdebit()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult QuaterlySettlement()
        {
            ClearUnwantedSessions();
            return View();
        }

        public ActionResult CKYCEntry()
        {
            ClearUnwantedSessions();
            DataTable dt = new DataTable();
            return View(dt);
        }

        public ActionResult ClientBrokChange()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult CombinedContractNote()
        {
            ClearUnwantedSessions();
            return View();
        }


        public ActionResult ComConfirmation()
        {
            ClearUnwantedSessions();
            return View();
        }

        [Authorize]
        public ActionResult ProfitLossView()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        [Authorize]
        public ActionResult BrokerageView()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult BillSummaryView()
        {
            ClearUnwantedSessions();
            return View();
        }

        public ActionResult TradeListingView(string Code, string ExchSeg, string FDate, string TDate, string Report, string GroupBy, string security, string SearchBy)
        {
            ClearUnwantedSessions();
            modTrades cp = new modTrades();
            UtilityDBModel dbmyutil = new UtilityDBModel();
            TDate = dbmyutil.GetMaxTrxDate();
            TDate = DateTime.ParseExact(TDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString();
            IEnumerable<ClientPositionModel> dp = cp.GetClientPositionReport("", "", "", "", TDate, "CC", "CL", "", "");
            ModelState.Clear();
            ViewBag.Report = "CC";

            UtilityModel myutil = new UtilityModel();
            TDate = myutil.DbToDate(TDate);
            ViewBag.DateFilter = "[" + TDate + "]";
            return View(dp);
        }


        public ActionResult FOutstandingPosition()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult ECommissionRep()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult RiskManagementView(string type, string code, bool blnFromDashBoard = false)
        {
            if (Session["LoginAccessOld"] != null)
            {
                ClearUnwantedSessions();
                LibraryModel objRMSlib = new LibraryModel();

                DataTable dt = null;

                dt = objRMSlib.OpenDataTable("SELECT sp_sysvalue FROM Sysparameter where sp_sysvalue like '%rs_NFiller2%'");

                if (dt.Rows.Count > 0)
                {
                    ViewBag.RMSFORMULA = dt.Rows[0]["sp_sysvalue"];
                }

                string strSQL = "";
                string newLn = Environment.NewLine;
                LibraryModel mylib = new LibraryModel();
                string RMSParameter = mylib.GetScalarValueString("Select sp_sysvalue From sysparameter Where sp_parmcd = 'TNETRMSHEAD'");

                string RMSReportHeading = "";



                if (!RMSParameter.Contains("-a"))
                {
                    strSQL += "cast((sum(rs_tld))as decimal(15,2)) as 'TDAY'," + newLn;
                    RMSReportHeading = "TDAY,";
                }
                if (!RMSParameter.Contains("-b"))
                {
                    strSQL += "cast((sum(rs_t2ld))as decimal(15,2)) as 'T2DAY'," + newLn;
                    RMSReportHeading += "T2DAY,";
                }
                if (!RMSParameter.Contains("-c"))
                {
                    strSQL += "cast((sum(rs_uncleared))as decimal(15,2)) as 'UnCleared'," + newLn;
                    RMSReportHeading += "UnCleared,";
                }
                if (!RMSParameter.Contains("-d"))
                {
                    strSQL += "cast((sum(rs_CashColl))as decimal(15,2)) as 'CashDeposit'," + newLn;
                    RMSReportHeading += "CashDeposit,";
                }
                UtilityModel myutil = new UtilityModel();
                bool isBPT = myutil.fnisBPT();
                if (isBPT)
                {
                    if (!RMSParameter.Contains("-e"))
                    {
                        strSQL += "cast((sum(rs_Collateral))as decimal(15,2)) as 'ApprovedShares'," + newLn;
                        RMSReportHeading += "ApprovedShares,";
                    }
                }
                else
                {
                    if (!RMSParameter.Contains("-e"))
                    {

                        strSQL += "cast((sum(rs_Collateral))as decimal(15,2)) as 'ShareCollateral'," + newLn;
                        RMSReportHeading += "ShareCollateral,";
                    }
                }
                if (!RMSParameter.Contains("-f"))
                {
                    strSQL += "cast((sum(rs_Margin)) as decimal(15,2)) as 'Margin'," + newLn;
                    RMSReportHeading += "Margin,";
                }
                if (!RMSParameter.Contains("-g"))
                {
                    strSQL += "cast((sum(rs_PoolHolding)) as decimal(15,2)) as 'Pool'," + newLn;
                    RMSReportHeading += "Pool,";
                }
                if (!RMSParameter.Contains("-h"))
                {
                    strSQL += "cast((sum(rs_DPHolding)) as decimal(15,2)) as 'DPHolding'," + newLn;
                    RMSReportHeading += "DPHolding,";
                }
                if (isBPT)
                {
                    if (!RMSParameter.Contains("-i"))
                    {
                        strSQL += "cast((sum(rs_Collateral+rs_BenHoldingWH + rs_ExpValueWH + rs_PoolHoldingWH - rs_UndelvValueWH - rs_ShortPayin))as decimal(15,2)) as 'Stock'," + newLn;
                        RMSReportHeading += "Stock,";
                    }
                    if (!RMSParameter.Contains("-j"))
                    {
                        strSQL += "cast(( sum(-rs_T2ld+rs_BenHolding+rs_PoolHolding+rs_ExpValue-rs_UndelvValue+rs_Collateralwh+rs_cashcoll+rs_BGColl))as decimal(15,2)) as 'Net'," + newLn;
                        RMSReportHeading += "Net,";
                    }
                }
                else
                {
                    if (!RMSParameter.Contains("-i"))
                    {
                        strSQL += "cast((sum(rs_BenHolding + rs_ExpValue + rs_PoolHolding + rs_DPHolding - rs_UndelvValue - rs_ShortPayin)) as decimal(15,2)) as 'Stock'," + newLn;
                        RMSReportHeading += "Stock,";
                    }
                    if (!RMSParameter.Contains("-j"))
                    {
                        strSQL += "cast((sum(-rs_T2ld+rs_BenHolding+rs_PoolHolding+rs_DPHolding+rs_ExpValue-rs_UndelvValue+rs_Collateralwh+rs_cashcoll+rs_BGColl))as decimal(15,2)) as 'Net'," + newLn;
                        RMSReportHeading += "Net,";
                    }
                }
                if (!RMSParameter.Contains("-k"))
                {
                    strSQL += "cast((sum(rs_outabovedays)) as decimal(15,2)) as 'Abovedays'," + newLn;
                    RMSReportHeading += "Abovedays,";
                }
                strSQL = Strings.Left(strSQL, strSQL.Length - 1);

                string[] ArrRMSCol = RMSParameter.Split(',');
                for (int i = 0; i < ArrRMSCol.Length; i++)
                {
                    if (Strings.Left(ArrRMSCol[i].Trim(), 1) != "-" && Strings.Left(ArrRMSCol[i].Trim(), 1) != "")
                    {
                        string parameter = "select sp_sysvalue from sysparameter where sp_parmcd like 'RMSFormula" + ArrRMSCol[i].Trim() + "' and sp_sysvalue<>'' ";
                        string rmsformula = mylib.GetScalarValueString(parameter);
                        if (rmsformula.Trim() != "")
                        {
                            parameter = "select sp_sysvalue from sysparameter where sp_parmcd like 'RMSHEAD" + ArrRMSCol[i].Trim() + "' and sp_sysvalue<>'' ";
                            string rhead = mylib.GetScalarValueString(parameter);
                            if (rhead.Trim() != "")
                            {
                                strSQL += ", cast(" + (rmsformula.ToUpper().Contains("SUM") ? "" : "Sum") + "(" + rmsformula + ") as decimal(15, 2)) '" + rhead.Trim().Replace(" ", "").Replace(".", "") + "'";
                                RMSReportHeading += rhead.Trim().Replace(" ", "").Replace(".", "") + ",";
                            }
                        }
                    }
                }
                ViewBag.RMSHEAD = RMSReportHeading;
                ViewBag.blnFromDashBoard = blnFromDashBoard;
                return View();

            }
            else
            { return RedirectToAction("Login1"); }
        }

        public ActionResult SharePayoutRequest()
        {
            ClearUnwantedSessions();
            return View();
        }

        public ActionResult BulkPayout()
        {
            ClearUnwantedSessions();
            return View();
        }

        public ActionResult MTFRMSummary()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                bool blnBSE = mylib.GetSysParmSt("MTFP_LICBSE", "") == "Y";
                bool blnNSE = mylib.GetSysParmSt("MTFP_LICNSE", "") == "Y";
                if (blnBSE && blnNSE)
                {
                    ViewBag.Exch = "A";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult MTFClientWiseReport()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult MTFSecurityList()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult MTFTradeListing()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }

        }

        public ActionResult MTFContinuousShortfall()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult ChangePwd()
        {
            ClearUnwantedSessions();
            return View();
        }

        //End #region Menu

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty));
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetNoStore();
            //return View();
            //FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("Login1", "Tplus");
        }
        public ActionResult MyAccount()
        {
            LibraryModel myLib = new LibraryModel();
            string strsql = "select cm_cd, cm_Name from Client_master b Where exists(select RM_GLActCode from Client_master a, SubBrokers where  a.cm_subbroker = RM_CD   " + Session["LoginAccessOld"] + " and b.cm_cd = RM_GLActCode)";
            // string strsql = "select distinct RM_CD,RM_Name from Client_master A join SubBrokers B on A.cm_subbroker=B.RM_CD  " + Session["LoginAccessOld"];
            DataTable dt = myLib.OpenDataTable(strsql);

            return View(dt);
        }

        //LOGIN
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login1()
        {
            return View();
        }

        //LOGIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login1(UserMasterModel um)
        {
            // this action is for handle post (login)
            AfterLoginExecution objTpluseModle = new AfterLoginExecution();
            modUserDetailsModel ud = new modUserDetailsModel();
            UserMasterModel user = ud.GetUserDetails(um.um_user_id);
            UtilityModel util = new UtilityModel();

            if (util.ckyc() == true)
            {

                if (ModelState.IsValid && um != null) // this is check validity
                {


                    if (user == null)
                    {

                    }
                    else
                    {
                        if (user.Um_Locked == "Y")
                        {
                            ViewBag.Message = "User Has Been Locked, Contact Head office to get it re-activated..";
                        }
                        else if (user.um_status == "I")
                        {
                            ViewBag.Message = "User Is Inactive.";
                        }
                        else
                        {
                            UtilityModel myutil = new UtilityModel();
                            if (um.um_passwd == myutil.Decrypt(user.um_passwd))
                            {

                                EntityMasterModel em = GetEntityDetails();
                                ConfigurationManager.AppSettings["OrganizationName"] = em.em_name;

                                this.Session["gstrUsercd"] = um.um_user_id.Trim().ToUpper();
                                FormsAuthentication.SetAuthCookie(um.um_user_id, false);
                                //objTpluseModle.insertlastlogindate(um.um_user_id);
                                return RedirectToAction("DashBoard");


                                // return RedirectToAction("ProfitLossViewCash");
                            }
                            else
                            {
                                ViewBag.Message = "Please enter valid password";
                            }

                        }


                    }
                }
                else
                {
                    ViewBag.Message = "Please enter valid UserName / Password";
                }
            }
            else
            { ViewBag.Message = "You are not authorise  to use TradeNet Application."; }

            return View(um);
        }
        // LOGIN
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login1(UserMasterModel um)
        //{
        //    // this action is for handle post (login)
        //    if (ModelState.IsValid && um != null) // this is check validity
        //    {

        //        modUserDetailsModel ud = new modUserDetailsModel();
        //        UserMasterModel user = ud.GetUserDetails(um.um_user_id, um.um_passwd);
        //        if (user == null)
        //        {
        //        }
        //        else
        //        {
        //            EntityMasterModel em = GetEntityDetails();
        //            ConfigurationManager.AppSettings["OrganizationName"] = em.em_name;

        //            this.Session["gstrUsercd"] = um.um_user_id.Trim().ToUpper();
        //            FormsAuthentication.SetAuthCookie(um.um_user_id, false);

        //            return RedirectToAction("DashBoard");
        //            //return RedirectToAction("ProfitLossView");
        //        }
        //    }
        //    @ViewBag.Message = "Please enter valid UserName / Password";
        //    return View(um);
        //}
        public EntityMasterModel GetEntityDetails()
        {
            modEntityMaster em = new modEntityMaster();
            EntityMasterModel umodel = em.GetEntityMaster();
            return umodel;
        }

        public ActionResult SessionTimeOut()
        {
            return View();
        }


        //PROFIT LOSS REPORT CASH//ajax call: url = '@Url.Action("ViewProfitLoss", "Tplus")'
        public ActionResult ProfitLossViewCash(string code, string startDt, string toDt, string CName)
        {
            ClearUnwantedSessions();
            ViewBag.code = code;
            ViewBag.CName = CName;
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                modProfitLoss pl = new modProfitLoss();
                IEnumerable<ProfitLossCashModel> ulist = pl.GetProfitLossCash(code, startDt, toDt);
                return View(ulist);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        //PROFIT LOSS REPORT FO //ajax call:url = '@Url.Action("ViewProfitLossFO", "Tplus")'
        public ActionResult ProfitLossViewFO(string code, string strexchange, string segment, string startDt, string toDt, string CName)
        {
            ClearUnwantedSessions();
            ViewBag.code = code;
            ViewBag.CName = CName;
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                modProfitLoss pl = new modProfitLoss();
                IEnumerable<ProfitLossFOModel> ulist = pl.GetProfitLossFO(strexchange, segment, code, startDt, toDt);
                return View(ulist);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult ProfitLossViewCommodity(string code, string strexchange, string segment, string startDt, string toDt)
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                modProfitLoss pl = new modProfitLoss();
                DataTable ulist = pl.GetProfitLossCommodity(strexchange, segment, code, startDt, toDt);
                return View(ulist);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult ProfitLossDetail(string code, string strscripcd, string Fromdt, string Todt, string strscripnm)
        {
            ViewBag.strscripcd = strscripcd;
            ViewBag.strscripnm = strscripnm;
            modProfitLoss pl = new modProfitLoss();
            DataTable dt = pl.GetProfitLossDetail(code, strscripcd, Fromdt, Todt);
            return View(dt);

        }

        //Search for BOID Name and BackOfficeCD Code;
        public ActionResult GetDPServiceDetails(string criteria, string SearchBy)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                modClientInformationModel cm = new modClientInformationModel();
                ModelState.Clear();
                var freeze = cm.GetDPServiceMaster(criteria, SearchBy);
                return Json(freeze, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }
        public ActionResult GetDPServiceCrossDetails(string criteria, string SearchBy)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                modClientInformationModel cm = new modClientInformationModel();
                ModelState.Clear();
                var freeze = cm.GetDPServiceCrossMaster(criteria, SearchBy);
                return Json(freeze, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult GetBSENSEISINDetails(string criteria, string RdBSE, string RdISIN)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                modClientInformationModel cm = new modClientInformationModel();
                ModelState.Clear();
                var freeze = cm.GetBSENSEISINMaster(criteria, RdBSE, RdISIN);
                return Json(freeze, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult GetClientPanDetails(string criteria)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                modClientInformationModel cm = new modClientInformationModel();
                ModelState.Clear();
                var freeze = cm.GetClientMasterWithPan(criteria);
                return Json(freeze, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        //url: '@Url.Action("GetClientSummary", "Tplus")',
        public ActionResult GetClientSummary(string Code)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                UtilityModel myUtil = new UtilityModel();
                if (myUtil.fnisBPT())
                {
                    ViewBag.isBPT = "True";
                }
                else
                {
                    ViewBag.isBPT = "False";
                }
                modClientInformationModel cm = new modClientInformationModel(Code);
                DashBoardClientSummaryModel dc = cm.GetDashBoardClientSummary();
                return View(dc);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }


        //Ajax Call : url: '@Url.Action("LedgerViewReport", "Tplus")',
        public ActionResult LedgerViewReport(string Select, string Code1, string Code, string FromDate, string ToDate, string strDPID = "", string strLedgerListid = "", string strchkConfirmation = "", string strcmbLedgerList = "", string strcmbActType = "", string chkMrgnlegr = "")
        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            ViewBag.clientcondition = "Notview";
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.strchkConfirmation = strchkConfirmation;
            if (Code != "")
            {

                ViewBag.clientcondition = "view";
                ViewBag.strCompOff = mylib.GetSysParmSt("LGR_COMPOFF", "Stationary");
            }

            //FromDate = myutil.dtos(FromDate);
            //ToDate = myutil.dtos(ToDate);
            modLedger led = new modLedger();
            IEnumerable<LedgerModel> ulist = led.GetLedger(Select, Code1, Code, FromDate, ToDate, strDPID, strLedgerListid, strchkConfirmation, strcmbLedgerList, strcmbActType, chkMrgnlegr);


            return View(ulist);
        }

        public ActionResult MyAccountLedger(string Select, string Code, string FromDate, string ToDate, string strDPID = "", string strLedgerListid = "", string strchkConfirmation = "", string strcmbLedgerList = "", string strcmbActType = "", string chkMrgnlegr = "", string ddlLedgerYear = "")
        {
            ViewBag.Code = Code;

            ViewBag.LedgerYear = ddlLedgerYear;
            ViewBag.Dfdate = FromDate;
            if (ddlLedgerYear.ToString().Trim() == "Current Year")
            { ViewBag.DTdate = DateTime.Now.ToString("dd/MM/yyyy"); }
            else
            {
                ViewBag.DTdate = ToDate;
            }

            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            ViewBag.clientcondition = "Notview";
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.strchkConfirmation = strchkConfirmation;
            if (Code != "")
            {
                ViewBag.clientcondition = "view";
                ViewBag.strCompOff = mylib.GetSysParmSt("LGR_COMPOFF", "Stationary");
            }

            FromDate = myutil.dtos(FromDate);
            ToDate = myutil.dtos(ToDate);
            modLedger led = new modLedger();
            IEnumerable<LedgerModel> ulist = led.GetLedger(Select, "", Code, FromDate, ToDate, strDPID, strLedgerListid, strchkConfirmation, strcmbLedgerList, strcmbActType, chkMrgnlegr);

            return View(ulist);
        }

        public ActionResult LedgerViewComReport(string Select, string Code, string FromDate, string ToDate, string strDPID, string strcmbActType)
        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            ClearUnwantedSessions();
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            ViewBag.clientcondition = "Notview";
            if (Code != "")
            {

                ViewBag.clientcondition = "view";
                ViewBag.strCompOff = mylib.GetSysParmSt("LGR_COMPOFF", "Stationary");
            }
            modLedger led = new modLedger();
            IEnumerable<LedgerModel> ldt = led.GetLedgerCom(Select, Code, FromDate, ToDate, strDPID, strcmbActType);



            return View(ldt);
        }

        public ActionResult ComBillPrint(string strDpid, string Code, string FromDt)
        {
            modLedger led = new modLedger();
            DataTable dt = led.GetComBillPrint(strDpid, Code, FromDt);
            return View(dt);

        }

        //Ajax Call : url: '@Url.Action("LedgerViewReport", "Tplus")',
        public ActionResult BrokerageViewAnalysis(string Select, string Code, string FromDate, string ToDate, string strDPID = "", string Viewtype = "", string ClientType = "")
        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }

            modBrokerage led = new modBrokerage();
            DataTable dt = led.GetClientBrokerage(Select, Code, FromDate, ToDate, strDPID, ClientType);

            //IEnumerable<BrokerageModel> ulist = led.GetClientBrokerage(Select, Code, FromDate, ToDate, strDPID, ClientType);
            return View(dt);
        }

        public ActionResult RMSLink(string Code, string curyear)
        {
            string NewLn = Environment.NewLine;
            modLedger led = new modLedger();
            List<LedgerViewModel> dv = led.GetClientLedgerSummary(Code, curyear, "Yes");
            ModelState.Clear();
            return View(dv);
        }

        //url: '@Url.Action("GetRMSByCode", "Tplus")',
        public ActionResult GetRMSByCode(string Code)
        {
            modRiskManagementSystem rms = new modRiskManagementSystem()
            {
                Code = Code,
                type = "CL"
            };
            List<DyanmicModel> ulist = rms.GetRMSReportByCode();
            ModelState.Clear();
            return View(ulist);
        }

        //url: '@Url.Action("GetQuarterlySquareOffByCode", "Home")',
        public ActionResult GetQuarterlySquareOffByCode(string Code, string strdate)
        {
            if (ModelState.IsValid)
            {

                modQuarterlySquareOff qs = new modQuarterlySquareOff();
                QuarterlySquareOffModel ulist = qs.GetQuarterlySquareOffByCode(Code, strdate);
                ModelState.Clear();
                return View(ulist);
            }
            else
            {
                return View();
            }
        }

        //url: '@Url.Action("GetDashBoardClientCount", "Tplus")',
        public ActionResult GetDashBoardClientCount()
        {
            modReceiptPayment rec = new modReceiptPayment();
            DashBoardClientCountModel dc = rec.GetDashBoardClient();
            ModelState.Clear();
            return View(dc);
        }

        //url: '@Url.Action("BillsDashBoardCountSum", "Tplus")',
        public ActionResult BillsDashBoardCountSum(string billDt)
        {

            modBillsDetails bill = new modBillsDetails();
            DashBoardClientCountModel dc = bill.BillsDashBoardCountSum(billDt);
            return View(dc);
        }

        //url: '@Url.Action("RMSDashBoardCountSum", "Tplus")',
        public ActionResult RMSDashBoardCountSum()
        {
            modRiskManagementSystem rms = new modRiskManagementSystem();
            DashBoardClientCountModel dc = rms.RMSDashBoardCountSum();
            ModelState.Clear();
            return View(dc);
        }

        //DASHBOARD CHARTS
        //START
        //RMS
        public ActionResult GetRMSChart()
        {

            modRiskManagementSystem rms = new modRiskManagementSystem();
            var freeze = rms.GetDashBoardRMSChart();
            return Json(freeze, JsonRequestBehavior.AllowGet);
        }

        //BROKERAGE ANALYSIS
        public ActionResult GetBrokerageChart()
        {
            modBillsDetails bd = new modBillsDetails();
            var freeze = bd.GetDashBoardBrokerageChart();
            return Json(freeze, JsonRequestBehavior.AllowGet);
        }

        //RECEIVABLE/ PAYABLE
        public ActionResult GetRECPAYChart(string strtype)
        {
            modReceiptPayment rec = new modReceiptPayment()
            {
                strFlag = strtype
            };
            var result = rec.GetDashBoardRecPayChart();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //END

        //url: '@Url.Action("CodeNameAmount", "Tplus")',
        public ActionResult CodeNameAmount(string xbutton, string xval)
        {
            modReceiptPayment rec = new modReceiptPayment();
            IEnumerable<CodeNameAmountModel> recpay = rec.GetCodeNameAmount(xbutton, xval);
            ModelState.Clear();
            return View(recpay);
        }

        //url: '@Url.Action("GetRMSViewDashBoard", "Tplus")',
        public ActionResult GetRMSViewDashBoard(string type)
        {
            modRiskManagementSystem risk = new modRiskManagementSystem();
            IEnumerable<CodeNameAmountModel> rms = risk.RMSDashBoardCountSumInner(type == "A");
            ModelState.Clear();
            return View(rms);
        }

        //url: '@Url.Action("GetBillsViewDashBoard", "Tplus")',
        public ActionResult GetBillsViewDashBoard(string type, string billdt)
        {
            UtilityModel myutil = new UtilityModel();
            string dbDate = myutil.dtos(billdt);
            modBillsDetails bill = new modBillsDetails();
            IEnumerable<BillsViewDashBoardModel> ulist = bill.GetBillsViewDashBoard(type, dbDate);
            ModelState.Clear();
            return View(ulist);
        }

        //BILLS url: '@Url.Action("GetBillStlmnt", "Tplus")',
        public ActionResult GetBillStlmnt(string Code, string billdate)
        {
            UtilityModel myutil = new UtilityModel();
            string dbDate = myutil.dtos(billdate);
            modBillsDetails bill = new modBillsDetails();
            IEnumerable<BillSettlementModel> ulist = bill.GetBillSettlement(Code, dbDate);
            ModelState.Clear();
            return View(ulist);
        }

        //url: '@Url.Action("GetBeneficiaryHolding", "Tplus")',
        public ActionResult GetBeneficiaryHolding(string Code)
        {
            string NewLn = Environment.NewLine;
            modHolding hold = new modHolding();
            IEnumerable<DPHoldingModel> dp = hold.GetBenHolding(Code);
            ModelState.Clear();
            return View(dp);
        }

        //url: '@Url.Action("GetClientLedgerSummary", "Tplus")',
        public ActionResult GetClientLedgerSummary(string Code, string curyear)
        {
            string NewLn = Environment.NewLine;
            modLedger led = new modLedger();
            List<LedgerViewModel> dv = led.GetClientLedgerSummary(Code, curyear);
            ModelState.Clear();
            return View(dv);
        }

        //url: '@Url.Action("GetMarginShortFall", "Tplus")',
        public ActionResult GetMarginShortFall(string Code)
        {
            modMargin mar = new modMargin();
            IEnumerable<CombinedMarginModel> dp = mar.GetMarginShorFallByCode(Code);
            ModelState.Clear();
            return View(dp);
        }

        //url: '@Url.Action("GetRetainHoldingSummary", "Tplus")',
        public ActionResult GetRetainHoldingSummary(string Code)
        {
            modDemat dmt = new modDemat();
            IEnumerable<RetainHoldingSummaryModel> ret = dmt.GetRetainHoldingSummary(Code);
            ModelState.Clear();
            return View(ret);
        }

        //url: '@Url.Action("GetShareCollateralSummary", "Tplus")',
        public ActionResult GetShareCollateralSummary(string Code)
        {
            modCollateral col = new modCollateral();
            IEnumerable<ShareCollateralModel> ret = col.GetShareCollateralSummary(Code);
            ModelState.Clear();
            return View(ret);
        }

        //url: '@Url.Action("GetClientLedger", "Tplus")',
        public ActionResult GetClientLedger(string Select, string Code, string curyear, string strDPID = "", string strDPIDMargin = "", string Viewtype = "", string FromDate = "", string Segment = "")
        {
            ViewBag.LgCode = Code;
            if (Viewtype == "")
            {
                modLedger led = new modLedger();
                List<LedgerModel> ulist = led.GetClientLedger(Select, Code, curyear, strDPID, strDPIDMargin);
                ModelState.Clear();
                return View(ulist);
            }
            else if (Viewtype == "BILL")


            {
                modLedger led = new modLedger();
                IEnumerable<LedgerModel> ulist = led.GetLedgerFromBill(FromDate, Code, Segment);
                ModelState.Clear();
                return View(ulist);
            }
            else
            {
                return Content("");
            }
        }



        //RMS
        #region old rmssummary code
        //public ActionResult RMSSummary()
        //{
        //    //if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
        //    //{
        //    //    return RedirectToAction("LogoutPage", "Tplus");
        //    //}

        //    //DataTable dtRMSHEAD = new DataTable();
        //    //dtRMSHEAD.Clear();
        //    //dtRMSHEAD.Columns.Add("RMSHEAD");           

        //    //LibraryModel objRMSlib = new LibraryModel();
        //    //DataTable dt = null;
        //    //DataTable dt2 = null;

        //    //dt = objRMSlib.OpenDataTable("SELECT * FROM Sysparameter where sp_sysvalue like '%rs_NFiller2%'");

        //    //if (dt.Rows.Count > 0)
        //    //{
        //    //    for (int iR = 0; iR < dt.Columns.Count; iR++)
        //    //    {
        //    //        object[] o = { objRMSlib.GetScalarValueString("select sp_sysvalue from sysparameter where sp_parmcd='" + dt.Rows[iR]["sp_parmcd"].ToString() + "'") };
        //    //        dtRMSHEAD.Rows.Add(o);
        //    //    }

        //    //    Session["RMSFORMULA"] = dtRMSHEAD;
        //    //}


        //    ViewBag.chkTp = chkTp;
        //    ViewBag.chkComm = chkComm;
        //    ViewBag.chkNBFC = chkNBFC;
        //    DataTable Results;
        //    if (blnFetch)
        //    {
        //        Session["RiskManagement"] = null;

        //    }
        //    if (Session["RiskManagement"] != null)
        //    {
        //        Results = (DataTable)Session["RiskManagement"];
        //        if (FilterRMS1.Trim() != "")
        //        {
        //            string filter1 = FilterRMS1.Split('~')[0];
        //            string sign1 = FilterRMS1.Split('~')[1];
        //            Double amount = Conversion.Val(FilterRMS1.Split('~')[2]);

        //            DataView dv = new DataView(Results);
        //            dv.RowFilter = filter1 + sign1 + amount; // query example = "id = 10"
        //            Results = dv.ToTable();
        //        }
        //        //return View(Results);
        //        //return Request.CreateResponse(DataSourceLoader.Load());
        //    }
        //    else
        //    {
        //        if (blnRefresh && Session["RiskManagement"] != null)
        //        {
        //            Results = (DataTable)Session["RiskManagement"];
        //        }
        //        string type = "";
        //        string code = "";
        //        if (rmsvalue.Trim() != "")
        //        {
        //            type = rmsvalue.Split('~')[0];
        //            code = rmsvalue.Split('~')[1];
        //        }

        //        modRiskManagementSystem rms = new modRiskManagementSystem()
        //        {
        //            type = type,
        //            Code = code
        //        };

        //        Results = rms.GetRMSReport(chkTp, chkComm, chkNBFC);
        //        Session["RiskManagement"] = Results;
        //    }

        //    var json12 = JsonConvert.SerializeObject(Results, Formatting.Indented);

        //    List<RiskManagmentResponse> riskManagments = new List<RiskManagmentResponse>();

        //    riskManagments = Results.AsEnumerable()
        //        .Select(row => new RiskManagmentResponse
        //        {
        //            Code = row.Field<string>("Code"),
        //            Name = row.Field<string>("Name"),
        //            TDay = row.Field<decimal>("TDay"),
        //            T2Day = row.Field<decimal>("T2Day"),
        //            //UnCleared = row.Field<decimal>("UnCleared"),
        //            //CashDeposit = row.Field<decimal>("CashDeposit"),
        //            //ShareCollateral = row.Field<decimal>("ShareCollateral"),
        //            //Margin = row.Field<decimal>("Margin"),
        //            //Pool = row.Field<decimal>("Pool"),
        //            //DPHolding = row.Field<decimal>("DPHolding"),
        //            //Stoke = row.Field<decimal>("Stoke"),
        //            //Net = row.Field<decimal>("Net"),
        //            //AboveDays = row.Field<decimal>("AboveDays"),
        //            //Collection = row.Field<decimal>("Collection"),
        //            //ActualRisk = row.Field<decimal>("ActualRisk"),
        //            //FundPayout = row.Field<decimal>("FUNDPAYOUT"),
        //            //ProjectedRisk = row.Field<decimal>("PROJECTEDRISK"),
        //            //SharePayout = row.Field<decimal>("SHAREPAYOUT"),
        //            //FoMargin = row.Field<decimal>("fomargin"),
        //            //StokeBH = row.Field<decimal>("StokeBH"),
        //            //ProvInt = row.Field<decimal>("PtovInt"),
        //            //AvailMargin = row.Field<decimal>("AvailMargin"),
        //            //OptionM2M = row.Field<decimal>("OptionM2M"),
        //            //DebitOlder5Days = row.Field<decimal>("DebitOlder5Days"),
        //            //RmsLimit = row.Field<decimal>("RMSLIMIT"),
        //            //StokeAH = row.Field<decimal>("StokeAH"),
        //            //ExchApprStk = row.Field<decimal>("ExchApprStk"),
        //        }).ToList();

        //    //return Json(DataSourceLoader.Load(riskManagments, optionsBase), JsonRequestBehavior.AllowGet);
        //    return View(Results);
        //}
        #endregion

        [HttpGet]
        public ActionResult LedgerReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LedgerReportView(string Select, string Code1, string Code, string FromDate, string ToDate, string strDPID = "", string strLedgerListid = "", string strchkConfirmation = "", string strcmbLedgerList = "", string strcmbActType = "", string chkMrgnlegr = "")
        {
            LedgerReport3 ledgerReport = new LedgerReport3();

            //string Select = "CL";
            //string Code1 = "";
            //string Code = "";
            //string FromDate = "20210401";
            //string ToDate = "20220331";
            //string strDPID = "BC,BF,BM,MC,MF,MK,NC,NF,NK";
            //string strLedgerListid = "Receipts,Payments,Journals,Debit Notes,Credit Notes,Bills,Margins,Expenses,Opening Bal";
            //string strchkConfirmation = "0";
            //string strcmbLedgerList = "0";
            //string strcmbActType = "0";
            //string chkMrgnlegr = "1";

            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            ViewBag.clientcondition = "Notview";
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.strchkConfirmation = strchkConfirmation;
            if (Code != "")
            {

                ViewBag.clientcondition = "view";
                ViewBag.strCompOff = mylib.GetSysParmSt("LGR_COMPOFF", "Stationary");
            }

            //FromDate = myutil.dtos(FromDate);
            //ToDate = myutil.dtos(ToDate);
            modLedger led = new modLedger();
            IEnumerable<LedgerModel> ulist = led.GetLedger(Select, Code1, Code, FromDate, ToDate, strDPID, strLedgerListid, strchkConfirmation, strcmbLedgerList, strcmbActType, chkMrgnlegr);


            //var amodel = ulist.Take<LedgerModel>(100);

            ledgerReport.DataSource = ulist;
            return View(ledgerReport);
        }

        public ActionResult GetDetailsTest()
        {
            XtraReport1 Report1 = new XtraReport1();

            string Select = "CL";
            string Code1 = "";
            string Code = "";
            string FromDate = "20210401";
            string ToDate = "20220331";
            string strDPID = "BC,BF,BM,MC,MF,MK,NC,NF,NK";
            string strLedgerListid = "Receipts,Payments,Journals,Debit Notes,Credit Notes,Bills,Margins,Expenses,Opening Bal";
            string strchkConfirmation = "0";
            string strcmbLedgerList = "0";
            string strcmbActType = "0";
            string chkMrgnlegr = "1";

            modLedger led = new modLedger();
            IEnumerable<LedgerModel> ulist = led.GetLedger(Select, Code1, Code, FromDate, ToDate, strDPID, strLedgerListid, strchkConfirmation, strcmbLedgerList, strcmbActType, chkMrgnlegr);

            Report1.DataSource = ulist;
            return View(Report1);
        }

        [HttpGet]
        public ActionResult GetLedgerReport(string Select, string Code1, string Code, string FromDate, string ToDate, string strDPID = "", string strLedgerListid = "", string strchkConfirmation = "", string strcmbLedgerList = "", string strcmbActType = "", string chkMrgnlegr = "")
        {
            LedgerReport4 ledgerReport = new LedgerReport4();
            UtilityDBModel utilityDB = new UtilityDBModel();
            //string Select = "CL";
            //string Code1 = "";
            //string Code = "";
            //string FromDate = "20210401";
            //string ToDate = "20220331";
            //string strDPID = "BC,BF,BM,MC,MF,MK,NC,NF,NK";
            //string strLedgerListid = "Receipts,Payments,Journals,Debit Notes,Credit Notes,Bills,Margins,Expenses,Opening Bal";
            //string strchkConfirmation = "0";
            //string strcmbLedgerList = "0";
            //string strcmbActType = "0";
            //string chkMrgnlegr = "1";

            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            ViewBag.clientcondition = "Notview";
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.strchkConfirmation = strchkConfirmation;
            if (Code != "")
            {

                ViewBag.clientcondition = "view";
                ViewBag.strCompOff = mylib.GetSysParmSt("LGR_COMPOFF", "Stationary");
            }

            //FromDate = myutil.dtos(FromDate);
            //ToDate = myutil.dtos(ToDate);
            modLedger led = new modLedger();
            IEnumerable<LedgerModel> ulist = led.GetLedger(Select, Code1, Code, FromDate, ToDate, strDPID, strLedgerListid, strchkConfirmation, strcmbLedgerList, strcmbActType, chkMrgnlegr);
            List<LedgerReportModel> reportModel = new List<LedgerReportModel>()
            {
                new LedgerReportModel
                {
                CompanyLogo = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + utilityDB.getlogoimageURL().Replace("~/", "/"),
                //CompanyLogo = "http://localhost:49411/CompanyLogo.BMP",
                LedgerBalanceDate = "Ledger Balance From " + myutil.stod(FromDate).ToString("dd/MM/yyyy").Replace("-", "/") + " To " + myutil.stod(ToDate).ToString("dd/MM/yyyy").Replace("-", "/"),
                LedgerModels = ulist
                }
            };
            //var amodel = ulist.Take<LedgerModel>(100);

            ledgerReport.DataSource = reportModel.AsEnumerable();

            //ledgerReport.RollPaper = true;
            return View(ledgerReport);
        }


        [HttpGet]
        public ActionResult RMSSummary(string rmsvalue, string chkTp = "", string chkComm = "", string chkNBFC = "", string FilterRMS1 = "", bool blnRefresh = false, bool blnFetch = false)
        {
            UtilityDBModel utilityDB = new UtilityDBModel();
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }

            ViewBag.chkTp = chkTp;
            ViewBag.chkComm = chkComm;
            ViewBag.chkNBFC = chkNBFC;
            DataTable Results;
            if (blnFetch)
            {
                Session["RiskManagement"] = null;

            }
            if (Session["RiskManagement"] != null)
            {
                Results = (DataTable)Session["RiskManagement"];
                if (FilterRMS1.Trim() != "")
                {
                    string filter1 = FilterRMS1.Split('~')[0];
                    string sign1 = FilterRMS1.Split('~')[1];
                    Double amount = Conversion.Val(FilterRMS1.Split('~')[2]);

                    DataView dv = new DataView(Results);
                    dv.RowFilter = filter1 + sign1 + amount; // query example = "id = 10"
                    Results = dv.ToTable();
                }
                //return View(Results);
                //return Request.CreateResponse(DataSourceLoader.Load());
            }
            else
            {
                if (blnRefresh && Session["RiskManagement"] != null)
                {
                    Results = (DataTable)Session["RiskManagement"];
                    DataRow[] dtRows = Results.Select();
                    if (dtRows.Length != 0)
                    {
                        return View(new DevExreamData { Data = Results, ColumnStyles = RiskData(), IsZeroVisible = false, IsThousandSep = false, CompanyLogo = utilityDB.getlogoimageURL().Replace("~/", "../"), Header = "Tradeplus Technologies Limited", SubHeader1 = "2022 - 23", SubHeader2 = "Risk summary" });
                    }
                    else
                    {
                        return RedirectToAction("RiskManagementView");
                        TempData["Nodata"] = "No data was found.";
                    }
                }
                string type = "";
                string code = "";
                if (rmsvalue.Trim() != "")
                {
                    type = rmsvalue.Split('~')[0];
                    code = rmsvalue.Split('~')[1];
                }

                modRiskManagementSystem rms = new modRiskManagementSystem()
                {
                    type = type,
                    Code = code
                };

                Results = rms.GetRMSReport(chkTp, chkComm, chkNBFC);
                Session["RiskManagement"] = Results;

            }
            DataRow[] dataRows = Results.Select();
            if (dataRows.Length != 0)
            {
                return View(new DevExreamData { Data = Results, ColumnStyles = RiskData(), IsZeroVisible = false, IsThousandSep = false, CompanyLogo = utilityDB.getlogoimageURL().Replace("~/", "../"), Header = "Tradeplus Technologies Limited", SubHeader1 = "2022 - 23", SubHeader2 = "Risk summary" });
            }
            else
            {
                //TempData["Nodata"] = "No data was found.";
                return null;
            }

        }

        public ActionResult RiskManagementReport(string rmsvalue, string chkTp, string chkComm, string chkNBFC)
        {
            string type = "";
            string code = "";
            if (rmsvalue.Trim() != "")
            {
                type = rmsvalue.Split('~')[0];
                code = rmsvalue.Split('~')[1];
            }


            modRiskManagementSystem rms = new modRiskManagementSystem()
            {
                type = type,
                Code = code
            };

            DataTable Results = rms.GetRMSReport(chkTp, chkComm, chkNBFC);

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;


            foreach (DataRow dr in Results.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in Results.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            //var res = serializer.Serialize(rows);
            var jsonData = new
            {
                rows = rows
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        //SHARE PAYOUT
        public ActionResult SharePayoutRequestReport(string type, string select, string Code)
        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            modSharePayoutProcess procsp = new modSharePayoutProcess();
            IEnumerable<SharePayoutModel> spayout = procsp.ProcessSharePayputRequest(type, select, Code);
            return View(spayout);
        }

        //SHARE PAYOUT
        public ActionResult SharePayoutRequestReportEdit(string Code, string name, decimal ledbal, decimal holding, decimal rmsamount)
        {
            modSharePayoutProcess procsp = new modSharePayoutProcess();
            IEnumerable<SharePayoutEditModel> spayout = procsp.ProcessSharePayputRequestEdit(Code, name, ledbal, holding, rmsamount);
            return View(spayout);
        }

        //SHARE PAYOUT
        public ActionResult GetSettlementQty(string Code, string scrip)
        {
            modSharePayoutProcess payout = new modSharePayoutProcess();
            IEnumerable<SettlementQtyModel> qty = payout.GetSettlementQty(Code, scrip);
            return View(qty);
        }

        //SHARE PAYOUT
        public ActionResult SaveSharePayoutRequest(IEnumerable<SharePayoutEditModel> share, string client)
        {
            modSharePayoutProcess sp = new modSharePayoutProcess();
            sp.SaveSharePayout(share, client);
            return Content("Record Saved");
        }


        //BULK PAYOUT
        public ActionResult BulkPayoutReport(string DPID, string strDt, string format, string type, string code)
        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            UtilityModel myutil = new UtilityModel();
            string dt = myutil.dtos(strDt);

            modBulkPayoutProcess proc1 = new modBulkPayoutProcess()
            {
                strDate = dt,
                strDPID = DPID,
                format = format,
                type = type,
                Code = code
            };
            var freeze = proc1.GetBulkPayout();
            return Json(freeze, JsonRequestBehavior.AllowGet);
            //IEnumerable<BulkPayout> bp = proc1.GetBulkPayout();
            //return View(bp);
        }
        //BULK PAYOUT
        public JsonResult BulkPayoutSave(List<BulkPayoutModel> ulist, string strDPID, string strDate)
        {
            UtilityModel myutil = new UtilityModel();
            string dt = myutil.dtos(strDate);
            modBulkPayoutProcess bpp = new modBulkPayoutProcess();
            bpp.SaveDataBulkPayout(ulist, strDPID, dt);

            return Json("");
        }

        [HttpPost]
        public ActionResult SaveReceiptPaymentImage()
        {
            byte[] strImage = null;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["HelpSectionImages"];
                BinaryReader reader = new BinaryReader(pic.InputStream);
                strImage = reader.ReadBytes(pic.ContentLength);
            }

            ReceiptTableModel rec = new ReceiptTableModel()
            {
                mode = System.Web.HttpContext.Current.Request["MODE"],
                rc_srno = (System.Web.HttpContext.Current.Request["SRNO"] == "" ? 0 : Convert.ToDouble(System.Web.HttpContext.Current.Request["SRNO"])),
                rc_dpid = System.Web.HttpContext.Current.Request["DPID"],
                rc_bankclientcd = System.Web.HttpContext.Current.Request["BANK"],
                rc_voucherno = System.Web.HttpContext.Current.Request["VOUCHER"],
                rc_receiptdt = System.Web.HttpContext.Current.Request["DATE"],
                rc_clientcd = System.Web.HttpContext.Current.Request["CODE"],
                rc_particular = System.Web.HttpContext.Current.Request["PARTICULAR"],
                rc_batchno = double.Parse(System.Web.HttpContext.Current.Request["RECEIVED"].ToString()),
                rc_chequeno = System.Web.HttpContext.Current.Request["CHEQUENO"],
                rc_micr = System.Web.HttpContext.Current.Request["MICR"],
                rc_BankActNo = System.Web.HttpContext.Current.Request["ACNO"],
                rc_amount = decimal.Parse(System.Web.HttpContext.Current.Request["AMOUNT"].ToString()),
                rc_debitflag = System.Web.HttpContext.Current.Request["DEBITFLAG"],
                rc_cleareddt = System.Web.HttpContext.Current.Request["CLEARDT"],
                rc_entryno = 1,
                mkrid = "",
                mkrdt = "",
                rc_accyear = "",
                rc_authid1 = "",
                rc_authid2 = "",
                rc_authdt1 = "",
                rc_authdt2 = "",
                rc_status = "",
                rc_authremarks = "",
                rc_commondt = "",
                rc_common = "",
                mkrtm = "",
                rc_authtm1 = "",
                rc_authtm2 = "",
                rc_costcenter = "",
                ReceiptImage = strImage
            };
            modReceiptPayment recsave = new modReceiptPayment();
            string val = recsave.SaveReceiptPayment(rec);
            ViewBag.Message = val;

            return Content(val);
        }

        public JsonResult FillComboExchangeSegment(string DPID)
        {
            modReceiptPayment rec = new modReceiptPayment();
            var result = rec.FillExchangeSegment();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReceipt()
        {
            ReceiptTableModel rec = new ReceiptTableModel()
            {
                mode = System.Web.HttpContext.Current.Request["MODE"],
                rc_srno = (System.Web.HttpContext.Current.Request["SRNO"] == "" ? 0 : Convert.ToDouble(System.Web.HttpContext.Current.Request["SRNO"])),
                rc_dpid = System.Web.HttpContext.Current.Request["DPID"],
                rc_bankclientcd = System.Web.HttpContext.Current.Request["BANK"],
                rc_voucherno = System.Web.HttpContext.Current.Request["VOUCHER"],
                rc_receiptdt = System.Web.HttpContext.Current.Request["DATE"],
                rc_clientcd = System.Web.HttpContext.Current.Request["CODE"],
                rc_particular = System.Web.HttpContext.Current.Request["PARTICULAR"],
                rc_batchno = double.Parse(System.Web.HttpContext.Current.Request["RECEIVED"].ToString()),
                rc_chequeno = System.Web.HttpContext.Current.Request["CHEQUENO"],
                rc_micr = System.Web.HttpContext.Current.Request["MICR"],
                rc_BankActNo = System.Web.HttpContext.Current.Request["ACNO"],
                rc_amount = decimal.Parse(System.Web.HttpContext.Current.Request["AMOUNT"].ToString()),
                rc_debitflag = System.Web.HttpContext.Current.Request["DEBITFLAG"],
                rc_cleareddt = System.Web.HttpContext.Current.Request["CLEARDT"],
                rc_entryno = 1,
                mkrid = "",
                mkrdt = "",
                rc_accyear = "",
                rc_authid1 = "",
                rc_authid2 = "",
                rc_authdt1 = "",
                rc_authdt2 = "",
                rc_status = "",
                rc_authremarks = "",
                rc_commondt = "",
                rc_common = "",
                mkrtm = "",
                rc_authtm1 = "",
                rc_authtm2 = "",
                rc_costcenter = ""


            };
            modReceiptPayment recsave = new modReceiptPayment();
            string val = recsave.DeleteReceipt(rec);

            //string val ="":
            return Content(val);

        }

        public JsonResult FillBankFromDB(string DPID)
        {
            string strSql = "";
            string IsTplusCommex;
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            if ((string)Session["IsTplusCommex"] == "Y")
            {
                IsTplusCommex = "Y";
            }
            else
            {
                IsTplusCommex = "N";
            }
            modReceiptPayment rec = new modReceiptPayment();
            if ((Strings.Mid(DPID, 3, 1) ?? "") == "X")
            {
                if (IsTplusCommex == "Y")
                {
                    //strDpId = strDpId.Substring(1);

                    var result = rec.FillBankDetails(DPID);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    SqlConnection ObjCommexCon = null;
                    ObjCommexCon = mydbutil.commexTemp_conn("Commex");

                    if (ObjCommexCon.Database != "" & ObjCommexCon.DataSource != "")
                    {
                        if (ObjCommexCon.State == ConnectionState.Closed)
                        {
                            ObjCommexCon.Open();

                        }
                    }
                    // strDpId = strDpId.Substring(1);

                    strSql = myutil.getBankSqlCommex(DPID);
                    DataTable dt = myLib.OpenDataTable(strSql, ObjCommexCon);


                    List<JsonComboModel> ulist = new List<JsonComboModel>();

                    ulist = dt.AsEnumerable()
                    .Select(row => new JsonComboModel
                    {
                        Display = row.Field<string>("cm_name"),
                        Value = row.Field<string>("cm_cd"),
                    }).ToList();

                    var result = ulist;
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {

                var result = rec.FillBankDetails(DPID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult FillMICRFromDB(string Code)
        {
            modReceiptPayment rec = new modReceiptPayment();
            var result = rec.FillMICR(Code);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillBankAcnoFromDB(string Code, string Micr)
        {
            modReceiptPayment rec = new modReceiptPayment();

            var result = rec.FillBankAcNo(Code, Micr);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillBankNameFromDB(string Micr)
        {
            modReceiptPayment rec = new modReceiptPayment();
            var result = rec.FillBankName(Micr);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ViewReceiptPayment(int Srno, string DPID, string Date)
        {
            modReceiptPayment rec = new modReceiptPayment();
            var result = rec.FetchReceiptPayment(Srno, DPID, Date);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BillSummaryViewReport(string type, string Code, string groupby, string fromdate, string strDpid)

        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            modBillSummary bs = new modBillSummary();
            IEnumerable<BillSummaryModel> ulist = bs.GetBillSummary(type, Code, groupby, fromdate, strDpid);
            return View(ulist);
        }
        public ActionResult GetTradesDashBoard(string strType, string strDate)
        {
            modTrades rec = new modTrades();
            if (strType == "1")
            {
                var result = rec.GetTradesDashBoard(strDate);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (strType == "2")
            {
                var result1 = rec.GetTradesDashBoardDelSpec(strDate);
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("");
            }
        }
        [HandleError]
        public ActionResult GetBillPrint(string code, string strstlmnt, string strOrderBy, string FromDate, string strSelect)
        {
            modTrades trd = new modTrades();
            ViewBag.FromDate = FromDate;
            IEnumerable<BillPrintModel> ulist = trd.GetCashBill(strstlmnt, strOrderBy, code, strSelect, FromDate);

            return View(ulist);
        }
        public ActionResult GetBillPrintOld(string code, string strstlmnt, string strOrderBy, string FromDate, string strSelect)
        {
            modTrades trd = new modTrades();
            IEnumerable<BillPrintModel> ulist = trd.GetBillPrint(strstlmnt, strOrderBy, code, strSelect, FromDate);
            return View(ulist);
        }
        public ActionResult GetFOBillPrint(string Code, string strOrderBy, string strDate, string strexchange, string strSegment)
        {
            modTrades trd = new modTrades();
            UtilityModel utl = new UtilityModel();
            ViewBag.strDate = utl.DbToDate(strDate);
            IEnumerable<FOBillprintModel> ulist = trd.GetFOBillPrint(Code, strOrderBy, strDate, strexchange, strSegment);
            return View(ulist);
        }
        public ActionResult GetFOVbnetBillPrint(string Code, string strOrderBy, string strDate, string strexchange, string strSegment)
        {
            modTrades trd = new modTrades();
            UtilityModel utl = new UtilityModel();
            IEnumerable<FOBillprintModel> ulist = trd.GetFOVbnetBillPrint(Code, strOrderBy, strDate, strexchange, strSegment);
            return View(ulist);
        }

        public ActionResult GetMfBillPrint(string Code, string strOrderBy, string strDate, string strexchange, string strSegment)
        {
            modTrades trd = new modTrades();
            DataTable ulist = trd.GetMfBillPrint(Code, strOrderBy, strDate, strexchange, strSegment);
            return View(ulist);
        }
        public ActionResult FOutstandingPositionReport(string strDPID, string strClient, string FDate, string cmbSelect, string Select, string Margin, string PL)
        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }

            ViewBag.Condition = cmbSelect;
            modReports led = new modReports();
            DataTable dt = led.GetOutStandingPosition(strDPID, strClient, FDate, cmbSelect, Select, Margin, PL);
            return View(dt);
        }

        public ActionResult MTFRMSummaryReport(string strClient, string FDate, string cmbSelect, string strExchange)
        {
            modReports led = new modReports();
            UtilityModel util = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            DataTable dt;
            //if (Convert.ToInt64(util.FnCheckDt()) < Convert.ToInt64(mylib.OpenDataTable("select convert(char(8),getdate(),112)").Rows[0][0]))
            //{
            //    dt = led.GetMTFRMSummaryReport(strClient, FDate, cmbSelect);
            //}
            //else
            //{
            // dt = led.GetMTFRMSummaryReport1(strClient, FDate, cmbSelect);
            dt = led.GetMTFRMSummaryReport2(strClient, FDate, cmbSelect, strExchange);
            //}

            return View(dt);
        }

        // public ActionResult MTFRMSummaryReport1(string strClient, string FDate, string cmbSelect)
        //{
        //     modReports led = new modReports();
        //     DataTable dt = led.GetMTFRMSummaryReport1(strClient, FDate, cmbSelect);
        //     return View(dt);
        // }
        public ActionResult GetMTFRMSummaryReport(string strClient, string stdate)
        {
            modReports trd = new modReports();
            IEnumerable<MasterMTPRMS> ulist = trd.GetMTFRMSummaryReport(strClient, stdate);
            return View(ulist);
        }
        public ActionResult MTFSecurityListReport(string Fdate)

        {
            ViewBag.ConNSE = "";
            ViewBag.ConBSE = "";
            modReports led = new modReports();
            DataTable dt = led.GetMTFSecurityListReport1(Fdate);
            return View(dt);
        }
        public ActionResult MTFTradeListingReport(string Select, string Code, string FDate, string ToDate, string ClientType)
        {
            modReports led = new modReports();
            DataTable dt = led.GetMTFTradeListingReport(Select, Code, FDate, ToDate, ClientType);
            return View(dt);
        }
        public ActionResult GetTradeListpopup(string code, string strExchSeg, string strStlMnt, string strScrCd, string strDate, string ClientName, string strScrname, string FDate, string TDate)
        {

            ViewBag.MSGDate = "Client Cumulative For " + FDate + " to " + TDate;
            ViewBag.Code = code;
            ViewBag.strScrCd = strScrCd;
            ViewBag.ClientName = ClientName;
            ViewBag.strScrname = strScrname;
            ViewBag.strStlMnt = strStlMnt;
            modTrades cp = new modTrades();
            DataTable dt = cp.GetTradeList(code, strExchSeg, strStlMnt, strScrCd, strDate, ClientName, strScrname);
            return View(dt);
        }

        public ActionResult GetMTFClientWiseReport(string strClient, string FDate, string cmbSelectReport, string cmbSelectGroupBy, string cmbSelect)
        {
            ViewBag.Report = cmbSelectReport;
            ViewBag.GroupBy = cmbSelectGroupBy;
            modReports led = new modReports();
            DataTable dt = led.GetMTFClientWiseReport(strClient, FDate, cmbSelectReport, cmbSelectGroupBy, cmbSelect);

            return View(dt);
        }

        public ActionResult MTFContinuousShortfallReport(string strClient, string FDate, string cmbSelect)
        {

            modReports led = new modReports();
            DataTable dt = led.GetMTFContinuousShortReport(strClient, FDate, cmbSelect);
            return View(dt);
        }
        public ActionResult GetContinuousShortfall(string strClient, string stdate)
        {
            modReports trd = new modReports();
            //ViewBag.message = "Shortfall Since Last " + ObjUtility.fnGetSysParam("MTFDEBITDAYS")) + " Days ";
            IEnumerable<DataTable> ulist = trd.GetContinuousShortfall(strClient, stdate);
            return View(ulist);
        }

        public ActionResult RMSHawkEYE()
        {
            modRiskManagementSystem rms = new modRiskManagementSystem();
            IEnumerable<RMSHawkEYEModel> ulist = rms.GetRMSHawkEYE();
            return View(ulist);
        }
        public ActionResult RMSHawkEYEByCode(string Code)
        {
            modRiskManagementSystem rms = new modRiskManagementSystem();
            IEnumerable<RMSClientPositionModel> ulist = rms.GetRMSHawkEYEByCode(Code);
            return View(ulist);
        }

        public ActionResult TradeListingReport(string Code, string ExchSeg, string FDate, string TDate, string Report, string GroupBy, string security, string SearchBy)
        {
            string ExchSegTplus = "";
            string ExchSegComm = "";
            if ((string)Session["IsTplusCommex"] == "Y")
            {
                string[] arrExch;
                arrExch = ExchSeg.Split(',');
                for (int i = 0; i < arrExch.Length; i++)
                {
                    if (Strings.Right(arrExch[i], 1) == "X")
                    { ExchSegComm += (ExchSegComm != "" ? ",'" + Strings.Right(arrExch[i], 2) + "'" : "'" + Strings.Right(arrExch[i], 2) + "'"); }
                    else
                    { ExchSegTplus += (ExchSegTplus != "" ? ",'" + Strings.Right(arrExch[i], 2) + "'" : "'" + Strings.Right(arrExch[i], 2) + "'"); }
                }
                ExchSegComm = (ExchSegComm != "" ? "(" + ExchSegComm + ")" : "");
                ExchSegTplus = (ExchSegTplus != "" ? "(" + ExchSegTplus + ")" : "");
            }
            else
            {
                string[] strArray = ExchSeg.Split(',');
                bool first = true;
                foreach (string obj in strArray)
                {
                    if (obj.Length >= 2 && obj != "FX")
                    {
                        if (first)
                        {
                            ExchSegTplus = obj;
                            first = false;
                        }
                        else
                        {
                            ExchSegTplus = obj + "," + ExchSegTplus;
                        }
                    }

                    //your insert query
                }
                bool Cofirst = true;
                foreach (string obj in strArray)
                {
                    if (obj == "FX" || obj.Length < 2)
                    {
                        if (Cofirst)
                        {
                            ExchSegComm = obj;
                            Cofirst = false;
                        }
                        else
                        {
                            ExchSegComm = obj + "," + ExchSegComm;
                        }
                    }

                    //your insert query
                }
                ExchSegComm = "('" + ExchSegComm.Replace(",", "','") + "')";
                ExchSegTplus = "('" + ExchSegTplus.Replace(",", "','") + "')";
            }

            ViewBag.Report = Report;
            ViewBag.DateFilter = "[" + FDate + " - " + TDate + "]";
            modTrades cp = new modTrades();
            IEnumerable<ClientPositionModel> dp = cp.GetClientPositionReport(Code, ExchSegTplus, ExchSegComm, FDate, TDate, Report, GroupBy, security, SearchBy);
            ModelState.Clear();
            return View(dp);
        }

        public ActionResult FillComboCES(string criteria, string SearchBy)
        {

            modEntityMaster cm = new modEntityMaster();
            ModelState.Clear();
            IEnumerable<JsonComboModel> ulist = cm.GetCompanyExchSeg(criteria);
            return View(ulist);
        }

        public ActionResult FillComboCESSingle(string criteria, string SearchBy)
        {
            modEntityMaster cm = new modEntityMaster();
            ModelState.Clear();
            IEnumerable<JsonComboModel> ulist = cm.GetCompanyExchSeg(criteria);
            return View(ulist);
        }
        public ActionResult FillComboCESBankRecipt()
        {
            modEntityMaster cm = new modEntityMaster();
            ModelState.Clear();
            IEnumerable<JsonComboModel> ulist = cm.GetCompanyExchSegForBankRecipt();
            return View(ulist);
        }
        //RMS

        public ActionResult StockRMS(string CId)
        {
            string NewLn = Environment.NewLine;
            modRiskManagementSystem dp = new modRiskManagementSystem();
            IEnumerable<StockRMSModel> dstrock = dp.GetStockLink(CId);
            ModelState.Clear();
            return View(dstrock);
        }
        //RMS
        public ActionResult ApprovedShareRMS(string CId)
        {
            string NewLn = Environment.NewLine;
            modRiskManagementSystem dp = new modRiskManagementSystem();
            IEnumerable<ApprovedShareRMSModel> dsapprove = dp.GetApprovedShareRMS(CId);
            ModelState.Clear();
            return View(dsapprove);
        }
        public ActionResult NEWCOLLRMS(string CId)
        {
            string NewLn = Environment.NewLine;
            modRiskManagementSystem dp = new modRiskManagementSystem();
            IEnumerable<ApprovedShareRMSModel> dsapprove = dp.GetNEWCOLLRMS(CId);
            ModelState.Clear();
            return View(dsapprove);
        }

        public ActionResult DpHolding(string CId)
        {
            string NewLn = Environment.NewLine;
            modRiskManagementSystem dp = new modRiskManagementSystem();
            IEnumerable<DPHodlingRMSModel> dpholding = dp.GetDpHoldingLink(CId);
            ModelState.Clear();
            return View(dpholding);
        }
        //RMS

        public ActionResult GetStockbylink(string CCode, string ScriptName)
        {
            string NewLn = Environment.NewLine;
            modRiskManagementSystem dp = new modRiskManagementSystem();
            IEnumerable<StockRMSSModel> dslink = dp.GetStockbylink(CCode, ScriptName);
            ModelState.Clear();
            return View(dslink);
        }


        public ActionResult CombinedContractNoteReport(string Select, string Code, string FDate, string ToDate, string cmbclient)
        {

            UtilityModel myutil = new UtilityModel();

            FDate = myutil.dtos(FDate);
            ToDate = myutil.dtos(ToDate);



            string IsAllView = "False";
            if (FDate == ToDate)
            {
                IsAllView = "True";
            }

            ViewBag.IsAllView = IsAllView;

            UtilityDBModel mydbutil = new UtilityDBModel();

            SqlConnection EsignConnection = mydbutil.EsignConnectionString("ESIGN-TRADEPLUS");
            if (EsignConnection != null)
            {
                if (EsignConnection.Database != null & EsignConnection.DataSource != null)
                {
                    try
                    {
                        EsignConnection.Close();
                        EsignConnection.Open();
                        modReports led = new modReports();
                        DataTable dt = led.GetCConfimationReport(Select, Code, FDate, ToDate, cmbclient);
                        ViewBag.Message = null;


                        return View(dt);
                    }

                    catch (Exception e)
                    {

                        ViewBag.Message = e.Message;

                        return View();
                    }
                }
                else
                {

                    ViewBag.Message = "Unable to Connect Esign Database";
                    return View();
                }
            }
            else
            {

                ViewBag.Message = "Unable to Connect Esign Database";
                return View();
            }
        }

        [HttpGet]
        public FileResult CashConfirmationReportDownload(string FDate, string ToDate, string strClient)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            SqlConnection connection;

            connection = mydbutil.EsignConnectionString(Session["DBConn"].ToString());

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var ds = new DataTable();
            ds = (DataTable)Session["dsCombine"];

            //string strFile = Server.MapPath("..") + @"\TempFolder\" + FDate + ToDate + Session.SessionID + Session["gstrUsercd"] + ".PDF";
            //  string strClient = string.Empty;

            //strClient = ds.Rows[0]["dd_clientcd"].ToString();

            DataTable dsFilename;

            string strsql = "";
            strsql = " select dd_filename,dd_document from  digital_details ";
            strsql += " Where dd_dt between '" + FDate + "' and '" + ToDate + "'";
            strsql += " and dd_filetype='CNOTE' and dd_clientcd ='" + strClient + "'";
            strsql += " Order by dd_Dt ";

            dsFilename = myLib.OpenDataTable(strsql, connection);


            byte[] filecontent;
            string filename = "CombineContract_" + FDate + "_" + ToDate + "_" + Strings.RTrim(strClient) + ".PDF";

            if (dsFilename.Rows.Count == 1)
            {
                filecontent = (byte[])dsFilename.Rows[0]["dd_document"];
            }
            else
            {
                List<byte[]> pdfFileList = new List<byte[]>();
                for (int intCnt = 0; intCnt < dsFilename.Rows.Count; intCnt++)
                {
                    pdfFileList.Add((byte[])dsFilename.Rows[intCnt]["dd_document"]);
                }
                filecontent = MergePDFFromDataBase(pdfFileList);
            }

            return File(filecontent, "application/pdf", filename);
        }

        [HttpGet]
        public FileResult CashConfirmationReportDownloadAll(string FDate, string ToDate)
        {
            //DataTable AllDT = new DataTable();
            //AllDT = (DataTable)Session["dsCombine"];

            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            SqlConnection connection;

            connection = mydbutil.EsignConnectionString(Session["DBConn"].ToString());

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var ds = new DataTable();
            ds = (DataTable)Session["dsCombine"];

            //string strFile = Server.MapPath("..") + @"\TempFolder\" + FDate + ToDate + Session.SessionID + Session["gstrUsercd"] + ".PDF";
            //  string strClient = string.Empty;

            //strClient = ds.Rows[0]["dd_clientcd"].ToString();
            string filename = "CombineContract_" + FDate + "_" + ToDate + "" + ".PDF";
            byte[] filecontent = null;
            List<byte[]> pdfFileList = new List<byte[]>();
            for (int i = 0; i < ds.Rows.Count; i++)
            {


                DataTable dsFilename;

                string strsql = "";
                strsql = " select dd_filename,dd_document from  digital_details ";
                strsql += " Where dd_dt between '" + FDate + "' and '" + ToDate + "'";
                strsql += " and dd_filetype='CNOTE' and dd_clientcd ='" + ds.Rows[i]["dd_clientcd"] + "'";
                strsql += " Order by dd_Dt ";

                dsFilename = myLib.OpenDataTable(strsql, connection);

                pdfFileList.Add((byte[])dsFilename.Rows[0]["dd_document"]);


                filecontent = MergePDFFromDataBase(pdfFileList);



            }
            return File(filecontent, "application/pdf", filename);
        }
        public static byte[] MergePDFFromDataBase(List<byte[]> pdfByteContent)
        {

            using (var ms = new MemoryStream())
            {
                var outputDocument = new Document();
                var writer = new PdfCopy(outputDocument, ms);
                outputDocument.Open();

                foreach (var doc in pdfByteContent)
                {
                    var reader = new PdfReader(doc);
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        writer.AddPage(writer.GetImportedPage(reader, i));
                    }
                    writer.FreeReader(reader);
                    reader.Close();
                }

                writer.Close();
                outputDocument.Close();
                var allPagesContent = ms.GetBuffer();
                ms.Flush();

                return allPagesContent;
            }
        }

        public ActionResult ComConfirmationReport(string Code, string date, string RdTrade, string RdContract, string cmbSelect, string cmbExchSeg, string chkBranchClient, string chkSep)
        {
            ViewBag.date = date;
            ViewBag.RdContract = RdContract;
            ViewBag.RdTrade = RdTrade;
            modReports led = new modReports();
            DataTable dt = new DataTable();
            dt = led.GetComConfirmationReport(Code, date, RdTrade, RdContract, cmbSelect, cmbExchSeg, chkBranchClient, chkSep);
            return View(dt);
        }

        public string QuaterlySettlementDate(string Segment)
        {
            UtilityModel myutil = new UtilityModel();
            modReports led = new modReports();
            DataTable dt = led.GetQuaterlySettlementReport(Segment);
            string result = " As On : " + myutil.DbToDate(dt.Rows[0][0].ToString());
            return result;
        }
        public ActionResult QuaterlySettlementReport(string Select, string Code, string Report, string Sortby, string Segment)
        {
            modReports led = new modReports();
            ViewBag.Report = Report;
            DataTable dt = led.getQtlySettlementReport(Select, Code, Report, Sortby, Segment);
            return View(dt);
        }



        public ActionResult GetQuaterlySettlementReport(string strClient, string stdate)
        {
            modReports led = new modReports();
            DataTable dt = led.GetQuaterlySettlementReport(strClient, stdate);
            return View(dt);
        }


        public ActionResult GetDailystatusReport(string strClient, string stdate)
        {
            modReports led = new modReports();
            DataTable dt = led.GetDailystatusReport(strClient, stdate);
            return View(dt);
        }


        public ActionResult cmbproduct()
        {
            modReports cm = new modReports();
            IEnumerable<ddlcmbproduct> ulist = cm.Populateproduct();
            return View(ulist);
        }


        public ActionResult ClientMasterShow()
        {
            return View();
        }


        public ActionResult GetClientMasterDetails(string Code)
        {

            string NewLn = Environment.NewLine;
            LibraryModel myLib = new LibraryModel();

            DataTable dt = new DataTable();
            string strsql = "";
            strsql = "select cm_freezeyn from client_master where cm_cd='" + Code + "'";
            dt = myLib.OpenDataTable(strsql);
            if (dt.Rows.Count > 0)
            {
                ViewBag.cm_freezeyn = dt.Rows[0]["cm_freezeyn"].ToString();
            }
            else
            {
                ViewBag.cm_freezeyn = "Y";
            }

            ViewBag.fundpayout = myLib.fnFireQuery("client_info", "case cm_fundpayout when 'O' then 'On Demand' when 'A' then 'Auto' else '' end as cm_fundpayout1 ", "cm2_cd", Code, true);
            ViewBag.paymentmode = myLib.fnFireQuery("client_master", "case cm_bankacttype when 'C' then 'Cheque' when 'D' then 'Demand Draft' when 'T' then 'Transfer' else '' end as cm_bankacttype1 ", "cm_cd", Code, true);
            ViewBag.sharepayout = myLib.fnFireQuery("Client_master", "case cm_directpayout when 'Y' then 'Direct' when 'N' then 'On Demand' when 'W' then 'Weekly' when 'A' then 'Always Hold' when 'D' then 'Daily' when 'M' then 'Monthly' else '' end as cm_directpayout1", "cm_cd", Code, true);
            modReports led = new modReports();
            ClientMasterDetails dp = led.GetClientMasterDetails(Code);
            ModelState.Clear();

            DataTable rsContact;

            rsContact = myLib.OpenDataTable("Select * from Common_Contacts Where cc_Client = '" + Code.Trim() + "' and cc_Contact = '" + dp.Mobile.Trim() + "'");
            if (!(rsContact.Rows.Count == 0))
            {
                ViewBag.cmbMobileStatus = Strings.Trim(rsContact.Rows[0]["cc_Relation"].ToString());
            }

            rsContact = myLib.OpenDataTable("Select * from Common_Contacts Where cc_Client = '" + Code.Trim() + "' and cc_Contact = '" + dp.EmailId.Trim() + "'");
            if (!(rsContact.Rows.Count == 0))
            {
                ViewBag.cmbEmailStatus = Strings.Trim(rsContact.Rows[0]["cc_Relation"].ToString());
            }
            return View(dp);

        }

        public ActionResult GetComClientMasterDetails(string Code)
        {
            string NewLn = Environment.NewLine;
            LibraryModel myLib = new LibraryModel();
            ViewBag.fundpayout = myLib.fnFireQuery("client_info", "case cm_fundpayout when 'O' then 'On Demand' when 'A' then 'Auto' else '' end as cm_fundpayout1 ", "cm2_cd", Code, true);
            ViewBag.paymentmode = myLib.fnFireQuery("client_master", "case cm_bankacttype when 'C' then 'Cheque' when 'D' then 'Demand Draft' when 'T' then 'Transfer' else '' end as cm_bankacttype1 ", "cm_cd", Code, true);
            ViewBag.sharepayout = myLib.fnFireQuery("Client_master", "case cm_directpayout when 'Y' then 'Direct' when 'N' then 'On Demand' when 'W' then 'Weekly' when 'A' then 'Always Hold' when 'D' then 'Daily' when 'M' then 'Monthly' else '' end as cm_directpayout1", "cm_cd", Code, true);
            modReports led = new modReports();
            ClientMasterDetails dp = led.GetComClientMasterDetails(Code);
            ModelState.Clear();
            return View(dp);

        }

        [HttpPost]
        public ActionResult ChangePwd(Changepwd model)
        {
            modReports led = new modReports();
            string dt = led.GetChangePwdReport(model.code, model.old_pwd, model.new_pwd);
            ViewBag.ResultMsg = dt;
            return View();

        }

        public ActionResult BulkpayoutNew()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult ContinuousDebitView(string Code, string Date, int Days, string Reporttype, string SearchBy, string excludeclient, string exceedamt, bool blnFromDashBoard = false)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            int SMS28DAYST = 0;
            int SMS28DAYS = 0;
            if (mylib.GetSysPARM("SMS28DAYST").Trim() != "")
                SMS28DAYST = Convert.ToInt32(mylib.GetSysPARM("SMS28DAYST").Trim());
            if (mylib.GetSysPARM("SMS28DAYS").Trim() != "")
                SMS28DAYS = Convert.ToInt32(mylib.GetSysPARM("SMS28DAYS").Trim());

            ViewBag.Reporttype = Reporttype;
            int intDaysback = (SMS28DAYST == 0 ? (SMS28DAYS == 0 ? 5 : SMS28DAYS) : SMS28DAYST);
            Days = intDaysback;


            ViewBag.blnFromDashBoard = blnFromDashBoard;
            if (ViewBag.Reporttype == "CMBCLOSINGBALANCE")
            {
                ViewBag.chkexcludeclient = "Exclude Clients Exceeding " + Days + " Days of Continuous Debit";
            }
            modContinuousDebit cd = new modContinuousDebit();
            IEnumerable<ContinuousDebitModel> dp = cd.GetContinuousDebit(Code, Date, Days, Reporttype, SearchBy, excludeclient, exceedamt);

            ModelState.Clear();
            return View(dp);
        }

        public ActionResult ClientDetails()
        {
            DataTable dt = null;
            string sql;
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                UtilityModel myutil = new UtilityModel();
                LibraryModel lib = new LibraryModel();
                sql = " select count(*) as IdCount from client_master,client_info where cm_cd=cm2_cd and  cm_name like '%'  and cm_schedule=49843750 and exists (select cm_cd from client_master, LoginAccess where((la_grouping = 'B' and LA_GrCode = cm_brboffcode) or(LA_grouping = 'G' and la_grcode = cm_groupcd) or(LA_grouping = 'A' and cm_cd = cm_cd) or(LA_grouping = 'C' and la_grcode = cm_cd) or(LA_grouping = 'F' and LA_GrCode = cm_familycd) or(LA_grouping = 'R' and LA_GrCode = cm_subbroker)) and cm_cd = cm2_cd and LA_UserId = 'harshad' )   and cm_specialyn <> 'Y' ";
                dt = lib.OpenDataTable(sql);
                ViewBag.dt = Math.Ceiling((double)(Convert.ToDecimal(dt.Rows[0]["IdCount"]) / 100));
                string todaydate = DateTime.Now.ToString("yyyyMMdd");

                string strnewdate = myutil.dtos(myutil.SubDayDT(todaydate, 90).ToString());
                //string strnewdate = "20191201";

                string strstrnewdateformated = myutil.DbToDate(strnewdate.Substring(0, strnewdate.Length - 2) + "01");
                ViewBag.date = strstrnewdateformated;
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult ClientDetailReport(string Code, string cmbField, string cmbValue, string date, string pageCondition, string cmbSort)
        {
            if (cmbField == "M" && cmbValue == "Bad")
            {
                ViewBag.select = "between 1 and 100";
            }
            else
            {
                ViewBag.select = pageCondition;
            }
            modReports led = new modReports();
            DataTable dt = led.GetClientDetails(Code, cmbField, cmbValue, date, pageCondition, cmbSort);
            return View(dt);
        }

        public ActionResult ComClientDetailReport(string Code, string cmbField, string cmbValue, string date, string pageCondition, string cmbSort)
        {
            if (cmbField == "M" && cmbValue == "Bad")
            {
                ViewBag.select = "between 1 and 100";
            }
            else
            {
                ViewBag.select = pageCondition;
            }
            //modClientInformationModel ulist = new modClientInformationModel();
            modReports led = new modReports();
            DataTable dt = led.GetComClientDetails(Code, cmbField, cmbValue, date, pageCondition, cmbSort);
            return View(dt);
        }
        public ActionResult GetCKYCEntry(string PANNo, string valNew)
        {

            if (PANNo != "" && PANNo != null)
            {
                string StrMsg = "";
                string StrSql = "";
                UtilityModel myutil = new UtilityModel();
                LibraryModel mylib = new LibraryModel();
                List<ClientKYCMasterModel> ulist = new List<ClientKYCMasterModel>();
                DataTable dt;


                StrSql = "select cm_panno,cm_name from client_master where cm_PANNO = '" + PANNo + "' and cm_schedule = '49843750'";

                if (mylib.fnGetSysParam("DEPOSITSFX").Trim().Replace("$", "") != "")
                {
                    StrSql = StrSql + " and cm_cd not in (select cm_brkggroup from Client_master Where cm_brkggroup <> '')";
                }
                dt = mylib.OpenDataTable(StrSql);

                if (dt.Rows.Count > 1)
                {
                    int i;
                    StrMsg = "Multiple UCC exist for PAN [ " + PANNo + @" ]\n";
                    ViewBag.Message = StrMsg;

                    for (i = 0; i <= dt.Rows.Count - 1; i++)
                        StrMsg = StrMsg + dt.Rows[i]["cm_name"].ToString() + @"\n";
                    StrMsg = StrMsg + "Update Dummy PAN against UCC not to be used for CKYC.";
                    ViewBag.Message = StrMsg;
                    return Content(StrMsg);
                }
                else
                {

                    if (dt.Rows.Count > 0)
                    {

                        StrSql = "select cm_cd,cm_name from client_master where cm_panno='" + PANNo + "'" + Session["LoginAccessOld"];

                        StrSql = StrSql + " and cm_schedule =  '49843750'";


                        if (mylib.fnGetSysParam("DEPOSITSFX").Trim().Replace("$", "") != "")
                        {
                            StrSql = StrSql + " and cm_cd not in (select cm_brkggroup from Client_master Where cm_brkggroup <> '')";
                        }


                        dt = mylib.OpenDataTable(StrSql);


                        if (dt.Rows.Count == 1)
                        {
                            if (mylib.fnFireQuery("Client_Info", "cm_constitution", "cm2_cd", dt.Rows[0]["cm_cd"].ToString().Trim(), true) != "1")
                            {
                                ViewBag.Message = "CKYC Entry Is Available Only For Individual Clients";
                                return Content("Yes1");
                            }

                            StrSql = "select * from Client_CKYC where CK_Panno='" + PANNo.Trim() + "' and CK_Status='Y'  and CK_ActType <> '03'";

                            dt = mylib.OpenDataTable(StrSql);

                            if (dt.Rows.Count > 0)
                            {
                                StrSql = " Select * From Client_CKYCImages where CI_SRNO = " + dt.Rows[0]["CK_SrNo"].ToString().Trim() + " Order by case ci_type When 'PH' Then 0 When 'SG' Then 1 When 'ID' Then 2 When 'AD' Then 3 else 99 end";
                                DataTable imgdt = mylib.OpenDataTable(StrSql);

                                byte[] Image;
                                int j;
                                var arrCIType = Strings.Split("PH,SG,ID,AD", ",");
                                if (imgdt.Rows.Count > 0)
                                {
                                    for (int i = 0, loopTo = imgdt.Rows.Count - 1; i <= loopTo; i++)
                                    {
                                        j = Array.IndexOf(arrCIType, imgdt.Rows[i]["CI_Type"]);
                                        // Image = Encoding.ASCII.GetBytes(imgdt.Rows[i]["CI_Image"].ToString());
                                        Image = (byte[])imgdt.Rows[i]["CI_Image"];
                                        Session["File" + j] = Image;
                                        Session["ContentType" + j] = imgdt.Rows[i]["CI_ContentType"].ToString().Trim();
                                        Session["FileSize" + j] = Image.Length;

                                        Session["File" + j] = "data:image/png;base64," + Convert.ToBase64String(Image, 0, Image.Length);

                                    }
                                }
                                else
                                {
                                    Session["File0"] = null;
                                    Session["File1"] = null;
                                    Session["File2"] = null;
                                    Session["File3"] = null;
                                    Session["File4"] = null;
                                }

                                ulist = dt.AsEnumerable()
                             .Select(row => new ClientKYCMasterModel
                             {
                                 CK_fatherspouseflag = row.Field<string>("CK_fatherspouseflag").Trim(),
                                 CK_FatherPrefix = row.Field<string>("CK_FatherPrefix").Trim(),
                                 CK_FatherFname = row.Field<string>("CK_FatherFname").Trim(),
                                 CK_FatherMname = row.Field<string>("CK_FatherMname").Trim(),
                                 CK_FatherLname = row.Field<string>("CK_FatherLname").Trim(),


                                 CK_MotherPrefix = row.Field<string>("CK_MotherPrefix").Trim(),
                                 CK_Motherfname = row.Field<string>("CK_Motherfname").Trim(),
                                 CK_MotherMname = row.Field<string>("CK_MotherMname").Trim(),
                                 CK_MotherLname = row.Field<string>("CK_MotherLname").Trim(),
                                 CK_ActType = row.Field<string>("CK_ActType").Trim(),
                                 CK_citizenship = row.Field<string>("CK_citizenship").Trim(),
                                 CK_Resistatus = row.Field<string>("CK_Resistatus").Trim(),
                                 CK_CityOfBirth = row.Field<string>("CK_CityOfBirth").Trim(),
                                 CK_CountyOfBirth = row.Field<string>("CK_CountyOfBirth").Trim(),


                                 CK_IdentityProof = row.Field<string>("CK_IdentityProof").Trim(),
                                 CK_IdentityProofID = row.Field<string>("CK_IdentityProofID").Trim(),
                                 CK_IdentityProofExpDt = row.Field<string>("CK_IdentityProofExpDt").Trim(),


                                 CK_PermAddrProof = row.Field<string>("CK_PermAddrProof").Trim(),
                                 CK_PermAddrProofID = row.Field<string>("CK_PermAddrProofID").Trim(),
                                 CK_PermAddrProofExpDt = row.Field<string>("CK_PermAddrProofExpDt").Trim(),

                                 CK_AddrProof = row.Field<string>("CK_AddrProof").Trim(),
                                 CK_AddrProofID = row.Field<string>("CK_AddrProofID").Trim(),
                                 CK_AddrProofExpDt = row.Field<string>("CK_AddrProofExpDt").Trim(),

                                 CK_SRNO = row.Field<Decimal>("CK_SRNO"),

                                 Image0 = (string)Session["File0"],
                                 Image1 = (string)Session["File1"],
                                 Image2 = (string)Session["File2"],
                                 Image3 = (string)Session["File3"]



                             }).ToList();


                                return Json(ulist, JsonRequestBehavior.AllowGet);

                            }

                            else
                            {
                                ViewBag.Message = "New Entry";
                                return Content("New Entry");
                            }










                        }
                        else
                        {
                            return Content("Edit");
                        }







                    }
                    else
                    {

                        ViewBag.Message = "Invalid PAN";
                        return Content("Yes");
                    }




                }





            }
            else
            {
                ViewBag.Message = "Invalid PAN";
                return Content("Yes");
            }

        }

        public ActionResult CKYCEntryUpdate(string HldStrNo, string PANNo, string MaidenTitle, string MaidenFNm, string MaidenMNm, string MaidenLNm, string fatherTitle, string FatherFNm, string FatherMNm, string FatherLNm, string MotherTitle, string MotherFNm, string MotherMNm, string MotherLNm, string FatherOrSpouse, string Citizenship, string ResStatus, string IdentityProofID, string IdentityProof, string IdentityProofExpDt, string CorrosAddProof, string AddrProofID, string AddrProofExpDt, string PermanantAddProof, string PermAddrProofID, string PermAddrProofExpDt, string BirthCountry, string BirthPlace, string ACType)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string StrSql = "";


            string lngSRNO = HldStrNo;
            StrSql = "Update Client_CKYC ";
            StrSql = StrSql + "Set CK_MaidenPrefix ='" + MaidenTitle + "',CK_MaidenFname ='" + MaidenFNm + "',CK_MaidenMName ='" + MaidenMNm + "',CK_MaidenLname ='" + MaidenLNm + "',";
            StrSql = StrSql + "CK_FatherPrefix ='" + fatherTitle + "',CK_FatherFname ='" + FatherFNm + "',CK_FatherMname ='" + FatherMNm + "',CK_FatherLname ='" + FatherLNm + "',";
            StrSql = StrSql + "CK_MotherPrefix ='" + MotherTitle + "',CK_Motherfname ='" + MotherFNm + "',CK_MotherMname ='" + MotherMNm + "',CK_MotherLname ='" + MotherLNm + "',";
            StrSql = StrSql + "CK_fatherspouseflag ='" + FatherOrSpouse + "', CK_citizenship ='" + Citizenship + "',";
            StrSql = StrSql + "CK_Resistatus ='" + ResStatus + "', CK_ResiTaxPurpose ='',";
            StrSql = StrSql + "CK_IdentityProof ='" + IdentityProofID + "',CK_IdentityProofID ='" + IdentityProof + "',CK_IdentityProofExpDt ='" + myutil.dtos(IdentityProofExpDt) + "',";
            StrSql = StrSql + "CK_AddrProof ='" + CorrosAddProof + "',CK_AddrProofID ='" + AddrProofID + "',CK_AddrProofExpDt ='" + myutil.dtos(AddrProofExpDt) + "',";
            StrSql = StrSql + "CK_PermAddrProof ='" + PermanantAddProof + "', CK_PermAddrProofID ='" + PermAddrProofID + "', CK_PermAddrProofExpDt ='" + myutil.dtos(PermAddrProofExpDt) + "', ";
            StrSql = StrSql + "CK_CityOfBirth ='" + BirthCountry + "',CK_CountyOfBirth ='" + BirthPlace + "',CK_verifyBy ='', CK_Status ='Y',";
            StrSql = StrSql + "CK_batchno =0, CK_batchDt ='',CK_Reference='', mkrId ='" + Session["gstrusercd"] + "', mkrDt ='" + myutil.gstrDBToday() + "',CK_ActType='" + ACType + "', Ck_RespType='', Ck_AppType=''";
            StrSql = StrSql + " where CK_Panno='" + PANNo + "' and CK_SRNO='" + lngSRNO + "'";

            myLib.ExecSQL(StrSql);


            return View();


        }

        public ActionResult CKYCEntrySave()
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            string PANNo = "";
            string MaidenTitle = "";
            string MaidenFNm = "";
            string MaidenMNm = "";
            string MaidenLNm = "";
            string fatherTitle = "";
            string FatherFNm = "";
            string FatherMNm = "";
            string FatherLNm = "";
            string MotherTitle = "";
            string MotherFNm = "";
            string MotherMNm = "";
            string MotherLNm = "";
            string FatherOrSpouse = "";
            string Citizenship = "";
            string ResStatus = "";
            string IdentityProofID = "";
            string IdentityProof = "";
            string IdentityProofExpDt = "";
            string CorrosAddProof = "";
            string AddrProofID = "";
            string AddrProofExpDt = "";
            string PermanantAddProof = "";
            string PermAddrProofID = "";
            string PermAddrProofExpDt = "";
            string BirthCountry = "";
            string BirthPlace = "";
            string ACType = "";
            string FileIdentity = "";
            string FileAddress = "";
            string FilePhoto = "";
            string FileSignature = "";

            string StrSql = "";
            int lngSRNO = 1;
            lngSRNO = Convert.ToInt32(myLib.fnFireQuery("Client_CKYC", "isnull(max(CK_SRNO),0)", "1", "1", true)) + 1;
            StrSql = "Insert into Client_CKYC ";
            StrSql = StrSql + "Select '" + lngSRNO + "' CK_SRNO,'" + PANNo + "' CK_Panno,'" + MaidenTitle + "' CK_MaidenPrefix,'" + MaidenFNm + "' CK_MaidenFname,'" + MaidenMNm + "' CK_MaidenMName,'" + MaidenLNm + "' CK_MaidenLname, ";
            StrSql = StrSql + "'" + fatherTitle + "' CK_FatherPrefix,'" + FatherFNm + "' CK_FatherFname,'" + FatherMNm + "' CK_FatherMname,'" + FatherLNm + "' CK_FatherLname, ";
            StrSql = StrSql + "'" + MotherTitle + "' CK_MotherPrefix,'" + MotherFNm + "'  CK_Motherfname,'" + MotherMNm + "' CK_MotherMname,'" + MotherLNm + "' CK_MotherLname,";
            StrSql = StrSql + "'" + FatherOrSpouse + "' CK_fatherspouseflag,'" + Citizenship + "' CK_citizenship,'" + ResStatus + "' CK_Resistatus ,'' CK_ResiTaxPurpose , ";
            StrSql = StrSql + "'" + IdentityProof + "' CK_IdentityProof,'" + IdentityProofID + "' CK_IdentityProofID,'" + myutil.dtos(IdentityProofExpDt) + "' CK_IdentityProofExpDt,";
            StrSql = StrSql + "'" + CorrosAddProof + "' CK_AddrProof,'" + AddrProofID + "' CK_AddrProofID,'" + myutil.dtos(AddrProofExpDt) + "' CK_AddrProofExpDt,";
            StrSql = StrSql + "'" + PermanantAddProof + "' CK_PermAddrProof ,'" + PermAddrProofID + "' CK_PermAddrProofID,'" + myutil.dtos(PermAddrProofExpDt) + "' CK_PermAddrProofExpDt,";
            StrSql = StrSql + "'" + BirthPlace + "' CK_CityOfBirth,'" + BirthCountry + "' CK_CountyOfBirth,'' CK_verifyBy,";
            StrSql = StrSql + "'Y' CK_Status,0 CK_batchno,'' CK_batchDt,'' CK_Reference,'" + Session["gstrusercd"] + "' mkrId,'" + Session["gstrToday"] + "' mkrDt,'" + ACType + "' CK_ActType,'' Ck_RespType,'' Ck_AppType,";
            StrSql = StrSql + "''Ck_Filler1,''  Ck_Filler2,''  Ck_Filler3,''  Ck_NFiller1,0  Ck_Nfiller2,0  Ck_NFiller3";

            myLib.ExecSQL(StrSql);

            string fileName = Path.GetFileName(FileIdentity);
            string fileExt = Path.GetExtension(FileIdentity);

            string filepath = FileIdentity.Replace(@"\\", @"\");



            return View();


        }

        [HttpPost]
        public ActionResult CKYCEntryNew()
        {
            string btnvalue = System.Web.HttpContext.Current.Request["btnvalue"];

            int lngSRNO = 1;
            string StrSql = "";
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string chkckycdone = System.Web.HttpContext.Current.Request["chkckycdone"];
            if (chkckycdone == "1")
            {
                string txtckycrefno = System.Web.HttpContext.Current.Request["txtckycrefno"];
                string txtckycremark = System.Web.HttpContext.Current.Request["txtckycremark"];
                string PANNo = System.Web.HttpContext.Current.Request["txtUCCNm"];
                string HldStrNo = System.Web.HttpContext.Current.Request["HldStrNo"];
                if (btnvalue == "Save")
                {

                    lngSRNO = Convert.ToInt32(myLib.fnFireQuery("Client_CKYC", "isnull(max(CK_SRNO),0)", "1", "1", true)) + 1;
                    StrSql = "Insert into Client_CKYC ";
                    StrSql = StrSql + "Select '" + lngSRNO + "' CK_SRNO,'" + PANNo.Trim() + "' CK_Panno,'' CK_MaidenPrefix ,'' CK_MaidenFname,'' CK_MaidenMName,'' CK_MaidenLname,";
                    StrSql = StrSql + " '' CK_FatherPrefix,'' CK_FatherFname,'' CK_FatherMname,'' CK_FatherLname, '' CK_MotherPrefix,''  CK_Motherfname,'' CK_MotherMname,'' CK_MotherLname,";
                    StrSql = StrSql + " '' CK_fatherspouseflag,'' CK_citizenship,'' CK_Resistatus ,'' CK_ResiTaxPurpose , '' CK_IdentityProof,'' CK_IdentityProofID,'' CK_IdentityProofExpDt,";
                    StrSql = StrSql + " '' CK_AddrProof,'' CK_AddrProofID,'' CK_AddrProofExpDt,'' CK_PermAddrProof ,'' CK_PermAddrProofID,'' CK_PermAddrProofExpDt,'' CK_CityOfBirth,'' CK_CountyOfBirth,";
                    StrSql = StrSql + " '' CK_verifyBy,'S' CK_Status,0 CK_batchno,'' CK_batchDt,'' CK_Reference,'" + Session["gstrusercd"] + "' mkrId,'" + Session["gstrToday"] + "' mkrDt,'' CK_ActType,'' Ck_RespType,'' Ck_AppType,";
                    StrSql = StrSql + " '" + txtckycremark.Trim() + "' Ck_Filler1,'M' Ck_Filler2,'' Ck_Filler3,'" + txtckycrefno.Trim() + "' Ck_NFiller1,0 Ck_Nfiller2,0 Ck_NFiller3 ";
                    myLib.ExecSQL(StrSql);
                }
                else
                {
                    lngSRNO = Convert.ToInt32(HldStrNo);
                    StrSql = "Update Client_CKYC ";
                    StrSql = StrSql + "Set  Ck_NFiller1='" + txtckycrefno.Trim() + "' , Ck_Filler1 ='" + txtckycremark.Trim() + "',CK_Status='S', Ck_Filler2 ='M',";
                    StrSql = StrSql + " CK_MaidenPrefix ='',CK_MaidenFname ='',CK_MaidenMName ='',CK_MaidenLname ='',CK_FatherPrefix ='',CK_FatherFname ='',CK_FatherMname ='', ";
                    StrSql = StrSql + " CK_FatherLname ='',CK_MotherPrefix ='',CK_Motherfname ='',CK_MotherMname ='',CK_MotherLname ='',CK_fatherspouseflag ='', CK_citizenship ='',CK_Resistatus ='', ";
                    StrSql = StrSql + " CK_ResiTaxPurpose ='',CK_IdentityProof ='',CK_IdentityProofID ='',CK_IdentityProofExpDt ='',CK_AddrProof ='',CK_AddrProofID ='',CK_AddrProofExpDt ='', ";
                    StrSql = StrSql + " CK_PermAddrProof ='', CK_PermAddrProofID ='', CK_PermAddrProofExpDt ='', CK_CityOfBirth ='',CK_CountyOfBirth ='',CK_verifyBy ='',CK_batchno =0, CK_batchDt ='',";
                    StrSql = StrSql + " CK_Reference='', mkrId ='" + Session["gstrToday"] + "', mkrDt ='" + Session["gstrToday"] + "',CK_ActType='', Ck_RespType='' ";
                    StrSql = StrSql + " where CK_Panno='" + PANNo.Trim() + "' and CK_SRNO='" + lngSRNO + "'";
                    myLib.ExecSQL(StrSql);
                }

            }
            else
            {

                string FatherOrSpouse = System.Web.HttpContext.Current.Request["cmbFatherOrSpouseU"];
                string fatherTitle = System.Web.HttpContext.Current.Request["txtfatherTitle"];
                string FatherFNm = System.Web.HttpContext.Current.Request["txtFatherFNmU"];
                string FatherMNm = System.Web.HttpContext.Current.Request["txtFatherMNmU"];
                string FatherLNm = System.Web.HttpContext.Current.Request["txtFatherLNmU"];

                string MotherTitle = System.Web.HttpContext.Current.Request["txtMotherTitle"].Replace(",", "");
                string MotherFNm = System.Web.HttpContext.Current.Request["txtMotherFNm"].Replace(",", "");
                string MotherMNm = System.Web.HttpContext.Current.Request["txtMotherMNm"].Replace(",", "");
                string MotherLNm = System.Web.HttpContext.Current.Request["txtMotherLNm"].Replace(",", "");

                string MaidenTitle = System.Web.HttpContext.Current.Request["txtMaidenTitle"];
                string MaidenFNm = System.Web.HttpContext.Current.Request["txtMaidenFNm"];
                string MaidenMNm = System.Web.HttpContext.Current.Request["txtMaidenMNm"];
                string MaidenLNm = System.Web.HttpContext.Current.Request["txtMaidenLNm"];

                string ACType = System.Web.HttpContext.Current.Request["cmbACTypeU"];
                string APPType = "01";
                string Citizenship = System.Web.HttpContext.Current.Request["cmbCitizenshipU"];
                string ResStatus = System.Web.HttpContext.Current.Request["cmbResStatusU"];
                string BirthPlace = System.Web.HttpContext.Current.Request["txtBirthPlaceU"];


                string IdentityProofID = System.Web.HttpContext.Current.Request["txtIdentityProofIDU"];
                string IdentityProofExpDt = System.Web.HttpContext.Current.Request["txtIdentityProofExpDtU"];
                if (IdentityProofExpDt != "")
                { IdentityProofExpDt = myutil.dtos(IdentityProofExpDt); }

                string PermanantAddProof = System.Web.HttpContext.Current.Request["cmbPermanantAddProofU"];
                string PermAddrProofExpDt = System.Web.HttpContext.Current.Request["txtPermAddrProofExpDtU"];
                if (PermAddrProofExpDt != "")
                {
                    PermAddrProofExpDt = myutil.dtos(PermAddrProofExpDt);
                }

                string PermAddrProofID = System.Web.HttpContext.Current.Request["txtPermAddrProofIDU"];
                string CorrosAddProof = System.Web.HttpContext.Current.Request["cmbCorrosAddProofU"];

                string AddrProofExpDt = System.Web.HttpContext.Current.Request["txtAddrProofExpDtU"];
                if (AddrProofExpDt != "")
                {
                    AddrProofExpDt = myutil.dtos(AddrProofExpDt);
                }
                string AddrProofID = System.Web.HttpContext.Current.Request["txtAddrProofIDU"];
                string BirthCountry = System.Web.HttpContext.Current.Request["txtBirthCountryU"];
                string IdentityProof = System.Web.HttpContext.Current.Request["cmbIdentityProofU"];
                string PANNo = System.Web.HttpContext.Current.Request["txtUCCNm"];
                string HldStrNo = System.Web.HttpContext.Current.Request["HldStrNo"];




                string FilePhoto = "";
                string FileSignature = "";


                if (btnvalue == "Save")
                {


                    lngSRNO = Convert.ToInt32(myLib.fnFireQuery("Client_CKYC", "isnull(max(CK_SRNO),0)", "1", "1", true)) + 1;
                    StrSql = "Insert into Client_CKYC ";
                    StrSql = StrSql + "Select '" + lngSRNO + "' CK_SRNO,'" + PANNo + "' CK_Panno,'" + MaidenTitle + "' CK_MaidenPrefix,'" + MaidenFNm + "' CK_MaidenFname,'" + MaidenMNm + "' CK_MaidenMName,'" + MaidenLNm + "' CK_MaidenLname, ";
                    StrSql = StrSql + "'" + fatherTitle + "' CK_FatherPrefix,'" + FatherFNm + "' CK_FatherFname,'" + FatherMNm + "' CK_FatherMname,'" + FatherLNm + "' CK_FatherLname, ";
                    StrSql = StrSql + "'" + MotherTitle + "' CK_MotherPrefix,'" + MotherFNm + "'  CK_Motherfname,'" + MotherMNm + "' CK_MotherMname,'" + MotherLNm + "' CK_MotherLname,";
                    StrSql = StrSql + "'" + FatherOrSpouse + "' CK_fatherspouseflag,'" + Citizenship + "' CK_citizenship,'" + ResStatus + "' CK_Resistatus ,'' CK_ResiTaxPurpose , ";
                    StrSql = StrSql + "'" + IdentityProof + "' CK_IdentityProof,'" + IdentityProofID + "' CK_IdentityProofID,'" + IdentityProofExpDt + "' CK_IdentityProofExpDt,";
                    StrSql = StrSql + "'" + CorrosAddProof + "' CK_AddrProof,'" + AddrProofID + "' CK_AddrProofID,'" + AddrProofExpDt + "' CK_AddrProofExpDt,";
                    StrSql = StrSql + "'" + PermanantAddProof + "' CK_PermAddrProof ,'" + PermAddrProofID + "' CK_PermAddrProofID,'" + PermAddrProofExpDt + "' CK_PermAddrProofExpDt,";
                    StrSql = StrSql + "'" + BirthPlace + "' CK_CityOfBirth,'" + BirthCountry + "' CK_CountyOfBirth,'' CK_verifyBy,";
                    StrSql = StrSql + "'Y' CK_Status,0 CK_batchno,'' CK_batchDt,'' CK_Reference,'" + Session["gstrusercd"] + "' mkrId,'" + myutil.gstrDBToday() + "' mkrDt,'" + ACType + "' CK_ActType,'' Ck_RespType,'" + APPType + "' Ck_AppType,";
                    StrSql = StrSql + "''Ck_Filler1,''  Ck_Filler2,''  Ck_Filler3,''  Ck_NFiller1,0  Ck_Nfiller2,0  Ck_NFiller3";
                    myLib.ExecSQL(StrSql);
                }

                else
                {
                    lngSRNO = Convert.ToInt32(HldStrNo);
                    StrSql = "Update Client_CKYC ";
                    StrSql = StrSql + "Set CK_MaidenPrefix ='" + MaidenTitle + "',CK_MaidenFname ='" + MaidenFNm + "',CK_MaidenMName ='" + MaidenMNm + "',CK_MaidenLname ='" + MaidenLNm + "',";
                    StrSql = StrSql + "CK_FatherPrefix ='" + fatherTitle + "',CK_FatherFname ='" + FatherFNm + "',CK_FatherMname ='" + FatherMNm + "',CK_FatherLname ='" + FatherLNm + "',";
                    StrSql = StrSql + "CK_MotherPrefix ='" + MotherTitle + "',CK_Motherfname ='" + MotherFNm + "',CK_MotherMname ='" + MotherMNm + "',CK_MotherLname ='" + MotherLNm + "',";
                    StrSql = StrSql + "CK_fatherspouseflag ='" + FatherOrSpouse + "', CK_citizenship ='" + Citizenship + "',";
                    StrSql = StrSql + "CK_Resistatus ='" + ResStatus + "', CK_ResiTaxPurpose ='',";
                    StrSql = StrSql + "CK_IdentityProof ='" + IdentityProof + "',CK_IdentityProofID ='" + IdentityProofID + "',CK_IdentityProofExpDt ='" + IdentityProofExpDt + "',";
                    StrSql = StrSql + "CK_AddrProof ='" + CorrosAddProof + "',CK_AddrProofID ='" + AddrProofID + "',CK_AddrProofExpDt ='" + AddrProofExpDt + "',";
                    StrSql = StrSql + "CK_PermAddrProof ='" + PermanantAddProof + "', CK_PermAddrProofID ='" + PermAddrProofID + "', CK_PermAddrProofExpDt ='" + PermAddrProofExpDt + "', ";
                    StrSql = StrSql + "CK_CityOfBirth ='" + BirthPlace + "',CK_CountyOfBirth ='" + BirthCountry + "',CK_verifyBy ='', CK_Status ='Y',";
                    StrSql = StrSql + "CK_batchno =0, CK_batchDt ='',CK_Reference='', mkrId ='" + Session["gstrusercd"] + "', mkrDt ='" + myutil.gstrDBToday() + "',CK_ActType='" + ACType + "', Ck_RespType='', Ck_AppType='" + APPType + "'";
                    StrSql = StrSql + " where CK_Panno='" + PANNo + "' and CK_SRNO='" + lngSRNO + "'";
                    myLib.ExecSQL(StrSql);
                }


                StrSql = "Delete From Client_CKYCImages where CI_SRNO= " + lngSRNO;
                myLib.ExecSQL(StrSql);



                byte[] FIdentityProof = null;
                byte[] FPermAddrProof = null;
                byte[] FPhoto = null;
                byte[] FSignature = null;
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var pic1 = System.Web.HttpContext.Current.Request.Files["FIdentityProof"];
                    BinaryReader reader1 = new BinaryReader(pic1.InputStream);
                    FIdentityProof = reader1.ReadBytes(pic1.ContentLength);
                    Session["IFile0"] = FIdentityProof;
                    Session["IContentType0"] = Path.GetExtension(pic1.FileName).ToString().Replace(".", "");

                    var pic2 = System.Web.HttpContext.Current.Request.Files["FPermAddrProof"];
                    BinaryReader reader2 = new BinaryReader(pic2.InputStream);
                    FPermAddrProof = reader2.ReadBytes(pic2.ContentLength);
                    Session["IFile1"] = FPermAddrProof;
                    Session["IContentType1"] = Path.GetExtension(pic2.FileName).ToString().Replace(".", "");

                    var pic3 = System.Web.HttpContext.Current.Request.Files["FPhoto"];
                    BinaryReader reader3 = new BinaryReader(pic3.InputStream);
                    FPhoto = reader3.ReadBytes(pic3.ContentLength);
                    Session["IFile2"] = FPhoto;
                    Session["IContentType2"] = Path.GetExtension(pic3.FileName).ToString().Replace(".", "");

                    var pic4 = System.Web.HttpContext.Current.Request.Files["FSignature"];
                    BinaryReader reader4 = new BinaryReader(pic4.InputStream);
                    FSignature = reader4.ReadBytes(pic4.ContentLength);
                    Session["IFile3"] = FSignature;
                    Session["IContentType3"] = Path.GetExtension(pic4.FileName).ToString().Replace(".", "");


                }


                var arrCIType = Strings.Split("PH,SG,ID,AD", ",");
                SqlCommand insert;
                SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ConnectionString);
                for (int i = 0; i <= 3; i++)
                {

                    if (Session["IFile" + i].ToString() != "")
                    {
                        if (Session["IContentType" + i].ToString() == "PDF")
                        {
                            insert = new SqlCommand("Insert into Client_CKYCImages values('" + lngSRNO + "','" + PANNo + "','" + arrCIType[i] + "',@Data,'" + Session["gstrusercd"] + "','" + myutil.gstrDBToday() + "','" + Session["IContentType" + i] + "')", con);
                            insert.Parameters.Add("@Data", SqlDbType.VarBinary).Value = Session["IFile" + i];
                        }
                        else
                        {
                            insert = new SqlCommand("Insert into Client_CKYCImages values('" + lngSRNO + "','" + PANNo + "','" + arrCIType[i] + "',@ImageData,'" + Session["gstrusercd"] + "','" + myutil.gstrDBToday() + "','" + Session["IContentType" + i] + "')", con);
                            insert.Parameters.Add("@ImageData", SqlDbType.Image).Value = Session["IFile" + i];
                        }
                        con.Open();
                        insert.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }


            return Content("Data Submited");
        }

        public byte[] ConvertImagetoByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        [HttpGet]
        public FileResult DownLoadFile()
        {

            byte[] filecontent;
            string filename = "ABC.PDF";
            //filecontent = System.Text.Encoding.ASCII.GetBytes(Session["File1"].ToString());
            filecontent = (byte[])Session["File1"];

            return File(filecontent, "application/pdf", filename);

        }

        public JsonResult FillClientBrok(string Code)
        {
            modBrokerage rec = new modBrokerage();
            var result = rec.FillClientBrok(Code);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult FillComboBrokExchangeSegment(string Segment)
        {
            modBrokerage rec = new modBrokerage();
            var result = rec.FillComboBrokES(Segment);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveDataModification()
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            DataTable dsCLBr = new DataTable();

            string strSQL = "";
            string strsql2 = "";
            string strCM = "";
            int intTemp = 0;
            string strTime = myutil.GetTimeNow();
            string txtCltCd = System.Web.HttpContext.Current.Request["txtCltCd"];
            string txtCltName = System.Web.HttpContext.Current.Request["txtCltName"];

            string txtGrpCd = System.Web.HttpContext.Current.Request["txtGrpCd"];
            string txtGrpDesc = System.Web.HttpContext.Current.Request["txtGrpDesc"];

            string txtFmlyCd = System.Web.HttpContext.Current.Request["txtFmlyCd"];
            string txtFmlyDesc = System.Web.HttpContext.Current.Request["txtFmlyDesc"];



            string txtmobile = System.Web.HttpContext.Current.Request["txtmobile"];
            string txtemail = System.Web.HttpContext.Current.Request["txtemail"];



            strSQL = "select cm_cd, cm_panno,cm_familycd,cm_mobile,cm_email, fm_desc, cm_groupcd,cm_brboffcode, gr_desc, Client_Master.mkrid, Client_Master.mkrdt from ";
            strSQL = strSQL + " Client_Master, family_master, Group_master where cm_familycd = fm_cd and cm_groupcd = gr_cd ";
            strSQL = strSQL + " and cm_cd = '" + txtCltCd + "'";
            dsCLBr = myLib.OpenDataTable(strSQL);


            string strid = "select  CES_Cd from CompanyExchangesegments where Left(CES_Cd, 1) = '" + Session["CompanyCode"] + "'";
            DataTable dsid = myLib.OpenDataTable(strid);

            strSQL = "Insert into common_audit values('Client_master','" + dsid.Rows[0]["CES_Cd"] + "','cm_cd','" + txtCltCd + "',";
            strsql2 = ",'" + Environment.MachineName + "','" + Session["gstrUsercd"] + "','" + Session["gstrToday"] + "','" + strTime + "','" + dsCLBr.Rows[0]["mkrid"] + "','" + dsCLBr.Rows[0]["mkrdt"] + "','00:00:00','Client Master','Client Code')";

            if ((Strings.Trim(dsCLBr.Rows[0]["cm_groupcd"].ToString()) ?? "") != (Strings.Trim(txtGrpCd) ?? ""))
            {
                myLib.ExecSQL(strSQL + "'cm_groupcd','Group code','" + dsCLBr.Rows[0]["cm_groupcd"] + "','" + txtGrpCd + "'" + strsql2);

                strCM = strCM + " cm_groupcd='" + Strings.Trim(txtGrpCd) + "', ";
                intTemp = intTemp + 1;
            }

            if ((Strings.Trim(dsCLBr.Rows[0]["cm_familycd"].ToString()) ?? "") != (Strings.Trim(txtFmlyCd) ?? ""))
            {
                myLib.ExecSQL(strSQL + "'cm_familycd','Family code','" + dsCLBr.Rows[0]["cm_familycd"] + "','" + txtFmlyCd + "'" + strsql2);

                strCM = strCM + " cm_familycd='" + Strings.Trim(txtFmlyCd) + "', ";
                intTemp = intTemp + 1;
            }

            if ((Strings.Trim(dsCLBr.Rows[0]["cm_mobile"].ToString()) ?? "") != (Strings.Trim(txtmobile) ?? ""))
            {
                myLib.ExecSQL(strSQL + "'cm_mobile','Mobile No','" + dsCLBr.Rows[0]["cm_mobile"] + "','" + Strings.Trim(txtmobile) + "'" + strsql2);

                strCM = strCM + " cm_mobile='" + Strings.Trim(txtmobile) + "', ";
                intTemp = intTemp + 1;
            }

            if ((Strings.Trim(dsCLBr.Rows[0]["cm_email"].ToString()) ?? "") != (Strings.Trim(txtemail) ?? ""))
            {
                myLib.ExecSQL(strSQL + "'cm_email','Email Id','" + dsCLBr.Rows[0]["cm_email"] + "','" + Strings.Trim(txtemail) + "'" + strsql2);

                strCM = strCM + " cm_email='" + Strings.Trim(txtemail) + "', ";
                intTemp = intTemp + 1;
            }

            if (strCM != "")
            {
                myLib.ExecSQL("update client_master set " + Strings.Left(strCM, strCM.Length - 2) + " where cm_cd='" + Strings.Trim(txtCltCd) + "'");

            }

            string strMsg = (intTemp > 0 ? " Details of client: [" + txtCltCd + "] Modified !" : "No details changed");

            return Content(strMsg);
        }

        public ActionResult ECommissionReport(string Select, string cmbGroupBy, string Code, string FromDate, string ToDate, string strDPID = "", string ClientType = "")
        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }

            ViewBag.cmbGroupBy = cmbGroupBy;
            modReports led = new modReports();
            DataTable dt = led.GetECommissionRep(Select, cmbGroupBy, Code, FromDate, ToDate, strDPID, ClientType);

            //IEnumerable<BrokerageModel> ulist = led.GetClientBrokerage(Select, Code, FromDate, ToDate, strDPID, ClientType);
            return View(dt);
        }

        public ActionResult BrokScheme(string Code, string name, string strApp = "")
        {
            ViewBag.code = Code;
            ViewBag.name = name;
            modReports led = new modReports();

            DataTable dt = led.GetBrokScheme(Code, strApp);


            return View(dt);

        }

        public ActionResult BrokSchemecashFo(string ces, string brkscheme)
        {
            if (Strings.Right(ces.Trim(), 1) == "X")
            {
                ViewBag.Iscomex = "True";
            }
            else
            {
                ViewBag.Iscomex = "False";
            }


            ViewBag.brkscheme = brkscheme;

            DataTable dt = null;

            modReports led = new modReports();

            string strdpid = Strings.Right(ces.Trim(), 1);
            if (strdpid == "C")
            {
                ViewBag.strdpid = "C";
                dt = led.Brkschemecash(ces, brkscheme);

            }
            else

            {
                double perlot1 = 0;
                double perlot2 = 0;
                double perlot3 = 0;
                double max1 = 0;
                double max2 = 0;
                double max3 = 0;
                double colspan1 = 2;
                double colspan2 = 2;
                double colspan3 = 2;

                dt = led.BrkschemeFO(ces, brkscheme);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    perlot1 = perlot1 + Convert.ToDouble(dt.Rows[i]["br_perlot1"]);
                    perlot2 = perlot2 + Convert.ToDouble(dt.Rows[i]["br_perlot2"]);
                    perlot3 = perlot3 + Convert.ToDouble(dt.Rows[i]["br_perlot3"]);
                    max1 = max1 + Convert.ToDouble(dt.Rows[i]["br_max1"]);
                    max2 = max2 + Convert.ToDouble(dt.Rows[i]["br_max2"]);
                    max3 = max3 + Convert.ToDouble(dt.Rows[i]["br_max3"]);
                }
                if (perlot1 > 0)
                    colspan1 = colspan1 + 1;
                if (perlot2 > 0)
                    colspan2 = colspan2 + 1;
                if (perlot3 > 0)
                    colspan3 = colspan3 + 1;
                if (max1 > 0)
                    colspan1 = colspan1 + 1;
                if (max2 > 0)
                    colspan2 = colspan2 + 1;
                if (max3 > 0)
                    colspan3 = colspan3 + 1;

                ViewBag.perlot1 = perlot1;
                ViewBag.perlot2 = perlot2;
                ViewBag.perlot3 = perlot3;
                ViewBag.max1 = max1;
                ViewBag.max2 = max2;
                ViewBag.max3 = max3;
                ViewBag.colspan1 = colspan1;
                ViewBag.colspan2 = colspan2;
                ViewBag.colspan3 = colspan3;

            }

            return View(dt);
        }

        public void ClearUnwantedSessions()
        {
            //Clear All Sessions that are not required

            Session["RiskManagement"] = null;

        }

        public ActionResult QueryView()
        {

            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }


        public ActionResult CommunDepartment()
        {
            LibraryModel mylib = new LibraryModel();
            DataTable dt = new DataTable();
            string strsql = "select qd_cd,qd_Name from qdepartments";
            dt = mylib.OpenDataTable(strsql);
            return View(dt);
        }
        public ActionResult Communication()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }

        }

        public ActionResult CommunicationReport(string strDepartment, string cmb)
        {
            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }

            modReports led = new modReports();
            DataTable dt = led.GetCommunicationREP(strDepartment, cmb);


            return View(dt);

        }
        public ActionResult GetCommData(string Id)
        {
            modReports objComm = new modReports();

            IEnumerable<CommMasterDetails> ulist = objComm.GetCommData(Id);
            return View(ulist);



        }

        public ActionResult GetcmdFaq(string id, string faqstring)
        {
            modReports objComm = new modReports();
            string StringMsg = objComm.GetcmdFaq(id, faqstring);
            return Content(StringMsg);
        }
        public ActionResult Getcmd_agree(string id)
        {
            modReports objComm = new modReports();
            string StringMsg = objComm.GetcmdAgree(id);
            return Content(StringMsg);
        }
        public FileResult ConfirmationReportDownload(string Name)
        {
            string strsql = "Select ea_filename,ea_document,ea_srno FROM ECircular_attach WHERE ea_srno = " + Name.Trim();
            LibraryModel myLib = new LibraryModel();
            DataTable dt = myLib.OpenDataTable(strsql);

            byte[] filecontent;
            string filename = "ABC.PDF";
            //filecontent = System.Text.Encoding.ASCII.GetBytes(Session["File1"].ToString());
            filecontent = (byte[])dt.Rows[0]["ea_document"];

            return File(filecontent, "application/pdf", filename);

        }
        public ActionResult LogoutPage()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty));
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetNoStore();
            //return View();
            //FormsAuthentication.RedirectToLoginPage();          
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml, string reporttittle, string reportname, string colwidth = null, string isCommex = "N")
        {

            UtilityDBModel myUTI = new UtilityDBModel();
            string LogoURL = myUTI.getlogoimageURL();
            string strCompanyName = "";
            if (isCommex == "Y")
            {
                strCompanyName = Convert.ToString(System.Web.HttpContext.Current.Session["strCommCompName"]).Trim();
            }
            else
            {
                strCompanyName = ConfigurationManager.AppSettings["OrganizationName"].ToString().Trim();
            }
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                //var header = iTextSharp.text.Image.GetInstance(Server.MapPath("../Img/Tradeplus.jpg"));
                //header.SetAbsolutePosition(0, 730); // X and Y Accroding to need
                //var footer = iTextSharp.text.Image.GetInstance(Server.MapPath("../Img/Tradeplus.jpg"));
                //footer.SetAbsolutePosition(0, 0); // X and Y Accroding to need
                string[] arr = reportname.Split(',');
                int PDFCount = 95;
                string Rname = arr[0];
                if (arr.Length > 1)
                {
                    PDFCount = PDFCount + Convert.ToInt32(arr[1]);
                }

                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Image addLogo = default(iTextSharp.text.Image);

                addLogo = iTextSharp.text.Image.GetInstance(Server.MapPath(LogoURL));
                addLogo.SetAbsolutePosition(25, 765); //(23, 1152);
                addLogo.ScaleAbsolute(300, 600);
                addLogo.ScaleToFit(700f, 40f);

                if (colwidth != null && colwidth != "")
                {
                    int sumcolwidth = 0;
                    string[] colarr = colwidth.Split(',');
                    int[] intcolarr = colarr.Select(int.Parse).ToArray();
                    sumcolwidth = intcolarr.Sum();
                    PDFCount = sumcolwidth + intcolarr.Length;
                    if (PDFCount < 68)
                    {
                        PDFCount = 68;
                    }
                }

                var pgSize = new iTextSharp.text.Rectangle(Convert.ToInt32(PDFCount * 8.75), 842);
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(pgSize, 5f, 10f, 20f, 5f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                //Paragraph p = new Paragraph((string)System.Configuration.ConfigurationManager.AppSettings["OrganizationName"].ToString().Trim());
                Paragraph p = new Paragraph(strCompanyName);
                Paragraph p2 = new Paragraph(WebUtility.HtmlDecode(reporttittle), contentFont);
                Paragraph p3 = new Paragraph(".");
                p.Alignment = Element.ALIGN_TOP;
                p.Alignment = Element.ALIGN_CENTER;
                p2.Alignment = Element.ALIGN_TOP;
                p2.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Open();

                pdfDoc.Add(p);
                pdfDoc.Add(p2);
                pdfDoc.Add(p3);
                pdfDoc.Add(addLogo);
                //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                //PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                //htmlparser.Parse(sr);

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();


                //addLogo.SetAbsolutePosition(2, 790);
                //addLogo.ScaleAbsolute(700, 400);
                //addLogo.ScaleToFit(700f, 40f);
                //StringReader sr = new StringReader(GridHtml);
                //Document pdfDoc = new Document(PageSize.A4, 5f, 10f, 20f, 5f);


                return File(stream.ToArray(), "application/pdf", reportname + ".pdf");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult BenHoldingPDF(string reporttittle, string reportname)
        {
            Int16 j;
            Int16 k;
            string[] arrAlign;
            int[] arrWdth;
            string[] arrVar;
            string[] arrTotal;
            string[] arrRound;
            bool blnGrpTotal;
            bool blnGrdTotal;
            string[] arrHdr1;
            string[] arrHdr2;
            string[] arrHdr3;
            int intTWidth = 0;
            double dblBal = 0;
            string strTmp;
            UtilityDBModel myUTI = new UtilityDBModel();
            string sHeader1 = "";
            string sHeader2 = "";
            string sHeader3 = "ISIN,ISIN Name,Holding,Rate,Value,%,Balance Type";
            string sColWdth = "10,18,7,7,8,5,9";
            string sColAlign = "Left,Left,Right,Right,Right,Right,Left";

            string sRound = "X,X,0,2,2,X,X";
            string gstrColTotal = "N,N,Y,N,G,N,N";
            //condition
            string gstrColVar = "Isin,Isin Name,Holding,Rate,Value,%,Balance Type";
            DataTable dtReport = (DataTable)Session["PDFTABLE"];
            DataTable dsComHeader = (DataTable)Session["ClientInfo"];
            arrRound = sRound.Split(',');
            arrWdth = sColWdth.Split(',').Select(int.Parse).ToArray();
            arrAlign = sColAlign.Split(',');
            arrVar = gstrColVar.Split(',');
            arrTotal = gstrColTotal.Split(',');

            iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font font = iTextSharp.text.FontFactory.GetFont("Arial sans-serif", 10, iTextSharp.text.Font.NORMAL);

            arrHdr1 = sHeader1.Split(',');
            arrHdr2 = sHeader2.Split(',');
            arrHdr3 = sHeader3.Split(',');
            blnGrpTotal = false;
            if (gstrColTotal.IndexOf("Y") != -1 || gstrColTotal.IndexOf("S") != -1)
            {
                blnGrpTotal = true;
            }
            blnGrdTotal = false;
            if (gstrColTotal.IndexOf("G") != -1)
            {
                blnGrdTotal = true;
            }
            Single[] ArrPDFWidth = new Single[arrVar.Length];
            intTWidth = 0;
            for (k = 0; k <= arrWdth.Length - 1; k++)
            {
                intTWidth += arrWdth[k] + 1;
            }
            double[] ArrGrpT = new double[arrVar.Length];
            double[] ArrGrdT = new double[arrVar.Length];

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                iTextSharp.text.Image addLogo = default(iTextSharp.text.Image);
                string LogoURL = myUTI.getlogoimageURL();
                addLogo = iTextSharp.text.Image.GetInstance(Server.MapPath(LogoURL));
                addLogo.SetAbsolutePosition(25, 765); //(23, 1152);
                addLogo.ScaleAbsolute(300, 600);
                addLogo.ScaleToFit(700f, 40f);
                Paragraph p = new Paragraph((string)System.Configuration.ConfigurationManager.AppSettings["OrganizationName"].ToString().Trim());
                Paragraph p2 = new Paragraph(reporttittle, contentFont);
                Paragraph p3 = new Paragraph(".");
                Paragraph p4 = new Paragraph("");
                p.Alignment = Element.ALIGN_TOP;
                p.Alignment = Element.ALIGN_CENTER;
                p2.Alignment = Element.ALIGN_TOP;
                p2.Alignment = Element.ALIGN_CENTER;
                Document PDFdoc = new Document(new iTextSharp.text.Rectangle((float)(intTWidth * 8.75), 842), 36, 36, 36, 36);
                PdfWriter PDFWrite = PdfWriter.GetInstance(PDFdoc, stream);
                PdfPCell cell;
                PdfPTable PdfTab = new PdfPTable(7);
                PDFdoc.Open();

                for (k = 0; k < arrWdth.Length; k++)
                {
                    ArrPDFWidth[k] = arrWdth[k] * 9;
                }
                PdfTab.SetTotalWidth(ArrPDFWidth);
                PdfTab.LockedWidth = true;
                PdfTab.HorizontalAlignment = Element.ALIGN_LEFT;
                string strTemp = "";

                strTemp = "Holding as on " + System.Web.HttpContext.Current.Session["Date"] + " Based On [" + System.Web.HttpContext.Current.Session["RateDate"] + "] Rate ";
                cell = new PdfPCell(new Phrase(strTemp, contentFont));
                cell.Colspan = arrVar.Length;
                cell.HorizontalAlignment = 1;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "  ";
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = arrVar.Length;
                cell.HorizontalAlignment = 1;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "Client       :   " + dsComHeader.Rows[0]["cm_name"].ToString().Trim() + " [ " + dsComHeader.Rows[0]["cm_cd"].ToString().Trim() + " ] ";
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = arrVar.Length;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "                    " + dsComHeader.Rows[0]["cm_add1"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = arrVar.Length - 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "Family          :   " + dsComHeader.Rows[0]["fm_desc"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "                    " + dsComHeader.Rows[0]["cm_add2"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = arrVar.Length - 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "Group           :   " + dsComHeader.Rows[0]["gr_desc"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "                    " + dsComHeader.Rows[0]["cm_add3"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = arrVar.Length - 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "Branch          :   " + dsComHeader.Rows[0]["bm_branchname"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                if (dsComHeader.Rows[0]["cm_cd"].ToString().Trim().Length > 8)
                {
                    strTemp = "                    " + dsComHeader.Rows[0]["cm_city"].ToString().Trim() + " " + dsComHeader.Rows[0]["cm_pin"].ToString().Trim();
                    cell = new PdfPCell(new Phrase(strTemp, font));
                    cell.Colspan = arrVar.Length - 4;
                    cell.HorizontalAlignment = 0;
                    cell.Border = 0;
                    PdfTab.AddCell(cell);
                }
                else
                {
                    strTemp = "                    " + dsComHeader.Rows[0]["cm_pin"].ToString().Trim();
                    cell = new PdfPCell(new Phrase(strTemp, font));
                    cell.Colspan = arrVar.Length - 4;
                    cell.HorizontalAlignment = 0;
                    cell.Border = 0;
                    PdfTab.AddCell(cell);
                }

                strTemp = "Scheme        :   " + dsComHeader.Rows[0]["cm_chgsscheme"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                if (dsComHeader.Rows[0]["cm_cd"].ToString().Trim().Length > 8)
                {
                    strTemp = "Tele         :   " + dsComHeader.Rows[0]["cm_tele1"].ToString().Trim() + ", " + dsComHeader.Rows[0]["cm_tele2"].ToString().Trim();
                    cell = new PdfPCell(new Phrase(strTemp, font));
                    cell.Colspan = arrVar.Length - 4;
                    cell.HorizontalAlignment = 0;
                    cell.Border = 0;
                    PdfTab.AddCell(cell);
                }
                else
                {
                    strTemp = "Tele         :   " + dsComHeader.Rows[0]["cm_tele1"].ToString().Trim();
                    cell = new PdfPCell(new Phrase(strTemp, font));
                    cell.Colspan = arrVar.Length - 4;
                    cell.HorizontalAlignment = 0;
                    cell.Border = 0;
                    PdfTab.AddCell(cell);
                }

                if (dsComHeader.Rows[0]["cm_cd"].ToString().Trim().Length > 8)
                {
                    strTemp = "Client Type   :   " + dsComHeader.Rows[0]["cm_clienttype"].ToString().Trim();
                    cell = new PdfPCell(new Phrase(strTemp, font));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = 0;
                    cell.Border = 0;
                    PdfTab.AddCell(cell);
                }
                else
                {
                    strTemp = "Client Type   :   " + dsComHeader.Rows[0]["cm_clienttype"].ToString().Trim();
                    cell = new PdfPCell(new Phrase(strTemp, font));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = 0;
                    cell.Border = 0;
                    PdfTab.AddCell(cell);
                }

                strTemp = "Joint(1)    :   " + dsComHeader.Rows[0]["cm_sech_name"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = arrVar.Length - 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "Status           :   " + dsComHeader.Rows[0]["bs_description"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                strTemp = "Joint(2)    :   " + dsComHeader.Rows[0]["cm_thih_name"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = arrVar.Length - 4;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                if (dsComHeader.Rows[0]["cm_cd"].ToString().Trim().Length > 8)
                {
                    if (dsComHeader.Rows[0]["cm_opendate"].ToString().Trim() != "")
                    {
                        strTemp = "Open Date    :   " + dsComHeader.Rows[0]["cm_opendate"].ToString().Trim();
                        cell = new PdfPCell(new Phrase(strTemp, font));
                        cell.Colspan = 4;
                        cell.HorizontalAlignment = 0;
                        cell.Border = 0;
                        PdfTab.AddCell(cell);
                    }
                }
                else
                {
                    if (dsComHeader.Rows[0]["cm_opendate"].ToString().Trim() != "")
                    {
                        strTemp = "Open Date    :   " + dsComHeader.Rows[0]["cm_opendate"].ToString().Trim();
                        cell = new PdfPCell(new Phrase(strTemp, font));
                        cell.Colspan = 4;
                        cell.HorizontalAlignment = 0;
                        cell.Border = 0;
                        PdfTab.AddCell(cell);
                    }
                }

                if (dsComHeader.Rows[0]["cm_cd"].ToString().Trim().Length > 8)
                {
                    if (dsComHeader.Rows[0]["cm_acc_closuredate"].ToString().Trim() != "")
                    {
                        strTemp = "Closure Date     :   " + dsComHeader.Rows[0]["cm_acc_closuredate"].ToString().Trim();
                        cell = new PdfPCell(new Phrase(strTemp, font));
                        cell.Colspan = arrVar.Length;
                        cell.HorizontalAlignment = 0;
                        cell.Border = 0;
                        PdfTab.AddCell(cell);
                    }
                }

                strTemp = "BackOffice Code : " + dsComHeader.Rows[0]["cm_blsavingcd"].ToString().Trim();
                cell = new PdfPCell(new Phrase(strTemp, font));
                cell.Colspan = arrVar.Length - 0;
                cell.HorizontalAlignment = 0;
                cell.Border = 0;
                PdfTab.AddCell(cell);

                if (Strings.RTrim(sHeader3) != "")
                {
                    for (int l = 0; l < arrHdr3.Length; l++)
                    {
                        strTemp = arrHdr3[l];
                        cell = new PdfPCell(new Phrase(strTemp, font));
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        if (Strings.Left(arrAlign[l], 1) == "L")
                        {
                            cell.HorizontalAlignment = 0;
                        }
                        else if (Strings.Left(arrAlign[l], 1) == "R")
                        {
                            cell.HorizontalAlignment = 2;
                        }
                        else if (Strings.Left(arrAlign[l], 1) == "C")
                        {
                            cell.HorizontalAlignment = 1;
                        }
                        PdfTab.AddCell(cell);
                    }
                }

                decimal valueSum = 0;
                decimal percentage = 0;
                for (int i = 0; i <= dtReport.Rows.Count - 1; i++)
                {
                    valueSum += Convert.ToDecimal(dtReport.Rows[i]["Value"]);
                }
                Boolean blnINF = false;
                for (int i = 0; i <= dtReport.Rows.Count - 1; i++)
                {
                    for (k = 0; k <= arrVar.Length - 1; k++)
                    {
                        if (arrRound[k] != "X")
                        {
                            if (Strings.Left(dtReport.Rows[i]["ISIN"].ToString(), 3) == "INF" && dtReport.Columns[arrVar[k]].ToString() == "Holding")
                            {
                                blnINF = true;
                                strTmp = dtReport.Rows[i][arrVar[k]].ToString();
                            }
                            else if (dtReport.Columns[arrVar[k]].ToString() == "Holding" && Convert.ToInt32(dtReport.Rows[i]["allowdecimal"]) > 0)
                            {
                                strTmp = String.Format("{0:0.00}", dtReport.Rows[i][arrVar[k]]);
                            }
                            else
                            {
                                strTmp = String.Format("{0:0.00}", dtReport.Rows[i][arrVar[k]]);
                            }
                        }
                        else
                        {
                            strTmp = dtReport.Rows[i][arrVar[k]].ToString();
                        }
                        if (k == 5)
                        {
                            percentage = (Convert.ToDecimal(dtReport.Rows[i]["Value"]) / valueSum) * 100;
                            strTmp = String.Format("{0:0.00}", percentage);
                        }
                        cell = new PdfPCell(new Phrase(strTmp, font));
                        if (Strings.Left(arrAlign[k], 1) == "L")
                        {
                            cell.HorizontalAlignment = 0;
                        }
                        else if (Strings.Left(arrAlign[k], 1) == "R")
                        {
                            cell.HorizontalAlignment = 2;
                        }
                        else if (Strings.Left(arrAlign[k], 1) == "C")
                        {
                            cell.HorizontalAlignment = 1;
                        }
                        PdfTab.AddCell(cell);
                        if (arrTotal[k] == "Y")
                        {
                            ArrGrpT[k] = ArrGrpT[k] + Convert.ToDouble(strTmp);
                        }
                        if (k == 3)
                        {
                            dblBal = dblBal + Convert.ToDouble(strTmp);
                        }
                        else if (k == 4)
                        {
                            dblBal = dblBal - Convert.ToDouble(strTmp);
                        }
                        if (arrTotal[k] == "G" || arrTotal[k] == "Y")
                        {
                            ArrGrdT[k] = ArrGrdT[k] + Convert.ToDouble(strTmp);
                        }
                    }
                }

                if (blnGrdTotal == true)
                {
                    for (k = 0; k <= arrTotal.Length - 1; k++)
                    {
                        if (k == 1)
                        {
                            strTmp = "Total";
                        }
                        else
                        {
                            if (arrTotal[k] == "G" || arrTotal[k] == "Y" && arrRound[k] != "X")
                            {
                                strTmp = String.Format("{0:0.00}", ArrGrdT[k]);
                                if (blnINF && dtReport.Columns[arrVar[k]].ToString() == "Holding")
                                {
                                    strTmp = String.Format("{0:0.00}", ArrGrdT[k]);
                                }
                            }
                            else
                            {
                                strTmp = "";
                            }
                        }
                        cell = new PdfPCell(new Phrase(strTmp, font));
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        if (Strings.Left(arrAlign[k], 1) == "L")
                        {
                            cell.HorizontalAlignment = 0;
                        }
                        else if (Strings.Left(arrAlign[k], 1) == "R")
                        {
                            cell.HorizontalAlignment = 2;
                        }
                        else if (Strings.Left(arrAlign[k], 1) == "C")
                        {
                            cell.HorizontalAlignment = 1;
                        }
                        PdfTab.AddCell(cell);
                    }
                }

                PDFdoc.Add(addLogo);
                PDFdoc.Add(p);
                PDFdoc.Add(p2);
                PDFdoc.Add(p3);
                PDFdoc.Add(PdfTab);
                PDFdoc.Close();
                return File(stream.ToArray(), "application/pdf", reportname + ".pdf");
            }
        }

        public ActionResult MailQuery()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }

        }
        public ActionResult getQueryReport(string ddlStat)
        {
            LibraryModel mylib = new LibraryModel();
            DataTable dt = new DataTable();
            string strsql = "select a.*,b.qd_name from msg_master a,QDepartments b where a.ms_initiator='" + this.Session["gstrUsercd"] + "' and a.ms_initflag='I' and a.ms_deptid=b.qd_cd and a.ms_msgstatus='" + ddlStat + "' order by convert(datetime,ms_date) desc,convert(datetime,ms_time) desc";
            dt = mylib.OpenDataTable(strsql);
            foreach (DataRow row in dt.Rows)
            {
                row["ms_date"] = myutil.DbToDate(row["ms_date"].ToString());
            }

            return View(dt);

        }

        public ActionResult getQueryReportDetail(string msgref)
        {
            LibraryModel mylib = new LibraryModel();
            DataTable dt1 = new DataTable();
            string strsql1 = "select a.*,b.um_user_name from msg_master a inner join  user_master b on um_user_id=a.ms_initiator where  a.ms_initiator='" + this.Session["gstrUsercd"] + "' and a.ms_interref='" + msgref + "'";
            dt1 = mylib.OpenDataTable(strsql1);
            if (dt1.Rows.Count > 0)
            {
                ViewBag.ms_msgstatus = dt1.Rows[0]["ms_msgstatus"].ToString();
                ViewBag.Date = myutil.DbToDate(dt1.Rows[0]["ms_date"].ToString());
                double Tdats = (myutil.stod(dt1.Rows[0]["ms_date"].ToString()) - DateTime.Today).TotalDays;
                ViewBag.daysdiffer = (-Tdats);
            }


            DataTable dt = new DataTable();
            string strsql = "select * from msg_submaster where mssub_msgref='" + msgref + "' order by convert(datetime,mssub_date) desc,convert(datetime,mssub_time) desc";
            dt = mylib.OpenDataTable(strsql);
            return View(dt);

        }
        [HttpPost]
        public JsonResult FillcmbDeptQueryMail()
        {
            DataTable dt = new DataTable();
            string strsql = "select qd_cd,qd_name from QDepartments where qd_active='Y'";

            dt = mylib.OpenDataTable(strsql);
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(dt);
            return Json(JSONresult);
        }

        public string SaveSendMsg()
        {
            string Msg = "";
            string inputType = System.Web.HttpContext.Current.Request["inputType"];
            string cmbDept = System.Web.HttpContext.Current.Request["cmbDept"];
            string cmbDeptVal = System.Web.HttpContext.Current.Request["cmbDeptVal"];
            string txtQuery = System.Web.HttpContext.Current.Request["txtQuery"];
            string strtxtSub = System.Web.HttpContext.Current.Request["txtSubject"];
            string msgref = System.Web.HttpContext.Current.Request["msgref"];
            string strsql = "";
            long lngRecno = 0L;
            int i, x;
            var dsTemp = new DataSet();
            string strVal;
            string msgNum;
            string fnGetTimeRet = default(string);

            string strSQL = "Select convert(char(8),getdate(),108) curtm";
            DataTable dtParm = mylib.OpenDataTable(strSQL);
            if (dtParm.Rows.Count > 0)
            {
                fnGetTimeRet = dtParm.Rows[0][0].ToString().Trim();
            }

            DataTable dt = new DataTable();
            try
            {
                if (inputType == "SaveNew")
                {
                    strsql = "select ms_interref from msg_master";
                    dt = mylib.OpenDataTable(strsql);
                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        if (lngRecno < Conversions.ToLong(Strings.Mid(dt.Rows[i][0].ToString(), 3)))
                        {
                            lngRecno = Conversions.ToLong(Strings.Mid(dt.Rows[i][0].ToString(), 3));
                        }
                    }

                    msgNum = lngRecno.ToString();
                    if (Strings.Len(msgNum) < 6)
                    {
                        i = 6 - Strings.Len(msgNum);
                        msgNum = "";
                        var loopTo1 = i;
                        for (x = 1; x <= loopTo1; x++)
                            msgNum = msgNum + "0";
                        strVal = "ms" + msgNum + (lngRecno + 1L).ToString();
                    }
                    else
                    {
                        strVal = (lngRecno + 1L).ToString();
                    }


                    strsql = "insert into msg_master values ('" + Session["gstrUsercd"] + " ','','" + cmbDept.Trim() + "','','P','" + strVal + "','" + cmbDeptVal.Trim() + "','I','" + Strings.Trim(strtxtSub) + "','N',1,'" + myutil.gstrDBToday() + "','" + fnGetTimeRet + "')";
                    mylib.ExecSQL(strsql);

                    strsql = "insert into msg_submaster values('" + strVal + "','" + Session["gstrUsercd"] + "','" + txtQuery.Trim() + "','I','N','" + myutil.gstrDBToday() + "','" + fnGetTimeRet + "')";
                    mylib.ExecSQL(strsql);
                }
                else if (inputType == "Reply")
                {
                    strsql = "insert into msg_submaster values('" + msgref.Trim() + "','" + Session["gstrUsercd"] + "','" + txtQuery.Trim() + "','I','N','" + myutil.gstrDBToday() + "','" + fnGetTimeRet + "')";
                    mylib.ExecSQL(strsql);
                    strsql = "update msg_master set ms_lastrplyby='" + Session["gstrUsercd"] + "',ms_readflag='N' where ms_interref='" + msgref.Trim() + "'";
                    mylib.ExecSQL(strsql);
                }

                Msg = "Query Recorded Successfully";
            }
            catch (Exception Ex)
            {
                Msg = Ex.Message;
            }

            return Msg;


        }

        public ActionResult ACHEntry()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }

        }

        public ActionResult getACHEntryReport()
        {
            LibraryModel mylib = new LibraryModel();
            DataTable dt = new DataTable();
            string strSql = "";
            strSql = "select Ach_Clientcd Code,cm_Name CName,ACH_IFSC IFSC,ACH_ActNumber BankActNo,";
            strSql += "Case ACH_Frequency When 'M' Then 'Monthly' when 'Q' then 'Quarterly' when 'H' then 'Half Yearly' when 'Y' then 'Yearly' Else 'Presentment' End  'Frequency',";
            strSql += " case ACH_Status when 'P' then 'Pending' when 'F' then 'Submitted' when 'A' then 'Accepted' when 'R' then 'Rejected' When 'C'  Then 'Cancelled' When 'Y' Then 'Authorise' When 'E' Then 'Exported' When 'T' Then 'To Be Cancel' Else '' end  'Status', convert(char,convert(datetime,ACH_ConsentDt),103) ACH_ConsentDt,ACH_Status";
            strSql += " from Client_master,ACH  where Ach_Clientcd=cm_cd and ACH_ConsentDt <> '' and ACH_Status <> 'C' and cm_schedule = '49843750'" + Session["LoginAccessOld"];
            strSql += " Order by convert(datetime,ACH_ConsentDt) desc";
            dt = mylib.OpenDataTable(strSql);
            return View(dt);

        }

        [HttpGet]
        public ActionResult getACHEntryClientDetail(string ClientCode)
        {
            string strSql = "";
            DataTable dt = new DataTable();
            ACHEntryClientdetail objclient = new ACHEntryClientdetail();
            List<FillActDtls> ulist = new List<FillActDtls>();

            if (Convert.ToInt32(mylib.fnFireQuery("ach", "count(0)", "ACH_ClientCd", ClientCode.Trim(), true)) == 0)
            {
                strSql = "Select cm_name,cm_mobile,cm_email from Client_Master where cm_cd='" + ClientCode.Trim() + "' and cm_schedule = '49843750'" + Session["LoginAccessOld"];
                dt = mylib.OpenDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    objclient.ClName = dt.Rows[0]["cm_name"].ToString();
                    objclient.Email = dt.Rows[0]["cm_email"].ToString();
                    objclient.Mob = dt.Rows[0]["cm_mobile"].ToString();
                    objclient.Message = "Success";

                    if (Convert.ToInt32(mylib.fnFireQuery("bankact", "count(0)", "ba_clientcd", ClientCode.Trim(), true)) == 0)
                    {
                        objclient.Message = "Bank Details Not Found";
                        objclient.ClName = "";
                        objclient.Email = "";
                        objclient.Mob = "";
                    }
                    else
                    {
                        strSql = "select 'Select' AC, 'Select' Dtl ";
                        strSql += " union all select distinct rtrim(ltrim(ba_micr)) +' / '+ rtrim(ltrim(ba_ifsccode)) +' / '+ rtrim(ltrim(ba_actno)) AC, ";
                        strSql += " rtrim(ltrim(ba_micr)) +'/'+ rtrim(ltrim(ba_ifsccode)) +'/'+ rtrim(ltrim(ba_actno)) Dtl ";
                        strSql += " from bankact,client_master where ba_clientcd=cm_cd and ba_clientcd='" + ClientCode.Trim() + "'";
                        strSql += " and len(ba_ifsccode)= 11 and len(ba_micr)= 9 and cm_schedule = '49843750'" + Session["LoginAccessOld"];
                        dt = mylib.OpenDataTable(strSql);
                        ulist = dt.AsEnumerable()
               .Select(row => new FillActDtls
               {
                   Id = row.Field<string>("AC").Trim(),
                   Name = row.Field<string>("Dtl").Trim(),
               }).ToList();
                    }

                    objclient.ActDtls = ulist;

                }
                else
                {
                    objclient.Message = " Client Not Found";
                    objclient.ClName = "";
                    objclient.Email = "";
                    objclient.Mob = "";

                }
            }
            else
            {
                objclient.Message = "Entry Already Exist For Client";
                objclient.ClName = "";
                objclient.Email = "";
                objclient.Mob = "";
            }
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(objclient);
            return Json(JSONresult, JsonRequestBehavior.AllowGet);
        }



        public ActionResult getBankDetailcmbACDtls(string ID)
        {
            string[] strcode = ID.Split('/');
            string ClName = "";
            ClName = mylib.fnFireQuery("bank_master", "bk_name", "bk_micr='" + strcode[0] + "' and bk_IFCCode", strcode[1], true);
            return Content(ClName);

        }

        public string SaveACHEntry()
        {
            string Msg = "";

            string ClientCode = System.Web.HttpContext.Current.Request["ClientCode"];
            string cmbMICR = System.Web.HttpContext.Current.Request["cmbMICR"];
            string cmbFrequencyVal = System.Web.HttpContext.Current.Request["cmbFrequencyVal"];
            string txtAmt = System.Web.HttpContext.Current.Request["txtAmt"];
            string RefNo = System.Web.HttpContext.Current.Request["RefNo"];
            string StrLmtType = System.Web.HttpContext.Current.Request["StrLmtType"];

            string strSql = "";
            long lngRecno = 0L;
            int i, x;
            var dsTemp = new DataSet();
            string strVal;
            string msgNum;
            string fnGetTimeRet = default(string);

            strSql = "delete from ach where ACH_ClientCd='" + ClientCode.Trim() + "' and ACH_Status='P'";
            mylib.ExecSQL(strSql);

            DataTable dt = new DataTable();
            try
            {
                string[] cmbMICR1 = cmbMICR.Split('/');


                string strSQL = "Select convert(char(8),getdate(),108) curtm";
                DataTable dtParm = mylib.OpenDataTable(strSQL);
                if (dtParm.Rows.Count > 0)
                {
                    fnGetTimeRet = dtParm.Rows[0][0].ToString().Trim();
                }

                strSql = "";
                strSql += "insert into ach (";
                strSql += "ACH_ClientCd,ACH_Segment,ACH_ConsentDt,ACH_MICR,ACH_IFSC,ACH_ActNumber,ACH_Frequency,ACH_LimitType,";
                strSql += "ACH_LimitAmount,ACH_ValidTill,ACH_Status,ACH_Filler1,ACH_Filler2,ACH_Filler3,ACH_Filler4,ACH_Filler5,ACH_Filler6,";
                strSql += "ACH_Filler7, ACH_Filler8, ACH_MKRID, ACH_MKRDT, ACH_ResUMNRno, ACH_ResRejectCD, ACH_ResRejectReason, ACH_InitRejectReason, ACH_ResProcessDT";
                strSql += ")";
                strSql += "select";
                strSql += "'" + ClientCode.Trim() + "' ACH_ClientCd,'E' ACH_Segment,'" + myutil.gstrDBToday() + "' ACH_ConsentDt,'" + cmbMICR1[0] + "' ACH_MICR,";
                strSql += "'" + cmbMICR1[1] + "' ACH_IFSC,'" + cmbMICR1[2] + "' ACH_ActNumber,'" + cmbFrequencyVal.Trim() + "' ACH_Frequency,'" + StrLmtType.Trim() + "' ACH_LimitType," + txtAmt.Trim() + " ACH_LimitAmount,";
                strSql += "'' ACH_ValidTill,'P' ACH_Status,0 ACH_Filler1,0 ACH_Filler2,0 ACH_Filler3,";
                strSql += "'' ACH_Filler4,'' ACH_Filler5,'" + RefNo.Trim() + "' ACH_Filler6,'' ACH_Filler7,'' ACH_Filler8,";
                strSql += "'" + Session["gstrUsercd"] + "' ACH_MKRID,'" + myutil.gstrDBToday() + "' ACH_MKRDT,'' ACH_ResUMNRno,'' ACH_ResRejectCD,'' ACH_ResRejectReason,";
                strSql += "'' ACH_InitRejectReason,'' ACH_ResProcessDT";
                mylib.ExecSQL(strSql);



                Msg = "Query Recorded Successfully";
            }
            catch (Exception Ex)
            {
                Msg = Ex.Message;
            }

            return Msg;


        }

        public string DeleteACHEntry()
        {
            string Msg = "";

            string ClientCode = System.Web.HttpContext.Current.Request["ClientCode"];

            string strSql = "";
            try
            {
                strSql = "delete from ach where ACH_ClientCd='" + ClientCode.Trim() + "' and ACH_Status='P'";
                mylib.ExecSQL(strSql);
                Msg = "Query Deleted Successfully";
            }
            catch (Exception Ex)
            {
                Msg = Ex.Message;
            }

            return Msg;


        }

        [HttpGet]
        public JsonResult fillMICRDDL(string ClientCode)
        {
            DataTable dt = new DataTable();
            ACHEntryClientdetail objclient = new ACHEntryClientdetail();
            List<FillActDtls> ulist = new List<FillActDtls>();
            string strSql = "select 'Select' AC, 'Select' Dtl ";
            strSql += " union all select distinct rtrim(ltrim(ba_micr)) +' / '+ rtrim(ltrim(ba_ifsccode)) +' / '+ rtrim(ltrim(ba_actno)) AC, ";
            strSql += " rtrim(ltrim(ba_micr)) +'/'+ rtrim(ltrim(ba_ifsccode)) +'/'+ rtrim(ltrim(ba_actno)) Dtl ";
            strSql += " from bankact,client_master where ba_clientcd=cm_cd and ba_clientcd='" + ClientCode.Trim() + "'";
            strSql += " and len(ba_ifsccode)= 11 and len(ba_micr)= 9 and cm_schedule = '49843750'" + Session["LoginAccessOld"];
            dt = mylib.OpenDataTable(strSql);
            ulist = dt.AsEnumerable()
            .Select(row => new FillActDtls
            {
                Id = row.Field<string>("AC").Trim(),
                Name = row.Field<string>("Dtl").Trim(),
            }).ToList();

            objclient.ActDtls = ulist;
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(objclient.ActDtls);
            return Json(JSONresult, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult getACHEntryDetailEdit(string ClientCode)
        {
            LibraryModel mylib = new LibraryModel();
            DataTable dt = new DataTable();
            string strSql = "";
            {
                strSql = "select Ach_Clientcd Code,cm_Name CName,ACH_IFSC IFSC,ACH_ActNumber BankActNo,";
                strSql += "Case ACH_Frequency When 'M' Then 'Monthly' when 'Q' then 'Quarterly' when 'H' then 'Half Yearly' when 'Y' then 'Yearly' Else 'Presentment' End  'Frequency',";
                strSql += " case ACH_Status when 'P' then 'Pending' when 'F' then 'Submitted' when 'A' then 'Accepted' when 'R' then 'Rejected' When 'C'  Then 'Cancelled' When 'Y' Then 'Authorise' When 'E' Then 'Exported' When 'T' Then 'To Be Cancel' Else '' end  'Status',";
                strSql += " ACH_MICR,ACH_Frequency,ACH_LimitType,ACH_LimitAmount,ACH_ValidTill,rtrim(ltrim(ACH_MICR))+'/'+rtrim(ltrim(ACH_IFSC)) +'/'+rtrim(ltrim(ACH_ActNumber))  ACDtls,ACH_Status,cm_email,cm_mobile,ACH_Filler6, convert(char,convert(datetime,ACH_ConsentDt),103) ACH_ConsentDt,ACH_ResUMNRno";
                strSql += " from Client_master,ACH  where Ach_Clientcd=cm_cd and cm_schedule = '49843750'" + Session["LoginAccessOld"];
                if (ClientCode.Trim() != "")
                {
                    strSql += " and Ach_Clientcd ='" + ClientCode.Trim() + "'";
                }
            }

            dt = mylib.OpenDataTable(strSql);
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(dt);
            return Json(JSONresult, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        public ActionResult IPO()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string strSql = "select count(0) From sysobjects Where name = 'IPOs'";
                DataTable dt = mylib.OpenDataTable(strSql);
                strSql = "select count(0) From sysobjects Where name = 'IPO_Application'";
                DataTable dt1 = mylib.OpenDataTable(strSql);
                if (int.Parse(dt.Rows[0][0].ToString()) > 0)
                {
                    if (int.Parse(dt1.Rows[0][0].ToString()) > 0)
                    {
                        strSql = "select IPO_Category,IPO_NSE_Symbol,IPO_Company_Name,convert(char,convert(date,IPO_Start_Date),103) as start_date,convert(char,convert(date,IPO_end_date),103) as end_date,convert(varchar,IPO_min_price) + ' - ' + convert(varchar,IPO_max_price) as price_range, ";
                        strSql += " IPO_min_order as min_order,IPO_Size as size,IPO_Tick_Size as tick_size,IPO_RHP_Link as rhp, case when convert(date,IPO_Start_Date,103) > CONVERT(date, getdate()) then 'Upcoming' when convert(date,IPO_Start_Date,103) <= CONVERT(date, getdate()) and convert(date,IPO_End_Date,103) >= CONVERT(date, getdate()) then 'Apply' else '' end as 'status' ";
                        strSql += " from IPOs where CONVERT(date, getdate()) <= IPO_End_Date and IPO_Category='IND' order by IPO_Start_Date asc ";
                        dt = mylib.OpenDataTable(strSql);
                        return View(dt);
                    }
                    else
                    {
                        return View(dt1);
                    }
                }
                else
                {
                    return View(dt);
                }
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        [Authorize]
        public ActionResult setIPOSession(string strIPO)
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Session["IPOScrip"] = strIPO;
                return this.Json(new { success = true });
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        [Authorize]
        public ActionResult IPOApplication()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string strIPO = "";
                if (Session["IPOScrip"] != null)
                {
                    strIPO = Session["IPOScrip"].ToString().Trim();
                }
                else
                {
                    return View();
                }
                string strSql = "select IPO_Company_Name,IPO_NSE_Symbol,IPO_Category,cast((IPO_min_price) as decimal(15,2)) as IPO_min_price,cast((IPO_max_price) as decimal(15,2)) as IPO_max_price, ";
                strSql += " convert(varchar,cast((IPO_min_price) as decimal(15,0))) + ' - ' + convert(varchar,cast((IPO_max_price) as decimal(15,0))) as price_range, IPO_min_order, IPO_tick_size, cast((IPO_discount) as decimal(15,2)) as IPO_discount ";
                strSql += " from IPOs where IPO_NSE_Symbol='" + strIPO + "' and IPO_Category='IND' order by IPO_Start_Date asc ";
                DataTable dt = mylib.OpenDataTable(strSql);
                return View(dt);
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        [HttpPost]
        public async Task<JsonResult> ApplyIPO(string strIPO, IEnumerable<IPODetails> clients, IEnumerable<IPOBids> ipobids)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            DataTable dt = new DataTable();
            modIPO ipo = new modIPO();
            List<IPOResponse> iporesponse = new List<IPOResponse>();
            string strSql = "";
            string strCred = mylib.fnFireQuery("sysparameter", "sp_sysvalue", "sp_parmcd", "BSEIPO", false);
            string strURL1 = mylib.fnFireQuery("sysparameter", "sp_sysvalue", "sp_parmcd", "BSEIPOURL1", false);
            string strURL2 = mylib.fnFireQuery("sysparameter", "sp_sysvalue", "sp_parmcd", "BSEIPOURL2", false);
            string strName = "";
            string strDematNo = "";
            string strPanno = "";
            string strUPIID = "";
            string strClientCd = "";
            string strMemberCode = "";
            string strLoginID = "";
            string strPassword = "";
            string strIBBSID = "";
            string strKey = "";
            string strJson = "";
            string strResponse = "";
            string[] arrCred = new string[5];
            string strAppno = "";
            var strToken = "";
            if (strCred.Trim() != "")
            {
                arrCred = strCred.Split('/');
                strMemberCode = arrCred[0].ToString();
                strLoginID = arrCred[1].ToString();
                strPassword = arrCred[2].ToString();
                strIBBSID = arrCred[3].ToString();
                strKey = arrCred[4].ToString();
            }
            else
            {
                iporesponse.Add(new IPOResponse { SrNo = "", Client = "", DematNo = "", Response = "Invalid Credentials" });
                return Json(iporesponse, JsonRequestBehavior.AllowGet);
            }

            string strQty = ipobids.ElementAt(0).Quantity.ToString();
            Boolean chkCutoff = ipobids.ElementAt(0).Cutoff;
            string strPrice = ipobids.ElementAt(0).Price.ToString();
            if (strQty.Trim() == "" || strPrice.Trim() == "")
            {
                iporesponse.Add(new IPOResponse { SrNo = "", Client = "", DematNo = "", Response = "Quantity/Price cannot be blank" });
                return Json(iporesponse, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int intMinOrder = Convert.ToInt32(mylib.fnFireQuery("IPOs", "IPO_Min_order", "IPO_NSE_Symbol", strIPO.Trim(), false));
                if (Convert.ToInt32(strQty.Trim()) % intMinOrder != 0)
                {
                    iporesponse.Add(new IPOResponse { SrNo = "", Client = "", DematNo = "", Response = "Enter valid Quantity" });
                    return Json(iporesponse, JsonRequestBehavior.AllowGet);
                }
            }

            List<OrderBidDetails> listBidDetails1 = new List<OrderBidDetails>();
            var orderno = 1;
            foreach (var biditem in ipobids)
            {
                if (biditem.Quantity.Trim() != "" && biditem.Price.Trim() != "")
                {
                    if (biditem.Quantity.Trim() == "" || biditem.Price.Trim() == "")
                    {
                        iporesponse.Add(new IPOResponse { SrNo = "", Client = "", DematNo = "", Response = "Quantity/Price cannot be blank" });
                        return Json(iporesponse, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        int intMinOrder = Convert.ToInt32(mylib.fnFireQuery("IPOs", "IPO_Min_order", "IPO_NSE_Symbol", strIPO.Trim(), false));
                        if (Convert.ToInt32(biditem.Quantity.Trim()) % intMinOrder != 0)
                        {
                            iporesponse.Add(new IPOResponse { SrNo = "", Client = "", DematNo = "", Response = "Enter valid Quantity" });
                            return Json(iporesponse, JsonRequestBehavior.AllowGet);
                        }
                    }
                    listBidDetails1.Add(new OrderBidDetails { bidid = "", quantity = biditem.Quantity.Trim(), rate = biditem.Price.Trim(), cuttoffflag = (biditem.Cutoff == true ? "1" : "0"), orderno = orderno.ToString(), actioncode = "n" });
                    orderno++;
                }
            }

            LoginRequest login = new LoginRequest();
            login.membercode = strMemberCode;
            login.loginid = strLoginID;
            login.password = strPassword;
            login.ibbsid = strIBBSID;
            strJson = JsonConvert.SerializeObject(login);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(strURL1);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var result = await client.PostAsync("login", new StringContent(strJson, Encoding.UTF8, "application/json"));
                    if (result.IsSuccessStatusCode)
                    {
                        strResponse = await result.Content.ReadAsStringAsync();
                        if (JObject.Parse(strResponse)["errorcode"].ToString().Trim() == "0")
                        {
                            strToken = JObject.Parse(strResponse)["token"].ToString();
                        }
                        else
                        {
                            iporesponse.Add(new IPOResponse { SrNo = "", Client = "", DematNo = "", Response = JObject.Parse(strResponse)["message"].ToString() });
                            return Json(iporesponse, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        iporesponse.Add(new IPOResponse { SrNo = "", Client = "", DematNo = "", Response = await result.Content.ReadAsStringAsync() });
                        return Json(iporesponse, JsonRequestBehavior.AllowGet);
                    }
                }

                var counter = 0;
                foreach (var items in clients)
                {
                    counter++;
                    if (items.UPIID.Trim() == "")
                    {
                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = items.ClientCode, DematNo = items.DematNo, Response = "UPI ID cannot be blank" });
                        continue;
                    }
                    if (items.UPIID.Contains("@") == false)
                    {
                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = items.ClientCode, DematNo = items.DematNo, Response = "Enter Valid UPI ID" });
                        continue;
                    }
                    if (items.DematNo.Trim().Length != 16)
                    {
                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = items.ClientCode, DematNo = items.DematNo, Response = "Enter Valid Demat Ac No." });
                        continue;
                    }
                    if (mylib.fnFireQuery("Client_master", "cm_panno", "cm_cd", items.ClientCode, false).Trim() == "")
                    {
                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = items.ClientCode, DematNo = items.DematNo, Response = "Pan No. cannot be blank" });
                        continue;
                    }
                    if (Convert.ToInt32(mylib.fnFireQuery("IPO_Application", "count(0)", "IPA_ScripID = '" + strIPO.Trim() + "' and IPA_ClientCd", items.ClientCode.Trim(), true)) > 0)
                    {
                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = items.ClientCode, DematNo = items.DematNo, Response = "IPO Application already exists" });
                        continue;
                    }
                }

                if (iporesponse != null && iporesponse.Any())
                {
                    return Json(iporesponse, JsonRequestBehavior.AllowGet);
                }

                counter = 0;
                foreach (var item in clients)
                {
                    counter++;
                    strClientCd = item.ClientCode.Trim();
                    strName = mylib.fnFireQuery("Client_master", "cm_name", "cm_cd", strClientCd, false);
                    strDematNo = item.DematNo.Trim();
                    strPanno = mylib.fnFireQuery("Client_master", "cm_panno", "cm_cd", strClientCd, false);
                    strUPIID = item.UPIID.Trim();

                    string strDep = "";
                    string strDPID = "";
                    string strBenID = "";
                    if (strDematNo.Trim().Length == 16)
                    {
                        if (strDematNo.Substring(0, 2) == "IN")
                        {
                            strDep = "NSDL";
                            strDPID = strDematNo.Substring(0, 8);
                            strBenID = strDematNo.Substring(9, 16);
                        }
                        else
                        {
                            strDep = "CDSL";
                            strDPID = "0";
                            strBenID = strDematNo;
                        }
                    }
                    else
                    {
                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = strClientCd, DematNo = strDematNo, Response = "Enter Proper Demat Ac No." });
                        continue;
                    }
                    strName = System.Text.RegularExpressions.Regex.Replace(strName.Trim(), @"[ ](?=[ ])|[^-_,A-Za-z0-9 ]+", " ");

                    List<BidDetails> listBidDetails = new List<BidDetails>();
                    listBidDetails.Add(new BidDetails { quantity = "", rate = "", cuttoffflag = "" });
                    listBidDetails.Add(new BidDetails { quantity = "", rate = "", cuttoffflag = "" });
                    listBidDetails.Add(new BidDetails { quantity = "", rate = "", cuttoffflag = "" });
                    IPOApplication application = new IPOApplication { };
                    application.scripid = strIPO.Trim();
                    application.clientname = "";
                    application.panno = "";
                    application.depository = "";
                    application.dpid = "";
                    application.clientbenfid = "";
                    application.bankname = "";
                    application.bankbranch = "";
                    application.bankcode = "";
                    application.location = "";
                    application.ifsccode = "";
                    application.accountnumber = "";
                    application.filler1 = "";
                    application.category = "IND";
                    application.address = "";
                    application.email = "";
                    application.contact = "";
                    application.bidtype = "2";
                    application.filler2 = "";
                    application.syndicatemembercode = "";
                    application.brokercode = "";
                    application.subbrokercode = "";
                    application.referencefield = "";
                    application.repartitiontype = "nr";
                    application.orderno = "";
                    application.bids = listBidDetails;
                    strJson = JsonConvert.SerializeObject(application);

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(strURL1);
                        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, strURL1 + "eforms"))
                        {
                            requestMessage.Headers.Add("Checksum", ipo.encrypt(strJson, strKey));
                            requestMessage.Headers.Add("Token", strToken);
                            requestMessage.Headers.Add("Membercode", strMemberCode);
                            requestMessage.Headers.Add("Login", strLoginID);
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            requestMessage.Content = new StringContent(strJson, Encoding.UTF8, "application/json");
                            var result = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                            if (result.IsSuccessStatusCode)
                            {
                                strResponse = await result.Content.ReadAsStringAsync();
                                if (JObject.Parse(strResponse)["errorcode"].ToString().Trim() == "0")
                                {
                                    strAppno = JObject.Parse(strResponse)["applicationno"].ToString().Trim();
                                }
                                else
                                {
                                    iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = strClientCd, DematNo = strDematNo, Response = JObject.Parse(strResponse)["message"].ToString().Trim() });
                                    continue;
                                }
                            }
                            else
                            {
                                iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = strClientCd, DematNo = strDematNo, Response = result.ReasonPhrase });
                                continue;
                            }
                        }
                    }

                    IPOOrder order = new IPOOrder { };
                    order.scripid = strIPO.Trim();
                    order.applicationno = strAppno;
                    order.category = "IND";
                    order.applicantname = strName;
                    order.depository = strDep;
                    order.dpid = strDPID;
                    order.clientbenfid = strBenID;
                    order.chequereceivedflag = "n";
                    order.chequeamount = "0";
                    order.panno = strPanno;
                    order.bankname = "8888";
                    order.location = "upiidl";
                    order.accountnumber_upiid = strUPIID;
                    order.ifsccode = "";
                    order.referenceno = "";
                    order.asba_upiid = "1";
                    order.bids = listBidDetails1;
                    strJson = JsonConvert.SerializeObject(order);
                    using (var client = new HttpClient())
                    {
                        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, strURL2 + "ipoorder"))
                        {
                            requestMessage.Headers.Add("Checksum", ipo.encrypt(strJson, strKey));
                            requestMessage.Headers.Add("Token", strToken);
                            requestMessage.Headers.Add("Membercode", strMemberCode);
                            requestMessage.Headers.Add("Login", strLoginID);
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            requestMessage.Content = new StringContent(strJson, Encoding.UTF8, "application/json");
                            var result = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                            if (result.IsSuccessStatusCode)
                            {
                                strResponse = await result.Content.ReadAsStringAsync();
                                if (JObject.Parse(strResponse)["statuscode"] != null)
                                {
                                    if (JObject.Parse(strResponse)["statuscode"].ToString().Trim() != "0")
                                    {
                                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = strClientCd, DematNo = strDematNo, Response = JObject.Parse(strResponse)["statusmessage"].ToString().Trim() });
                                        continue;
                                    }
                                }
                                if (JObject.Parse(strResponse)["errorcode"] != null)
                                {
                                    if (JObject.Parse(strResponse)["errorcode"].ToString().Trim() != "0")
                                    {
                                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = strClientCd, DematNo = strDematNo, Response = JObject.Parse(strResponse)["errormessage"].ToString().Trim() });
                                        continue;
                                    }
                                }
                                IPOOrderResponse IOR = JsonConvert.DeserializeObject<IPOOrderResponse>(strResponse);
                                var items = IOR.bids;
                                string strMsg = "Application No. = " + strAppno + " ";
                                Boolean blnAccept = false;
                                if (IOR.statuscode == "0")
                                {
                                    foreach (var bid in items)
                                    {
                                        if (bid.errorcode == "0")
                                        {
                                            blnAccept = true;
                                        }
                                        strMsg += "Bid 1 = " + bid.message + " ";
                                    }
                                    if (blnAccept == true)
                                    {
                                        strSql = "insert into IPO_Application values('" + strClientCd + "','" + strIPO + "','" + strAppno + "','" + strDPID + "','" + strBenID + "','" + strUPIID + "','','','','','','0.00','','','','" + myutil.dtos(System.DateTime.Today.Date.ToString()) + "','" + DateTime.Now.ToString("HH:mm:ss") + "','','','','0','0','0')";
                                        mylib.ExecSQL(strSql);
                                    }
                                    else
                                    {
                                        iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = strClientCd, DematNo = strDematNo, Response = strMsg });
                                        continue;
                                    }
                                }
                                else
                                {
                                    strMsg = IOR.statusmessage;
                                }
                                iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = strClientCd, DematNo = strDematNo, Response = strMsg });
                            }
                            else
                            {
                                iporesponse.Add(new IPOResponse { SrNo = counter.ToString(), Client = strClientCd, DematNo = strDematNo, Response = result.ReasonPhrase });
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(iporesponse, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillComboExchangeSegmentFO()
        {
            UtilityDBModel mydbutil = new UtilityDBModel();
            LibraryModel mylib = new LibraryModel();
            string StrSql = "";
            StrSql = "Select CES_Cd,CES_Exchange+'/'+CES_Segment ExchSeg from CompanyExchangeSegments Where CES_CompanyCd = '" + this.Session["CompanyCode"] + "' ";
            StrSql += " and Right(CES_Cd,1) in ('F','K','X')";
            List<JsonComboModel> ulist = new List<JsonComboModel>();

            DataTable dtRMS = mylib.OpenDataTable(StrSql);
            ulist = dtRMS.AsEnumerable()
            .Select(row => new JsonComboModel
            {
                Display = row.Field<string>("ExchSeg").Trim(),
                Value = row.Field<string>("CES_Cd").Trim(),
            }).ToList();
            var result = ulist;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #region style methods
        public static List<ColumnStyle> RiskData()
        {
            // fixed first two columns
            // sub total: columnname and group column name
            // gran total: columnname
            return new List<ColumnStyle>()
            {
               new ColumnStyle {ColumnName = "bm_branchcd", HeaderName="Branch", Alignment = HorizontalAlignment.Left, Width=80, Visible=true},
               new ColumnStyle {ColumnName = "bm_branchname", HeaderName="Branch Name", Alignment = HorizontalAlignment.Left, Width=120, Visible=true},
               new ColumnStyle { ColumnName="Code", Width=80, Alignment=HorizontalAlignment.Left },
               new ColumnStyle { ColumnName="Name", Alignment=HorizontalAlignment.Left,  Width =180, IsGroup = false },
               new ColumnStyle { ColumnName="TDAY", IsSum=true, Alignment=HorizontalAlignment.Right, Width = 100 },
               new ColumnStyle { ColumnName="T2DAY", Width=100, Visible=true, Alignment=HorizontalAlignment.Right, PopupUrl="/Tplus/RMSLink?CCode={{Col:Code}}&CName={{Col:Name}}&code={{Col:Code}}&curyear={{Fun:getcurrentyear}}" },
               new ColumnStyle { ColumnName="UnCleared", Width=120, Visible =true, Alignment=HorizontalAlignment.Right },
               //new ColumnStyle { ColumnName="ApprovedShares", Width=140, Visible =true, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="CashDeposit", Width= 120, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="ShareCollateral",Width=130, Visible=true, Alignment=HorizontalAlignment.Right, PopupUrl="/Tplus/ApprovedShareRMS?CCode={{Col:Code}}&CName={{Col:Name}}&CId={{Col:Code}}"},
               new ColumnStyle { ColumnName="Margin", Width=100, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="Pool", Width=100, Alignment=HorizontalAlignment.Right},
               new ColumnStyle { ColumnName="DPHolding", Width=100, Visible=true, Alignment=HorizontalAlignment.Right, PopupUrl="/Tplus/DpHolding?CCode={{Col:Code}}&CName={{Col:Name}}&CId={{Col:Code}}" },
               new ColumnStyle { ColumnName="Stock", Width=100, Alignment=HorizontalAlignment.Right, PopupUrl="/Tplus/StockRMS?CCode={{Col:Code}}&CName={{Col:Name}}&CId={{Col:Code}}" },
               new ColumnStyle { ColumnName="Net", Width = 100, Alignment=HorizontalAlignment.Right, IsSum=true  },
               new ColumnStyle { ColumnName="Abovedays", Width=100, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="Collection", Width=100, Visible=true, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="ActualRisk", Width=100, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="FUNDPAYOUT", Width=110, Alignment=HorizontalAlignment.Right},
               new ColumnStyle { ColumnName="PROJECTEDRISK", Width=120, Visible=true, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="SHAREPAYOUT", Width=110, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="fomargin", Width=100, Alignment=HorizontalAlignment.Right},
               new ColumnStyle { ColumnName="StockBH", Width=100, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="ProvInt", Width=100, Alignment=HorizontalAlignment.Right},
               new ColumnStyle { ColumnName="AvailMargin", Width=100, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="OptionM2M", Width=100, Alignment=HorizontalAlignment.Right},
               new ColumnStyle { ColumnName="DebitOlder5Days", Width=120, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="RMSLIMIT", Width=100, Alignment=HorizontalAlignment.Right},
               new ColumnStyle { ColumnName="StockAH", Width=100, Alignment=HorizontalAlignment.Right },
               new ColumnStyle { ColumnName="ExchApprstk", Width=100, Alignment=HorizontalAlignment.Right},
            };
        }
        #endregion

    }
}




