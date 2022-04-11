
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRADENET.Models;

namespace TRADENET.Controllers
{
    public class DPServiceController : Controller
    {
        // GET: DPService
        public ActionResult DPHolding()
        {
            string strDPs;
            LibraryModel mylib = new LibraryModel(true);
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ToString();
            {
                con.Open();
                strDPs = Strings.Trim(mylib.fnFireQuery("other_products", "sum(case op_product when 'Cross' then 1 else 2 end)", "op_product in ('Cross','Estro') and op_status", "A", true, con));
                con.Close();
            }
          //  strDPs = "0";
            Boolean SB = false;
            if (Strings.InStr(1, "23", strDPs) > 0)
            {
                SB = true;

            }
            else
            {
                SB = false;
            }
            ViewBag.strDPs = SB;
            return View();
        }
        public ActionResult DPHoldingReport(string Code, string cmbRep, string bsecode, string nsesymbol, string isincode, string Ckekclient, string CkekSecurity, string AsOn, string PrpFromDate, string holdingtype)
        {
            ViewBag.Ckekclient = Ckekclient;
            ViewBag.CkekSecurity = CkekSecurity;
            DPService dp = new DPService();
            LibraryModel mylib = new LibraryModel(true);
            SqlConnection con = new SqlConnection();
            string strDPs = "";
            con.ConnectionString = ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ToString();
            {
                con.Open();
                strDPs = Strings.Trim(mylib.fnFireQuery("other_products", "sum(case op_product when 'Cross' then 1 else 2 end)", "op_product in ('Cross','Estro') and op_status", "A", true, con));

                con.Close();
            }
            string StrHldType = "";

            string[] holdingtype1 = holdingtype.Split(',');


            short i = 0;
            if (holdingtype1.Length > 0)
            {
                for (i = 0; i <= holdingtype1.Length - 1; i++)
                {

                    if (holdingtype1[i].ToString().ToUpper().Trim() == "FREE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "'21','22',", "'10','11','20','30',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "PLEDGE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "'29',", "'14',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "DEMAT")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "'24',", "'12',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "LOCK IN")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'51',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "REMAT")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "'23',", "'13',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "PLEDGE SETUP")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'61',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "FROZED BALANCE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'50',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "PLEDGEE BALANCE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'62',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "RE-PLEDGE BALANCE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'63',");

                }
            }

            StrHldType = (StrHldType.Trim() != "" ? Strings.Left(StrHldType, StrHldType.Length - 1) : "");

            if (ViewBag.CkekSecurity == "1")
            {
                string strIsin = dp.GetISINDetails(isincode);
                ViewBag.ReportTitle = " DP Holding for " + strIsin.Trim() + " [" + isincode + "]";
            }

