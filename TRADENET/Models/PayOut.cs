using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TRADENET.Models
{
    public class PayOut : ConnectionModel
    {
        public DataTable GetBulkPayoutNew(string Code, string select, string date, string formate, string cmbFormat, Boolean chkInclOnDemPayOut)
        {
            DataTable dt = new DataTable();
            DataTable dtPayout = null;
            DataTable dtPayoutRequest = null;
            string strsql = null;

            string msg = "";
            string strMode = "";
            string StrSubBrok = null;
            string strTable1 = "#Style2";
            string strTmptblReport = "#tmptblReport";
            string strClientWhere = "";
            string strClientcd;
            string strDt = "";
            string PBValue = "";
            string strMatchALL = "";
            string strBal, strldDP;
            string RMS = "RMS";
            string CMSCHEDULE = "49843750";
            DataSet ObjDataset;
            string FundsRequest = "";
            string strMarginWhere;
            string strAmt1 = "";
            string strAmt2 = "";
            string strAmt3 = "";
           // Boolean chkInclOnDemPayOut = true;

            string gstrDBToday = DateTime.Today.ToString("yyyyMMdd");
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();

            strDt = myutil.dtos(date);
            //strDt = date;


            string strSqlDel;
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                strsql = " CREATE TABLE #Style2 ( ";
                strsql = strsql + " [Client] [char](8) NOT NULL ,";
                strsql = strsql + " [Name] [char](75) NULL,";
                strsql = strsql + " [dp_id] [char](10) NULL,";
                strsql = strsql + " [amt1] [money] NOT NULL, ";
                strsql = strsql + " [amt2] [money] NOT NULL, ";
                strsql = strsql + " [amt3] [money] NOT NULL, ";
                strsql = strsql + " [RAmt] [money] not null, ";
                strsql = strsql + " [minamt] [money] NOT NULL, ";
                strsql = strsql + " srno [numeric](18, 0) IDENTITY (1, 1) NOT NULL primary key";
                strsql = strsql + " ) ";
                myLib.ExecSQL(strsql, curCon);

                strMode = "A";
                StrSubBrok = myLib.fnGetSysParam("REMSCHEDULE");
                StrSubBrok = ((StrSubBrok != "") ? ",'" + StrSubBrok + "'" : "");

                if (strMode == "") { strMode = "V"; }


                //if (RMS == "RMS")
                //{
                //    prStyle1();
                //}
                double dblAmt1;
                double dblAmt2, dblAmt3, dblRequestAmt, dblFinal;
                string strGroupCode;
                int iRow;
                switch (cmbFormat)
                {
                    case "0":

                        dtPayout = fnGetsqlTradePlus(Code, select, date, formate, cmbFormat, chkInclOnDemPayOut);

                        if (strMode == "A")
                        {
                            strsql = strsql + "truncate table #Style2";
                            myLib.ExecSQL(strsql, curCon);

                        }

                       
                        iRow = 0;
                        while (iRow < dtPayout.Rows.Count)
                        {
                            strGroupCode = (dtPayout.Rows[iRow]["Client"].ToString());
                            dblAmt1 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt1"]);
                            dblAmt2 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt2"]);
                            dblAmt3 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt3"]);

                            strsql = "Insert into #Style2 (Client , Name,dp_id, Amt1, Amt2, amt3,RAmt, minamt) Values ('" + (dtPayout.Rows[iRow]["Client"]).ToString().Trim() + "','";
                            strsql = strsql + Strings.Left((dtPayout.Rows[iRow]["Name"].ToString()), 50) + "','" + (dtPayout.Rows[iRow]["dp_id"]).ToString().Trim() + "'," + dblAmt1 + "," + dblAmt2 + "," + dblAmt3 + "," + (dtPayout.Rows[iRow]["RAmt"]).ToString().Trim() + ",";
                            strsql = strsql + dblAmt3 + ")";
                            myLib.ExecSQL(strsql, curCon);
                            iRow = iRow + 1;
                        }

                        break;
                    case "1":
                                              

                        dtPayout = fnGetsqlTradePlus(Code, select, date, formate, cmbFormat, chkInclOnDemPayOut);

                        //dtPayout = myLib.OpenDataTable(strsql, curCon);



                        if (strMode == "A")
                        {
                            strsql = strsql + "truncate table #Style2";
                            myLib.ExecSQL(strsql, curCon);

                        }

                       
                        iRow = 0;
                        while (iRow < dtPayout.Rows.Count)
                        {
                            strGroupCode = (dtPayout.Rows[iRow]["Client"].ToString());
                            dblAmt1 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt1"]);
                            dblAmt2 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt2"]);
                            dblAmt3 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt3"]);

                            strsql = "Insert into #Style2 (Client , Name,dp_id, Amt1, Amt2, amt3,RAmt, minamt) Values ('" + (dtPayout.Rows[iRow]["Client"]).ToString().Trim() + "','";
                            strsql = strsql + Strings.Left((dtPayout.Rows[iRow]["Name"].ToString()), 50) + "','" + (dtPayout.Rows[iRow]["dp_id"]).ToString().Trim() + "'," + dblAmt1 + "," + dblAmt2 + "," + dblAmt3 + "," + (dtPayout.Rows[iRow]["RAmt"]).ToString().Trim() + ",";
                            strsql = strsql + dblAmt3 + ")";
                            myLib.ExecSQL(strsql, curCon);
                            iRow = iRow + 1;
                        }
                        break;

                }


                        strsql = "Select client,cast(round(sum(RAmt),2) as decimal (15,2)) as RAmt, name, cast(round(sum(amt1),2) as decimal (15,2)) as amt1 , ";
                        strsql += " cast(round(-1 * sum(amt2),2) as decimal (15,2)) as amt2,cast(round(-1 * sum(minamt),2) as decimal (15,2)) as minamt ";

                        if (Convert.ToInt32(myLib.fnFireQuery("sysobjects", "count(0)", "name", "FundsRequest", true)) > 0)
                        {
                            FundsRequest = "YES";
                            strsql += " ,isnull(CAST((select sum(Rq_Amount) Rq_Amount from FundsRequest where Rq_Satus1='P' and Rq_Clientcd = client) as decimal(15,2)),0) ClReqAmt";
                        }
                        else
                            strsql += " ,0 ClReqAmt";
                        strsql += " from #Style2  group by client,name having sum(minamt)<0  order by client";
                
                dtPayout = myLib.OpenDataTable(strsql, curCon);


                int i = 1;
                object info;
                dt.Columns.Add("client", typeof(string));
                dt.Columns.Add("RAmt", typeof(string));
                dt.Columns.Add("name", typeof(string));
                dt.Columns.Add("amt2", typeof(string));
                dt.Columns.Add("minamt", typeof(string));
                dt.Columns.Add("ClReqAmt", typeof(string));

                for (int j = 0; j < dtPayout.Rows.Count; j++)
                {
                    DataRow tempRow = dt.NewRow();
                    tempRow[0] = dtPayout.Rows[j][0];
                    tempRow[1] = dtPayout.Rows[j][1];
                    tempRow[2] = dtPayout.Rows[j][2];
                    tempRow[3] = dtPayout.Rows[j][4];
                    tempRow[4] = dtPayout.Rows[j][5];
                    tempRow[5] = dtPayout.Rows[j][6];

                    dt.Rows.Add(tempRow);

                    if (PBValue == null)
                    {
                        PBValue = "10";
                    }
                }



                //ObjAdapter.Fill(ObjDataset, "Tem")


                //strsql = "Delete from " + strTmptblReport + " where amt1 = 0 ";
                //myLib.ExecSQL(strsql);

                ////if (chkInclOnDemPayOut.Checked = False)
                ////{
                ////    strsql = "delete " + strTmptblReport + " from " + strTmptblReport + " inner join client_info on client=cm2_cd and cm_fundpayout <> 'A' ";
                ////}
                ////myLib.ExecSQL(strsql);

                //strsql = "update " + strTmptblReport + " set RAmt = isnull (  (select sum(rq_amt) from payout_release where rq_clientcd = Client and rq_relflag='N' ) , 0 )";
                //myLib.ExecSQL(strsql);

                //strsql = "truncate table " + strTmptblReport;
                //myLib.ExecSQL(strsql);

                //strAmt2 = " sum (case When ld_Dt <= '" + strDt + "' Then ld_amount else 0 end) ";
                //strAmt3 = " sum (case When ld_Dt <= '" + strDt + "' Then ld_amount else 0 end) + case When ( sum (case When ld_Dt > '" + strDt + "' Then ld_amount else 0 end)) > 0 Then sum (case When ld_Dt > '" + strDt + "' Then ld_amount else 0 end)	else 0 end ";

                //strsql = "Insert into " + strTmptblReport + " Select  cm_cd ,cm_name," + ((Code == "") ? "''" : "ld_dpid") + ", ";
                //strsql = strsql + " sum(ld_amount) amt1, ";
                //strsql = strsql + " Case When " + strAmt2 + " > 0 Then 0 else " + strAmt2 + " end amt2 , ";
                //strsql = strsql + " Case When " + strAmt3 + " > 0 Then 0 else " + strAmt3 + " end amt3 ";
                //strsql = strsql + " ,0 , max(case when L1.ld_dt > '" + strDt + "'  then ld_dt else 0 end) dt ";
                //strsql = strsql + " from Client_Master, Ledger L1 ";
                //strsql = strsql + " where (L1.ld_clientcd = cm_cd" + strMarginWhere + ") " + strClientcd.Trim() + " " + strWhere + strClientWhere;

                //if (StrSubBrok != "")
                //{
                //    strsql += " and cm_schedule in ('" + CMSCHEDULE + "'" + StrSubBrok + " ) ";
                //}
                //else
                //{
                //    strsql += " and cm_schedule=" + CMSCHEDULE + " and cm_freezeyn = 'N'";
                //}

                //strsql = strsql + " and exists " + myutil.LoginAccess("ClientCode");
                //strsql = strsql + " group by  cm_cd  ,cm_brkggroup,cm_name" + IIf(Session("ClientCode") = Nothing, "", ",ld_dpid");
                //strsql = strsql + " Having ( " + strAmt3 + " ) < 0";
                //strsql = strsql + " order by cm_cd  ";
                //myLib.ExecSQL(strsql);

                //if (false)//if (chkInclOnDemPayOut.Checked = False)
                //{
                //    strsql = "delete " + strTmptblReport + " from " + strTmptblReport + " inner join client_info on client=cm2_cd and cm_fundpayout <> 'A' ";
                //    myLib.ExecSQL(strsql);
                //}
                //strsql = "update " + strTmptblReport + " set RAmt = isnull ( (select sum(rq_amt) from payout_release where rq_clientcd = Client and rq_relflag='N' ) , 0 )";
                //myLib.ExecSQL(strsql);

                //strsql = "Select * from " + strTmptblReport + "";
                //myLib.ExecSQL(strsql);

                // fnGetsql = strsql;



            }
            HttpContext.Current.Session["DTRequest"] = dt;
            return dt;

        }

        public DataTable fnGetsqlTradePlus(string Code, string select, string date, string formate, string cmbFormat,Boolean chkInclOnDemPayOut)
        {
            DataTable dtPayout = null;
            string strsql = null;
            string StrSubBrok = null;
            string strTmptblReport = "#tmptblReport";
            string strClientWhere = "";
            string strClientcd;
            string strDt;
            string strWhere = "";
            string strMatchALL = "";
            string flag;
            string CMSCHEDULE = "49843750";
            //Boolean chkInclOnDemPayOut = true;
            string strMode;
            string strBal;
            string strldDP;
            string strFields;
            string strMarginWhere = "";
            string strAmt2 = "";
            string strAmt3 = "";

            string gstrDBToday = DateTime.Today.ToString("yyyyMMdd");
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            string[] arr;
            strMode = "A";
            StrSubBrok = myLib.fnGetSysParam("REMSCHEDULE");
            if (StrSubBrok.Trim().Length > 0)
            {
                arr = Strings.Split(StrSubBrok, ",");
                StrSubBrok = string.Join("','", arr);
            }
            StrSubBrok = ((StrSubBrok != "") ? ",'" + StrSubBrok + "'" : "");

            strMode = "V";
            HttpContext.Current.Session["blnExecute"] = false;

            if (Code != "")
            {
                if (select == "CL")
                {
                    strClientWhere = " and cm_cd = '" + Code + "'";
                }
                else if (select == "BR")
                {
                    strClientWhere = " and cm_brboffcode = '" + Code + "'";
                }
                else if (select == "GR")
                {
                    strClientWhere = " and cm_groupcd = '" + Code + "'";
                }
                else if (select == "FM")
                {
                    strClientWhere = " and cm_familycd = '" + Code + "'";
                }
                else if (select == "SB")
                {
                    strClientWhere = " and cm_subbroker = '" + Code + "'";
                }
            }

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();


                if (cmbFormat == "0")
                {

                    myLib.ExecSQL("Drop Table " + strTmptblReport + "", curCon);

                    strsql = " CREATE TABLE " + strTmptblReport + " ( ";
                    strsql = strsql + " [Client] [char](8) NOT NULL ,";
                    strsql = strsql + " [Name] [char](75) NOT NULL ,";
                    strsql = strsql + " [dp_id] [char](10) NOT NULL ,";
                    strsql = strsql + " [amt1] [money] NOT NULL, ";
                    strsql = strsql + " [amt2] [money] NOT NULL, ";
                    strsql = strsql + " [amt3] [money] not null, ";
                    strsql = strsql + " [RAmt] [money] not null, ";
                    strsql = strsql + " [dt] [char](8) NOT NULL ";
                    strsql = strsql + " ) ";
                    myLib.ExecSQL(strsql, curCon);

                }
                else if (cmbFormat == "1")
                {
                    myLib.ExecSQL("Drop Table " + strTmptblReport + "", curCon);

                    strsql = " CREATE TABLE " + strTmptblReport + " ( ";
                    strsql = strsql + " [Client] [char](8) NOT NULL ,";
                    strsql = strsql + " [Name] [char](75) NOT NULL ,";
                    strsql = strsql + " [dp_id] [char](10) NOT NULL ,";
                    strsql = strsql + " [amt1] [money] NOT NULL, ";
                    strsql = strsql + " [amt2] [money] NOT NULL, ";
                    strsql = strsql + " [amt3] [money] NOT NULL, ";
                    strsql = strsql + " [RAmt] [money] not null, ";
                    strsql = strsql + " [dt] [char](8) NOT NULL ";
                    strsql = strsql + " ) ";

                    myLib.ExecSQL(strsql, curCon);
                }


                strsql = "select Tnc_Filler1,Tnc_Filler2 from tradenetcontrol where Tnc_optcode='946'";
                dtPayout = myLib.OpenDataTable(strsql, curCon);


                strClientcd = "";
                strDt = myutil.dtos(date);
                //strDt = date;
                strWhere = " and (";
                strWhere = strWhere + ")";

                if (Strings.Right(strWhere, (strMatchALL).Length + 1) == strMatchALL + ")")
                {
                    strWhere = Strings.Left(strWhere, (strWhere).Length - (strMatchALL).Length - 1) + ")";

                }

                if (strWhere.Trim() == "and ()")
                {
                    strWhere = "";
                }

                strBal = " having ";
                if (strBal.Trim() == "having")
                {
                    strBal = "";
                }
                if (HttpContext.Current.Session["ClientCode"] == null)
                {

                    if (strBal == "")
                    {

                        strBal = "having sign(sum(L1.ld_amount))< 0";
                    }

                    else


                        strBal = strBal + " and sign(sum(L1.ld_amount))< 0 ";

                }
                else
                {
                    strBal = " ,ld_dpid";
                    strldDP = " and L1.ld_dpid = L2.ld_dpid ";
                }


                strClientcd = (HttpContext.Current.Session["ClientCode"] == null) ? "" : ("and cm_cd='" + HttpContext.Current.Session["ClientCode"].ToString().Trim() + "'");
                strFields = " cm_cd, cm_name, cm_groupcd, gr_desc, cm_familycd, fm_desc, cm_add1, cm_add2, cm_add3, cm_tele1, cm_tele2 ";


                //---------------------------------------**********************
                //As Per RMS Amount , added on 13 / 08 / 2009

                if (dtPayout.Rows.Count > 0)
                {
                    if ((dtPayout.Rows[0]["Tnc_Filler1"]).ToString().Trim() == "R")
                    {
                        // strsql = "truncate table " + strTmptblReport;
                        //myLib.ExecSQL(strsql, curCon);


                        strDt = myutil.dtos(date);
                        // strDt = date;
                        if (HttpContext.Current.Session["ClientCode"] == null)
                        {

                            strsql = " Insert into " + strTmptblReport + " Select cm_cd ,cm_name,rs_companycode, sum(rs_fundpayout) as amt1,  ";

                            strsql += "0 amt2 ,  sum(rs_fundpayout) amt3 , 0, Max(rs_dt) dt     from Client_Master, Rms_summary ";

                            strsql += "where rs_clientcd = cm_cd ";
                            if (StrSubBrok != "")
                            {
                                strsql += " and cm_schedule in ('" + CMSCHEDULE + "'" + StrSubBrok + " ) ";
                            }
                            else
                            {
                                strsql += " and cm_schedule=" + CMSCHEDULE + " and cm_freezeyn = 'N'";
                            }
                            // strsql += strClientWhere + " and rs_dt = '" + strDt + "' and ( cm_brboffcode='000012' ) and exists " + myutil.LoginAccess("cm_cd") + ((Code == "") ? "" : "and cm_cd = '" + Code.Trim() + "'") + "";
                            strsql += strClientWhere + " and rs_dt = '" + strDt + "' and exists " + myutil.LoginAccess("rs_clientcd") + ((HttpContext.Current.Session["ClientCode"] == null) ? "" : "and cm_cd = '" + HttpContext.Current.Session["ClientCode"].ToString().Trim() + "'") + "";   //000012
                            strsql += "group by  cm_cd ,cm_brkggroup,cm_name,rs_companycode ";
                            strsql += "having sign(sum(rs_fundpayout))< 0 order by cm_cd ";
                        }
                        else
                        {
                            strsql = " Insert into " + strTmptblReport + " select  cm_cd ,cm_name,ld_dpid,sum(ld_amount) amt1,0 amt2,sum(ld_amount) amt3,0,0 ";
                            strsql += "from  Client_Master, Ledger  where (ld_clientcd = cm_cd) ";


                            if (StrSubBrok != "")
                            {
                                strsql += " and cm_schedule in ('" + CMSCHEDULE + "'" + StrSubBrok + " ) ";
                            }
                            else
                            {
                                strsql += " and cm_schedule=" + CMSCHEDULE;

                            }
                          strsql += " and ld_clientcd = '" + HttpContext.Current.Session["ClientCode"].ToString().Trim() + "' and exists " + myutil.LoginAccess("ld_clientcd");
                            strsql += "group by  cm_cd ,cm_brkggroup,cm_name,ld_dpid order by cm_cd ";
                        }


                        flag = "RMS";

                        myLib.ExecSQL(strsql, curCon);

                        strsql = "Delete from " + strTmptblReport + " where amt1 = 0 ";
                        myLib.ExecSQL(strsql, curCon);

                        if (chkInclOnDemPayOut == false)
                        {
                            strsql = "delete " + strTmptblReport + " from " + strTmptblReport + " inner join client_info on client=cm2_cd and cm_fundpayout <> 'A' ";
                            myLib.ExecSQL(strsql, curCon);
                        }
                        if (flag == "RMS")
                        {
                            if (HttpContext.Current.Session["ClientCode"] == null)
                            {
                                strsql = "update " + strTmptblReport + " set RAmt = isnull((select sum(rq_amt) from payout_release where rq_clientcd = Client and rq_relflag='N'), 0 )";
                            }
                            else
                                strsql = "update " + strTmptblReport + " set RAmt = isnull((select sum(rq_amt) from payout_release where rq_clientcd = Client and rq_dpid = dp_ID and rq_relflag='N'), 0 )";
                        }
                        else
                            strsql = "update " + strTmptblReport + " set RAmt = isnull((select sum(rq_amt) from payout_release where rq_clientcd = Client and rq_relflag='N'), 0 )";


                        myLib.ExecSQL(strsql, curCon);

                        strsql = "Select * from " + strTmptblReport + "";
                        dtPayout = myLib.OpenDataTable(strsql, curCon);
                        return dtPayout;
                    }

                }
           
            if (dtPayout.Rows.Count > 0)
            {
                if ((dtPayout.Rows[0]["Tnc_Filler2"]).ToString().Trim()== "1")
                {
                    strMarginWhere = " Or L1.ld_clientcd = cm_brkggroup ";
                }
            }

            if (formate == "0")
            {
                //------Consider Margin Account also 
                strsql = "truncate table " + strTmptblReport;
                myLib.ExecSQL(strsql, curCon);

                strDt = myutil.dtos(date);
                strAmt2 = " sum (case When ld_Dt <= '" + strDt + "' Then ld_amount else 0 end) ";
                strAmt3 = " sum (case When ld_Dt <= '" + strDt + "' Then ld_amount else 0 end) + sum (case When ld_Dt > '" + strDt + "' and ld_debitflag = 'D' Then ld_amount else 0 end) ";
                strsql = "Insert into " + strTmptblReport + " Select cm_cd ,cm_name," + Interaction.IIf(HttpContext.Current.Session["ClientCode"] == null, "''", "ld_dpid") + " , ";
                strsql = strsql + " sum(ld_amount) amt1, ";
                strsql = strsql + " Case When " + strAmt2 + " > 0 Then 0 else " + strAmt2 + " end amt2 , ";
                strsql = strsql + " Case When " + strAmt3 + " > 0 Then 0 else " + strAmt3 + " end amt3 ,";
                strsql = strsql + " 0,max(Case When ld_dt > '" + strDt + "' and ld_debitflag = 'D' then ld_dt else 0 end) dt ";
                strsql = strsql + " from Client_Master, Ledger L1 ";
                strsql = strsql + " where (L1.ld_clientcd = cm_cd" + strMarginWhere + ") " + Strings.Trim(strClientcd) + " " + strWhere + strClientWhere;

                if (StrSubBrok != "")
                {
                    strsql += " and cm_schedule in ('" + CMSCHEDULE + "'" + StrSubBrok + " ) ";
                }
                else
                {
                    strsql += " and cm_schedule = 49843750  and cm_freezeyn = 'N'";
                }
                strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                strsql = strsql + " group by  cm_cd ,cm_brkggroup,cm_name" + Interaction.IIf(HttpContext.Current.Session["ClientCode"] == null, "", ",ld_dpid");
                strsql = strsql + " Having ( " + strAmt3 + " ) < 0";
                strsql = strsql + " order by cm_cd";
                myLib.ExecSQL(strsql, curCon);

                strsql = "Delete from " + strTmptblReport + " where amt1 = 0 ";
                myLib.ExecSQL(strsql, curCon);

                if (chkInclOnDemPayOut == false)
                {
                    strsql = "delete " + strTmptblReport + " from " + strTmptblReport + " inner join client_info on client=cm2_cd and cm_fundpayout <> 'A' ";
                    myLib.ExecSQL(strsql, curCon);

                }
                strsql = "update " + strTmptblReport + " set RAmt = isnull ( (select sum(rq_amt) from payout_release where rq_clientcd = Client and rq_relflag='N' ) , 0 )";
                myLib.ExecSQL(strsql, curCon);
            }
            else if(formate == "1")
            {
                strsql = "truncate table " + strTmptblReport;
                myLib.ExecSQL(strsql, curCon);

                strDt = myutil.dtos(date);
                strAmt2 = " sum (case When ld_Dt <= '" + strDt + "' Then ld_amount else 0 end) ";
                strAmt3 = " sum (case When ld_Dt <= '" + strDt + "' Then ld_amount else 0 end) + case When ( sum (case When ld_Dt > '" + strDt + "' Then ld_amount else 0 end)) > 0 Then sum (case When ld_Dt > '" + strDt + "' Then ld_amount else 0 end)	else 0 end ";
                strsql = "Insert into " + strTmptblReport + " Select  cm_cd ,cm_name," + Interaction.IIf(HttpContext.Current.Session["ClientCode"] == null, "''", "ld_dpid") + ", ";
                strsql = strsql + " sum(ld_amount) amt1, ";
                strsql = strsql + " Case When " + strAmt2 + " > 0 Then 0 else " + strAmt2 + " end amt2 , ";
                strsql = strsql + " Case When " + strAmt3 + " > 0 Then 0 else " + strAmt3 + " end amt3 ";
                strsql = strsql + " ,0 , max(case when L1.ld_dt > '" + strDt + "'  then ld_dt else 0 end) dt ";
                strsql = strsql + " from Client_Master, Ledger L1 ";
                strsql = strsql + " where (L1.ld_clientcd = cm_cd" + strMarginWhere + ") " + Strings.Trim(strClientcd) + " " + strWhere + strClientWhere;

                if (StrSubBrok != "")
                {
                    strsql += " and cm_schedule in ('" + CMSCHEDULE + "'" + StrSubBrok + " ) ";
                }
                else
                {
                    strsql += " and cm_schedule=" + "CMSCHEDULE" + " and cm_freezeyn = 'N'";
                }

                strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                strsql = strsql + " group by  cm_cd  ,cm_brkggroup,cm_name" + Interaction.IIf(HttpContext.Current.Session["ClientCode"] == null, "", ",ld_dpid");
                strsql = strsql + " Having ( " + strAmt3 + " ) < 0";
                strsql = strsql + " order by cm_cd  ";
                myLib.ExecSQL(strsql, curCon);




                if (chkInclOnDemPayOut == false)
                {
                    strsql = "delete " + strTmptblReport + " from " + strTmptblReport + " inner join client_info on client=cm2_cd and cm_fundpayout <> 'A' ";
                    myLib.ExecSQL(strsql, curCon);
                }

             
            }

                strsql = "Select * from " + strTmptblReport + "";
                dtPayout = myLib.OpenDataTable(strsql, curCon);
            }
           






            return dtPayout;





        }

        public DataTable[] GetBulkPayoutRequest(string Code, string select, string date, string formate, string cmbFormat, Boolean chkInclOnDemPayOut)
        {
            List<DataTable> list = new List<DataTable>();
            DataTable dt = new DataTable();
            DataTable dtPayout = null;
            DataTable dtPayoutRequest = null;
            string strsql = null;
            string msg = "";
            string strMode = "";
            string StrSubBrok = null;
            string strTable1 = "#Style2";
            string strTmptblReport = "#tmptblReport";
            string strClientWhere = "";
            string strClientcd;
            string strDt = "";
            string PBValue = "";
            string strMatchALL = "";
            string strBal, strldDP;
            string RMS = "RMS";
            string CMSCHEDULE = "49843750";
            DataSet ObjDataset;
            string FundsRequest = "";
            string strMarginWhere;
            string strAmt1 = "";
            string strAmt2 = "";
            string strAmt3 = "";
          //  Boolean chkInclOnDemPayOut = false;
            HttpContext.Current.Session["ClientCode"] = Code;

            string gstrDBToday = DateTime.Today.ToString("yyyyMMdd");
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                double dblAmt1;
                double dblAmt2, dblAmt3, dblRequestAmt, dblFinal;
                string strGroupCode;

                dtPayout = fnGetsqlTradePlus(Code, select, date, formate, cmbFormat, chkInclOnDemPayOut);

                //dtPayout = myLib.OpenDataTable(strsql, curCon);

                myLib.ExecSQL("drop table " + strTable1 + "", curCon);

                strsql = " CREATE TABLE " + strTable1 + " ( ";
                strsql = strsql + " [Client] [char](8) NOT NULL ,";
                strsql = strsql + " [Name] [char](75) NULL,";
                strsql = strsql + " [dp_id] [char](10) NULL,";
                strsql = strsql + " [amt1] [money] NOT NULL, ";
                strsql = strsql + " [amt2] [money] NOT NULL, ";
                strsql = strsql + " [amt3] [money] NOT NULL, ";
                strsql = strsql + " [RAmt] [money] not null, ";
                strsql = strsql + " [minamt] [money] NOT NULL, ";
                strsql = strsql + " srno [numeric](18, 0) IDENTITY (1, 1) NOT NULL primary key";
                strsql = strsql + " ) ";
                myLib.ExecSQL(strsql, curCon);

                if (strMode == "A")
                {
                    strsql = strsql + "truncate table #Style2";
                    myLib.ExecSQL(strsql, curCon);

                }

                int iRow;
                iRow = 0;
                if (dtPayout.Rows.Count > 0)
                {

                    while (iRow < dtPayout.Rows.Count)
                    {
                        strGroupCode = (dtPayout.Rows[iRow]["Client"].ToString());
                        dblAmt1 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt1"]);
                        dblAmt2 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt2"]);
                        dblAmt3 = Convert.ToDouble(dtPayout.Rows[iRow]["Amt3"]);

                        strsql = "Insert into #Style2 (Client , Name,dp_id, Amt1, Amt2, amt3,RAmt, minamt) Values ('" + (dtPayout.Rows[iRow]["Client"]).ToString().Trim() + "','";
                        strsql = strsql + Strings.Left((dtPayout.Rows[iRow]["Name"].ToString()), 50) + "','" + (dtPayout.Rows[iRow]["dp_id"]).ToString().Trim() + "'," + dblAmt1 + "," + dblAmt2 + "," + dblAmt3 + "," + (dtPayout.Rows[iRow]["RAmt"]).ToString().Trim() + ",";
                        strsql = strsql + dblAmt3 + ")";
                        myLib.ExecSQL(strsql, curCon);
                        iRow = iRow + 1;
                    }
                }
                if ((string)HttpContext.Current.Session["RMS"] == "RMS")
                {
                    strsql = "Select (ltrim(rtrim(ces_exchange)) + '/' + ltrim(rtrim(ces_segment)))Exch ,dp_id,client, name, cast(round(amt1, 2) as decimal (15, 2)) as amt1 , cast(round(amt2, 2) as decimal (15, 2)) as amt2, cast(round(-1 * minamt, 2) as decimal (15, 2)) as minamt,srno from #Style2 , companyexchangesegments where CES_CompanyCd = '" + HttpContext.Current.Session["CompanyCode"] + "' and dp_id = ces_Cd order by client";
                    dtPayoutRequest = myLib.OpenDataTable(strsql, curCon);
                    list.Add(dtPayoutRequest);
                }
                else
                {
                    strsql = "Select (ces_exchange + ces_segment)Exch ,dp_id,client, name, cast(round(amt1,2) as decimal (15,2)) as amt1 , cast(round(-1 * amt2,2) as decimal (15,2)) as amt2, cast(round(-1 * minamt,2) as decimal (15,2)) as minamt,srno from #Style2 , companyexchangesegments where  dp_id = ces_Cd order by client ";

                    dtPayoutRequest = myLib.OpenDataTable(strsql, curCon);
                    list.Add(dtPayoutRequest);

                }

                if (dtPayoutRequest.Rows.Count > 0)
                    strsql = "select sum(rq_amt) as requestamount,rq_dpid,(RTRIM(CES_Exchange)+'/'+CES_Segment) as Seg from payout_release a join CompanyExchangeSegments b on a.rq_dpid=b.CES_Cd where rq_clientcd='" + Code + "' and rq_relflag='N' group by rq_dpid,(RTRIM(CES_Exchange)+'/'+CES_Segment)"; // & ObjDataset.Tables("Seg").Rows(0).Item(0) & "'  "
                dt = myLib.OpenDataTable(strsql, curCon);


                list.Add(dt);

          



                return list.ToArray();

            }
            HttpContext.Current.Session["ClientCode"] = null;
        }

        public void SaveBulkPayout(IEnumerable<BulkPayoutRequestModel> BulkPayout)
        {
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            DataTable dt = new DataTable();
            string strsql = "";
            string code = "";
            try
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();

                    foreach (var item in BulkPayout)
                    {
                        code = item.ClientCode;
                    }

                    if (Convert.ToInt32(mylib.fnFireQuery("sysobjects", "count(0)", "name", "FundsRequest", true, curCon)) > 0)
                    {
                        // Update all request from  client  16/05/2018 
                        strsql = "Update FundsRequest set Rq_Satus1='R',Rq_Note='Changed By Branch' WHERE rq_clientcd='" + code.Trim() + "' and Rq_Satus1='P'";
                        mylib.ExecSQL(strsql, curCon);
                    }

                    strsql = "select * from payout_release where rq_clientcd='" + code.Trim() + "' and rq_relflag = 'N' ";
                    dt = mylib.OpenDataTable(strsql, curCon);

                    if (dt.Rows.Count > 0)
                    {
                        strsql = "DELETE  payout_release WHERE rq_clientcd='" + code.Trim() + "' and rq_relflag = 'N'";
                        mylib.ExecSQL(strsql, curCon);
                    }
                    string strTime = mylib.fnGetTime(curCon);
                    foreach (var item in BulkPayout)
                    {
                        if (item.RequestAmount != 0)
                        {
                            strsql = "insert into payout_release(rq_dpid,rq_clientcd,rq_amt,rq_date,rq_relAmt,rq_relflag,rq_RcSrNo,mkrid,mkrdt,mkrtm,machineId)";
                            strsql = strsql + " values ('" + item.dp_id + "','" + code + "','" + item.RequestAmount + "','" + myutil.dtos(item.strDt) + "','0','N','0','" + myutil.gstrUserCd() + "','" + myutil.gstrDBToday() + "','" + strTime + "','" + Environment.MachineName + "')";
                            mylib.ExecSQL(strsql, curCon);
                        }

                    }


                }
            }


            catch (Exception ex)
            {
                string strMsg = ex.Message;
                throw;
            }
        }


        public void RemoveAllBulkPR()
        {
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            DataTable dt = new DataTable();
            string strsql = "";
            string StrSubBrok = "";
            string code = "";
            string CMSCHEDULE = "49843750";
            try
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();

                    //StrSubBrok = mylib.fnGetSysParam("REMSCHEDULE");
                    //StrSubBrok = ((StrSubBrok != "") ? ",'" + StrSubBrok + "'" : "");

                    if (Convert.ToInt32(mylib.fnFireQuery("sysobjects", "count(0)", "name", "FundsRequest", true, curCon)) > 0)
                    {
                        strsql = "Update FundsRequest set Rq_Satus1='D',Rq_Note='Deleted By Branch' from client_master WHERE rq_clientcd=cm_cd and Rq_Satus1='P' and  cm_schedule in ('" + CMSCHEDULE + "'" + StrSubBrok + " )" + " and exists " + myutil.LoginAccess("rq_clientcd");                              /* "and " + "(cm_brboffcode='000012')";*/
                        mylib.ExecSQL(strsql, curCon);
                    }
                    string strTime = mylib.fnGetTime(curCon);
                  
                            strsql = "update payout_release set rq_relflag='D',mkrid='" + myutil.gstrUserCd() + "',mkrdt='" + myutil.gstrDBToday() + "',mkrtm='" + strTime + "',machineId='" + Environment.MachineName + "' from client_master WHERE rq_clientcd=cm_cd and rq_relflag = 'N' and  cm_schedule in ('" + CMSCHEDULE + "'" + StrSubBrok + " )" + " and exists " + myutil.LoginAccess("rq_clientcd");                             /*"and " + "(cm_brboffcode='000012')";*/
                            mylib.ExecSQL(strsql, curCon);
                     
                }
            }


            catch (Exception ex)
            {
                string strMsg = ex.Message;
                throw;
            }
        }
        public void SaveBulkFullPR(string strDt)
        {
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            DataTable dt = new DataTable();
            string strsql = "";
            string StrSubBrok = "";
            string code = "";
            string CMSCHEDULE = "49843750";
            try
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();

                    //StrSubBrok = mylib.fnGetSysParam("REMSCHEDULE");
                    //StrSubBrok = ((StrSubBrok != "") ? ",'" + StrSubBrok + "'" : "");

                    DataTable RDT = (DataTable)HttpContext.Current.Session["DTRequest"];

                    for (int i = 0; i < RDT.Rows.Count; i++)
                    {
                        double dblRmsAmt = Convert.ToDouble(RDT.Rows[i][4]);
                        string strclient = RDT.Rows[i][0].ToString();
                        PrAutorequest(strclient, dblRmsAmt, strDt);
                    }
                }
            }


            catch (Exception ex)
            {
                string strMsg = ex.Message;
                throw;
            }
        }

        private void PrAutorequest(string strclient, double dblRmsamt, string strDt)
        {
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            string strsql = "";
            DataTable dtPayout = new DataTable();
            string StrSubBrok = "";
            string CMSCHEDULE = "49843750";
            strDt = myutil.dtos(strDt);

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                StrSubBrok = mylib.fnGetSysParam("REMSCHEDULE");
                StrSubBrok = ((StrSubBrok != "") ? ",'" + StrSubBrok + "'" : "");
                if (Convert.ToInt32(mylib.fnFireQuery("sysobjects", "count(0)", "name", "FundsRequest", true, curCon)) > 0)
                {
                    strsql = "Update FundsRequest set Rq_Satus1='R',Rq_Note='Changed By Branch' WHERE rq_clientcd='" + strclient + "' and Rq_Satus1='P'";
                    mylib.ExecSQL(strsql, curCon);
                }



                strsql = "DELETE  payout_release WHERE rq_clientcd='" + strclient + "' and rq_relflag = 'N'";
                mylib.ExecSQL(strsql, curCon);

                strsql = "  select  cm_cd ,cm_name,ld_dpid,-sum(ld_amount) amt from  Client_Master, Ledger  ";
                strsql += "  where (ld_clientcd = cm_cd) and ld_clientcd = '" + strclient + "'";
                if (StrSubBrok != "")
                    strsql += " and cm_schedule in ('" + CMSCHEDULE + "'" + StrSubBrok + " ) ";
                else
                    strsql += " and cm_schedule=" + CMSCHEDULE;
                strsql += "  group by  cm_cd ,cm_brkggroup,cm_name,ld_dpid having -sum(ld_amount) > 0";
                dtPayout = mylib.OpenDataTable(strsql, curCon);
                string strTime = mylib.fnGetTime(curCon);
                if (dtPayout.Rows.Count != 0)
                {
                    for (int i = 0; i < dtPayout.Rows.Count; i++)
                    {
                        double dblPayable = Convert.ToDouble(dtPayout.Rows[i]["amt"]);
                        strsql = "insert into payout_release(rq_dpid,rq_clientcd,rq_amt,rq_date,rq_relAmt,rq_relflag,rq_RcSrNo,mkrid,mkrdt,mkrtm,machineId)";
                        strsql = strsql + " values ('" + (dtPayout.Rows[i]["ld_dpid"]).ToString().Trim() + "','" + strclient + "'," + ((dblPayable > dblRmsamt) ? dblRmsamt : dblPayable) + ", '" + strDt + "',0,'N',0,'" + myutil.gstrUserCd() + "','" + myutil.gstrDBToday() + "','" + strTime + "','" + Environment.MachineName + "')";
                        mylib.ExecSQL(strsql, curCon);
                        dblRmsamt = dblRmsamt - dblPayable;
                        if (dblRmsamt <= 0)
                            break;
                    }
                }
            }
        }

        public void SaveBulkCheckPR(string strDt, Boolean chkAll, IEnumerable<string> ClientCode)
        {
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            string code = "";
            try
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();

                    DataTable dt = (DataTable)HttpContext.Current.Session["DTRequest"];
                    DataTable RDT = new DataTable();
                    if (chkAll)
                    {
                        RDT = (DataTable)HttpContext.Current.Session["DTRequest"];
                    }
                    else
                    {
                        code = "'" + string.Join("','", ClientCode) + "'";
                        RDT = dt.Select("client in(" + code + ")").CopyToDataTable();
                    }

                    for (int i = 0; i < RDT.Rows.Count; i++)
                    {
                        double dblRmsAmt = Convert.ToDouble(RDT.Rows[i][4]);
                        string strclient = RDT.Rows[i][0].ToString();
                        PrAutorequest(strclient, dblRmsAmt, strDt);
                    }
                }
            }


            catch (Exception ex)
            {
                string strMsg = ex.Message;
                throw;
            }
        }
    }
}