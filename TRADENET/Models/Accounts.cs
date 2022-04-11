using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TRADENET.Models
{
    public class Accounts : ConnectionModel
    {
        public DataTable GetOutstandingAgeWise(string strClTo, string strClFrom, string cmbGroupBy, string strDpId, string strDays, string strAsOnDate, string strAllCompany, string cmbSelect, string cmbOrderBy, string cmbMarginAcBal)
        {
            DataTable dt = new DataTable();
            strAllCompany = HttpContext.Current.Session["CompanyCode"].ToString();

            string strGrpcd = "";
            string strFamcd = "";

            string strCompanyCode;
            string strWhere = "";
            string strExCES = "";
            string strCltCdFrom = "";
            string strCltCdTo = "";

            string strFam = "";
            string strGrp = "";


            string strfield = "123456789";

            string CMSCHEDULE = "49843750";
            string strGrpSelect = "";
            string strGrpSelect1 = "";
            string strGroupBy = "";
            string strGrpWhere = "";
            string strGrpTable = "";
            // string strFilter = "";
            string strwhere = "";
            string strClcd = "";
            string sIncNSEBSE = "";
            string strTpExchangeSeg = "";
            string strComExchange = "";
            string strsql = "";
            // string sShowHolding = "N";
            // string sShowBranchWise = "N";
            string strExchange = "N";
            string IsTplusCommex = "N";
            //  string strsql, GetQuery = "";

            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");

            string ExCommex = "";
            string Exchng = "";
            string[] strArray = strDpId.Split(',');
            bool first = true;
            foreach (string obj in strArray)
            {
                if (obj.Length >= 2)
                {
                    if (first)
                    {
                        Exchng = obj;
                        first = false;
                    }
                    else
                    {
                        Exchng = obj + "," + Exchng;
                    }
                }

                //your insert query
            }
            bool Cofirst = true;
            foreach (string obj in strArray)
            {
                if (obj.Length < 2)
                {
                    if (Cofirst)
                    {
                        ExCommex = obj;
                        Cofirst = false;
                    }
                    else
                    {
                        ExCommex = obj + "," + ExCommex;
                    }
                }

                //your insert query
            }


            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                strwhere = "";
                strClcd = "";
                if (strAllCompany == "")
                {
                    if (strDpId != "")
                        sIncNSEBSE = "";
                }
                else
                {
                    strDpId = "";
                    sIncNSEBSE = " and left(dm_stlmnt,1) = '" + strExchange + "'";
                }

                if (strGrpcd != "")
                    strwhere = "  cm_groupcd = '" + strGrpcd + "' and ";
                if (strFamcd != "")
                    strwhere = strwhere + "  cm_familycd='" + strFamcd + "' and ";
                if (cmbSelect == "Client")
                {
                    if (strClTo != "" & strClFrom != "")
                        strwhere = strwhere + " cm_cd >= '" + strClFrom.Trim() + "' and cm_cd<= '" + strClTo.Trim() + "' and  ";
                    else if (strClFrom != "" & strClTo == "")
                        strwhere = strwhere + " cm_cd = '" + strClFrom.Trim() + "' and ";
                    else if (strClFrom == "" & strClTo != "")
                        strwhere = strwhere + " cm_cd <= '" + strClTo.Trim() + "' and ";
                }
                if (!string.IsNullOrEmpty(strClFrom.Trim()))
                {
                    if (cmbSelect == "Family")
                        strwhere = strwhere + "  cm_familycd = '" + strClFrom.Trim() + "' and ";
                    else if (cmbSelect == "Group")
                        strwhere = strwhere + "  cm_groupcd = '" + strClFrom.Trim() + "' and ";
                    else if (cmbSelect == "Sub-Broker")
                        strwhere = strwhere + "  cm_subbroker = '" + strClFrom.Trim() + "' and ";
                }

                if (cmbGroupBy == "Name")
                {
                    strGroupBy = "";
                    strGrpSelect1 = " '' as [GrpColName],";
                    strGrpSelect = " [GrpColName], ";
                    strGrpWhere = "";
                    strGrpTable = "";
                }
                else if (cmbGroupBy == "Group")
                {
                    strGroupBy = " , gr_desc,gr_cd";
                    strGrpSelect = "[GrpColName],";
                    strGrpSelect1 = " rtrim(gr_desc) + '[' + ltrim(rtrim(gr_cd ))+ ']'  as [GrpColName],";
                    strGrpWhere = " and cm_groupcd = gr_cd ";
                    strGrpTable = " , Group_Master ";
                }
                else if (cmbGroupBy == "Family")
                {
                    strGroupBy = " ,fm_desc,fm_cd";
                    strGrpSelect = "[GrpColName],";
                    strGrpSelect1 = "  rtrim(fm_desc) + '[' + ltrim(rtrim(fm_cd)) + ']'  as [GrpColName],";
                    strGrpWhere = "  and cm_familycd=fm_cd ";
                    strGrpTable = " , Family_Master ";
                }
                else if (cmbGroupBy == "Branch")
                {
                    strGroupBy = " ,bm_branchname,bm_branchcd";
                    strGrpSelect = "[GrpColName],";
                    strGrpSelect1 = "  rtrim(bm_branchname)  + '[' + ltrim(rtrim(bm_branchcd)) + ']' as [GrpColName],";
                    strGrpWhere = " and cm_brboffcode = bm_branchcd ";
                    strGrpTable = " ,Branch_Master ";
                }


                string strrorderby;
                strrorderby = cmbOrderBy;
                string abc, abc1, abc2;

                abc = myutil.dtos((myutil.SubDayDT(myutil.dtos(strAsOnDate), Convert.ToInt32(strDays))).ToString("dd/MM/yyyy"));
                abc1 = myutil.dtos((myutil.SubDayDT(myutil.dtos(strAsOnDate), Convert.ToInt32(strDays) * 2)).ToString("dd/MM/yyyy"));
                abc2 = myutil.dtos((myutil.SubDayDT(myutil.dtos(strAsOnDate), Convert.ToInt32(strDays) * 3)).ToString("dd/MM/yyyy"));

                string chkamt;
                chkamt = "select cm_name,ld_clientcd," + strGrpSelect + " cast(case sign(b1-c1) when 1 then b1-c1 else 0 end as decimal(15,2)) - cast(case sign(b2-c2) when 1 then b2-c2 else 0 end as decimal(15,2))  A1 ,";
                chkamt = chkamt + "cast(case sign(b2-c2) when 1 then b2-c2 else 0 end as decimal(15,2)) - cast(case sign(b3-c3) when 1 then b3-c3 else 0 end as decimal(15,2)) A2,";
                chkamt = chkamt + "cast(case sign(b3-c3) when 1 then b3-c3 else 0 end as decimal(15,2)) - cast(case sign(b4-c4) when 1 then b4-c4 else 0 end as decimal(15,2)) A3,";
                chkamt = chkamt + " cast(case sign(b4-c4) when 1 then b4-c4 else 0 end as decimal(15,2)) A4,";
                chkamt = chkamt + " cast(bal as decimal(15,2)) bal from (select cm_name,cm_cd ld_clientcd,cm_subbroker," + strGrpSelect1;
                chkamt = chkamt + "sum(case sign(datediff(d,ld_dt,'" + myutil.dtos(strAsOnDate) + "') ) when -1 then 0 else ld_amount end ) B1, ";
                chkamt = chkamt + "sum(case sign(datediff(d,ld_dt,'" + myutil.dtos(strAsOnDate) + "') ) when -1 then case ld_debitflag when 'C' then -ld_amount else 0 end else 0 end ) C1,";
                chkamt = chkamt + "sum(case sign(datediff(d,ld_dt,'" + abc + "') )  when -1 then 0 else ld_amount end ) B2, ";
                chkamt = chkamt + "sum(case sign(datediff(d,ld_dt,'" + abc + "') )  when -1 then case ld_debitflag when 'C' then -ld_amount else 0 end else 0 end ) C2,";
                chkamt = chkamt + "sum(case sign(datediff(d,ld_dt,'" + abc1 + "') ) when -1 then 0 else ld_amount end ) B3,";
                chkamt = chkamt + "sum(case sign(datediff(d,ld_dt,'" + abc1 + "') ) when -1 then case ld_debitflag when 'C' then -ld_amount else 0 end else 0 end ) C3,";
                chkamt = chkamt + "sum(case sign(datediff(d,ld_dt,'" + abc2 + "') ) when -1 then 0 else ld_amount end ) B4,";
                chkamt = chkamt + "sum(case sign(datediff(d,ld_dt,'" + abc2 + "') ) when -1 then case ld_debitflag when 'C' then -ld_amount else 0 end else 0 end ) C4,";
                chkamt = chkamt + "sum(ld_amount) Bal From ( ";
                if (Exchng != "")
                {
                    chkamt = chkamt + " select ld_clientcd,ld_dt,ld_debitflag,sum(ld_amount) ld_amount from ledger , Client_master ";
                    chkamt = chkamt + " where ";
                    chkamt = chkamt + " substring(ld_dpid,2,2) in ('" + Exchng.Replace(",", "','") + "') and left(ld_DPID,1) = '" + HttpContext.Current.Session["CompanyCode"] + "'";
                    chkamt = chkamt + " and " + strwhere + " ";
                    if (cmbMarginAcBal == "0")
                    {
                        chkamt = chkamt + " (ld_clientcd=cm_cd Or ld_clientcd = cm_brkggroup ) ";
                        chkamt = chkamt + " and cm_cd not in (select cm_brkggroup From Client_master Where  cm_brkggroup <> '') ";
                    }
                    else if (cmbMarginAcBal == "1")
                        chkamt = chkamt + " ld_clientcd=cm_cd ";
                    else if (cmbMarginAcBal == "2")
                    {
                        chkamt = chkamt + " ld_clientcd=cm_cd ";
                        chkamt = chkamt + " and cm_cd not in (select cm_brkggroup From Client_master Where  cm_brkggroup <> '') ";
                    }
                    chkamt = chkamt + " and cm_Schedule = '" + CMSCHEDULE + "' and cm_type <> 'C'";
                    chkamt = chkamt + " and ld_dt<='" + myutil.dtos(strAsOnDate) + "'and exists " + myutil.LoginAccess("ld_clientcd"); // '''Session("LoginAccess")
                    chkamt = chkamt + " Group By ld_clientcd,ld_dt,ld_debitflag ";
                }
                if (SQLConnComex != null && ExCommex != "")
                {
                    if (Exchng != "")
                    {
                        if (!string.IsNullOrEmpty(chkamt))
                            chkamt += " union all ";
                    }
                    chkamt = chkamt + " select ld_clientcd,ld_dt,ld_debitflag,sum(ld_amount) ld_amount from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.ledger , Client_master ";
                    chkamt = chkamt + " where ";
                    if (IsTplusCommex == "Y")
                        chkamt = chkamt + " substring(ld_dpid,2,2) in ('" + ExCommex.Replace(",", "','") + "')";
                    else
                        chkamt = chkamt + " substring(ld_dpid,2,1) in ('" + ExCommex.Replace(",", "','") + "')";
                    chkamt = chkamt + " and " + strwhere + " ";
                    if (cmbMarginAcBal == "0")
                    {
                        chkamt = chkamt + " (ld_clientcd=cm_cd Or ld_clientcd = cm_brkggroup ) ";
                        chkamt = chkamt + " and cm_cd not in (select cm_brkggroup From Client_master Where  cm_brkggroup <> '') ";
                    }
                    else if (cmbMarginAcBal == "1")
                        chkamt = chkamt + " ld_clientcd=cm_cd ";
                    else if (cmbMarginAcBal == "2")
                    {
                        chkamt = chkamt + " ld_clientcd=cm_cd ";
                        chkamt = chkamt + " and cm_cd not in (select cm_brkggroup From Client_master Where  cm_brkggroup <> '') ";
                    }
                    chkamt = chkamt + " and cm_Schedule = '" + CMSCHEDULE + "' and cm_type <> 'C'";
                    chkamt = chkamt + " and ld_dt<='" + myutil.dtos(strAsOnDate) + "'and exists " + myutil.LoginAccess("ld_clientcd"); // '''Session("LoginAccess")
                    chkamt = chkamt + " Group By ld_clientcd,ld_dt,ld_debitflag ";
                }
                chkamt = chkamt + " ) a , Client_master " + strGrpTable;
                chkamt = chkamt + "Where ld_clientcd = cm_cd " + strGrpWhere;
                chkamt = chkamt + "group by cm_cd,cm_name,cm_subbroker ";
                chkamt = chkamt + strGroupBy + " having sum(ld_amount)>0 ";
                chkamt = chkamt + " ) xyz ";
                chkamt = chkamt + "order by [GrpColName], " + strrorderby;
                strsql = chkamt;

                //GetQuery = strsql;

                dt = myLib.OpenDataTable(strsql, curCon);

                return dt;

            }

        }


        public DataTable GetOutstandingAgeWiseReport(string strCode, string strname, string strAsOnDate, string FinYear)
        {
            DataTable dt = new DataTable();
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();



            string strDate = strAsOnDate;
            string strDateFr = "01/" + Strings.Right(strDate, 7);
            string strDateTo = strDate;
            string strsql = "";
            string[] strYear;
            string strdafrom = myutil.dtos(FinYear);
            string strdato = strDate; // To Date
            string result = "";
            string strtable = "ledger";
            string strType = "";
            string IsTplusCommex = "N";
            string strDpId = "";


            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                strsql = "select ld_clientcd,'" + strdafrom + "' ld_dt, 'Opening Balance' ld_particular, 'O' ld_documenttype, 0 ld_documentno,  case sign(sum(ld_amount)) when 1 then 'D' else 'C' end as ld_debitflag, sum(ld_amount) 'ld_amount', 0 ord ";
                strsql = strsql + " from " + strtable + " where ld_clientcd='" + strCode + "'";
                if (result == "X")
                    strsql = strsql + " and left(ld_DPID,1)= '" + HttpContext.Current.Session["Company"] + "'";

                if (strType.Trim() == "Commex" && IsTplusCommex == "Y")
                    strsql = strsql + " and Right(rtrim(ld_DPID),1)= 'X'";
                else
                    strsql = strsql + " and Right(rtrim(ld_DPID),1) <> 'X'";

                strsql = strsql + " and ld_dt < '" + strdafrom + "'" + strDpId + "  group by ld_clientcd having sum(ld_amount)<>0";
                strsql = strsql + " union all ";
                strsql = strsql + " select ld_clientcd,ld_dt, ld_particular, ld_documenttype,ld_documentno, ld_debitflag, ld_amount, 1 ord from " + strtable + "";
                strsql = strsql + " where ld_clientcd='" + strCode + "' ";

                if (strdato != "XXXXXXXX")
                    strsql = strsql + " and ld_dt between '" + strdafrom + "'" + " AND '" + myutil.dtos(strdato) + "'";
                else
                    strsql = strsql + " and ld_dt >= '" + strdafrom + "'";

                strsql = strsql + strDpId;
                if (result == "X")
                    strsql = strsql + " and left(ld_DPID,1)= '" + HttpContext.Current.Session["Company"] + "'";

                if (strType.Trim() == "Commex" & IsTplusCommex == "Y")
                    strsql = strsql + " and Right(rtrim(ld_DPID),1) = 'X'";
                else
                    strsql = strsql + " and Right(rtrim(ld_DPID),1) <> 'X'";

                strsql = strsql + "  order by ord, ld_dt";
                dt = myLib.OpenDataTable(strsql, curCon);

                return dt;
            }

        }

        public DataTable GetCReceiptsReport(string Code, string strDpId, string Frmdt, string Todt, string cmbSelect, string ChkAll, string cmbBank, string cmbChoices, string newcombo1)
        {
            DataTable dt = new DataTable();

            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            string strSql = "";
            string strwhere = "";
            string ExCommex = "";
            string Exchng = "";
            string CMSCHEDULE = "49843750";



            if ((Strings.Mid(strDpId, 3, 1) ?? "") == "X")
            {
                SqlConnection ObjCommexCon = mydbutil.commexTemp_conn("Commex");

                if (ObjCommexCon != null)
                {
                    if (ObjCommexCon.State == ConnectionState.Closed)
                    {
                        ObjCommexCon.Open();
                    }
                }
            }
            else
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();
                }

            }
            string SQLstrwhere = "";
            if (Code != "")
            {
                if (cmbSelect == "Client")
                    SQLstrwhere += " and A.rc_clientcd = '" + Code.Trim() + "' ";
                else if (cmbSelect == "Family")
                    SQLstrwhere += " and B.cm_familycd = '" + Code.Trim() + "'"; // and cm_familycd= fm_cd "
                else if (cmbSelect == "Group")
                    SQLstrwhere += " and B.cm_groupcd = '" + Code.Trim() + "' "; // and cm_groupcd=gr_cd "
                else if (cmbSelect == "Sub-Broker")
                    SQLstrwhere += " and B.cm_subbroker = '" + Code.Trim() + "' "; // and cm_subbroker=rm_Cd "
                else if (cmbSelect == "RM")
                    SQLstrwhere += " and rtrim(cm_dpactno) = '" + Code.Trim() + "'";
                else if (cmbSelect == "BR")
                    SQLstrwhere += " and cm_brboffcode = '" + Code.Trim() + "'";
                else if (cmbSelect == "ALL")
                    SQLstrwhere += " ";
            }

            string strTemp;
            string strHd = "";
            string strHead1 = "";

            Frmdt = myutil.dtos(Frmdt);
            Todt = myutil.dtos(Todt);

            if ((Strings.Mid(strDpId, 3, 1) ?? "") != "X")
            {
                strTemp = "Receipts Register          " + "From     " + Frmdt + "      To     " + Todt;
                strSql = "select rc_srno, rc_voucherno, substring(rc_receiptdt,7,2) + '/' + substring(rc_receiptdt,5,2) + '/' + substring(rc_receiptdt,1,4) Receiptdt, rc_clientcd,b.cm_name as Name, case isnull(rc_cleareddt,'') when '' then 'u' else '' end as ClrFlag, rc_particular,rc_chequeno, rc_amount,rtrim(c.cm_Name)+' ['+rtrim(rc_bankclientcd)+']' as BankName ,rc_debitflag,rc_authremarks from receipts A, Client_master B, client_master C ";
                strSql = strSql + "where A.rc_receiptdt between '" + Frmdt + "' and '" + Todt + "' and  A.rc_clientcd = B.cm_cd and exists " + myutil.LoginAccess("rc_clientcd") + " and a.rc_bankclientcd=c.cm_cd and "; // 'session("loginaccess")
                strSql = strSql + " b.cm_schedule= '" + CMSCHEDULE + "' ";
            }
            else
            {
                strTemp = "Receipts Register          " + "From     " + Frmdt + "      To     " + Todt;
                strSql = "select rc_srno, rc_voucherno, substring(rc_receiptdt,7,2) + '/' + substring(rc_receiptdt,5,2) + '/' + substring(rc_receiptdt,1,4) Receiptdt, rc_clientcd,b.cm_name as Name, case isnull(rc_cleareddt,'') when '' then 'u' else '' end as ClrFlag, rc_particular,rc_chequeno, rc_amount,rtrim(c.cm_Name)+' ['+rtrim(rc_bankclientcd)+']' as BankName ,rc_debitflag,rc_authremarks from receipts A, Client_master B, client_master C ";
                strSql = strSql + "where A.rc_receiptdt between '" + Frmdt + "' and '" + Todt + "' and  A.rc_clientcd = B.cm_cd and exists " + myutil.LoginAccessCommex("rc_clientcd") + " and a.rc_bankclientcd=c.cm_cd and "; // 'session("loginaccess")
                strSql = strSql + " b.cm_schedule= '" + CMSCHEDULE + "' ";

            }

            strSql = strSql + SQLstrwhere;
            if (ChkAll == "0")
            {
                if (cmbBank != "")
                {
                    strSql = strSql + " and  rc_bankclientcd= '" + Strings.Trim(cmbBank) + "' ";
                }
            }


            if (cmbChoices == "R") // Receipts
            {
                strSql = strSql + " and rc_debitflag = 'C'";
                strHead1 = "Receipt Checklist ";
            }
            else if (cmbChoices == "P") // Payments
            {
                strSql = strSql + " and rc_debitflag = 'D'";
                strHead1 = "Payment Checklist ";
            }
            else
            {
                strHead1 = "Checklist For Receipt And Payment";
            }

            if (newcombo1 == "Sort0")
            {
                strSql = strSql + " and rc_status='Y'";
            }
            else if (newcombo1 == "Sort1")
            {
                strSql = strSql + " and rc_status='N'";
            }
            else if (newcombo1 == "Sort2")
            {
                strSql = strSql + " and rc_status='R'";
            }
            else if (newcombo1 == "Sort3")
            {
            }

            strSql = strSql + " order by rc_bankclientcd, rc_receiptdt, rc_srno, rc_entryno ";

            if ((Strings.Mid(strDpId, 3, 1) ?? "") == "X")
            {
                SqlConnection ObjCommexCon = mydbutil.commexTemp_conn("Commex");

                if (ObjCommexCon != null)
                {
                    if (ObjCommexCon.State == ConnectionState.Closed)
                    {
                        ObjCommexCon.Open();
                    }
                }
                dt = myLib.OpenDataTable(strSql, ObjCommexCon);
            }
            else
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();
                    dt = myLib.OpenDataTable(strSql, curCon);
                }

            }

            if (dt.Rows.Count > 0)
            {

                dt.Columns.Add("DebitAmt");
                dt.Columns.Add("CreditAmt");
                for (int dc = 0, loopTo = dt.Rows.Count - 1; dc <= loopTo; dc++)
                {
                    if (Convert.ToDouble(dt.Rows[dc]["rc_amount"].ToString().Trim()) > 0)
                    {
                        dt.Rows[dc]["DebitAmt"] = Math.Abs(Convert.ToDecimal(dt.Rows[dc]["rc_amount"]));
                        dt.Rows[dc]["CreditAmt"] = "";
                    }
                    else
                    {
                        dt.Rows[dc]["DebitAmt"] = "";
                        dt.Rows[dc]["CreditAmt"] = Math.Abs(Convert.ToDecimal(dt.Rows[dc]["rc_amount"]));
                    }
                }
            }


            return dt;

        }


        public DataTable AOutstandingBalReport(string strClient, string dateFrom, string cmbRep, string CmbSelection, string cmbExchSeg, string cmbOrderBy, string cmbAmt, string cmbOutstanding, string cmbActType, string chkMarginact, string txtOutfrom, string txtOutTo)
        {
            string SQLstrwhere = "";
            string strsql = "";
            string Client = "";
            string strFields = "";
            string strLdgrDPID = "";
            string strMrgCol = "";
            string strWhere = "";
            string[] arrDPId = new string[22];
            int j = 0, I;
            string strGroup = "";
            string[] arrColNm = new string[22];
            string chrSpecialClient = "1";
            string gstrHeader3 = ",grName,Code,Name";
            string IsTplusCommex = (string)HttpContext.Current.Session["IsTplusCommex"];
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
            DataTable dt = new DataTable();
            DataTable dtReturn = new DataTable();

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();


                if (strClient != "")
                {
                    if (CmbSelection == "CL")
                        SQLstrwhere += " and CMCD = '" + strClient.Trim() + "' ";
                    else if (CmbSelection == "FM")
                        SQLstrwhere += " and cm_familycd = '" + strClient.Trim() + "'"; // and cm_familycd= fm_cd "
                    else if (CmbSelection == "GR")
                        SQLstrwhere += " and cm_groupcd = '" + strClient.Trim() + "' "; // and cm_groupcd=gr_cd "
                    else if (CmbSelection == "SB")
                        SQLstrwhere += " and cm_subbroker = '" + strClient.Trim() + "' "; // and cm_subbroker=rm_Cd "
                    else if (CmbSelection == "RM")
                        SQLstrwhere += " and rtrim(cm_dpactno) = '" + strClient.Trim() + "'";
                    else if (CmbSelection == "BR")
                        SQLstrwhere += " and cm_brboffcode = '" + strClient.Trim() + "'";
                    else if (CmbSelection == "ALL")
                        SQLstrwhere += " ";
                }

                if (chrSpecialClient == "0")
                {
                    Client = " and cm_specialyn <> 'Y' ";

                }

                if (cmbExchSeg != "")
                {
                    if (IsTplusCommex == "Y")
                    {
                        SQLConnComex = curCon;
                    }
                    else
                    {
                        SQLConnComex = mydbutil.commexTemp_conn("Commex");
                    }

                }

                string[] strArray = cmbExchSeg.Split(',');
                string Exchng = "";
                bool first = true;
                foreach (string obj in strArray)
                {
                    if (obj.Length >= 2)
                    {
                        if (first)
                        {
                            Exchng = obj;
                            first = false;
                        }
                        else
                        {
                            Exchng = obj + "," + Exchng;
                        }
                    }

                    //your insert query
                }
                string ExCommex = "";
                bool Cofirst = true;
                foreach (string obj in strArray)
                {
                    if (obj.Length < 2)
                    {
                        if (Cofirst)
                        {
                            ExCommex = obj;
                            Cofirst = false;
                        }
                        else
                        {
                            ExCommex = obj + "," + ExCommex;
                        }
                    }

                    //your insert query
                }

                // FnGetSql = True
                switch (cmbOutstanding)
                {
                    case "0":
                        {
                            strFields = " CES_Cd , '['+ CES_CompanyCd + ']' + Rtrim(CES_Exchange) + '/' + Rtrim(CES_Segment) ColumnName ";
                            strLdgrDPID = "LD_DPID";
                            break;
                        }

                    case "1":
                        {
                            strFields = " left(CES_Cd,2) CES_Cd , '['+ CES_CompanyCd + ']' + Rtrim(CES_Exchange) ColumnName ";
                            strLdgrDPID = " left(LD_DPID,2) ";
                            break;
                        }

                    case "2":
                        {
                            strFields = " left(CES_Cd,1) CES_Cd , '['+ CES_CompanyCd + ']' ColumnName ";
                            strLdgrDPID = (("IsTplusCommex") == "Y" ? " left(LD_DPID,2) " : " left(LD_DPID,1)");
                            break;
                        }

                }



                switch (cmbRep)
                {
                    case "0":
                        {
                            strsql = "select '' grName, ";
                         
                            break;
                        }

               

                    case "1":
                        {
                            strsql = "select rtrim(gr_desc) + ' [' + rtrim(cm_groupcd) + ']' grName, ";
                          
                            break;
                        }
                    case "2":
                        {
                            strsql = "select rtrim(fm_desc) + ' [' + rtrim(cm_familycd) + ']' grName,";
                       
                            break;
                        }
                    case "4":
                        {
                            strsql =  "select rtrim(bm_branchname)+' ['+rtrim(cm_brboffcode)+']' grName, ";
                        
                            break;
                        }
                    case "5":
                        {
                            strsql = "select rtrim(rm_name)+' ['+rtrim(cm_dpactno)+']' grName,";
                          
                            break;
                        }
                }

                strsql = strsql + "xyz.*, GTotalTmp" + strMrgCol + " as Gtotal from (select cm_cd CMCD,cm_dpactno, cm_name, cm_groupcd, cm_familycd, cm_brboffcode, cm_subbroker, ";

                string strsql1 = "Select distinct " + strFields + ",'T' Product  from CompanyExchangeSegments where CES_CompanyCd = '" + HttpContext.Current.Session["CompanyCode"] + "' and substring(Ces_Cd,2,2) in ('" + Exchng.Replace(",", "','") + "')";
                if (SQLConnComex != null)
                {
                    strsql1 += " union all Select distinct " + Strings.Replace(strFields, "Rtrim(CES_Segment)", "'Comm'") + ",'X' Product from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.CompanyExchangeSegments ";
                    strsql1 += " where   " + (("IsTplusCommex") == "Y" ? "substring(Ces_Cd,2,2)" : " substring(Ces_Cd,2,1)") + " in ('" + ExCommex.Replace(",", "','") + "')";
                }
                strsql1 += " order by Product,CES_Cd ";

                dt = mylib.OpenDataTable(strsql1, curCon);


                HttpContext.Current.Session["AOutBalHead"] = dt;

                for (I = 0; I <= 21; I++)
                {
                    if (I < dt.Rows.Count)
                    {
                        arrDPId[I] = Convert.ToString(dt.Rows[I][0]);
                        arrColNm[I] = Convert.ToString(dt.Rows[I][0]);
                        j = j + 1;
                    }
                    else if (I == 21)
                    {
                        arrColNm[I] = "Outstanding";
                    }
                    else
                    {

                        arrDPId[I] = "";
                        arrColNm[I] = "";

                    }
                    if (I < dt.Rows.Count)
                    {
                        strsql = strsql + " sum(Case when " + strLdgrDPID + " = '" + arrDPId[I] + "' and LD_Product = '" + Convert.ToString(dt.Rows[I][2]) + "' then ld_amount else 0 end) as Col" + I + ", ";
                    }
                    else
                    {
                        strsql = strsql + "'0' Col" + I + ",";

                    }

                }
                for (I = 0; I <= 21; I++)
                {
                    gstrHeader3 = gstrHeader3 + "," + arrColNm[I];
                }

                HttpContext.Current.Session[gstrHeader3] = "gstrHeader3";

                if (dateFrom != "")
                {
                    strWhere = "and ld_dt<= '" + myutil.dtos(dateFrom) + "' ";
                }
                strsql = strsql + " sum(ld_amount) GTotalTmp From ( ";

                if (Exchng != "")
                {
                    strsql = strsql + " select 'T' LD_Product,ld_DPID,cm_cd ld_clientcd,sum(ld_amount) ld_amount From Ledger,client_master ";
                    strsql = strsql + " where (" + (cmbActType == "1" ? " ld_clientcd = cm_brkggroup " : "ld_clientcd=cm_cd") + (chkMarginact == "1" ? " Or ld_clientcd = cm_brkggroup " : "") + " )" + strWhere;
                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
;                    strsql = strsql + " and left(ld_DPID,1) = '" + HttpContext.Current.Session["CompanyCode"] + "' and substring(ld_DPID,2,2) in ('" + Exchng.Replace(",", "','") + "')";
                    strsql = strsql + " and cm_type <> 'C' and cm_schedule =  49843750 " + Client + " Group By ld_DPID,cm_cd ";
                }

                if (SQLConnComex!= null & cmbExchSeg != "")
                {
                    if (ExCommex != "")
                    {
                        strsql += " union all ";                        
                        strsql = strsql + " select 'X' LD_Product,ld_DPID,cm_cd ld_clientcd,sum(ld_amount) ld_amount From " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Ledger,client_master ";
                        strsql = strsql + " where ( " + (cmbActType == "1" ? " ld_clientcd = cm_brkggroup " : "ld_clientcd=cm_cd") + (chkMarginact == "1" ? " Or ld_clientcd = cm_brkggroup " : "") + " )" + strWhere;
                        strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                        strsql = strsql + " and   " + (("IsTplusCommex") == "Y" ? "substring(ld_DPID,2,2)" : " substring(ld_DPID,2,1)") + " in ('" + ExCommex.Replace(",", "','") + "')";
                        strsql = strsql + " and cm_type <> 'C' and cm_schedule = 49843750 " + Client + " Group By ld_DPID,cm_cd ";
                    }

                }
                strsql = strsql + " ) A , client_master ";
                strsql = strsql + " where ld_clientcd=cm_cd ";
                strsql = strsql + " group by cm_cd,cm_dpactno, cm_name,cm_brkggroup, cm_groupcd, cm_familycd, cm_brboffcode, cm_subbroker ) XYZ ";

                mylib.ExecSQL(strsql, curCon);
                switch (cmbRep)
                {
                    case "0":
                        {
                            strsql = strsql + " where 1=1 ";
                            strGroup = "";
                            break;
                        }

                    case "1":
                        {
                            strsql = strsql + " , group_master where cm_groupcd=gr_cd ";
                            strGroup = "cm_groupcd,";
                            break;
                        }

                    case "2":
                        {
                            strsql = strsql + " ,family_master where cm_familycd=fm_cd ";
                            strGroup = "cm_familycd,";
                            break;
                        }
                    case "4":
                        {
                            strsql = strsql + " , branch_master where cm_brboffcode=bm_branchcd ";
                            strGroup = "cm_brboffcode,";
                            break;
                        }
                    case "5":
                        {
                            strsql = strsql + " , RM_master where cm_dpactno=rm_cd ";
                            strGroup = "cm_dpactno,";
                            break;
                        }
                }
                if (cmbAmt == "All")    // ALL 
                {
                    if (!string.IsNullOrEmpty(Strings.Trim(txtOutfrom)) & !string.IsNullOrEmpty(Strings.Trim(txtOutTo)))
                    {
                        strsql = strsql + " and isnull(abs(GTotalTmp),0)>=" + txtOutfrom + " ";// and isnull(abs(GTotalTmp),0)<=" + txtOutTo + "
                    }
                }
                else if (cmbAmt == "0")  // Debit   
                {
                    strsql = strsql + " and round(GTotalTmp" + strMrgCol + ",2) > 0 ";
                    if (!string.IsNullOrEmpty(Strings.Trim(txtOutfrom)) & !string.IsNullOrEmpty(Strings.Trim(txtOutTo)))
                    {
                        strsql = strsql + " and isnull(abs(GTotalTmp),0)>=" + txtOutfrom + " ";// and isnull(abs(GTotalTmp),0)<=" + txtOutTo + "
                    }
                }
                else
                {
                    strsql = strsql + " and round(GTotalTmp" + strMrgCol + ",2) < 0 "; // Credit
                    if (!string.IsNullOrEmpty(Strings.Trim(txtOutfrom)) & !string.IsNullOrEmpty(txtOutTo))
                    {
                        strsql = strsql + " and isnull(abs(GTotalTmp),0)>=" + (txtOutfrom) + " ";// and isnull(abs(GTotalTmp),0)<=" + txtOutTo + "
                    }
                }
                //if (cmbAmt == "All")
                //{

                //    strsql = strsql + " and round(GTotalTmp" + strMrgCol + ",2) > 0 ";
                //    if (txtOutfrom != "" & txtOutTo != "")
                //    {
                //        strsql = strsql + " and isnull(abs(GTotalTmp),0)>=" + txtOutfrom + " and isnull(abs(GTotalTmp),0)<=" + txtOutTo + " ";
                //    }
                //    else
                //    {
                //        strsql = strsql + " and round(GTotalTmp" + strMrgCol + ",2) < 0 "; // Credit
                //        if (txtOutfrom != "" & txtOutTo != "")
                //        {
                //            strsql = strsql + " and isnull(abs(GTotalTmp),0)>=" + txtOutfrom + " and isnull(abs(GTotalTmp),0)<=" + txtOutTo + " ";
                //        }

                //    }


                //}

                strsql = strsql + SQLstrwhere;

                switch (cmbOrderBy)
                {
                    case "0":
                        {
                            strsql = strsql + "order by " + strGroup + "GTotalTmp" + strMrgCol + " desc";
                            break;
                        }

                    case "1":
                        {
                            strsql = strsql + "order by " + strGroup + "GTotalTmp" + strMrgCol;
                            break;
                        }

                    case "2":
                        {
                            strsql = strsql + "order by " + strGroup + "cm_name";
                            break;
                        }
                    case "3":
                        {
                            strsql = strsql + "order by " + strGroup + "CMCD";
                            break;
                        }

                }
                dtReturn = mylib.OpenDataTable(strsql, curCon);

                //for (int l = 0; dtReturn.Rows.Count > l; l++)
                //{
                //    DataRow dr = dtReturn.Rows[l];

                //    for (int k = 0; dr.Table.Columns.Count > k; k++)
                //    {
                //        if (dtReturn.Rows[l][k].ToString() == "0.0000")
                //        {
                //            dtReturn.Rows[l][k] = DBNull.Value;
                //        }

                //    }

                //}
            }
            // FnGetSql = strsql


            return dtReturn;
        }

    }
}