using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TRADENET.Models;

namespace TRADENET.Models
{
    public class INVPL : ConnectionModel
    {

        public DataTable FnGetActualPLSummary(string ClientCode, string FromDt, string ToDt, string Type, string strignoresection = "N")

        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            DataTable ObjDataTable = new DataTable();

            string ScripCode = "";
            string strwhere = "";
            string strSql = "";
            // Dim objDAL As New DataAccesslayer
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                prGainLoss(ClientCode, FromDt, ToDt, ScripCode, false, strignoresection, curCon);

                if ((Type.ToUpper() ?? "") == "T")
                    strwhere = " Tmp_Flag in ('T') and Tmp_SDt between '" + FromDt + "' and '" + ToDt + "' and ";
                else if ((Type.ToUpper() ?? "") == "D")
                    strwhere = " Tmp_Flag in ('X') and Tmp_SDt between '" + FromDt + "' and '" + ToDt + "' and ";
                else
                    strwhere = " Tmp_Flag in ('T','X') and Tmp_SDt between '" + FromDt + "' and '" + ToDt + "' and ";

                strSql = " select Tmp_Scripcd,ss_lname , cast(Sum(case Tmp_Flag When 'S' Then 0 else Tmp_Qty End) as decimal(15,0)) BQty," + Constants.vbNewLine;
                strSql += " Sum(case Tmp_Flag When 'S'  Then 0 else  CAST((Tmp_BRate* Tmp_Qty) as decimal(15,2))  End) BAmount, cast(Sum(case Tmp_Flag When 'B'  Then 0 else Tmp_Qty End) as decimal(15,0)) SQty," + Constants.vbNewLine;
                strSql += " Sum(case Tmp_Flag When 'B'  Then 0 else   cast((Tmp_SRate* Tmp_Qty) as decimal(15,2))  End) SAmount," + Constants.vbNewLine;
                strSql += "  cast(Sum (case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End else 0 End) as decimal(15,0))  NetQty," + Constants.vbNewLine;
                strSql += " Sum(case When Tmp_Flag in ('B','S')  Then cast(((Tmp_BRate-Tmp_SRate)*Tmp_Qty) as decimal(15,2)) else 0 End) StockAtCost," + Constants.vbNewLine;
                strSql += " Sum(case Tmp_Flag When 'T'  Then   cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2))  else 0 End) Trading," + Constants.vbNewLine;
                strSql += " Sum(case When Tmp_Flag = 'X' and dateDiff(d,Tmp_BDt,Tmp_SDt) < 365  Then cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2)) else 0 End) ShortTerm," + Constants.vbNewLine;
                strSql += " Sum(case When Tmp_Flag ='X' and dateDiff(d,Tmp_BDt,Tmp_SDt) >= 365 Then  case when ((Tmp_LTCG='*') and (Tmp_SRate+Tmp_SStt) < Tmp_BRate) then 0 else  cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2)) end else 0 End) LongTerm, Tmp_MarketRate MarketRate," + Constants.vbNewLine;
                strSql += " CAST((Sum(case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End*Tmp_MarketRate else 0 End)) as decimal(15,2)) StockAtMkt," + Constants.vbNewLine;
                strSql += " CAST((Sum( case Tmp_Flag When 'B' Then Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) When 'S' Then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate) else 0 end)) as decimal(15,2))  UnRealGain ,cast(sum((Tmp_BStt*Tmp_Qty)+ (Tmp_SStt*Tmp_Qty)) as decimal(15,2)) STT" + Constants.vbNewLine;
                strSql += " from #TmpGainLoss,Securities Where " + strwhere + " Tmp_Scripcd = ss_cd Group By Tmp_Scripcd,ss_lname ,Tmp_MarketRate  Order by ss_lname " + Constants.vbNewLine;

                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
            }
            return ObjDataTable;
        }

        public DataTable FnGetNotionalSummary(string ClientCode, string strDate, string strignoresection = "N")
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                string strSql = "";
                string FromDt = strDate;
                string ScripCode = "";
                DataTable ObjDataTable;
                prGainLoss(ClientCode, FromDt, strDate, ScripCode, false, strignoresection, curCon);
                strSql = " select Tmp_Scripcd,ss_lname , cast(Sum(case Tmp_Flag When 'S'  Then 0 else Tmp_Qty End) as decimal(15,0)) BQty," + Constants.vbNewLine;
                strSql += " Sum(case Tmp_Flag When 'S'  Then 0 else  CAST((Tmp_BRate* Tmp_Qty) as decimal(15,2))  End) BAmount, cast(Sum(case Tmp_Flag When 'B'  Then 0 else Tmp_Qty End) as decimal(15,0)) SQty," + Constants.vbNewLine;
                strSql += " Sum(case Tmp_Flag When 'B'  Then 0 else   cast((Tmp_SRate* Tmp_Qty) as decimal(15,2))  End) SAmount," + Constants.vbNewLine;
                strSql += " cast(Sum (case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End else 0 End) as decimal(15,0))  NetQty," + Constants.vbNewLine;
                strSql += " Sum(case When Tmp_Flag in ('B','S')  Then cast(((Tmp_BRate-Tmp_SRate)*Tmp_Qty) as decimal(15,2)) else 0 End) StockAtCost," + Constants.vbNewLine;
                strSql += " Sum(case Tmp_Flag When 'T'  Then   cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2))  else 0 End) Trading,";
                strSql += " Sum(case When Tmp_Flag = 'X' and dateDiff(d,Tmp_BDt,Tmp_SDt) < 365  Then cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2)) else 0 End) ShortTerm," + Constants.vbNewLine;
                strSql += " Sum(case When Tmp_Flag ='X' and dateDiff(d,Tmp_BDt,Tmp_SDt) >= 365 Then  cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2)) else 0 End) LongTerm, cast(Tmp_MarketRate as decimal(15,2)) MarketRate," + Constants.vbNewLine;
                strSql += " CAST((Sum(case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End*Tmp_MarketRate else 0 End)) as decimal(15,2)) StockAtMkt," + Constants.vbNewLine;
                strSql += " CAST((Sum( case Tmp_Flag When 'B' Then (case when DATEDIFF(d,Tmp_BDt,'" + strDate + "') < 365 then Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) else 0 end)" + Constants.vbNewLine;
                strSql += " When 'S' Then  (case when DATEDIFF(d,Tmp_SDt,'" + strDate + "') < 365 then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate)  else 0 end) " + Constants.vbNewLine;
                strSql += " else 0 end)) as decimal(15,2))  UnRealGainShort," + Constants.vbNewLine;
                strSql += " CAST((Sum( case Tmp_Flag When 'B' Then (case when DATEDIFF(d,Tmp_BDt,'" + strDate + "') >= 365 then case when ((Tmp_LTCG='*') and Tmp_MarketRate < Tmp_BRate+Tmp_BStt) then 0 else Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) end else 0 end)" + Constants.vbNewLine;
                strSql += " When 'S' Then  (case when DATEDIFF(d,Tmp_SDt,'" + strDate + "') >= 365 then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate)  else 0 end)" + Constants.vbNewLine;
                strSql += " else 0 end)) as decimal(15,2))  UnRealGainLong  ,cast(sum((Tmp_BStt*Tmp_Qty)+ (Tmp_SStt*Tmp_Qty)) as decimal(15,2)) STT" + Constants.vbNewLine;
                strSql += " from #TmpGainLoss,Securities Where Tmp_Flag in ('B','S') and  Tmp_Scripcd = ss_cd Group By Tmp_Scripcd,ss_lname ,Tmp_MarketRate ,Tmp_Flag Order by Tmp_Flag, ss_lname ";
                // Dim objDAL As New DataAccesslayer
                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
                return ObjDataTable;
            }
        }

        public DataTable FnGetNotionalDetail(string ClientCode, string strDate, string ScripCode, string strignoresection = "N")
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            DataTable ObjDataTable = new DataTable();

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                string strSql = "";
                string strFromDt = strDate;
                prGainLoss(ClientCode, strFromDt, strDate, ScripCode, false, strignoresection, curCon);
                strSql = " select case When Tmp_Flag not in ('B') and Tmp_SDt <> ''  Then ltrim(rtrim(convert(char,convert(datetime,Tmp_SDt),103)))  else ''  End  as Tmp_SDt," + Constants.vbNewLine;
                strSql += " cast(Tmp_SRate as decimal(15,2)) Tmp_SRate , cast(case Tmp_Flag When 'S' Then -Tmp_Qty else Tmp_Qty End as decimal(15,0)) Tmp_Qty ," + Constants.vbNewLine;
                strSql += " case when  Tmp_Flag not in ('S')and Tmp_BDt <> ''  then ltrim(rtrim(convert(char,convert(datetime,Tmp_BDt),103)))  else '' end as Tmp_BDt, cast(Tmp_BRate as decimal(15,2)) Tmp_BRate ," + Constants.vbNewLine;
                strSql += " cast(case When Tmp_Flag in ('B','S') Then ((Tmp_BRate-Tmp_SRate)*Tmp_Qty) else 0 End as decimal(15,2)) StockAtCost," + Constants.vbNewLine;
                strSql += " cast(Round (case When Tmp_Flag in ('B','S') Then ((Tmp_BRate-Tmp_SRate)*Tmp_Qty) else 0 End  /cast(case Tmp_Flag When 'S' Then -Tmp_Qty else Tmp_Qty End as decimal(15,3)),2,1) as decimal(15,2)) Avgrate,";
                strSql += " cast(case When Tmp_Flag in ('B','S') Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End*Tmp_MarketRate  else 0 End as decimal(15,2)) StockAtMkt," + Constants.vbNewLine;
                strSql += " cast(case Tmp_Flag When 'T'  Then ((Tmp_SRate-Tmp_BRate)*Tmp_Qty) else 0 End as decimal(15,2)) Trading ," + Constants.vbNewLine;
                strSql += " cast(case When Tmp_Flag ='X' and dateDiff(d,Tmp_BDt,Tmp_SDt) >= 365 Then  ((Tmp_SRate-Tmp_BRate)*Tmp_Qty)  else 0 End as decimal(15,2)) LongTerm," + Constants.vbNewLine;
                strSql += " cast(case When Tmp_Flag ='X' and dateDiff(d,Tmp_BDt,Tmp_SDt) < 365 Then  ((Tmp_SRate-Tmp_BRate)*Tmp_Qty)  else 0 End as decimal(15,2)) ShortTerm," + Constants.vbNewLine;
                strSql += " CAST((case Tmp_Flag When 'B' Then (case when DATEDIFF(d,Tmp_BDt,'" + strDate + "') < 365 then Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) else 0 end)" + Constants.vbNewLine;
                strSql += " When 'S' Then  (case when DATEDIFF(d,Tmp_SDt,'" + strDate + "') < 365 then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate)  else 0 end) " + Constants.vbNewLine;
                strSql += " else 0 end) as decimal(15,2))  UnRealGainShort," + Constants.vbNewLine;
                strSql += " CAST((case Tmp_Flag When 'B' Then (case when DATEDIFF(d,Tmp_BDt,'" + strDate + "') >= 365 then case when ((Tmp_LTCG='*') and Tmp_MarketRate < Tmp_BRate+Tmp_BStt) then 0 else Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) end else 0 end)" + Constants.vbNewLine;
                strSql += " When 'S' Then  (case when DATEDIFF(d,Tmp_SDt,'" + strDate + "') >= 365 then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate)  else 0 end)" + Constants.vbNewLine;
                strSql += " else 0 end) as decimal(15,2))  UnRealGainLong," + Constants.vbNewLine;
                strSql += " case Tmp_Flag when 'T' then 'Trading' when 'X' then 'Delivery' when 'B' then 'Stock' when 'S' then 'Excess Sell' else '' end Type," + Constants.vbNewLine;
                strSql += " case Tmp_Flag when 'B' then dateDiff(d,Tmp_BDt,'" + strDate + "') when 'S'   then dateDiff(d,Tmp_SDt,'" + strDate + "') else 0  end days," + Constants.vbNewLine;
                strSql += " case Tmp_BSFlag when 'B' then ltrim(rtrim(convert(char,convert(datetime,Tmp_BDt),103))) when 'S' then ltrim(rtrim(convert(char,convert(datetime,Tmp_SDt),103))) else '' end TrxDate," + Constants.vbNewLine;
                strSql += " cast(Tmp_MarketRate as decimal(15,2))  Rate, TMp_Scripcd,ss_lname,cast((Tmp_BStt*Tmp_Qty)+ (Tmp_SStt*Tmp_Qty) as decimal(15,2)) STT,Tmp_LTCG" + Constants.vbNewLine;
                strSql += " from #TmpGainLoss ,Securities " + Constants.vbNewLine;
                strSql += " Where  Tmp_Flag in ('B','S') and  Tmp_Scripcd = ss_cd" + Constants.vbNewLine;
                if (!string.IsNullOrEmpty(Strings.Trim(ScripCode)))
                {
                    strSql += "  and TMp_Scripcd = '" + ScripCode + "'" + Constants.vbNewLine;
                }

                strSql += " order by  case Tmp_Flag When  'B' Then Tmp_BDt when  'S' then Tmp_SDt end ,Tmp_SDt,Tmp_BDt" + Constants.vbNewLine;
                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
            }// Dim objDAL As New DataAccesslayer

            return ObjDataTable;
        }

        public DataTable FnGetActualPLDetail(string ClientCode, string FromDt, string ToDt, string Type, string ScripCode, string strignoresection = "N")
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            DataTable ObjDataTable = new DataTable();

            string strwhere;
            string gstrToday = DateTime.Today.ToString("yyyyMMdd");
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                string strSql = "";
                prGainLoss(ClientCode, FromDt, ToDt, ScripCode, false, strignoresection, curCon);

                if ((Type.ToUpper() ?? "") == "T")
                {
                    strwhere = " 'T'";
                }
                else if ((Type.ToUpper() ?? "") == "D")
                {
                    strwhere = "'X'";
                }
                else
                {
                    strwhere = "'T','X'";
                }

                strSql = " select TMp_Scripcd,case When Tmp_Flag not in ('B') and Tmp_SDt <> ''  Then ltrim(rtrim(convert(char,convert(datetime,Tmp_SDt),103)))  else ''  End  as Tmp_SDt," + Constants.vbNewLine;
                strSql += " Tmp_SRate , cast(case Tmp_Flag When 'S' Then -Tmp_Qty else Tmp_Qty End as decimal(15,0)) Tmp_Qty ," + Constants.vbNewLine;
                strSql += " case when  Tmp_Flag not in ('S')and Tmp_BDt <> ''  then ltrim(rtrim(convert(char,convert(datetime,Tmp_BDt),103)))  else '' end as Tmp_BDt,Tmp_BRate," + Constants.vbNewLine;
                strSql += " cast(case When Tmp_Flag in ('B','S') Then ((Tmp_BRate-Tmp_SRate)*Tmp_Qty) else 0 End as decimal(15,2)) StockAtCost," + Constants.vbNewLine;
                strSql += " cast(case When Tmp_Flag in ('B','S') Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End*Tmp_MarketRate  else 0 End as decimal(15,2)) StockAtMkt," + Constants.vbNewLine;
                strSql += " cast(case Tmp_Flag When 'T'  Then ((Tmp_SRate-Tmp_BRate)*Tmp_Qty) else 0 End as decimal(15,2)) Trading ," + Constants.vbNewLine;
                strSql += " cast(case When Tmp_Flag ='X' and dateDiff(d,Tmp_BDt,Tmp_SDt) >= 365 Then case when ((Tmp_LTCG='*') and (Tmp_SRate+Tmp_SStt) < Tmp_BRate) then 0 else ((Tmp_SRate-Tmp_BRate)*Tmp_Qty) end else 0 End as decimal(15,2)) LongTerm," + Constants.vbNewLine;
                strSql += " cast(case When Tmp_Flag ='X' and dateDiff(d,Tmp_BDt,Tmp_SDt) < 365 Then  ((Tmp_SRate-Tmp_BRate)*Tmp_Qty)  else 0 End as decimal(15,2)) ShortTerm," + Constants.vbNewLine;
                strSql += " cast(case Tmp_Flag When 'B' Then Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) When 'S' Then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate) else 0 end as decimal(15,2)) UnRealGain ," + Constants.vbNewLine;
                strSql += " case Tmp_Flag when 'T' then 'Trading' when 'X' then 'Delivery' when 'B' then 'Stock' when 'S' then 'Excess Sell' else '' end Type," + Constants.vbNewLine;
                strSql += " dateDiff(d,Tmp_BDt,Tmp_SDt)  days," + Constants.vbNewLine;
                strSql += " cast(case Tmp_BSFlag when 'B' then Tmp_BRate when 'S' then Tmp_SRate else '' end as decimal(15,2))  Rate,ss_lname," + Constants.vbNewLine;
                strSql += "  case Tmp_Flag when 'T' then 0 else (Case when right(tmp_sdt,4) between '0401' and '0615' then  1 " + Constants.vbNewLine;
                strSql += " when right(tmp_sdt,4) between '0616' and '0915' then  2 " + Constants.vbNewLine;
                strSql += " when right(tmp_sdt,4) between '0916' and '1215' then  3 " + Constants.vbNewLine;
                strSql += " when right(tmp_sdt,4) between '0316' and '0331' then  5" + Constants.vbNewLine;
                strSql += " else 4 end) end  QtrSlab, TMp_Scripcd +' [ ' +  ss_lname  + ' ] ' as scripcode  ,cast((Tmp_BStt*Tmp_Qty) + (Tmp_SStt*Tmp_Qty) as decimal(15,2)) STT,Tmp_LTCG";
                strSql += " from #TmpGainLoss ,Securities " + Constants.vbNewLine;
                strSql += " Where  Tmp_Flag in (" + strwhere + ") and  Tmp_Scripcd = ss_cd " + Constants.vbNewLine;
                strSql += " and Tmp_SDt between '" + FromDt + "' and '" + ToDt + "'" + Constants.vbNewLine;
                if (!string.IsNullOrEmpty(Strings.Trim(ScripCode)))
                {
                    strSql += "  and TMp_Scripcd = '" + ScripCode + "'" + Constants.vbNewLine;
                }

                strSql += " order by ss_lname,case When Tmp_Flag in ('B','S') Then 1 else 0 end, convert(Char,Tmp_SDt,112),convert(Char,Tmp_BDt,112) ";
                // Dim objDAL As New DataAccesslayer

                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
            }
            return ObjDataTable;
        }


        private void prGainLoss(string strClient, string strFromDt, string strToDt, string strScripCd, bool blndividend = true, string strIgnoresection = "N", SqlConnection curCon = null)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            string strSql = "";
          //  bool blnTrdStlmnt;
            bool blnIncSttDel;
            bool blnIncSttTrd;
         //   blnTrdStlmnt = Convert.ToBoolean((myLib.fnGetSysParam("TRDPLONSTLMT") == "Y") ? true : false);
            DataTable dtFrom;
            DataTable dtTo;
            blnIncSttDel = Convert.ToBoolean((myLib.fnGetSysParam("GAINLOSSTTDL") == "Y") ? true : false);
            blnIncSttTrd = Convert.ToBoolean((myLib.fnGetSysParam("GAINLOSSTTTR") == "Y") ? true : false);



            prCreateTempTableGainLoss(curCon);
            strSql = "Insert into #TmpTRX ";
            strSql += " select td_dt, td_Stlmnt,td_TRXFlag ,td_TRDType,td_clientcd ,td_scripcd ,td_bsflag ,td_bqty,td_Sqty,td_Rate ,Round(td_ServiceTax+td_OtherChrgs1+td_OtherChrgs2,4),td_STT,'X',''";
            strSql += " From TRX_INVPL, Client_mAster Where cm_cd=td_clientcd and td_dt <= '" + strToDt + "' and td_clientcd = '" + strClient + "'";
            if (!string.IsNullOrEmpty(strScripCd))
                strSql += " and td_scripcd = '" + strScripCd + "'";
            strSql += " Order by td_clientcd,td_scripcd ,td_dt,td_TRXFlag,td_TRDType,td_Stlmnt,td_bsflag";
            myLib.ExecSQL(strSql, curCon);

            // //Trading Marking
            strSql = " select Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt,Sum(Tmp_BQty) Tmp_BQty,Sum(Tmp_SQty) Tmp_SQty ";
            strSql += " From #TmpTRX ";
            strSql += " Where Tmp_TRXFlag in ('N','M') ";
            strSql += " and Tmp_Dt between '" + strFromDt + "' and '" + strToDt + "'";
            strSql += " Group By Tmp_Dt,Tmp_Clientcd,Tmp_Scripcd ";
            strSql += " Having Sum(Tmp_BQty) > 0 and Sum(Tmp_SQty) > 0 ";
            strSql += " union ";
            strSql += " select Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt,Sum(Tmp_BQty) Tmp_BQty,Sum(Tmp_SQty) Tmp_SQty ";
            strSql += " From #TmpTRX ";
            strSql += " Where Tmp_TRXFlag in ('N','M') ";
            strSql += " and Tmp_Scripcd in ( select ca_scripcd from Corporate_Action ";
            strSql += " Where ca_purpose='S' and ca_dt < '" + strFromDt + "'";
            strSql += " union select MD_scripcd from MergerDMerger ";
            strSql += " Where MD_IsOldNew='O' and MD_dt < '" + strFromDt + "'";
            strSql += " union select Tmp_Scripcd from #TmpTRX  where Tmp_Dt < '" + strFromDt + "'";
            strSql += "  Group by Tmp_Scripcd Having Sum(Tmp_BQty-Tmp_SQty) <> 0 )";
            strSql += " Group By Tmp_Dt,Tmp_Clientcd,Tmp_Scripcd ";
            strSql += " Having Sum(Tmp_BQty) > 0 and Sum(Tmp_SQty) > 0 ";
            strSql += " Order by Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt";

            dtFrom = myLib.OpenDataTable(strSql, curCon);

            if (dtFrom.Rows.Count > 0)
            {
                int i;
                for (i = 0; i <= dtFrom.Rows.Count - 1; i++)
                {
                    double dblNetQty = 0;

                    dblNetQty = (Convert.ToDouble(dtFrom.Rows[i]["Tmp_BQty"]) > Convert.ToDouble(dtFrom.Rows[i]["Tmp_SQty"])) ? Convert.ToDouble(dtFrom.Rows[i]["Tmp_SQty"]) : Convert.ToDouble(dtFrom.Rows[i]["Tmp_BQty"]);

                    strSql = "select * from #TmpTRX with(index(#Idx_TmpTRX_Clientcd_Scripcd)) Where Tmp_TRXFlag in ('N','M') and Tmp_BSFlag = 'B' and Tmp_Dt = '" + dtFrom.Rows[i]["Tmp_Dt"] + "' and Tmp_Clientcd = '" + dtFrom.Rows[i]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[i]["Tmp_Scripcd"] + "'";
                    strSql += " Order by Tmp_Dt,case Tmp_TRDType When 'SQ' Then 0 else 1 end,Tmp_SRNO";
                    int J;
                    dtTo = myLib.OpenDataTable(strSql, curCon);


                    for (J = 0; J <= dtTo.Rows.Count - 1; J++)
                    {
                        if (Convert.ToDouble(dtTo.Rows[J]["Tmp_BQty"]) > dblNetQty)
                        {
                            strSql = "Insert into #TmpTRX select Tmp_Dt,Tmp_Stlmnt,Tmp_TRXFlag,Tmp_TRDType,Tmp_Clientcd,Tmp_Scripcd,Tmp_BSFlag," + (Convert.ToDouble(dtFrom.Rows[i]["Tmp_BQty"]) - dblNetQty) + ",Tmp_SQty,Tmp_Rate,Tmp_Chrgs,Tmp_STT, Tmp_Flag,''";
                            strSql += " From #TmpTRX Where Tmp_SRNo = " + dtTo.Rows[J]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);


                            strSql = " Update #TmpTRX set Tmp_BQty = " + dblNetQty + " Where Tmp_SRNo = " + dtTo.Rows[J]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);
                        }

                        strSql = " Update #TmpTRX set Tmp_Flag = 'T' Where Tmp_SRNo = " + dtTo.Rows[J]["Tmp_SRNO"];
                        myLib.ExecSQL(strSql, curCon);

                        dblNetQty -= Convert.ToDouble(dtTo.Rows[J]["Tmp_BQty"]);

                        if (dblNetQty <= 0)
                            break;
                    }

                    dblNetQty = Convert.ToDouble((Convert.ToDouble(dtFrom.Rows[i]["Tmp_BQty"]) > Convert.ToDouble(dtFrom.Rows[i]["Tmp_SQty"]) ? Convert.ToDouble(dtFrom.Rows[i]["Tmp_SQty"]) : Convert.ToDouble(dtFrom.Rows[i]["Tmp_BQty"])));

                    strSql = "select * from #TmpTRX with(index(#Idx_TmpTRX_Clientcd_Scripcd)) Where Tmp_TRXFlag in ('N','M') and Tmp_BSFlag = 'S' and Tmp_Dt = '" + dtFrom.Rows[i]["Tmp_Dt"] + "' and Tmp_Clientcd = '" + dtFrom.Rows[i]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[i]["Tmp_Scripcd"] + "'";
                    strSql += " Order by Tmp_Dt,case Tmp_TRDType When 'SQ' Then 0 else 1 end,Tmp_SRNO";
                    dtTo = myLib.OpenDataTable(strSql, curCon);
                    int K;
                    for (K = 0; K <= dtTo.Rows.Count - K; i++)
                    {
                        if (Convert.ToDouble(dtTo.Rows[K]["Tmp_SQty"]) > dblNetQty)
                        {
                            strSql = "Insert into #TmpTRX select Tmp_Dt,Tmp_Stlmnt,Tmp_TRXFlag,Tmp_TRDType,Tmp_Clientcd,Tmp_Scripcd,Tmp_BSFlag,Tmp_BQty," + (Convert.ToDouble(dtTo.Rows[K]["Tmp_SQty"]) - dblNetQty) + ",Tmp_Rate,Tmp_Chrgs,Tmp_STT,Tmp_Flag,''";
                            strSql += " From #TmpTRX Where Tmp_SRNo = " + dtTo.Rows[K]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);
                            myLib.ExecSQL("Update #TmpTRX set Tmp_SQty = " + dblNetQty + " Where Tmp_SRNo = '" + dtTo.Rows[K]["Tmp_SRNO"] + "'", curCon);

                        }
                        myLib.ExecSQL(" Update #TmpTRX set Tmp_Flag = 'T' Where Tmp_SRNo = '" + dtTo.Rows[K]["Tmp_SRNO"] + "'", curCon);

                        dblNetQty -= Convert.ToDouble(dtTo.Rows[K]["Tmp_SQty"]);
                        if (dblNetQty <= 0)
                            break;
                    }
                }
            }

            //DataTable Test = myLib.OpenDataTable("select * from #TmpTRX", curCon);

            // //Trading Marking
            prCorporateAction(strFromDt, strToDt, blndividend, curCon);

            if (blndividend)
                return;
            // Opening
            strSql = " select Tmp_Clientcd,Tmp_Scripcd,Sum(Tmp_Bqty-Tmp_Sqty) Net ";
            strSql += " from #TmpTRX Where TMp_Flag = 'X' and Tmp_Dt < '" + strFromDt + "'";
            strSql += " Group by Tmp_Clientcd,Tmp_Scripcd ";
            strSql += " Having Sum(Tmp_Bqty-Tmp_Sqty) <> 0";
            strSql += " Order by Tmp_Clientcd,Tmp_Scripcd ";

            dtFrom = myLib.OpenDataTable(strSql, curCon);

            if (dtFrom.Rows.Count > 0)
            {
                int iFrom = 0;

                for (iFrom = 0; iFrom <= dtFrom.Rows.Count - 1; iFrom++)
                {
                    double dblNetQty = 0;
                    dblNetQty = Math.Abs(Convert.ToDouble(dtFrom.Rows[iFrom]["Net"]));

                    strSql = " select * from #TmpTRX Where TMp_Flag = 'X' and Tmp_Dt < '" + strFromDt + "' and Tmp_BSFlag = '" + ((Convert.ToDouble(dtFrom.Rows[iFrom]["Net"]) > 0) ? "B" : "S") + "'";
                    strSql += " and Tmp_Clientcd = '" + dtFrom.Rows[iFrom]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[iFrom]["Tmp_Scripcd"] + "'";
                    strSql += " Order by Tmp_Dt desc,Tmp_SRNO desc";
                    dtTo = myLib.OpenDataTable(strSql, curCon);
                    int iTo = 0;
                    for (iTo = 0; iTo <= dtTo.Rows.Count - 1; iTo++)
                    {
                        if (Convert.ToDouble((Convert.ToDouble(dtFrom.Rows[iFrom]["Net"]) > 0) ? dtTo.Rows[iTo]["Tmp_BQty"] : dtTo.Rows[iTo]["Tmp_SQty"]) > dblNetQty)
                        {
                            strSql = "Insert into #TmpTRX select Tmp_Dt,Tmp_Stlmnt,Tmp_TRXFlag,'',Tmp_Clientcd,Tmp_Scripcd,Tmp_BSFlag,";
                            strSql += " " + ((Convert.ToDouble(dtFrom.Rows[iFrom]["Net"]) > 0) ? (Convert.ToDouble(dtTo.Rows[iTo]["Tmp_BQty"]) - dblNetQty).ToString() : "Tmp_BQty") + " Tmp_BQty,";
                            strSql += " " + ((Convert.ToDouble(dtFrom.Rows[iFrom]["Net"]) > 0) ? (Convert.ToDouble(dtTo.Rows[iTo]["Tmp_SQty"]) - dblNetQty).ToString() : "Tmp_SQty") + " Tmp_SQty,";
                            strSql += " Tmp_Rate,Tmp_Chrgs, Tmp_STT, Tmp_Flag,'' ";
                            strSql += " From #TmpTRX Where Tmp_SRNo = " + dtTo.Rows[iTo]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);

                            strSql = " Update #TmpTRX set " + ((Convert.ToDouble(dtFrom.Rows[iFrom]["Net"]) > 0) ? "Tmp_BQty" : "Tmp_SQty") + " = " + dblNetQty + " Where Tmp_SRNo = " + dtTo.Rows[iTo]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);
                        }

                        strSql = " Update #TmpTRX set Tmp_Flag = 'Z' Where Tmp_SRNo = " + int.Parse(dtTo.Rows[iTo]["Tmp_SRNO"].ToString());
                        myLib.ExecSQL(strSql, curCon);

                        dblNetQty -= ((Convert.ToDouble(dtFrom.Rows[iFrom]["Net"]) > 0) ? Convert.ToDouble(dtTo.Rows[iTo]["Tmp_BQty"]) : Convert.ToDouble(dtTo.Rows[iTo]["Tmp_SQty"]));

                        if (dblNetQty <= 0)
                            break;
                    }
                }
            }

            strSql = "Delete #TmpTRX Where TMp_Flag = 'X' and Tmp_Dt < '" + strFromDt + "'";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Update #TmpTRX Set TMp_Flag = 'X'  Where TMp_Flag = 'Z' and Tmp_Dt < '" + strFromDt + "'";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Delete #TmpTRX Where TMp_Flag = 'T' and Tmp_Dt < '" + strFromDt + "'";
            myLib.ExecSQL(strSql, curCon);

            // Opening
            // //Trading Gain Loss
            strSql = "Insert into #TmpGainLoss ";
            strSql += " select Tmp_Clientcd,Tmp_Scripcd,Tmp_BQty,Tmp_Dt,Tmp_Rate,Tmp_Chrgs, Tmp_STT,'' Tmp_SDt,0 Tmp_SRate,0 Tmp_SChrg, 0 Tmp_SStt,'B' Tmp_Flag,0, Tmp_BsFLAg ,Tmp_Remark,Tmp_Stlmnt";
            strSql += " From #TmpTRX Where TMp_Flag = 'T' and Tmp_BSFlag = 'B'";
            myLib.ExecSQL(strSql, curCon);

            strSql = "select * from #TmpTRX Where TMp_Flag = 'T' and Tmp_BSFLAG = 'S'";
            strSql += " Order by Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt";

            dtFrom = myLib.OpenDataTable(strSql, curCon);
            if (dtFrom.Rows.Count > 0)
            {
                int iFrom = 0;
                for (iFrom = 0; iFrom <= dtFrom.Rows.Count - 1; iFrom++)
                {
                    double dblQTy = Convert.ToDouble(dtFrom.Rows[iFrom]["Tmp_SQty"]);

                    strSql = "select * from #TmpGainLoss with(index(#Idx_TmpGainLoss_Clientcd_Scripcd)) Where Tmp_Clientcd='" + dtFrom.Rows[iFrom]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[iFrom]["Tmp_Scripcd"] + "' and Tmp_BDt = '" + dtFrom.Rows[iFrom]["Tmp_Dt"] + "' and Tmp_Flag='B'";
                    //if (blnTrdStlmnt == true)
                    //    strSql += " and Tmp_Stlmnt = '" + dtFrom.Rows[iFrom]["Tmp_Stlmnt"] + "'";
                    strSql += " Order by Tmp_BDt,Tmp_SRNO";
                    dtTo = myLib.OpenDataTable(strSql, curCon);

                    int iTo = 0;
                    for (iTo = 0; iTo <= dtTo.Rows.Count - 1; iTo++)
                    {
                        if (Convert.ToDouble(dtTo.Rows[iTo]["Tmp_Qty"]) > dblQTy)
                        {
                            strSql = "Insert into #TmpGainLoss";
                            strSql += " select Tmp_Clientcd,Tmp_Scripcd," + (Convert.ToDouble(dtTo.Rows[iTo]["Tmp_Qty"]) - dblQTy) + ",Tmp_BDt,Tmp_BRate,Tmp_BChrg,Tmp_BStt,Tmp_SDt,Tmp_SRate,Tmp_SChrg,Tmp_SStt,Tmp_Flag,0, TMp_Bsflag,Tmp_Remark,Tmp_Stlmnt";
                            strSql += " From #TmpGainLoss";
                            strSql += " Where Tmp_SRNO = " + dtTo.Rows[iTo]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);

                            strSql = " Update #TmpGainLoss set Tmp_Qty = " + dblQTy;
                            strSql += " Where Tmp_SRNO = " + dtTo.Rows[iTo]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);
                        }

                        strSql = " Update #TmpGainLoss set Tmp_Flag = 'T',Tmp_Sdt=" + dtFrom.Rows[iFrom]["Tmp_Dt"] + ",Tmp_SRate=" + dtFrom.Rows[iFrom]["Tmp_Rate"] + ",Tmp_SChrg=" + dtFrom.Rows[iFrom]["Tmp_Chrgs"] + ", Tmp_SStt=" + dtFrom.Rows[iFrom]["Tmp_STT"];
                        strSql += " Where Tmp_SRNO = " + dtTo.Rows[iTo]["Tmp_SRNO"];
                        myLib.ExecSQL(strSql, curCon);

                        dblQTy -= Convert.ToDouble(dtTo.Rows[iTo]["Tmp_Qty"]);
                        if (Convert.ToDouble(dblQTy) <= 0)
                            break;
                    }
                }
            }


            // Trading Gain Loss
            // Delivery Gain Loss
            strSql = "Insert into #TmpGainLoss ";
            strSql += " select Tmp_Clientcd,Tmp_Scripcd,Tmp_BQty,Tmp_Dt,Tmp_Rate,Tmp_Chrgs, Tmp_STT,'' Tmp_SDt,0 Tmp_SRate,0 Tmp_SChrg, 0 Tmp_SStt,'B' Tmp_Flag,0, tmp_bsFlag,Tmp_Remark,Tmp_Stlmnt";
            strSql += " From #TmpTRX Where TMp_Flag <> 'T' and Tmp_BSFlag = 'B'";
            strSql += " Order by Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt,Tmp_SRNO";
            myLib.ExecSQL(strSql, curCon);

            strSql = "select * from #TmpTRX Where TMp_Flag <> 'T' and Tmp_BSFLAG = 'S'";
            strSql += " Order by Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt";

            dtFrom = myLib.OpenDataTable(strSql, curCon);
            if (dtFrom.Rows.Count > 0)
            {
                int iFrom = 0;
                for (iFrom = 0; iFrom <= dtFrom.Rows.Count - 1; iFrom++)
                {
                    double dblQTy = Convert.ToDouble(dtFrom.Rows[iFrom]["Tmp_SQty"]);

                    strSql = "select * from #TmpGainLoss with(index(#Idx_TmpGainLoss_Clientcd_Scripcd)) Where Tmp_Clientcd='" + dtFrom.Rows[iFrom]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[iFrom]["Tmp_Scripcd"] + "' and Tmp_BDt <= '" + dtFrom.Rows[iFrom]["Tmp_Dt"] + "' and Tmp_Flag='B'";
                    strSql += " Order by Tmp_BDt,isnull(Tmp_Filler1,''),Tmp_BRate desc,Tmp_SRNO";
                    dtTo = myLib.OpenDataTable(strSql, curCon);

                    int iTo = 0;
                    for (iTo = 0; iTo <= dtTo.Rows.Count - 1; iTo++)

                    {
                        if (Convert.ToDouble(dtTo.Rows[iTo]["Tmp_Qty"]) > dblQTy)
                        {
                            strSql = "Insert into #TmpGainLoss";
                            strSql += " select Tmp_Clientcd,Tmp_Scripcd," + (Convert.ToDouble(dtTo.Rows[iTo]["Tmp_Qty"]) - dblQTy) + ",Tmp_BDt,Tmp_BRate,Tmp_BChrg,Tmp_BStt, Tmp_SDt,Tmp_SRate,Tmp_SChrg,Tmp_SStt, Tmp_Flag,0, tmp_bsFlag ,tmp_remark,Tmp_Stlmnt";
                            strSql += " From #TmpGainLoss";
                            strSql += " Where Tmp_SRNO = " + dtTo.Rows[iTo]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);

                            strSql = " Update #TmpGainLoss set Tmp_Qty = " + dblQTy;
                            strSql += " Where Tmp_SRNO = " + dtTo.Rows[iTo]["Tmp_SRNO"];
                            myLib.ExecSQL(strSql, curCon);
                        }

                        strSql = " Update #TmpGainLoss set Tmp_Flag = 'X',Tmp_Sdt=" + dtFrom.Rows[iFrom]["Tmp_Dt"] + ",Tmp_SRate=" + dtFrom.Rows[iFrom]["Tmp_Rate"] + ",Tmp_SChrg=" + dtFrom.Rows[iFrom]["Tmp_Chrgs"] + ",Tmp_SStt=" + dtFrom.Rows[iFrom]["Tmp_STT"];
                        strSql += " Where Tmp_SRNO = " + dtTo.Rows[iTo]["Tmp_SRNO"];
                        myLib.ExecSQL(strSql, curCon);

                        dblQTy -= Convert.ToDouble(dtTo.Rows[iTo]["Tmp_Qty"]);
                        if (Convert.ToDouble(dblQTy) <= 0)
                            break;
                    }

                    if (Convert.ToDouble(dblQTy) > 0)
                    {
                        strSql = "Insert into #TmpGainLoss Values ('" + dtFrom.Rows[iFrom]["Tmp_Clientcd"] + "','" + dtFrom.Rows[iFrom]["Tmp_Scripcd"] + "'," + dblQTy + ",'',0,0,0,";
                        strSql += "'" + dtFrom.Rows[iFrom]["Tmp_dt"] + "'," + dtFrom.Rows[iFrom]["Tmp_Rate"] + "," + dtFrom.Rows[iFrom]["Tmp_Chrgs"] + "," + dtFrom.Rows[iFrom]["Tmp_STT"] + ",'S',0,'" + dtFrom.Rows[iFrom]["Tmp_BSFlag"] + "' , '" + dtFrom.Rows[iFrom]["Tmp_Remark"] + "','" + dtFrom.Rows[iFrom]["Tmp_Stlmnt"] + "' )";
                        myLib.ExecSQL(strSql, curCon);
                    }
                }
            }
            myLib.ExecSQL(" Delete from #TmpGainLoss where Tmp_Qty = 0", curCon);


            // Delivery Gain Loss

            myLib.ExecSQL("insert into #TmpRates select distinct Tmp_Scripcd,0 from #TmpGainLoss", curCon);
            string strRMSVALATLTRT;
            strRMSVALATLTRT = myLib.GetSysPARM("RMSVALATLTRT");

            strSql = "Update #TmpRates set T_Rate =  mk_closerate";
            strSql += " From Market_Rates Where T_Scripcd = Mk_Scripcd and mk_Exchange = 'B'";
            strSql += " and mk_dt = (select max(mk_dt) From Market_Rates Where T_Scripcd = Mk_Scripcd and mk_Exchange = 'B'";
            strSql += " and mk_Scripcd in (select T_Scripcd from #TmpRates)";
            if (Conversion.Val(strRMSVALATLTRT) > 0)
                strSql = strSql + " and mk_dt >='" + myutil.dtos(myLib.AddDayDT(strToDt, -Convert.ToInt32(strRMSVALATLTRT)).ToString()) + "'";
            strSql += " and mk_dt <='" + strToDt + "')";
            strSql += " and mk_Scripcd in (select T_Scripcd from #TmpRates)";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Update #TmpRates set T_Rate = mk_closerate";
            strSql += " From Market_Rates Where T_Scripcd = Mk_Scripcd and mk_Exchange = 'N'";
            strSql += " and mk_dt = (select max(mk_dt) From Market_Rates Where T_Scripcd = Mk_Scripcd and mk_Exchange = 'N'";
            strSql += " and mk_Scripcd in (select T_Scripcd from #TmpRates)";
            if (Conversion.Val(strRMSVALATLTRT) > 0)
                strSql = strSql + " and mk_dt >='" + myutil.dtos(myLib.AddDayDT(strToDt, -Convert.ToInt32(strRMSVALATLTRT)).ToString()) + "'";
            strSql += " and mk_dt <='" + strToDt + "')";
            strSql += " and mk_Scripcd in (select T_Scripcd from #TmpRates)";
            myLib.ExecSQL(strSql, curCon);

            myLib.ExecSQL("Update #TmpGainLoss set Tmp_MarketRate = T_Rate from #TmpRates where Tmp_Scripcd = T_scripcd ", curCon);


            myLib.ExecSQL("Update #TmpGainLoss set Tmp_BRate = Tmp_BRate + Tmp_BChrg ,Tmp_SRate = Tmp_SRate -Tmp_SChrg ", curCon);


            if (blnIncSttDel)
            {
                myLib.ExecSQL("Update #TmpGainLoss set Tmp_BRate = Tmp_BRate + Tmp_BStt ,Tmp_SRate = Tmp_SRate - Tmp_SStt where Tmp_Flag <> 'T'", curCon);
                myLib.ExecSQL("Update #TmpGainLoss set Tmp_BStt = 0 ,Tmp_SStt = 0 where Tmp_Flag <> 'T'", curCon);
            }
            if (blnIncSttTrd)
            {
                myLib.ExecSQL("Update #TmpGainLoss set Tmp_BRate = Tmp_BRate + Tmp_BStt ,Tmp_SRate = Tmp_SRate - Tmp_SStt where Tmp_Flag = 'T'", curCon);
                myLib.ExecSQL("Update #TmpGainLoss set Tmp_BStt = 0 ,Tmp_SStt = 0 where Tmp_Flag = 'T'", curCon);

            }
            myLib.ExecSQL("Alter Table #TmpGainLoss Add Tmp_LTCG char(1) DEFAULT ('') NOT NULL ", curCon);

            //if ((strIgnoresection == "N") && (Convert.ToDouble(strToDt) > Convert.ToDouble(fnRateYear() + "0331")))
            if (strIgnoresection == "N" && fnchkTable(("Market_Rates" + fnRateYear() + "0131"), curCon) && (Convert.ToDouble(strToDt) > Convert.ToDouble(fnRateYear() + "0331")))
            {
                strSql = "Update #TmpGainLoss  " + Constants.vbNewLine + " Set Tmp_BRate = mk_Rate,Tmp_LTCG = '*',Tmp_BStt=0  " + Constants.vbNewLine + " From Market_Rates" + fnRateYear() + "0131 , Securities " + Constants.vbNewLine + " Where Tmp_scripcd = ss_cd and  Tmp_scripcd = mk_scripcd " + Constants.vbNewLine + " and DateDiff(d,Case When Tmp_BSFlag = 'S' then Tmp_SDt else Tmp_BDt end ,'" + strToDt + "') >= 365" + Constants.vbNewLine + " and Tmp_Flag = 'B' and mk_Rate > (Tmp_BRate+Tmp_BStt) and ss_chargestt = 'Y'";
                myLib.ExecSQL(strSql, curCon);

                strSql = "Update #TmpGainLoss  " + Constants.vbNewLine + " Set Tmp_BRate = mk_Rate,Tmp_LTCG = '*',Tmp_BStt=0  " + Constants.vbNewLine + " From Market_Rates" + fnRateYear() + "0131,Securities" + Constants.vbNewLine + " Where Tmp_scripcd = ss_cd and  Tmp_scripcd = mk_scripcd and tmp_bdt <= '" + fnRateYear() + "0131' and tmp_sdt > '" + fnRateYear() + "0331' " + Constants.vbNewLine + " and DateDiff(d,Tmp_BDt,Tmp_SDt) >= 365 " + Constants.vbNewLine + " and Tmp_Flag = 'X' and mk_Rate > (Tmp_BRate+Tmp_BStt)  and ss_chargestt = 'Y'" + Constants.vbNewLine + " and (Tmp_SRate-Tmp_SStt) > (Tmp_BRate+Tmp_BStt) ";

                myLib.ExecSQL(strSql, curCon);
            }

            DataTable dtTestFrom = myLib.OpenDataTable("select * from #TmpGainLoss", curCon);

        }


        public Boolean fnchkTable(string strTable, SqlConnection curCon)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            Boolean fnchkTableRet = false;
            DataTable dtChk = myLib.OpenDataTable("select count(*) from sysobjects where name ='" + strTable + "'", curCon);
            if (Convert.ToDouble(dtChk.Rows[0][0]) > 0) 
            {
                fnchkTableRet = true;
            }
            return fnchkTableRet;
        }
        public string fnRateYear()
        {
            return "2018";
        }


        public void prCorporateAction(string strFromDt, string strToDt, bool blndividend = false, SqlConnection curCon = null)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strSql = "";
            DataTable dtRst;
            DataTable dtData;
            DataTable dtFrom;
            DataTable dtTRX;
            DataTable dtCorpAct;
            int i;
            int j;
            int k;
            int l;
            int p;


            strSql = " Select distinct xtype, ca_dt From ";
            strSql += " ( ";
            strSql += " Select 'C' xtype , ca_dt from Corporate_action Where  ca_dt <= '" + strToDt + "' and ca_purpose <> 'D' ";
            strSql += "  and exists ( select Tmp_scripcd,min(Tmp_Dt) from #TmpTRX Where Tmp_scripcd = ca_scripcd  Group by Tmp_scripcd  Having ca_dt >  min(Tmp_Dt) )  ";
            strSql += " Union All ";
            strSql += " Select 'C' xtype , ca_dt from Corporate_action Where  ca_dt between '" + strFromDt + "' and '" + strToDt + "' and ca_purpose = 'D' ";
            strSql += "  and exists ( select Tmp_scripcd,min(Tmp_Dt) from #TmpTRX Where Tmp_scripcd = ca_scripcd  Group by Tmp_scripcd  Having ca_dt >  min(Tmp_Dt) )  ";
            strSql += " Union All ";
            strSql += " Select 'X' xtype, MD_dt ca_dt from MergerDMerger Where MD_dt <= '" + strToDt + "' and MD_IsOldNew='O' ";
            strSql += " and exists ( select Tmp_scripcd,min(Tmp_Dt) from #TmpTRX Where Tmp_scripcd = md_scripcd  Group by Tmp_scripcd  Having MD_dt >  min(Tmp_Dt) )   ";
            strSql += " ) A ";
            strSql += " Order by ca_dt ";

            dtData = myLib.OpenDataTable(strSql, curCon);

            if (dtData.Rows.Count > 0)
            {
                for (i = 0; i <= dtData.Rows.Count - 1; i++)
                {
                    if ((dtData.Rows[i]["xtype"]).ToString().Trim() == "C")
                    {
                        strSql = " select Tmp_scripcd,ca_purpose from (";
                        strSql += " select Tmp_clientcd,Tmp_scripcd,ca_purpose,ca_rate ";
                        strSql += " from #TmpTRX left join Settlements on Tmp_Stlmnt = se_stlmnt,Corporate_Action";
                        strSql += " where case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end <= ca_dt and TMp_Flag <> case ca_type When 'S' Then 'T' else 'ZZ' End ";
                        strSql += " and Tmp_scripcd = ca_scripcd and ca_dt = '" + dtData.Rows[i]["ca_dt"] + "'" + ((!blndividend) ? " and ca_purpose <> 'D' " : "");
                        strSql += " Group By Tmp_clientcd,Tmp_scripcd,ca_purpose,ca_rate";
                        strSql += " Having sum(Tmp_bqty - Tmp_sqty)>0 ) a ";
                        strSql += " Group by Tmp_scripcd,ca_purpose";
                        strSql += " Order by Tmp_scripcd,case ca_purpose When 'S' Then 0 else 1 end";

                        dtCorpAct = myLib.OpenDataTable(strSql, curCon);


                        for (j = 0; j <= dtCorpAct.Rows.Count - 1; j++)
                        {
                            strSql = "Insert into #tmpfcaction ";
                            strSql += " select Tmp_clientcd,Tmp_scripcd,sum(Tmp_bqty - Tmp_sqty),0,0,'Y',ca_purpose,ca_rate,0,0,'" + dtData.Rows[i]["ca_dt"] + "'";
                            strSql += " from #TmpTRX left join Settlements on Tmp_Stlmnt = se_stlmnt,Corporate_Action";
                            strSql += " where case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end <= ca_dt and TMp_Flag <> case ca_type When 'S' Then 'T' else 'ZZ' End ";
                            strSql += " and Tmp_scripcd = ca_scripcd and ca_dt = '" + dtData.Rows[i]["ca_dt"] + "'" + ((!blndividend) ? " and ca_purpose <> 'D' " : "");
                            strSql += " and Tmp_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_purpose = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                            strSql += " Group By Tmp_clientcd,Tmp_scripcd,ca_purpose,ca_rate";
                            strSql += " Having sum(Tmp_bqty - Tmp_sqty)>0";
                            myLib.ExecSQL(strSql, curCon);

                            if (Convert.ToInt32(myLib.fnFireQuery("#tmpfcaction", "count(0)", "ca_date", dtData.Rows[i]["ca_dt"].ToString(), true, curCon)) > 0)
                            {
                                strSql = " Update #tmpfcaction ";
                                strSql += " Set ca_rate1 = Case ca_type When 'D' then  Case Ca_value When '' then 0 else Convert(decimal(15,2),Ca_value) end when 'B' then Left(Rtrim(Ltrim(ca_value)), charindex(':',Rtrim(Ltrim(ca_value))) - 1) when 'S' then Convert(decimal(15,2),Ca_value) else 0 end , ";
                                strSql += " ca_rate2 = Case ca_type When 'B' then SUBSTRING(Rtrim(Ca_value),charindex(':',Rtrim(Ca_value)) + 1,LEN(Rtrim(Ca_value))) else 0 end where ca_type <> 'M'  ";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                myLib.ExecSQL(strSql, curCon);

                                strSql = "Update #tmpfcaction Set ca_Amt = ca_OldQty * Ca_Rate1 where ca_type in ('D')  ";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                myLib.ExecSQL(strSql, curCon);

                                strSql = "Update #tmpfcaction Set ca_NewQty = ca_OldQty * Ca_Rate1 where ca_type in ('S')  ";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                myLib.ExecSQL(strSql, curCon);

                                strSql = "Update #tmpfcaction Set ca_NewQty = floor(ca_OldQty * (Ca_Rate1 + Ca_Rate2) / Ca_Rate2) where ca_type = 'B'";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                myLib.ExecSQL(strSql, curCon);

                                strSql = "Update #tmpfcaction Set ca_Amt = (ca_OldQty * (Ca_Rate1 + Ca_Rate2) / Ca_Rate2 ) - ca_NewQty where ca_type = 'B'";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                myLib.ExecSQL(strSql, curCon);

                                strSql = "Update #tmpfcaction Set ca_NewQty = floor(ca_OldQty * 1) where ca_type = 'M'";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                myLib.ExecSQL(strSql, curCon);

                                strSql = "select ca_clientcd,ca_scripcd,sum(ca_NewQty-ca_OldQty) ca_NewQty, ";
                                strSql += " 'Bonus (' + Rtrim(CONVERT(char,ca_value)) + ')' Remark ";
                                strSql += " from #tmpfcaction ";
                                strSql += " where ca_YN='Y' and ca_Type in ('B')";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                strSql += " Group by ca_clientcd,ca_scripcd,ca_value Having sum(ca_NewQty-ca_OldQty) > 0 ";
                                dtRst = myLib.OpenDataTable(strSql, curCon);
                                if (dtRst.Rows.Count > 0)
                                {
                                    for (k = 0; k <= dtRst.Rows.Count - 1; k++)
                                    {
                                        strSql = " insert into #TmpTRX values('" + dtData.Rows[i]["ca_dt"] + "','','C','',";
                                        strSql += " '" + dtRst.Rows[k]["ca_clientcd"] + "',";
                                        strSql += " '" + dtRst.Rows[k]["ca_scripcd"] + "','B'," + dtRst.Rows[k]["ca_NewQty"] + ",0,0,0,0,'X','" + dtRst.Rows[k]["Remark"] + "')";
                                        myLib.ExecSQL(strSql, curCon);
                                    }
                                }

                                strSql = "select ca_clientcd,ca_scripcd,Ca_Rate1,sum(ca_NewQty) ca_NewQty,Sum(ca_OldQty) ca_OldQty, ";
                                strSql += " 'Split (' + Rtrim(CONVERT(char,CONVERT(numeric, Ca_Value))) + ') of ' + CONVERT(char,CONVERT(datetime, ca_date),103) Remark ";
                                strSql += " from #tmpfcaction  ";
                                strSql += " where ca_YN='Y' and ca_Type in ('S')";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                strSql += " Group by ca_clientcd,ca_scripcd,Ca_Rate1,Ca_Value,ca_date Having sum(ca_NewQty-ca_OldQty) > 0 ";
                                dtRst = myLib.OpenDataTable(strSql, curCon);
                                if (dtRst.Rows.Count > 0)
                                {
                                    for (k = 0; k <= dtRst.Rows.Count - 1; k++)
                                    {
                                        double dblOldQty = Convert.ToDouble(dtRst.Rows[k]["ca_OldQty"]);

                                        strSql = " select case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end Tmp_dt , Sum(Tmp_bqty-Tmp_Sqty) NetQty ";
                                        strSql += " From #TmpTRX left join Settlements on Tmp_Stlmnt = se_stlmnt ";
                                        strSql += " Where case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end <= '" + dtData.Rows[i]["ca_dt"] + "' and Tmp_clientcd = '" + dtRst.Rows[k]["ca_clientcd"].ToString().Trim() + "'";
                                        strSql += " and Tmp_scripcd = '" + dtRst.Rows[k]["ca_scripcd"].ToString().Trim() + "' and TMp_Flag <> 'T'";
                                        strSql += " Group By case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end  ";
                                        strSql += " Having Sum(Tmp_bqty-Tmp_Sqty)  > 0 ";
                                        strSql += " Order by Tmp_dt desc ";
                                        dtFrom = myLib.OpenDataTable(strSql, curCon);
                                        for (l = 0; l <= dtFrom.Rows.Count - 1; l++)
                                        {
                                            double lngDLVQty = Math.Abs(Convert.ToDouble(dtFrom.Rows[l]["NetQty"]));

                                            lngDLVQty = Convert.ToDouble((lngDLVQty > dblOldQty) ? dblOldQty : lngDLVQty);

                                            strSql = "select Tmp_SRNo,Tmp_bqty+Tmp_sqty NetQty from #TmpTRX left join Settlements on Tmp_Stlmnt = se_stlmnt ";
                                            strSql += " Where case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end = '" + dtFrom.Rows[l]["Tmp_dt"] + "' and Tmp_clientcd = '" + dtRst.Rows[k]["ca_Clientcd"] + "'";
                                            strSql += " and Tmp_scripcd = '" + dtRst.Rows[k]["ca_scripcd"] + "'";
                                            strSql += " and Tmp_BSFlag='B' and TMp_Flag <> 'T'";
                                            strSql += " Order by case Tmp_TRXFlag When 'C' Then 1 else 0 end";
                                            dtTRX = myLib.OpenDataTable(strSql, curCon);

                                            for (p = 0; p <= dtTRX.Rows.Count - 1; p++)
                                            {
                                                double dblQTY = Convert.ToDouble((Convert.ToDouble(dtTRX.Rows[p]["NetQty"]) > lngDLVQty) ? lngDLVQty : dtTRX.Rows[p]["NetQty"]);
                                                double dblNewQty = dblQTY * Convert.ToDouble(dtRst.Rows[k]["Ca_Rate1"]);

                                                if (Convert.ToDouble(dtTRX.Rows[p]["NetQty"]) > dblQTY)
                                                {
                                                    strSql = "Insert into #TmpTRX ";
                                                    strSql += " select Tmp_dt,Tmp_Stlmnt,Tmp_TRXFlag,'',Tmp_clientcd,Tmp_scripcd,Tmp_bsflag,Tmp_bqty" + -dblQTY + ",Tmp_Sqty,Tmp_Rate,Tmp_Chrgs, Tmp_STT,'X',''";
                                                    strSql += " From #TmpTRX Where Tmp_SRNo = " + dtTRX.Rows[p]["Tmp_SRNo"];
                                                    myLib.ExecSQL(strSql, curCon);
                                                }

                                                strSql = "Update #TmpTRX set Tmp_TRXFlag='C',Tmp_bqty=" + dblNewQty + ",Tmp_rate =Round(Tmp_rate/" + dtRst.Rows[k]["Ca_Rate1"] + ",4),";
                                                strSql += " Tmp_Chrgs=Round(Tmp_Chrgs/" + dtRst.Rows[k]["Ca_Rate1"] + ",4),Tmp_STT=Round(Tmp_STT/" + dtRst.Rows[k]["Ca_Rate1"] + ",4),Tmp_Remark='" + dtRst.Rows[k]["Remark"] + "'";
                                                strSql += " Where Tmp_SRNo = " + dtTRX.Rows[p]["Tmp_SRNo"];
                                                myLib.ExecSQL(strSql, curCon);

                                                lngDLVQty -= Convert.ToDouble(dtTRX.Rows[p]["NetQty"]);
                                                dblOldQty -= Convert.ToDouble(dtTRX.Rows[p]["NetQty"]);
                                                if (lngDLVQty <= 0)
                                                    break;
                                                if (dblOldQty <= 0)
                                                    break;
                                            }
                                            if (dblOldQty <= 0)
                                                break;
                                        }
                                    }
                                }

                                strSql = "select ca_clientcd,ca_scripcd,Ca_Rate1,sum(ca_NewQty) ca_NewQty,Sum(ca_OldQty) ca_OldQty, ";
                                strSql += " 'Consolidation (' + Rtrim(CONVERT(char,CONVERT(numeric(18,6), Ca_Value))) + ') of ' + CONVERT(char,CONVERT(datetime, ca_date),103) Remark ";
                                strSql += " from #tmpfcaction  ";
                                strSql += " where ca_YN='Y' and ca_Type in ('S')";
                                strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                                strSql += " and ca_scripcd = '" + dtCorpAct.Rows[j]["Tmp_scripcd"] + "' and ca_type = '" + dtCorpAct.Rows[j]["ca_purpose"] + "'";
                                strSql += " Group by ca_clientcd,ca_scripcd,Ca_Rate1,Ca_Value,ca_date Having sum(ca_OldQty-ca_NewQty) > 0 ";
                                dtRst = myLib.OpenDataTable(strSql, curCon);
                                int h;
                                if (dtRst.Rows.Count > 0)
                                {
                                    for (h = 0; h <= dtRst.Rows.Count - 1; h++)
                                    {
                                        double dblOldQty = Convert.ToDouble(dtRst.Rows[h]["ca_OldQty"]);

                                        strSql = " select case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end Tmp_dt , Sum(Tmp_bqty-Tmp_Sqty) NetQty ";
                                        strSql += " From #TmpTRX left join Settlements on Tmp_Stlmnt = se_stlmnt ";
                                        strSql += " Where case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end <= '" + dtData.Rows[i]["ca_dt"] + "' and Tmp_clientcd = '" + dtRst.Rows[h]["ca_clientcd"].ToString().Trim() + "'";
                                        strSql += " and Tmp_scripcd = '" + dtRst.Rows[h]["ca_scripcd"].ToString() + "' and TMp_Flag <> 'T'";
                                        strSql += " Group By case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end  ";
                                        strSql += " Having Sum(Tmp_bqty-Tmp_Sqty)  > 0 ";
                                        strSql += " Order by Tmp_dt desc ";
                                        dtFrom = myLib.OpenDataTable(strSql, curCon);
                                        int kdtFrom = 0;
                                        for (kdtFrom = 0; kdtFrom <= dtFrom.Rows.Count - 1; kdtFrom++)
                                        {
                                            double lngDLVQty = Math.Abs(Convert.ToDouble(dtFrom.Rows[kdtFrom]["NetQty"]));

                                            lngDLVQty = Convert.ToDouble((lngDLVQty > dblOldQty) ? dblOldQty : lngDLVQty);

                                            strSql = "select Tmp_SRNo,Tmp_bqty+Tmp_sqty NetQty from #TmpTRX left join Settlements on Tmp_Stlmnt = se_stlmnt ";
                                            strSql += " Where case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end = '" + dtFrom.Rows[kdtFrom]["Tmp_dt"] + "' and Tmp_clientcd = '" + dtRst.Rows[h]["ca_Clientcd"] + "'";
                                            strSql += " and Tmp_scripcd = '" + dtRst.Rows[h]["ca_scripcd"] + "'";
                                            strSql += " and Tmp_BSFlag='B' and TMp_Flag <> 'T'";
                                            strSql += " Order by case Tmp_TRXFlag When 'C' Then 1 else 0 end";
                                            dtTRX = myLib.OpenDataTable(strSql, curCon);
                                            int k1;
                                            for (k1 = 0; k1 <= dtTRX.Rows.Count - 1; k1++)
                                            {
                                                double dblQTY = Convert.ToDouble((Convert.ToDouble(dtTRX.Rows[k1]["NetQty"]) > lngDLVQty) ? lngDLVQty : dtTRX.Rows[k1]["NetQty"]);
                                                double dblNewQty = dblQTY * Convert.ToDouble(dtRst.Rows[h]["Ca_Rate1"]);

                                                if (Convert.ToDouble(dtTRX.Rows[k1]["NetQty"]) > dblQTY)
                                                {
                                                    strSql = "Insert into #TmpTRX ";
                                                    strSql += " select Tmp_dt,Tmp_Stlmnt,Tmp_TRXFlag,'',Tmp_clientcd,Tmp_scripcd,Tmp_bsflag,Tmp_bqty" + -dblQTY + ",Tmp_Sqty,Tmp_Rate,Tmp_Chrgs,Tmp_STT,'X',''";
                                                    strSql += " From #TmpTRX Where Tmp_SRNo = " + dtTRX.Rows[k1]["Tmp_SRNo"];
                                                    myLib.ExecSQL(strSql, curCon);
                                                }

                                                strSql = "Update #TmpTRX set Tmp_TRXFlag='C',Tmp_bqty=" + dblNewQty + ",Tmp_rate =Round(Tmp_rate/" + dtRst.Rows[h]["Ca_Rate1"] + ",4),";
                                                strSql += " Tmp_Chrgs=Round(Tmp_Chrgs/" + dtRst.Rows[h]["Ca_Rate1"] + ",4),Tmp_STT=Round(Tmp_STT/" + dtRst.Rows[h]["Ca_Rate1"] + ",4),Tmp_Remark='" + dtRst.Rows[h]["Remark"] + "'";
                                                strSql += " Where Tmp_SRNo = " + dtTRX.Rows[k1]["Tmp_SRNo"];
                                                myLib.ExecSQL(strSql, curCon);

                                                lngDLVQty -= Convert.ToDouble(dtTRX.Rows[k1]["NetQty"]);
                                                dblOldQty -= Convert.ToDouble(dtTRX.Rows[k1]["NetQty"]);
                                                if (lngDLVQty <= 0)
                                                    break;
                                                if (dblOldQty <= 0)
                                                    break;
                                            }
                                            if (dblOldQty <= 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if ((dtData.Rows[i]["xtype"]).ToString().Trim() == "X")
                    {
                        strSql = "Insert into #tmpfcaction ";
                        strSql += " select Tmp_clientcd,Tmp_scripcd,sum(Tmp_bqty - Tmp_sqty),0,0,'Y','X' MD_purpose,0,MD_QtyRatio,MD_ValueRatio,'" + dtData.Rows[i]["CA_dt"] + "'";
                        strSql += " from #TmpTRX left join Settlements on Tmp_Stlmnt = se_stlmnt,MergerDmerger";
                        strSql += " where case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end <= MD_dt and TMp_Flag <> 'T' ";
                        strSql += " and Tmp_scripcd = MD_scripcd and MD_dt = '" + dtData.Rows[i]["CA_dt"] + "' and MD_IsOldNew = 'O'";
                        strSql += " Group By Tmp_clientcd,Tmp_scripcd,MD_QtyRatio,MD_ValueRatio";
                        strSql += " Having sum(Tmp_bqty - Tmp_sqty)>0";
                        myLib.ExecSQL(strSql, curCon);

                        if (Convert.ToInt32(myLib.fnFireQuery("#tmpfcaction", "count(0)", "ca_Type", "X", true, curCon)) > 0)
                        {
                            strSql = "select ca_clientcd,ca_scripcd,ca_date,ca_oldQty, ca_NewQty, ca_Rate1,ca_Rate2  ";
                            strSql += " from #tmpfcaction ";
                            strSql += " where ca_YN='Y' and ca_Type in ('X') and ca_oldQty > 0 ";
                            strSql += " and ca_date = '" + dtData.Rows[i]["ca_dt"] + "'";
                            dtRst = myLib.OpenDataTable(strSql, curCon);
                            if (dtRst.Rows.Count > 0)
                            {
                                int j1 = 0; ;
                                for (j1 = 0; j1 <= dtRst.Rows.Count - 1; j1++)

                                {

                                    prCreateTempTableMerger(curCon);
                                    strSql = " select * from #TmpTRX left join Settlements on Tmp_Stlmnt = se_stlmnt ";
                                    strSql += " where case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end < '" + dtData.Rows[i]["CA_dt"] + "' and TMp_Flag <> 'T' and Tmp_BSFLAG='B' ";
                                    strSql += " and Tmp_Clientcd = '" + dtRst.Rows[j1]["ca_clientcd"] + "' and Tmp_Scripcd = '" + dtRst.Rows[j1]["ca_scripcd"] + "'";
                                    strSql += " Order by case Tmp_TRXFlag When 'N' Then se_payoutdt else Tmp_Dt end desc ";

                                    DataTable dtMergerTRX = myLib.OpenDataTable(strSql, curCon);

                                    if (dtMergerTRX.Rows.Count > 0)
                                    {
                                        double dblMergerQty = Convert.ToDouble(dtRst.Rows[j1]["ca_OldQty"]);
                                        int intMerger = 0;

                                        while (dblMergerQty > 0)
                                        {
                                            strSql = " Insert into #TmpTRXMERGER Values ('" + dtMergerTRX.Rows[intMerger]["Tmp_Dt"] + "','','C',";
                                            strSql += "'" + dtMergerTRX.Rows[intMerger]["Tmp_Clientcd"] + "','" + dtMergerTRX.Rows[intMerger]["Tmp_Scripcd"] + "','B',";
                                            strSql += ((Convert.ToDouble(dtMergerTRX.Rows[intMerger]["Tmp_BQty"]) > dblMergerQty) ? dblMergerQty : dtMergerTRX.Rows[intMerger]["Tmp_BQty"]) + ",0,";
                                            if (int.Parse(dtRst.Rows[j1]["ca_Rate1"].ToString()) == 1 && int.Parse(dtRst.Rows[j1]["ca_Rate2"].ToString()) > 0)

                                                strSql += Math.Round(Convert.ToDouble(dtMergerTRX.Rows[intMerger]["Tmp_Rate"]) + Convert.ToDouble(dtMergerTRX.Rows[intMerger]["Tmp_Chrgs"]) + Convert.ToDouble(dtMergerTRX.Rows[intMerger]["Tmp_STT"]), 4) + "," + 0 + "," + 0;
                                            else
                                                strSql += dtMergerTRX.Rows[intMerger]["Tmp_Rate"] + "," + dtMergerTRX.Rows[intMerger]["Tmp_Chrgs"] + "," + dtMergerTRX.Rows[intMerger]["Tmp_STT"];
                                            strSql += ",'X','Merger/Dmerger of " + myutil.DbToDate(dtData.Rows[i]["ca_dt"].ToString()) + "')";
                                            myLib.ExecSQL(strSql, curCon);


                                            if (int.Parse(dtRst.Rows[j1]["ca_Rate1"].ToString()) == 1 && int.Parse(dtRst.Rows[j1]["ca_Rate2"].ToString()) > 0)
                                            {
                                                if (Convert.ToDouble(dtMergerTRX.Rows[intMerger]["Tmp_BQty"]) > dblMergerQty)
                                                {
                                                    strSql = "Insert into #TmpTRX ";
                                                    strSql += "select Tmp_Dt,Tmp_Stlmnt,Tmp_TRXFlag,Tmp_TRDType,Tmp_Clientcd,Tmp_Scripcd,Tmp_BSFlag," + dblMergerQty + " Tmp_BQty,Tmp_SQty,Tmp_Rate,Tmp_Chrgs,Tmp_STT,Tmp_Flag,Tmp_Remark from #TmpTRX ";
                                                    strSql += " Where Tmp_SRNO = " + dtMergerTRX.Rows[intMerger]["Tmp_SRNO"] + "";
                                                    myLib.ExecSQL(strSql, curCon);

                                                    strSql = "Update #TmpTRX set Tmp_BQty=Tmp_BQty-" + dblMergerQty + ",Tmp_Chrgs=0,Tmp_STT=0";
                                                    strSql += " Where Tmp_SRNO = " + dtMergerTRX.Rows[intMerger]["Tmp_SRNO"] + "";
                                                    myLib.ExecSQL(strSql, curCon);

                                                    strSql = "Update #TmpTRX set Tmp_Rate = Round((Tmp_Rate+Tmp_Chrgs+Tmp_STT)*" + dtRst.Rows[j1]["ca_Rate2"] + ",4),Tmp_Chrgs=0,Tmp_STT=0";
                                                    strSql += " Where Tmp_SRNO = " + myLib.OpenDataSet("SELECT IDENT_CURRENT('#TmpTRX')", curCon).Tables[""].Rows[0][0] + "";
                                                    myLib.ExecSQL(strSql, curCon);
                                                }
                                                else
                                                {
                                                    strSql = "Update #TmpTRX set Tmp_Rate = Round((Tmp_Rate+Tmp_Chrgs+Tmp_STT)*" + dtRst.Rows[j1]["ca_Rate2"] + ",4),Tmp_Chrgs=0,Tmp_STT=0";
                                                    strSql += " Where Tmp_SRNO = " + dtMergerTRX.Rows[intMerger]["Tmp_SRNO"] + "";
                                                    myLib.ExecSQL(strSql, curCon);
                                                }
                                            }

                                            dblMergerQty -= Convert.ToDouble(dtMergerTRX.Rows[intMerger]["Tmp_BQty"]);
                                            intMerger += 1;
                                            if (dblMergerQty <= 0)
                                                break;
                                            if (intMerger >= dtMergerTRX.Rows.Count)
                                                break;
                                        }
                                    }

                                    if (int.Parse(dtRst.Rows[j1]["ca_Rate1"].ToString()) == 0 && int.Parse(dtRst.Rows[j1]["ca_Rate2"].ToString()) == 0)
                                    {
                                        strSql = "Insert into #TmpTRX ";
                                        strSql += " select '" + dtData.Rows[i]["ca_dt"] + "' Tmp_Dt,Tmp_Stlmnt,Tmp_TRXFlag,'',Tmp_Clientcd,Tmp_Scripcd,'S' Tmp_BSFlag,0 Tmp_BQty,Tmp_BQty Tmp_SQty,Tmp_Rate,Tmp_Chrgs,0 Tmp_Stt,Tmp_Flag,Tmp_Remark";
                                        strSql += " from #TmpTRXMERGER ";
                                        myLib.ExecSQL(strSql, curCon);
                                    }
                                    else
                                    {
                                        strSql = "Insert into #TmpTRX ";
                                        strSql += " select '" + dtData.Rows[i]["ca_dt"] + "' Tmp_Dt ,Tmp_Stlmnt,Tmp_TRXFlag,'',Tmp_Clientcd,Tmp_Scripcd,'S' Tmp_BSFlag,0 Tmp_BQty,";
                                        strSql += " Round(Tmp_BQty-(Tmp_BQty*" + dtRst.Rows[j1]["ca_Rate1"] + "),0) Tmp_SQty,";
                                        strSql += " Round(Tmp_Rate*" + dtRst.Rows[j1]["ca_Rate2"] + ",4) Tmp_Rate,";
                                        strSql += " 0 Tmp_Chrgs,0 Tmp_Stt,Tmp_Flag,Tmp_Remark";
                                        strSql += " from #TmpTRXMERGER Where Round(Tmp_BQty-(Tmp_BQty*" + dtRst.Rows[j1]["ca_Rate1"] + "),0) > 0 ";
                                        myLib.ExecSQL(strSql, curCon);
                                    }

                                    strSql = " Select * from MergerDMerger";
                                    strSql += " Where md_Srno in ";
                                    strSql += " (Select md_Srno From MergerDMerger Where Md_scripcd = '" + dtRst.Rows[j1]["ca_scripcd"].ToString().Trim() + "' ";
                                    strSql += " and Md_dt = '" + dtRst.Rows[j1]["ca_date"].ToString().Trim() + "' and MD_IsOldNew='O')";
                                    strSql += " and MD_IsOldNew='N'";

                                    DataTable dtMerge = myLib.OpenDataTable(strSql, curCon);

                                    if (dtMerge.Rows.Count > 0)
                                    {
                                        int e = 0;
                                        for (e = 0; e <= dtMerge.Rows.Count - 1; e++)
                                            strSql = "Insert into #TmpTRX ";
                                        strSql += " select Tmp_Dt,Tmp_Stlmnt,Tmp_TRXFlag,'',Tmp_Clientcd,'" + dtMerge.Rows[e]["MD_Scripcd"] + "' Tmp_Scripcd,'B' Tmp_BSFlag,";
                                        strSql += " floor(Tmp_BQty*" + dtMerge.Rows[e]["MD_QtyRatio"] + ") Tmp_BQty,0 Tmp_SQty,";
                                        strSql += " Round(Tmp_Rate*" + dtMerge.Rows[e]["MD_ValueRatio"] + ",4) Tmp_Rate,";
                                        strSql += " 0 Tmp_Chrgs, 0 Tmp_Stt,Tmp_Flag,Tmp_Remark";
                                        strSql += " from #TmpTRXMERGER ";
                                        myLib.ExecSQL(strSql, curCon);

                                        strSql = " insert into #tmpfcaction ";
                                        strSql += " select Tmp_Clientcd ca_clientcd,'" + dtMerge.Rows[e]["MD_Scripcd"] + "' ca_scripcd,";
                                        strSql += " Round(Tmp_BQty*" + dtMerge.Rows[e]["MD_QtyRatio"] + " - floor(Tmp_BQty*" + dtMerge.Rows[e]["MD_QtyRatio"] + "),2) ca_OldQty,0 ca_NewQty,  ";
                                        strSql += " Round(Round(Tmp_BQty*" + dtMerge.Rows[e]["MD_QtyRatio"] + " - floor(Tmp_BQty*" + dtMerge.Rows[e]["MD_QtyRatio"] + "),2) * Round(Tmp_Rate*" + dtMerge.Rows[e]["MD_ValueRatio"] + ",4),2) ca_Amt ";
                                        strSql += " ,'Y' ca_YN,'D' ca_Type, 0 ca_Value,0 ca_Rate1,0 ca_Rate2,Tmp_dt ca_date ";
                                        strSql += " from #TmpTRXMERGER  ";
                                        strSql += " wHERE (Tmp_BQty* " + dtMerge.Rows[e]["MD_QtyRatio"] + " - floor(Tmp_BQty*" + dtMerge.Rows[e]["MD_QtyRatio"] + ")) > 0 ";
                                        myLib.ExecSQL(strSql, curCon);
                                    }
                                }
                            }
                        }
                    }
                }
                // DataTable Test = myLib.OpenDataTable("select * from #TmpTRX", curCon);
                myLib.ExecSQL("Delete From #tmpfcaction Where ca_type <> 'D'", curCon);
                myLib.ExecSQL("Delete from #TmpTRX where Tmp_TRXFLAG = 'C' and (Tmp_BQty+Tmp_SQty) = 0", curCon);
                // DataTable Test1 = myLib.OpenDataTable("select * from #tmpfcaction", curCon);
            }

        }



        public void prCreateTempTableGainLoss(SqlConnection curCon)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strSql = "";
            strSql = "Drop Table #TmpRates";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Create Table #TmpRates ( ";
            strSql += " T_Scripcd VarChar(6) Primary Key Clustered,";
            strSql += " T_Rate Money";
            strSql += " ) ";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Drop Table #TmpTRX";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Create Table #TmpTRX ( ";
            strSql += " Tmp_SRNO Numeric Identity(1,1) Primary Key Clustered,";
            strSql += " Tmp_Dt Char(8),";
            strSql += " Tmp_Stlmnt Char(9),";
            strSql += " Tmp_TRXFlag Char(1),";
            strSql += " Tmp_TRDType Char(2),";
            strSql += " Tmp_Clientcd VarChar(8),";
            strSql += " Tmp_Scripcd VarChar(6),";
            strSql += " Tmp_BSFlag Char(1),";
            strSql += " Tmp_BQty Numeric(18,3),";
            strSql += " Tmp_SQty Numeric(18,3),";
            strSql += " Tmp_Rate Money,";
            strSql += " Tmp_Chrgs Money,  ";
            strSql += " Tmp_STT Money ,  ";
            strSql += " Tmp_Flag Char(1), ";
            strSql += " Tmp_Remark VarChar(100))";
            myLib.ExecSQL(strSql, curCon);



            strSql = "Create Index #Idx_TmpTRX_Clientcd_Scripcd on #TmpTRX ( Tmp_Clientcd ,Tmp_Scripcd )";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Drop Table #TmpGainLoss";
            myLib.ExecSQL(strSql, curCon);


            strSql = "Create Table #TmpGainLoss ( ";
            strSql += " Tmp_SRNO Numeric Identity(1,1) primary key ,";
            strSql += " Tmp_Clientcd VarChar(8),";
            strSql += " Tmp_Scripcd VarChar(6),";
            strSql += " Tmp_Qty Numeric(18,3),";
            strSql += " Tmp_BDt Char(8),";
            strSql += " Tmp_BRate Money,";
            strSql += " Tmp_BChrg Money,";
            strSql += " Tmp_BStt Money,";
            strSql += " Tmp_SDt Char(8),";
            strSql += " Tmp_SRate Money,";
            strSql += " Tmp_SChrg Money,";
            strSql += " Tmp_SStt Money,";
            strSql += " Tmp_Flag Char(1),";
            strSql += " Tmp_MarketRate Money, ";
            strSql += " Tmp_BSFlag Char(1), ";
            strSql += " Tmp_Remark VarChar(100), ";
            strSql += " Tmp_Stlmnt Char(9))";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Create Index #Idx_TmpGainLoss_Clientcd_Scripcd on #TmpGainLoss (Tmp_Clientcd ,Tmp_Scripcd )";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Create Index #Idx_TmpGainLoss_SRNO on #TmpGainLoss (Tmp_SRNO)";
            myLib.ExecSQL(strSql, curCon);

            strSql = "drop table #tmpfcaction";
            myLib.ExecSQL(strSql, curCon);


            strSql = "Create table #tmpfcaction(";
            strSql += " ca_clientcd char(8) not null,";
            strSql += " ca_scripcd char(6) not null,";
            strSql += " ca_OldQty numeric(18,3) not null, ";
            strSql += " ca_NewQty numeric(18,3) not null, ";
            strSql += " ca_Amt Money Not null, ";
            strSql += " ca_YN char(1) Not null, ";
            strSql += " ca_Type char(1) Not null, ";
            strSql += " ca_Value varchar(10) Not null , ";
            strSql += " ca_Rate1 Numeric(18,6) Not null , ";
            strSql += " ca_Rate2 Numeric(18,6) Not null, ";
            strSql += " ca_date varchar(8)  Not null ";
            strSql += " )";
            myLib.ExecSQL(strSql, curCon);

        }

        public void prCreateTempTableMerger(SqlConnection curCon)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            string strSql = "";

            strSql = "Drop Table #TmpTRXMERGER";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Create Table #TmpTRXMERGER ( ";
            strSql += " Tmp_SRNO Numeric Identity(1,1),";
            strSql += " Tmp_Dt Char(8),";
            strSql += " Tmp_Stlmnt Char(9),";
            strSql += " Tmp_TRXFlag Char(1),";
            strSql += " Tmp_Clientcd VarChar(8),";
            strSql += " Tmp_Scripcd VarChar(6),";
            strSql += " Tmp_BSFlag Char(1),";
            strSql += " Tmp_BQty Numeric(18,3),";
            strSql += " Tmp_SQty Numeric(18,3),";
            strSql += " Tmp_Rate Money,";
            strSql += " Tmp_Chrgs Money,  ";
            strSql += " Tmp_Stt Money,  ";
            strSql += " Tmp_Flag Char(1) ,";
            strSql += " Tmp_Remark VarChar(100))";
            myLib.ExecSQL(strSql, curCon);

        }


        public DataTable FnGetDividend(string Code, string strFromDt, string strToDt)
        {
            DataTable ObjDataTable;
            string strSql = "";
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                //prGainLoss[ClientCode, FromDt, ToDt, "", true];
                prGainLoss(Code, strFromDt, strToDt, "", true, "N", curCon);
                strSql = "SELECT ca_clientcd ClientCode,   ltrim(rtrim(convert(char,convert(datetime,ca_date),103)))  DivDate,ca_scripcd ScripCode,convert(decimal(15,0),ca_OldQty) Qty, convert(decimal(15,2),ca_Value)  Rate,convert(decimal(15,2),ca_Amt)  Amount,ss_lname ScripName From #tmpfcaction  , Securities Where ca_scripcd = ss_cd  and ca_type = 'D' and ca_amt>0";
                strSql += " and ca_date between '" + strFromDt + "' and '" + strToDt + "'";
                strSql += " Order by  ca_clientcd,ca_date,ss_lname ";
                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
            }
            return ObjDataTable;
        }


        public DataTable FnGetTradeListingSummary(string ClientCode, string FromDt, string ToDt)
        {
            DataTable ObjDataTable;
            string strSql = "";
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                strSql = "select td_scripcd , Rtrim(ss_lname) as ScripName,";
                strSql += " convert(decimal(15,0) ,sum(td_bqty)) Bqty,convert(decimal(15,2) ,case When sum(td_rate*td_bqty) > 0 then sum(td_rate*td_bqty)/sum(td_bqty) else 0 end) BRate,convert(decimal(15,2),sum(td_bqty*td_rate)) BAmt,";
                strSql += " convert(decimal(15,0) ,sum(td_sqty)) sqty,convert(decimal(15,2) ,case When sum(td_rate*td_sqty) > 0 then sum(td_rate*td_sqty)/sum(td_sqty) else 0 end) SRate,";
                strSql += " convert(decimal(15,2),sum(td_sqty*td_rate)) SAmt,convert(decimal(15,0) ,sum(td_bqty-td_sqty)) NetQty  ,convert(decimal(15,2) ,sum((td_bqty-td_sqty)*td_rate)) NAmt from TRX_INVPL,Securities ";
                strSql += " Where td_clientcd = '" + ClientCode + "' and td_scripcd = ss_cd and td_dt between '" + FromDt + "' and '" + ToDt + "'";
                strSql += " group by td_scripcd,Rtrim(ss_lname) order by Rtrim(ss_lname)";
                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
                return ObjDataTable;
            }




        }


        public DataTable FnGetTradeListingDetail(string ClientCode, string FromDt, string ToDt, string ScripCode)
        {
            DataTable ObjDataTable;
            string strSql = "";
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                strSql = " select td_srno,td_TRXFlag,case td_TRDType when 'DL' then 'Delivery' When 'SQ' then 'Jobbing' else '' end  td_TRDType , ";
                strSql += " ltrim(rtrim(convert(char,convert(datetime,td_dt),103))) as td_dt,td_stlmnt ,";
                strSql += " case td_bsflag when 'B' then 'Buy' when 'S' then 'Sell' else '' end td_bsflag , ";
                strSql += " convert(decimal(15,0) , td_bqty+td_sqty) as Qty, ";
                strSql += " convert(decimal(15,2) ,td_Rate) td_Rate,convert(decimal(15,2) ,td_Rate*(td_bqty+td_sqty)) Value , convert(decimal(15,2) ,td_ServiceTax*(td_bqty+td_sqty)) td_ServiceTax,";
                strSql += " convert(decimal(15,2) ,td_STT *(td_bqty+td_sqty)) td_STT,convert(decimal(15,2) ,td_OtherChrgs1*(td_bqty+td_sqty)) td_OtherChrgs1,convert(decimal(15,2) ,td_OtherChrgs2*(td_bqty+td_sqty)) td_OtherChrgs2 ";
                strSql += " from TRX_INVPL,Securities where td_clientcd='" + ClientCode + "' and td_scripcd = ss_cd ";
                strSql += " and td_scripcd = '" + ScripCode + "' and td_dt between '" + FromDt + "' and '" + ToDt + "' ";
                strSql += " order by  CONVERT (char,convert(datetime,td_dt),112)";
                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
                return ObjDataTable;
            }

        }
    }


}