            DataTable ulist = dp.GetDPHoldingReport(Code, cmbRep, bsecode, nsesymbol, isincode, Ckekclient, CkekSecurity, AsOn, PrpFromDate, StrHldType);
            return View(ulist);
        }

        //url: '@Url.Action("GetBeneficiaryHolding", "Tplus")',
        public ActionResult GetBeneficiaryHolding(string Code, string BIOCode, string chkAsOn, string PrpFromDate, string ObjLBlName,string holdingtype)
        {
            string NewLn = Environment.NewLine;
            DPService hold = new DPService();
            LibraryModel mylib = new LibraryModel(true);
            SqlConnection con = new SqlConnection();
            string strDPs = "";
            con.ConnectionString = ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ToString();
            {
                con.Open();
                strDPs = Strings.Trim(mylib.fnFireQuery("other_products", "sum(case op_product when 'Cross' then 1 else 2 end)", "op_product in ('Cross','Estro') and op_status", "A", true, con));

                con.Close();
            }
            string StrHldType = "";

            string[] holdingtype1 = holdingtype.Split(',');


            short i = 0;
            if (holdingtype1.Length > 0)
            {
                for (i = 0; i <= holdingtype1.Length - 1; i++)
                {

                    if (holdingtype1[i].ToString().ToUpper().Trim() == "FREE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "'21','22',", "'10','11','20','30',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "PLEDGE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "'29',", "'14',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "DEMAT")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "'24',", "'12',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "LOCK IN")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'51',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "REMAT")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "'23',", "'13',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "PLEDGE SETUP")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'61',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "FROZED BALANCE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'50',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "PLEDGEE BALANCE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'62',");
                    if (holdingtype1[i].ToString().ToUpper().Trim() == "RE-PLEDGE BALANCE")
                        StrHldType = StrHldType + Interaction.IIf(Strings.InStr(1, "23", strDPs) > 0, "", "'63',");

                }
            }

            StrHldType = (StrHldType.Trim() != "" ? Strings.Left(StrHldType, StrHldType.Length - 1) : "");

            //IEnumerable<DPHoldingModel> dp = hold.GetBenHoldingService(Code, BIOCode, chkAsOn, PrpFromDate, ObjLBlName);
            DataTable ulist = hold.GetBenHoldingService(Code, BIOCode, chkAsOn, PrpFromDate, ObjLBlName, StrHldType);
            ModelState.Clear();
            return View(ulist);
        }

        public ActionResult TransactionDtl(string Code, string BIOCode)
        {
            return View();
        }

        public ActionResult TransactionStatement()
        {
            return View();
        }

        public ActionResult TransactionStatementReport(string Code, string FromDate, string ToDate, string cmbRep)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            DPService dp = new DPService();
            DataTable ulist = dp.GetTransactionStatementReport(Code, FromDate, ToDate, cmbRep);
            return View(ulist);
        }

        public ActionResult NextAMCDue()
        {
            return View();

        }

        public ActionResult NextAMCDueReport(string Code, string cmbRep)
        {
            DPService dp = new DPService();
            DataTable ulist = dp.GetNextAMCDueReport(Code, cmbRep);
            return View(ulist);
        }


        public ActionResult CrossLedger()
        {
            return View();
        }

        public ActionResult CrossLedgerReport(string Code, string ChkListOfBal, string FromDate, string ToDate, string cmbRep, string DDlOnlyBal)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ChkListOfBal = ChkListOfBal;
            DPService dp = new DPService();
            DataTable ulist = dp.GetCrossLedgerReport(Code, ChkListOfBal, FromDate, ToDate, cmbRep, DDlOnlyBal);
            return View(ulist);
        }

        public ActionResult PledgeRequest()
        {
            return View();
        }

        public ActionResult PledgeRequestReport(string code, string cmbSelect)
        {
            DPService dp = new DPService();
            DataTable ulist = dp.GetPledgeRequestReport(code, cmbSelect);
            return View(ulist);

        }

        public ActionResult GetPledgeRequest(string code, string ClientCode)
        {

            Session["code"] = code;
            Session["ClientCode"] = ClientCode;
            DataTable DsPledgeReq = new DataTable();
            DataTable Dt = new DataTable();

            DsPledgeReq = (DataTable)Session["DtlReport"];

            if (DsPledgeReq.Rows.Count > 0)
            {
                var dr = DsPledgeReq.Select(string.Format("th_DematActNo ='{0}'", code.ToString().Trim()));
                //var dr = DsPledgeReq.Select("th_DematActNo = " + code.Trim());
                //var dvPledge = new DataView(DsPledgeReq);
                //dvPledge.RowFilter = "th_DematActNo='" + code.Trim() + "'";
                Dt = dr.CopyToDataTable();
            }
            else
            {

            }
            return View(Dt);

        }
        public ActionResult SavePledgeRequest(IEnumerable<BulkPledgeRequestModel> BulkPledgeRequest)
        {

            DPService SavePR = new DPService();

            SavePR.SavePledgeRequest(BulkPledgeRequest);
            return Content("Record Saved");



        }

        public ActionResult BuyBackOffer()
        {

            return View();
        }


        public ActionResult buybackSeurity()
        {

            var freeze = Seurity();
            return Json(freeze, JsonRequestBehavior.AllowGet);
        }
        public IEnumerable<JsonComboModel> Seurity()
        {
            LibraryModel lib = new LibraryModel();
            string strSql = "";
            string msg = "";
            List<JsonComboModel> ulist = new List<JsonComboModel>();

            strSql = " select distinct ltrim(rtrim(ss_name))  +' ['+ ltrim(rtrim(BB_ScripCd)) +'] / ' + ltrim(rtrim(BB_Stlmnt)) + case when se_stdt < '" + DateTime.Now.ToString("yyyyMMdd") + "' Then ' (Settled)' when Right(BB_DematAct,8) < '" + DateTime.Now.ToString("yyyyMMdd") + "' Then ' (Closed)' else '' end ScripNm ,";
            strSql += " ltrim(rtrim(BB_ScripCd)) + '/' + ltrim(rtrim(BB_Stlmnt)) ScripCd,se_stdt,BB_ScripCd ";
            strSql += " from BuyBacks,Settlements,Securities Where ss_cd = BB_ScripCd and BB_Stlmnt = se_stlmnt and se_endt >= '" + DateTime.Now.ToString("yyyyMMdd") + "' and BB_clientcd='BUYBACK!'  order by se_stdt desc ,BB_ScripCd";
            DataTable dt = lib.OpenDataTable(strSql);


            ulist = dt.AsEnumerable()
            .Select(row => new JsonComboModel
            {
                Display = row.Field<string>("ScripNm").Trim(),
                Value = row.Field<string>("ScripCd").Trim(),
            }).ToList();


            return ulist;

        }



        public ActionResult setvaluesSeurity(string CmbSecurity)
        {
            LibraryModel lib = new LibraryModel();
            UtilityModel util = new UtilityModel();
            string strSql = "";
            List<buybackofferModel> ulist = new List<buybackofferModel>();


            string[] arrScr = null;

            arrScr = CmbSecurity.Split('/');

            strSql = " select se_stlmnt,BB_Rate,se_stdt,se_endt,se_payindt,ss_bsymbol,ss_nsymbol,BB_DematAct,se_payoutdt,se_exchange from BuyBacks,Settlements,Securities Where  ss_cd = BB_ScripCd and BB_Stlmnt = se_stlmnt and  BB_ScripCd ='" + arrScr[0] + "' and BB_Stlmnt = '" + arrScr[1] + "' and BB_clientcd='BUYBACK!' ";
            DataTable DtSecurityData = lib.OpenDataTable(strSql);
            ulist = DtSecurityData.AsEnumerable()
                       .Select(row => new buybackofferModel
                       {
                           se_stlmnt = row.Field<string>("se_stlmnt").Trim(),
                           BB_Rate = row.Field<decimal>("BB_Rate"),
                           BB_DematAct = row.Field<string>("BB_DematAct").Trim(),

                           lblOfferDt = util.stod(Strings.Left(row.Field<string>("BB_DematAct").Trim(), 8)).ToString("dd/MM/yyyy"),
                           lblTDt = util.stod(Strings.Right(row.Field<string>("BB_DematAct").Trim(), 8)).ToString("dd/MM/yyyy"),
                           lblisin = lib.fnFireQuery("isin", "im_isin", "im_active = 'Y' and im_priority =(select min(im_priority) from Isin Where im_scripcd =  '" + arrScr[0] + "' and im_active = 'Y' ) and im_scripcd", arrScr[0], true),
                           se_stdt = util.stod(Strings.Right(row.Field<string>("se_stdt").Trim(), 8)).ToString("dd/MM/yyyy"),
                           se_payoutdt = util.stod(row.Field<string>("se_payoutdt")).ToString("dd/MM/yyyy"),
                           se_payindt = util.stod(row.Field<string>("se_payindt")).ToString("dd/MM/yyyy"),
                           ss_bsymbol = row.Field<string>("ss_bsymbol").Trim(),
                           ss_nsymbol = row.Field<string>("ss_nsymbol").Trim(),
                           se_endt = util.stod(row.Field<string>("se_endt")).ToString("dd/MM/yyyy"),
                           se_exchange = row.Field<string>("se_exchange").Trim(),
                           cmbExchStlmnt = Strings.Trim(lib.fnFireQuery("Settlement_type", "sy_desc", "sy_exchange = '" + row.Field<string>("se_stlmnt").ToString().Substring(0, 1) + "' and sy_type", row.Field<string>("se_stlmnt").ToString().Substring(1, 1), true)),

                       }).ToList();


            return Json(ulist, JsonRequestBehavior.AllowGet);

        }


        public ActionResult BuyBackOfferReport(string CmbSecurity, string lblisin, string lblRate, string lblstlmnt)
        {
            DPService dp = new DPService();
            DataTable ulist = dp.GetBuyBackOfferReport(CmbSecurity, lblisin, lblRate, lblstlmnt);
            return View(ulist);

        }

        public ActionResult BuyBackOfferSave(IEnumerable<BuyBackOfferSaveRequestModel> BuyBackOffer)
        {
            UtilityModel util = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            foreach (var item in BuyBackOffer)
            {
                string strSql = "";
                string[] array = item.Security.Trim().Split('/');
                if (item.Offered != 0)
                {
                    mylib.ExecSQL("Delete from Buybacks where BB_ClientCd='" + item.ClientCode.Trim() + "' and BB_Status='T' and BB_Stlmnt='" + array[1].Trim() + "' and BB_ScripCd ='" + array[0].Trim() + "' ");


                    if ((item.CKStatus.Trim() == "X" | item.CKStatus.Trim() == "T"))
                    {
                        strSql = " Insert into Buybacks Values(";
                        strSql = strSql + " '','" + array[0].Trim() + "','" + array[1].Trim() + "','" + item.ClientCode.Trim() + "'," + item.Offered + ",'" + item.Value + "','" + item.DematAccount.Trim() + "','" + DateTime.Now.ToString("yyyyMMdd") + "','" + util.dtos(item.PayInDt.Trim()) + "',0,";
                        strSql = strSql + "'T',0,0,'N','" + Session["gstrusercd"] + "','" + DateTime.Now.ToString("yyyyMMdd") + "'";
                        strSql = strSql + " )";
                        mylib.ExecSQL(strSql);

                    }
                }

            }
            return Content("Record Saved");

        }

    }
}
