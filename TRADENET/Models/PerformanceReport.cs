using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TRADENET.Models
{
    public class PerformanceReport : ConnectionModel
    {
        public DataTable GetPerformance(string Select, string Code, string FromDate, string ToDate, string strDPID = "", string cmbRep = "", string cmbGroupBy = "")
        {
          
            string strwhere = "";
         
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
           DataTable dt=null;
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

                    strsql = "Drop Table #TmpPerformance";
                    mylib.ExecSQL(strsql, curCon);

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

                    // ------------------------------Start create Condition table------------------------------------
                    if (Code != "")
                    {
                        if (Select == "CL")
                            strwhere += " and cm_Cd = '" + Code.Trim() + "' ";
                        else if (Select == "FM")
                            strwhere += " and cm_familycd = '" + Code.Trim() + "'"; // and cm_familycd= fm_cd "
                        else if (Select == "GR")
                            strwhere += " and cm_groupcd = '" + Code.Trim() + "' "; // and cm_groupcd=gr_cd "
                        else if (Select == "SB")
                            strwhere += " and cm_subbroker = '" + Code.Trim() + "' "; // and cm_subbroker=rm_Cd "
                        else if (Select == "RM")
                            strwhere += " and rtrim(cm_dpactno) = '" + Code.Trim() + "'";
                        else if (Select == "BR")
                            strwhere += " and cm_brboffcode = '" + Code.Trim() + "'";
                        else if (Select == "ALL")
                            strwhere += " ";
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

                        case "RE":
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
                    if (cmbRep == "CL")
                        strGroupBy = "  td_clientcd , cm_name  ";
                    else if (cmbRep == "GW")
                    {
                        strTable = " ,group_master ";
                        strWhere = " and cm_groupcd=gr_cd ";
                        strGroupBy = "  cm_groupcd, gr_desc ";
                    }
                    else if (cmbRep == "FA")
                    {
                        strTable = " ,family_master ";
                        strWhere = " and cm_familycd=fm_cd ";
                        strGroupBy = "  cm_familycd, fm_desc ";
                    }
                    else if (cmbRep == "RW")
                    {
                        strTable = " ,subbrokers sb ";
                        strWhere = " and cm_subbroker=sb.rm_Cd ";
                        strGroupBy = "  cm_subbroker, sb.rm_name ";
                    }
                    else if (cmbRep == "BW")
                    {
                        strTable = " ,branch_master ";
                        strWhere = " and cm_brboffcode=bm_branchcd  ";
                        strGroupBy = "  cm_brboffcode, bm_branchname ";
                    }
                    else if (cmbRep == "RM")
                    {
                        strTable = " ,RM_master rm ";
                        strWhere = " and rtrim(cm_dpactno)=rm.rm_cd ";
                        strGroupBy = "  cm_dpactno, rm.rm_name ";
                    }

                 //'' - FOR COMMODITY---------------------------------------------------------------------------------- -

                    if (SQLConnComex.Database != "" & SQLConnComex.DataSource != "" & strDPID != "")
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

                        strsqlComm = strsqlComm + strWhereIDWise + "and cm_brboffcode = '000012'";
                        strsqlComm = strsqlComm + " and td_dt between '" + FromDate + "' and '" + ToDate + "'   ";
                        strsqlComm = strsqlComm + " and substring(ces_cd,2,1) in ('" + (strDPID.Replace("X", "")).Replace(",", "','") + "') ";
                        strsqlComm = strsqlComm + " and td_companyCode + td_exchange +'F'= CES_Cd ";
                        strsqlComm = strsqlComm + strwhere + strwhere1 + "'and exists " + myutil.LoginAccessCommex("td_clientcd"); // "'and exists " + myutil.LoginAccess("ld_clientcd")//LoginAccessCommex
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
                        strsqlComm = strsqlComm + " and substring(ces_cd,2,1) in ('" + (strDPID.Replace("X", "")).Replace(",", "','") + "') ";
                        strsqlComm = strsqlComm + " and dl_companyCode+dl_exchange+'F'= CES_Cd ";
                        strsqlComm = strsqlComm + strWhere.Replace("td_", "dl_") + strwhere1.Replace("td_", "dl_") + "'and exists " + myutil.LoginAccessCommex("dl_clientcd"); ;
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
                    strsql += " and td_dt between '" + FromDate + "' and '" + ToDate + "'   " + "'and exists " + myutil.LoginAccess("td_clientcd");
                    if (strDPID != "")
                        strsql += " and substring(ces_cd,2,2) in ('" + strDPID.Replace(",", "','") + "') ";

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
                    strsql += " and td_companycode+td_exchange+td_segment= CES_Cd and td_clientcd = cm_cd " + "'and exists " + myutil.LoginAccess("td_clientcd");
                    strsql += " and td_dt between '" + FromDate + "' and '" + ToDate + "'   ";
                    if (strDPID != "")
                        strsql += " and substring(ces_cd,2,2) in ('" + strDPID.Replace(",", "','") + "') ";
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

                        case "RE":
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
                    strsql += " and ex_companycode+ex_exchange+ex_segment = CES_Cd and ex_clientcd = cm_cd " + "'and exists " + myutil.LoginAccess("ex_clientcd");

                    strsql += " and ex_dt between '" + FromDate + "' and '" + ToDate + "'   ";

                    if (strDPID != "")
                        strsql += " and substring(ces_cd,2,2) in ('" + strDPID.Replace(",", "','") + "') ";
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
                    dt = mylib.OpenDataTable(strsql,  curCon);

                 
                }

            }



            return dt;
        }
    }
}






