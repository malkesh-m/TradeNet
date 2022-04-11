using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace TRADENET.Models
{
    public class Report : ConnectionModel
    {
        public DataTable GetPerformance(string Select, string Code, string FromDate, string ToDate, string strDPID = "", string cmbRep = "", string cmbGroupBy = "")
        {

            string strwhere = "";
            string SQLstrwhere = "";

            string StrGroup1 = "";
            string strGroup2 = ""; ;
            string strTable1 = "";
            string strWhereIDWise = "";
            string strGroupBy = "";

            string strwhere1 = "";
            string strsql = "";
            string strsqlComm = "";
            string strTable = "";
            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            DataTable dt = null;
            //var ObjCommexCon = new SqlConnection();

            //DataSet  dsDetail = ObjUtility.DataRetriveCommex(strsql)
            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");

            string ExCommex = "";
            string Exchng = "";
            string[] strArray = strDPID.Split(',');
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
                if ((string)HttpContext.Current.Session["IsTplusCommex"] == "Y")
                {
                    if (obj.Substring(1, 1) == "X")
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
                }
                else
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

                }


                //your insert query
            }




            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {

                curCon.Open();
                try
                {
                    mylib.ExecSQL("Drop Table #TmpPerformance", curCon);
                }
                catch (Exception)
                {

                }
                finally
                {
                    //------------------------------Start create TmpPerformance table------------------------------------

                    strsql = " Create Table #TmpPerformance ( ";
                    strsql += " tmpcode varchar(60) not null, ";
                    strsql += " tmpclientcd varchar(8) not null, ";
                    strsql += " tmpname varchar(50) not null, ";
                    strsql += " tmpCD varchar(10) not null, ";
                    strsql += " tmpNM varchar(50) not null, ";
                    strsql += " tmpExchange varchar(10) not null, ";
                    strsql += " tmpSegment varchar(10) not null, ";
                    strsql += " tmpCashBuy numeric(26) not null, ";
                    strsql += " tmpCashSell numeric(26) not null , ";
                    strsql += " tmpCashBrok numeric(26) not null, ";
                    strsql += " tmpFutBuy numeric(26) not null, ";
                    strsql += " tmpFutSell numeric(26) not null , ";
                    strsql += " tmpFutBrok numeric(26) not null ) ";

                    mylib.ExecSQL(strsql, curCon);

                    //strsql = "Drop Table #TmpPerformance";
                    //mylib.ExecSQL(strsql, curCon);

                    //strsql = " Create Table #TmpPerformance ( ";
                    //strsql += " tmpcode varchar(60) not null, ";
                    //strsql += " tmpclientcd varchar(8) not null, ";
                    //strsql += " tmpname varchar(50) not null, ";
                    //strsql += " tmpCD varchar(10) not null, ";
                    //strsql += " tmpNM varchar(50) not null, ";
                    //strsql += " tmpExchange varchar(10) not null, ";
                    //strsql += " tmpSegment varchar(10) not null, ";
                    //strsql += " tmpCashBuy numeric(26) not null, ";
                    //strsql += " tmpCashSell numeric(26) not null , ";
                    //strsql += " tmpCashBrok numeric(26) not null, ";
                    //strsql += " tmpFutBuy numeric(26) not null, ";
                    //strsql += " tmpFutSell numeric(26) not null , ";
                    //strsql += " tmpFutBrok numeric(26) not null ) ";
                    //mylib.ExecSQL(strsql, curCon);

                    // ------------------------------Start create Condition table------------------------------------
                    if (Code != "")
                    {
                        if (Select == "Client")
                            SQLstrwhere += " and cm_Cd = '" + Code.Trim() + "' ";
                        else if (Select == "Family")
                            SQLstrwhere += " and cm_familycd = '" + Code.Trim() + "'"; // and cm_familycd= fm_cd "
                        else if (Select == "Group")
                            SQLstrwhere += " and cm_groupcd = '" + Code.Trim() + "' "; // and cm_groupcd=gr_cd "
                        else if (Select == "Sub-Broker")
                            SQLstrwhere += " and cm_subbroker = '" + Code.Trim() + "' "; // and cm_subbroker=rm_Cd "
                        else if (Select == "RM")
                            SQLstrwhere += " and rtrim(cm_dpactno) = '" + Code.Trim() + "'";
                        else if (Select == "BR")
                            SQLstrwhere += " and cm_brboffcode = '" + Code.Trim() + "'";
                        else if (Select == "ALL")
                            SQLstrwhere += " ";
                    }
                    //------------------------------


                    switch (cmbRep)
                    {
                        case "CL":
                            {
                                StrGroup1 = "td_clientcd Code, td_clientcd , cm_name  ";
                                break;
                            }

                        case "GR":
                            {
                                StrGroup1 = "rtrim(gr_desc)+' ['+cm_groupcd+']' Code,cm_groupcd td_clientcd , gr_desc cm_name ";
                                break;
                            }

                        case "FA":
                            {
                                StrGroup1 = "rtrim(fm_desc)+' ['+cm_familycd+']' Code ,cm_familycd td_clientcd , fm_desc cm_name ";
                                break;
                            }

                        case "SU":
                            {
                                StrGroup1 = "rtrim(sb.rm_name)+' ['+cm_subbroker+']' Code ,cm_subbroker td_clientcd , sb.rm_name cm_name  ";
                                break;
                            }

                        case "BR":
                            {
                                StrGroup1 = "rtrim(bm_branchname)+' ['+cm_brboffcode+']' Code,cm_brboffcode td_clientcd , bm_branchname cm_name ";
                                break;
                            }

                        case "RM":
                            {
                                StrGroup1 = "isnull(rtrim(rm.rm_name),'')+' ['+cm_dpactno +']' Code,cm_dpactno td_clientcd , rm.rm_name cm_name ";
                                break;
                            }
                    }

                    //--------------------------------------------------------------------------

                    string strGrpBy = "";
                    string strOrderBy = "";
                    string strWhere = "";
                    switch (cmbGroupBy)
                    {
                        case "NO":
                            {
                                strGroup2 = ",'' cd, '' names";
                                break;
                            }

                        case "CL":
                            {
                                strGroup2 = " , td_clientcd cd, cm_name names";
                                strGrpBy = " , td_clientcd , cm_name ";
                                strOrderBy = "  cm_name, ";
                                break;
                            }

                        case "GR":
                            {
                                strGroup2 = " ,cm_groupcd cd, gr_desc names ";
                                strTable1 = " ,Group_master";
                                strwhere1 = " and cm_groupcd=gr_cd ";
                                strGrpBy = " ,cm_groupcd , gr_desc ";
                                strOrderBy = "  gr_desc, ";
                                break;
                            }

                        case "FA":
                            {
                                strGroup2 = "  ,cm_familycd cd, fm_desc names ";
                                strTable1 = " ,Family_master ";
                                strwhere1 = " and cm_familycd=fm_cd ";
                                strGrpBy = "  ,cm_familycd , fm_desc ";
                                strOrderBy = " fm_desc, ";
                                break;
                            }

                        case "SU":
                            {
                                strGroup2 = "  ,cm_subbroker cd,sb.rm_name names ";
                                strTable1 = " ,subbrokers sb ";
                                strwhere1 = " and cm_subbroker=sb.rm_Cd ";
                                strGrpBy = "  ,cm_subbroker , sb.rm_name ";
                                strOrderBy = " sb.rm_name, ";
                                break;
                            }

                        case "BR":
                            {
                                strGroup2 = "  ,cm_brboffcode cd, bm_branchname names ";
                                strTable1 = " ,branch_master ";
                                strwhere1 = " and cm_brboffcode=bm_branchcd  ";
                                strGrpBy = "  ,cm_brboffcode , bm_branchname ";
                                strOrderBy = " bm_branchname, ";
                                break;
                            }

                        case "RM":
                            {
                                strGroup2 = " ,cm_dpactno cd, rm.rm_name names ";
                                strTable1 = " ,RM_master rm ";
                                strwhere1 = " and rtrim(cm_dpactno)=rm.rm_cd ";
                                strGrpBy = " ,cm_dpactno , rm.rm_name ";
                                strOrderBy = " rm.rm_name, ";
                                break;
                            }
                    }
                    //--------------------------

                    string stnSub = cmbRep.Substring(0, 1);
                    if (stnSub == "C")
                        strGroupBy = "  td_clientcd , cm_name  ";
                    else if (stnSub == "G")
                    {
                        strTable = " ,group_master ";
                        strWhere = " and cm_groupcd=gr_cd ";
                        strGroupBy = "  cm_groupcd, gr_desc ";
                    }
                    else if (stnSub == "F")
                    {
                        strTable = " ,family_master ";
                        strWhere = " and cm_familycd=fm_cd ";
                        strGroupBy = "  cm_familycd, fm_desc ";
                    }
                    else if (stnSub == "S")
                    {
                        strTable = " ,subbrokers sb ";
                        strWhere = " and cm_subbroker=sb.rm_Cd ";
                        strGroupBy = "  cm_subbroker, sb.rm_name ";
                    }
                    else if (stnSub == "B")
                    {
                        strTable = " ,branch_master ";
                        strWhere = " and cm_brboffcode=bm_branchcd  ";
                        strGroupBy = "  cm_brboffcode, bm_branchname ";
                    }
                    else if (stnSub == "R")
                    {
                        strTable = " ,RM_master rm ";
                        strWhere = " and rtrim(cm_dpactno)=rm.rm_cd ";
                        strGroupBy = "  cm_dpactno, rm.rm_name ";
                    }






                    //'' - FOR COMMODITY---------------------------------------------------------------------------------- -

                    if (SQLConnComex != null)
                    {

                        strsqlComm = " insert into #TmpPerformance(tmpcode,tmpclientcd,tmpname,tmpCD,tmpNM,tmpExchange,tmpSegment,tmpCashBuy,tmpCashSell,tmpCashBrok,tmpFutBuy,tmpFutSell,tmpFutBrok)";
                        strsqlComm = strsqlComm + " Select " + StrGroup1 + strGroup2 + ",";
                        strsqlComm = strsqlComm + " CES_Exchange , CES_Segment , 0,0,0,";
                        strsqlComm = strsqlComm + " cast( Sum(td_bqty*td_marketRate*sm_multiplier) as decimal(15,0)) FutBuy ,";
                        strsqlComm = strsqlComm + " cast( Sum(td_sqty*td_marketRate*sm_multiplier) as decimal(15,0)) FutSell, ";
                        strsqlComm = strsqlComm + " cast(Sum((td_bqty+td_sqty)*td_brokerage*sm_multiplier) as decimal(15,0)) FutBrok ";
                        strsqlComm = strsqlComm + " from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Trades," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Series_master," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.CompanyExchangeSegments ," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master ";
                        strsqlComm = strsqlComm + strTable.Replace(",", " , " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.") + strTable1.Replace(",", " , " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.");
                        strsqlComm = strsqlComm + " Where td_exchange = sm_exchange And td_seriesid = sm_Seriesid ";
                        strsqlComm = strsqlComm + " and td_clientcd = cm_cd ";

                        strsqlComm = strsqlComm + strWhereIDWise + SQLstrwhere;
                        strsqlComm = strsqlComm + " and td_dt between '" + FromDate + "' and '" + ToDate + "'   ";
                        strsqlComm = strsqlComm + " and substring(ces_cd,2,1) in ('" + (ExCommex.Replace("X", "")).Replace(",", "','") + "') ";
                        strsqlComm = strsqlComm + " and td_companyCode + td_exchange +'F'= CES_Cd ";
                        strsqlComm = strsqlComm + strWhere + strwhere1 + "  and exists " + myutil.LoginAccessCommex("td_clientcd") + SQLstrwhere;
                        strsqlComm = strsqlComm + "Group By CES_Exchange , CES_Segment ";
                        strsqlComm = strsqlComm + "," + strGroupBy + strGrpBy;
                        strsqlComm = strsqlComm + " Order by td_clientcd , CES_Exchange , CES_Segment  ";
                        mylib.ExecSQL(strsqlComm, curCon);



                        //  '' - Delivery Table Added -------------------------------------------------------------------------------------------------------

                        strsqlComm = " insert into #TmpPerformance(tmpcode,tmpclientcd,tmpname,tmpCD,tmpNM,tmpExchange,tmpSegment,tmpCashBuy,tmpCashSell,tmpCashBrok,tmpFutBuy,tmpFutSell,tmpFutBrok)";
                        strsqlComm = strsqlComm + " Select " + StrGroup1.Replace("td_", "dl_") + strGroup2.Replace("td_", "dl_") + ",";
                        strsqlComm = strsqlComm + " CES_Exchange , CES_Segment , ";
                        strsqlComm = strsqlComm + " cast( Sum(Case When Right(sm_prodtype,1) = 'F' Then dl_bqty*dl_marketRate*sm_multiplier else 0 end) as decimal(15,0)) FutBuy ,";
                        strsqlComm = strsqlComm + " cast( Sum(Case When Right(sm_prodtype,1) = 'F' Then dl_sqty*dl_marketRate*sm_multiplier else 0 end) as decimal(15,0)) FutSell, ";
                        strsqlComm = strsqlComm + " cast(Sum(Case When Right(sm_prodtype,1) = 'F' Then (dl_bqty+dl_sqty)*dl_brokerage*sm_multiplier else 0 end) as decimal(15,0)) FutBrok ";
                        strsqlComm = strsqlComm + " ,0,0,0 from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Delivery," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Series_master, " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.CompanyExchangeSegments ," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master ";
                        strsqlComm = strsqlComm + strTable.Replace(",", " , " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.") + strTable1.Replace(",", " , " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.");
                        strsqlComm = strsqlComm + " Where dl_exchange = sm_exchange And dl_seriesid = sm_Seriesid ";
                        strsqlComm = strsqlComm + " and dl_clientcd = cm_cd ";
                        strsqlComm = strsqlComm + strWhereIDWise.Replace("td_", "dl_");
                        strsqlComm = strsqlComm + " and dl_dt between '" + FromDate + "' and '" + ToDate + "'   ";
                        strsqlComm = strsqlComm + " and substring(ces_cd,2,1) in ('" + (ExCommex.Replace("X", "")).Replace(",", "','") + "') ";
                        strsqlComm = strsqlComm + " and dl_companyCode+dl_exchange+'F'= CES_Cd ";
                        strsqlComm = strsqlComm + strWhere.Replace("td_", "dl_") + strwhere1.Replace("td_", "dl_") + "   and exists " + myutil.LoginAccessCommex("dl_clientcd") + SQLstrwhere;
                        strsqlComm = strsqlComm + " Group By CES_Exchange , CES_Segment ";
                        strsqlComm = strsqlComm + "," + strGroupBy.Replace("td_", "dl_") + strGrpBy.Replace("td_", "dl_");

                        mylib.ExecSQL(strsqlComm, curCon);

                        //  '' - Delivery Table ended -----------------------------------------------------------------------------------
                    }
                    strsql = "Select " + StrGroup1 + strGroup2;
                    strsql += " ,Rtrim(CES_Exchange)+' / '+CES_Segment  as ExchSeg , ";
                    strsql += " cast(Sum( td_bqty*td_marketRate ) as decimal(15,0)) CashBuy ,cast(Sum( td_sqty*td_marketRate ) as decimal(15,0)) CashSell, ";
                    strsql += " cast(Sum( (td_bqty+td_sqty)*td_Brokerage ) as decimal(15,2)) CashBrok, ";
                    strsql += " 0 FutBuy ,0 FutSell , 0 FutBrok ,0 OptBuy,0 OptSell ,0 OptBrok ";
                    strsql += " from Trx, CompanyExchangeSegments , client_master ";
                    strsql += strTable + strTable1;
                    strsql += " Where CES_CompanyCd = '" + HttpContext.Current.Session["CompanyCode"] + "' and td_companycode+Left(td_stlmnt,1)+'C' = CES_Cd and td_clientcd = cm_cd ";
                    strsql += strWhereIDWise;
                    strsql += " and td_dt between '" + FromDate + "' and '" + ToDate + "'   " + " and exists " + myutil.LoginAccess("td_clientcd") + SQLstrwhere;
                    if (Exchng != "")
                        strsql += " and substring(ces_cd,2,2) in ('" + Exchng.Replace(",", "','") + "') ";

                    else
                        strsql += " and substring(ces_cd,2,2) in ('') ";

                    strsql += strWhere + strwhere1;
                    strsql += " Group By " + strGroupBy + strGrpBy + ",Rtrim(CES_Exchange)+' / '+CES_Segment  ";
                    strsql += " union ";

                    strsql += " select Code, td_clientcd , cm_name, cd, names , Rtrim(CES_Exchange)+' / '+CES_Segment  as ExchSeg ,  0 CashBuy ,0 CashSell,0 CashBrok, ";
                    strsql += " sum(FutBuy) FutBuy , sum(FutSell ) FutSell , sum(FutBrok ) FutBrok  , sum(OptBuy) OptBuy,sum(OptSell) OptSell , sum(OptBrok) OptBrok  ";
                    strsql += " from ( ";


                    strsql += " Select " + StrGroup1 + strGroup2;
                    strsql += " ,CES_Exchange , CES_Segment , ";
                    strsql += " 0 CashBuy ,0 CashSell,0 CashBrok,";
                    strsql += " cast( Sum(Case When Right(sm_prodtype,1) = 'F' Then td_bqty*td_marketRate*sm_multiplier else 0 end) as decimal(15,0)) FutBuy ,";
                    strsql += " cast( Sum(Case When Right(sm_prodtype,1) = 'F' Then td_sqty*td_marketRate*sm_multiplier else 0 end) as decimal(15,0)) FutSell ,";
                    strsql += " cast(Sum(Case When Right(sm_prodtype,1) = 'F' Then (td_bqty+td_sqty)*td_brokerage*sm_multiplier else 0 end) as decimal(15,0)) FutBrok , ";
                    strsql += " cast( Sum(Case When Right(sm_prodtype,1) = 'O' Then td_bqty*td_marketRate*sm_multiplier else 0 end) as decimal(15,0)) OptBuy ,";
                    strsql += " cast( Sum(Case When Right(sm_prodtype,1) = 'O' Then td_sqty*td_marketRate*sm_multiplier else 0 end) as decimal(15,0)) OptSell, ";
                    strsql += " cast(Sum(Case When Right(sm_prodtype,1) = 'O' Then (td_bqty+td_sqty)*td_brokerage*sm_multiplier else 0 end) as decimal(15,0)) OptBrok ";
                    strsql += " from Trades, Series_master, CompanyExchangeSegments , client_master ";
                    strsql += strTable + strTable1;
                    strsql += " Where CES_CompanyCd = '" + HttpContext.Current.Session["CompanyCode"] + "' and td_exchange = sm_exchange And td_seriesid = sm_Seriesid and td_segment = sm_segment";
                    strsql += strWhereIDWise;
                    //strsql += " and td_companycode+td_exchange+td_segment= CES_Cd and td_clientcd = cm_cd " + "and exists " + myutil.LoginAccess("td_clientcd") + SQLstrwhere;
                    strsql += " and td_companycode+td_exchange+td_segment= CES_Cd and td_clientcd = cm_cd " + "and exists " + myutil.LoginAccess("td_clientcd") + SQLstrwhere;
                    strsql += " and td_dt between '" + FromDate + "' and '" + ToDate + "'   ";
                    if (Exchng != "")
                        strsql += " and substring(ces_cd,2,2) in ('" + Exchng.Replace(",", "','") + "') ";
                    else
                        strsql += " and substring(ces_cd,2,2) in ('') ";

                    strsql += strWhere + strwhere1;
                    strsql += " Group By " + strGroupBy + strGrpBy + " ,CES_Exchange , CES_Segment ";

                    strsql += " union ";

                    string StrGroup3 = "";
                    string strGroup4 = "";
                    string strTable2 = "";
                    string strwhere2 = "";


                    switch (cmbRep)
                    {
                        case "CL":
                            {
                                StrGroup3 = " ex_clientcd Code, ex_clientcd , cm_name  ";
                                break;
                            }

                        case "GR":
                            {
                                StrGroup3 = " rtrim(gr_desc)+' ['+cm_groupcd+']' Code,cm_groupcd ex_clientcd , gr_desc cm_name ";
                                break;
                            }

                        case "FA":
                            {
                                StrGroup3 = " rtrim(fm_desc)+' ['+cm_familycd+']' Code , cm_familycd ex_clientcd , fm_desc cm_name";
                                break;
                            }

                        case "SU":
                            {
                                StrGroup3 = " rtrim(sb.rm_name)+' ['+cm_subbroker+']' Code , cm_subbroker ex_clientcd , sb.rm_name cm_name ";
                                break;
                            }

                        case "BR":
                            {
                                StrGroup3 = " rtrim(bm_branchname)+' ['+cm_brboffcode+']' Code, cm_brboffcode ex_clientcd , bm_branchname cm_name ";
                                break;
                            }

                        case "RM":
                            {
                                StrGroup3 = " isnull(rtrim(rm.rm_name),'')+' ['+cm_dpactno +']' Code, cm_dpactno ex_clientcd , rm.rm_name cm_name ";
                                break;
                            }
                    }
                    //----------------------------------------------
                    string strGrp = "";
                    switch (cmbGroupBy)
                    {
                        case "NO":
                            {
                                strGroup4 = " , '' cd, '' names";
                                break;
                            }

                        case "CL":
                            {
                                strGroup4 = " ,ex_clientcd cd , cm_name names ";
                                strGrp = " ,ex_clientcd , cm_name ";
                                break;
                            }

                        case "GR":
                            {
                                strGroup4 = " ,cm_groupcd cd , gr_desc names ";
                                strTable2 = " ,Group_master";
                                strwhere2 = " and cm_groupcd=gr_cd ";
                                strGrp = " ,cm_groupcd  , gr_desc  ";
                                break;
                            }

                        case "FA":
                            {
                                strGroup4 = "  ,cm_familycd cd, fm_desc names ";
                                strTable2 = " ,Family_master ";
                                strwhere2 = " and cm_familycd=fm_cd ";
                                strGrp = "  ,cm_familycd, fm_desc ";
                                break;
                            }

                        case "SU":
                            {
                                strGroup4 = "  ,cm_subbroker cd, sb.rm_name names ";
                                strTable2 = " ,subbrokers sb ";
                                strwhere2 = " and cm_subbroker=sb.rm_Cd ";
                                strGrp = "  ,cm_subbroker,sb.rm_name ";
                                break;
                            }

                        case "BR":
                            {
                                strGroup4 = "  ,cm_brboffcode cd, bm_branchname names ";
                                strTable2 = " ,branch_master ";
                                strwhere2 = " and cm_brboffcode=bm_branchcd ";
                                strGrp = "  ,cm_brboffcode, bm_branchname ";
                                break;
                            }

                        case "RM":
                            {
                                strGroup4 = " ,cm_dpactno cd, rm.rm_name names ";
                                strTable2 = " ,RM_master rm ";
                                strwhere2 = " and rtrim(cm_dpactno)=rm.rm_cd ";
                                strGrp = " ,cm_dpactno, rm.rm_name ";
                                break;
                            }
                    }

                    strsql += " Select " + StrGroup3 + strGroup4 + ", CES_Exchange , CES_Segment , ";
                    strsql += " 0 CashBuy ,0 CashSell,0 CashBrok,";
                    strsql += " cast( Sum(Case When Right(sm_prodtype,1) = 'F' Then ex_eqty*ex_mainbrdiffrate*sm_multiplier else 0 end) as decimal(15,0)) FutBuy ,";
                    strsql += " cast( Sum(Case When Right(sm_prodtype,1) = 'F' Then ex_aqty*ex_mainbrdiffrate*sm_multiplier else 0 end) as decimal(15,0)) FutSell ,";
                    strsql += " cast(Sum(Case When Right(sm_prodtype,1) = 'F' Then (ex_eqty+ex_aqty)*ex_brokerage*sm_multiplier else 0 end) as decimal(15,0)) FutBrok , ";
                    strsql += " cast( Sum(Case When Right(sm_prodtype,1) = 'O' Then ex_eqty*ex_mainbrdiffrate*sm_multiplier else 0 end) as decimal(15,0)) OptBuy ,";
                    strsql += " cast( Sum(Case When Right(sm_prodtype,1) = 'O' Then ex_aqty*ex_mainbrdiffrate*sm_multiplier else 0 end) as decimal(15,0)) OptSell, ";
                    strsql += " cast(Sum(Case When Right(sm_prodtype,1) = 'O' Then (ex_eqty+ex_aqty)*ex_brokerage*sm_multiplier else 0 end) as decimal(15,0)) OptBrok ";
                    strsql += " from Exercise, Series_master, CompanyExchangeSegments , client_master ";
                    strsql += strTable + strTable2;
                    strsql += " Where CES_CompanyCd = '" + HttpContext.Current.Session["CompanyCode"] + "' and ex_exchange = sm_exchange And ex_seriesid = sm_Seriesid ";
                    strsql += strWhereIDWise;
                    //strsql += " and ex_companycode+ex_exchange+ex_segment = CES_Cd and ex_clientcd = cm_cd " + "and exists " + myutil.LoginAccess("ex_clientcd") + SQLstrwhere;
                    strsql += " and ex_companycode+ex_exchange+ex_segment = CES_Cd and ex_clientcd = cm_cd " + "and exists " + myutil.LoginAccess("ex_clientcd") + SQLstrwhere;
                    strsql += " and ex_dt between '" + FromDate + "' and '" + ToDate + "'   ";

                    if (Exchng != "")
                        strsql += " and substring(ces_cd,2,2) in ('" + Exchng.Replace(",", "','") + "') ";
                    else
                        strsql += " and substring(ces_cd,2,2) in ('') ";

                    strsql += strWhere + strwhere2;
                    strsql += " Group By " + strGroupBy.Replace("td_clientcd", "ex_clientcd") + strGrp + " ,CES_Exchange , CES_Segment ";
                    strsql += " ) A ";
                    strsql += " Group By Code, td_clientcd , cm_name, cd, names ,Rtrim(CES_Exchange)+' / '+CES_Segment  ";

                    strsql += " union ";
                    strsql += " Select tmpcode, Tmpclientcd , Tmpname,tmpCD,tmpNM, TmpExchange +'/'+ 'Comm' as ExchSeg , ";
                    strsql += " TmpCashBuy , TmpCashSell ,TmpCashBrok , TmpFutBuy , TmpFutSell ,TmpFutBrok , 0 OptBuy , 0 OptSell , 0 OptBrok ";
                    strsql += " from [#TmpPerformance] ";
                    strsql += " Order by code,names,Rtrim(CES_Exchange)+' / '+CES_Segment   ";
                    dt = mylib.OpenDataTable(strsql, curCon);


                }

            }



            return dt;
        }

        public DataTable GetDeliverypending(string Code, string strDpid, string cmbSelect, string cmbReport, string strdate, string cmbBS)
        {
            string strField = "";
            string strClientWhere;
            string strMode = "p";
            strClientWhere = "";
            string strsql = "";
            //string strTpExchangeSeg;
            // string strComExchange;
            // strTpExchangeSeg = strDpid;
            // strComExchange = strDpid;
            DataTable dt = null;
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            strdate = myutil.dtos(strdate);

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                try
                {

                }
                catch (Exception)
                {

                }
                finally
                {

                    if (Code != "")
                    {
                        if (cmbSelect == "CL")
                            strClientWhere += " and cm_Cd = '" + Code.Trim() + "' ";
                        else if (cmbSelect == "FM")
                            strClientWhere += " and cm_familycd = '" + Code.Trim() + "'"; // and cm_familycd= fm_cd "
                        else if (cmbSelect == "GR")
                            strClientWhere += " and cm_groupcd = '" + Code.Trim() + "' "; // and cm_groupcd=gr_cd "
                        else if (cmbSelect == "SB")
                            strClientWhere += " and cm_subbroker = '" + Code.Trim() + "' "; // and cm_subbroker=rm_Cd "                       
                        else if (cmbSelect == "ALL")
                            strClientWhere += " ";
                    }
                    // strClientWhere = strClientWhere + cmbSelect.Replace("cm_cd", "dm_clientcd");
                    if (strMode == "E")
                    {
                        strField = "cm_email ,";
                    }

                    if (cmbReport == "1")
                        strsql = " select cm_name + (case when cm_poa = 'Y' then '(POA)' else '' end ) + [cm_Cd] as  GroupByValue ,";
                    else if (cmbReport == "2")
                        strsql = " select ss_name + '[' + dm_scripcd + ']'+' '+ 'ISIN :'+ isnull((select top 1 im_isin from Isin where im_scripcd = dm_scripcd order by im_priority),'') as GroupByValue,";
                    else if (cmbReport == "3")
                        strsql = " select gr_Desc + '['+ cm_GroupCd +']' as GroupByValue,";
                    else
                        strsql = " select cm_brboffcode + '-['+ bm_branchname + ']' as GroupByValue ,";
                    if (strDpid != "")
                        strsql = strsql + " demat.*, cm_name + (case when cm_poa = 'Y' then '(POA)' else '' end ) + cm_Cd as  ClientName,ss_name, dm_stlmnt,";
                    strsql = strsql + " cm_brboffcode,cm_groupcd,cm_familycd,cm_poa,bm_branchcd,bm_branchname,isnull(bm_email,'') bm_email, ";
                    strsql = strsql + " isnull((select top 1 im_isin from Isin where im_scripcd = dm_scripcd order by im_priority),'') im_isin, isnull(cm_email,'') cm_email,";
                    strsql = strsql + " gr_desc , fm_desc,(case when dm_qty < 0 then dm_qty * -1 else 0 end) Purchase,(case when dm_qty > 0 then dm_qty  else 0 end) Sell, ";
                    strsql = strsql + " isNull((select top 1 case When left(da_dpid,2) = 'IN' Then da_dpid + da_actno else da_actno end from DematAct,Dps Where da_dpid = dp_dpid and da_clientcd =dm_clientcd and da_defaultyn = 'Y'),'') Dematact";
                    strsql = strsql + " From Demat, Securities, Client_master, Branch_master, Ourdps, Group_master, Family_master, settlements ";
                    strsql = strsql + " where se_stdt = '" + strdate + "'";
                    strsql = strsql + " and dm_clientcd = cm_cd and od_cd = dm_ourdp and od_Acttype = 'P' ";
                    strsql = strsql + " and dm_stlmnt = se_stlmnt ";
                    strsql = strsql + " and cm_brboffcode = bm_branchcd and dm_scripcd=ss_cd ";
                    strsql = strsql + " and cm_groupcd = gr_cd and cm_Familycd = fm_cd ";
                    strsql = strsql + " and dm_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and left(dm_stlmnt,1)+'c' in ('" + strDpid.Replace(",", "','") + "') ";
                    strsql = strsql + " and ((dm_type = 'BC' and dm_locked = 'N' and dm_transfered = 'N') Or (dm_type = 'CB' and dm_locked = 'N' and dm_transfered <> 'S')) ";
                    //strsql = strsql + "and(cm_brboffcode = '000012')"; // and exists " + myutil.LoginAccess("ld_clientcd");
                    // strsql = strsql + " and exists " + myutil.LoginAccess("dm_clientcd");
                    strsql = strsql + strClientWhere + HttpContext.Current.Session["LoginAccessOld"];
                    //mylib.ExecSQL(strsql, curCon);

                    if (strMode == "E")
                    {
                        strsql = strsql + " and cm_email <> ''";
                    }


                    if (cmbBS == "1")
                        strsql = strsql + " and dm_qty < 0 ";
                    else if (cmbBS == "2")
                        strsql = strsql + " and dm_qty > 0 ";
                    if (cmbReport == "1")
                        strsql = strsql + " order by dm_clientcd,ss_name ";
                    else if (cmbReport == "2")
                        strsql = strsql + " order by dm_scripcd,ss_name ";
                    else if (cmbReport == "3")
                        strsql = strsql + " order by cm_groupcd,dm_clientcd,ss_name";
                    else if (cmbReport == "4")
                        strsql = strsql + " order by cm_brboffcode,dm_clientcd,ss_name ";

                    dt = mylib.OpenDataTable(strsql, curCon);


                }
                return dt;
            }
        }

        //public DataTable GetCMAReport(string Select, string Code, string strdate, string grouping, string chkOnlyShortFall)
        //{

        //    string strClientWhere = "";
        //    if (Code.Trim() != "")
        //    {
        //        switch (Select)
        //        {
        //            case "CL":
        //                {
        //                    strClientWhere = " and cm_cd = '" + Code + "'";
        //                    break;
        //                }

        //            case "GR":
        //                {
        //                    strClientWhere = " and cm_groupcd = '" + Code + "'";
        //                    break;
        //                }

        //            case "FM":
        //                {
        //                    strClientWhere = " and cm_familycd = '" + Code + "'";
        //                    break;
        //                }

        //            case "SB":
        //                {
        //                    strClientWhere = " and cm_subbroker = '" + Code + "'";
        //                    break;
        //                }

        //            case "BR":
        //                {
        //                    strClientWhere = " and cm_brboffcode = '" + Code + "'";
        //                    break;
        //                }
        //            case "ALL":
        //                {
        //                    strClientWhere = "";
        //                    break; ;
        //                }
        //        }
        //    }
        //    DataTable dt = null;
        //    DataTable dtCorpAct = new DataTable();
        //    string strSql = "";
        //    string IsTplusCommex = (string)HttpContext.Current.Session["IsTplusCommex"];
        //    LibraryModel mylib = new LibraryModel();
        //    UtilityModel myutil = new UtilityModel();
        //    UtilityDBModel mydbutil = new UtilityDBModel();
        //    SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
        //    strdate = myutil.dtos(strdate);
        //    using (SqlConnection curCon = new SqlConnection(connectionstring))
        //    {
        //        curCon.Open();


        //        strSql = " SELECT fm_companycode,fm_exchange,fm_Segment,1 SortORder,COUNT(0) CNT FROM FMargins,Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange,fm_Segment";
        //        if (IsTplusCommex == "Y" && SQLConnComex != null)
        //        {
        //            strSql += " union all " + " SELECT fm_companycode,fm_exchange,'X',2 SortORder,COUNT(0) FROM " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange ";

        //        }

        //        strSql += " order by fm_Segment,fm_exchange";



        //        dtCorpAct = mylib.OpenDataTable(strSql, curCon);

        //        if (dtCorpAct.Rows.Count > 0)
        //        {
        //            try
        //            {
        //                mylib.ExecSQL("Drop Table #TmpMarginAnalysis", curCon);
        //            }
        //            catch (Exception)
        //            {

        //            }
        //            finally
        //            {
        //                string strCreate = "";
        //                string strInsert = "";
        //                string strReportSelect = "";
        //                strCreate = " Create Table #TmpMarginAnalysis ( ";
        //                strCreate += " Tmp_Clientcd Varchar(8),";
        //                strCreate += " Tmp_Mrgn Money,";
        //                strCreate += " Tmp_Coll Money,";
        //                strCreate += " Tmp_Short Money , ";


        //                strCreate += " Tmp_Ledger Money , ";
        //                strCreate += " Tmp_BenfHolding Money , ";
        //                strCreate += " Tmp_AvailCollat Money , ";
        //                strCreate += " Tmp_AvailDP Money , ";
        //                strCreate += " Tmp_EarlyPayIn Money , ";
        //                strCreate += " Tmp_UnClear Money , ";
        //                strCreate += " Tmp_Collected1 Money , ";
        //                strCreate += " Tmp_OtherCollected Money , ";
        //                strCreate += " Tmp_OtherCollected1 Money , ";

        //                strInsert = " select fm_clientcd, ";
        //                strInsert += " 0 , ";
        //                strInsert += " 0 , ";
        //                strInsert += " 0, ";
        //                strInsert += " 0 , ";
        //                strInsert += " 0 , ";
        //                strInsert += " 0, ";
        //                strInsert += " 0 , ";
        //                strInsert += " 0 , ";
        //                strInsert += " 0, ";
        //                strInsert += " 0 , ";
        //                strInsert += " 0 , ";
        //                strInsert += " 0, ";

        //                strReportSelect = "  Tmp_Clientcd, ";
        //                strReportSelect += " cm_Name ,";
        //                strReportSelect += " Tmp_Mrgn Money,";
        //                strReportSelect += " Tmp_Coll Money,";
        //                strReportSelect += " Tmp_Short Money , ";
        //                strReportSelect += " Tmp_Ledger , ";
        //                strReportSelect += " Tmp_BenfHolding , ";
        //                strReportSelect += " Tmp_AvailCollat , ";
        //                strReportSelect += " Tmp_AvailDP , ";
        //                strReportSelect += " Tmp_EarlyPayIn , ";
        //                strReportSelect += " Tmp_UnClear , ";
        //                strReportSelect += " Tmp_Collected1 , ";
        //                strReportSelect += " Tmp_OtherCollected , ";
        //                strReportSelect += " Tmp_OtherCollected1 , ";


        //                int intCols = 5;
        //                int i;
        //                for (i = 0; i < dtCorpAct.Rows.Count; i++)
        //                {
        //                    string strExchSeg = dtCorpAct.Rows[i]["fm_exchange"] + "" + dtCorpAct.Rows[i]["fm_Segment"];
        //                    string strExchSegName = "[" + UtilityModel.mfnGetExchangeCode2Desc(dtCorpAct.Rows[i]["fm_exchange"].ToString().Trim()) + "-" + UtilityModel.mfnGetSegmentCode2Desc(dtCorpAct.Rows[i]["fm_Segment"].ToString().Trim()) + "]";
        //                    strCreate += " Tmp_InitMrgn" + strExchSeg + " Money,";
        //                    strInsert += " 0 , ";
        //                    strReportSelect += " Tmp_InitMrgn" + strExchSeg + ",";

        //                    strCreate += " Tmp_M2MMrgn" + strExchSeg + " Money,";
        //                    strInsert += " 0 , ";
        //                    strReportSelect += " Tmp_M2MMrgn" + strExchSeg + ",";

        //                    strCreate += " Tmp_InitColl" + strExchSeg + " Money,";
        //                    strInsert += " 0 , ";
        //                    strReportSelect += " Tmp_InitColl" + strExchSeg + ",";

        //                    strCreate += " Tmp_M2MColl" + strExchSeg + " Money,";
        //                    strInsert += " 0 , ";
        //                    strReportSelect += " Tmp_M2MColl" + strExchSeg + ",";

        //                    strCreate += " Tmp_InitShort" + strExchSeg + " Money,";
        //                    strInsert += " 0 , ";
        //                    strReportSelect += " Tmp_InitShort" + strExchSeg + ",";

        //                    strCreate += " Tmp_M2MShort" + strExchSeg + " Money,";
        //                    strInsert += " 0 , ";
        //                    strReportSelect += " Tmp_M2MShort" + strExchSeg + ",";

        //                    strCreate += " Tmp_Mrgn" + strExchSeg + " Money,";
        //                    strInsert += " 0 , ";
        //                    strReportSelect += " Tmp_Mrgn" + strExchSeg + ",";

        //                    strCreate += " Tmp_Coll" + strExchSeg + " Money,";
        //                    strInsert += " 0 , ";
        //                    strReportSelect += " Tmp_Coll" + strExchSeg + ",";

        //                    strCreate += " Tmp_Short" + strExchSeg + " Money " + ((i < dtCorpAct.Rows.Count - 1) ? " , " : "");
        //                    strInsert += " 0 " + ((i < dtCorpAct.Rows.Count - 1) ? " , " : "");
        //                    strReportSelect += " Tmp_Short" + strExchSeg + ((i < dtCorpAct.Rows.Count - 1) ? " , " : "");
        //                }
        //                strCreate += " ,Tmp_Statement  char(15)";
        //                strInsert += " , 'Not Available' ";
        //                strReportSelect += ",Tmp_Statement";
        //                strCreate += " ) ";

        //                mylib.ExecSQL(strCreate, curCon);

        //                strSql = " insert into #TmpMarginAnalysis ";
        //                strSql += strInsert + " from ( ";
        //                strSql += " SELECT distinct fm_clientcd FROM FMargins,Client_master Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + "and exists " + myutil.LoginAccess("fm_clientcd");
        //                if (IsTplusCommex == "Y")
        //                {
        //                    strSql += " union SELECT distinct fm_clientcd FROM " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "' " + strClientWhere + "";
        //                }
        //                strSql += " ) a ";

        //                mylib.ExecSQL(strSql, curCon);


        //                for (i = 0; i < dtCorpAct.Rows.Count; i++)
        //                {
        //                    string strExchSeg = dtCorpAct.Rows[i]["fm_exchange"].ToString().Trim() + "" + dtCorpAct.Rows[i]["fm_segment"].ToString().Trim();

        //                    if (dtCorpAct.Rows[i]["fm_segment"].ToString() == "X")
        //                    {
        //                        strSql = " Update #TmpMarginAnalysis set Tmp_InitMrgn" + strExchSeg + " = fm_initialmargin,Tmp_M2MMrgn" + strExchSeg + " = fm_MTMLoss,";
        //                        strSql += " Tmp_InitColl" + strExchSeg + " = fm_collected,Tmp_M2MColl" + strExchSeg + " = fm_collected1,";
        //                        strSql += " Tmp_InitShort" + strExchSeg + " = (fm_initialmargin)-fm_collected,Tmp_M2MShort" + strExchSeg + " = fm_MTMLoss-fm_collected1,";
        //                        strSql += " Tmp_Mrgn" + strExchSeg + " = fm_initialmargin+fm_MTMLoss,Tmp_Coll" + strExchSeg + " = fm_collected+fm_collected1,Tmp_Short" + strExchSeg + " = fm_initialmargin+fm_MTMLoss-fm_collected-fm_collected1,";
        //                        strSql += " Tmp_Mrgn = Tmp_Mrgn+(fm_initialmargin+fm_MTMLoss),Tmp_Coll=Tmp_Coll+(fm_collected+fm_collected1),Tmp_Short= Tmp_Short+(fm_initialmargin+fm_MTMLoss-fm_collected-fm_collected1)";
        //                        strSql += " from (  ";
        //                        strSql += " Select fm_Exchange,fm_clientcd,(case fm_exchange When 'M' Then fm_Regmargin + fm_exposurevalue + fm_buypremmargin else fm_initialmargin + fm_exposurevalue end) fm_initialmargin,  case fm_Exchange When 'M' Then fm_additionalmargin + fm_Tndmargin + fm_Dlvmargin - fm_SpreadBen + fm_ConcMargin + fm_DelvPMargin else fm_additionalmargin + fm_SplMargin end fm_MTMLoss, fm_collected, fm_collectedT2 fm_collected1 ";
        //                        strSql += " From " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master  Where cm_cd=fm_clientcd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
        //                        strSql += " and fm_Dt = '" + strdate + "'";
        //                        strSql += " union ";
        //                        strSql += " Select po_exchange,po_clientcd,0 fm_initialmargin,-sum(po_futvalue) fm_MTMLoss,0 fm_collected,0 fm_collected1 ";
        //                        strSql += " from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Fpositions," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master ";
        //                        strSql += " Where po_clientcd=cm_cd and po_companycode='" + HttpContext.Current.Session["CompanyCode"] + "'  and po_dt ='" + strdate + "'";
        //                        strSql += " Group By po_companycode,po_exchange,po_clientcd ";
        //                        strSql += " Having -sum(po_futvalue) > 0 ";
        //                        strSql += " ) a ";
        //                        strSql += " Where Tmp_Clientcd =fm_clientcd and fm_Exchange  = '" + Strings.Left(strExchSeg, 1) + "'";



        //                        mylib.ExecSQL(strSql, curCon);


        //                        strSql = " Update #TmpMarginAnalysis set Tmp_InitColl" + strExchSeg + " = Tmp_InitColl" + strExchSeg + " + fc_collected,Tmp_M2MColl" + strExchSeg + " = Tmp_M2MColl" + strExchSeg + " + fc_collected1,";
        //                        strSql += " Tmp_InitShort" + strExchSeg + " = Tmp_InitShort" + strExchSeg + "-fc_collected,Tmp_M2MShort" + strExchSeg + " = Tmp_M2MShort" + strExchSeg + "-fc_collected1,";
        //                        strSql += " Tmp_Coll" + strExchSeg + " = Tmp_Coll" + strExchSeg + "+fc_collected+fc_collected1,Tmp_Short" + strExchSeg + " = Tmp_Short" + strExchSeg + "-fc_collected-fc_collected1,";
        //                        strSql += " Tmp_Coll=Tmp_Coll+(fc_collected+fc_collected1),Tmp_Short= Tmp_Short-fc_collected-fc_collected1";
        //                        strSql += " From Fmargin_Clients  ";
        //                        strSql += " Where Tmp_Clientcd =fc_Filler1 and fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fc_Exchange = '" + Strings.Left(strExchSeg, 1) + "' and fc_segment = '" + Strings.Right(strExchSeg, 1) + "'";
        //                        strSql += " and fc_Dt = '" + strdate + "' and fc_Filler1 <> '' ";

        //                        mylib.ExecSQL(strSql, curCon);
        //                    }
        //                    else
        //                    {
        //                        strSql = " Update #TmpMarginAnalysis set Tmp_InitMrgn" + strExchSeg + " = fm_TotalMrgn-(fm_MTMLoss+case When fm_Segment = 'C' Then fm_additionalmargin else 0 end),Tmp_M2MMrgn" + strExchSeg + " = (fm_MTMLoss+case When fm_Segment = 'C' Then fm_additionalmargin else 0 end),";
        //                        strSql += " Tmp_InitColl" + strExchSeg + " = fm_collected,Tmp_M2MColl" + strExchSeg + " = fm_collected1,";
        //                        strSql += " Tmp_InitShort" + strExchSeg + " = (fm_TotalMrgn-(fm_MTMLoss+case When fm_Segment = 'C' Then fm_additionalmargin else 0 end))-fm_collected,Tmp_M2MShort" + strExchSeg + " = (fm_MTMLoss+case When fm_Segment = 'C' Then fm_additionalmargin else 0 end)-fm_collected1,";
        //                        strSql += " Tmp_Mrgn" + strExchSeg + " = fm_TotalMrgn,Tmp_Coll" + strExchSeg + " = fm_collected+fm_collected1,Tmp_Short" + strExchSeg + " = fm_TotalMrgn-fm_collected-fm_collected1,";
        //                        strSql += " Tmp_Mrgn = Tmp_Mrgn+(fm_TotalMrgn),Tmp_Coll=Tmp_Coll+(fm_collected+fm_collected1),Tmp_Short= Tmp_Short+(fm_TotalMrgn-fm_collected-fm_collected1)";
        //                        strSql += " From FMargins  ";
        //                        strSql += " Where Tmp_Clientcd =fm_clientcd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_Exchange = '" + Strings.Left(strExchSeg, 1) + "' and fm_segment = '" + Strings.Right(strExchSeg, 1) + "'";
        //                        strSql += " and fm_Dt = '" + strdate + "'";
        //                        mylib.ExecSQL(strSql, curCon);


        //                        strSql = " Update #TmpMarginAnalysis set Tmp_InitColl" + strExchSeg + " = Tmp_InitColl" + strExchSeg + " + fc_collected,Tmp_M2MColl" + strExchSeg + " = Tmp_M2MColl" + strExchSeg + " + fc_collected1,";
        //                        strSql += " Tmp_InitShort" + strExchSeg + " = Tmp_InitShort" + strExchSeg + "-fc_collected,Tmp_M2MShort" + strExchSeg + " = Tmp_M2MShort" + strExchSeg + "-fc_collected1,";
        //                        strSql += " Tmp_Coll" + strExchSeg + " = Tmp_Coll" + strExchSeg + "+fc_collected+fc_collected1,Tmp_Short" + strExchSeg + " = Tmp_Short" + strExchSeg + "-fc_collected-fc_collected1,";
        //                        strSql += " Tmp_Coll=Tmp_Coll+(fc_collected+fc_collected1),Tmp_Short= Tmp_Short-fc_collected-fc_collected1";
        //                        strSql += " From Fmargin_Clients  ";
        //                        strSql += " Where Tmp_Clientcd =fc_Filler1 and fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fc_Exchange = '" + Strings.Left(strExchSeg, 1) + "' and fc_segment = '" + Strings.Right(strExchSeg, 1) + "'";
        //                        strSql += " and fc_Dt = '" + strdate + "' and fc_Filler1 <> '' ";
        //                        mylib.ExecSQL(strSql, curCon);

        //                    }
        //                }

        //                strSql = " Update #TmpMarginAnalysis set Tmp_Ledger = isnull(fc_TLedger,0),Tmp_BenfHolding = isnull(fc_AvailBenf,0),Tmp_AvailCollat = isnull(fc_AvailCollat,0),Tmp_AvailDP = isnull(fc_AvailDP,0),Tmp_EarlyPayIn = isnull(fc_FillerN2,0),Tmp_UnClear = isnull(fc_UnClear,0), Tmp_Collected1 = isnull(fc_Collected1,0) ";
        //                strSql += " From Fmargin_Clients  Where Tmp_Clientcd = fc_clientcd and fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fc_Exchange = '' and fc_dt = '" + strdate + "'";
        //                mylib.ExecSQL(strSql, curCon);


        //                strSql = " Update #TmpMarginAnalysis set Tmp_OtherCollected = isnull(fc_Collected,0) , Tmp_OtherCollected1 = isnull(fc_Collected1,0) from (  ";
        //                strSql += " select fc_Filler1 , Sum(fc_Collected) fc_Collected , Sum(fc_Collected1) fc_Collected1 ";
        //                strSql += " From Fmargin_Clients  Where fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'";
        //                strSql += " and fc_Exchange <> '' and fc_dt = '" + strdate + "'";
        //                strSql += " Group by fc_Filler1 ) a Where Tmp_Clientcd = fc_Filler1 ";
        //                mylib.ExecSQL(strSql, curCon);

        //                if (chkOnlyShortFall == "1")
        //                {
        //                    strSql += " delete from #TmpMarginAnalysis where Tmp_Short <= 0";
        //                    mylib.ExecSQL(strSql, curCon);
        //                }



        //                string StrValidConn = mydbutil.CheckConnection("ESIGN-TRADEPLUS");
        //                string StrActiveConn = mylib.fnFireQuery("other_products", "Count(0)", "op_status='A' AND op_product", "ESIGN-TRADEPLUS", true);

        //                if (StrValidConn.Trim() == "1")
        //                {
        //                    SqlConnection EsignConnection = mydbutil.commexTemp_conn("ESIGN-TRADEPLUS");
        //                    strSql = " Update #TmpMarginAnalysis set Tmp_Statement='Download' from  " + "[" + EsignConnection.DataSource + "]" + "." + EsignConnection.Database + ".dbo.digital_details ";
        //                    strSql += " Where dd_dt ='" + strdate + "'";
        //                    strSql += " and dd_filetype='CMRG' and dd_clientcd = Tmp_Clientcd";
        //                    mylib.ExecSQL(strSql, curCon);
        //                }


        //                string strSelect = "";
        //                switch (grouping.Trim())
        //                {
        //                    case "CL":
        //                        {
        //                            strSelect = "select '' as [BranchCode],";
        //                            break;
        //                        }

        //                    case "BR":
        //                        {
        //                            strSelect = "select rtrim(bm_branchname) + '['+ ltrim(rtrim(cm_brboffcode))+']' as [BranchCode],";
        //                            break;
        //                        }

        //                    case "GR":
        //                        {
        //                            strSelect = "select rtrim(gr_desc) + '['+ ltrim(rtrim(cm_groupcd)) + ']' as [BranchCode],";
        //                            break;
        //                        }

        //                    case "FM":
        //                        {
        //                            strSelect = "select rtrim(fm_desc) + '[' + ltrim(rtrim(cm_familycd)) + ']' as [BranchCode],";
        //                            break;
        //                        }
        //                }





        //                {
        //                    strReportSelect = strSelect + strReportSelect;
        //                    strReportSelect += " from #TmpMarginAnalysis,Client_master,branch_master, group_master, family_master Where Tmp_Clientcd = cm_cd and cm_brboffcode = bm_branchcd and cm_groupcd=gr_cd and cm_familycd=fm_cd  ";
        //                    strReportSelect += " Order by BranchCode,cm_Name,Tmp_Clientcd ";
        //                }


        //                // strReportSelect += " from #TmpMarginAnalysis , Client_master Where Tmp_Clientcd = cm_cd  Order by cm_Name,Tmp_Clientcd ";

        //                dt = mylib.OpenDataTable(strReportSelect, curCon);

        //                HttpContext.Current.Session["Data"] = dt;
        //                HttpContext.Current.Session["Data1"] = dt;



        //            }
        //        }

        //    }
        //    return dt;
        //}

        //public DataTable GetCMADetail(string strcd, string strdate)
        //{
        //    DataTable DsMrgnDtl = null;

        //    string IsTplusCommex = "N";
        //    string strExchSegName;
        //    string strSql = "";
        //    string StrData = "";
        //    DataTable dtTemp;
        //    string strClientWhere = "";
        //    strClientWhere = " and fm_clientcd = '" + strcd + "'";
        //    LibraryModel mylib = new LibraryModel();
        //    UtilityModel myutil = new UtilityModel();
        //    UtilityDBModel mydbutil = new UtilityDBModel();
        //    SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
        //    strdate = myutil.dtos(strdate);
        //    using (SqlConnection curCon = new SqlConnection(connectionstring))
        //    {
        //        curCon.Open();

        //        strSql = " SELECT fm_companycode,fm_exchange,fm_Segment,1 SortORder,COUNT(0) CNT FROM FMargins,Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange,fm_Segment";
        //        if (SQLConnComex != null)
        //            strSql += " union all " + " SELECT fm_companycode,fm_exchange,'X',2 SortORder,COUNT(0) FROM " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange ";
        //        strSql += " ORder by fm_Segment,fm_exchange";
        //        dtTemp = mylib.OpenDataTable(strSql, curCon);


        //    }

        //    return dtTemp;


        //}

        public DataTable GetCMAReport(string Select, string Code, string strdate, string grouping, string chkOnlyShortFall)
        {

            string strClientWhere = "";
            if (Code.Trim() != "")
            {
                switch (Select)
                {
                    case "CL":
                        {
                            strClientWhere = " and cm_cd = '" + Code + "'";
                            break;
                        }

                    case "GR":
                        {
                            strClientWhere = " and cm_groupcd = '" + Code + "'";
                            break;
                        }

                    case "FM":
                        {
                            strClientWhere = " and cm_familycd = '" + Code + "'";
                            break;
                        }

                    case "SB":
                        {
                            strClientWhere = " and cm_subbroker = '" + Code + "'";
                            break;
                        }

                    case "BR":
                        {
                            strClientWhere = " and cm_brboffcode = '" + Code + "'";
                            break;
                        }
                    case "ALL":
                        {
                            strClientWhere = "";
                            break; ;
                        }
                }
            }
            DataTable dt = null;

            DataTable dtCorpAct = new DataTable();
            string strSql = "";
            string IsTplusCommex = (string)HttpContext.Current.Session["IsTplusCommex"];
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
            strdate = myutil.dtos(strdate);
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                mylib.ExecSQL("if OBJECT_ID('tempdb..#TmpPeakColl') is not null Drop Table #TmpPeakColl", curCon);
                mylib.ExecSQL("Create Table #TmpPeakColl (Tmp_Clientcd VarChaR(8),Tmp_CompanyCode VarChaR(3),Tmp_PeakMargin Money,Tmp_PeakColl Money,Tmp_Shortfall Money, Tmp_exchange char(1), Tmp_segment char(1) , Tmp_Nfiller4 money, Tmp_Nfiller5 money, Tmp_statement char(15) )", curCon);


                if (Conversion.Val(mylib.fnFireQuery("Fmargin_PeakMargin", "Count(0)", "fc_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and fc_dt ", strdate, true).ToString()) > 0)
                {
                    strSql = " insert into #TmpPeakColl ";
                    strSql += " select fm_clientcd,'" + HttpContext.Current.Session["CompanyCode"] + "'+fm_exchange+fm_Segment ,isNull(fm_NFiller4,0), isNull(fm_NFiller5,0) , 0,  fm_exchange, fm_Segment ,isNull(fm_NFiller4,0), isNull(fm_NFiller5,0), 'Not Available' ";
                    strSql += " from Fmargins, Client_master ";
                    strSql += " Where fm_clientcd=cm_cd and fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' " + strClientWhere + HttpContext.Current.Session["LoginAccessOld"];
                    if (IsTplusCommex == "Y" && SQLConnComex != null)
                    {
                        strSql += " union ";
                        strSql += " select fm_clientcd,'" + HttpContext.Current.Session["CompanyCode"] + "'+fm_exchange+'X',isNull(fm_PeakMargin,0)+isNull(fm_Filler2,0),isNull(fm_Filler1,0), 0 ,  fm_exchange, 'X' fm_Segment ,isNull(fm_PeakMargin,0)+isNull(fm_Filler2,0),isNull(fm_Filler1,0),'Not Available'";
                        strSql += " from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Fmargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master ";
                        // strSql += " from " + SQLConnComex.Database + ".dbo.Fmargins, " + SQLConnComex.Database + ".dbo.Client_master ";
                        strSql += " Where fm_clientcd=cm_cd and fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' " + strClientWhere + HttpContext.Current.Session["LoginAccessOld"];
                    }
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " Update #TmpPeakColl set Tmp_PeakMargin = Round(Tmp_PeakMargin * " + mylib.fnPeakFactor(strdate) + "/100,2) Where Right(Tmp_CompanyCode,3) <> 'MX' ";
                    mylib.ExecSQL(strSql, curCon);
                }
                else
                {
                    strSql = " insert into #TmpPeakColl ";
                    strSql += " select fm_clientcd,fm_exchange+fm_Segment,isNull(fm_NFiller4,0), 0 , 0 , fm_exchange, fm_Segment ,isNull(fm_NFiller4,0), isNull(fm_NFiller5,0),'Not Available' ";
                    strSql += " from Fmargins, Client_master  ";
                    strSql += " Where fm_clientcd=cm_cd and fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' " + strClientWhere + HttpContext.Current.Session["LoginAccessOld"];
                    if (IsTplusCommex == "Y" && SQLConnComex != null)
                    {
                        strSql += " union ";
                        strSql += " select fm_clientcd,fm_exchange+'X',isNull(fm_PeakMargin,0)+isNull(fm_Filler2,0), 0 , 0 , fm_exchange, 'X' fm_Segment, isNull(fm_PeakMargin,0)+isNull(fm_Filler2,0),isNull(fm_Filler1,0),'Not Available' ";
                        strSql += " from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Fmargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master ";
                        // strSql += " from " + SQLConnComex.Database + ".dbo.Fmargins," + SQLConnComex.Database + ".dbo.Client_master ";
                        strSql += " Where fm_clientcd=cm_cd and fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' " + strClientWhere + HttpContext.Current.Session["LoginAccessOld"];
                    }
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " Update #TmpPeakColl set Tmp_PeakMargin = Round(Tmp_PeakMargin * " + mylib.fnPeakFactor(strdate) + "/100,2) Where Right(Tmp_CompanyCode,3) <> 'MX'";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " Update #TmpPeakColl set Tmp_PeakColl = case When isNull(fc_FillerN9,0) > 0 then isNull(fc_FillerN9,0) else 0 end ";
                    strSql += " from Fmargin_clients ";
                    strSql += " Where fc_clientcd=Tmp_clientcd and fc_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fc_Exchange = '' and fc_dt = '" + strdate + "' ";
                    mylib.ExecSQL(strSql, curCon);
                }

                strSql = " Update #TmpPeakColl set Tmp_PeakMargin=0,Tmp_PeakColl=0 Where Tmp_PeakColl>=Tmp_PeakMargin";
                mylib.ExecSQL(strSql, curCon);


                strSql = " Insert into #TmpPeakColl ";
                strSql += " select Tmp_Clientcd,'',0 , 0 , case When Sum(Tmp_PeakMargin-Tmp_PeakColl) > 0 then Sum(Tmp_PeakMargin-Tmp_PeakColl) else 0 end, '', ''  , 0 , 0 , 'Not Available' from #TmpPeakColl ";
                strSql += " Group by Tmp_Clientcd ";

                mylib.ExecSQL(strSql, curCon);

                strSql = " Delete #TmpPeakColl Where Tmp_CompanyCode <> '' ";
                mylib.ExecSQL(strSql, curCon);

                strSql = " SELECT fm_companycode,fm_exchange,fm_Segment,1 SortORder,COUNT(0) CNT FROM FMargins,Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange,fm_Segment";
                if (IsTplusCommex == "Y" && SQLConnComex != null)
                {
                    //strSql += " union all " + " SELECT fm_companycode,fm_exchange,'X',2 SortORder,COUNT(0) FROM " + SQLConnComex.Database + ".dbo.FMargins," + SQLConnComex.Database + ".dbo.Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange ";
                    strSql += " union all " + " SELECT fm_companycode,fm_exchange,'X',2 SortORder,COUNT(0) FROM " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange ";
                }

                strSql += " order by fm_Segment,fm_exchange";

                dtCorpAct = mylib.OpenDataTable(strSql, curCon);

                if (IsTplusCommex == "Y" && SQLConnComex != null)
                {
                    try
                    {
                        mylib.ExecSQL("Drop table #FmarginsRpt", curCon);
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        strSql = " CREATE TABLE [#FmarginsRpt]( ";
                        strSql += " [fm_companycode] [char](1) NOT NULL,[fm_exchange] [char](1) NOT NULL,[fm_dt] [char](8) NOT NULL, ";
                        strSql += " [fm_clientcd] [char](8) NOT NULL,[fm_spanmargin] [money] NOT NULL,[fm_buypremmargin] [money] NOT NULL, ";
                        strSql += " [fm_initialmargin] [money] NOT NULL,[fm_exposurevalue] [money] NOT NULL,[fm_clienttype] [char](1) NOT NULL, ";
                        strSql += " [fm_additionalmargin] [money] NOT NULL,[fm_collected] [money] NOT NULL,[fm_mainbrcd] [char](8) NOT NULL, ";
                        strSql += " [mkrid] [char](8) NOT NULL,[mkrdt] [char](8) NOT NULL,[fm_Regmargin] [money] NULL,[fm_Tndmargin] [money] NULL, ";
                        strSql += " [fm_Dlvmargin] [money] NULL,[fm_SpreadBen] [money] NULL,[fm_SplMargin] [money] NULL,[fm_collectedT2] [money] NOT NULL, ";
                        strSql += " [fm_InitShort] [money] NOT NULL,[fm_MTMAddShort] [money] NOT NULL,[fm_OthShort] [money] NOT NULL,[fm_ConcMargin] [money] NOT NULL, ";
                        strSql += " [fm_DelvPMargin] [money] NOT NULL,[fm_MTMLoss] [money] NOT NULL) ";
                        mylib.ExecSQL(strSql, curCon);
                    }

                    strSql = " Insert into #FmarginsRpt select fm_companycode,fm_exchange,fm_dt,fm_clientcd,Sum(fm_spanmargin),Sum(fm_buypremmargin),Sum(fm_initialmargin),Sum(fm_exposurevalue),''fm_clienttype,";
                    strSql += " Sum(fm_additionalmargin),Sum(fm_collected),'' fm_mainbrcd,'' mkrid,'' mkrdt,Sum(fm_Regmargin),Sum(fm_Tndmargin),Sum(fm_Dlvmargin),Sum(fm_SpreadBen),Sum(fm_SplMargin),Sum(fm_collectedT2),Sum(fm_InitShort),";
                    strSql += " Sum(fm_MTMAddShort),Sum(fm_OthShort),Sum(fm_ConcMargin),Sum(fm_DelvPMargin),Sum(fm_MTMLoss) from ( ";
                    strSql += " select fm_companycode,fm_exchange,fm_dt,fm_clientcd,fm_spanmargin,fm_buypremmargin,fm_initialmargin,fm_exposurevalue,''fm_clienttype,fm_additionalmargin,fm_collected,";
                    strSql += " ''fm_mainbrcd,''mkrid,''mkrdt,fm_Regmargin,fm_Tndmargin,fm_Dlvmargin,fm_SpreadBen,fm_SplMargin,fm_collectedT2,fm_InitShort,fm_MTMAddShort,fm_OthShort,fm_ConcMargin,fm_DelvPMargin,0 fm_MTMLoss ";
                    strSql += " from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Fmargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master ";
                    strSql += " Where fm_clientcd = cm_cd and fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' " + strClientWhere + HttpContext.Current.Session["LoginAccessOld"];
                    strSql += " union all ";
                    strSql += " select po_companycode,po_exchange,po_dt,po_clientcd,0 fm_spanmargin,0 fm_buypremmargin,0 fm_initialmargin,0 fm_exposurevalue,0 fm_clienttype,0 fm_additionalmargin,";
                    strSql += " 0 fm_collected,'' fm_mainbrcd,'' mkrid,'' mkrdt,0 fm_Regmargin,0 fm_Tndmargin,0 fm_Dlvmargin,0 fm_SpreadBen,0 fm_SplMargin,0 fm_collectedT2,0 fm_InitShort,";
                    strSql += " 0 fm_MTMAddShort,0 fm_OthShort,0 fm_ConcMargin,0 fm_DelvPMargin,case When -sum(po_futvalue) > 0 Then -sum(po_futvalue) else 0 end MarginReq ";
                    strSql += " from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Fpositions, " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master";
                    strSql += " Where po_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and po_clientcd = cm_cd ";
                    strSql += " and po_dt ='" + strdate + "' " + strClientWhere + HttpContext.Current.Session["LoginAccessOld"];
                    strSql += " Group by po_clientcd,po_companycode,po_exchange,po_dt";
                    strSql += " Having case When -sum(po_futvalue) > 0 Then -sum(po_futvalue) else 0 end > 0 ";
                    strSql += " ) a Group by fm_companycode,fm_exchange,fm_dt,fm_clientcd ";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " Update #FmarginsRpt set fm_collected = fc_collected , fm_collectedT2  = fc_Collected1 ";
                    strSql += " From Fmargin_Clients  ";
                    strSql += " Where fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fc_exchange = fm_Exchange and fc_Segment = 'X' and fc_dt = '" + strdate + "' and fm_clientcd = fc_clientcd ";
                    strSql += " and not exists ( Select fm_clientcd from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Fmargins Where fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' and fc_clientcd  = fm_clientcd and fm_Exchange = fc_Exchange ) ";
                    mylib.ExecSQL(strSql, curCon);
                }

                strSql = "select  distinct  '0' 'Product' ,(fm_exchange+fm_segment) fm_ExchSeg,fm_segment Seg from Fmargins, Client_master ";
                strSql += " Where fm_clientcd=cm_cd and fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' ";
                if (IsTplusCommex == "Y" && SQLConnComex != null)
                {
                    strSql += " union ";
                    strSql += " select distinct '1' 'Product' ,(fm_exchange+'X') fm_ExchSeg,'X' Seg from #FmarginsRpt ";
                    strSql += " Where fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' ";
                }
                strSql += " Order by Product,Seg,fm_ExchSeg ";
                dtCorpAct = mylib.OpenDataTable(strSql, curCon);

                string strTemp = "";
                string strFild = "";
                string strSegCode = "";
                string strCase = "";
                string strTotalShortFallX = "";
                string strTotalCollectedX = "";
                //string[] arrExch = new string[];
                List<String> lstExch = new List<String>();
                string strTotMargin = "";

                for (int lngExchSeg = 0; lngExchSeg <= dtCorpAct.Rows.Count - 1; lngExchSeg++)
                {
                    DataRow drExchSeg = dtCorpAct.Rows[lngExchSeg];
                    string strProduct = drExchSeg["Product"].ToString().Trim();
                    string strExchSegme = drExchSeg["fm_ExchSeg"].ToString().Trim();

                    if (Strings.Right(strExchSegme, 1) == "C")
                    {
                        strSegCode = "Cash";
                    }
                    else if (Strings.Right(strExchSegme, 1) == "F")
                    {
                        strSegCode = "Fo";
                    }
                    else if (Strings.Right(strExchSegme, 1) == "K")
                    {
                        strSegCode = "Fx";
                    }
                    else if (Strings.Right(strExchSegme, 1) == "X")
                    {
                        strSegCode = "Cx";
                    }
                    strCase += "case fm_exchange+fm_Segment when '" + strExchSegme + "' then case When (fm_TotalMrgn-(fm_collected+fm_collected1)) > 0 Then (fm_TotalMrgn-(fm_collected+fm_collected1)) else 0 end else 0 end TotalShort" + strSegCode + strExchSegme + ",";
                    if (strProduct == "0")
                    {
                        strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then ((fm_TotalMrgn-(fm_MTMLoss+ case When fm_Segment = 'C' Then fm_additionalmargin else 0 end))-fm_collected) else 0 end InitShort" + strExchSegme + ",";
                        strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then fm_MTMLoss+ case When fm_Segment = 'C' Then fm_additionalmargin else 0 end - fm_collected1 else 0 end M2MShort" + strExchSegme + ",";
                    }
                    else if (strProduct == "1")
                    {
                        strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then (isNull(FM_INITIALMARGIN,0)-isNull(fm_collected,0)) else 0 end InitShort" + strExchSegme + ",";
                        strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then (isNull(FM_MTMLOSS,0)-isNull(fm_collected1,0)) else 0 end M2MShort" + strExchSegme + ",";
                    }
                    strTotalShortFallX += "sum(TotalShort" + strSegCode + strExchSegme + ")+";
                    strTemp += "isNull(sum(InitShort" + strExchSegme + "),0) InitShort" + strExchSegme + ",";
                    strTemp += " isNull(sum(M2MShort" + strExchSegme + "),0) M2MShort" + strExchSegme + ",";

                    strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then (fm_collected+fm_collected1) else 0 end Collected" + strSegCode + strExchSegme + ",";
                    strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then fm_collected else 0 end InitCollected" + strExchSegme + ",";
                    strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then fm_collected1 else 0 end M2MCollected" + strExchSegme + ",";
                    strTotalCollectedX += "sum(Collected" + strSegCode + strExchSegme + ")+";
                    strTemp += " sum(InitCollected" + strExchSegme + ") InitCollected" + strExchSegme + ",";
                    strTemp += " sum(M2MCollected" + strExchSegme + ") M2MCollected" + strExchSegme + ",";

                    strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then fm_TotalMrgn else 0 end TotalMrgn" + strSegCode + strExchSegme + ",";
                    if (strProduct == "0")
                    {
                        strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then fm_TotalMrgn-(fm_MTMLoss+ case When fm_Segment = 'C' Then fm_additionalmargin else 0 end) else 0 end InitMrgn" + strExchSegme + ",";
                        strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then fm_MTMLoss+ case When fm_Segment = 'C' Then fm_additionalmargin else 0 end else 0 end M2MMrgn" + strExchSegme + ",";
                    }
                    else if (strProduct == "1")
                    {
                        strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then isNull(FM_INITIALMARGIN,0) else 0 end InitMrgn" + strExchSegme + ",";
                        strCase += " case fm_exchange+fm_Segment when '" + strExchSegme + "' then isNull(FM_MTMLOSS,0) else 0 end M2MMrgn" + strExchSegme + ",";
                    }
                    strTemp += "sum(TotalMrgn" + strSegCode + strExchSegme + ") TotalMrgn" + strSegCode + strExchSegme + ",";
                    strTemp += "sum(InitMrgn" + strExchSegme + ") InitMrgn" + strExchSegme + ",";
                    strTemp += "sum(M2MMrgn" + strExchSegme + ") M2MMrgn" + strExchSegme + ",";

                    strTotMargin += "sum(TotalMrgn" + strSegCode + strExchSegme + ") +";
                    //lstExch.Add("sum(TotalMrgn" + strSegCode + strExchSegme + ")");
                }

                //strTotMargin = string.Join("+", lstExch);
                //strTotMargin += " TotalMrgnAllExch";
                if (strCase.Trim() == "")
                {
                    return dt;
                }
                //strTotMargin = Strings.Left(strTotMargin.Trim(), strTotMargin.Trim().Length - 1);
                strCase = Strings.Left(strCase.Trim(), strCase.Trim().Length - 1);
                strFild = "";
                strFild = " fm_clientcd,isNull(cm_Name,'Not Found') cm_Name,cm_email,bm_email,cm_brboffcode,bm_branchname,bm_add1 ,cm_groupcd,gr_desc,cm_familycd,fm_desc,cm_add1,bm_add2 ,cm_add2,bm_add3 ,cm_add3,Tmp_statement,";
                strFild += " Tmp_Shortfall PeakShort,";

                if (Strings.Right(strTotalShortFallX.Trim(), 1) == "+")
                {
                    strTotalShortFallX = Strings.Left(strTotalShortFallX, strTotalShortFallX.Length - 1) + " TotalShort";
                    strFild += strTotalShortFallX + ", ";
                }
                if (Strings.Right(strTotalCollectedX.Trim(), 1) == "+")
                {
                    strTotalCollectedX = Strings.Left(strTotalCollectedX, strTotalCollectedX.Length - 1) + " Collected";
                    strFild += strTotalCollectedX + ", ";
                }
                if (Strings.Right(strTotMargin.Trim(), 1) == "+")
                {
                    strTotMargin = Strings.Left(strTotMargin, strTotMargin.Length - 1) + " TotalMarginAll";
                    strFild += strTotMargin + ", ";
                }
                strFild += strTemp;
                strFild = Strings.Left(strFild.Trim(), strFild.Trim().Length - 1);

                string StrActiveConn = mylib.fnFireQuery("other_products", "Count(0)", "op_status='A' AND op_product", "ESIGN-TRADEPLUS", true);
                if (StrActiveConn.Trim() == "1")
                {
                    string StrValidConn = mydbutil.CheckConnection("ESIGN-TRADEPLUS");
                    if (StrValidConn.Trim() == "1")
                    {
                        SqlConnection EsignConnection = mydbutil.commexTemp_conn("ESIGN-TRADEPLUS");
                        strSql = " Update #TmpPeakColl set Tmp_statement='Download' from  " + "[" + EsignConnection.DataSource + "]" + "." + EsignConnection.Database + ".dbo.digital_details ";
                        strSql += " Where dd_dt ='" + strdate + "'";
                        strSql += " and dd_filetype='CMRG' and dd_clientcd = Tmp_Clientcd";
                        mylib.ExecSQL(strSql, curCon);
                    }
                }

                string strSelect = "";
                switch (grouping.Trim())
                {
                    case "CL":
                        {
                            strSelect = "select '' as [BranchCode],";
                            break;
                        }

                    case "BR":
                        {
                            strSelect = "select rtrim(bm_branchname) + '['+ ltrim(rtrim(cm_brboffcode))+']' as [BranchCode],";
                            break;
                        }

                    case "GR":
                        {
                            strSelect = "select rtrim(gr_desc) + '['+ ltrim(rtrim(cm_groupcd)) + ']' as [BranchCode],";
                            break;
                        }

                    case "FM":
                        {
                            strSelect = "select rtrim(fm_desc) + '[' + ltrim(rtrim(cm_familycd)) + ']' as [BranchCode],";
                            break;
                        }
                }

                strSql = strSelect + " *  ";
                strSql += "  from ( ";
                strSql += "  select " + strFild + " From (";
                strSql += " Select fm_clientcd," + strCase;
                strSql += " from ( select fm_clientcd,fm_exchange,fm_Segment,Sum(fm_TotalMrgn) fm_TotalMrgn,Sum(fm_collected) fm_collected,Sum(fm_collected1) fm_collected1,Sum(fm_initialmargin) fm_initialmargin,sum(fm_MTMLoss) fm_MTMLoss,sum(fm_additionalmargin) fm_additionalmargin from ( ";
                strSql += " select fm_clientcd,fm_exchange,fm_Segment,fm_TotalMrgn,fm_collected,fm_collected1,fm_initialmargin,fm_MTMLoss,fm_additionalmargin  from Fmargins, Client_master Where cm_cd=fm_clientcd and fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' " + HttpContext.Current.Session["LoginAccessOld"];
                if (mylib.GetSysParmSt("FMRGCombined", "") == "F")
                {
                    strSql += " union all ";
                    strSql += " select fc_Filler1,fc_exchange,fc_Segment,0 fm_TotalMrgn,fc_collected,fc_collected1,0 fm_initialmargin,0 fm_MTMLoss,0 fm_additionalmargin from Fmargin_Clients, Client_master Where cm_cd=fc_clientcd and fc_exchange <> '' and fc_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fc_dt = '" + strdate + "' " + HttpContext.Current.Session["LoginAccessOld"];
                }
                strSql += " ) a Group By fm_clientcd,fm_exchange,fm_Segment ";
                strSql += " ) z , Client_master,branch_master,Group_master,Family_master ";
                strSql += " Where fm_clientcd = cm_cd and cm_brboffcode = bm_branchcd and cm_groupcd=gr_cd and cm_familycd=fm_cd " + strClientWhere;
                if (IsTplusCommex == "Y" && SQLConnComex != null)
                {
                    strSql += " union all ";
                    string strTotalMargin = "";
                    string strInitMargin = "";
                    string strM2MMargin = "";
                    strTotalMargin = " (case fm_exchange When 'M' Then fm_Regmargin + fm_exposurevalue + fm_buypremmargin else fm_initialmargin + fm_exposurevalue end) + case fm_Exchange When 'M' Then fm_additionalmargin + fm_Tndmargin + fm_Dlvmargin - fm_SpreadBen + fm_ConcMargin + fm_DelvPMargin else fm_additionalmargin + fm_SplMargin end + fm_MTMLoss ";
                    strInitMargin = " case fm_exchange When 'M' Then fm_Regmargin + fm_exposurevalue + fm_buypremmargin else fm_initialmargin + fm_exposurevalue end ";
                    strM2MMargin = " case fm_Exchange When 'M' Then fm_additionalmargin + fm_Tndmargin + fm_Dlvmargin - fm_SpreadBen + fm_ConcMargin + fm_DelvPMargin else fm_additionalmargin + fm_SplMargin end+fm_MTMLoss ";
                    strCase = Strings.Replace(strCase, "fm_Segment", "'X'");
                    strCase = Strings.Replace(strCase, "fm_TotalMrgn", strTotalMargin);
                    strCase = Strings.Replace(strCase, "(fm_collected+fm_collected1)", "(fm_collected+fm_collectedt2)");
                    strCase = Strings.Replace(strCase, "fm_collected1", "fm_collectedt2");
                    strCase = Strings.Replace(strCase, "FM_INITIALMARGIN", strInitMargin);
                    strCase = Strings.Replace(strCase, "FM_MTMLOSS", strM2MMargin);
                    strSql += " Select fm_clientcd," + strCase;
                    strSql += " from #FmarginsRpt," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.branch_master," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.group_master," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.family_master  Where fm_clientcd = cm_cd and cm_brboffcode = bm_branchcd and cm_groupcd=gr_cd and cm_familycd=fm_cd and fm_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' " + HttpContext.Current.Session["LoginAccessOld"];
                }
                strSql += " ) a , Client_master,branch_master ,Group_master,Family_master, #TmpPeakColl ";
                strSql += " Where fm_clientcd = cm_cd and cm_brboffcode = bm_branchcd and cm_groupcd=gr_cd and cm_familycd=fm_cd and fm_clientcd = Tmp_Clientcd ";
                strSql += " Group by fm_clientcd,cm_Name,cm_email,bm_email,cm_brboffcode,bm_branchname,cm_groupcd,gr_desc,cm_familycd,fm_desc,bm_add1 ,cm_add1,bm_add2 ,cm_add2,bm_add3 ,cm_add3,Tmp_Shortfall,Tmp_statement ";
                strSql += " ) b ";
                if (chkOnlyShortFall == "1")
                {
                    strSql += " Where PeakShort > 0 Or TotalShort > 0 ";
                }
                strSql += " Order By fm_clientcd";
                dt = mylib.OpenDataTable(strSql, curCon);

                //if (dtCorpAct.Rows.Count > 0)
                //{
                //    try
                //    {
                //        mylib.ExecSQL("Drop Table #TmpMarginAnalysis", curCon);
                //    }
                //    catch (Exception)
                //    {

                //    }
                //    finally
                //    {
                //        string strCreate = "";
                //        string strInsert = "";
                //        string strReportSelect = "";
                //        strCreate = " Create Table #TmpMarginAnalysis ( ";
                //        strCreate += " Tmp_Clientcd Varchar(8),";
                //        strCreate += " Tmp_Mrgn Money,";
                //        strCreate += " Tmp_Coll Money,";
                //        strCreate += " Tmp_Short Money , ";
                //        strCreate += " Tmp_Ledger Money , ";
                //        strCreate += " Tmp_BenfHolding Money , ";
                //        strCreate += " Tmp_AvailCollat Money , ";
                //        strCreate += " Tmp_AvailDP Money , ";
                //        strCreate += " Tmp_EarlyPayIn Money , ";
                //        strCreate += " Tmp_UnClear Money , ";
                //        strCreate += " Tmp_Collected1 Money , ";
                //        strCreate += " Tmp_OtherCollected Money , ";
                //        strCreate += " Tmp_OtherCollected1 Money , ";

                //        strInsert = " select fm_clientcd, ";
                //        strInsert += " 0 , ";
                //        strInsert += " 0 , ";
                //        strInsert += " 0, ";
                //        strInsert += " 0 , ";
                //        strInsert += " 0 , ";
                //        strInsert += " 0, ";
                //        strInsert += " 0 , ";
                //        strInsert += " 0 , ";
                //        strInsert += " 0, ";
                //        strInsert += " 0 , ";
                //        strInsert += " 0 , ";
                //        strInsert += " 0, ";

                //        strReportSelect = "  M.Tmp_Clientcd, ";
                //        strReportSelect += " cm_Name ,";
                //        strReportSelect += " Tmp_Mrgn Money,";
                //        strReportSelect += " Tmp_Coll Money,";
                //        strReportSelect += " Tmp_Short Money , ";
                //        strReportSelect += " Tmp_Ledger , ";
                //        strReportSelect += " Tmp_BenfHolding , ";
                //        strReportSelect += " Tmp_AvailCollat , ";
                //        strReportSelect += " Tmp_AvailDP , ";
                //        strReportSelect += " Tmp_EarlyPayIn , ";
                //        strReportSelect += " Tmp_UnClear , ";
                //        strReportSelect += " Tmp_Collected1 , ";
                //        strReportSelect += " Tmp_OtherCollected , ";
                //        strReportSelect += " Tmp_OtherCollected1 , ";
                //        strReportSelect += " Tmp_Shortfall PeakShort,";

                //        int intCols = 5;
                //        int i;
                //        for (i = 0; i < dtCorpAct.Rows.Count; i++)
                //        {
                //            string strExchSeg = dtCorpAct.Rows[i]["fm_exchange"] + "" + dtCorpAct.Rows[i]["fm_Segment"];
                //            string strExchSegName = "[" + UtilityModel.mfnGetExchangeCode2Desc(dtCorpAct.Rows[i]["fm_exchange"].ToString().Trim()) + "-" + UtilityModel.mfnGetSegmentCode2Desc(dtCorpAct.Rows[i]["fm_Segment"].ToString().Trim()) + "]";
                //            strCreate += " Tmp_InitMrgn" + strExchSeg + " Money,";
                //            strInsert += " 0 , ";
                //            strReportSelect += " Tmp_InitMrgn" + strExchSeg + ",";

                //            strCreate += " Tmp_M2MMrgn" + strExchSeg + " Money,";
                //            strInsert += " 0 , ";
                //            strReportSelect += " Tmp_M2MMrgn" + strExchSeg + ",";

                //            strCreate += " Tmp_InitColl" + strExchSeg + " Money,";
                //            strInsert += " 0 , ";
                //            strReportSelect += " Tmp_InitColl" + strExchSeg + ",";

                //            strCreate += " Tmp_M2MColl" + strExchSeg + " Money,";
                //            strInsert += " 0 , ";
                //            strReportSelect += " Tmp_M2MColl" + strExchSeg + ",";

                //            strCreate += " Tmp_InitShort" + strExchSeg + " Money,";
                //            strInsert += " 0 , ";
                //            strReportSelect += " Tmp_InitShort" + strExchSeg + ",";

                //            strCreate += " Tmp_M2MShort" + strExchSeg + " Money,";
                //            strInsert += " 0 , ";
                //            strReportSelect += " Tmp_M2MShort" + strExchSeg + ",";

                //            strCreate += " Tmp_Mrgn" + strExchSeg + " Money,";
                //            strInsert += " 0 , ";
                //            strReportSelect += " Tmp_Mrgn" + strExchSeg + ",";

                //            strCreate += " Tmp_Coll" + strExchSeg + " Money,";
                //            strInsert += " 0 , ";
                //            strReportSelect += " Tmp_Coll" + strExchSeg + ",";

                //            strCreate += " Tmp_Short" + strExchSeg + " Money " + ((i < dtCorpAct.Rows.Count - 1) ? " , " : "");
                //            strInsert += " 0 " + ((i < dtCorpAct.Rows.Count - 1) ? " , " : "");
                //            strReportSelect += " Tmp_Short" + strExchSeg + ((i < dtCorpAct.Rows.Count - 1) ? " , " : "");
                //        }
                //        strCreate += " ,Tmp_Statement  char(15)";
                //        strInsert += " , 'Not Available' ";
                //        strReportSelect += ",Tmp_Statement";
                //        strCreate += " ) ";
                //        mylib.ExecSQL(strCreate, curCon);

                //        strSql = " insert into #TmpMarginAnalysis ";
                //        strSql += strInsert + " from ( ";
                //        strSql += " SELECT distinct fm_clientcd FROM FMargins,Client_master Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + "and exists " + myutil.LoginAccess("fm_clientcd");
                //        if (IsTplusCommex == "Y")
                //        {
                //            // strSql += " union SELECT distinct fm_clientcd FROM " + SQLConnComex.Database + ".dbo.FMargins," + SQLConnComex.Database + ".dbo.Client_master Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "' " + strClientWhere + " and exists " + myutil.LoginAccess("fm_clientcd");
                //            strSql += " union SELECT distinct fm_clientcd FROM " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "' " + strClientWhere + " and exists " + myutil.LoginAccess("fm_clientcd");
                //        }
                //        strSql += " ) a ";
                //        mylib.ExecSQL(strSql, curCon);

                //        for (i = 0; i < dtCorpAct.Rows.Count; i++)
                //        {
                //            string strExchSeg = dtCorpAct.Rows[i]["fm_exchange"].ToString().Trim() + "" + dtCorpAct.Rows[i]["fm_segment"].ToString().Trim();

                //            if (dtCorpAct.Rows[i]["fm_segment"].ToString() == "X")
                //            {
                //                strSql = " Update #TmpMarginAnalysis set Tmp_InitMrgn" + strExchSeg + " = fm_initialmargin,Tmp_M2MMrgn" + strExchSeg + " = fm_MTMLoss,";
                //                strSql += " Tmp_InitColl" + strExchSeg + " = fm_collected,Tmp_M2MColl" + strExchSeg + " = fm_collected1,";
                //                strSql += " Tmp_InitShort" + strExchSeg + " = (fm_initialmargin)-fm_collected,Tmp_M2MShort" + strExchSeg + " = fm_MTMLoss-fm_collected1,";
                //                strSql += " Tmp_Mrgn" + strExchSeg + " = fm_initialmargin+fm_MTMLoss,Tmp_Coll" + strExchSeg + " = fm_collected+fm_collected1,Tmp_Short" + strExchSeg + " = fm_initialmargin+fm_MTMLoss-fm_collected-fm_collected1,";
                //                strSql += " Tmp_Mrgn = Tmp_Mrgn+(fm_initialmargin+fm_MTMLoss),Tmp_Coll=Tmp_Coll+(fm_collected+fm_collected1),Tmp_Short= Tmp_Short+(fm_initialmargin+fm_MTMLoss-fm_collected-fm_collected1)";
                //                strSql += " from (  ";
                //                strSql += " Select fm_Exchange,fm_clientcd,(case fm_exchange When 'M' Then fm_Regmargin + fm_exposurevalue + fm_buypremmargin else fm_initialmargin + fm_exposurevalue end) fm_initialmargin,  case fm_Exchange When 'M' Then fm_additionalmargin + fm_Tndmargin + fm_Dlvmargin - fm_SpreadBen + fm_ConcMargin + fm_DelvPMargin else fm_additionalmargin + fm_SplMargin end fm_MTMLoss, fm_collected, fm_collectedT2 fm_collected1 ";
                //                strSql += " From " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master  Where cm_cd=fm_clientcd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                //                strSql += " and fm_Dt = '" + strdate + "'";
                //                strSql += " union ";
                //                strSql += " Select po_exchange,po_clientcd,0 fm_initialmargin,-sum(po_futvalue) fm_MTMLoss,0 fm_collected,0 fm_collected1 ";
                //                strSql += " from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Fpositions," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master ";
                //                strSql += " Where po_clientcd=cm_cd and po_companycode='" + HttpContext.Current.Session["CompanyCode"] + "'  and po_dt ='" + strdate + "'";
                //                strSql += " Group By po_companycode,po_exchange,po_clientcd ";
                //                strSql += " Having -sum(po_futvalue) > 0 ";
                //                strSql += " ) a ";
                //                strSql += " Where Tmp_Clientcd =fm_clientcd and fm_Exchange  = '" + Strings.Left(strExchSeg, 1) + "'";
                //                mylib.ExecSQL(strSql, curCon);

                //                strSql = " Update #TmpMarginAnalysis set Tmp_InitColl" + strExchSeg + " = Tmp_InitColl" + strExchSeg + " + fc_collected,Tmp_M2MColl" + strExchSeg + " = Tmp_M2MColl" + strExchSeg + " + fc_collected1,";
                //                strSql += " Tmp_InitShort" + strExchSeg + " = Tmp_InitShort" + strExchSeg + "-fc_collected,Tmp_M2MShort" + strExchSeg + " = Tmp_M2MShort" + strExchSeg + "-fc_collected1,";
                //                strSql += " Tmp_Coll" + strExchSeg + " = Tmp_Coll" + strExchSeg + "+fc_collected+fc_collected1,Tmp_Short" + strExchSeg + " = Tmp_Short" + strExchSeg + "-fc_collected-fc_collected1,";
                //                strSql += " Tmp_Coll=Tmp_Coll+(fc_collected+fc_collected1),Tmp_Short= Tmp_Short-fc_collected-fc_collected1";
                //                strSql += " From Fmargin_Clients  ";
                //                strSql += " Where Tmp_Clientcd =fc_Filler1 and fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fc_Exchange = '" + Strings.Left(strExchSeg, 1) + "' and fc_segment = '" + Strings.Right(strExchSeg, 1) + "'";
                //                strSql += " and fc_Dt = '" + strdate + "' and fc_Filler1 <> '' ";
                //                mylib.ExecSQL(strSql, curCon);
                //            }
                //            else
                //            {
                //                strSql = " Update #TmpMarginAnalysis set Tmp_InitMrgn" + strExchSeg + " = fm_TotalMrgn-(fm_MTMLoss+case When fm_Segment = 'C' Then fm_additionalmargin else 0 end),Tmp_M2MMrgn" + strExchSeg + " = (fm_MTMLoss+case When fm_Segment = 'C' Then fm_additionalmargin else 0 end),";
                //                strSql += " Tmp_InitColl" + strExchSeg + " = fm_collected,Tmp_M2MColl" + strExchSeg + " = fm_collected1,";
                //                strSql += " Tmp_InitShort" + strExchSeg + " = (fm_TotalMrgn-(fm_MTMLoss+case When fm_Segment = 'C' Then fm_additionalmargin else 0 end))-fm_collected,Tmp_M2MShort" + strExchSeg + " = (fm_MTMLoss+case When fm_Segment = 'C' Then fm_additionalmargin else 0 end)-fm_collected1,";
                //                strSql += " Tmp_Mrgn" + strExchSeg + " = fm_TotalMrgn,Tmp_Coll" + strExchSeg + " = fm_collected+fm_collected1,Tmp_Short" + strExchSeg + " = fm_TotalMrgn-fm_collected-fm_collected1,";
                //                strSql += " Tmp_Mrgn = Tmp_Mrgn+(fm_TotalMrgn),Tmp_Coll=Tmp_Coll+(fm_collected+fm_collected1),Tmp_Short= Tmp_Short+(fm_TotalMrgn-fm_collected-fm_collected1)";
                //                strSql += " From FMargins  ";
                //                strSql += " Where Tmp_Clientcd =fm_clientcd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_Exchange = '" + Strings.Left(strExchSeg, 1) + "' and fm_segment = '" + Strings.Right(strExchSeg, 1) + "'";
                //                strSql += " and fm_Dt = '" + strdate + "'";
                //                mylib.ExecSQL(strSql, curCon);

                //                strSql = " Update #TmpMarginAnalysis set Tmp_InitColl" + strExchSeg + " = Tmp_InitColl" + strExchSeg + " + fc_collected,Tmp_M2MColl" + strExchSeg + " = Tmp_M2MColl" + strExchSeg + " + fc_collected1,";
                //                strSql += " Tmp_InitShort" + strExchSeg + " = Tmp_InitShort" + strExchSeg + "-fc_collected,Tmp_M2MShort" + strExchSeg + " = Tmp_M2MShort" + strExchSeg + "-fc_collected1,";
                //                strSql += " Tmp_Coll" + strExchSeg + " = Tmp_Coll" + strExchSeg + "+fc_collected+fc_collected1,Tmp_Short" + strExchSeg + " = Tmp_Short" + strExchSeg + "-fc_collected-fc_collected1,";
                //                strSql += " Tmp_Coll=Tmp_Coll+(fc_collected+fc_collected1),Tmp_Short= Tmp_Short-fc_collected-fc_collected1";
                //                strSql += " From Fmargin_Clients  ";
                //                strSql += " Where Tmp_Clientcd =fc_Filler1 and fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fc_Exchange = '" + Strings.Left(strExchSeg, 1) + "' and fc_segment = '" + Strings.Right(strExchSeg, 1) + "'";
                //                strSql += " and fc_Dt = '" + strdate + "' and fc_Filler1 <> '' ";
                //                mylib.ExecSQL(strSql, curCon);
                //            }
                //        }

                //        strSql = " Update #TmpMarginAnalysis set Tmp_Ledger = isnull(fc_TLedger,0),Tmp_BenfHolding = isnull(fc_AvailBenf,0),Tmp_AvailCollat = isnull(fc_AvailCollat,0),Tmp_AvailDP = isnull(fc_AvailDP,0),Tmp_EarlyPayIn = isnull(fc_FillerN2,0),Tmp_UnClear = isnull(fc_UnClear,0), Tmp_Collected1 = isnull(fc_Collected1,0) ";
                //        strSql += " From Fmargin_Clients  Where Tmp_Clientcd = fc_clientcd and fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fc_Exchange = '' and fc_dt = '" + strdate + "'";
                //        mylib.ExecSQL(strSql, curCon);

                //        strSql = " Update #TmpMarginAnalysis set Tmp_OtherCollected = isnull(fc_Collected,0) , Tmp_OtherCollected1 = isnull(fc_Collected1,0) from (  ";
                //        strSql += " select fc_Filler1 , Sum(fc_Collected) fc_Collected , Sum(fc_Collected1) fc_Collected1 ";
                //        strSql += " From Fmargin_Clients  Where fc_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'";
                //        strSql += " and fc_Exchange <> '' and fc_dt = '" + strdate + "'";
                //        strSql += " Group by fc_Filler1 ) a Where Tmp_Clientcd = fc_Filler1 ";
                //        mylib.ExecSQL(strSql, curCon);

                //        //if (chkOnlyShortFall == "1")
                //        //{
                //        //    strSql = " delete from #TmpMarginAnalysis where Tmp_Short <=0 ";
                //        //    mylib.ExecSQL(strSql, curCon);
                //        //}

                //        //if (chkOnlyShortFall == "1")
                //        //{
                //        //    strSql = " delete from #TmpPeakColl where Tmp_Shortfall <=0 ";

                //        //    mylib.ExecSQL(strSql, curCon);
                //        //}

                //        string StrActiveConn = mylib.fnFireQuery("other_products", "Count(0)", "op_status='A' AND op_product", "ESIGN-TRADEPLUS", true);
                //        if (StrActiveConn.Trim() == "1")
                //        {
                //            string StrValidConn = mydbutil.CheckConnection("ESIGN-TRADEPLUS");
                //            if (StrValidConn.Trim() == "1")
                //            {
                //                SqlConnection EsignConnection = mydbutil.commexTemp_conn("ESIGN-TRADEPLUS");
                //                strSql = " Update #TmpMarginAnalysis set Tmp_Statement='Download' from  " + "[" + EsignConnection.DataSource + "]" + "." + EsignConnection.Database + ".dbo.digital_details ";
                //                strSql += " Where dd_dt ='" + strdate + "'";
                //                strSql += " and dd_filetype='CMRG' and dd_clientcd = Tmp_Clientcd";
                //                mylib.ExecSQL(strSql, curCon);
                //            }
                //        }
                //        dt = mylib.OpenDataTable("select * from #TmpMarginAnalysis", curCon);
                //        dt = mylib.OpenDataTable("select * from #TmpPeakColl", curCon);
                //        string strSelect = "";
                //        switch (grouping.Trim())
                //        {
                //            case "CL":
                //                {
                //                    strSelect = "select '' as [BranchCode],";
                //                    break;
                //                }

                //            case "BR":
                //                {
                //                    strSelect = "select rtrim(bm_branchname) + '['+ ltrim(rtrim(cm_brboffcode))+']' as [BranchCode],";
                //                    break;
                //                }

                //            case "GR":
                //                {
                //                    strSelect = "select rtrim(gr_desc) + '['+ ltrim(rtrim(cm_groupcd)) + ']' as [BranchCode],";
                //                    break;
                //                }

                //            case "FM":
                //                {
                //                    strSelect = "select rtrim(fm_desc) + '[' + ltrim(rtrim(cm_familycd)) + ']' as [BranchCode],";
                //                    break;
                //                }
                //        }
                //        {
                //            strReportSelect = strSelect + strReportSelect;
                //            strReportSelect += ", Case When Tmp_Shortfall > Tmp_Short then Tmp_Shortfall else Tmp_Short end as 'Tmp_HighestShortFall'   ";
                //            strReportSelect += " from #TmpMarginAnalysis M ,Client_master,branch_master, group_master, family_master,#TmpPeakColl P Where M.Tmp_Clientcd = cm_cd and P.Tmp_Clientcd=cm_cd and cm_brboffcode = bm_branchcd and cm_groupcd=gr_cd and cm_familycd=fm_cd ";
                //            if (chkOnlyShortFall == "1")
                //            {

                //                strReportSelect += "and(Tmp_Short > 0 or Tmp_Shortfall > 0)";
                //            }

                //            strReportSelect += " Order by BranchCode,cm_Name,M.Tmp_Clientcd ";
                //        }
                //        // strReportSelect += " from ##TmpMarginAnalysis , Client_master Where Tmp_Clientcd = cm_cd  Order by cm_Name,Tmp_Clientcd ";
                //        dt = mylib.OpenDataTable(strReportSelect, curCon);
                //    }
                //}
                HttpContext.Current.Session["Data"] = dt;
                HttpContext.Current.Session["Data1"] = dt;
                return dt;
            }
        }

        public DataSet GetCMADetail(string strcd, string strdate)
        {
            DataTable DsMrgnDtl = null;

            string IsTplusCommex = "N";
            string strExchSegName;
            string strSql = "";
            string StrData = "";
            DataTable dtTemp;
            string strClientWhere = "";
            strClientWhere = " and fm_clientcd = '" + strcd + "'";
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
            //  strdate = myutil.dtos(strdate);
            DataSet ds = new DataSet();
            DataTable dtPeak = new DataTable();
            DataTable dtPeak1 = new DataTable();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                strSql = " SELECT fm_companycode,fm_exchange,fm_Segment,1 SortORder,COUNT(0) CNT FROM FMargins,Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange,fm_Segment";
                if (SQLConnComex != null)
                    strSql += " union all " + " SELECT fm_companycode,fm_exchange,'X',2 SortORder,COUNT(0) FROM " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins," + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange ";
                //strSql += " union all " + " SELECT fm_companycode,fm_exchange,'X',2 SortORder,COUNT(0) FROM " + SQLConnComex.Database + ".dbo.FMargins," + SQLConnComex.Database + ".dbo.Client_master " + " Where fm_clientcd = cm_cd and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt= '" + strdate + "'" + strClientWhere + " Group By fm_companycode,fm_exchange ";

                strSql += " ORder by fm_Segment,fm_exchange";
                dtTemp = mylib.OpenDataTable(strSql, curCon);

                double dblPeakMarginCash = 0;
                double dblPeakMarginFO = 0;
                double dblPeakMarginCurr = 0;
                double dblPeakMarginMCX = 0;
                double dblPeakMarginNCDEX = 0;
                double dblPeakMarginTotal = 0;
                double dblPeakCollected = 0;
                double dblTotalPeakShortFall = 0;
                string StrSelect = "";
                Boolean blnExchSegPeakCollected = false;

                if (Conversion.Val(mylib.fnFireQuery("Fmargin_PeakMargin", "Count(0)", "fc_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and fc_dt ", strdate, true)) > 0)
                { blnExchSegPeakCollected = true; }

                string strSQL;
                strSQL = "Select Upper(fm_segment) fm_segment , sum(fm_Nfiller4) fm_Nfiller4 From ";
                strSQL += " ( ";
                strSQL += " select  Case fm_segment When 'C' then 'Cash' when 'F' then 'F&O' when 'K' then 'Currency' end as fm_segment, isNull(fm_Nfiller4,0) fm_Nfiller4 ";
                strSQL += " From   FMargins ";
                strSQL += " Where fm_clientcd = '" + strcd + "' and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                strSQL += " and fm_Dt = '" + strdate + "' ";
                if (IsTplusCommex == "Y")
                {
                    strSQL += " Union All ";
                    strSQL += " select  Case fm_exchange When 'M' then 'MCX' when 'F' then 'NCDEX' when 'N' then 'NCDEX' end as fm_segment, isNull(fm_peakmargin, 0)+isNull(fm_Filler2,0) ";
                    strSQL += " From " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.FMargins ";
                    // strSQL += " From " + SQLConnComex.Database + ".dbo.FMargins ";
                    strSQL += " Where fm_clientcd = '" + strcd + "' and fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                    strSQL += " and fm_Dt = '" + strdate + "' ";
                }
                strSQL += " ) X ";
                strSQL += " Group By fm_segment ";
                dtPeak = mylib.OpenDataTable(strSQL, curCon);
                int intRow = 0;
                intRow = 0;
                StrSelect = "";
                DataTable dtP = new DataTable();
                DataTable DtPeakShort = new DataTable();
                strSQL = "";
                strSQL = " select Isnull(Sum(isNull(fc_FillerN9,0)),0) ";
                strSQL += " from Fmargin_Clients,Client_master ";
                strSQL += " where fc_clientcd=cm_cd and fc_Companycode ='" + HttpContext.Current.Session["CompanyCode"] + "' and fc_exchange = '' and fc_dt = '" + strdate + "' ";
                strSQL += " and fc_clientcd = '" + strcd + "' ";
                strSQL += " Group By fc_clientcd ";
                strSQL += " Having Isnull(Sum(isNull(fc_FillerN9,0)),0) > 0 ";
                dtP = mylib.OpenDataTable(strSQL);
                if (dtP.Rows.Count > 0)
                    dblPeakCollected = Conversion.Val(dtP.Rows[0][0].ToString());
                if (dtPeak.Rows.Count > 0)
                {
                    strSQL = "";
                    for (int iR = 0; iR < dtPeak.Rows.Count; iR++)
                    {
                        intRow += 1;
                        if (dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() == "CASH")
                        {
                            dblPeakMarginCash = Math.Round(Conversion.Val(dtPeak.Rows[iR]["fm_Nfiller4"]) * mylib.fnPeakFactor(strdate) / 100, 2);
                            strSQL += "Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + dblPeakMarginCash + "' as Value ";
                            if (blnExchSegPeakCollected)
                            {
                                dblPeakCollected = Conversion.Val(mylib.fnFireQuery("Fmargins", "isNull(Sum(isNull(fm_NFiller5,0)),0)", "fm_companycode+fm_Segment = '" + HttpContext.Current.Session["CompanyCode"] + "C' and fm_dt = '" + strdate.Trim() + "' and fm_clientcd", strcd, true));
                            }
                            StrSelect += (StrSelect != "" ? "Union all" : "") + " Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + ((dblPeakMarginCash - dblPeakCollected) > 0 ? dblPeakMarginCash - dblPeakCollected : 0) + "' as Value ";
                            dblTotalPeakShortFall += (dblPeakMarginCash - dblPeakCollected > 0 ? dblPeakMarginCash - dblPeakCollected : 0);
                        }
                        else if (dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() == "F&O")
                        {
                            dblPeakMarginFO = Math.Round(Conversion.Val(dtPeak.Rows[iR]["fm_Nfiller4"]) * mylib.fnPeakFactor(strdate) / 100, 2);
                            strSQL += (strSQL != "" ? "Union all " : "");
                            strSQL += "Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + dblPeakMarginFO + "' as Value ";
                            if (blnExchSegPeakCollected)
                            {
                                dblPeakCollected = Conversion.Val(mylib.fnFireQuery("Fmargins", "isNull(Sum(isNull(fm_NFiller5,0)),0)", "fm_companycode+fm_Segment = '" + HttpContext.Current.Session["CompanyCode"] + "F' and fm_dt = '" + strdate.Trim() + "' and fm_clientcd", strcd, true));
                            }
                            StrSelect += (StrSelect != "" ? "Union all" : "") + " Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + ((dblPeakMarginFO - dblPeakCollected) > 0 ? dblPeakMarginFO - dblPeakCollected : 0) + "' as Value ";
                            dblTotalPeakShortFall += (dblPeakMarginFO - dblPeakCollected > 0 ? dblPeakMarginFO - dblPeakCollected : 0);
                        }
                        else if (dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() == "CURRENCY")
                        {
                            dblPeakMarginCurr = Math.Round(Conversion.Val(dtPeak.Rows[iR]["fm_Nfiller4"]) * mylib.fnPeakFactor(strdate) / 100, 2);
                            strSQL += (strSQL != "" ? "Union all " : "");
                            strSQL += "Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + dblPeakMarginCurr + "' as Value ";
                            if (blnExchSegPeakCollected)
                            {
                                dblPeakCollected = Conversion.Val(mylib.fnFireQuery("Fmargins", "isNull(Sum(isNull(fm_NFiller5,0)),0)", "fm_companycode+fm_Segment = '" + HttpContext.Current.Session["CompanyCode"] + "K' and fm_dt = '" + strdate.Trim() + "' and fm_clientcd", strcd, true));
                            }
                            StrSelect += (StrSelect != "" ? "Union all" : "") + " Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + ((dblPeakMarginCurr - dblPeakCollected) > 0 ? dblPeakMarginCurr - dblPeakCollected : 0) + "' as Value ";
                            dblTotalPeakShortFall += (dblPeakMarginCurr - dblPeakCollected > 0 ? dblPeakMarginCurr - dblPeakCollected : 0);
                        }
                        else if (dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() == "MCX")
                        {
                            dblPeakMarginMCX = Math.Round(Conversion.Val(dtPeak.Rows[iR]["fm_Nfiller4"]), 2);
                            strSQL += (strSQL != "" ? "Union all " : "");
                            strSQL += "Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + dblPeakMarginMCX + "' as Value ";
                            if (blnExchSegPeakCollected)
                            {
                                dblPeakCollected = Conversion.Val(mylib.fnFireQuery("Fmargins", "isNull(Sum(isNull(fm_NFiller5,0)),0)", "fm_companycode+fm_Segment = '" + HttpContext.Current.Session["CompanyCode"] + "M' and fm_dt = '" + strdate.Trim() + "' and fm_clientcd", strcd, true));
                            }
                            StrSelect += (StrSelect != "" ? "Union all" : "") + " Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + ((dblPeakMarginMCX - dblPeakCollected) > 0 ? dblPeakMarginMCX - dblPeakCollected : 0) + "' as Value ";
                            dblTotalPeakShortFall += (dblPeakMarginMCX - dblPeakCollected > 0 ? dblPeakMarginMCX - dblPeakCollected : 0);
                        }
                        else if (dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() == "NCDEX")
                        {
                            dblPeakMarginNCDEX = Math.Round(Conversion.Val(dtPeak.Rows[iR]["fm_Nfiller4"]) * mylib.fnPeakFactor(strdate) / 100, 2);
                            strSQL += (strSQL != "" ? "Union all " : "");
                            strSQL += "Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + dblPeakMarginNCDEX + "' as Value ";
                            if (blnExchSegPeakCollected)
                            {
                                dblPeakCollected = Conversion.Val(mylib.fnFireQuery("Fmargins", "isNull(Sum(isNull(fm_NFiller5,0)),0)", "fm_companycode+fm_Segment = '" + HttpContext.Current.Session["CompanyCode"] + "F' and fm_dt = '" + strdate.Trim() + "' and fm_clientcd", strcd, true));
                            }
                            StrSelect += (StrSelect != "" ? "Union all" : "") + " Select '" + dtPeak.Rows[iR]["fm_segment"].ToString().Trim().ToUpper() + "' fm_segment ,'" + ((dblPeakMarginNCDEX - dblPeakCollected) > 0 ? dblPeakMarginNCDEX - dblPeakCollected : 0) + "' as Value ";
                            dblTotalPeakShortFall += (dblPeakMarginNCDEX - dblPeakCollected > 0 ? dblPeakMarginNCDEX - dblPeakCollected : 0);
                        }
                        dblPeakMarginTotal += Conversion.Val(dtPeak.Rows[iR]["fm_Nfiller4"]);
                    }
                    if (strSQL.Trim() != "")
                    {
                        strSQL += "Union all select 'Total'," + (dblPeakMarginCash + dblPeakMarginFO + dblPeakMarginCurr + dblPeakMarginMCX + dblPeakMarginNCDEX) + "";
                    }
                    dtPeak1 = mylib.OpenDataTable(strSQL, curCon);
                }
                strSQL = "";
                if (mylib.fnisPeakMargin(strdate))
                {
                    if (blnExchSegPeakCollected)
                    {
                        strSQL = "select Case fm_segment when 'C' then 'Cash' When 'F' then 'F&O' when 'K' then 'Currency' when 'X' then 'Commodity' end as fc_segment , isNull(fm_NFiller5,0) fc_collected ";
                        strSQL += " from Fmargins Where fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' and fm_clientcd = '" + strcd + "'";
                        if (IsTplusCommex == "Y")
                        {
                            strSQL += " Union all ";
                            strSQL += " select case fm_Exchange When 'M' then 'MCX' When 'F' Then 'NCDEX' else 'Commodity' end  as fm_segment , isNull(fm_Filler1,0) fc_collected ";
                            strSQL += " from " + "[" + SQLConnComex.DataSource + "]" + "." + SQLConnComex.Database + ".dbo.Fmargins Where fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' and fm_clientcd = '" + strcd + "'";
                            //  strSQL += " from " + SQLConnComex.Database + ".dbo.Fmargins Where fm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and fm_dt = '" + strdate + "' and fm_clientcd = '" + strcd + "'";
                        }
                        dtP = new DataTable();
                        dtP = mylib.OpenDataTable(strSQL);
                        double dblTotalPC = 0;
                        strSQL = "";
                        if (dtP.Rows.Count > 0)
                        {
                            for (int iR = 0; iR < dtP.Rows.Count; iR++)
                            {
                                double dblCollected = Conversion.Val(dtP.Rows[iR]["fc_collected"]);
                                intRow += 1;
                                strSQL += "Select '" + dtP.Rows[iR]["fc_segment"].ToString() + "' as fc_segment,'" + dblCollected + "' as Collected ";
                                dblTotalPC += Conversion.Val(dtP.Rows[iR]["fc_collected"]);
                            }
                            if (intRow > 1)
                            {
                                strSQL += "Select 'Total' as fc_segment,'" + dblTotalPC + "' as Collected ";
                            }
                        }
                        else
                        {
                            strSQL += "Select 'Collected' as fc_segment,'" + dblTotalPC + "' as Collected ";
                        }
                    }
                    else
                    {
                        strSQL += "Select 'Collected' as fc_segment,'" + dblPeakCollected + "' as Collected ";
                    }
                    dtP = new DataTable();
                    dtP = mylib.OpenDataTable(strSQL, curCon);
                    if (StrSelect.Trim() != "")
                    {
                        StrSelect += "Union all select 'Total','" + dblTotalPeakShortFall + "'";
                        DtPeakShort = mylib.OpenDataTable(StrSelect, curCon);
                    }
                }
                dtTemp.TableName = "Margin";
                ds.Tables.Add(dtTemp);
                dtPeak.TableName = "Peak";
                ds.Tables.Add(dtPeak);
                dtPeak1.TableName = "Peak1";
                ds.Tables.Add(dtPeak1);
                dtP.TableName = "dtP";
                ds.Tables.Add(dtP);
                DtPeakShort.TableName = "DtPeakShort";
                ds.Tables.Add(DtPeakShort);
            }
            return ds;
        }
        public DataTable Getholding(string cmbSelect, string strClient, string ChkExchange, string Chkdeposit, string chkInStNR, string CheckAllCompany, string chkDetails, string chkDPBalances, string chkShowLedger, string DT, string cmbActType, string ledgerBalDate, string cmbBranchWise, string chkExecuted)
        {
            DataTable Holding = new DataTable();
            string StrOrderBy = "";
            string strCompanyName;
            string strDPName = "";
            var cmdFetch = new SqlCommand();
            SqlCommand cmdtemp;
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            // string chkExecuted = "0";
            //string chkDetails = "0";
            //chkShowLedger = "1";
            string strActType = "";
            string strsql = "";
            //string chkShowLedger = "";
            //string Chkdeposit = "0";
            string ChkBranchWise = "0";
            string Addbranch = "";
            string addBranchtable = "";
            string AddBranchJoin = "";
            string AddBranchGroup = "";
            // string ledgerBalDate = "01/03/2020";
            //string chkInStNR = "";
            // string chkDPBalances = "0";
            string strpoa = "";
            string strSettle = "";
            string valSession = "";
            string strGroup = "";

            //--------optional pera ChkExchange
            // DT = "01/03/2020";
            // CheckAllCompany = "0";
            // cmbBranchWise = "Client";
            //  cmbActType = "A";
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {

                curCon.Open();

                DataTable dt = new DataTable();

                //if (chkExecuted == "1")
                //{
                //    //if (txtDT.Text == "")
                //    //{
                //    //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Error", "alert('Enter Date.');", true);
                //    //    return;
                //    //}
                //}


                strCompanyName = myLib.fnFireQueryOnlyPayout("Entity_master", "Em_Name", "em_name", "DANI SHARES%", curCon, true);

                strsql = "drop table #tmpHoldingrep";
                myLib.ExecSQL(strsql, curCon);

                strsql = "Create table #tmpHoldingrep (";
                strsql = strsql + " tmp_Ord char(1),";
                strsql = strsql + " tmp_mType Varchar(50),";
                strsql = strsql + " tmp_clientcd char(8),";
                strsql = strsql + " tmp_scripcd char(6),";
                strsql = strsql + " tmp_ssname nVarchar(24),";
                strsql = strsql + " tmp_dt char(8),";
                strsql = strsql + " tmp_isin char(12),";
                strsql = strsql + " tmp_actno Varchar(25),";
                strsql = strsql + " tmp_DpName Varchar(135),";
                strsql = strsql + " tmp_stlmnt char(9),";
                strsql = strsql + " tmp_cbqty numeric ,";
                strsql = strsql + " tmp_bcqty numeric ,";
                strsql = strsql + " tmp_Value money ,";
                strsql = strsql + " tmp_HairCut money ,";
                strsql = strsql + " tmp_AfterHairCut money ,";
                strsql = strsql + " tmp_Status Varchar(25),";
                strsql = strsql + " tmp_Days char(9) ,";
                strsql = strsql + " tmp_lnkUnPledge Varchar(135),";
                strsql = strsql + " tmp_UnPledge Varchar(135),";
                strsql = strsql + " tmp_Ledger money,";
                strsql = strsql + " tmp_lastprice money";
                strsql = strsql + " )";

                myLib.ExecSQL(strsql, curCon);

                strsql = "Create Clustered index #idx_tmpHoldingrep_clientcd on #tmpHoldingrep (tmp_clientcd)";
                myLib.ExecSQL(strsql, curCon);

                strsql = "Create index #idx_tmpHoldingrep_scripcd on #tmpHoldingrep (tmp_scripcd)";
                myLib.ExecSQL(strsql, curCon);



                string strWhere = "";
                string strWhere1 = "";
                string strStl = "";
                string strGrp = "";
                string str = "";
                string sql;
                string StrDate = "";
                string gHAIRCUT = Strings.Trim(myLib.GetSysParmSt("HAIRCUTVAL", ""));
                string gValuation = Strings.Trim(myLib.GetSysParmSt("RMSVALATLTRT", ""));
                double gAddHairCut;


                if (strClient.Trim() != "")
                {
                    switch (cmbSelect)
                    {
                        case "CL":
                            {
                                strWhere = strWhere + " and cm_cd = '" + strClient + "'";
                                strWhere1 = strWhere1 + " and cm_cd = '" + strClient + "'";
                                break;
                            }

                        case "GR":
                            {
                                strWhere = strWhere + " and cm_groupcd = '" + strClient + "'";
                                strWhere1 = strWhere1 + " and cm_groupcd = '" + strClient + "'";

                                break;
                            }

                        case "FM":
                            {
                                strWhere = strWhere + " and cm_familycd = '" + strClient + "'";
                                strWhere1 = strWhere1 + " and cm_familycd = '" + strClient + "'";
                                break;
                            }

                        case "SB":
                            {
                                strWhere = strWhere + " and cm_subbroker = '" + strClient + "'";
                                strWhere1 = strWhere1 + " and cm_subbroker = '" + strClient + "'";
                                break;
                            }

                        case "SE":
                            {
                                strWhere = strWhere + " and dm_scripcd = '" + strClient + "'";
                                strWhere1 = strWhere1 + " and im_scripcd = '" + strClient + "'";
                                break;
                            }
                        case "ALL":
                            {
                                strWhere = strWhere + " ";
                                strWhere1 = strWhere1 + " ";
                                break;
                            }
                    }

                }
                if (myLib.GetSysParmSt("FMRADDHRCUT", "") != "")

                {
                    gAddHairCut = Convert.ToDouble(myLib.GetSysParmSt("FMRADDHRCUT", ""));
                }
                else
                { gAddHairCut = 0; };

                sql = "select * from TradeNetControl where TNC_OptCode='948'";  // 948
                dt = myLib.OpenDataTable(sql, curCon);



                if (dt.Rows.Count > 0)
                {
                    string[] objarray;
                    objarray = dt.Rows[0]["tnc_filler1"].ToString().Trim().Split('|');

                    foreach (string m in objarray)
                    {
                        str = str + "'" + m + "'";
                        str = str + ",";
                    }


                    str = str.Substring(0, str.Length - 1);

                    strWhere = strWhere + " and ((od_acttype in ('B','M')  and dm_ourdp in (" + str + ")) or od_acttype not in ('B','M'))";

                }

                if (chkDetails == "1")
                {
                    strStl = "dm_stlmnt,";
                    strGrp = ",dm_stlmnt";
                }
                else
                {
                    strStl = "0 as dm_stlmnt,";
                }



                if (ChkBranchWise == "1")
                {
                    Addbranch = " select rtrim(bm_branchname) + ' [' + rtrim(bm_branchcd)+ ']' as Code ,";
                    addBranchtable = " ,Branch_master ";
                    AddBranchJoin = " cm_brboffcode=bm_branchcd and ";
                    AddBranchGroup = " bm_branchcd,bm_branchname, ";
                }
                else
                {
                    Addbranch = "select '1' ord,'Retained Holding' mType,rtrim(cm_name) + ' [' + rtrim(dm_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ) as Code,";

                }

                var switchExpr = cmbBranchWise;
                switch (switchExpr)
                {
                    case "Client":

                        Addbranch = "select rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ) as tmp_Code,'' as tmp_Code1,";
                        AddBranchGroup = "rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ),";
                        StrOrderBy = " cm_cd ";
                        break;

                    case "Branch":

                        Addbranch = " select rtrim(bm_branchname) + ' [' + rtrim(cm_brboffcode)+ ']' as tmp_Code ,rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ) as tmp_Code1, ";
                        addBranchtable = " ,Branch_master ";
                        AddBranchJoin = " cm_brboffcode=bm_branchcd and ";
                        AddBranchGroup = " cm_brboffcode,bm_branchname,rtrim(bm_branchname) + ' [' + rtrim(cm_brboffcode)+ ']'  ,rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ), ";
                        StrOrderBy = " cm_brboffcode,bm_branchcd";
                        break;
                    case "Group":

                        Addbranch = " select rtrim(gr_desc) + ' [' + rtrim(cm_groupcd)+ ']' as tmp_Code ,rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ) as tmp_Code1,";
                        addBranchtable = " ,Group_master ";
                        AddBranchJoin = " cm_groupcd=gr_cd and ";
                        AddBranchGroup = " cm_groupcd,gr_desc,rtrim(gr_desc) + ' [' + rtrim(cm_groupcd)+ ']'  ,rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ), ";
                        StrOrderBy = " cm_groupcd ,gr_cd ";
                        break;
                    case "Family":
                        Addbranch = " select rtrim(fm_desc) + ' [' + rtrim(cm_familycd)+ ']' as tmp_Code ,rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ) as tmp_Code1,";
                        addBranchtable = " ,Family_master ";
                        AddBranchJoin = " cm_familycd=fm_cd and ";
                        AddBranchGroup = " cm_familycd,fm_desc,rtrim(fm_desc) + ' [' + rtrim(cm_familycd)+ ']' ,rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ), ";
                        StrOrderBy = " cm_familycd, fm_cd ";
                        break;

                    case "Sub-Broker":
                        Addbranch = " select rtrim(rm_name) + ' [' + rtrim(rm_cd)+ ']' as tmp_Code,rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ) as tmp_Code1,";
                        addBranchtable = " ,Subbrokers ";
                        AddBranchJoin = " cm_subbroker=rm_cd and ";
                        AddBranchGroup = " rm_cd,rm_name,rtrim(rm_name) + ' [' + rtrim(rm_cd)+ ']'  ,rtrim(cm_name) + ' [' + rtrim(tmp_clientcd)+ ']'  + (case cm_poa when 'Y' then ' POA' else '' end ) , ";
                        StrOrderBy = " cm_subbroker,rm_cd ";
                        break;
                }

                //if (chkShowLedger == "1")
                //{
                //    strsql = "Drop table #tmphldLedger";
                //    myLib.ExecSQL(strsql, curCon);

                //    strsql = "select  ld_clientcd ,sum(ld_amount) as ld_amount into #tmphldLedger ";
                //    strsql = strsql + " from Ledger,Client_master ";
                //    strsql = strsql + " where ld_clientcd = cm_cd and  cm_schedule = 49843750 and left(ld_DPID,1) = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                //    strsql = strsql + " and cm_cd in (select dm_clientcd From #tmpHoldingrep) ";
                //    strsql = strsql + " and ld_dt <= '" + myutil.dtos(DT) + "'";
                //    strsql = strsql + " group by ld_clientcd ";
                //    myLib.ExecSQL(strsql, curCon);

                //}

                // string str = "";
                // string sql = "";
                sql = "select * from TradeNetControl where TNC_OptCode='948'";
                DataTable dt1 = new DataTable();

                dt1 = myLib.OpenDataTable(sql, curCon);

                if (dt1.Rows.Count > 0)
                {
                    string[] objarray;
                    objarray = dt1.Rows[0]["tnc_filler1"].ToString().Trim().Split('|');


                    foreach (string m in objarray)
                    {
                        str = str + "'" + m + "'";
                        str = str + ",";
                    }


                    str = str.Substring(0, str.Length - 1);
                    strsql = strsql + " and ((od_acttype in ('B','M')  and dm_ourdp in (" + str + ")) or od_acttype not in ('B','M'))";
                }





                if (chkExecuted == "1")
                {
                    StrDate = myutil.dtos(DT);
                    strsql = " Insert into #tmpHoldingrep ";
                    strsql = strsql + " Select '1' ord,'Retained Holding' mType,dm_clientcd tmp_clientcd,dm_scripcd tmp_scripcd,ss_name tmp_ssname,'' tmp_dt,";
                    strsql = strsql + " dm_isin tmp_isin,'' tmp_actno," + Interaction.IIf(Chkdeposit == "1", " rtrim(od_actname) + ' [' + rtrim(dm_ourdp) + ']'", " ''") + " tmp_DpName,";
                    strsql = strsql + Interaction.IIf(chkDetails == "1", " dm_stlmnt ", "'0'") + " as tmp_stlmnt,";
                    strsql = strsql + "0 tmp_cbqty, abs(sum(dm_qty)) tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut, ";
                    strsql = strsql + "'' tmp_Status,'' tmp_Days,'' tmp_lnkUnPledge,'' tmp_UnPledge,";
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", "isNull(ld_amount,0) ", "0") + "as tmp_Ledger, ";
                    strsql = strsql + "0 as tmp_Ledger, ";
                    strsql = strsql + " 0 tmp_lastprice ";
                    //strsql = strsql + " from Demat,Settlements, ourdps,securities ,Client_master" + Interaction.IIf(chkShowLedger == "1", " left outer join #tmphldLedger on ld_clientcd =  cm_cd", "") + " where ";
                    strsql = strsql + " from Demat,Settlements, ourdps,securities ,Client_master where ";
                    if (CheckAllCompany == "0")
                    {
                        strsql = strsql + " dm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                    }
                    else
                    {
                        strsql = strsql + " dm_companycode like '%' ";
                    }

                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                    strsql = strsql + " and dm_ourdp = od_cd and dm_stlmnt = se_stlmnt and dm_clientcd = cm_cd and cm_cd not in ( 'BEN999' )   and   dm_Dt <= '" + StrDate.Trim() + "' and dm_type = 'BC' and dm_locked = 'N' and dm_transfered = 'N' " + strWhere + strActType;
                    strsql = strsql + " and dm_scripcd=ss_cd group by Left(dm_stlmnt,1),dm_clientcd,dm_scripcd,ss_name,dm_isin  , case When charindex(Upper('Corporate Action'),Upper(dm_remark)) > 0 Then 'Y' else '' End" + Interaction.IIf(Chkdeposit == "1", " ,dm_ourdp,od_actname", "") + Interaction.IIf(chkDetails == "1", " ,dm_stlmnt ", "");
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", ",ld_amount ", "");
                    strsql = strsql + " having abs(sum(dm_qty)) > 0";

                    myLib.ExecSQL(strsql, curCon);



                    strsql = " Insert into #tmpHoldingrep ";
                    strsql = strsql + " Select '1' ord,'Retained Holding' mType,dm_clientcd tmp_clientcd,dm_scripcd tmp_scripcd,ss_name tmp_ssname,'' tmp_dt,";
                    strsql = strsql + " dm_isin tmp_isin,'' tmp_actno," + Interaction.IIf(Chkdeposit == "1", " rtrim(od_actname) + ' [' + rtrim(dm_ourdp) + ']' ", " ''") + " tmp_DpName,";
                    strsql = strsql + Interaction.IIf(chkDetails == "1", " dm_stlmnt ", "'0'") + " as tmp_stlmnt,";
                    strsql = strsql + "0 tmp_cbqty,sum(dm_qty) tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut, ";
                    strsql = strsql + "'' tmp_Status,'' tmp_Days,'' tmp_lnkUnPledge,'' tmp_UnPledge,";
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", "isNull(ld_amount,0) ", "0") + "as tmp_Ledger, ";
                    strsql = strsql + "0 as tmp_Ledger, ";
                    strsql = strsql + " 0 tmp_lastprice ";
                    //strsql = strsql + " from Demat,Settlements,ourdps,securities,client_master " + Interaction.IIf(chkShowLedger == "1", " left outer join #tmphldLedger on ld_clientcd =  cm_cd", "") + " where ";

                    strsql = strsql + " from Demat,Settlements,ourdps,securities,client_master where ";
                    if (CheckAllCompany == "0")
                    {
                        strsql = strsql + " dm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                    }
                    else
                    {
                        strsql = strsql + " dm_companycode like '%' ";
                    }

                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                    strsql = strsql + " and dm_stlmnt = se_stlmnt ";
                    strsql = strsql + " and od_cd = dm_ourdp and od_acttype = 'P'";
                    strsql = strsql + " and dm_clientcd = cm_cd and cm_cd not in ( 'BEN999' ) " + strWhere + strActType;
                    strsql = strsql + " and dm_type = 'BC'  and dm_locked in('X') ";
                    strsql = strsql + " and ( dm_execdt > '" + StrDate.Trim() + "' Or IsNull(dm_execdt,0) = 0 ) ";
                    strsql = strsql + " and dm_Dt <= '" + StrDate.Trim() + "' and dm_scripcd=ss_cd group by Left(dm_stlmnt,1),dm_clientcd,dm_scripcd,ss_name,dm_isin  ";
                    strsql = strsql + " ,case When charindex(Upper('Corporate Action'),Upper(dm_remark)) > 0 Then 'Y' else '' End " + Interaction.IIf(Chkdeposit == "1", " ,dm_ourdp,od_actname", "") + Interaction.IIf(chkDetails == "1", " ,dm_stlmnt ", "");
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", ",ld_amount ", "");
                    strsql = strsql + " having abs(sum(dm_qty)) > 0 ";

                    myLib.ExecSQL(strsql, curCon);



                    strsql = " Insert into #tmpHoldingrep ";
                    strsql = strsql + " Select '1' ord,'Retained Holding' mType,dm_clientcd tmp_clientcd,dm_scripcd tmp_scripcd,ss_name tmp_ssname,'' tmp_dt,";
                    strsql = strsql + " dm_isin tmp_isin,'' tmp_actno," + Interaction.IIf(Chkdeposit == "1", " rtrim(od_actname) + ' [' + rtrim(dm_ourdp) + ']' ", " ''") + " tmp_DpName,";
                    strsql = strsql + Interaction.IIf(chkDetails == "1", " dm_stlmnt ", "'0'") + " as tmp_stlmnt,";
                    strsql = strsql + "0 tmp_cbqty,sum(dm_qty) tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut, ";
                    strsql = strsql + "'' tmp_Status,'' tmp_Days,'' tmp_lnkUnPledge,'' tmp_UnPledge,";
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", "isNull(ld_amount,0) ", "0") + "as tmp_Ledger, ";
                    strsql = strsql + " 0 as tmp_Ledger, ";
                    strsql = strsql + " 0 tmp_lastprice ";
                    //strsql = strsql + " from Demat,Settlements,ourdps,securities,client_master " + Interaction.IIf(chkShowLedger == "1", " left outer join #tmphldLedger on ld_clientcd =  cm_cd", "") + " where ";
                    strsql = strsql + " from Demat,Settlements,ourdps,securities,client_master where ";
                    if (CheckAllCompany == "0")
                    {
                        strsql = strsql + " dm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                    }
                    else
                    {
                        strsql = strsql + " dm_companycode like '%' ";
                    }

                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                    strsql = strsql + " and dm_stlmnt = se_stlmnt and dm_clientcd = cm_cd and cm_cd not in ( 'BEN999' )";
                    strsql = strsql + " and od_cd = dm_ourdp and od_acttype in ('B','M','T','C') ";
                    strsql = strsql + " and dm_type = 'BC' and dm_locked in ('B','C','X') " + strWhere + strActType;
                    strsql = strsql + " and ( dm_execdt > '" + StrDate.Trim() + "' Or IsNull(dm_execdt,0) = 0 ) ";
                    strsql = strsql + " and dm_dt <= '" + StrDate.Trim() + "' and dm_scripcd=ss_cd  group by Left(dm_stlmnt,1),dm_clientcd,dm_scripcd,ss_name,dm_isin";
                    strsql = strsql + " ,case When charindex(Upper('Corporate Action'),Upper(dm_remark)) > 0 Then 'Y' else '' End" + Interaction.IIf(Chkdeposit == "1", " ,dm_ourdp,od_actname", "") + Interaction.IIf(chkDetails == "1", " ,dm_stlmnt ", "");
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", ",ld_amount ", "");
                    strsql = strsql + " having abs(sum(dm_qty)) > 0";

                    myLib.ExecSQL(strsql, curCon);



                    strsql = " Insert into #tmpHoldingrep ";
                    strsql = strsql + " Select '1' ord,'Retained Holding' mType,dm_clientcd tmp_clientcd,dm_scripcd tmp_scripcd,ss_name tmp_ssname,'' tmp_dt,";
                    strsql = strsql + " dm_isin tmp_isin,'' tmp_actno," + Interaction.IIf(Chkdeposit == "1", " rtrim(od_actname) + ' [' + rtrim(dm_ourdp) + ']' ", " ''") + " tmp_DpName,";
                    strsql = strsql + Interaction.IIf(chkDetails == "1", " dm_stlmnt ", "'0'") + " as tmp_stlmnt,";
                    strsql = strsql + "0 tmp_cbqty,sum(dm_qty * (-1)) tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut, ";
                    strsql = strsql + "'' tmp_Status,'' tmp_Days,'' tmp_lnkUnPledge,'' tmp_UnPledge,";
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", "isNull(ld_amount,0) ", "0") + "as tmp_Ledger, ";
                    strsql = strsql + " 0 as tmp_Ledger, ";
                    strsql = strsql + " 0 tmp_lastprice ";
                    //strsql = strsql + " from Demat,Settlements,ourdps,securities,client_master " + Interaction.IIf(chkShowLedger == "1", " left outer join #tmphldLedger on ld_clientcd =  cm_cd", "") + " where ";
                    strsql = strsql + " from Demat,Settlements,ourdps,securities,client_master where ";

                    if (CheckAllCompany == "0")
                    {
                        strsql = strsql + " dm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                    }
                    else
                    {
                        strsql = strsql + " dm_companycode like '%' ";
                    }

                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                    strsql = strsql + " and dm_stlmnt = se_stlmnt and dm_clientcd = cm_cd and cm_cd not in ( 'BEN999' )";
                    strsql = strsql + " and od_cd = dm_ourdp and od_acttype = 'P' ";
                    strsql = strsql + " and dm_type = 'BC' and dm_locked In('Y','O','C') " + strWhere + strActType;
                    strsql = strsql + " and ( dm_execdt > '" + StrDate.Trim() + "' Or IsNull(dm_execdt,0) = 0 ) ";
                    strsql = strsql + " and dm_dt <= '" + StrDate.Trim() + "'  and dm_scripcd=ss_cd   group by Left(dm_stlmnt,1),dm_clientcd,dm_scripcd,ss_name,dm_isin";
                    strsql = strsql + " ,case When charindex(Upper('Corporate Action'),Upper(dm_remark)) > 0 Then 'Y' else '' End" + Interaction.IIf(Chkdeposit == "1", " ,dm_ourdp,od_actname", "") + Interaction.IIf(chkDetails == "1", " ,dm_stlmnt ", "");
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", ",ld_amount ", "");
                    strsql = strsql + " having abs(sum(dm_qty)) > 0 ";
                    myLib.ExecSQL(strsql, curCon);




                    strsql = " Insert into #tmpHoldingrep ";
                    strsql = strsql + " Select '1' ord,'Retained Holding' mType,dm_clientcd tmp_clientcd,dm_scripcd tmp_scripcd,ss_name tmp_ssname,'' tmp_dt,";
                    strsql = strsql + " dm_isin tmp_isin,'' tmp_actno," + Interaction.IIf(Chkdeposit == "1", " rtrim(od_actname) + ' [' + rtrim(dm_ourdp) + ']'", " ''") + " tmp_DpName,";
                    strsql = strsql + Interaction.IIf(chkDetails == "1", " dm_stlmnt ", "'0'") + " as tmp_stlmnt,";
                    strsql = strsql + "0 tmp_cbqty,sum(dm_qty) tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut, ";
                    strsql = strsql + "'' tmp_Status,'' tmp_Days,'' tmp_lnkUnPledge,'' tmp_UnPledge,";
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", "isNull(ld_amount,0) ", "0") + "as tmp_Ledger, ";
                    strsql = strsql + " 0 as tmp_Ledger, ";
                    strsql = strsql + " 0 tmp_lastprice ";
                    //strsql = strsql + " from Demat,Settlements, ourdps,securities,client_master " + Interaction.IIf(chkShowLedger == "1", " left outer join #tmphldLedger on ld_clientcd =  cm_cd", "") + " where ";
                    strsql = strsql + " from Demat,Settlements,ourdps,securities,client_master where ";

                    if (CheckAllCompany == "0")
                    {
                        strsql = strsql + " dm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                    }
                    else
                    {
                        strsql = strsql + " dm_companycode like '%' ";
                    }

                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                    strsql = strsql + " and dm_ourdp = od_cd and dm_stlmnt = se_stlmnt and dm_clientcd = cm_cd and cm_cd not in ( 'BEN999' )";
                    strsql = strsql + " and dm_type = 'BC'  and dm_locked = 'N' and dm_transfered in ('Y','S') " + strWhere + strActType;
                    strsql = strsql + " and ( dm_execdt > '" + StrDate.Trim() + "' Or IsNull(dm_execdt,0) = 0 ) ";
                    strsql = strsql + " and dm_dt <= '" + StrDate.Trim() + "'  and dm_scripcd=ss_cd  group by Left(dm_stlmnt,1),dm_clientcd,dm_scripcd,ss_name,dm_isin ";
                    strsql = strsql + " ,case When charindex(Upper('Corporate Action'),Upper(dm_remark)) > 0 Then 'Y' else '' End " + Interaction.IIf(Chkdeposit == "1", " ,dm_ourdp,od_actname", "") + Interaction.IIf(chkDetails == "1", " ,dm_stlmnt ", "");
                    //strsql = strsql + Interaction.IIf(chkShowLedger == "1", ",ld_amount ", "");
                    strsql = strsql + " having abs(sum(dm_qty)) > 0";

                    myLib.ExecSQL(strsql, curCon);







                }
                else
                {







                    if (Strings.InStr("1", Strings.InStr(1, strCompanyName, "DANI SHARES").ToString()) > 0)
                    {
                    }
                    else
                    {
                        StrDate = DateTime.Now.ToString("yyyyMMdd");



                        strsql = " Insert into #tmpHoldingrep ";
                        strsql = strsql + " Select '1' ord,'Retained Holding' mType, dm_clientcd tmp_clientcd,dm_scripcd tmp_scripcd,ss_name tmp_ssname,";
                        strsql = strsql + " se_stdt tmp_dt,dm_isin tmp_isin,(case dm_tmkttype when 'X' then 'Mark For Sell' else '' end) tmp_actno,";
                        strsql = strsql + Interaction.IIf(Chkdeposit == "1", " rtrim(od_actname) + ' [' + rtrim(od_cd) + ']' ", " ''") + " as tmp_DpName,";
                        strsql = strsql + Interaction.IIf(chkDetails == "1", " dm_stlmnt ", "'0'") + " as tmp_stlmnt,";
                        strsql = strsql + " sum(case dm_type when 'CB' then dm_qty else 0 end ) tmp_cbqty,sum(case dm_type when 'BC' then (-1)*dm_qty else 0 end ) tmp_bcqty,";
                        strsql = strsql + " 0 tmp_Value,";
                        strsql = strsql + " 0 tmp_HairCut, 0 tmp_AfterHairCut,case sign(datediff(d,se_shpayoutdt,convert(char(8),getdate(),112))) when -1 then 'Expected' else '' end tmp_Status,";
                        if (chkDetails == "1")
                        {
                            strsql = strsql + " convert(varchar,max(datediff(dd,se_shpayoutdt,convert(char(8),getdate(),112)))) tmp_Days,";
                        }
                        else
                        {
                            strsql = strsql + " '' tmp_Days,";
                        }

                        strsql = strsql + "'' tmp_lnkUnPledge,'' tmp_UnPledge,";
                        strsql = strsql + " 0 as tmp_Ledger ,0 tmp_lastprice ";
                        //strsql = strsql + " from Demat, Settlements, Client_master " + Interaction.IIf(chkShowLedger == "1", "left outer join #tmphldLedger on ld_clientcd =  cm_cd", "") + ", securities,ourdps ";

                        strsql = strsql + " from Demat, Settlements, Client_master, securities,ourdps ";
                        if (CheckAllCompany == "1")
                        {
                            strsql = strsql + " where dm_companycode like '%' ";
                        }
                        else
                        {
                            strsql = strsql + " where dm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'";
                        }

                        strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                        strsql = strsql + HttpContext.Current.Session["LoginAccessOld"] + valSession + " and "; // 'session("loginaccess")
                        strsql = strsql + " dm_stlmnt = se_stlmnt and od_cd = dm_ourdp and dm_clientcd = cm_cd and ";
                        strsql = strsql + " dm_scripcd=ss_cd and (( dm_type='BC' and dm_locked = 'N' and dm_transfered = 'N') ";
                        strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                        if (chkInStNR == "1")
                        {
                            strsql = strsql + " or ( dm_type='CB' and dm_locked = 'N' and dm_transfered in ('Y','N'))) ";
                        }
                        else
                        {
                            strsql = strsql + " ) ";
                        }

                        if (cmbActType == "A")
                        {
                            strActType = "";
                        }
                        else if (cmbActType == "B")
                        {
                            strActType = " and od_acttype = 'B' ";
                        }
                        else if (cmbActType == "P")
                        {
                            strActType = " and od_acttype = 'P' ";
                        }
                        else if (cmbActType == "R")
                        {
                            strActType = " and od_acttype = 'R' ";
                        }
                        else if (cmbActType == "M")
                        {
                            strActType = " and od_acttype = 'M' ";
                        }

                        strsql = strsql + strWhere + strpoa + strActType + " group by " + strGroup + "dm_clientcd,dm_scripcd,dm_isin, cm_name, dm_type, ss_name,";
                        //strsql = strsql + Interaction.IIf(Chkdeposit == "1", " od_cd,od_actname,", "");
                        strsql = strsql + Interaction.IIf(Chkdeposit == "1", " od_cd,od_actname,", "") + Interaction.IIf(chkDetails == "1", " dm_stlmnt, ", "");
                        //strsql = strsql + " ss_MCXRATE,ss_BSERate,ss_NSERate ,ss_NSERateDT,ss_BSERATEDT,isnull((case when ss_BSERATEDT > ss_NSERateDT then ss_BSERATE else ss_NSERate end),0) ,";
                        //strsql = strsql + " case sign(datediff(d,se_shpayoutdt,convert(char(8),getdate(),112))) when -1 then 'Expected' else '' end ,dm_tmkttype,se_stdt" + Interaction.IIf(chkShowLedger == "1", ",ld_amount", "") + Interaction.IIf(chkDetails == "1", " ,dm_stlmnt ", "") + "  having abs(sum(dm_qty)) > 0";
                        strsql = strsql + " ss_MCXRATE,ss_BSERate,ss_NSERate ,ss_NSERateDT,ss_BSERATEDT,isnull((case when ss_BSERATEDT > ss_NSERateDT then ss_BSERATE else ss_NSERate end),0) ,";
                        strsql = strsql + " case sign(datediff(d,se_shpayoutdt,convert(char(8),getdate(),112))) when -1 then 'Expected' else '' end ,dm_tmkttype,se_stdt having abs(sum(dm_qty)) > 0";

                        myLib.ExecSQL(strsql, curCon);


                    }




                }


                if (chkDPBalances == "1")
                {

                    DataTable DTDPBalances;
                    DTDPBalances = myLib.OpenDataTable("select * from other_products where op_product = 'CROSS' and op_status = 'A'");
                    if (DTDPBalances.Rows.Count > 0)
                    {

                        string strCrossCon = "[" + DTDPBalances.Rows[0]["OP_Server"].ToString().Trim() + "]" + "." + "[" + DTDPBalances.Rows[0]["OP_DataBase"].ToString().Trim() + "]" + ".[dbo].";

                        strsql = " Insert into #tmpHoldingrep ";
                        strsql = strsql + " select '3' tmp_Ord,'Client DP Holding' tmp_mType,";
                        strsql = strsql + " cm_cd tmp_clientcd,isNull(im_scripcd,'') tmp_scripcd,isNull((select ss_Name from securities where isNull(im_scripcd,'') = ss_cd  and isNull(im_active,'') = 'Y'),sc_isinname ) tmp_ssname,'' tmp_dt,";
                        strsql = strsql + " hld_isin_code tmp_isin,'A/c  ' + da_actno tmp_actno,'DP' tmp_DpName,'' tmp_stlmnt, ";
                        strsql = strsql + " 0 tmp_cbqty,hld_ac_pos tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut,";
                        strsql = strsql + " '' tmp_Status,'' tmp_Days,'' tmp_lnkUnPledge,'' tmp_UnPledge,";
                        //strsql = strsql + Interaction.IIf(chkShowLedger == "1", "isNull(ld_amount,0) ", "0") + " as tmp_Ledger  ,0 tmp_lastprice ";
                        strsql = strsql + " 0 as tmp_Ledger  ,0 tmp_lastprice ";

                        //strsql = strsql + " from Client_master " + Interaction.IIf(chkShowLedger == "1", "left outer join #tmphldLedger on ld_clientcd =  cm_cd", "") + ",DematAct," + strCrossCon + "Holding left outer join ISIN on hld_isin_code = im_isin ," + strCrossCon + "security" + Interaction.IIf(cmbBranchWise == "Client", "", "," + strCrossCon + addBranchtable.Replace(",", ""));
                        strsql = strsql + " from Client_master ,DematAct," + strCrossCon + "Holding left outer join ISIN on hld_isin_code = im_isin ," + strCrossCon + "security" + Interaction.IIf(cmbBranchWise == "Client", "", "," + strCrossCon + addBranchtable.Replace(",", ""));

                        strsql = strsql + " where cm_cd = da_clientcd  and da_defaultyn = 'Y' and da_status = 'A' " + HttpContext.Current.Session["LoginAccessOld"] + strWhere1;
                        if (cmbSelect == "SE")
                        {
                            strsql = strsql + " and hld_isin_code = im_isin and im_active = 'Y' and da_actno = hld_ac_code and hld_isin_code = sc_isincode  ";
                        }
                        else
                        {
                            strsql = strsql + " and im_active = 'Y' and da_actno = hld_ac_code and hld_isin_code = sc_isincode  ";
                        }

                        strsql = strsql + " and im_scripcd not between '600001' and '699999' and hld_Ac_type in ('11')";

                        myLib.ExecSQL(strsql, curCon);


                    }

                    DTDPBalances = myLib.OpenDataTable("select * from other_products where op_product = 'ESTRO' and op_status = 'A'");

                    if (DTDPBalances.Rows.Count > 0)
                    {
                        String strEstroCon = "[" + DTDPBalances.Rows[0]["OP_Server"].ToString().Trim() + "]" + "." + "[" + DTDPBalances.Rows[0]["OP_DataBase"].ToString().Trim() + "]" + ".[dbo].";

                        strsql = " Insert into #tmpHoldingrep ";
                        strsql = strsql + " select '2' tmp_Ord,'Client DP Holding' tmp_mType,";
                        strsql = strsql + " cm_cd tmp_clientcd,isNull(im_scripcd,'') tmp_scripcd,isNull((select ss_Name from securities where isNull(im_scripcd,'') = ss_cd  and isNull(im_active,'') = 'Y'),sc_isinname )  tmp_ssname,'' tmp_dt, ";
                        strsql = strsql + " hld_isin_code tmp_isin, 'A/c  ' + da_actno tmp_actno,'DP' tmp_DpName,'' tmp_stlmnt, ";
                        strsql = strsql + " 0 tmp_cbqty, hld_ac_pos tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut, ";
                        strsql = strsql + " '' tmp_Status,'' tmp_Days,'' tmp_lnkUnPledge,'' tmp_UnPledge,";
                        //strsql = strsql + Interaction.IIf(chkShowLedger == "1", "isNull(ld_amount,0) ", "0") + " as tmp_Ledger ,0 tmp_lastprice  ";
                        strsql = strsql + " 0 as tmp_Ledger  ,0 tmp_lastprice ";
                        //strsql = strsql + " from Client_master " + Interaction.IIf(chkShowLedger == "1", "left outer join #tmphldLedger on ld_clientcd =  cm_cd", "") + ",DematAct," + strEstroCon + "sysParameter," + strEstroCon + "Holding left outer join ISIN on hld_isin_code = im_isin ," + strEstroCon + "security" + Interaction.IIf(cmbBranchWise == "Client", "", "," + strEstroCon + addBranchtable.Replace(",", ""));
                        strsql = strsql + " from Client_master ,DematAct," + strEstroCon + "sysParameter," + strEstroCon + "Holding left outer join ISIN on hld_isin_code = im_isin ," + strEstroCon + "security" + Interaction.IIf(cmbBranchWise == "Client", "", "," + strEstroCon + addBranchtable.Replace(",", ""));

                        strsql = strsql + " where cm_cd = da_clientcd  and da_defaultyn = 'Y' and da_status = 'A' " + HttpContext.Current.Session["LoginAccessOld"] + strWhere1;
                        if (cmbSelect == "SE")
                        {
                            strsql = strsql + " and hld_isin_code = im_isin and im_active = 'Y' and da_actno = hld_ac_code and sp_parmcd= 'DPID' and sp_sysvalue = da_dpid  and hld_isin_code = sc_isincode  ";
                        }
                        else
                        {
                            strsql = strsql + " and im_active = 'Y' and da_actno = hld_ac_code and sp_parmcd= 'DPID' and sp_sysvalue = da_dpid and hld_isin_code = sc_isincode  ";
                        }

                        strsql = strsql + " and im_scripcd not between '600001' and '699999' and  hld_Ac_type in ('22') ";

                        myLib.ExecSQL(strsql, curCon);

                    }

                }
                if (Convert.ToInt16(myLib.fnFireQuery("sysobjects", "count(0)", "name", "MrgPledge_TRX", true)) > 0)
                {

                    strsql = " Insert into #tmpHoldingrep ";
                    strsql = strsql + " select '4' tmp_Ord,'Pledge Holding' tmp_mType,";
                    strsql = strsql + " MPT_clientcd tmp_clientcd,MPT_scripcd tmp_scripcd,ss_name tmp_ssname,'' tmp_dt, ";
                    strsql = strsql + " im_isin tmp_isin,'Pledge Balance' as tmp_actno,'Margin Pledge' tmp_DpName,'' tmp_stlmnt, ";
                    strsql = strsql + " 0 tmp_cbqty,sum(case When MPT_DRCR ='C' Then MPT_Qty else -MPT_Qty  end) tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut, ";
                    strsql = strsql + " '' tmp_Status,'' tmp_Days, ";
                    strsql = strsql + " rtrim(cm_name) + ' [' + rtrim(cm_cd)+ ']' + (case cm_poa when 'Y' then ' POA' else '' end ) +'/'+ MPT_scripcd +'/' + ss_name +'/'+ convert(varchar,sum(case When MPT_DRCR ='C' Then MPT_Qty else -MPT_Qty  end)) +'/'+ rtrim(cm_cd) +'/P' tmp_lnkUnPledge, ";
                    strsql = strsql + " 'Un-Pledge ' + isNull((select case when Rq_Qty > 0 then ' [' + convert(varchar,Rq_Qty) +'] Qty' else '' end from PledgeRequest where Rq_Status1='X' and Rq_Status3 ='P' and Rq_Clientcd=MPT_clientcd and Rq_Scripcd=MPT_scripcd),'') tmp_UnPledge,0 tmp_Ledger ";
                    strsql = strsql + " ,0 tmp_lastprice";
                    strsql = strsql + " from MrgPledge_TRX a, Isin, Client_master, Securities where MPT_clientcd = cm_cd and  MPT_scripcd=ss_cd and ";
                    strsql = strsql + " im_scripcd = ss_cd and im_active = 'Y' and im_priority = (select min(im_priority) from Isin Where im_scripcd = MPT_scripcd and im_active = 'Y' ) and MPT_scripcd = im_scripcd ";

                    strsql = strsql + strWhere1;

                    strsql = strsql + " and MPT_dt <= '" + StrDate.Trim() + "' ";
                    strsql = strsql + " and exists ( select MPT_OurDP from MrgPledge_TRX b Where MPT_TRXFlag ='P' and a.MPT_OurDP = b.MPT_OurDP ";

                    if (cmbSelect != "SE")
                        strsql = strsql + Interaction.IIf(!string.IsNullOrEmpty(Strings.Trim(strClient)), " and MPT_clientcd = '" + Strings.Trim(strClient) + "'", "");

                    strsql = strsql + ")";
                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                    strsql = strsql + " group by rtrim(cm_name) + ' [' + rtrim(MPT_clientcd)+ ']'  + 'Margin PLedge',";
                    strsql = strsql + " MPT_clientcd, MPT_scripcd, im_isin, cm_name, ss_name,ss_BSERATEDT,ss_NSERateDT,ss_BSERATE,ss_NSERate,cm_cd,cm_poa ";
                    strsql = strsql + " having(abs(sum(case When MPT_DRCR ='C' Then MPT_Qty else -MPT_Qty  end)) > 0)";

                    myLib.ExecSQL(strsql, curCon);



                    strsql = " Insert into #tmpHoldingrep ";
                    strsql = strsql + " Select '4' tmp_Ord,'Pledge Holding' tmp_mType,";
                    strsql = strsql + " MPT_clientcd tmp_clientcd, MPT_scripcd tmp_scripcd,ss_name tmp_ssname, '' tmp_dt, ";
                    strsql = strsql + " im_isin tmp_isin,'Re-pledge Balance' tmp_actno,'Margin Pledge' tmp_DpName,'' tmp_stlmnt, ";
                    strsql = strsql + " 0 tmp_cbqty,Sum(case MPT_DRCR when 'D' Then MPT_Qty else -MPT_Qty end) tmp_bcqty,0 tmp_Value,0 tmp_HairCut,0 tmp_AfterHairCut,";
                    strsql = strsql + " '' tmp_Status,'' tmp_Days, ";
                    strsql = strsql + " rtrim(cm_name) + ' [' + rtrim(cm_cd)+ ']' + (case cm_poa when 'Y' then ' POA' else '' end ) +'/'+ MPT_scripcd +'/' + ss_name +'/'+ convert(varchar,sum(case When MPT_DRCR ='D' Then MPT_Qty else -MPT_Qty  end))+'/'+ rtrim(cm_cd) +'/R' tmp_lnkUnPledge, ";
                    strsql = strsql + " 'Un-Re-Pledge ' + isNull((select case when Rq_Qty > 0 then ' [' + convert(varchar,Rq_Qty) +'] Qty' else '' end from PledgeRequest where Rq_Status1='X' and Rq_Status3 ='R' and Rq_Clientcd=MPT_clientcd and Rq_Scripcd=MPT_scripcd),'') tmp_UnPledge,0 tmp_Ledger ";
                    strsql = strsql + " ,0 tmp_lastprice";
                    strsql = strsql + " from MrgPledge_TRX a, Isin, Client_master, Securities where MPT_TRXFlag ='R' and MPT_clientcd = cm_cd and  MPT_scripcd=ss_cd and ";
                    strsql = strsql + " im_scripcd = ss_cd and im_active = 'Y' and im_priority = (select min(im_priority) from Isin Where im_scripcd = MPT_scripcd and im_active = 'Y' ) and MPT_scripcd = im_scripcd ";
                    strsql = strsql + strWhere1;
                    strsql = strsql + " and MPT_dt <= '" + StrDate.Trim() + "' ";
                    strsql = strsql + " and exists ( select MPT_OurDP from MrgPledge_TRX b Where MPT_TRXFlag ='P' and a.MPT_OurDP = b.MPT_OurDP ";
                    if (cmbSelect != "SE")
                        strsql = strsql + Interaction.IIf(!string.IsNullOrEmpty(Strings.Trim(strClient)), " and MPT_clientcd = '" + Strings.Trim(strClient) + "'", "");

                    strsql = strsql + ")";

                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"];
                    strsql = strsql + " group by rtrim(cm_name) + ' [' + rtrim(MPT_clientcd)+ ']'  + 'Margin PLedge',";
                    strsql = strsql + " MPT_clientcd, MPT_scripcd, im_isin, cm_name, ss_name,ss_BSERATEDT,ss_NSERateDT,ss_BSERATE,ss_NSERate,cm_cd,cm_poa ";
                    strsql = strsql + " having(abs(Sum(case MPT_DRCR when 'D' Then MPT_Qty else -MPT_Qty end)) > 0)";


                    myLib.ExecSQL(strsql, curCon);


                }

                string strRMSVALATLTRT = "";

                strRMSVALATLTRT = myLib.GetSysParmSt("RMSVALATLTRT", "");

                strsql = "update #tmpHoldingrep set tmp_lastprice = mk_closerate from Market_rates";
                strsql = strsql + " where mk_scripcd = tmp_scripcd and mk_exchange ='B'";
                strsql = strsql + " and mk_dt = (select max(mk_dt) from Market_rates where mk_exchange = 'B'";
                strsql = strsql + " and mk_scripcd = tmp_scripcd ";
                if (Convert.ToInt32(strRMSVALATLTRT) > 0)
                {
                    strsql = strsql + " and mk_dt >='" + myutil.dtos(myutil.SubDayDT(StrDate.Trim(), Convert.ToInt32(strRMSVALATLTRT)).ToString()) + "'";
                }

                strsql = strsql + " and mk_dt <='" + StrDate.Trim() + "'";
                strsql = strsql + " )";
                myLib.ExecSQL(strsql, curCon);



                strsql = "update #tmpHoldingrep set tmp_lastprice = mk_closerate from Market_rates";
                strsql = strsql + " where mk_scripcd = tmp_scripcd and mk_exchange ='N'";
                strsql = strsql + " and mk_dt = (select max(mk_dt) from Market_rates where mk_exchange = 'N'";
                strsql = strsql + " and mk_scripcd = tmp_scripcd ";
                if (Convert.ToInt32(strRMSVALATLTRT) > 0)
                {
                    strsql = strsql + " and mk_dt >='" + myutil.dtos(myutil.SubDayDT(StrDate.Trim(), Convert.ToInt32(strRMSVALATLTRT)).ToString()) + "'";
                }

                strsql = strsql + " and mk_dt <='" + StrDate.Trim() + "'";
                strsql = strsql + " )";
                myLib.ExecSQL(strsql, curCon);


                strsql = "update #tmpHoldingrep set tmp_Value = (tmp_bcqty * tmp_lastprice)  ";
                myLib.ExecSQL(strsql, curCon);

                strsql = "Update #tmpHoldingrep set tmp_HairCut = 100 ";
                myLib.ExecSQL(strsql, curCon);

                strsql = " update #tmpHoldingrep set tmp_HairCut = case vm_exchange When 'B' then vm_margin_rate  else vm_applicable_var end from VarMargin ";
                strsql = strsql + " where vm_exchange = 'B' and vm_scripcd = tmp_scripcd ";
                strsql = strsql + " and vm_dt =(select max(vm_dt) from VarMargin where vm_exchange = 'B' and vm_scripcd = tmp_scripcd ";
                strsql = strsql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + StrDate.Trim() + "')";
                myLib.ExecSQL(strsql, curCon);


                if (Convert.ToInt32(gValuation) > 0)
                {
                    strsql = " update #tmpHoldingrep set tmp_HairCut = case vm_exchange When 'B' then vm_margin_rate  else vm_applicable_var end from VarMargin,Securities ";
                    strsql = strsql + " where vm_exchange = 'B' and vm_scripcd = tmp_scripcd And vm_scripcd = ss_cd And ss_group = 'F' ";
                    strsql = strsql + " and vm_dt =(select max(vm_dt) from VarMargin where vm_exchange = 'B' and vm_scripcd = tmp_scripcd ";
                    strsql = strsql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "1", "<=", "<") + " '" + StrDate.Trim() + "')";
                    myLib.ExecSQL(strsql, curCon);
                }


                strsql = " update #tmpHoldingrep set tmp_HairCut = case vm_exchange When 'B' then vm_margin_rate  else vm_applicable_var end from VarMargin ";
                strsql = strsql + " where vm_exchange = 'N' and vm_scripcd = tmp_scripcd ";
                strsql = strsql + " and vm_dt =(select max(vm_dt) from VarMargin where vm_exchange = 'N' and vm_scripcd = tmp_scripcd ";
                strsql = strsql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + StrDate.Trim() + "')";
                myLib.ExecSQL(strsql, curCon);

                if (Convert.ToInt32(gValuation) > 0)
                {
                    strsql = " update #tmpHoldingrep set tmp_HairCut = case vm_exchange When 'B' then vm_margin_rate  else vm_applicable_var end from VarMargin,Securities ";
                    strsql = strsql + " where vm_exchange = 'N' and vm_scripcd = tmp_scripcd And vm_scripcd = ss_cd And ss_group = 'F'";
                    strsql = strsql + " and vm_dt =(select max(vm_dt) from VarMargin where vm_exchange = 'N' and vm_scripcd = tmp_scripcd ";
                    strsql = strsql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + StrDate.Trim() + "')";
                    myLib.ExecSQL(strsql, curCon);
                }

                if (Convert.ToInt32(gAddHairCut) > 0)
                {
                    strsql = " update #tmpHoldingrep set tmp_HairCut = tmp_HairCut + " + gAddHairCut + " Where tmp_HairCut <= 100 - " + gAddHairCut;
                    myLib.ExecSQL(strsql, curCon);
                }

                strsql = " update #tmpHoldingrep set tmp_AfterHairCut = case tmp_Value when 0 then 0 else tmp_Value - (round((tmp_Value * tmp_HairCut)/100,2)) end";
                myLib.ExecSQL(strsql, curCon);


                //if (chkShowLedger == "1")
                //{
                strsql = " update #tmpHoldingrep Set tmp_ledger = X.ld_amount";
                strsql = strsql + " from (select ld_clientcd, isNull(sum(ld_amount),0) ld_amount from Ledger,Client_master  ";
                strsql = strsql + " where ld_clientcd = cm_cd and  cm_schedule = 49843750 and left(ld_DPID,1) = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                strsql = strsql + " and ld_dt <= '" + myutil.dtos(ledgerBalDate) + "'";
                strsql = strsql + "   and ld_clientcd in (select  tmp_clientcd from #tmpHoldingrep ) group by ld_clientcd ) X Where tmp_clientcd = X.ld_clientcd";
                myLib.ExecSQL(strsql, curCon);
                //}
                //strsql = Addbranch + "tmp_clientcd,tmp_ord,tmp_dt,tmp_mType,tmp_scripcd,tmp_ssname,tmp_isin,tmp_DpName,tmp_cbqty,tmp_stlmnt,tmp_bcqty,abs(cast(tmp_Value as decimal(15,2))) tmp_Value,tmp_Status,tmp_actno,";
                //strsql = strsql + " abs(cast(tmp_HairCut as decimal(15,2))) tmp_HairCut,abs(cast(tmp_AfterHairCut as decimal(15,2))) tmp_AfterHairCut,tmp_Ledger,tmp_Days,tmp_lnkUnPledge,tmp_UnPledge";
                //strsql = strsql + " From  #tmpHoldingrep,Client_Master " + addBranchtable + " where " + AddBranchJoin + " cm_cd=tmp_clientcd Group by " + AddBranchGroup + StrOrderBy + ", cm_cd,tmp_clientcd,";
                //strsql = strsql + " tmp_ord,tmp_dt,tmp_mType,tmp_scripcd,tmp_ssname,tmp_isin,tmp_DpName,tmp_cbqty,tmp_stlmnt,tmp_bcqty,tmp_Value,tmp_Status,tmp_actno,tmp_Ledger,tmp_Days,tmp_lnkUnPledge,tmp_UnPledge,tmp_HairCut,tmp_AfterHairCut ";
                //strsql = strsql + " Order by " + StrOrderBy + " ,tmp_ssname,tmp_ord  ";

                strsql = Addbranch + "tmp_clientcd,tmp_ord,tmp_dt,tmp_mType,tmp_scripcd,tmp_ssname,tmp_isin,tmp_DpName,sum(tmp_cbqty) as tmp_cbqty,tmp_stlmnt,abs(sum(tmp_bcqty)) as tmp_bcqty,tmp_lastprice as rate ,abs(cast(sum(tmp_Value) as decimal(15,2))) tmp_Value,tmp_Status,tmp_actno,";
                strsql = strsql + " abs(cast(tmp_HairCut as decimal(15,2))) tmp_HairCut,abs(cast(sum(tmp_AfterHairCut) as decimal(15,2))) tmp_AfterHairCut,tmp_Ledger,tmp_Days,tmp_lnkUnPledge,tmp_UnPledge";
                strsql = strsql + " From  #tmpHoldingrep,Client_Master " + addBranchtable + " where " + AddBranchJoin + " cm_cd=tmp_clientcd Group by " + AddBranchGroup + StrOrderBy + ", cm_cd,tmp_clientcd,";
                strsql = strsql + " tmp_ord,tmp_dt,tmp_mType,tmp_scripcd,tmp_ssname,tmp_isin,tmp_DpName,tmp_cbqty,tmp_stlmnt,tmp_bcqty,tmp_Value,tmp_Status,tmp_actno,tmp_Ledger,tmp_Days,tmp_lnkUnPledge,tmp_UnPledge,tmp_HairCut,tmp_AfterHairCut,tmp_lastprice ";
                strsql = strsql + " Order by " + StrOrderBy + " ,tmp_ssname,tmp_ord  ";


                Holding = myLib.OpenDataTable(strsql, curCon);

                if (chkShowLedger == "1")
                {
                    string strClCd;
                    int i = 0;
                    var loopTo = Holding.Rows.Count - 1;
                    for (i = 0; i <= loopTo; i++)
                    {
                        if (cmbBranchWise == "Client")
                        {
                            strClCd = Holding.Rows[i]["tmp_Code"].ToString();
                            if (strClCd == Holding.Rows[i]["tmp_Code"].ToString())
                            {
                                Holding.Rows[i]["tmp_Code"] = Strings.Trim(Holding.Rows[i]["tmp_Code"].ToString()) + "  " + " [Ledger Balance " + Holding.Rows[i]["tmp_Ledger"].ToString() + " " + Interaction.IIf(Convert.ToInt32(Holding.Rows[i]["tmp_Ledger"]) < 0, "Cr", "Dr") + " as on " + ledgerBalDate.Trim().ToString() + "]";
                            }
                        }
                        else
                        {
                            strClCd = Holding.Rows[i]["tmp_Code1"].ToString();
                            if (strClCd == Holding.Rows[i]["tmp_Code1"].ToString())
                            {
                                Holding.Rows[i]["tmp_Code1"] = Strings.Trim(Holding.Rows[i]["tmp_Code1"].ToString()) + "  " + " [Ledger Balance " + Holding.Rows[i]["tmp_Ledger"].ToString() + " " + Interaction.IIf(Convert.ToInt32(Holding.Rows[i]["tmp_Ledger"]) < 0, "Cr", "Dr") + " as on " + ledgerBalDate.Trim().ToString() + "]";
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j <= Holding.Rows.Count - 1; j++)
                    {
                        Holding.Rows[j]["tmp_Code"] = Strings.Trim(Holding.Rows[j]["tmp_Code"].ToString());
                    }
                }

                if (cmbBranchWise != "Client")
                {
                    Holding.Columns["tmp_Code"].ColumnName = "New";
                    Holding.Columns["tmp_Code1"].ColumnName = "New1";

                    Holding.Columns["New"].ColumnName = "tmp_Code1";
                    Holding.Columns["New1"].ColumnName = "tmp_Code";
                    Holding.AcceptChanges();
                }

            }


            return Holding;
        }



        public DataTable GetInvplGainLossDetail(string cmbReport, string Code, string FDate, string ToDate, string AsonDate, string CBJobbing, string CBDelivery, string CBIgnoreEffect, string CBDetail)
        {

            DataTable dt = null;
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            INVPLProcess ObjINVPL = new INVPLProcess();
            DataTable objdatatable = new DataTable();

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                string gstrToday = DateTime.Today.ToString("yyyyMMdd");
                HttpContext.Current.Session["gstrDBToday"] = mylib.OpenDataSet("select convert(char(8),getdate(),112)").Tables[0].Rows[0][0];
                HttpContext.Current.Session["colspan"] = "";

                if (cmbReport == "PL" & CBDetail == "0")
                {
                    if (CBDelivery == "1" & CBJobbing == "1")
                    {
                        dt = ObjINVPL.FnGetActualPLSummary(Code.Trim(), FDate, ToDate, "B", (CBIgnoreEffect == "1") ? "Y" : "N");
                        HttpContext.Current.Session["colspan"] = "9";
                    }
                    else if (CBDelivery == "1")
                    {
                        dt = ObjINVPL.FnGetActualPLSummary(Code.Trim(), FDate, ToDate, "D", (CBIgnoreEffect == "1") ? "Y" : "N");
                        HttpContext.Current.Session["colspan"] = "8";
                    }
                    else if (CBJobbing == "1")
                    {
                        dt = ObjINVPL.FnGetActualPLSummary(Code.Trim(), FDate, ToDate, "T", (CBIgnoreEffect == "1") ? "Y" : "N");
                        HttpContext.Current.Session["colspan"] = "7";
                    }
                }
                else if (cmbReport == "H")
                {
                    AsonDate = myutil.dtos(AsonDate);
                    dt = ObjINVPL.FnGetNotionalSummary(Code.Trim(), AsonDate, (CBIgnoreEffect == "1") ? "Y" : "N");
                    HttpContext.Current.Session["colspan"] = "4";
                }
                else if (cmbReport == "N")
                {
                    dt = ObjINVPL.FnGetNotionalSummary(Code.Trim(), HttpContext.Current.Session["gstrDBToday"].ToString(), (CBIgnoreEffect == "1") ? "Y" : "N");
                    HttpContext.Current.Session["colspan"] = "8";
                }
                else if (cmbReport == "PL" & CBDetail == "1")
                {
                    if (CBDelivery == "1" & CBJobbing == "1")
                    {
                        dt = ObjINVPL.FnGetActualPLDetail(Code.Trim(), FDate, ToDate, "B", "", (CBIgnoreEffect == "1") ? "Y" : "N");
                        HttpContext.Current.Session["colspan"] = "10";
                    }
                    else if (CBDelivery == "1")
                    {
                        dt = ObjINVPL.FnGetActualPLDetail(Code.Trim(), FDate, ToDate, "D", "", (CBIgnoreEffect == "1") ? "Y" : "N");
                        HttpContext.Current.Session["colspan"] = "8";
                    }
                    else if (CBJobbing == "1")
                    {
                        dt = ObjINVPL.FnGetActualPLDetail(Code.Trim(), FDate, ToDate, "T", "", (CBIgnoreEffect == "1") ? "Y" : "N");
                        HttpContext.Current.Session["colspan"] = "7";
                    }
                }

                if (cmbReport == "PL" & CBDetail == "1")
                {
                    dt.Columns.Add("Tmp_Incstar", typeof(string));
                    for (int I = 0; I < dt.Rows.Count; I++)
                    {
                        if (!Convert.IsDBNull(dt.Rows[I]["Tmp_112ARate"]))
                        {
                            if (dt.Rows[I]["Tmp_LTCG"].ToString().Trim() == "*")
                            {
                                dt.Rows[I]["Tmp_Incstar"] = Convert.ToDecimal(dt.Rows[I]["Tmp_112ARate"]).ToString("0.00") + dt.Rows[I]["Tmp_LTCG"];
                            }
                            else
                            {
                                dt.Rows[I]["Tmp_Incstar"] = Convert.ToDecimal(dt.Rows[I]["Tmp_112ARate"]).ToString("0.00");
                            }
                        }
                        else
                        {
                            dt.Rows[I]["Tmp_Incstar"] = dt.Rows[I]["Tmp_112ARate"];
                        }
                    }
                }

                return dt;
            }
        }

        public DataTable GetInvplGainLossPopupDetail(string strtype, string strclientcd, string strfrdt, string strtodt, string strscripcd, string strscripname, string strclientname, string strIgnoresection)
        {
            DataTable dt = null;
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            INVPLProcess ObjINVPL = new INVPLProcess();
            DataTable objdatatable = new DataTable();

            if (HttpContext.Current.Session["cmbReport"].ToString() == "H" || HttpContext.Current.Session["cmbReport"].ToString() == "N")
            {
                dt = ObjINVPL.FnGetNotionalDetail(strclientcd, strfrdt, strscripcd, strIgnoresection);



            }
            else
            {
                dt = ObjINVPL.FnGetActualPLDetail(strclientcd, strfrdt, strtodt, strtype, strscripcd, strIgnoresection);

            }



            return dt;
        }

        public DataTable GetInvplGainLossPopUp(string strscripcd)
        {
            DataTable dt = null;
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strsql = "";
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {

                curCon.Open();
                strsql = "";
                strsql = "select ss_cd,ss_lname,ss_nsymbol,ss_nseries,  cast(ss_nserate as decimal(15,2)) ss_nserate, case isdate(ss_nseratedt) when 1  then  ltrim(rtrim(convert(char,convert(datetime,ss_nseratedt),103))) else '' end ss_nseratedt, ";
                strsql += " ss_bsymbol,  cast(ss_bserate as decimal(15,2)) ss_bserate ,case isdate(ss_bseratedt) when 1  then ltrim(rtrim(convert(char,convert(datetime,ss_bseratedt),103))) else '' end ss_bseratedt,ss_group ,";
                strsql += " isNull((select Top 1 ltrim(im_ISIN) from ISIN Where im_scripcd = '" + strscripcd + "' and im_active = 'Y'  and im_priority = ";
                strsql += " (select min(im_priority) from Isin Where im_scripcd =  '" + strscripcd + "' and im_active = 'Y' )),'') SS_ISIN from  Securities where ss_cd =  '" + strscripcd + "' ";

                dt = mylib.OpenDataTable(strsql, curCon);
            }


            return dt;
        }

        public DataTable GetINVPLDividendReport(string Code, string FDate, string ToDate)
        {
            INVPLProcess ObjINVPL = new INVPLProcess();
            DataTable objdatatable;

            objdatatable = ObjINVPL.FnGetDividend(Code, FDate, ToDate);


            return objdatatable;


        }

        public DataTable GetInvpltradelistingReport(string Code, string FDate, string ToDate)
        {
            INVPLProcess ObjINVPL = new INVPLProcess();
            DataTable objdatatable;

            objdatatable = ObjINVPL.FnGetTradeListingSummary(Code, FDate, ToDate);


            return objdatatable;

        }

        public DataTable GetInvpltradelistingDetail(string Code, string FDate, string ToDate, string scripcd)
        {
            INVPLProcess ObjINVPL = new INVPLProcess();
            DataTable objdatatable;

            objdatatable = ObjINVPL.FnGetTradeListingDetail(Code, FDate, ToDate, scripcd);


            return objdatatable;

        }

        public string SaveRecordInvpltradelisting(string Code, string strdate, string strstlmnt, string strTRDType, string ScripCode, string dblqty, string dblRate, string value, string bsflag, string OtherChrgs1 = "0", string ServiceTax = "0", string STT = "0", string OtherChrgs2 = "0")
        {
            double dblservicetax = 0, dblstt = 0, dblchrg1 = 0, dblchrg2 = 0;
            INVPLProcess objINVPL = new INVPLProcess();
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            DataTable dt = new DataTable();
            string ControlerMsg = "";
            //int Qty = Convert.ToInt32(dblqty);

            double Qty = Convert.ToDouble(dblqty);
            if (ServiceTax != "")
                dblservicetax = Math.Round(Convert.ToDouble(ServiceTax) / Qty, 4);
            if (STT != "")
                dblstt = Math.Round(Convert.ToDouble(STT) / Qty, 4);
            if (OtherChrgs1 != "")
                dblchrg1 = Math.Round(Convert.ToDouble(OtherChrgs1) / Qty, 4);
            if (OtherChrgs2 != "")
                dblchrg2 = Math.Round(Convert.ToDouble(OtherChrgs2) / Qty, 4);

            var SbInsert = new StringBuilder();
            SbInsert.Append("<Trade>");
            SbInsert.Append("<srno>0</srno>");
            SbInsert.Append("<Trxflag>M</Trxflag>");
            SbInsert.Append("<Date>" + myutil.dtos(strdate) + "</Date>");
            SbInsert.Append("<stlmnt> " + strstlmnt + " </stlmnt>");
            SbInsert.Append("<TRDType>" + strTRDType + "</TRDType>");
            SbInsert.Append("<bsflag> " + bsflag.ToString() + "</bsflag>");
            SbInsert.Append("<qty>  " + Conversion.Val(Qty) + "</qty>");
            SbInsert.Append("<Rate>" + dblRate + "</Rate>");
            SbInsert.Append("<ServiceTax>" + dblservicetax + "</ServiceTax>");
            SbInsert.Append("<STT> " + dblstt + "</STT>");
            SbInsert.Append("<OtherChrgs1>" + dblchrg1 + "</OtherChrgs1>");
            SbInsert.Append("<OtherChrgs2>" + dblchrg2 + "</OtherChrgs2>");
            SbInsert.Append("<mkrid>" + HttpContext.Current.User.Identity.Name + "</mkrid>");
            SbInsert.Append("</Trade>");

            string Msg = objINVPL.FnTradeInsert(Code, ScripCode, SbInsert.ToString());
            DataTable ObjDataTable;

            ObjDataTable = mylib.OpendatatableXml(Msg);
            if (ObjDataTable.Rows.Count > 0)
            {
                if (ObjDataTable.Rows[0]["Status"].ToString().Trim() == "Y")
                {
                    ControlerMsg = "Saved";
                }
                else
                {
                    ControlerMsg = ObjDataTable.Rows[0]["Remarks"].ToString().Replace(Constants.vbCrLf, "-");

                }
            }


            return ControlerMsg;
        }


        public string UpdateRecordInvpltradelisting(string Code, string strdate, string strstlmnt, string strTRDType, string ScripCode, string dblqty, string dblRate, string value, string OtherChrgs1, string ServiceTax, string STT, string OtherChrgs2, string bsflag, string td_srno, string td_TRXFlag)
        {

            INVPLProcess objINVPL = new INVPLProcess();
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            double dblservicetax = 0, dblstt = 0, dblchrg1 = 0, dblchrg2 = 0;
            DataTable dt = new DataTable();
            string ControlerMsg = "";


            double Qty = Convert.ToDouble(dblqty);


            if (ServiceTax != "")
            {
                dblservicetax = Math.Round(Convert.ToDouble(ServiceTax) / Qty, 4);
            }
            if (STT != "")
            {
                dblstt = Math.Round(Convert.ToDouble(STT) / Qty, 4);
            }
            if (OtherChrgs1 != "")
            {
                dblchrg1 = Math.Round(Convert.ToDouble(OtherChrgs1) / Qty, 4);
            }
            if (OtherChrgs2 != "")
            {
                dblchrg2 = Math.Round(Convert.ToDouble(OtherChrgs2) / Qty, 4);
            }
            var SbInsert = new StringBuilder();
            SbInsert.Append("<Trade>");
            SbInsert.Append("<srno>" + td_srno + "</srno>");
            SbInsert.Append("<Trxflag>" + td_TRXFlag + "</Trxflag>");
            SbInsert.Append("<Date>" + myutil.dtos(strdate) + "</Date>");
            SbInsert.Append("<stlmnt> " + strstlmnt + " </stlmnt>");
            SbInsert.Append("<TRDType>" + strTRDType + "</TRDType>");
            SbInsert.Append("<bsflag> " + bsflag.ToString() + "</bsflag>");
            SbInsert.Append("<qty>  " + Conversion.Val(Qty) + "</qty>");
            SbInsert.Append("<Rate>" + dblRate + "</Rate>");
            SbInsert.Append("<ServiceTax>" + dblservicetax + "</ServiceTax>");
            SbInsert.Append("<STT> " + dblstt + "</STT>");
            SbInsert.Append("<OtherChrgs1>" + dblchrg1 + "</OtherChrgs1>");
            SbInsert.Append("<OtherChrgs2>" + dblchrg2 + "</OtherChrgs2>");
            SbInsert.Append("<mkrid>" + HttpContext.Current.User.Identity.Name + "</mkrid>");
            SbInsert.Append("</Trade>");

            string Msg = objINVPL.FnTradeUpdate(Code, ScripCode, SbInsert.ToString());
            DataTable ObjDataTable;

            ObjDataTable = mylib.OpendatatableXml(Msg);
            if (ObjDataTable.Rows.Count > 0)
            {
                if (ObjDataTable.Rows[0]["Status"].ToString().Trim() == "Y")
                {
                    ControlerMsg = "Saved";
                }
                else
                {
                    ControlerMsg = ObjDataTable.Rows[0]["Remarks"].ToString().Replace(Constants.vbCrLf, "-");

                }
            }


            return ControlerMsg;
        }

        public string DeleteRecordInvplRequest(string Code, string td_srno)
        {
            INVPLProcess objINVPL = new INVPLProcess();
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            DataTable dt = new DataTable();
            string ControlerMsg = "";

            ControlerMsg = objINVPL.FnTradeDelete(Code, td_srno);
            DataTable ObjDataTable;

            ObjDataTable = mylib.OpendatatableXml(ControlerMsg);
            if (ObjDataTable.Rows.Count > 0)
            {
                if (ObjDataTable.Rows[0]["Status"].ToString().Trim() == "Y")
                {
                    ControlerMsg = "Deleted";
                }
                else
                {
                    ControlerMsg = ObjDataTable.Rows[0]["Remarks"].ToString().Replace(Constants.vbCrLf, "-");

                }

            }
            return ControlerMsg;
        }
        public DataTable getDematReport(string strDpid, string cmbDate, string cmbClient, string cmbGroupBy, string cmbStatus, string cmbOrderBy, string txtCode, string cmbBranchWise, string dateFrom, string dateTo)
        {

            var strsql = "";
            var orderby = "";
            var selectstr = "";
            var chrdmtype = "N";
            var strquery = "";
            var strTemp = "";
            var strTemp1 = "";
            var strWhere = "";
            var strStlmnt = "";
            var strAccess = "";

            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            DataTable dt = null;

            string strFromDate = myutil.dtos(dateFrom);
            string strToDate = myutil.dtos(dateTo);

            strquery = "select  syscolumns.name,sysobjects.name from syscolumns,sysobjects ";
            strquery = strquery + "where syscolumns.name in ('dm_tdpid','dm_tclientid') and syscolumns.id=sysobjects.id ";
            strquery = strquery + " and sysobjects.name='demat'";

            //DataSet dsnew;
            //dsnew = mylib.OpenDataSet(strquery);
            //if (dsnew.Tables[0].Rows.Count > 1)
            //{
            //    chrdmtype = "Y";
            //}
            //else
            //{
            //    chrdmtype = "N";
            //}
            //dsnew = null;

            chrdmtype = "Y";

            if (cmbGroupBy == "GC")
            {
                strTemp = strTemp + " and dm_type in('BC','')";
            }
            else if (cmbGroupBy == "RC")
            {
                strTemp = strTemp + " and dm_type in('CB','')";
            }
            else if (cmbGroupBy == "IS")
            {
                strTemp = strTemp + " and dm_type in('CB','') and dm_locked='Y'";
            }

            if (cmbStatus == "PE")
            {
                strTemp1 = strTemp1 + "'N',";
            }
            else if (cmbStatus == "TD")
            {
                strTemp1 = strTemp1 + "'Y',";
            }
            else if (cmbStatus == "SC")
            {
                strTemp1 = strTemp1 + "'S',";
            }

            if (strTemp1 == "")
            {

            }
            else
            {
                strTemp1 = " and dm_transfered in (" + strTemp1 + " '')";
            }

            if (strFromDate != "" && strToDate != "")
            {
                if (cmbDate == "TD")
                {
                    strWhere = strWhere + " and se_stdt between '" + strFromDate + "'" + "And" + "'" + strToDate + "'";
                }
                else if (cmbDate == "EX")
                {
                    strWhere = strWhere + " and dm_execdt between '" + strFromDate + "'" + "And" + "'" + strToDate + "'";
                }
            }
            else if (strFromDate != "" && strToDate == "")
            {
                if (cmbDate == "TD")
                {
                    strWhere = strWhere + " and se_stdt >='" + strFromDate + "'";
                }
                else if (cmbDate == "EX")
                {
                    strWhere = strWhere + " and se_stdt >='" + strFromDate + "'";
                }
            }
            else if (strFromDate == "" && strToDate != "")
            {
                if (cmbDate == "TD")
                {
                    strWhere = strWhere + " and se_stdt <='" + strFromDate + "'";
                }
                else if (cmbDate == "EX")
                {
                    strWhere = strWhere + " and dm_execdt <= '" + strFromDate + "'";
                }
            }

            strStlmnt = " dm_stlmnt ";
            strAccess = "";

            var strDematact = "";

            if (txtCode != "")
            {
                if (strDematact.Trim() == "")
                {
                    strDematact = " and (";
                }
                else
                {
                    strDematact = strDematact + " and ";
                }

                strDematact = strDematact + " dm_clientcd = '" + txtCode.Trim() + "'";
            }

            if (strDematact != "")
            {
                strDematact = strDematact + ")";
            }

            if (cmbOrderBy == "CL")
            {
                orderby = "BranchCode,cm_cd,dm_clientcd,ss_name,dm_stlmnt,dm_tstlmnt";
            }
            else if (cmbOrderBy == "CN")
            {
                orderby = "BranchCode,cm_cd, cm_name,ss_name,dm_stlmnt,dm_tstlmnt";
            }
            else if (cmbOrderBy == "SC")
            {
                orderby = " BranchCode,cm_cd, dm_scripcd,ss_name,cm_name,dm_stlmnt,dm_tstlmnt ";
            }
            else if (cmbOrderBy == "SN")
            {
                orderby = " BranchCode,cm_cd , ss_name,cm_name,dm_stlmnt,dm_tstlmnt ";
            }
            else if (cmbOrderBy == "ED")
            {
                orderby = "BranchCode,cm_cd, dm_execdt,dm_stlmnt,cm_name,ss_name,dm_tstlmnt ";
            }
            else
            {
                orderby = " BranchCode,cm_cd ,dm_stlmnt,cm_name,ss_name,dm_tstlmnt ";
            }

            switch (cmbBranchWise.Trim())
            {
                case "CL":
                    selectstr = "select '' as [BranchCode],";
                    break;
                case "BR":
                    selectstr = "select rtrim(bm_branchname) + '['+ ltrim(rtrim(cm_brboffcode))+']' as [BranchCode],";
                    break;
                case "GR":
                    selectstr = "select rtrim(gr_desc) + '['+ ltrim(rtrim(cm_groupcd)) + ']' as [BranchCode],";
                    break;
                case "FM":
                    selectstr = "select rtrim(fm_desc) + '[' + ltrim(rtrim(cm_familycd)) + ']' as [BranchCode],";
                    break;
                case "SE":
                    selectstr = "select rtrim(dm_Stlmnt) as [BranchCode] ,";
                    break;
            }

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                try
                {

                }
                catch (Exception)
                {

                }
                finally

                {
                    strsql = selectstr + "cm_cd,cm_name,ss_cd,ss_name,dm_isin,dm_stlmnt,dm_qty,dm_transfered,dm_locked,cm_poa,case when dm_type='BC' then dm_tdpid else dm_dpid end as [DP ID], case when dm_type='BC' then dm_tclientid else dm_clientid end as [DP A/C ID],convert(char,convert(datetime,dm_execdt),103) as [Exc Dt],";
                    strsql += "case when dm_locked in('O','Y','S') then dm_tstlmnt else '' end Interstlmnt,";
                    strsql += "case when dm_locked='N' then '' when dm_locked='O' then 'Pool To Pool' when dm_locked='Y' then 'Inter stlmnt'";
                    strsql += " when dm_locked = 'X' then 'Transferred to other A/c' when dm_locked = 'B' then 'Received from beneficiery'";
                    strsql += " when dm_locked = 'A' then 'Interlocked' when dm_locked = 'S' then 'Transferred to other stlmnt' when dm_locked = 'L' then '' end Description";
                    strsql += " from Demat a,Client_master,Branch_Master,Securities,settlements,group_master,family_master";
                    strsql += " where se_stdt between '" + strFromDate + "'" + "And" + "'" + strToDate + "'";
                    strsql += " and dm_type not in ('PD','PC') and dm_clientcd not in ('PLD999')";
                    strsql += " and cm_groupcd=gr_cd and cm_familycd=fm_cd ";
                    strsql += "and dm_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and left(dm_stlmnt,1)+'c' in ('" + strDpid.Replace(",", "','") + "') ";
                    strsql += " and dm_clientcd=cm_cd and cm_brboffcode=bm_branchcd and dm_scripcd=ss_cd and dm_Stlmnt = se_stlmnt ";
                    strsql += " " + strWhere + "" + strTemp + "" + strTemp1 + "" + strDematact + "  and exists " + myutil.LoginAccess("dm_clientcd");
                    strsql += "order by " + orderby;
                    dt = mylib.OpenDataTable(strsql, curCon);
                }
            }
            foreach (DataRow row in dt.Rows)
            {
                if (row["dm_transfered"].ToString() == "Y")
                {
                    row.SetField("dm_transfered", "In Transit");
                }
                else if (row["dm_transfered"].ToString() == "S")
                {
                    row.SetField("dm_transfered", "Success");
                }
                else
                {
                    row.SetField("dm_transfered", "Pending");
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                if (row["Exc dt"].ToString().Trim() == "01/01/1900")
                {
                    row.SetField("Exc dt", "--/--/----");
                }
            }

            return dt;
        }

        public DataTable getDeliveryStatementReport(string Code, string strDpid, string cmbSelect, string cmbReport, string strdate, string cmbBS, string cmbNoDelivery)
        {
            string strMode = "P";
            string strClientWhere = "";
            string strsql = "";
            string strField = "";
            string StrMsg = "";
            string StrStlmntNew = "";
            Boolean blnInterOP = false;
            string[] arrStlmnt = new string[3];
            string IsTplusCommex = (string)HttpContext.Current.Session["IsTplusCommex"];
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
            DataTable dt = new DataTable();
            strdate = myutil.dtos(strdate);
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {

                curCon.Open();

                if (Code != "")
                {
                    if (cmbSelect == "CL")
                        strClientWhere += " and td_clientcd = '" + Code.Trim() + "' ";
                    else if (cmbSelect == "FM")
                        strClientWhere += " and cm_familycd = '" + Code.Trim() + "'"; // and cm_familycd= fm_cd "
                    else if (cmbSelect == "GR")
                        strClientWhere += " and cm_groupcd = '" + Code.Trim() + "' "; // and cm_groupcd=gr_cd "
                    else if (cmbSelect == "SB")
                        strClientWhere += " and cm_subbroker = '" + Code.Trim() + "' "; // and cm_subbroker=rm_Cd "                       
                    else if (cmbSelect == "ALL")
                        strClientWhere += " ";
                }
                StrMsg = mylib.fnCheckInterOperability(strdate, "C", curCon);

                if (StrMsg.ToUpper().Trim() == "TRUE")
                {
                    blnInterOP = true;
                }
                if (blnInterOP)
                {
                    arrStlmnt = Strings.Split(mylib.fnGetInterOpStlmntsForMultipleExch(strDpid, strdate), ",");

                    if (Convert.ToInt16(arrStlmnt.Length) == 1)
                    {
                        // ReDim Preserve arrStlmnt(1)

                        arrStlmnt[1] = (mylib.fnGetInterOpStlmntsForMultipleExch(strDpid, strdate));
                    }
                    StrStlmntNew = "'" + arrStlmnt[0] + "'";
                }
                else
                {
                    StrStlmntNew = "";
                    arrStlmnt = Strings.Split(",", ",");
                }

                switch (cmbReport)
                {
                    case "1":
                        {
                            strsql = "select ltrim(rtrim(cm_name))+' '+case when (cm_poa) = 'Y'then '(POA)' else '(def act)' end [GroupByValue],";
                            break;
                        }

                    case "2":
                        {
                            strsql = " select Rtrim(ss_name) + ' ['  + td_scripcd + ']'  as [GroupByValue] ,";
                            break;
                        }

                    case "3":
                        {
                            strsql = "select rtrim(gr_desc) + '['+ ltrim(rtrim(cm_groupcd)) + ']'as [GroupByValue],";
                            break;
                        }
                    case "4":
                        {
                            strsql = "select Rtrim(bm_branchname) + ' ['  + cm_brboffcode + ']' as [GroupByValue],";
                            break;
                        }

                }

                if (strMode == "E")
                {
                    strField = "cm_email ,";
                }
                if (blnInterOP)
                {
                    strsql = strsql + " td_scripcd,td_clientcd,sy_maptype,";
                    strsql = strsql + " case sy_maptype When 'N' Then '" + arrStlmnt[0] + "' When 'C' Then '" + arrStlmnt[1] + "' else td_stlmnt end td_stlmnt, ";
                    strsql = strsql + strField + " ss_name,BQty,SQty,cm_groupcd,gr_desc, cm_name,";
                    strsql = strsql + " ltrim(rtrim(cm_name))+' '+case when (cm_poa) = 'Y' then '(POA)' else '(def act)' end ClCode,";
                    strsql = strsql + " Demat,ltrim(rtrim(cm_name))+'['+ltrim(rtrim(td_clientcd))+']'+'/'+ ltrim(rtrim(Demat))+case when (cm_poa) = 'Y' then ' (POA)' else ' (def act)' end CLName ,";
                    strsql = strsql + " cm_brboffcode, bm_branchname,sr_nodelyn,im_ISIN from ( ";

                    strsql = strsql + " select td_scripcd,td_clientcd,sy_maptype,case When sy_maptype in ('N') Then sy_maptype else td_stlmnt end td_stlmnt, ss_name,";
                    strsql = strsql + " case when Sum(nQty) > 0 then Sum(nQty) else 0 end as BQty,";
                    strsql = strsql + " case when Sum(nqty) > 0 then 0 else Sum((-1)*NQty) End SQty,cm_name,cm_poa,cm_groupcd,gr_desc,Demat,cm_brboffcode, bm_branchname,sr_nodelyn,im_ISIN";
                    strsql = strsql + " From vwcDeliveryListWebdt, Settlements, Settlement_type ";
                    strsql = strsql + " where td_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                    strsql = strsql + " and left(td_stlmnt,1) + 'C' in ('" + strDpid.Replace(",", "','") + "') and td_dt= '" + strdate + "'";
                    strsql = strsql + " and td_stlmnt = se_stlmnt and se_exchange = sy_exchange and se_type = sy_type ";
                    //  strsql = strsql + Replace(objApplicationUser.fnLoginFilter(), "cm_cd", "td_clientcd");
                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"].ToString().Replace("cm_cd", "td_clientcd");
                    strsql = strsql + strClientWhere;


                    if (strMode == "E")
                    {
                        strsql = strsql + " and cm_email <> ''";
                    }
                    if (cmbBS == "Buy")
                    {
                        strsql = strsql + " and nQty > 0 ";

                    }
                    else if (cmbBS == "Sell")
                    {
                        strsql = strsql + " and nQty < 0 ";
                    }

                    strsql = strsql + " Group By td_scripcd,td_clientcd,ss_name,sy_maptype,";
                    strsql = strsql + " case When sy_maptype in ('N') Then sy_maptype else td_stlmnt end,cm_name,cm_poa,cm_groupcd,gr_desc,Demat, cm_brboffcode, bm_branchname,sr_nodelyn,im_ISIN";
                    strsql = strsql + " ) a";
                    strsql = strsql + " Where (BQty+SQty) > 0";
                }
                else
                {
                    strsql = strsql + " td_scripcd, td_clientcd,cm_groupcd,gr_desc, cm_name,td_stlmnt , " + strField + " ss_name,";
                    strsql = strsql + "case when nQty > 0 then nQty else 0 end as BQty, case when nqty > 0 then 0 else (-1)*NQty End SQty,";
                    strsql = strsql + "sr_nodelyn, im_isin, sr_makingrate, cm_brboffcode, bm_branchname, cm_poa, ";
                    strsql = strsql + "ltrim(rtrim(cm_name))+' '+case when (cm_poa) = 'Y' then '(POA)' else '(def act)' end ClCode, Demat,";
                    strsql = strsql + "ltrim(rtrim(cm_name))+'['+ltrim(rtrim(td_clientcd))+']'+'/'+ ltrim(rtrim(Demat))+case when (cm_poa) = 'Y' then ' (POA)' else ' (def act)' end CLName ";
                    strsql = strsql + " From vwcDeliveryListWebdt";
                    strsql = strsql + " where td_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and left(td_stlmnt,1) + 'C' in ('" + strDpid.Replace(",", "','") + "') and td_dt= '" + strdate + "'";
                    strsql = strsql + HttpContext.Current.Session["LoginAccessOld"].ToString().Replace("cm_cd", "td_clientcd"); // Session("LoginAccess") changed on 02/06/2009
                    strsql = strsql + strClientWhere;
                    if (cmbNoDelivery == "1")
                    {
                        strsql = strsql + strClientWhere + " and sr_nodelyn='N'";
                    }
                    if (strMode == "E")
                    {
                        strsql = strsql + " and cm_email <> ''";
                    }
                    if (cmbBS == "Buy")
                    {
                        strsql = strsql + " and nQty > 0 ";
                    }
                    else if (cmbBS == "Sell")
                    {
                        strsql = strsql + " and nQty < 0 ";
                    }
                }

                if (cmbReport == "1")
                {
                    strsql = strsql + "  order by " + (blnInterOP ? " case sy_maptype When 'N' Then '" + arrStlmnt[0] + "' When 'C' Then '" + arrStlmnt[1] + "' else td_stlmnt end," : "") + " td_clientcd,ss_name ";
                }
                else if (cmbReport == "2")
                {
                    strsql = strsql + " order by " + (blnInterOP ? " case sy_maptype When 'N' Then '" + arrStlmnt[0] + "' When 'C' Then '" + arrStlmnt[1] + "' else td_stlmnt end," : "") + " td_scripcd,ss_name ";
                }
                else if (cmbReport == "3")
                {
                    strsql = strsql + " order by " + (blnInterOP ? " case sy_maptype When 'N' Then '" + arrStlmnt[0] + "' When 'C' Then '" + arrStlmnt[1] + "' else td_stlmnt end," : "") + "cm_groupcd,td_clientcd,ss_name";
                }
                else if (cmbReport == "4")
                {
                    strsql = strsql + " order by " + (blnInterOP ? " case sy_maptype When 'N' Then '" + arrStlmnt[0] + "' When 'C' Then '" + arrStlmnt[1] + "' else td_stlmnt end," : "") + "cm_brboffcode,td_clientcd,ss_name ";
                }

                dt = mylib.OpenDataTable(strsql, curCon);
            }
            return dt;
        }

        public DataTable getInvesterBasedFReport(string Code, string Cname, string cmbSelect, string FromDt, string ToDate, string cmbExchSeg, string openopt, string ChkAvgRate, string ChkBuySellValue, string chkconsider)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strCondition = "";
            string strClient = "";
            string strClientWhere = "";
            string strParm = "";
            DataTable dt = new DataTable();
            string strFirstDate = "";
            string strLastDate = "";
            string strsql = "";
            int strBillstDt;
            int strBillenDt;
            int i = 0;

            FromDt = myutil.dtos(FromDt);
            ToDate = myutil.dtos(ToDate);
            // cmbExchSeg = cmbExchSeg == "F" ? "F" : "K";

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {

                curCon.Open();

                if (Code != "")
                {
                    strCondition = " and cm_cd = '" + Code.Trim() + "' ";
                    strClientWhere = strCondition;
                }

                if (Code != "")
                {
                    if (cmbSelect == "CL")
                        strClientWhere += " and cm_cd = '" + Code.Trim() + "' ";
                    else if (cmbSelect == "FM")
                        strClientWhere += " and cm_familycd = '" + Code.Trim() + "'"; // and cm_familycd= fm_cd "
                    else if (cmbSelect == "GR")
                        strClientWhere += " and cm_groupcd = '" + Code.Trim() + "' "; // and cm_groupcd=gr_cd "
                    else if (cmbSelect == "SB")
                        strClientWhere += " and cm_subbroker = '" + Code.Trim() + "' "; // and cm_subbroker=rm_Cd "                       
                    else if (cmbSelect == "ALL")
                        strClientWhere += " ";
                }

                mylib.ExecSQL("drop table #finvdates", curCon);

                strsql = "CREATE TABLE [#finvdates] (";
                strsql += "[bd_dt] [char] (8) NOT NULL ";
                strsql += ")";

                mylib.ExecSQL(strsql, curCon);

                strBillstDt = Convert.ToInt32(FromDt);
                strBillenDt = Convert.ToInt32(ToDate);



                while (strBillstDt <= strBillenDt)
                {

                    strsql = "select count(*) cnt from Fholiday_master with (nolock) where hm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'" + myutil.newline();
                    strsql += " and hm_exchange = '" + Strings.Mid(cmbExchSeg, 2, 1) + "'  and hm_segment ='" + Strings.Right(cmbExchSeg, 1) + "'";
                    strsql += " and hm_dt = '" + strBillstDt + "'";
                    dt = mylib.OpenDataTable(strsql, curCon);

                    if (Convert.ToInt32(dt.Rows[0]["cnt"]) <= 0)
                    {
                        strsql = "insert into #finvdates values('" + strBillstDt + "')";

                        mylib.ExecSQL(strsql, curCon);
                    }
                    dt.Dispose();

                    strBillstDt = Convert.ToInt32(myutil.dtos((myutil.AddDayDT(strBillstDt.ToString(), 1)).ToString("dd/MM/yyyy")));
                }

                mylib.ExecSQL("drop table #tmpFinvcharges", curCon);


                strsql = "CREATE TABLE [#tmpFinvcharges] (";
                strsql += "[bc_dt] [char] (8) NOT NULL,";
                strsql += "[bc_clientcd] [char] (8) NOT NULL,";
                strsql += "[bc_desc] [char] (40) NOT NULL,";
                strsql += "[bc_amount] [money] NOT NULL,";
                strsql += "[bc_billno] [numeric] NOT NULL";
                strsql += ")";

                mylib.ExecSQL(strsql, curCon);

                mylib.ExecSQL("drop table #tmpfinvestorrep", curCon);

                strsql = "Create table #tmpfinvestorrep (";
                strsql += " fi_dt char(8) not null,";
                strsql += " fi_clientcd char(8) not null,";
                strsql += " fi_exchange char(1) not null,";
                strsql += " fi_seriesid numeric not null,";
                strsql += " fi_bqty numeric not null,";
                strsql += " fi_bvalue money not null,";
                strsql += " fi_sqty numeric not null,";
                strsql += " fi_svalue money not null,";
                strsql += " fi_netqty numeric not null,";
                strsql += " fi_netvalue money not null,";
                strsql += " fi_rate money not null,";
                strsql += " fi_closeprice money not null,";
                strsql += " fi_mtm money not null,";
                strsql += " fi_listorder numeric not null,";
                strsql += " fi_controlflag numeric not null,";
                strsql += " fi_prodtype char(2) not null,";
                strsql += " fi_type char(1) not null,";
                strsql += " fi_balfield char(1) not null,";
                strsql += " fi_multiplier money,";
                strsql += " fi_segment char(1) not null)";

                mylib.ExecSQL(strsql, curCon);

                strsql = "select isnull(min(bd_dt),'" + FromDt + "'),isNull(max(bd_dt),'') from #finvdates";
                DataTable dtmin = mylib.OpenDataTable(strsql, curCon);

                strFirstDate = Convert.ToString(dtmin.Rows[0][0]);
                strLastDate = Convert.ToString(dtmin.Rows[0][1]);
                dtmin.Dispose();


                strsql = "Insert into #tmpfinvestorrep ";
                strsql += " Select '" + strFirstDate + "',td_clientcd,td_exchange,";
                strsql += " td_seriesid,case sign(sum(td_bqty - td_sqty)) when 1 then abs(sum(td_bqty - td_sqty)) else 0 end  td_bqty,";
                strsql += " 0,";
                strsql += " case sign(sum(td_bqty - td_sqty)) when 1 then 0 else abs(sum(td_bqty - td_sqty)) end td_sqty,0,";
                strsql += " 0,0,0,0 td_closeprice,0,";
                strsql += " case sm_prodtype when 'IF' then 1 when 'CF' then 1 when 'TF' then 2 when 'RF' then 2 when 'EF' then 2 when 'IO' then 3 else 4 end td_sortlist,";
                strsql += " 1 td_controlflag,sm_prodtype,'N','O',sm_multiplier,td_segment";
                strsql += " From Trades with(index(idx_trades_dt_clientcd)),Series_master,Client_master";
                strsql += " Where td_clientcd = cm_cd and td_exchange = sm_exchange and td_segment = sm_segment And td_seriesid = sm_seriesid";
                strsql += " and sm_expirydt >= '" + strFirstDate + "' and  td_dt < '" + strFirstDate + "'";
                strsql += " and td_companycode + td_exchange + td_segment = '" + cmbExchSeg + "'";
                //strsql += " and td_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and  td_exchange = '" + Strings.Mid(cmbExchSeg, 2, 1) + "' and td_segment = '" + Strings.Right(cmbExchSeg, 1) + "'";
                strsql += " and sm_prodtype in('IF','EF','CF','RF','TF')";
                strsql += strCondition;

                if (strClientWhere == "")
                {
                    strsql += " and cm_type <> 'C'";
                }

                strsql += " group by td_exchange,td_clientcd,td_seriesid,sm_prodtype,sm_multiplier,td_segment";
                strsql += " having sum(td_bqty - td_sqty) <> 0";

                mylib.ExecSQL(strsql, curCon);


                if (chkconsider == "1")
                {
                    strsql = "insert into #tmpfinvestorrep ";
                    strsql += " select '" + strFirstDate + "',td_clientcd,td_exchange,";
                    strsql += " td_seriesid, sum(case sale when 0 then buy else 0 end) td_bqty,";
                    strsql += " 0,";
                    strsql += " sum(case sale when 0 then 0 else sale end) td_sqty,0,";
                    strsql += " 0,0," + (openopt == "Donotvaluate" ? "0" : " sum((buy-sale)*td_rate) / sum((buy-sale))") + ",0 td_closeprice,0,";
                    strsql += " case sm_prodtype when 'IF' then 1 when 'CF' then 1 when 'TF' then 2 when 'RF' then 2 when 'EF' then 2 when 'IO' then 3 else 4 end td_sortlist,";
                    strsql += " 1 td_controlflag,sm_prodtype,'N','O',sm_multiplier,td_segment";
                    strsql += " From vwFoutstandingposweb  ";
                    strsql += " Where sm_expirydt >= '" + FromDt + "' and  td_dt < '" + FromDt + "'";

                    strsql += " and  td_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and  td_exchange = '" + Strings.Mid(cmbExchSeg, 2, 1) + "' and  td_segment = '" + Strings.Right(cmbExchSeg, 1) + "' and sm_prodtype in('IO','EO','CO')";
                    strsql += strCondition;
                    strsql += " and cm_type <> 'C'";
                    strsql += " group by td_exchange,td_clientcd,td_seriesid,sm_prodtype,sm_multiplier,td_segment";
                    strsql += " having sum(sale - buy) <> 0 ";
                    mylib.ExecSQL(strsql, curCon);

                }

                strsql = "insert into #tmpfinvestorrep ";
                strsql += " select td_dt,td_clientcd,td_exchange,";
                strsql += " td_seriesid,td_bqty,0,td_sqty,0,0,0,";
                strsql += " td_rate,0.0000 td_closeprice,0 mtm,";
                strsql += " case sm_prodtype when 'IF' then 1 when 'CF' then 1 when 'TF' then 2 when 'RF' then 2 when 'EF' then 2 when 'IO' then 3 else 4 end td_sortlist,";
                strsql += " 2,sm_prodtype,'N','Y',sm_multiplier,td_segment";
                strsql += " From Trades with(index(idx_trades_dt_clientcd)) , Series_master,Client_master";
                strsql += " Where td_clientcd = cm_cd and td_exchange = sm_exchange and td_segment = sm_segment and td_seriesid = sm_seriesid";
                strsql += " and td_companycode + td_exchange + td_segment = '" + cmbExchSeg + "'";
                // strsql += " and  td_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and  td_exchange = '" + Strings.Mid(cmbExchSeg, 2, 1) + "' and td_segment = '" + Strings.Right(cmbExchSeg, 1) + "'";
                strsql += " and sm_expirydt >= '" + strFirstDate + "' and td_dt between '" + strFirstDate + "' and '" + strLastDate + "'";
                strsql += strCondition;
                mylib.ExecSQL(strsql, curCon);

                strSQL = "insert into #tmpfinvestorrep ";
                strSQL += " select ex_dt,ex_clientcd,ex_exchange,";
                strSQL += " ex_seriesid,ex_eqty,0,ex_aqty,0,0,0,";
                strSQL += " ex_diffbrokrate,ex_settlerate,0,";
                strSQL += " case sm_prodtype when 'IF' then 1 when 'CF' then 1 when 'TF' then 2  when 'RF' then 2 when 'EF' then 2 when 'IO' then 3 else 4 end + 5 td_sortlist,";
                strSQL += " case ex_eaflag when 'E' then 3 else 4 end td_controlflag,sm_prodtype,'N','Y',sm_multiplier,ex_segment";
                strSQL += " From Exercise, Series_master,Client_master";
                strSQL += " Where ex_clientcd = cm_cd and ex_exchange = sm_exchange and ex_segment = sm_segment And ex_seriesid = sm_seriesid";
                strSQL += " and ex_companycode + ex_exchange + ex_segment = '" + cmbExchSeg + "'";
                //strSQL += " and  td_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and  td_exchange = '" + Strings.Mid(cmbExchSeg, 2, 1) + "' and td_segment = '" + Strings.Right(cmbExchSeg, 1) + "'";
                strSQL += " and sm_expirydt >= '" + strFirstDate + "' and  ex_dt between '" + strFirstDate + "' and '" + strLastDate + "'";
                strSQL += strCondition;

                mylib.ExecSQL(strSQL, curCon);


                if (openopt != "Donotvaluate")
                {
                    //Reverse Option Outstanding

                    strsql = "insert into #tmpfinvestorrep ";
                    strsql += " select '" + strLastDate + "',fi_clientcd,fi_exchange,";
                    strsql += " fi_seriesid,case sign(sum(case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) when -1 then abs(sum(case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) else 0 end td_bqty,";
                    strsql += " 0,";
                    strsql += " case sign(sum(case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) when 1 then abs(sum(case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) else 0 end td_sqty,0,";
                    if (openopt == "AveragePrice")
                    {
                        strsql += " 0,0,";
                        strsql += " round(abs(sum((case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)*fi_rate))/abs(sum(case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)),2),";
                        strsql += " round(abs(sum((case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)*fi_rate))/abs(sum(case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)),2),0,";

                    }
                    else
                    {
                        strsql += " 0,0,0,0 td_closeprice,0,";
                    }
                    strsql += " case fi_prodtype when 'IF' then 1 when 'CF' then 1 when 'TF' then 2 when 'RF' then 2 when 'EF' then 2 when 'IO' then 3 else 4 end + 6 td_sortlist,";
                    strsql += " 5 td_controlflag,fi_prodtype,'R','N',sm_multiplier,fi_segment";
                    strsql += " From #tmpfinvestorrep ,Series_master";
                    strsql += " Where fi_exchange = sm_exchange and fi_segment = sm_segment and sm_seriesid = fi_seriesid and fi_prodtype in('IO','EO','CO') ";
                    strsql += " and sm_expirydt > '" + ToDate + "'";
                    strsql += " group by fi_exchange,fi_clientcd,fi_seriesid,fi_prodtype,sm_multiplier,fi_segment";
                    strsql += " having sum(case fi_controlflag when 1 then fi_bqty - fi_sqty when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end) <> 0";
                    mylib.ExecSQL(strsql, curCon);

                }

                //Reverse Future Outstanding      

                strsql = "insert into #tmpfinvestorrep ";
                strsql += " select '" + strLastDate + "',fi_clientcd,fi_exchange,";
                strsql += " fi_seriesid,case sign(sum(case fi_controlflag when 1 then case fi_dt when '" + strFirstDate + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) when -1 then abs(sum(case fi_controlflag when 1 then case fi_dt when '" + strFirstDate + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) else 0 end td_bqty,";
                strsql += " 0,";
                strsql += " case sign(sum(case fi_controlflag when 1 then case fi_dt when '" + strFirstDate + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) when 1 then abs(sum(case fi_controlflag when 1 then case fi_dt when '" + strFirstDate + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) else 0 end td_sqty,0,";
                strsql += " 0,0,0,0 td_closeprice,0,";
                strsql += " case fi_prodtype when 'IF' then 1 when 'CF' then 1 when 'TF' then 2 when 'RF' then 2 when 'EF' then 2 when 'IO' then 3 else 4 end + 6 td_sortlist,";
                strsql += " 5 td_controlflag,fi_prodtype,'R','N',sm_multiplier,fi_segment";
                strsql += " From #tmpfinvestorrep ,Series_master";
                strsql += " Where fi_exchange = sm_exchange and fi_segment = sm_segment and sm_seriesid = fi_seriesid and fi_prodtype in('IF','EF','CF','RF','TF') ";
                strsql += " and sm_expirydt <= '" + ToDate + "'";
                strsql += " group by fi_exchange,fi_clientcd,fi_seriesid,fi_prodtype,sm_multiplier,fi_segment";
                strsql += " having sum(case fi_controlflag when 1 then case fi_dt when '" + strFirstDate + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end) <> 0";
                mylib.ExecSQL(strsql, curCon);

                //Update Last Market Price for Options

                if (openopt == "AveragePrice")
                {
                    strsql = "update #tmpfinvestorrep  set fi_rate = fi_rate,fi_closeprice = case fi_type when 'R' then fi_closeprice else ms_lastprice end from #tmpfinvestorrep ,Market_summary";
                }
                else
                {
                    strsql = "update #tmpfinvestorrep  set fi_rate = case fi_type when 'R' then ms_lastprice else fi_rate end,fi_closeprice = ms_lastprice from #tmpfinvestorrep ,Market_summary";
                }

                strsql += " where ms_seriesid = fi_seriesid ";
                strsql += " and ms_exchange = fi_exchange and ms_segment = fi_segment ";
                strsql += " and ms_dt = (select max(ms_dt) from Market_summary where ms_exchange = fi_exchange and ms_segment = fi_segment ";
                strsql += " and ms_seriesid = fi_seriesid and ms_lastprice <> 0 and ms_dt <= '" + ToDate + "' )";
                strsql += " and fi_prodtype in('IO','EO','CO')";
                mylib.ExecSQL(strsql, curCon);

                if (openopt == "UnderlyingClosePrice")
                {
                    //Update Cash Market Price for Options

                    strsql = "update #tmpfinvestorrep  set fi_rate = sm_strikeprice,fi_closeprice = mk_closerate";
                    strsql += " from #tmpfinvestorrep ,Product_master,Series_master,Market_rates,Securities";
                    strsql += " where fi_seriesid = sm_seriesid ";
                    strsql += " and sm_prodtype = pm_type and sm_productcd = pm_cd";
                    strsql += " and sm_exchange = fi_exchange and sm_segment = fi_segment";
                    strsql += " and sm_exchange = pm_exchange and pm_segment = sm_segment";
                    strsql += " and mk_exchange = sm_exchange ";
                    strsql += " and mk_scripcd = ss_cd";
                    if (Strings.Mid(cmbExchSeg, 2, 1) == "N")
                    {
                        strsql += " and ss_nsymbol = pm_assetcd and ss_nsymbol <> ''";
                        strsql += " and ss_nseries <> ''";
                    }
                    else
                    {
                        strsql += " and ss_cd = pm_scripcd";
                    }
                    strSQL += " and mk_dt =(select max(mk_dt) from Market_rates";
                    strSQL += " where mk_scripcd = ss_cd and mk_exchange = sm_exchange";
                    strSQL += " and mk_dt <= '" + ToDate + "')";
                    strSQL += " and fi_type = 'R'";
                    mylib.ExecSQL(strsql, curCon);
                }

                if (openopt == "ClosingPremium")
                {

                    strsql = "update #tmpfinvestorrep  set fi_rate = ms_lastprice from Market_summary";
                    strsql += " where ms_seriesid = fi_seriesid ";
                    strsql += " and ms_exchange = fi_exchange and ms_segment = fi_segment ";
                    strsql += " and ms_dt = (select max(ms_dt) from Market_summary where ms_exchange = fi_exchange and ms_segment = fi_segment ";
                    strsql += " and ms_seriesid = fi_seriesid and ms_prcloseprice <> 0 and ms_dt < '" + FromDt + "' )";
                    strsql += " and fi_prodtype in('IO','EO','CO') and fi_controlflag = 1 ";
                    mylib.ExecSQL(strsql, curCon);
                }
                if (openopt == "UnderlyingClosePrice")
                {
                    //Update Cash Market Price for Options
                    strsql = "update #tmpfinvestorrep  set fi_rate = mk_closerate ";
                    strsql += " from Product_master,Series_master,Market_rates,Securities";
                    strsql += " where fi_seriesid = sm_seriesid ";
                    strsql += " and sm_prodtype = pm_type and sm_productcd = pm_cd";
                    strsql += " and sm_exchange = fi_exchange and sm_segment = fi_segment";
                    strsql += " and sm_exchange = pm_exchange and pm_segment = sm_segment";
                    strsql += " and mk_exchange = sm_exchange ";
                    strsql += " and mk_scripcd = ss_cd";

                    if (Strings.Mid(cmbExchSeg, 2, 1) == "N")
                    {
                        strsql += " and ss_nsymbol = pm_assetcd and ss_nsymbol <> ''";
                        strsql += " and ss_nseries <> ''";
                    }
                    else
                    {

                        strsql += " and ss_cd = pm_scripcd";
                    }

                    strsql += " and mk_dt =(select max(mk_dt) from Market_rates";
                    strsql += " where mk_scripcd = ss_cd and mk_exchange = sm_exchange";
                    strsql += " and mk_dt < '" + ToDate + "')";
                    strsql += " and fi_controlflag = 1 ";
                    mylib.ExecSQL(strsql, curCon);
                }
                //Update Previous close and today's close prices
                strsql = "update #tmpfinvestorrep  set fi_closeprice =  isNull((select ms_lastprice From Market_summary ";
                strsql += " Where ms_exchange = '" + Strings.Mid(cmbExchSeg, 2, 1) + "' and  ms_segment = '" + Strings.Right(cmbExchSeg, 1) + "'  and ms_seriesid = fi_seriesid ";
                strsql += " and ms_dt = (select Max(ms_dt) From Market_Summary ";
                strsql += " Where ms_exchange ='" + Strings.Mid(cmbExchSeg, 2, 1) + "' and ms_segment ='" + Strings.Right(cmbExchSeg, 1) + "'  and ms_seriesid = fi_seriesid ";
                strsql += " and ms_dt <='" + ToDate + "')),0) ";
                strsql += " where fi_controlflag in('1','2') and fi_prodtype in('IF','EF','CF','RF','TF') ";
                mylib.ExecSQL(strsql, curCon);

                //  'Update close Price If Expiry Trade is Not Generated.
                strsql = "update #tmpfinvestorrep  set fi_rate =  ms_lastprice  , fi_closeprice = ms_lastprice ";
                strsql += " from #tmpfinvestorrep ,Market_summary , Series_master ";
                strsql += " where sm_Exchange ='" + Strings.Mid(cmbExchSeg, 2, 1) + "'and  sm_segment = '" + Strings.Right(cmbExchSeg, 1) + "' and sm_seriesid = fi_seriesid ";
                strsql += " and sm_exchange = ms_exchange and sm_segment = ms_segment and sm_seriesid = ms_seriesid  and sm_expirydt = ms_dt ";
                strsql += " and ms_dt < '" + ToDate + "'";
                strsql += " and fi_prodtype in('IF','EF','CF','RF','TF') and fi_type = 'R' ";
                mylib.ExecSQL(strsql, curCon);

                strsql = "update #tmpfinvestorrep  set fi_rate = ms_prcloseprice from #tmpfinvestorrep ,Market_summary";
                strsql += " where ms_seriesid = fi_seriesid and fi_controlflag = 1";
                strsql += " and ms_exchange ='" + Strings.Mid(cmbExchSeg, 2, 1) + "' and ms_segment ='" + Strings.Right(cmbExchSeg, 1) + "'";
                strsql += " and ms_dt = fi_dt";
                strsql += " and fi_prodtype in('IF','EF','CF','RF','TF')";
                mylib.ExecSQL(strsql, curCon);


                //'Service tax here for Trades  

                strsql = "insert into #tmpFinvcharges select td_dt,td_clientcd,'SERVICE TAX',round(sum(td_servicetax),2),0 from Trades with(index(idx_trades_dt_clientcd)) ,#finvdates,Client_master,Series_master";
                strsql += " where td_clientcd = cm_cd and td_dt = bd_dt";
                strsql += " and td_exchange = sm_exchange and td_segment = sm_segment ";
                strsql += " and td_seriesid = sm_seriesid";
                strsql += " and  td_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and  td_exchange = '" + Strings.Mid(cmbExchSeg, 2, 1) + "' and td_segment = '" + Strings.Right(cmbExchSeg, 1) + "'";
                strsql += strCondition;
                strsql += " group by td_dt,td_clientcd having sum(td_servicetax) <> 0";
                mylib.ExecSQL(strsql, curCon);

                //Service tax here for Exercise

                strsql = "insert into #tmpFinvcharges select ex_dt,ex_clientcd,'SERVICE TAX',round(sum(ex_servicetax),2),0 from Exercise,#finvdates,Client_master,Series_master";
                strsql += " where ex_clientcd = cm_cd and ex_dt = bd_dt";
                strsql += " and ex_exchange = sm_exchange and ex_segment = sm_segment ";
                strsql += " and ex_seriesid = sm_seriesid";
                strsql += " and  ex_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and  ex_exchange = '" + Strings.Mid(cmbExchSeg, 2, 1) + "' and  ex_segment ='" + Strings.Right(cmbExchSeg, 1) + "'";
                strsql += Strings.Replace(strCondition, "td_", "ex_");
                strsql += " group by ex_dt,ex_clientcd having sum(ex_servicetax) <> 0";
                mylib.ExecSQL(strsql, curCon);

                //'Charges here
                //    '-----------from specialcharges start
                strsql = "insert into #tmpFinvcharges select fc_dt,fc_clientcd,fc_desc,round(sum(fc_amount),2),0 from Fspecialcharges,#finvdates,Client_master";
                strsql += " where  fc_companycode ='" + Strings.Left(cmbExchSeg, 1) + "' and fc_clientcd = cm_cd and fc_dt = bd_dt";
                strsql += strClientWhere;
                strsql += " and fc_desc not like '%{New%' and fc_exchange='" + Strings.Mid(cmbExchSeg, 2, 1) + "' and  fc_segment='" + Strings.Right(cmbExchSeg, 1) + "' and fc_desc not like '%{Old%'";
                strsql += " and fc_chargecode not in ('EMR') ";
                strsql += " group by fc_dt,fc_clientcd,fc_desc having round(sum(fc_amount),2) <> 0";
                mylib.ExecSQL(strsql, curCon);

                strsql = "insert into #tmpFinvcharges select fc_dt,fc_clientcd,'SERVICE TAX',round(sum(fc_servicetax),2),0 from Fspecialcharges,#finvdates,Client_master";
                strsql += " where fc_companycode ='" + HttpContext.Current.Session["CompanyCode"] + "'and fc_exchange='" + Strings.Mid(cmbExchSeg, 2, 1) + "' and fc_segment='" + Strings.Right(cmbExchSeg, 1) + "' and fc_clientcd = cm_cd and fc_dt = bd_dt";
                strsql += strClientWhere;
                strsql += " group by fc_dt,fc_clientcd,fc_desc having round(sum(fc_servicetax),2) <> 0";
                mylib.ExecSQL(strsql, curCon);

                //'---------------Update values for MTM and Premium

                strsql = "update #tmpfinvestorrep  set fi_bvalue = fi_bqty*fi_rate*fi_multiplier,fi_svalue = fi_sqty*fi_rate*fi_multiplier,";
                strsql += "fi_netqty = fi_bqty - fi_sqty,fi_netvalue = (fi_bqty - fi_sqty)*fi_rate*fi_multiplier";
                strsql += " where fi_controlflag not in(3,4)";
                mylib.ExecSQL(strsql, curCon);

                strsql = "update #tmpfinvestorrep  set fi_bvalue = fi_bqty*fi_rate*fi_multiplier,fi_svalue = fi_sqty*fi_rate*fi_multiplier,";
                strsql += "fi_netqty = fi_sqty - fi_bqty,fi_netvalue = (fi_bqty + fi_sqty)*fi_rate*fi_multiplier";
                strsql += " where fi_controlflag in(3,4)";
                mylib.ExecSQL(strsql, curCon);

                strsql = "update #tmpfinvestorrep  set fi_mtm = round((((fi_sqty - fi_bqty)*fi_rate*fi_multiplier) - ((fi_sqty - fi_bqty)*fi_closeprice*fi_multiplier)),4)";
                strsql += " where fi_prodtype in('IF','EF','CF','RF','TF')";
                mylib.ExecSQL(strsql, curCon);

                strsql = "update #tmpfinvestorrep  set fi_mtm = round(((case fi_controlflag when 3  then (fi_bqty + fi_sqty) when 4 then (fi_bqty + fi_sqty) else (fi_bqty - fi_sqty)*(-1) end) *fi_rate*fi_multiplier),4)";
                strsql += " where fi_prodtype in('IO','EO','CO')";

                mylib.ExecSQL(strsql, curCon);

                //'end
                if (openopt == "UnderlyingClosePrice")
                {
                    strsql = "update #tmpfinvestorrep  set fi_netvalue = 0 where fi_type = 'R'";
                    mylib.ExecSQL(strsql, curCon);

                    strsql = "update #tmpfinvestorrep  ";
                    strsql += " set fi_netvalue = (fi_bqty - fi_sqty)*";
                    strsql += " case sm_callput when 'C' then fi_closeprice - fi_rate else fi_rate - fi_closeprice end*fi_multiplier";
                    strsql += " from #tmpfinvestorrep ,Series_master";
                    strsql += " where fi_exchange = sm_exchange and fi_segment = sm_segment and fi_seriesid = sm_seriesid ";
                    strsql += " and fi_type = 'R'";
                    strsql += " and (fi_bqty - fi_sqty) < 0";
                    mylib.ExecSQL(strsql, curCon);

                    strsql = "update #tmpfinvestorrep  ";
                    strsql += " set fi_netvalue = (fi_bqty - fi_sqty)*";
                    strsql += " case sm_callput when 'C' then fi_rate - fi_closeprice else fi_closeprice - fi_rate end*fi_multiplier";
                    strsql += " from #tmpfinvestorrep ,Series_master";
                    strsql += " where fi_exchange = sm_exchange and fi_segment = sm_segment and fi_seriesid = sm_seriesid ";
                    strsql += " and fi_type = 'R'";
                    strsql += " and (fi_bqty - fi_sqty) > 0";
                    mylib.ExecSQL(strsql, curCon);

                    strsql = "update #tmpfinvestorrep  set fi_mtm = fi_netvalue *(-1) where fi_type = 'R'";

                    mylib.ExecSQL(strsql, curCon);



                }

                // dt = mylib.OpenDataTable("select * from #tmpfinvestorrep", curCon);

                strsql = "select fi_clientcd,cm_name,sm_seriesid,sm_symbol,sm_sname,sm_desc,      ";
                strsql += " case sm_prodtype when 'IF' then 1 when 'EF' then 2 when 'IO' then 3 else 4 end listorder,      ";
                strsql += " sum(case fi_balfield when 'O' then fi_bqty - fi_sqty else 0 end) opnqty,      ";
                strsql += " sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_bqty else 0 end else 0 end) buyqty,      ";
                strsql += " Cast (Round(case When sum(case when fi_type = 'N' and fi_controlflag = 2 then fi_bqty else 0 end)  > 0       ";
                strsql += " Then sum(case when fi_type = 'N' and fi_controlflag = 2 then fi_bvalue else 0 end) / sum(case when fi_type = 'N' and fi_controlflag = 2 then fi_bqty else 0 end)      ";
                strsql += " else 0 end,2) as decimal(18,2)) buyRate,      ";
                strsql += " cast ( sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_bvalue else 0 end else 0 end) as decimal(18,2)) buyvalue,      ";
                strsql += " sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_sqty else 0 end else 0 end) sellqty,      ";
                strsql += " cast(Round(case When sum(case when fi_type = 'N' and fi_controlflag = 2 then fi_sqty else 0 end ) > 0       ";
                strsql += " Then sum(case when fi_type = 'N' and fi_controlflag = 2 then fi_svalue else 0 end) / sum(case when fi_type = 'N' and fi_controlflag = 2 then fi_sqty else 0 end)      ";
                strsql += " else 0 end,2) as decimal(18,2)) sellRate ,      ";
                strsql += " cast ( sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_svalue else 0 end else 0 end) as decimal(18,2)) sellvalue,      ";
                strsql += " sum(case fi_type when 'N' then case fi_controlflag when 3 then abs(fi_bqty) else 0 end else 0 end) exerqty,      ";
                strsql += " sum(case fi_type when 'N' then case fi_controlflag when 4 then abs(fi_sqty) else 0 end else 0 end) assgnqty,      ";
                strsql += " sum(case when sm_expirydt <= '" + ToDate + "' then 0 else case fi_balfield when 'N' then  0 else (fi_bqty)*case fi_controlflag when 3 then -1 else 1 end  - (fi_sqty)*case fi_controlflag when 4 then -1 else 1 end end end) outqty,      ";
                strsql += " sum(case fi_type when 'R' then  fi_sqty - fi_bqty else 0 end) closeqty,      ";
                strsql += " sum(fi_netvalue) netvalue,      ";
                strsql += " sum(case fi_type when 'R' then 0 else fi_netvalue end) pnetvalue,      ";
                strsql += " sum(fi_mtm) mtm,0 fi_closeprice       ";
                strsql += " from #tmpfinvestorrep  a,Client_master,Series_master      ";
                strsql += " where(fi_clientcd = cm_cd And fi_exchange = sm_exchange)";
                strsql += " and fi_Segment = sm_Segment and fi_seriesid = sm_seriesid      ";
                strsql += " group by fi_clientcd,cm_name,sm_seriesid,sm_prodtype,sm_symbol,sm_sname,sm_desc,fi_multiplier       ";
                strsql += " order by fi_clientcd,cm_name,listorder,sm_symbol    ";

                dt = mylib.OpenDataTable(strsql, curCon);

                strsql = "select bc_desc,sum(bc_amount*(-1))  chrg from #tmpFinvcharges group by bc_desc ";
                DataTable dtcharges = new DataTable();
                dtcharges = mylib.OpenDataTable(strsql, curCon);
                HttpContext.Current.Session["dtcharges"] = dtcharges;
            }
            return dt;

        }

        public DataTable getInvestorBasedReportCash(string Select, string Code, string FromDate, string ToDate, string strDPID, string cmbReport, string cmbtype, string cmbstock)
        {
            DataTable dtIBRC = new DataTable();
            string strDatefrom;
            string strDate = "";
            string strChargeswhere = "";
            string strWhere = "";
            string strSql;
            string strterminals = "";
            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            Boolean blnIBroker = false;

            if (FromDate != "" || ToDate != "")
            {
                strDate = " and td_dt>= '" + Strings.Trim(FromDate) + "' and td_dt<='" + Strings.Trim(ToDate) + "'   ";
                strDatefrom = " and td_dt = '" + Strings.Trim(FromDate) + "'";
                strChargeswhere = strChargeswhere + " and se_endt between '" + Strings.Trim(FromDate) + "' and '" + Strings.Trim(ToDate) + "'";
            }
            else
            {
                //strDate = " and td_stlmnt >= '" + Strings.Trim(txtSetFrom.Text) + "' and td_stlmnt <='" + Strings.Trim(txtSetTo.Text) + "'";
                //strDatefrom = " and td_stlmnt = '" + Strings.Trim(txtSetFrom.Text) + "'";
                //strChargeswhere = strChargeswhere + " and se_stlmnt between '" + Strings.Trim(txtSetFrom.Text) + "' and '" + Strings.Trim(txtSetTo.Text) + "'";
            }

            if (!string.IsNullOrEmpty(Strings.Trim(Code)) && Select == "Client")
            {
                strWhere = strWhere + " and td_clientcd ='" + Strings.Trim(Code) + "' ";
                strChargeswhere = strChargeswhere + " and sh_clientcd='" + Strings.Trim(Code) + "'";
            }

            if (!string.IsNullOrEmpty(Strings.Trim(Code)) & Select == "Pan No")
            {
                strWhere = strWhere + " and cm_PanNo ='" + Strings.Trim(Code) + "' ";
                strChargeswhere = strChargeswhere + " and cm_PanNo='" + Strings.Trim(Code) + "'";
            }

            bool first = true;
            string Exchng = "";
            string[] strArray = strDPID.Split(',');

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

            strWhere += " and left(td_stlmnt,1)+'C' in ('" + Strings.Replace(Exchng, ",", "','") + "')";
            strChargeswhere += " and left(sh_stlmnt,1)+'C' in ('" + Strings.Replace(Exchng, ",", "','") + "')";

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();


                string strType = "";

                if (cmbtype == "All")
                {
                    strType = "";
                }
                else if (cmbtype == "Square off")
                {
                    strType = " and td_flag='Y'";
                }
                else if (cmbtype == "Delivery")
                {
                    strType = " and td_flag='N'";
                }

                string StrTrxIndex = string.Empty;

                if (!string.IsNullOrEmpty(Strings.Trim(strterminals)))
                {
                    strterminals = Strings.Left(strterminals, strterminals.Length - 1);
                }

                if (Convert.ToInt32(mylib.fnFireQuery("sysobjects a, sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'Trx' and b.name", "idx_Trx_Clientcd", true, curCon)) == 1)
                {
                    StrTrxIndex = "index(idx_trx_clientcd),";
                }


                strSql = "Drop Table #Vx";
                mylib.ExecSQL(strSql, curCon);


                strSql = " CREATE TABLE [dbo].[#Vx] (";
                strSql = strSql + " [td_companycode] [char] (1) NOT NULL ,";
                strSql = strSql + " [td_stlmnt] [char] (9) NOT NULL ,";
                strSql = strSql + " [td_clientcd] [char] (8) NOT NULL ,";
                strSql = strSql + " [td_scripcd] [char] (6) NOT NULL ,";
                strSql = strSql + " [td_tradeid] [numeric](18, 0) NOT NULL ,";
                strSql = strSql + " [td_subtradeid] [numeric](18, 0) NOT NULL ,";
                strSql = strSql + " [td_dt] [char] (8) NOT NULL ,";
                strSql = strSql + " [td_srno] [numeric](18, 0) IDENTITY(1,1)  NOT NULL ,";
                strSql = strSql + " [td_ssrno] [numeric](18, 0) NOT NULL ,";
                strSql = strSql + " [td_bsflag] [char] (1) NOT NULL ,";
                strSql = strSql + " [td_bqty] [numeric](18, 0) NOT NULL ,";
                strSql = strSql + " [td_sqty] [numeric](18, 0) NOT NULL ,";
                strSql = strSql + " [td_rate] [money] NOT NULL ,";
                strSql = strSql + " [td_time] [char] (8) NULL ,";
                strSql = strSql + " [td_marketrate] [money] NOT NULL ,";
                strSql = strSql + " [td_cfflag] [char] (1) NOT NULL ,";
                strSql = strSql + " [td_dqty] [numeric](18, 0) NOT NULL ,";
                strSql = strSql + " [td_dsqty] [numeric](18, 0) NOT NULL ,";
                strSql = strSql + " [td_flag] [VarChar](1) Not null ";
                strSql = strSql + " PRIMARY KEY(td_srno))";
                mylib.ExecSQL(strSql, curCon);


                strSql = "Create INDEX [IX_" + myutil.fnGetTime() + "_index] ON [dbo].[#Vx]";
                strSql = strSql + " ([td_clientcd], [td_scripcd],[td_dt],[td_tradeid],[td_stlmnt])";
                mylib.ExecSQL(strSql, curCon);


                strSql = "insert into #Vx  SELECT td_companycode, td_stlmnt, ";

                if (Select == "Client")
                {
                    strSql = strSql + " td_clientcd , ";
                }
                else
                {
                    strSql = strSql + "'" + mylib.fnFireQuery("Client_master", "cm_cd", "cm_PaNNo", Code, true, curCon) + "',";
                }

                strSql = strSql + " td_scripcd, 0 Td_tradeid, ";
                strSql = strSql + " 0 td_subtradeid, td_dt, 0 td_ssrno, ";
                strSql = strSql + " td_bsflag, sum(td_bqty), sum(td_sqty), sum(td_rate*(td_bqty+td_sqty)), ";
                strSql = strSql + " '' td_time, 0 , '' td_cfflag, 0 td_dqty ,0 td_dsqty ,'Y' td_flag ";
                strSql = strSql + " FROM Trx with (" + StrTrxIndex + "nolock),Client_master where ";  // td_companycode = '" & lstCompany.Items(lngLoop).Value & "'"
                strSql = strSql + Strings.Mid(strDate, 6, 200) + strWhere + " and td_cfflag = 'N' and td_clientcd = cm_cd ";
                strSql = strSql + " and td_stlmnt not in ( select se_stlmnt From Settlements,settlement_type Where se_exchange = sy_exchange and se_type=sy_type and sy_maPType in ('S','W','V'))";
                strSql = strSql + " group by td_companycode, td_stlmnt,td_scripcd, td_dt, td_bsflag ";
                if (Select == "Client")
                {
                    strSql = strSql + " ,td_clientcd ";
                }

                mylib.ExecSQL(strSql, curCon);

                strSql = "update #Vx set td_rate=td_rate/(td_bqty+td_sqty), td_marketrate=td_marketrate/(td_bqty+td_sqty) ";

                mylib.ExecSQL(strSql, curCon);

                // Procedure to include charges 
                if (!(cmbReport == "StockPosition"))
                {
                    strSql = "Drop Table #invcharges";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " CREATE TABLE [dbo].[#invcharges] (";
                    strSql = strSql + " [ic_desc] [char] (20) NOT NULL ,";
                    strSql = strSql + " [ic_amount] money";
                    strSql = strSql + " ) ON [PRIMARY]";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = "insert into #invcharges select left(sh_desc,12), sum(sh_amount) ";
                    strSql = strSql + " from Specialcharges, Settlements,Client_master Where sh_stlmnt = se_stlmnt and sh_clientcd = cm_cd ";
                    strSql = strSql + " and se_exchange+se_type not in (select sy_exchange+sy_type from Settlement_type Where sy_maptype in ('S','W','V')) ";
                    strSql = strSql + strChargeswhere + " group by left(sh_desc,12) union all ";
                    strSql = strSql + " select 'Service Tax', isNull(sum(sh_servicetax),0) ";
                    strSql = strSql + " from Specialcharges, Settlements,Client_master where sh_stlmnt = se_stlmnt and sh_clientcd = cm_cd ";
                    strSql = strSql + " and se_exchange+se_type not in (select sy_exchange+sy_type from Settlement_type Where sy_maptype in ('S','W','V')) ";
                    strSql = strSql + " and sh_servicetaxyn = 'Y' and sh_servicetax > 0 ";
                    strSql = strSql + strChargeswhere + " union all ";
                    strSql = strSql + "select left(cg_desc,12), sum(bc_amount) ";
                    strSql = strSql + " from Cbilled_charges, Charges_master, Settlements,Client_master Where bc_companycode = cg_companycode ";
                    strSql = strSql + " And Left(bc_stlmnt, 1) = cg_exchange and bc_chargecode = cg_cd and bc_stlmnt = se_stlmnt and bc_clientcd = cm_cd ";
                    strSql = strSql + " and se_exchange+se_type not in (select sy_exchange+sy_type from Settlement_type Where sy_maptype in ('S','W','V')) ";
                    strSql = strSql + strChargeswhere.Replace("sh_", "bc_") + " group by left(cg_desc,12) ";
                    mylib.ExecSQL(strSql, curCon);
                }


                strSql = "update a set a.td_flag = 'N' from #Vx a";
                strSql = strSql + " where a.td_clientcd + a.td_scripcd + a.td_stlmnt";
                strSql = strSql + " in(select b.td_clientcd + b.td_scripcd + b.td_stlmnt";
                strSql = strSql + " from #Vx b where a.td_clientcd = b.td_clientcd";
                strSql = strSql + " and a.td_scripcd = b.td_scripcd";
                strSql = strSql + " and a.td_stlmnt = b.td_stlmnt";
                strSql = strSql + " group by td_clientcd,td_scripcd,td_stlmnt";
                strSql = strSql + " having sum(td_bqty) = 0 or sum(td_sqty) = 0)";
                mylib.ExecSQL(strSql, curCon);

                strSql = "insert into #Vx ";
                strSql = strSql + "(td_companycode,td_stlmnt,td_clientcd,td_scripcd,td_tradeid,td_subtradeid,td_dt,td_ssrno,td_bsflag,td_bqty,td_sqty,";
                strSql = strSql + " td_rate,  td_time, td_marketrate,  td_cfflag,td_dqty, td_dsqty, td_flag )";
                strSql = strSql + " SELECT  td_companycode, td_stlmnt, td_clientcd, td_scripcd, 0,";
                strSql = strSql + " 0, td_dt, 0, case sign(sum(td_bqty-td_sqty)) when 1 then 'B' else 'S' end td_bsflag, ";
                strSql = strSql + " case sign(sum(td_bqty-td_sqty)) when 1 then sum(td_bqty-td_sqty) else 0 end  td_bqty,";
                strSql = strSql + " case sign(sum(td_bqty-td_sqty)) when 1 then 0 else sum(td_sqty-td_bqty) end td_bqty,";
                strSql = strSql + " case sign(sum(td_bqty-td_sqty)) when 1 then sum(td_bqty*td_rate)/sum(td_bqty) else sum(td_sqty*td_rate)/sum(td_sqty) end td_rate,";
                strSql = strSql + " '' td_time, 0 ,'' td_cfflag, 0,0,'X' ";
                strSql = strSql + " FROM #Vx where td_flag = 'Y'  ";
                strSql = strSql + " group by td_companycode, td_stlmnt,td_clientcd, td_scripcd,td_dt";
                strSql = strSql + " having(sum(td_bqty - td_sqty) <> 0)";
                mylib.ExecSQL(strSql, curCon);

                strSql = "update a set td_bqty=a.td_bqty - b.td_bqty, td_sqty=a.td_sqty - b.td_sqty from #Vx a, #Vx b";
                strSql = strSql + " where(a.td_clientcd = b.td_clientcd And a.td_scripcd = b.td_scripcd And a.td_stlmnt = b.td_stlmnt)";
                strSql = strSql + " and a.td_bsflag=b.td_bsflag and a.td_flag='Y' and b.td_flag='X'";
                mylib.ExecSQL(strSql, curCon);

                strSql = "update #Vx set td_flag='N' where td_flag='X'";
                mylib.ExecSQL(strSql, curCon);

                strSql = " select * from ClearingHouse Where CH_CompanyCode = '" + HttpContext.Current.Session["CompanyCode"] + "' and CH_Segment = 'C' ";
                strSql += " and  CH_EffDt = (select Min(CH_EffDt) from ClearingHouse ";
                strSql += "Where CH_CompanyCode = '" + HttpContext.Current.Session["CompanyCode"] + "' and CH_Segment = 'C' and CH_EffDt <= '" + Strings.Trim(ToDate) + "') ";
                DataTable dtTemp = mylib.OpenDataTable(strSql, curCon);
                if (dtTemp.Rows.Count > 0)
                {
                    strSql = " Update X set td_stlmnt = b.se_stlmnt ";
                    strSql += " from #VX X, Settlements a, Settlements b ";
                    strSql += " Where td_dt >= '" + dtTemp.Rows[0]["CH_EffDt"].ToString() + "' and left(td_stlmnt,2) in ('BW','NN') and td_stlmnt = a.se_stlmnt ";
                    strSql += " and a.se_stdt = b.se_stdt and left(a.se_stlmnt,2) = '" + (dtTemp.Rows[0]["CH_ClgHs"].ToString() == "N" ? "BW" : "NN") + "' and left(b.se_stlmnt,2) = '" + (dtTemp.Rows[0]["CH_ClgHs"].ToString() == "N" ? "NN" : "BW") + "'";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " Update X set td_stlmnt = b.se_stlmnt ";
                    strSql += " from #VX X, Settlements a, Settlements b ";
                    strSql += " Where td_dt >= '" + dtTemp.Rows[0]["CH_EffDt"].ToString() + "' and left(td_stlmnt,2) in ('BC','NZ') and td_stlmnt = a.se_stlmnt ";
                    strSql += " and a.se_stdt = b.se_stdt and left(a.se_stlmnt,2) = '" + (dtTemp.Rows[0]["CH_ClgHs"].ToString() == "N" ? "BC" : "NZ") + "' and left(b.se_stlmnt,2) = '" + (dtTemp.Rows[0]["CH_ClgHs"].ToString() == "N" ? "NZ" : "BC") + "'";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " Update X set td_stlmnt = b.se_stlmnt ";
                    strSql += " from #VX X, Settlements a, Settlements b ";
                    strSql += " Where td_dt >= '" + dtTemp.Rows[0]["CH_EffDt"].ToString() + "' and left(td_stlmnt,2) in ('BR','NA') and td_stlmnt = a.se_stlmnt ";
                    strSql += " and a.se_stdt = b.se_stdt and left(a.se_stlmnt,2) = '" + (dtTemp.Rows[0]["CH_ClgHs"].ToString() == "N" ? "BR" : "NA") + "' and left(b.se_stlmnt,2) = '" + (dtTemp.Rows[0]["CH_ClgHs"].ToString() == "N" ? "NA" : "BR") + "'";
                    mylib.ExecSQL(strSql, curCon);
                }

                if (cmbstock == "Cost")
                {
                    string strclient = "";
                    string strscrip = "";
                    string strstlmnt = "";
                    string strDelSide = "";
                    long lngDelQty = 0L;
                    long lngBalqty = 0L;
                    var adpInvestor = new SqlDataAdapter();
                    var dsInvestor = new DataTable();
                    var cmdInvestor = new SqlCommand();
                    long lngCurSerial;
                    strSql = "Select td_clientcd , td_scripcd,cm_name,ss_name,";
                    strSql = strSql + " sum(td_bqty) td_bqty ,sum(td_sqty) td_sqty, sum(td_bqty-td_sqty) net";
                    strSql = strSql + " FROM #Vx,Client_master,Securities ";
                    strSql = strSql + " where td_clientcd = cm_cd and td_scripcd = ss_cd and td_flag = 'N'";
                    strSql = strSql + " group by td_clientcd,cm_name,td_scripcd,ss_name ";
                    strSql = strSql + " having sum(td_bqty - td_sqty) <> 0";
                    strSql = strSql + " ORDER BY td_clientcd , td_scripcd";
                    DataTable dsProcess = new DataTable();

                    dsProcess = mylib.OpenDataTable(strSql, curCon);

                    if (dsProcess.Rows.Count > 0)
                    {

                        for (int j = 0; j < dsProcess.Rows.Count; j++)
                        {
                            if (Convert.ToInt32(dsProcess.Rows[j]["net"]) > 0)
                            {
                                strDelSide = "B";
                                lngDelQty = Math.Abs(Convert.ToInt32(dsProcess.Rows[j]["net"]));
                            }
                            else
                            {
                                strDelSide = "S";
                                lngDelQty = Math.Abs(Convert.ToInt32(dsProcess.Rows[j]["net"]));
                            }

                            strclient = (dsProcess.Rows[j]["td_Clientcd"]).ToString();
                            strscrip = (dsProcess.Rows[j]["td_scripcd"]).ToString();
                            // strstlmnt = dsProcess.Tables(0).Rows(j).Item("td_stlmnt")

                            strSql = "select * from #Vx where td_clientcd = '" + strclient + "' and td_scripcd = '" + strscrip + "'";
                            strSql = strSql + " and td_bsflag = '" + strDelSide + "' and td_flag = 'N' order by td_dt desc,td_stlmnt desc";
                            // cmdInvestor = New SqlCommand
                            // dsInvestor = New DataSet
                            // cmdInvestor.Connection = ObjConnection
                            // cmdInvestor.CommandText = strSql
                            // adpInvestor = New SqlDataAdapter(cmdInvestor)
                            // adpInvestor.Fill(dsInvestor)

                            dsInvestor = mylib.OpenDataTable(strSql, curCon);

                            for (int k = 0; k < dsInvestor.Rows.Count; k++)
                            {
                                while (lngDelQty > 0L)
                                {
                                    lngCurSerial = Convert.ToInt64(dsInvestor.Rows[k]["td_SrNo"]);

                                    if (Convert.ToInt64(dsInvestor.Rows[k]["td_bqty"]) + Convert.ToInt64(dsInvestor.Rows[k]["td_Sqty"]) > lngDelQty)
                                    {
                                        // Split
                                        lngBalqty = Convert.ToInt64(dsInvestor.Rows[k]["td_bqty"]) + Convert.ToInt64(dsInvestor.Rows[k]["td_Sqty"]) - lngDelQty;
                                        strSql = " insert into #Vx select td_companycode ,td_stlmnt,td_clientcd , td_scripcd,td_tradeid,td_subtradeid, td_dt,td_ssrno,td_bsflag,";
                                        if (strDelSide == "B")
                                        {
                                            strSql = strSql + lngDelQty + ", td_sqty";
                                        }
                                        else
                                        {
                                            strSql = strSql + " td_bqty ," + lngDelQty;
                                        }

                                        strSql = strSql + ", td_rate,td_time, td_marketrate,td_cfflag,td_dqty,td_dsqty,'X' from #Vx where td_srno =" + lngCurSerial;
                                        mylib.ExecSQL(strSql, curCon);

                                        strSql = "update #Vx set ";
                                        if (strDelSide == "B")
                                        {
                                            strSql = strSql + " td_bqty = ";
                                        }
                                        else
                                        {
                                            strSql = strSql + " td_sqty = ";
                                        }

                                        strSql = strSql + lngBalqty + " where td_srno = " + lngCurSerial;
                                        mylib.ExecSQL(strSql, curCon);
                                        lngDelQty = 0L;
                                    }
                                    else
                                    {
                                        strSql = "update #Vx set td_flag = 'X' where td_srno = " + lngCurSerial;
                                        mylib.ExecSQL(strSql, curCon);
                                        lngDelQty = lngDelQty - (Convert.ToInt64(dsInvestor.Rows[k]["td_bqty"]) + Convert.ToInt64(dsInvestor.Rows[k]["td_Sqty"]));
                                    }

                                    k = k + 1;
                                }
                            }
                        }
                    }
                    cmdInvestor.Dispose();
                }


                if (cmbReport == "StockPosition")
                {
                    // cmdTemp = New SqlCommand("delete from #Vx where td_flag='Y'", ObjConnection)
                    // cmdTemp.ExecuteNonQuery()
                    // cmdTemp.Dispose()
                    // cmdTemp = New SqlCommand("delete from #Vx where td_scripcd+'x'+td_clientcd in (select td_scripcd+'x'+td_clientcd from #Vx group by td_clientcd, td_scripcd having sum(td_bqty-td_sqty) =0)", ObjConnection)
                    // cmdTemp.ExecuteNonQuery()
                    // cmdTemp.Dispose()
                    mylib.ExecSQL("delete from #Vx where td_flag='Y'", curCon);
                    mylib.ExecSQL("delete from #Vx where td_scripcd+'x'+td_clientcd in (select td_scripcd+'x'+td_clientcd from #Vx group by td_clientcd, td_scripcd having sum(td_bqty-td_sqty) =0)", curCon);

                }

                if (cmbstock == "Cost")
                {
                    strSql = "update #Vx set td_MarketRate = td_Rate Where td_flag = 'X' ";
                    // cmdReport.Connection = ObjConnection
                    mylib.ExecSQL(strSql, curCon);
                }
                else
                {
                    strSql = "update #Vx set td_marketrate = isNull((select top 1 mk_closerate From Market_rates where mk_scripcd = td_scripcd ";
                    strSql = strSql + " and mk_dt = (select  max(mk_dt) from Market_rates ";
                    strSql = strSql + " where mk_scripcd = td_scripcd ";
                    strSql = strSql + "  and mk_dt<='" + ToDate + "') ";
                    strSql = strSql + " Order by mk_exchange desc),0)";
                    mylib.ExecSQL(strSql, curCon);
                }

                if (cmbReport == "StlmntWise")
                {
                    strSql = " select '1' Sortorder,'1' RateType, Rtrim(ss_name) + ' ['  + td_scripcd + ']'  as [GroupByValue],td_stlmnt,td_clientcd ,case td_flag when 'Y' then 'Spc' else 'Dlv' end as td_flag,";
                    strSql = strSql + " sum(td_bqty) Bqty,case When sum(td_rate*td_bqty) > 0 then sum(td_rate*td_bqty)/sum(td_bqty) else 0 end BRate, sum(td_rate*td_bqty) BValue,";
                    strSql = strSql + " sum(td_sqty) sqty,case When sum(td_rate*td_sqty) > 0 then sum(td_rate*td_sqty)/sum(td_sqty) else 0 end SRate, sum(td_rate*td_sqty) SValue,";
                    strSql = strSql + " td_dt from #Vx, securities where(ss_cd = td_scripcd) " + strType + " Group By td_stlmnt,td_clientcd,td_scripcd,ss_name,td_Flag,td_dt";
                    strSql = strSql + " union all";
                    strSql = strSql + " select '2' Sortorder,'1' RateType, Rtrim(ss_name) + ' ['  + td_scripcd + ']'  as [GroupByValue],case When sum(td_bqty-td_sqty) > 0 Then 'Clsg. Stk' else 'Exs. Sale' end,td_clientcd,'N',";
                    strSql = strSql + " case When sum(td_sqty-td_bqty) > 0 Then sum(td_sqty-td_bqty) else 0 end Bqty,case When sum(td_sqty-td_bqty) > 0 Then sum((td_sqty-td_bqty)*td_marketrate)/sum(td_sqty-td_bqty) else 0 end BRAte,case When sum(td_sqty-td_bqty) > 0 Then sum((td_sqty-td_bqty)*td_marketrate) else 0 end BValue,";
                    strSql = strSql + " case When sum(td_bqty-td_sqty) > 0 Then sum(td_bqty-td_sqty) else 0 end Sqty,case When sum(td_bqty-td_sqty) > 0 Then sum((td_bqty-td_sqty)*td_marketrate)/sum(td_bqty-td_sqty) else 0 end SRAte,case When sum(td_bqty-td_sqty) > 0 Then sum((td_bqty-td_sqty)*td_marketrate) else 0 end SValue,";
                    strSql = strSql + " '' from #Vx, securities where(ss_cd = td_scripcd) " + strType + " Group By td_clientcd,td_scripcd,ss_name  Having sum(td_bqty-td_sqty)  <> 0";
                    if (!(cmbReport == "StockPosition"))
                    {
                        strSql = strSql + " union all select '3' Sortorder,'2' RateType,'',ic_desc,'','Z',0,0,0,0,0,convert(decimal(15,2),sum((-1)*ic_amount)),'' from #invcharges group by ic_desc";
                    }

                    strSql = strSql + " Order by RateType,GroupByValue,td_clientcd,Sortorder,td_dt";
                    DataTable dsProcess = new DataTable();

                    dsProcess = mylib.OpenDataTable(strSql, curCon);

                    if (dsProcess.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsProcess.Rows)
                        {
                            if (dr["td_dt"] != "")
                            {
                                dr["td_dt"] = myutil.stod(dr["td_dt"].ToString()).ToString("dd/MM/yyyy");
                            }
                        }
                        dtIBRC = dsProcess;
                        HttpContext.Current.Session["DataGridRpt"] = dsProcess;

                    }
                    else
                    {

                    }
                }
                else if (cmbReport == "ScripWise" || cmbReport == "StockPosition")
                {
                    strSql = " select ";
                    if (Select == "Client")
                    {
                        strSql = strSql + " '" + Strings.Trim(Code) + " [" + Strings.Trim(Code) + "]' as Code,";
                    }
                    else
                    {
                        strSql = strSql + " '" + Strings.Trim(Code) + "' as Code,";
                    }

                    strSql = strSql + " td_scripcd, ss_name, case td_flag when 'Y' then 'Spc' else 'Dlv' end as td_flag,sum(td_rate) as td_rate,avg(td_marketrate) as td_marketrate, sum(td_bqty) Bqty,";
                    strSql = strSql + " cast(Case when sum(td_bqty) > 0 then sum(td_rate*td_bqty)/sum(td_bqty) else 0 end as decimal(15,2)) Rate1, ";
                    strSql = strSql + " cast(sum(td_rate*td_bqty) as decimal(15,2)) BAmt, ";
                    strSql = strSql + " sum(td_sqty) Sqty, ";
                    strSql = strSql + " cast(case when sum(td_sqty)>0 then sum(td_rate*td_sqty)/sum(td_sqty) else 0 end as decimal(15,2)) Rate2, ";
                    strSql = strSql + " cast(sum(td_rate*td_sqty) as decimal(15,2)) SAmt, ";
                    strSql = strSql + " sum(td_bqty-td_sqty) NQty, ";
                    strSql = strSql + " case When sum(td_bqty-td_sqty) = 0 Then 0 else case when Max(td_marketrate) > 0 Then sum(td_marketrate*(td_bqty-td_sqty)) else sum(td_bqty-td_sqty)* ((sum(td_bqty*td_Rate)+sum(td_sqty*td_Rate))/abs(sum(td_bqty-td_sqty))) End End as NAmt,";
                    strSql = strSql + " 0.00 as Profit,";
                    strSql = strSql + " cast(case sign(sum(td_bqty-td_sqty)) when 0 then 0 else max(td_marketrate) end as decimal(15,2)) MktRate, 0.00 NetRate, 0 Ordr  ";
                    strSql = strSql + " from #Vx, securities where(ss_cd = td_scripcd) " + strType + "";
                    strSql = strSql + " group by td_scripcd, ss_name, td_flag ";
                    if (!(cmbReport == "StockPosition"))
                    {
                        strSql = strSql + " union all select '','',ic_desc,'Z',0,0,0,0,0,0,0,0,0,0,convert(decimal(15,2),sum((-1)*ic_amount)),0,0,1 from #invcharges group by ic_desc ";
                    }

                    strSql = strSql + " order by ordr,ss_name,td_flag";
                    DataTable dsProcess = new DataTable();
                    dsProcess = mylib.OpenDataTable(strSql, curCon);
                    int j;
                    var loopTo = dsProcess.Rows.Count;
                    for (j = 0; j < loopTo; j++)
                    {
                        if ((string)dsProcess.Rows[j]["td_flag"] == "Z")
                        {
                            dsProcess.Rows[j]["Profit"] = dsProcess.Rows[j]["Profit"];
                        }
                        else if ((Decimal)dsProcess.Rows[j]["NQty"] == 0)
                        {
                            dsProcess.Rows[j]["Profit"] = (Decimal)dsProcess.Rows[j]["SAmt"] - (Decimal)dsProcess.Rows[j]["BAmt"];
                        }
                        else if ((Decimal)dsProcess.Rows[j]["td_marketrate"] != 0)
                        {
                            dsProcess.Rows[j]["Profit"] = Convert.ToDecimal(Convert.ToDecimal(dsProcess.Rows[j]["SAmt"]) - Convert.ToDecimal(dsProcess.Rows[j]["BAmt"]) + Convert.ToDecimal(dsProcess.Rows[j]["NAmt"])).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(dsProcess.Rows[j]["Bqty"]) < Convert.ToDecimal(dsProcess.Rows[j]["Sqty"]))
                        {
                            dsProcess.Rows[j]["Profit"] = Convert.ToDecimal(Convert.ToDecimal(dsProcess.Rows[j]["SAmt"]) - Convert.ToDecimal(dsProcess.Rows[j]["NAmt"]) - Convert.ToDecimal(dsProcess.Rows[j]["BAmt"])).ToString("0.00");
                        }
                        else
                        {
                            dsProcess.Rows[j]["Profit"] = Convert.ToDecimal(Convert.ToDecimal(dsProcess.Rows[j]["SAmt"]) - Convert.ToDecimal(dsProcess.Rows[j]["BAmt"]) + Convert.ToDecimal(dsProcess.Rows[j]["NAmt"])).ToString("0.00");
                        }

                        if (Convert.ToDecimal(dsProcess.Rows[j]["Bqty"]) - Convert.ToDecimal(dsProcess.Rows[j]["Sqty"]) != 0)
                        {

                            dsProcess.Rows[j]["NetRate"] = Convert.ToDecimal((Convert.ToDecimal(dsProcess.Rows[j]["BAmt"]) - Convert.ToDecimal(dsProcess.Rows[j]["SAmt"])) / (Convert.ToDecimal(dsProcess.Rows[j]["Bqty"]) - (Decimal)dsProcess.Rows[j]["Sqty"])).ToString("0.00");
                        }
                        else
                        {
                            dsProcess.Rows[j]["NetRate"] = 0.0d;
                        }
                    }

                    if (dsProcess.Rows.Count > 0)
                    {
                        dtIBRC = dsProcess;
                        HttpContext.Current.Session["DataGrid"] = dsProcess;
                    }
                    else
                    {

                    }
                }

                if (!(cmbReport == "StockPosition"))
                {
                    strSql = "Update #Vx set td_flag  = 'N' Where td_flag='X' ";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " select td_scripcd, ss_name, td_flag, sum(td_bqty) Bqty, sum(td_rate*td_bqty) BAmt, ";
                    strSql = strSql + " sum(td_sqty) Sqty, sum(td_rate*td_sqty) SAmt,";
                    strSql = strSql + " sum(td_bqty-td_sqty) NQty, sum((td_bqty-td_sqty)*td_marketrate) NAmt,";
                    strSql = strSql + " Case When td_flag = 'Z' Then 0.00 else Case When sum(td_bqty-td_sqty) = 0 then sum(td_rate*td_sqty) - sum(td_rate*td_bqty) else  ";
                    strSql = strSql + " Case When avg(td_marketrate) <> 0  Then sum(td_rate*td_sqty) - sum(td_rate*td_bqty) + (avg(td_marketrate) * (sum(td_bqty)-sum(td_sqty)))";
                    strSql = strSql + " Else  ";
                    strSql = strSql + " Case When sum(td_bqty) < sum(td_sqty) Then ";
                    strSql = strSql + " sum(td_sqty*td_rate)-sum(td_bqty*td_rate) -  (sum(td_bqty-td_sqty) * (sum(td_bqty*td_rate)+sum(td_sqty*td_rate)) / sum(td_bqty+td_sqty)) ";
                    strSql = strSql + " else sum(td_sqty*td_rate)-sum(td_bqty*td_rate) + (sum(td_bqty-td_sqty) * (sum(td_bqty*td_rate)+sum(td_sqty*td_rate)) / sum(td_bqty+td_sqty)) end ";
                    strSql = strSql + " End end end Profit, ";
                    strSql = strSql + " case sign(sum(td_bqty-td_sqty)) when 0 then 0 else Case When td_marketrate > 0 Then max(td_marketrate) else (sum(td_rate*td_bqty)+sum(td_rate*td_sqty)) / sum(td_bqty+td_Sqty) end end MktRate ";
                    strSql = strSql + " into #Vx1 from #Vx, securities where(ss_cd = td_scripcd) " + strType + "";
                    strSql = strSql + " group by td_scripcd, ss_name, td_flag , td_marketrate  order  by ss_name, td_flag";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = "drop table #Vx";
                    mylib.ExecSQL(strSql, curCon);

                    strSql = " select 'Profit' 'desc', 0 Qty, cast(isnull(sum(case td_flag when 'N' then case sign(Profit) when 1 then profit else 0 end else 0 end),0) as decimal(15,2)) Delivery, 0 Qty, ";
                    strSql = strSql + " cast(isnull(sum(case td_flag when 'N' then 0 else case sign(Profit) when 1 then profit else 0 end end),0) as decimal(15,2)) Squaredoff, cast(isnull(sum(case sign(profit) when 1 then Profit else 0 end),0) as decimal(15,2)) 'Total',0 'Net' from #Vx1 union all ";
                    strSql = strSql + " select 'Loss', 0, isnull(sum(case td_flag when 'N' then case sign(Profit) when 1 then 0 else (-1)*profit end else 0 end),0), 0, ";
                    strSql = strSql + " isnull(sum(case td_flag when 'N' then 0 else case sign(Profit) when 1 then 0 else (-1)*profit end end),0), isnull(sum(case sign(profit) when 1 then 0 else (-1)*Profit end ),0),0 from #Vx1 union all";
                    strSql = strSql + " select 'P/L', 0,isnull(sum(case td_flag when 'N' then profit else 0 end),0), 0, ";
                    strSql = strSql + " isnull(sum(case td_flag when 'N' then 0 else Profit end),0) , 0, isnull(sum(Profit),0) from #Vx1 union all ";
                    strSql = strSql + " select 'Expense', 0,0,0,0,0,isnull(sum(ic_amount),0) from #invcharges where ic_amount>0 union all ";   // (-1)*
                    strSql = strSql + " select 'Net P/L',0,0,0,0,0, (( Select isnull(sum(Profit),0) from #Vx1) - (select isnull(sum(ic_amount),0) from #invcharges )) union all";
                    strSql = strSql + " select 'Bought', isnull(sum(case td_flag when 'N' then Bqty else 0 end),0) ,isnull(sum(case td_flag when 'N' then Bamt else 0 end),0), ";
                    strSql = strSql + " isnull(sum(case td_flag when 'N' then 0 else bqty end),0), isnull(sum(case td_flag when 'N' then 0 else Bamt end),0), ";
                    strSql = strSql + " isnull(sum(Bamt),0) , 0 from #Vx1 union all ";
                    strSql = strSql + " select 'Sold', isnull(sum(case td_flag when 'N' then Sqty else 0 end),0) , isnull(sum(case td_flag when 'N' then Samt else 0 end),0), ";
                    strSql = strSql + " isnull(sum(case td_flag when 'N' then 0 else Sqty end),0), isnull(sum(case td_flag when 'N' then 0 else Samt end),0), ";
                    strSql = strSql + " isnull(sum(Samt),0) , 0 from #Vx1 union all ";
                    strSql = strSql + " select 'Turnover', 0, isnull(sum(case td_flag when 'N' then bamt+Samt else 0 end),0), ";
                    strSql = strSql + " 0, isnull(sum(case td_flag when 'N' then 0 else bamt+Samt end),0),";
                    strSql = strSql + " isnull(sum(bamt+Samt),0) ,0 from #Vx1 union all ";  // isnull(sum(samt-bamt),0)
                    strSql = strSql + " select 'Stock', isnull(sum(nqty),0), isnull(sum(nqty*mktrate),0),0, 0, 0 , isnull(sum(nqty*mktrate),0) from #Vx1 where nqty >0 union all";
                    strSql = strSql + " select 'ExcSale', isnull(sum((-1)*nqty),0), isnull(sum((-1)*nqty*mktrate),0), 0, 0, 0 , isnull(sum(-nqty*mktrate),0) from #Vx1 where nqty<0 union all ";
                    strSql = strSql + " select 'Income', 0,0,0,0,0,isnull(sum((-1)*ic_amount),0) from #invcharges where ic_amount<0"; // union all "
                                                                                                                                      // strSql = strSql & " select 'Expense', 0,0,0,0,0,isnull(sum((-1)*ic_amount),0) from #invcharges where ic_amount>0 "
                    DataTable dsProcess = new DataTable();
                    dsProcess = mylib.OpenDataTable(strSql, curCon);
                    int i;
                    var loopTo1 = dsProcess.Rows.Count - 1;
                    for (i = 0; i <= loopTo1; i++)
                    {
                        dsProcess.Rows[i]["Delivery"] = Convert.ToDecimal(dsProcess.Rows[i]["Delivery"]).ToString("0.00");
                        dsProcess.Rows[i]["Squaredoff"] = Convert.ToDecimal(dsProcess.Rows[i]["Squaredoff"]).ToString("0.00");
                        dsProcess.Rows[i]["Total"] = Convert.ToDecimal(dsProcess.Rows[i]["Total"]).ToString("0.00");
                        dsProcess.Rows[i]["Net"] = Convert.ToDecimal(dsProcess.Rows[i]["Net"]).ToString("0.00");
                    }

                    double dblNetPL;
                    // For i = 0 To dsProcess.Tables(0).Rows.Count - 1
                    dblNetPL = Convert.ToDouble(dsProcess.Rows[2]["net"]) - Convert.ToDouble(dsProcess.Rows[8]["net"]) + Convert.ToDouble(dsProcess.Rows[9]["net"]) - Convert.ToDouble(dsProcess.Rows[3]["net"]);

                    if (dsProcess.Rows.Count > 0)
                    {
                        dtIBRC = dsProcess;
                        HttpContext.Current.Session["DataStlGrid"] = dsProcess;
                    }

                    strSql = "drop table #Vx1";
                    mylib.ExecSQL(strSql, curCon);
                    strSql = "drop table #invcharges";
                    mylib.ExecSQL(strSql, curCon);
                }
                else
                {

                }




            }

            return dtIBRC;
        }
        public DataTable getInvestorBasedReportCashDetail(string Code, string Cname, string FromDate, string ToDate, string ScripCd, string ScripName)
        {
            DataTable Detail = new DataTable();

            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strsql = "";

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                strsql = "select convert(char(10),convert(datetime,td_dt),103) 'Date', td_stlmnt 'Stlmnt', sum(td_bqty) BuyQty, cast(case when sum(td_bqty) > 0 then sum(td_bqty*td_rate)/ sum(td_bqty) else 0 end as decimal(15,2)) BuyAvgRate, convert(numeric(15,2),sum(td_bqty*td_rate)) BuyAmt, ";
                strsql = strsql + " sum(td_sqty) SaleQty, cast(case when sum(td_sqty) > 0 Then sum(td_sqty*td_rate)/ sum(td_sqty) else 0 end as decimal(15,2)) SellAvgRate ,convert(numeric(15,2),sum(td_sqty*td_rate)) SaleAmt, sum(td_bqty-td_sqty) Nqty,";
                strsql = strsql + " convert(numeric(15,2),sum((td_sqty-td_bqty)*td_rate)) NetAmt ";
                strsql = strsql + " from trx With(index(idx_trx_dt_clientcd)) ,client_master, securities ";
                strsql = strsql + " where td_clientcd=cm_cd and td_scripcd=ss_cd and cm_cd='" + Code + "' and ss_cd='" + ScripCd + "' ";
                strsql = strsql + " and td_dt between '" + FromDate + "' and '" + ToDate + "' ";
                strsql = strsql + " group by  cm_cd, cm_name, ss_cd, ss_name,td_dt, td_stlmnt order by td_dt ";


                Detail = mylib.OpenDataTable(strsql, curCon);






            }

            return Detail;
        }

        public DataTable getInvestorBasedReportCom(string Code, string Cname, string cmbSelect, string FromDt, string ToDt, string cmbExchSeg, string openopt, string ChkAvgRate, string ChkBuySellValue, string chkconsider)
        {


            DataTable dt = new DataTable();
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strLastDate = "";
            string strCondition = "";

            string strClientWhere = "";

            string strCompanyWhere;

            var dsHoliday = new DataSet();
            var ObjDataSet = new DataSet();

            int strBillstDt;
            int strBillenDt;
            string strSQL = "";
            SqlConnection ObjCommexCon;

            ObjCommexCon = mydbutil.commexTemp_conn("Commex");

            if (ObjCommexCon.State == ConnectionState.Closed)
            {
                ObjCommexCon.Open();

            }


            FromDt = myutil.dtos(FromDt);
            ToDt = myutil.dtos(ToDt);


            //string ExCommex = "";
            //string Exchng = "";
            //string[] strArray = cmbExchSeg.Split(',');
            //bool first = true;
            //foreach (string obj in strArray)
            //{
            //    if (obj.Length >= 2)
            //    {
            //        if (first)
            //        {
            //            Exchng = obj;
            //            first = false;
            //        }
            //        else
            //        {
            //            Exchng = obj + "," + Exchng;
            //        }
            //    }

            //    //your insert query
            //}
            //bool Cofirst = true;
            //foreach (string obj in strArray)
            //{
            //    if ((string)HttpContext.Current.Session["IsTplusCommex"] == "Y")
            //    {
            //        if (obj.Substring(1, 1) == "X")
            //        {
            //            if (Cofirst)
            //            {
            //                ExCommex = obj;
            //                Cofirst = false;
            //            }
            //            else
            //            {
            //                ExCommex = obj + "," + ExCommex;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (obj.Length < 2)
            //        {
            //            if (Cofirst)
            //            {
            //                ExCommex = obj;
            //                Cofirst = false;
            //            }
            //            else
            //            {
            //                ExCommex = obj + "," + ExCommex;
            //            }
            //        }

            //    }


            //    //your insert query
            //}



            if (Code != "")
            {
                strCondition = " and cm_cd = '" + Code + "'";
                strClientWhere = strCondition;

            }

            mylib.ExecSQL("drop table #Finvdates", ObjCommexCon);

            strCompanyWhere = " and ltrim(rtrim(left(td_exchange,1))) = '" + cmbExchSeg + "'";

            strSQL = "CREATE TABLE [#Finvdates] (";
            strSQL = strSQL + "[bd_dt] [char] (8) NOT NULL ";
            strSQL = strSQL + ")";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            strBillstDt = Convert.ToInt32(FromDt);
            strBillenDt = Convert.ToInt32(ToDt);



            while (strBillstDt <= strBillenDt)
            {

                strSQL = "select count(*) cnt from Fholiday_master with (nolock) where hm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'" + myutil.newline();
                strSQL += " and hm_exchange = '" + Strings.Left(cmbExchSeg, 1) + "'";
                strSQL += " and hm_dt = '" + strBillstDt + "'";
                dt = mylib.OpenDataTable(strSQL, ObjCommexCon);

                if (Convert.ToInt16(dt.Rows[0]["cnt"]) == 0)
                {
                    strSQL = "insert into #finvdates values('" + strBillstDt + "')";

                    mylib.ExecSQL(strSQL, ObjCommexCon);
                }
                dt.Dispose();

                strBillstDt = Convert.ToInt32(myutil.dtos((myutil.AddDayDT(strBillstDt.ToString(), 1)).ToString("dd/MM/yyyy")));
            }
            mylib.ExecSQL("drop table #tmpfinvcharges", ObjCommexCon);
            strSQL = "CREATE TABLE [dbo].[#tmpfinvcharges] (";
            strSQL = strSQL + "[bc_dt] [char] (8) NOT NULL,";
            strSQL = strSQL + "[bc_clientcd] [char] (8) NOT NULL,";
            strSQL = strSQL + "[bc_desc] [char] (40) NOT NULL,";
            strSQL = strSQL + "[bc_amount] [money] NOT NULL,";
            strSQL = strSQL + "[bc_billno] [numeric] NOT NULL";
            strSQL = strSQL + ")";

            mylib.ExecSQL(strSQL, ObjCommexCon);
            mylib.ExecSQL("drop table #tmpfinvestorrep", ObjCommexCon);
            strSQL = "Create table #tmpfinvestorrep(";
            strSQL = strSQL + " fi_dt char(8) not null,";
            strSQL = strSQL + " fi_clientcd char(8) not null,";
            strSQL = strSQL + " fi_exchange char(1) not null,";
            strSQL = strSQL + " fi_seriesid numeric not null,";
            strSQL = strSQL + " fi_bqty numeric not null,";
            strSQL = strSQL + " fi_bvalue money not null,";
            strSQL = strSQL + " fi_sqty numeric not null,";
            strSQL = strSQL + " fi_svalue money not null,";
            strSQL = strSQL + " fi_netqty numeric not null,";
            strSQL = strSQL + " fi_netvalue money not null,";
            strSQL = strSQL + " fi_rate money not null,";
            strSQL = strSQL + " fi_closeprice money not null,";
            strSQL = strSQL + " fi_mtm money not null,";
            strSQL = strSQL + " fi_listorder numeric not null,";
            strSQL = strSQL + " fi_controlflag numeric not null,";
            strSQL = strSQL + " fi_prodtype char(2) not null,";
            strSQL = strSQL + " fi_type char(1) not null,";
            strSQL = strSQL + " fi_balfield char(1) not null,";
            strSQL = strSQL + " fi_Days Numeric not null,";
            strSQL = strSQL + " fi_OSeriesid Numeric not null)";

            mylib.ExecSQL(strSQL, ObjCommexCon);

            strSQL = "select isnull(min(bd_dt),'" + FromDt + "'),isnull(max(bd_dt),'" + FromDt + "') from #Finvdates";
            dt = mylib.OpenDataTable(strSQL, ObjCommexCon);

            if (dt.Rows.Count != 0)
            {
                strLastDate = Convert.ToString(dt.Rows[0][1]);
            }

            //SPOT Exchanges
            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,td_clientcd,td_exchange,";
            strSQL += " td_seriesid,td_bqty,td_bqty*td_Rate,case When td_orderdt > bd_dt Then 0 else td_sqty end,td_sqty*td_Rate,0,0,";
            strSQL += " 0,0.0000 td_closeprice ,(td_sqty-td_bqty)*td_Rate mtm,";
            strSQL += " 6 td_sortlist,";
            strSQL += " 2,sm_prodtype,'N','Y',0,0";
            strSQL += " From Trades, #Finvdates, Series_master,Client_master";
            strSQL += " Where td_clientcd = cm_cd and td_exchange = sm_exchange and td_seriesid = sm_seriesid";
            strSQL += " and sm_expirydt >= bd_dt and  td_dt = bd_dt ";
            strSQL += " and td_exchange in ('S','D') " + strCompanyWhere;
            strSQL += strCondition;
            mylib.ExecSQL(strSQL, ObjCommexCon);


            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,td_clientcd,td_exchange,";
            strSQL += " td_seriesid,0,td_sqty*td_Rate,0,td_bqty*td_marketrate,0,0,";
            strSQL += " 0,0.0000 td_closeprice ,(td_bqty-td_sqty)*td_marketrate mtm,";
            strSQL += " 6 td_sortlist,";
            strSQL += " 2,sm_prodtype,'N','Y',0,0";
            strSQL += " From Trades, #Finvdates, Series_master,Client_master";
            strSQL += " Where td_clientcd = cm_cd and td_exchange = sm_exchange and td_seriesid = sm_seriesid";
            strSQL += " and td_dt = bd_dt and td_orderdt <> bd_dt ";
            strSQL += " and td_exchange in ('S','D') " + strCompanyWhere;
            strSQL += strCondition;


            mylib.ExecSQL(strSQL, ObjCommexCon);

            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,td_clientcd,td_exchange,";
            strSQL += " td_seriesid,td_bqty,td_bqty*td_Rate,td_sqty,td_sqty*td_marketrate,0,0,";
            strSQL += " 0,0.0000 td_closeprice ,(td_sqty-td_bqty)*td_marketrate mtm,";
            strSQL += " 6 td_sortlist,";
            strSQL += " 2,sm_prodtype,'N','Y',0,0";
            strSQL += " From Trades, #Finvdates, Series_master,Client_master";
            strSQL += " Where td_clientcd = cm_cd and td_exchange = sm_exchange and td_seriesid = sm_seriesid";
            strSQL += " and td_dt <> bd_dt and td_orderdt = bd_dt ";
            strSQL += " and td_exchange in ('S','D') " + strCompanyWhere;
            strSQL += strCondition;
            mylib.ExecSQL(strSQL, ObjCommexCon);

            //SPOT Exchanges
            //Normal Exchanges
            //Futures opening

            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select '" + FromDt + "',td_clientcd,td_exchange,";
            strSQL += " td_seriesid,case sign(sum(td_bqty - td_sqty)) when 1 then abs(sum(td_bqty - td_sqty)) else 0 end  td_bqty,";
            strSQL += " 0,";
            strSQL += " case sign(sum(td_bqty - td_sqty)) when 1 then 0 else abs(sum(td_bqty - td_sqty)) end td_sqty,0,";
            strSQL += " 0,0,0,0 td_closeprice  ,0,";
            strSQL += " case sm_prodtype when 'CF' then 1 when 'CO' then 3 else 4 end td_sortlist,";
            strSQL += " 1 td_controlflag,sm_prodtype,'N','O',0,0";
            strSQL += " From Trades,Series_master,Client_master";
            strSQL += " Where td_clientcd = cm_cd and td_exchange = sm_exchange And td_seriesid = sm_seriesid";
            strSQL += " and sm_expirydt >= '" + FromDt + "' and  td_dt < '" + FromDt + "' and sm_prodtype='CF' and ltrim(rtrim(td_groupid)) <>'B'";
            strSQL += " and td_exchange not in ('S','D') " + strCompanyWhere;
            strSQL += strCondition;
            strSQL += " group by td_exchange,td_clientcd,td_seriesid,sm_prodtype";
            strSQL += " having sum(td_bqty - td_sqty) <> 0";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            //  'Future opening for each day
            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,td_clientcd,td_exchange,";
            strSQL += " td_seriesid,case sign(sum(td_bqty - td_sqty)) when 1 then abs(sum(td_bqty - td_sqty)) else 0 end  td_bqty,";
            strSQL += " 0,";
            strSQL += " case sign(sum(td_bqty - td_sqty)) when 1 then 0 else abs(sum(td_bqty - td_sqty)) end td_sqty,0,";
            strSQL += " 0,0,0,0 td_closeprice,0,";
            strSQL += " case sm_prodtype when 'CF' then 1 when 'CO' then 3 else 4 end td_sortlist,";
            strSQL += " 1 td_controlflag,sm_prodtype,'N','N',0,0";
            strSQL += " From Trades, #Finvdates, Series_master,Client_master";
            strSQL += " Where td_clientcd = cm_cd and td_exchange = sm_exchange And td_seriesid = sm_seriesid";
            strSQL += " and sm_expirydt >= bd_dt and  td_dt < bd_dt and sm_prodtype='CF' and ltrim(rtrim(td_groupid)) <>'B'";
            strSQL += " and td_exchange not in ('S','D') " + strCompanyWhere;
            strSQL += strCondition;
            strSQL += " and bd_dt <> '" + FromDt + "'";
            strSQL += " group by td_exchange,bd_dt,td_clientcd,td_seriesid,sm_prodtype";
            strSQL += " having sum(td_bqty - td_sqty) <> 0";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            if (chkconsider == "1")
            {
                //Opening for Options

                strSQL = "insert into #tmpfinvestorrep";
                strSQL += " select '" + FromDt + "',td_clientcd,td_exchange,";
                strSQL += " td_seriesid,case sale when 0 then buy else 0 end  td_bqty,";
                strSQL += " 0,";
                strSQL += " case sale when 0 then 0 else sale end td_sqty,0,";
                strSQL += " 0,0,td_rate,0 td_closeprice  ,0,";
                strSQL += " case sm_prodtype when 'CF' then 1 when 'CO' then 3 else 4 end td_sortlist,";
                strSQL += " 1 td_controlflag,sm_prodtype,'N','O',0,0";
                strSQL += " From vwFoutstandingpos";
                strSQL += " Where sm_expirydt >= '" + FromDt + "' and  td_dt < '" + FromDt + "'";
                strSQL += " and td_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'" + strCompanyWhere;
                strSQL += " and sm_prodtype in('CO','EO')";
                strSQL += strCondition;
                strSQL += " and cm_type = 'C'";
                mylib.ExecSQL(strSQL, ObjCommexCon);

            }
            // 'Futures/Options running

            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,td_clientcd,td_exchange,";
            strSQL += " td_seriesid,td_bqty,0,td_sqty,0,0,0,";
            strSQL += " td_rate  ,0.0000 td_closeprice ,0 mtm,";
            strSQL += " case sm_prodtype when 'CF' then 1 when 'CO' then 3 else 4 end td_sortlist,";
            strSQL += " 2,sm_prodtype,'N','Y',0,0";
            strSQL += " From Trades, #Finvdates, Series_master,Client_master";
            strSQL += " Where td_clientcd = cm_cd and td_exchange = sm_exchange and td_seriesid = sm_seriesid";
            strSQL += " and sm_expirydt >= bd_dt and  td_dt = bd_dt ";
            strSQL += " and td_exchange not in ('S','D') " + strCompanyWhere;
            strSQL += strCondition;
            mylib.ExecSQL(strSQL, ObjCommexCon);


            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,td_clientcd,td_exchange,";
            strSQL += " td_seriesid,td_sqty,0,td_bqty,0,0,0,";
            strSQL += " td_MarketRate  ,0.0000 td_closeprice ,0 mtm,";
            strSQL += " case sm_prodtype when 'CF' then 1 when 'CO' then 3 else 4 end td_sortlist,";
            strSQL += " 2,sm_prodtype,'N','Y',0,0";
            strSQL += " From Trades, #Finvdates, Series_master,Client_master";
            strSQL += " Where td_clientcd = cm_cd and td_exchange = sm_exchange and td_seriesid = sm_seriesid";
            strSQL += " and sm_expirydt >= bd_dt and  td_dt = bd_dt and sm_prodtype='CF' and ltrim(rtrim(td_groupid)) ='B' ";
            strSQL += " and td_exchange not in ('S','D') " + strCompanyWhere;
            strSQL += strCondition;

            mylib.ExecSQL(strSQL, ObjCommexCon);

            //Exercise/Assignments
            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,ex_clientcd,ex_exchange,";
            strSQL += " ex_seriesid,ex_aqty,0,ex_eqty,0,0,0,";
            strSQL += " ex_diffBrokrate,ex_settlerate,0,";
            strSQL += " case sm_prodtype when 'CF' then 1 when 'EF' then 2 when 'CO' then 3 else 4 end + 5 td_sortlist,";
            strSQL += " case ex_eaflag when 'E' then 4 else 3 end td_controlflag,sm_prodtype,'N','Y',0,0";
            strSQL += " From Exercise, #Finvdates, Series_master,Client_master";
            strSQL += " Where ex_clientcd = cm_cd and ex_exchange = sm_exchange And ex_seriesid = sm_seriesid";
            strSQL += " and  ex_dt = bd_dt and ex_companycode = ''" + Strings.Replace(strCompanyWhere, "td_", "ex_");
            strSQL += strCondition;
            mylib.ExecSQL(strSQL, ObjCommexCon);

            //  'Reverse Future Outstanding
            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select '" + strLastDate + "',fi_clientcd,fi_exchange,";
            strSQL += " fi_seriesid,case sign(sum(case fi_controlflag when 1 then case fi_dt when '" + FromDt + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) when -1 then abs(sum(case fi_controlflag when 1 then case fi_dt when '" + FromDt + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) else 0 end td_bqty,";
            strSQL += " 0,";
            strSQL += " case sign(sum(case fi_controlflag when 1 then case fi_dt when '" + FromDt + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) when 1 then abs(sum(case fi_controlflag when 1 then case fi_dt when '" + FromDt + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end)) else 0 end td_sqty,0,";
            strSQL += " 0,0,0,0 td_closeprice,0,";
            strSQL += " case fi_prodtype when 'CF' then 1 when 'EF' then 2 when 'CO' then 3 else 4 end + 6 td_sortlist,";
            strSQL += " 5 td_controlflag,fi_prodtype,'R','N',0,0";
            strSQL += " From #tmpfinvestorrep,Series_master";
            strSQL += " Where fi_exchange = sm_exchange and sm_seriesid = fi_seriesid and fi_prodtype in('CF','EF') ";
            strSQL += " and sm_expirydt <= '" + ToDt + "'";
            strSQL += " group by fi_exchange,fi_clientcd,fi_seriesid,fi_prodtype";
            strSQL += " having sum(case fi_controlflag when 1 then case fi_dt when '" + FromDt + "' then fi_bqty - fi_sqty else 0 end when 2 then fi_bqty - fi_sqty else fi_sqty - fi_bqty end) <> 0";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            //  'Update Last Market Price for Options

            strSQL = "update #tmpfinvestorrep set fi_rate = fi_rate,fi_closeprice = case fi_type when 'R' then fi_closeprice else ms_lastprice end from #tmpfinvestorrep,Market_summary";
            strSQL += " where ms_seriesid = fi_seriesid ";
            strSQL += " and ms_exchange = fi_exchange";
            strSQL += " and ms_dt = (select max(ms_dt) from Market_summary where ms_exchange = fi_exchange";
            strSQL += " and ms_seriesid = fi_seriesid and ms_lastprice <> 0)";
            strSQL += " and fi_prodtype in('CO','EO')";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            // 'Update Previous close and today's close prices

            strSQL = "update #tmpfinvestorrep set fi_closeprice = ms_lastprice from #tmpfinvestorrep,Market_summary";
            strSQL += " where ms_seriesid = fi_seriesid and fi_controlflag in('1','2')";
            strSQL += " and ms_exchange = fi_exchange ";
            strSQL += " and ms_dt = fi_dt";
            strSQL += " and fi_prodtype in('CF','EF')";
            mylib.ExecSQL(strSQL, ObjCommexCon);


            strSQL = "update #tmpfinvestorrep set fi_rate = ms_prcloseprice from #tmpfinvestorrep,Market_summary";
            strSQL += " where ms_seriesid = fi_seriesid and fi_controlflag = 1";
            strSQL += " and ms_exchange = fi_exchange";
            strSQL += " and ms_dt = fi_dt";
            strSQL += " and fi_prodtype in('CF','EF')";
            mylib.ExecSQL(strSQL, ObjCommexCon);
            // 'End of updation of close prices

            // 'Service tax here for Trades
            strSQL = "insert into #tmpfinvcharges select td_dt,td_clientcd,'SERVICE TAX',round(sum(td_servicetax),2),0 from Trades,#Finvdates,Client_master,Series_master";
            strSQL += " where td_clientcd = cm_cd and td_dt = bd_dt";
            strSQL += " and td_exchange = sm_exchange and td_seriesid = sm_seriesid";
            strSQL += strCondition + strCompanyWhere;
            strSQL += " group by td_dt,td_clientcd having sum(td_servicetax) <> 0";
            mylib.ExecSQL(strSQL, ObjCommexCon);



            // Service tax here for Trades
            strSQL = "insert into #tmpfinvcharges select dl_dt,dl_clientcd,'SERVICE TAX',round(sum(dl_servicetax),2),0 from Delivery,#Finvdates,Client_master,Series_master";
            strSQL += " where dl_clientcd = cm_cd and dl_BillDate = bd_dt";
            strSQL += " and dl_exchange = sm_exchange and dl_seriesid = sm_seriesid";
            strSQL += strCondition + Strings.Replace(strCompanyWhere, "td_", "dl_");
            strSQL += " group by dl_dt,dl_clientcd having sum(dl_servicetax) <> 0";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            //  'Service tax here for Exercise

            strSQL = "insert into #tmpfinvcharges select ex_dt,ex_clientcd,'SERVICE TAX',round(sum(ex_servicetax),2),0 from Exercise,#Finvdates,Client_master,Series_master";
            strSQL += " where ex_clientcd = cm_cd and ex_dt = bd_dt";
            strSQL += " and ex_exchange = sm_exchange and ex_seriesid = sm_seriesid";
            strSQL += Strings.Replace(strCondition, "td_", "ex_") + Strings.Replace(strCompanyWhere, "td_", "ex_");
            strSQL += " group by ex_dt,ex_clientcd having sum(ex_servicetax) <> 0";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            //  'Charges here
            //  '-----------from specialcharges start

            strSQL = "insert into #tmpfinvcharges select fc_dt,fc_clientcd,fc_desc,round(sum(fc_amount),2),0 from Fspecialcharges,#Finvdates,Client_master";
            strSQL += " where fc_clientcd = cm_cd and fc_dt = bd_dt";
            strSQL += strClientWhere + Strings.Replace(strCompanyWhere, "td_", "fc_");
            strSQL += " and fc_desc not like '%{New%' and fc_desc not like '%{Old%'";
            strSQL += " group by fc_dt,fc_clientcd,fc_desc having round(sum(fc_amount),2) <> 0";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            strSQL = "insert into #tmpfinvcharges select fc_dt,fc_clientcd,'SERVICE TAX',round(sum(fc_servicetax),2),0 from Fspecialcharges,#Finvdates,Client_master";
            strSQL += " where fc_clientcd = cm_cd and fc_dt = bd_dt";
            strSQL += strClientWhere + Strings.Replace(strCompanyWhere, "td_", "fc_");
            strSQL += " group by fc_dt,fc_clientcd,fc_desc having round(sum(fc_servicetax),2) <> 0";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            // '-----------from specialcharges end

            strSQL = "update #tmpfinvestorrep set fi_bvalue = fi_bqty*fi_rate,fi_svalue = fi_sqty*fi_rate,";
            strSQL += "fi_netqty = fi_bqty - fi_sqty,fi_netvalue = (fi_bqty - fi_sqty)*fi_rate";
            strSQL += " where fi_controlflag not in(3,4) and fi_Exchange not in ('S','D')";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            strSQL = "update #tmpfinvestorrep set fi_bvalue = fi_bqty*fi_rate,fi_svalue = fi_sqty*fi_rate,";
            strSQL += "fi_netqty = fi_sqty - fi_bqty,fi_netvalue = (fi_bqty + fi_sqty)*fi_rate";
            strSQL += " where fi_controlflag in(3,4) and fi_Exchange not in ('S','D')";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            strSQL = "update #tmpfinvestorrep set fi_mtm = round((((fi_sqty - fi_bqty)*fi_rate)  - ((fi_sqty - fi_bqty)*fi_closeprice)),4)";
            strSQL += " where fi_prodtype in('CF','EF')";

            mylib.ExecSQL(strSQL, ObjCommexCon);

            strSQL = "update #tmpfinvestorrep set fi_mtm = round(((case fi_controlflag when 3  then (fi_bqty + fi_sqty) when 4 then (fi_bqty + fi_sqty) else (fi_bqty - fi_sqty)*(-1) end) *fi_rate),4)";
            strSQL += " where fi_prodtype in('CO','EO')";
            mylib.ExecSQL(strSQL, ObjCommexCon);

            //Delivery

            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,dl_clientcd,dl_exchange,";
            strSQL += " dl_seriesid,case dl_type when 'DL' Then dl_bqty else 0 end,case dl_type when 'DL' Then dl_bqty*dl_rate else 0 end,case dl_type when 'DL' Then dl_sqty else 0 end ,case dl_type when 'DL' Then dl_sqty*dl_rate else 0 end,case dl_type when 'DL' Then (dl_sqty-dl_bqty) else 0 end,case dl_type when 'DL' Then (dl_sqty-dl_bqty)*dl_rate else 0 end,";
            strSQL += " case dl_type when 'DL' Then dl_rate else 0 end,case dl_type when 'DL' Then dl_rate else 0 end, case dl_type when 'DL' Then  (dl_sqty-dl_bqty)*dl_rate else 0 end,";
            strSQL += " 6 td_sortlist,";
            strSQL += " 2 td_controlflag,sm_prodtype,'N','Y',0,0";
            strSQL += " From Delivery, #Finvdates, Series_master,Client_master";
            strSQL += " Where dl_clientcd = cm_cd and dl_exchange = sm_exchange And dl_seriesid = sm_seriesid";
            strSQL += " and dl_Billdate = bd_dt and dl_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'" + Strings.Replace(strCompanyWhere, "td_", "dl_");
            strSQL += " and dl_exchange not in ('S','D') " + Strings.Replace(strCompanyWhere, "td_", "dl_");
            strSQL += " and dl_type In ('DL','SL','PD','DS') ";
            strSQL += strCondition;
            mylib.ExecSQL(strSQL, ObjCommexCon);


            strSQL = "insert into #tmpfinvestorrep";
            strSQL += " select bd_dt,dl_clientcd,dl_exchange,";
            strSQL += " dl_seriesid,0,0,0,0,0,0,";
            strSQL += " 0,0, (dl_sqty-dl_bqty)*dl_rate,";
            strSQL += " 7 td_sortlist,";
            strSQL += " 2 td_controlflag,sm_prodtype,'N','Y',0,0";
            strSQL += " From Delivery, #Finvdates, Series_master,Client_master";
            strSQL += " Where dl_clientcd = cm_cd and dl_exchange = sm_exchange And dl_seriesid = sm_seriesid";
            strSQL += " and dl_Billdate = bd_dt and dl_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'" + Strings.Replace(strCompanyWhere, "td_", "dl_");
            strSQL += " and dl_exchange not in ('S','D') " + Strings.Replace(strCompanyWhere, "td_", "dl_");
            strSQL += " and dl_type In ('SL','PD','DS') ";
            strSQL += strCondition;
            mylib.ExecSQL(strSQL, ObjCommexCon);


            strSQL = "select fi_clientcd,cm_name,cm_email,sm_seriesid,sm_symbol,sm_sname,Rtrim(sm_desc) + case fi_listorder When 6 Then '-DLV' When '7' Then '-DLV' else '' end sm_desc ,sm_multiplier,cm_brboffcode, ";
            strSQL = strSQL + " case sm_prodtype when 'CF' then 1 when 'EF' then 2 when 'CO' then 3 else 4 end listorder,sm_expirydt,";
            strSQL = strSQL + " sum(case fi_balfield when 'O' then fi_bqty - fi_sqty else 0 end) opnqty,";
            strSQL = strSQL + " sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_bqty else 0 end else 0 end) buyqty,";
            strSQL = strSQL + " sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_bvalue else 0 end else 0 end)* sm_multiplier buyvalue,";
            strSQL = strSQL + " Case When sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_bqty else 0 end else 0 end) > 0 Then ";
            strSQL = strSQL + " Convert(Money,(sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_bvalue else 0 end else 0 end)/sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_bqty else 0 end else 0 end))) Else 0 End buyRate,";
            strSQL = strSQL + " sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_sqty else 0 end else 0 end) sellqty,";
            strSQL = strSQL + " sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_svalue else 0 end else 0 end)* sm_multiplier sellvalue,";
            strSQL = strSQL + " Case When sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_sqty else 0 end else 0 end) > 0 Then ";
            strSQL = strSQL + " Convert(Money,(sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_svalue else 0 end else 0 end)/sum(case fi_type when 'N' then case fi_controlflag when 2 then fi_sqty else 0 end else 0 end))) Else 0 End sellRate, ";
            strSQL = strSQL + " sum(case fi_type when 'N' then case fi_controlflag when 3 then abs(fi_bqty) else 0 end else 0 end) exerqty,";
            strSQL = strSQL + " sum(case fi_type when 'N' then case fi_controlflag when 4 then abs(fi_sqty) else 0 end else 0 end) assgnqty,";
            strSQL = strSQL + " sum(case when sm_expirydt <= '" + FromDt + "' then 0 else case fi_balfield when 'N' then  0 else (fi_bqty)*case fi_controlflag when 3 then -1 else 1 end  - (fi_sqty)*case fi_controlflag when 4 then -1 else 1 end end end) outqty,";
            strSQL = strSQL + " Case When sum(case when sm_expirydt <= '" + FromDt + "' then 0 else case fi_balfield when 'N' then  0 else (fi_bqty)*case fi_controlflag when 3 then -1 else 1 end  - (fi_sqty)*case fi_controlflag when 4 then -1 else 1 end end end) > 0 Then ";
            strSQL = strSQL + " Convert(Money,(sum(case fi_type when 'R' then 0 else fi_netvalue end) /sum(case when sm_expirydt <= '" + FromDt + "' then 0 else case fi_balfield when 'N' then  0 else (fi_bqty)*case fi_controlflag when 3 then -1 else 1 end  - (fi_sqty)*case fi_controlflag when 4 then -1 else 1 end end end))) Else 0 End outvalue, ";
            strSQL = strSQL + " sum(case fi_type when 'R' then  fi_sqty - fi_bqty else 0 end) closeqty,";
            strSQL = strSQL + " sum(fi_netvalue) netvalue,";
            strSQL = strSQL + " sum(case fi_type when 'R' then 0 else fi_netvalue end) pnetvalue,";
            strSQL = strSQL + " sum(fi_mtm * sm_multiplier) mtm,0 fi_closeprice ";
            strSQL = strSQL + " from #tmpfinvestorrep a,Client_master,Series_master";
            strSQL = strSQL + " where fi_clientcd = cm_cd ";
            strSQL = strSQL + " and fi_exchange = sm_exchange and fi_seriesid = sm_seriesid ";
            strSQL = strSQL + " group by fi_clientcd,cm_name,sm_seriesid,sm_prodtype,sm_symbol,sm_sname,Rtrim(sm_desc)+case fi_listorder When 6 Then '-DLV' When '7' Then '-DLV' else '' end,sm_multiplier,cm_brboffcode,cm_email,sm_expirydt,";
            strSQL = strSQL + " case sm_prodtype when 'CF' then 1 when 'EF' then 2 when 'CO' then 3 else 4 end,fi_listorder ";
            strSQL = strSQL + " order by fi_clientcd,cm_name,case fi_listorder   When 6 Then 1 When 7 Then 2 else 0 end,sm_symbol,sm_expirydt";

            dt = mylib.OpenDataTable(strSQL, ObjCommexCon);

            strSQL = "select bc_desc,sum(bc_amount*(-1))  chrg from #tmpFinvcharges group by bc_desc ";
            DataTable dtcharges = new DataTable();
            dtcharges = mylib.OpenDataTable(strSQL, ObjCommexCon);
            HttpContext.Current.Session["dtcharges"] = dtcharges;

            return dt;
        }
        public List<DDLSettlementSearch> PopulateDDLSettlementSearch()
        {
            LibraryModel mylib = new LibraryModel();
            List<DDLSettlementSearch> items = new List<DDLSettlementSearch>();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {

                curCon.Open();
                string query = "select distinct CES_Exchange, substring(CES_cd,2,1) Ces_cd from CompanyExchangeSegments where substring(CES_cd,3,1) = 'C'";
                DataTable dtSettlementSearch = mylib.OpenDataTable(query, curCon);
                if (dtSettlementSearch.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSettlementSearch.Rows.Count; i++)
                    {
                        items.Add(new DDLSettlementSearch
                        {
                            Name = dtSettlementSearch.Rows[i]["CES_Exchange"].ToString(),
                            Id = dtSettlementSearch.Rows[i]["Ces_cd"].ToString()
                        });
                    }
                }

            }

            return items;
        }
        public List<DDLSettlementSearch> PopulateDDLSettlementSearchcmbType(string cmbExch)
        {
            LibraryModel mylib = new LibraryModel();
            List<DDLSettlementSearch> items = new List<DDLSettlementSearch>();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {

                curCon.Open();
                string query = "select left(sy_desc,1) code ,substring(sy_desc,patindex('%-%',sy_desc)+ Case sy_exchange when 'M' then '1' else '2' end ,len(sy_desc)) name  from Settlement_type where sy_exchange = '" + cmbExch + "' order by sy_srno";
                DataTable dtSettlementSearch = mylib.OpenDataTable(query, curCon);
                if (dtSettlementSearch.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSettlementSearch.Rows.Count; i++)
                    {
                        items.Add(new DDLSettlementSearch
                        {
                            Name = dtSettlementSearch.Rows[i]["name"].ToString(),
                            Id = dtSettlementSearch.Rows[i]["code"].ToString()
                        });
                    }
                }

            }

            return items;
        }

        public DataTable PayInStatusReport(string code, string StlMnt, string Select, string Exchange, string SettType, string ReportType, string Order)
        {
            string strOpt = "";
            string strWhere = "";
            string strParameter = "";
            string strPayinDt = "";
            UtilityModel myutil = new UtilityModel();
            LibraryModel lib = new LibraryModel();

            DataTable dt = null;
            if (SettType == "1")
            {
                if (Exchange == "1")
                { strOpt = "YY"; }

                else
                { strOpt = "YN"; }
            }

            else
            {
                if (Exchange == "1")
                {
                    strOpt = "NY";
                }
                else
                {
                    strOpt = "NN";
                }

            }

            if (Exchange == "1")
            {
                if (SettType == "1")
                {
                    HttpContext.Current.Session["strTradeTitle"] = "Pay-In Status Report for Pay-In date : " + strPayinDt;
                }
                else
                {
                    if (StlMnt.Substring(1, 1).ToString() == "N" || StlMnt.Substring(1, 1).ToString() == "W")
                    {
                        HttpContext.Current.Session["strTradeTitle"] = "Pay-In Status Report for Normal settlement of Pay-In date : " + strPayinDt;
                    }
                    else
                    {
                        HttpContext.Current.Session["strTradeTitle"] = "Pay-In Status Report for T2T settlement of Pay-In date : " + strPayinDt;
                    }
                }
            }
            else
            {
                if (SettType == "1")
                {
                    HttpContext.Current.Session["strTradeTitle"] = "Pay-In Status Report for Settlement : " + Strings.Left(StlMnt, 1) + "/" + Strings.Right(StlMnt, Strings.Len(StlMnt) - 2);
                }
                else
                {
                    HttpContext.Current.Session["strTradeTitle"] = "Pay-In Status Report for Settlement : " + StlMnt;
                }
            }

            if (Select == "CL")
            {
                strWhere = HttpContext.Current.Session["LoginAccessOld"].ToString();
                strWhere = myutil.RPlace(strWhere, "'", "|");
            }
            else
            {
                strWhere = " and cm_cd = ''" + Select + "''";
            }

            strParameter = "'" + StlMnt + "','" + strOpt; ;
            strParameter = strParameter + "','" + (String.IsNullOrEmpty((string)HttpContext.Current.Session["CrossAlias"]) == true ? "" : HttpContext.Current.Session["CrossAlias"]) + "','";
            strParameter = strParameter + (String.IsNullOrEmpty((string)HttpContext.Current.Session["EstroAlias"]) == true ? "" : HttpContext.Current.Session["EstroAlias"]);
            strParameter = strParameter + "','" + strWhere + "'";
            dt = lib.OpenDataTable("exec sp_DPLinkHolding " + strParameter);


            return dt;
        }

        //public DataTable GetDailyConfirmationReport(string clientcode, string Date, string Type)
        //{
        //    string strsql = string.Empty;
        //    string Strsql = string.Empty;
        //    UtilityDBModel mydbutil = new UtilityDBModel();
        //    UtilityModel myutil = new UtilityModel();
        //    LibraryModel lib = new LibraryModel();
        //    //string clientcode = "10105";
        //    //Date = "01/01/2021";
        //    Type = "Cumulative1";
        //    DataTable dt = null;
        //    string StrTradesIndex = "index(idx_trades_clientcd),";
        //    string StrTRXIndex = "index(idx_trx_clientcd)";
        //    string StrComTradesIndex = "";

        //    SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
        //    if (Type == "Cumulative")
        //    {


        //        //if (ConfigurationManager.AppSettings["IsTradeWeb"] == "O")
        //        //{
        //        //If IsTradeWeb is false means it will connect to TradePlus Database(Live)
        //        strsql = " Select 1 as td_order,'Equity : ' + td_stlmnt as td_type, td_stlmnt,td_scripcd,replace(rtrim(ss_Name)+' ('+td_scripcd+')','&','') td_scripnm, ";
        //        strsql = strsql + " sum(td_bqty) 'bqty',sum(convert(decimal(15,2),convert(money,td_bqty * td_rate))) 'bvalue', sum(td_sqty) 'sqty', ";
        //        strsql = strsql + " sum(convert(decimal(15,4),convert(money,td_sqty * td_rate))) 'svalue', sum(td_bqty - td_sqty) 'netqty', ";
        //        strsql = strsql + " sum(convert(decimal(15,4),convert(money,(td_sqty - td_bqty)*td_rate))) 'netvalue', ";
        //        strsql = strsql + " cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0 else sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty) ";
        //        strsql = strsql + " end) as decimal(15,2)) 'average', ";
        //        strsql = strsql + " cast(sum(td_brokerage*(td_bqty+td_sqty)) as decimal(15,4)) td_Brokerage, ";
        //        strsql = strsql + " replace(td_stlmnt+td_dt+td_scripcd,'&','-') 'td_Lookup' ";
        //        strsql = strsql + " from trx with(" + StrTRXIndex + "nolock) , securities with (nolock), Settlements with (nolock) ";
        //        strsql = strsql + " where td_clientcd= '" + clientcode + "'  and td_stlmnt = se_stlmnt and td_scripcd = ss_cd and td_dt='" + Date + "' ";
        //        strsql = strsql + " group by 'Equity : ' + td_stlmnt ,td_stlmnt,td_scripcd,ss_Name ,td_dt";
        //        //}
        //        //else
        //        //{
        //        //    //If IsTradeWeb is true means it will connect to TradeWeb Database
        //        //    strsql = " Select 1 as td_order,'Equity : ' + td_stlmnt as td_type, td_stlmnt,td_scripcd,replace(rtrim(td_scripnm)+' ('+td_scripcd+')','&','') td_scripnm, ";
        //        //    strsql = strsql + " sum(td_bqty) 'bqty',sum(convert(decimal(15,2),convert(money,td_bqty * td_rate))) 'bvalue', sum(td_sqty) 'sqty', ";
        //        //    strsql = strsql + " sum(convert(decimal(15,4),convert(money,td_sqty * td_rate))) 'svalue', sum(td_bqty - td_sqty) 'netqty', ";
        //        //    strsql = strsql + " sum(convert(decimal(15,4),convert(money,(td_sqty - td_bqty)*td_rate))) 'netvalue', ";
        //        //    strsql = strsql + " cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0 else sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty) ";
        //        //    strsql = strsql + " end) as decimal(15,2)) 'average', ";
        //        //    strsql = strsql + " cast(sum(td_brokerage*(td_bqty+td_sqty)) as decimal(15,4)) td_Brokerage, ";
        //        //    strsql = strsql + " replace(td_stlmnt+td_dt+td_scripcd,'&','-') 'td_Lookup' ";
        //        //    strsql = strsql + " from trx with(" + StrTRXIndex + "nolock), Settlements with (nolock) ";
        //        //    strsql = strsql + " where td_clientcd= '" + clientcode + "'  and td_stlmnt = se_stlmnt and td_dt='" + myutil.dtos(Date) + "' ";
        //        //    strsql = strsql + " group by td_stlmnt,td_scripcd,td_scripnm ,td_dt";
        //        //}
        //        strsql = strsql + " Union All";
        //        strsql = strsql + " select case right(sm_prodtype,1) when 'F' then 2 else 3 end, 'Equity '+ ";
        //        strsql = strsql + " case right(sm_prodtype,1) when 'F' then 'Future' else 'Option' end td_type, 'Exp: '+";
        //        strsql = strsql + " convert(char(10),convert(datetime,sm_expirydt),105), case right(sm_prodtype,1) when 'F' then '' else";
        //        strsql = strsql + " ltrim(convert(char(8),sm_strikeprice))+sm_callput end ,replace(rtrim(sm_symbol)+case right(sm_prodtype,1) ";
        //        strsql = strsql + " when 'F' then '' else ' ('+ltrim(convert(char(8),sm_strikeprice))+sm_callput+sm_optionstyle+')' end,'&','') ,sum(td_bqty) 'bqty', ";
        //        strsql = strsql + " sum(convert(decimal(15,2),convert(money,td_bqty * td_rate))) 'bvalue', sum(td_sqty) 'sqty',";
        //        strsql = strsql + " sum(convert(decimal(15,4),convert(money,td_sqty * td_rate))) 'svalue', sum(td_bqty - td_sqty) 'netqty', ";
        //        strsql = strsql + " sum(convert(decimal(15,4),convert(money,(td_sqty - td_bqty)*td_rate))) 'netvalue', ";
        //        strsql = strsql + " cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0 else sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty)";
        //        strsql = strsql + " end) as decimal(15,2)) 'average' ,";
        //        strsql = strsql + " cast(sum(td_brokerage*(td_bqty+td_sqty)) as decimal(15,4)) td_brokerage,";
        //        strsql = strsql + " replace(td_Exchange+td_Segment+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol+'/'+convert(char(9),sm_strikeprice)+sm_callput,'&','-') ";

        //        //if (ConfigurationManager.AppSettings["IsTradeWeb"] == "O")
        //        strsql = strsql + " from trades with(" + StrTradesIndex + "nolock), series_master with (nolock) ";
        //        //else
        //        //    strsql = strsql + " from trades with(" + StrTradesIndex + "nolock), series_master with (nolock) ";

        //        strsql = strsql + " where td_clientcd='" + clientcode + "' and td_seriesid=sm_seriesid and td_Exchange = sm_exchange and td_Segment = sm_Segment and td_dt ='" + Date + "' and td_segment in ('F') ";
        //        strsql = strsql + " group by sm_prodtype,sm_symbol,sm_desc, sm_expirydt, sm_strikeprice, sm_callput,sm_optionstyle,td_dt,td_Exchange,td_Segment";
        //        strsql = strsql + " Union All";

        //        strsql = strsql + " select case ex_eaflag when 'E' then 4 else 5 end ,case ex_eaflag when 'E' then 'Exercise' else 'Assignment' ";
        //        strsql = strsql + " end Td_Type, 'Exp: ' +convert(char(10),convert(datetime,sm_expirydt),105),";
        //        strsql = strsql + " ltrim( convert(char(8),sm_strikeprice))+sm_callput, replace(rtrim(sm_symbol)+' ('+";
        //        strsql = strsql + " ltrim( convert(char(8),sm_strikeprice))+sm_callput+sm_optionstyle+')','&',''), sum(ex_aqty) Bqty,  sum(ex_aqty*ex_diffrate) BAmt,";
        //        strsql = strsql + " sum(ex_eqty) Sqty, sum(ex_eqty*ex_diffrate) SAmt, sum(ex_aqty-ex_eqty) NQty,  sum((ex_aqty-ex_eqty)*ex_diffrate) NAmt,";
        //        strsql = strsql + " cast(convert(money,case when sum(ex_aqty-ex_eqty)=0 then 0 else sum((ex_aqty-ex_eqty)*ex_diffrate)/sum(ex_aqty-ex_eqty) end) as decimal(15,2)) 'average',";
        //        strsql = strsql + " cast(sum(ex_brokerage*(ex_eqty+ex_aqty)) as decimal(15,4)) td_Brokerage,replace(ex_exchange+ex_Segment+ex_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol,'&','-') ";

        //        strsql = strsql + " from exercise with (nolock), series_master with (nolock)";
        //        strsql = strsql + " where ex_clientcd='" + clientcode + "' and ex_exchange=sm_exchange and ex_Segment=sm_Segment and ex_seriesid=sm_seriesid and ex_dt ='" + Date + "'";
        //        strsql = strsql + " group by ex_eaflag, sm_symbol,sm_desc, sm_expirydt, sm_strikeprice, sm_callput,sm_optionstyle,ex_exchange,ex_Segment,ex_dt,sm_prodtype";
        //        strsql = strsql + " Union All";

        //        strsql = strsql + " select case right(sm_prodtype,1) when 'F' then 6 else 7 end, case right(sm_prodtype,1) when 'F' then 'Currency Future'";
        //        strsql = strsql + " else 'Currency Option' end td_type, 'Exp: ' +convert(char(10),convert(datetime,sm_expirydt),105), case right(sm_prodtype,1)";
        //        strsql = strsql + " when 'F' then '' else convert(char(8),sm_strikeprice)+sm_callput end,replace(sm_symbol,'&',''), sum(td_bqty) ,";
        //        strsql = strsql + " sum(round(convert(money,td_bqty * td_rate*sm_multiplier),2)) , sum(td_sqty) ,";
        //        strsql = strsql + " sum(round(convert(money,td_sqty * td_rate*sm_multiplier),2)) , sum(td_bqty - td_sqty) ,";
        //        strsql = strsql + " sum(round(convert(money,(td_sqty - td_bqty)*td_rate*sm_multiplier),4)) , ";
        //        strsql = strsql + " cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0 else sum((td_sqty - td_bqty)*td_rate*sm_multiplier)/sum(td_bqty-td_sqty) end) as decimal(15,2)) ,";
        //        strsql = strsql + " cast(sum(td_brokerage*(td_bqty+td_sqty)) as decimal(15,4)) td_brokerage, replace(td_exchange+td_Segment+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol+'/'+convert(char(9),sm_strikeprice)+sm_callput,'&','-')";

        //        //if (ConfigurationManager.AppSettings["IsTradeWeb"] == "O")
        //        strsql = strsql + " from trades with(" + StrTradesIndex + "nolock), series_master with (nolock)";
        //        //else
        //        //    strsql = strsql + " from trades with(" + StrTradesIndex + "nolock), series_master with (nolock)";

        //        strsql = strsql + " where td_clientcd='" + clientcode + "' and td_seriesid=sm_seriesid and td_Exchange = sm_exchange and td_Segment = sm_Segment and td_dt ='" + Date + "' and td_Segment in ('K')";
        //        strsql = strsql + " group by sm_prodtype,sm_symbol,sm_desc, sm_expirydt, sm_callput, sm_strikeprice, td_exchange,td_Segment,td_dt  ";


        //        strsql = strsql + " Union All";
        //        strsql = strsql + " select 8, 'Commodity Futures' as  td_type, 'Exp: ' +convert(char(10),convert(datetime,sm_expirydt),105),'', ";
        //        strsql = strsql + " replace(sm_symbol,'&',''),sum(td_bqty) ,sum(round(convert(money,td_bqty * td_rate),4)) , sum(td_sqty) ,  ";
        //        strsql = strsql + " sum(round(convert(money,td_sqty * td_rate),4)) , sum(td_bqty - td_sqty) ,  ";
        //        strsql = strsql + " sum(round(convert(money,(td_sqty - td_bqty)*td_rate),4)) , ";
        //        strsql = strsql + " cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0 else sum((td_sqty - td_bqty)*td_rate)/sum(td_bqty-td_sqty) end) as decimal(15,4)),";
        //        strsql = strsql + " cast(sum(td_brokerage*(td_bqty+td_sqty)*sm_multiplier) as decimal(15,4)), replace(td_exchange+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol,'&','-')";

        //        //  SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");

        //        if (SQLConnComex != null)
        //        {
        //            if (Convert.ToInt16(lib.fnFireQueryCommex(SQLConnComex + ".sysobjects a, " + SQLConnComex + ".sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'trades' and b.name", "idx_trades_clientcd", true)) == 1)
        //            { StrComTradesIndex = "index(idx_trades_clientcd),"; }
        //            strsql = strsql + " from " + SQLConnComex + ".trades with(" + StrComTradesIndex + "nolock)  ," + SQLConnComex + ".Series_master with (nolock) ";
        //        }

        //        strsql = strsql + " where td_clientcd='" + clientcode + "' and td_seriesid=sm_seriesid and td_Exchange = sm_exchange ";
        //        strsql = strsql + " and td_dt ='" + Date + "'";
        //        strsql = strsql + " group by sm_prodtype,sm_symbol,sm_desc, sm_expirydt, td_exchange ";
        //        strsql = strsql + " ,td_dt";

        //        strsql = strsql + " Order by td_order,td_stlmnt,td_scripnm ";
        //    }
        //    else
        //    {
        //        Strsql = " SELECT 1 as orderid,1 as td_order,ss_nsymbol,'Equity : ' + td_stlmnt as td_type,td_stlmnt, td_scripcd,replace(ss_lname,'&','') as td_scripnm, td_bqty as bqty, td_sqty as sqty, cast((td_marketrate) as decimal(15,4)) as rate, cast((td_rate) as decimal(15,4)) as netrate,cast((((td_bqty + td_sqty) * td_rate)) * (Case td_bsflag when 'S' then '-1' else '1' end ) as decimal(15,2)) amount, ((td_bqty + td_Sqty) * td_brokerage) as BrokerageAmount,cast((td_brokerage)as decimal(15,4)) td_brokerage,replace(td_stlmnt+td_dt+td_scripcd,'&','-') as td_lookup,td_bsflag ";
        //        Strsql = Strsql + " FROM trx with(" + StrTRXIndex + "nolock), Settlements with (nolock),securities with (nolock) ";
        //        Strsql = Strsql + " WHERE td_clientcd='" + clientcode + "'  and td_stlmnt = se_stlmnt and td_dt='" + Date + "' and td_scripcd=ss_cd";



        //        Strsql = Strsql + " union all ";
        //        //Strsql = Strsql + " Select  3,2 as td_order,case right(sm_prodtype,1) when 'F' then 'Future' else 'Option' end td_type,'',sm_symbol,sm_desc,td_bqty as bqty, ";
        //        Strsql = Strsql + " Select  3,2 as td_order,'',Case td_segment when 'F' then case TD_EXCHANGE When 'N' Then 'NSE F&O' When 'B' Then 'BSE F&O' end  when 'K' then case TD_EXCHANGE When 'M' Then 'MCX FX' When 'N' Then 'NSE FX' end end td_type,'',sm_symbol,replace(sm_desc,'&',''),td_bqty as bqty, ";
        //        Strsql = Strsql + " td_sqty as sqty, cast((td_marketrate)as decimal(15,4)) as diffrate,cast((td_rate)as decimal(15,4)) as netrate,convert(decimal(15,2),(td_bqty - td_sqty) * td_rate * sm_multiplier)  as amount,((td_bqty + td_Sqty) * td_brokerage) as BrokerageAmount,cast((td_brokerage)as decimal(15,4)),replace((td_exchange+td_Segment+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol+'/'+convert(char(9),sm_strikeprice)+sm_callput),'&','-') as td_lookup,td_bsflag from trades with(" + StrTradesIndex + "nolock),series_master with (nolock) where td_seriesid=sm_seriesid and td_Exchange = sm_exchange and td_Segment = sm_Segment and td_clientcd='" + clientcode + "'  and  ";
        //        Strsql = Strsql + " td_dt ='" + Date + "' and td_trxflag='N' ";




        //        if (SQLConnComex != null)
        //        {


        //            if (Convert.ToInt16(lib.fnFireQuery(SQLConnComex.Database + ".[dbo].sysobjects a, " + SQLConnComex.Database + ".[dbo].sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'trades' and b.name", "idx_trades_clientcd", true)) == 1)
        //            { StrComTradesIndex = "index(idx_trades_clientcd),"; }

        //            Strsql = Strsql + " union all ";
        //            Strsql = Strsql + " Select 5,3,'',case ex_eaflag when 'E' then 'Exercise' else 'Assignment' end td_type,'',sm_symbol,'',replace(sm_desc,'&',''), (ex_eqty * (-1)) as bqty, (ex_aqty * (-1)) as sqty, cast((ex_diffrate) as decimal(15,4)) as diffrate, ";
        //            Strsql = Strsql + " cast((ex_mainbrdiffrate) as decimal(15,4)) as netrate,0,cast((ex_brokerage) as decimal(15,4)),replace((ex_exchange+ex_Segment+ex_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol+'/'+convert(char(9),sm_strikeprice)+sm_callput),'&','-') as td_lookup,'' from exercise with (nolock),series_master with (nolock) where ex_exchange=sm_exchange and sm_Segment=ex_Segment and  sm_seriesid=ex_seriesid and ex_clientcd='" + clientcode + "'  and ex_dt ='" + Date + "' ";
        //            Strsql = Strsql + " union all ";
        //            Strsql = Strsql + " Select 6,4 as td_order,'',CASE TD_EXCHANGE When 'N' Then 'NCDX Commodities' When 'M' Then 'MCX Commodities' When 'S' Then 'NSEL Commodities' When 'D' Then 'NSX Commodities' end as td_type,'',sm_symbol,replace(sm_desc,'&',''),td_bqty as bqty, ";
        //            Strsql = Strsql + " td_sqty as sqty,cast((td_marketrate) as decimal(15,4)) as diffrate,cast((td_rate)as decimal(15,4)) as netrate, ((td_bqty + td_Sqty) * td_brokerage) as BrokerageAmount,convert(decimal(15,2),td_rate*(td_bqty-td_sqty)*sm_multiplier),cast((td_brokerage)as decimal(15,4)),replace(td_exchange+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol,'&','-') as td_lookup,td_bsflag ";
        //            Strsql = Strsql + " from " + SQLConnComex.Database + ".[dbo].trades with(" + StrComTradesIndex + "nolock) ," + SQLConnComex.Database + ".[dbo].series_master with (nolock) ";
        //            Strsql = Strsql + " where sm_seriesid=td_seriesid and sm_exchange=td_exchange ";
        //        }

        //        // Strsql = Strsql + " from Ctrades with (nolock),Cseries_master with (nolock) ";
        //        Strsql = Strsql + " and td_clientcd='" + clientcode + "'  and  ";
        //        Strsql = Strsql + " td_dt ='" + Date + "' and td_trxflag='N' ";


        //        Strsql = Strsql + " order by td_type,orderid,td_order,td_stlmnt,td_scripnm ";
        //    }

        //    dt = lib.OpenDataTable(Strsql);

        //    return dt;
        //}


        public DataTable GetDailyConfirmationReport(string clientcode, string Date, string Type)
        {
            string strsql = string.Empty;
            string Strsql = string.Empty;
            UtilityDBModel mydbutil = new UtilityDBModel();
            UtilityModel objUtility = new UtilityModel();
            LibraryModel lib = new LibraryModel();

            Type = "Trade";
            DataTable dt = null;
            string StrTradesIndex = "index(idx_trades_clientcd),";
            string StrTRXIndex = "index(idx_trx_clientcd)";
            string StrComTradesIndex = "";

            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
            // string StrTradesIndex = "";
            if (Convert.ToInt16(lib.fnFireQuery("sysobjects a, sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'trades' and b.name", "idx_trades_clientcd", true)) == 1)
            { StrTradesIndex = "index(idx_trades_clientcd),"; }

            // string StrTRXIndex = "";
            if (Convert.ToInt16(lib.fnFireQuery("sysobjects a, sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'Trx' and b.name", "idx_Trx_Clientcd", true)) == 1)
            { StrTRXIndex = "index(idx_trx_clientcd),"; }


            strsql = " select cast(sum(td_brokerage*(td_bqty+td_sqty)) as decimal(15,4)) bamount,1 Td_order,ss_nsymbol,Trx.td_brokerage,'' as td_ac_type,'' as td_trxdate,'' as td_isin_code,'' as sc_company_name,";
            strsql = strsql + " cast((case when sum(td_bqty-td_sqty)=0 then 0 else sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty) end)as";
            strsql = strsql + " decimal(15,4) )   as rate,'Equity' Td_Type,'' as FScripNm,'' as FExDt,";
            strsql = strsql + " rtrim(td_scripcd)td_scripnm , rtrim(ss_lname) snm,";
            strsql = strsql + " sum(td_bqty) Bqty, convert(decimal(15,2), sum(td_bqty*td_rate)) BAmt, ";
            strsql = strsql + " sum(td_sqty) Sqty, convert(decimal(15,2), sum(td_sqty*td_rate)) SAmt, sum(td_bqty-td_sqty) NQty, convert(decimal(15,2), sum((td_bqty-td_sqty)*td_rate)) NAmt, ";
            strsql = strsql + " '' as td_debit_credit,0 as sm_strikeprice,'' as sm_callput,'Equity|'+''+'|'+td_scripcd  LinkCode ";
            strsql = strsql + " from Trx with (" + StrTRXIndex + "nolock) , securities with(nolock)";
            strsql = strsql + " where td_clientcd='" + clientcode + "' and td_dt = '" + Date + "' ";
            strsql = strsql + " and td_Scripcd = ss_cd";
            strsql = strsql + " group by td_scripcd, ss_lname,'Equity|'+''+'|'+td_scripcd,ss_nsymbol,td_brokerage ";

            strsql = strsql + " union all ";
            strsql = strsql + " select 0,case left(sm_productcd,1) when 'F' then 2 else 3 end,'','','', '','' as td_isin_code,'' as sc_company_name, ";
            strsql = strsql + " cast((case when  sum(td_bqty-td_sqty)=0 then 0 else sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty) end)as decimal(15,4) ) as rate,";
            strsql = strsql + " Case When td_segment='K' then 'Currency ' else 'Equity ' end + ";
            strsql = strsql + " Case left(sm_productcd,1) when 'F' Then 'Future ' else 'Option ' end Td_Type,rtrim(sm_symbol), sm_expirydt,rtrim(sm_symbol), case left(sm_productcd,1) when 'F' then 'Fut ' else 'Opt ' end+ rtrim(sm_symbol)+'  Exp: '+ ltrim(rtrim(convert(char,convert(datetime,sm_expirydt),103))) + case left(sm_productcd,1) when 'F' then '' else ''+rtrim(convert(char(9),sm_strikeprice))+sm_callput+sm_optionstyle end, sum(td_bqty) Bqty, convert(decimal(15,2),sum(td_bqty*td_rate*sm_multiplier)) BAmt,  ";
            strsql = strsql + " sum(td_sqty) Sqty, convert(decimal(15,2),sum(td_sqty*td_rate*sm_multiplier)) SAmt, sum(td_bqty-td_sqty) NQty,  ";
            strsql = strsql + " convert(decimal(15,2),sum((td_bqty-td_sqty)*td_rate *sm_multiplier)) NAmt,'' as td_debit_credit ,sm_strikeprice, sm_callput,";
            strsql = strsql + " Case When td_segment='K' then 'Currency' else 'Equity' end + ";
            strsql = strsql + " Case left(sm_productcd,1) when 'F' Then 'Future' else 'Option' end + '|' + '' + '|' + replace(sm_symbol,'&','-')  + '|' + left(sm_productcd,1) + '|' + sm_expirydt + '|' + Rtrim(Ltrim(Convert(char,sm_strikeprice))) + '|' +  sm_callput + '|' +  sm_optionstyle + '|' +  td_Segment LinkCode";
            strsql = strsql + " from trades with (" + StrTradesIndex + "nolock), series_master with (nolock) ";
            strsql = strsql + " where td_clientcd='" + clientcode + "' and sm_exchange=td_exchange and sm_Segment=td_Segment and td_seriesid=sm_seriesid ";
            strsql = strsql + " and td_dt = '" + Date + "' and td_trxflag <> 'O'  ";
            strsql = strsql + " group by sm_symbol, sm_productcd,td_exchange,td_Segment, sm_expirydt, sm_strikeprice, sm_callput ,sm_optionstyle";
            strsql = strsql + " union all ";
            strsql = strsql + " select 4 ,0,'','','','' as td_trxdate,'' as td_isin_code,'' as sc_company_name,cast((case when  sum(ex_aqty-ex_eqty)=0 then 0 else sum((ex_aqty-ex_eqty)*ex_diffrate *case ex_eaflag When 'A' Then -1 else 1 end)/sum(ex_aqty-ex_eqty) end)as decimal(15,2) ) as rate , ";
            strsql = strsql + " Case When ex_Segment='K' then 'Currency ' else 'Equity ' end + case ex_eaflag when 'E' then 'Exercise ' else 'Assignment ' end Td_Type, '','', rtrim(sm_symbol), case left(sm_productcd,1) when 'F' then 'Fut ' else 'Opt ' end+ rtrim(sm_symbol)+'  Exp: '+ ltrim(rtrim(convert(char,convert(datetime,sm_expirydt),103))) + case left(sm_productcd,1) when 'F' then '' else ''+rtrim(convert(char(9),sm_strikeprice))+sm_callput+sm_optionstyle end, sum(ex_aqty) Bqty, ";
            strsql = strsql + " convert(decimal(15,2),sum(ex_aqty*ex_diffrate *case ex_eaflag When 'A' Then -1 else 1 end *sm_multiplier)) BAmt, sum(ex_eqty) Sqty, convert(decimal(15,2),sum(ex_eqty*ex_diffrate *case ex_eaflag When 'A' Then -1 else 1 end  *sm_multiplier)) SAmt, ";
            strsql = strsql + " sum(ex_aqty-ex_eqty) NQty, convert(decimal(15,2),sum((ex_aqty-ex_eqty)*ex_diffrate *case ex_eaflag When 'A' Then -1 else 1 end    *sm_multiplier)) NAmt,'' as td_debit_credit,0,'', ";
            strsql = strsql + " Case When ex_segment='K' then 'Currency' else 'Equity' end + ";
            strsql = strsql + " case ex_eaflag when 'E' then 'Exercise' else 'Assignment' end + '|' + '' + '|' + replace(sm_symbol,'&','-')  + '|' + left(sm_productcd,1) + '|' + sm_expirydt + '|' + Rtrim(Ltrim(Convert(char,sm_strikeprice))) + '|' +  sm_callput + '|' +  sm_optionstyle + '|' +  ex_Segment LinkCode";
            strsql = strsql + " from exercise with (nolock), series_master with (nolock) where ex_clientcd='" + clientcode + "' ";
            strsql = strsql + " and ex_exchange=sm_exchange and ex_Segment=sm_Segment and ex_seriesid=sm_seriesid ";
            strsql = strsql + " and ex_dt = '" + Date + "' group by ex_eaflag, sm_symbol,ex_Segment,sm_productcd,sm_expirydt, sm_strikeprice, sm_callput ,sm_optionstyle ";
            dt = lib.OpenDataTable(strsql);

            if (SQLConnComex != null)
            {
                //  string StrCommexConn = "";
                // StrCommexConn = objUtility.GetCommexConnection();
                if (Convert.ToInt16(lib.fnFireQuery(SQLConnComex.Database + ".[dbo].sysobjects a, " + SQLConnComex.Database + ".[dbo].sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'trades' and b.name", "idx_trades_clientcd", true)) == 1)
                // if (Convert.ToInt16(lib.fnFireQuery(SQLConnComex + ".sysobjects a, " + SQLConnComex + ".sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'trades' and b.name", "idx_trades_clientcd", true)) == 1)
                { StrComTradesIndex = "index(idx_trades_clientcd),"; }

                strsql = strsql + " union all ";
                strsql = strsql + " select 0, case left(sm_productcd,1) when 'F' then 5 else 6 end,'', '',0,'','' as td_isin_code,";
                strsql = strsql + " '' as sc_company_name,   cast((case when  sum(td_bqty-td_sqty)=0 then 0 else ";
                strsql = strsql + " sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty) end)as decimal(15,4) ) as rate,";
                strsql = strsql + " case left(sm_productcd,1) when 'F' then 'Future (Commodities)' else 'Option (Commodities)' end Td_Type,rtrim(sm_symbol), sm_expirydt,rtrim(sm_symbol), case left(sm_productcd,1) ";
                strsql = strsql + " when 'F' then 'Fut ' else 'Opt ' end+ rtrim(sm_symbol)+'  Exp: '+ ";
                strsql = strsql + " ltrim(rtrim(convert(char,convert(datetime,sm_expirydt),103))) + ";
                strsql = strsql + " case left(sm_productcd,1) when 'F' then '' else ''+rtrim(convert(char(9),sm_strikeprice))+sm_callput end, ";
                strsql = strsql + " sum(td_bqty) Bqty, convert(decimal(15,2), sum(td_bqty*td_rate *sm_multiplier)) BAmt,  sum(td_sqty) Sqty, convert(decimal(15,2), sum(td_sqty*td_rate*sm_multiplier)) SAmt, ";
                strsql = strsql + " sum(td_bqty-td_sqty) NQty, convert(decimal(15,2),sum((td_bqty-td_sqty)*td_rate*sm_multiplier)) NAmt,'' as td_debit_credit ,sm_strikeprice, sm_callput,'Commodities' + '|' + '' + '|' + replace(sm_symbol,'&','-')  + '|' + sm_expirydt LinkCode ";
                strsql = strsql + " from " + SQLConnComex.Database + ".[dbo].trades with(" + StrComTradesIndex + "nolock) ," + SQLConnComex.Database + ".[dbo].series_master with (nolock) ";
                //  strsql = strsql + " from " + SQLConnComex + ".trades with(" + StrComTradesIndex + "nolock), " + SQLConnComex + ".series_master with (nolock) ";
                strsql = strsql + " where td_clientcd='" + clientcode + "' and sm_exchange=td_exchange and td_seriesid=sm_seriesid and td_dt = '" + Date + "' and td_trxflag <> 'O'  ";
                strsql = strsql + " group by sm_symbol, sm_productcd,sm_expirydt, sm_strikeprice, sm_callput  ";
                strsql = strsql + " order by td_order, td_type, snm ,td_scripnm";

                lib.ExecSQL(strsql);
            }

            else
            {
                string StrCTradesIndex = "";
                if (Convert.ToInt16(lib.fnFireQuery("sysobjects a, sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'Ctrades' and b.name", "idx_Ctrades_clientcd", true)) == 1)
                { StrCTradesIndex = "index(idx_Ctrades_clientcd),"; }

                strsql = strsql + " union all ";
                strsql = strsql + " select case left(sm_productcd,1) when 'F' then 5 else 6 end,'', '','' as td_isin_code,";
                strsql = strsql + " '' as sc_company_name,   cast((case when  sum(td_bqty-td_sqty)=0 then 0 else ";
                strsql = strsql + " sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty) end)as decimal(15,4) ) as rate,";
                strsql = strsql + " 'Commodities ' Td_Type,rtrim(sm_symbol), sm_expirydt,rtrim(sm_symbol), case left(sm_productcd,1) ";
                strsql = strsql + " when 'F' then 'Fut ' else 'Opt ' end+ rtrim(sm_symbol)+'  Exp: '+ ";
                strsql = strsql + " ltrim(rtrim(convert(char,convert(datetime,sm_expirydt),103))) + ";
                strsql = strsql + " case left(sm_productcd,1) when 'F' then '' else ''+rtrim(convert(char(9),sm_strikeprice))+sm_callput end, ";
                strsql = strsql + " sum(td_bqty) Bqty, convert(decimal(15,2), sum(td_bqty*td_rate *sm_multiplier)) BAmt,  sum(td_sqty) Sqty, convert(decimal(15,2), sum(td_sqty*td_rate*sm_multiplier)) SAmt, ";
                strsql = strsql + " sum(td_bqty-td_sqty) NQty, convert(decimal(15,2),sum((td_bqty-td_sqty)*td_rate*sm_multiplier)) NAmt,'' as td_debit_credit ,sm_strikeprice, sm_callput,";
                strsql = strsql + " 'Commodities' + '|' + '' + '|' + replace(sm_symbol,'&','-')  + '|' + sm_expirydt LinkCode";
                strsql = strsql + " from Ctrades with (" + StrCTradesIndex + "nolock), Cseries_master with(nolock) ";
                strsql = strsql + " where td_clientcd='" + clientcode + "' and sm_exchange=td_exchange and td_seriesid=sm_seriesid and td_dt ='" + Date + "' and td_trxflag <> 'O'  ";
                strsql = strsql + " group by sm_symbol, sm_productcd,sm_expirydt, sm_strikeprice, sm_callput  ";
                strsql = strsql + " order by Td_order,td_type, snm, td_scripnm";
                lib.ExecSQL(strsql);
            }
            lib.OpenDataTable(strsql);

            return dt;

        }

        public List<DDLSettlementSearch> getClientCodewithMobile(string MobileNo)
        {
            List<DDLSettlementSearch> ulist = new List<DDLSettlementSearch>();
            if (MobileNo != null)
            {
                LibraryModel mylib = new LibraryModel();
                DataTable dt = new DataTable();

                string strSql = "SELECT (cm_name+' - '+ cm_cd) as Name, cm_cd from [Client_master] where cm_mobile='" + MobileNo + "'";
                dt = mylib.OpenDataTable(strSql);
                ulist = dt.AsEnumerable()
                .Select(row => new DDLSettlementSearch
                {
                    Id = row.Field<string>("cm_cd").Trim(),
                    Name = row.Field<string>("Name").Trim(),
                }).ToList();
            }


            return ulist;
        }
        public DataTable GetConfirmation1(string Code, string StrType, string StrScripCode, string StrScripName, string StrOrder, string StrDate, string StrLookup, string Strbsflag)
        {
            UtilityDBModel mydbutil = new UtilityDBModel();
            UtilityModel myutil = new UtilityModel();
            LibraryModel lib = new LibraryModel();

            string Strsql = "";
            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");


            StrLookup = StrLookup.Replace("-", "&").ToString();
            string StrTRXIndex = string.Empty;
            if (Convert.ToInt16(lib.fnFireQuery("sysobjects a, sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'Trx' and b.name", "idx_Trx_Clientcd", true)) == 1)
            { StrTRXIndex = "index(idx_trx_clientcd),"; }

            string StrTradesIndex = "";
            if (Convert.ToInt16(lib.fnFireQuery("sysobjects a, sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'trades' and b.name", "idx_trades_clientcd", true)) == 1)
            { StrTradesIndex = "index(idx_trades_clientcd),"; }


            if (StrOrder == "1")
            {
                if (SQLConnComex != null)
                {
                    //If IsTradeWeb is false means it will connect to TradePlus Database(Live)
                    Strsql = " Select 1 as td_order,'Equity' as td_type, td_stlmnt,td_scripcd,rtrim(ss_Name)+' ('+td_scripcd+')' td_scripnm, ";
                    Strsql = Strsql + " sum(td_bqty) 'bqty',sum(round(convert(money,td_bqty * td_rate),4)) 'bvalue', sum(td_sqty) 'sqty', ";
                    Strsql = Strsql + " sum(round(convert(money,td_sqty * td_rate),4)) 'svalue', sum(td_bqty - td_sqty) 'netqty', ";
                    Strsql = Strsql + " sum(round(convert(money,(td_sqty - td_bqty)*td_rate),4)) 'netvalue', ";
                    Strsql = Strsql + " cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0 else sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty) ";
                    Strsql = Strsql + " end) as decimal(15,2)) 'average', ";
                    Strsql = Strsql + " cast(sum(td_brokerage*(td_bqty+td_sqty)) as decimal(15,4)) td_Brokerage, ";
                    Strsql = Strsql + " td_stlmnt+td_dt+td_scripcd 'td_Lookup' ";
                    Strsql = Strsql + " from Trx with (nolock), securities with (nolock), Settlements with (nolock) ";
                    Strsql = Strsql + " where td_clientcd= '" + Code + "' and td_stlmnt = se_stlmnt and td_scripcd = ss_cd and td_dt='" + StrDate + "' ";
                    if (StrScripCode.Trim() != "")
                    { Strsql += " and td_scripcd = '" + StrScripCode.Trim() + "'"; }
                    if (Strbsflag.Trim() != "")
                    { Strsql += " and td_bsflag = '" + Strbsflag.Trim() + "'"; }
                    Strsql = Strsql + " group by td_stlmnt,td_scripcd,ss_Name ,td_dt";
                }
                else
                {
                    Strsql = "Select 1 as td_order,'Equity' as td_type, td_stlmnt,td_scripcd,rtrim(td_scripnm)+' ('+td_scripcd+')' td_scripnm,";
                    Strsql = Strsql + " sum(td_bqty) 'bqty',sum(round(convert(money,td_bqty * td_rate),4)) 'bvalue', sum(td_sqty) 'sqty', ";
                    Strsql = Strsql + " sum(round(convert(money,td_sqty * td_rate),4)) 'svalue', sum(td_bqty - td_sqty) 'netqty', ";
                    Strsql = Strsql + " sum(round(convert(money,(td_sqty - td_bqty)*td_rate),4)) 'netvalue',  ";
                    Strsql = Strsql + " cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0 else ";
                    Strsql = Strsql + " sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty)  end) as decimal(15,2)) 'average', ";
                    Strsql = Strsql + " cast(sum(td_brokerage*(td_bqty+td_sqty)) as decimal(15,4)) td_Brokerage,  td_stlmnt+td_dt+td_scripcd 'td_Lookup' ";
                    Strsql = Strsql + " from Trx with(" + StrTRXIndex + "nolock) , Settlements with (nolock)  where td_clientcd= '" + Code + "'  ";
                    Strsql = Strsql + " and td_stlmnt = se_stlmnt  and td_dt='" + StrDate + "' and td_stlmnt+td_dt+td_scripcd='" + StrLookup + "' ";
                    if (Strbsflag.Trim() != "")
                    { Strsql += " and td_bsflag = '" + Strbsflag.Trim() + "'"; }
                    Strsql = Strsql + "  group by td_stlmnt,td_scripcd,td_scripnm ,td_dt ";
                }
            }
            else if (StrOrder == "2")
            {
                Strsql = "select case right(sm_prodtype,1) when 'F' then 2 else 3 end, 'Equity '+  case right(sm_prodtype,1) ";
                Strsql = Strsql + " when 'F' then 'Future' else 'Option' end td_type, 'Exp: '+ convert(char(10),convert(datetime,sm_expirydt),105) as td_stlmnt, case right(sm_prodtype,1) ";
                Strsql = Strsql + " when 'F' then '' else ltrim(convert(char(8),sm_strikeprice))+sm_callput end ,rtrim(sm_symbol)+case right(sm_prodtype,1)  when 'F' then ''  else ' ('+ltrim(convert(char(8),sm_strikeprice))+sm_callput+')' end as td_Scripnm ,sum(td_bqty) 'bqty',  ";
                Strsql = Strsql + " sum(round(convert(money,td_bqty * td_rate),4)) 'bvalue', sum(td_sqty) 'sqty',";
                Strsql = Strsql + " sum(round(convert(money,td_sqty * td_rate),4)) 'svalue', sum(td_bqty - td_sqty) 'netqty',   sum(round(convert(money,(td_sqty - td_bqty)*td_rate),4)) 'netvalue', ";
                Strsql = Strsql + " cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0 else  sum((td_bqty-td_sqty)*td_rate)/sum(td_bqty-td_sqty) end) as decimal(15,2)) 'average' , ";
                Strsql = Strsql + " cast(sum(td_brokerage*(td_bqty*td_sqty)) as decimal(15,4)) td_brokerage,  'N'+td_dt+sm_expirydt+right(sm_prodtype,1)+ sm_symbol+'/'+convert(char(9),sm_strikeprice)+sm_callput ";
                if (SQLConnComex != null)
                { Strsql = Strsql + " from trades with (nolock) , series_master with (nolock) "; }
                else
                { Strsql = Strsql + " from  trades with(" + StrTradesIndex + "nolock), series_master with (nolock) "; }

                Strsql = Strsql + " where td_clientcd='" + Code + "' and  td_exchange+td_segment+td_dt+sm_expirydt+right(sm_prodtype,1)+ sm_symbol+'/'+convert(char(9),sm_strikeprice)+sm_callput='" + StrLookup + "' and td_seriesid=sm_seriesid and td_trxFlag = 'N' and td_dt='" + StrDate + "'";
                Strsql = Strsql + " group by sm_prodtype,sm_symbol,sm_desc, sm_expirydt, sm_strikeprice, sm_callput,td_dt ";
                Strsql = Strsql + " Union All ";

                Strsql = Strsql + " select case ex_eaflag when 'E' then 4 else 5 end ,case ex_eaflag when 'E' then 'Exercise' else 'Assignment'  end Td_Type, 'Exp: ' +convert(char(10), ";
                Strsql = Strsql + " convert(datetime,sm_expirydt),105), ltrim( convert(char(8),sm_strikeprice))+sm_callput,   rtrim(sm_symbol)+' ('+ ltrim( convert(char(8),sm_strikeprice))+sm_callput+')', sum(ex_aqty) Bqty, ";
                Strsql = Strsql + " sum(ex_aqty*ex_diffrate) BAmt, sum(ex_eqty) Sqty, sum(ex_eqty*ex_diffrate) SAmt, sum(ex_aqty-ex_eqty) NQty,   sum((ex_aqty-ex_eqty)*ex_diffrate) NAmt, ";
                Strsql = Strsql + " cast(convert(money,case when sum(ex_aqty-ex_eqty)=0 then 0  else sum((ex_aqty-ex_eqty)*ex_diffrate)/sum(ex_aqty-ex_eqty) end) as decimal(15,2)) 'average', ";
                Strsql = Strsql + " cast(sum(ex_brokerage*(ex_eqty+ex_aqty)) as decimal(15,4)) td_Brokerage, ex_exchange+ex_Segment+ex_dt+sm_expirydt+ right(sm_prodtype,1)+sm_symbol ";
                Strsql = Strsql + " from exercise with (nolock), series_master with (nolock) where ex_clientcd='" + Code + "' and  ";
                Strsql = Strsql + " ex_exchange=sm_exchange and ex_Segment=sm_Segment and ex_seriesid=sm_seriesid and  ex_exchange+ex_Segment+ex_dt+sm_expirydt+ right(sm_prodtype,1)+sm_symbol='" + StrLookup + "' and ex_dt='" + StrDate + "' group by ex_eaflag, sm_symbol,sm_desc, ";
                Strsql = Strsql + " sm_expirydt, sm_strikeprice, sm_callput,ex_exchange,ex_Segment,ex_dt,sm_prodtype  ";
            }

            else if (StrOrder == "3")
            {
                Strsql = " select case right(sm_prodtype,1) when 'F' then 6 else 7 end,  case right(sm_prodtype,1) when 'F' then 'CurrentFuture' else 'CurrentOption' end td_type,";
                Strsql = Strsql + " 'Exp: ' +convert(char(10),convert(datetime,sm_expirydt),105) as td_stlmnt,  case right(sm_prodtype,1) when 'F' then '' else convert(char(8),sm_strikeprice)+sm_callput end,sm_symbol,  ";
                Strsql = Strsql + " sum(td_bqty) , sum(round(convert(money,td_bqty * td_rate*sm_multiplier),4)) , sum(td_sqty) ,  sum(round(convert(money,td_sqty * td_rate*sm_multiplier),4)) , sum(td_bqty - td_sqty) , ";
                Strsql = Strsql + " sum(round(convert(money,(td_sqty - td_bqty)*td_rate*sm_multiplier),2)) ,   cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0  ";
                Strsql = Strsql + " else sum((td_sqty - td_bqty)*td_rate*sm_multiplier)/sum(td_bqty-td_sqty) end) as decimal(15,4)) ,  cast(sum(td_brokerage*(td_bqty+td_sqty)) as decimal(15,4)) td_brokerage, ";
                Strsql = Strsql + " (td_exchange+td_Segment+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol+'/'+convert(char(9),sm_strikeprice)+sm_callput)  as td_lookup ";
                if (SQLConnComex != null)
                { Strsql = Strsql + " from trades with (nolock), series_master with (nolock)"; }
                else
                { Strsql = Strsql + " from trades with(" + StrTradesIndex + "nolock), series_master with (nolock) "; }

                Strsql = Strsql + " where td_clientcd='" + Code + "' and td_dt='" + StrDate + "' and td_exchange+td_Segment+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol+'/'+convert(char(9),sm_strikeprice)+sm_callput='" + StrLookup + "' and td_seriesid=sm_seriesid ";
                Strsql = Strsql + " and td_exchange<>'N' group by sm_prodtype,sm_symbol,sm_desc, sm_expirydt, sm_callput,  sm_strikeprice, td_exchange, td_Segment,td_dt   ";

            }
            else if (StrOrder == "4")
            {
                Strsql = " select 8, 'Commodity Futures' as  td_type, 'Exp: ' +convert(char(10),convert(datetime,sm_expirydt),105) as td_stlmnt,'', ";
                Strsql = Strsql + " sm_symbol as td_Scripnm,sum(td_bqty) AS bqty ,sum(round(convert(money,td_bqty * td_rate),4)) as bvalue , sum(td_sqty) as sqty , ";
                Strsql = Strsql + " sum(round(convert(money,td_sqty * td_rate),4)) AS svalue , sum(td_bqty - td_sqty) as netqty ,  ";
                Strsql = Strsql + " sum(round(convert(money,(td_sqty - td_bqty)*td_rate),4)) as netvalue ,   cast(convert(money,case when sum(td_bqty - td_sqty)=0 then 0  ";
                Strsql = Strsql + " else sum((td_sqty - td_bqty)*td_rate)/sum(td_bqty-td_sqty) end) as decimal(15,2)) as average,  cast(sum(td_brokerage*(td_bqty+td_sqty)*sm_multiplier) as decimal(15,4)) td_brokerage, ";
                Strsql = Strsql + " (td_exchange+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol) as td_lookup ";


                if (SQLConnComex != null)
                {
                    Strsql = Strsql + " from " + SQLConnComex + ".Trades," + SQLConnComex + ".Series_master";
                }

                else
                {
                    Strsql = Strsql + " from Ctrades with (nolock),Cseries_master with (nolock)";
                }
                Strsql = Strsql + " where td_clientcd='" + Code + "'  ";
                Strsql = Strsql + " and td_seriesid=sm_seriesid and td_dt ='" + StrDate + "' and  td_exchange+td_dt+sm_expirydt+right(sm_prodtype,1)+sm_symbol='" + StrLookup + "' group by sm_prodtype,sm_symbol,sm_desc, ";
                Strsql = Strsql + "  sm_expirydt, td_exchange,td_dt ";
            }
            DataTable ObjDataSet = new DataTable();
            ObjDataSet = lib.OpenDataTable(Strsql);
            return ObjDataSet;
        }
        public DataTable GetConfirmation(string clientcode, string snm, string td_scripnm, string LinkCode, string td_type, string date)
        {
            clientcode = "10105";
            string strLinkCode;
            DataTable dt = null;
            strLinkCode = LinkCode.Replace("-", "&");
            UtilityModel objUtility = new UtilityModel();
            string strsql = "";
            LibraryModel lib = new LibraryModel();
            string StrTRXIndex = string.Empty;
            if (Convert.ToInt16(lib.fnFireQuery("sysobjects a, sysindexes b", "COUNT(0)", "a.id = b.id and a.name = 'Trx' and b.name", "idx_Trx_Clientcd", true)) == 1)
            { StrTRXIndex = "index(idx_trx_clientcd),"; }
            if (strLinkCode.Split('|')[0].ToString() == "Equity")
            {
                strsql = "snm" + td_scripnm;

                strsql = "select  td_orderid,td_tradeid,td_time, ltrim(rtrim(convert(char,convert(datetime,td_dt),103))) as td_dt,convert(decimal(15,4), case when sum(td_bqty) >0 then  sum(td_bqty*td_rate)/sum(td_bqty) else 0 end )rate1,td_stlmnt, sum(td_bqty) Bqty, convert(decimal(15,2),sum(td_bqty*td_rate)) BAmt, ";
                strsql = strsql + "sum(td_sqty) Sqty, convert(decimal(15,2),sum(td_sqty*td_rate))  SAmt,convert(decimal(15,4),case when sum(td_sqty) > 0 then sum(td_sqty*td_rate)/sum(td_sqty) else  0 end)  rate2,";
                strsql = strsql + "sum(td_bqty-td_sqty) NQty, convert(decimal(15,2),sum((td_bqty-td_sqty)*td_rate)) NAmt ";
                strsql = strsql + " from Trx with(" + StrTRXIndex + "nolock)  where td_clientcd='" + clientcode + "'";
                //strsql = strsql + " and left(td_stlmnt,1) = '" + strLinkCode.Split('|')[1].ToString() + "'";
                strsql = strsql + " and td_scripcd='" + strLinkCode.Split('|')[2].ToString() + "' and td_dt ='" + date + "' ";
                strsql = strsql + "group by td_dt, td_stlmnt,td_orderid,td_tradeid,td_time ";
                dt = lib.OpenDataTable(strsql);

            }
            return dt;
        }
    }
}






