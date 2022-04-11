using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace TRADENET.Models
{
    public class INVPLProcess : ConnectionModel
    {
        string strSql;
        DataTable ObjDataTable;
        DataAccesslayer objDAL = new DataAccesslayer();
        bool blnIncSttDel;
        bool blnIncSttTrd;
        string strversion = "20180514";
        UtilityModel objutility = new UtilityModel();
        LibraryModel objlibrary = new LibraryModel();
        public DataTable FnGetNotionalSummary(string ClientCode, string strDate, string strignoresection = "N", string AdvisoryTrade = "I")
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
                int intMissing = 0;
                Boolean chkIgnoreSection = false;
                Boolean chkdelivery = false;

                Boolean chkjobing = false; ;
                //prGainLoss(ClientCode, FromDt, strDate, ScripCode, false, strignoresection, AdvisoryTrade, curCon);

                prGainLoss("Notional", ClientCode, FromDt, strDate, ScripCode, false, strignoresection, AdvisoryTrade, curCon);

                {
                    strSql = " select Tmp_Scripcd,Tmp_Flag,ss_lname , cast(Sum(case Tmp_Flag When 'S'  Then 0 else Tmp_Qty End) as decimal(15,0)) BQty," + Constants.vbNewLine;
                    strSql += " Sum(case Tmp_Flag When 'S'  Then 0 else  CAST((Tmp_BRate* Tmp_Qty) as decimal(15,2))  End) BAmount, cast(Sum(case Tmp_Flag When 'B'  Then 0 else Tmp_Qty End) as decimal(15,0)) SQty," + Constants.vbNewLine;
                    strSql += " Sum(case Tmp_Flag When 'B'  Then 0 else   cast((Tmp_SRate* Tmp_Qty) as decimal(15,2))  End) SAmount," + Constants.vbNewLine;
                    strSql += " cast(Sum (case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End else 0 End) as decimal(15,0))  NetQty," + Constants.vbNewLine;
                    strSql += " Sum(case When Tmp_Flag in ('B','S')  Then cast(((Tmp_BRate-Tmp_SRate)*Tmp_Qty) as decimal(15,2)) else 0 End) StockAtCost," + Constants.vbNewLine;
                    strSql += " cast(Sum(case When Tmp_Flag in ('B','S')  Then cast(((Tmp_BRate-Tmp_SRate)*Tmp_Qty) as decimal(15,2)) else 0 End) /  cast(Sum (case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End else 0 End) as decimal(15,0)) as decimal(15,2)) HoldingRate," + Constants.vbNewLine;
                    strSql += " Sum(case Tmp_Flag When 'T'  Then   cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2))  else 0 End) Trading,";
                    strSql += " Sum(case When Tmp_Flag = 'X' and dateDiff(d,Tmp_BDt,Tmp_SDt) < 365  Then cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2)) else 0 End) ShortTerm," + Constants.vbNewLine;
                    strSql += " Sum(case When Tmp_Flag ='X' and dateDiff(d,Tmp_BDt,Tmp_SDt) >= 365 Then  cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2)) else 0 End) LongTerm, cast(Tmp_MarketRate as decimal(15,2)) MarketRate," + Constants.vbNewLine;
                    strSql += " CAST((Sum(case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End*Tmp_MarketRate else 0 End)) as decimal(15,2)) StockAtMkt," + Constants.vbNewLine;
                    strSql += " CAST((Sum( case Tmp_Flag When 'B' Then (case when DATEDIFF(d,Tmp_BDt,'" + strDate + "') < 365 then Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) else 0 end)";
                    strSql += " When 'S' Then  (case when DATEDIFF(d,'" + strDate + "',Tmp_SDt) < 365 then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate)  else 0 end) ";
                    strSql += " else 0 end)) as decimal(15,2))  UnRealGainShort,";
                    strSql += " CAST((Sum( case Tmp_Flag When 'B' Then (case when DATEDIFF(d,Tmp_BDt,'" + strDate + "') >= 365 then case when ((Tmp_LTCG='*') and Tmp_MarketRate < Tmp_BRate+Tmp_BStt) then 0 else Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) end else 0 end)";
                    strSql += " When 'S' Then  (case when DATEDIFF(d,'" + strDate + "',Tmp_SDt) >= 365 then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate)  else 0 end)";
                    strSql += " else 0 end)) as decimal(15,2))  UnRealGainLong,";
                    strSql += " cast(sum((Tmp_BStt*Tmp_Qty)+ (Tmp_SStt*Tmp_Qty)) as decimal(15,2)) STT,Tmp_ISIN" + Constants.vbNewLine;
                    strSql += " from #TmpGainLoss,Securities Where Tmp_Flag in ('B','S') and  Tmp_Scripcd = ss_cd Group By Tmp_Scripcd,ss_lname ,Tmp_MarketRate ,Tmp_Flag,Tmp_ISIN " + Constants.vbNewLine;
                    strSql += " having abs(round(Sum (case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End else 0 End),0)) > 0";
                    strSql += "Order by Tmp_Flag, ss_lname ";
                }

                ObjDataTable = myLib.OpenDataTable(strSql, curCon);




                return ObjDataTable;
            }
        }

        public DataTable FnGetNotionalDetail(string ClientCode, string strDate, string ScripCode, string strignoresection = "N", string AdvisoryTrade = "I")
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
                // prGainLoss(ClientCode, strFromDt, strDate, ScripCode, false, strignoresection, AdvisoryTrade, curCon);
                prGainLoss("Notional", ClientCode, strFromDt, strDate, ScripCode, false, strignoresection, AdvisoryTrade, curCon);
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
        public DataTable FnGetActualPLSummary(string ClientCode, string FromDt, string ToDt, string Type, string strignoresection = "N", string AdvisoryTrade = "I")

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

                // prGainLoss(ClientCode, FromDt, ToDt, ScripCode, false, strignoresection, AdvisoryTrade, curCon);
                prGainLoss("ActualPL", ClientCode, FromDt, ToDt, ScripCode, false, strignoresection, AdvisoryTrade, curCon);
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

        public DataTable FnGetActualPLDetail(string ClientCode, string FromDt, string ToDt, string Type, string ScripCode, string strignoresection = "N", string AdvisoryTrade = "I")
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
                //prGainLoss(ClientCode, FromDt, ToDt, ScripCode, false, strignoresection, AdvisoryTrade, curCon);

                DataTable dt = new DataTable();



                prGainLoss("ActualPL", ClientCode, FromDt, ToDt, ScripCode, false, strignoresection, AdvisoryTrade, curCon);

                myLib.ExecSQL("Create Index #Idx_TmpGainLoss_Clientcd_Scripcd on #TmpGainLossRep (Tmp_Clientcd ,Tmp_Scripcd )");
                //myLib.ExecSQL("Alter Table #TmpGainLossRep Add Tmp_LTCG char(1) DEFAULT ('') NOT NULL ");
                //myLib.ExecSQL("Alter Table #TmpGainLossRep Add Tmp_ActualRate Money DEFAULT (0) NOT NULL ");
                //myLib.ExecSQL("Update #TmpGainLossRep Set Tmp_ActualRate = Tmp_BRate ");
                //myLib.ExecSQL("Alter Table #TmpGainLossRep Add Tmp_112ARate Money DEFAULT (0) NOT NULL ");

                strSql = " select Tmp_Clientcd,bm_branchcd,cm_name,cm_add1,cm_add2,cm_add3, cm_add4, cm_pincode,cm_tele1,cm_tele2,cm_panno,Tmp_Scripcd,ss_lname , cast(Sum(case Tmp_Flag When 'S'  Then 0 else Tmp_Qty End) as decimal(15,0)) BQty,";  // ,cm_email
                strSql += " Sum(case Tmp_Flag When 'S'  Then 0 else  CAST((Tmp_BRate* Tmp_Qty) as decimal(15,2))  End) BAmount, cast(Sum(case Tmp_Flag When 'B'  Then 0 else Tmp_Qty End) as decimal(15,0)) SQty,";
                strSql += " Sum(case Tmp_Flag When 'B'  Then 0 else   cast((Tmp_SRate* Tmp_Qty) as decimal(15,2))  End) SAmount,";
                strSql += " cast(Sum (case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End else 0 End) as decimal(15,0))  NetQty,";
                strSql += " Sum(case When Tmp_Flag in ('B','S')  Then cast(((Tmp_BRate-Tmp_SRate)*Tmp_Qty) as decimal(15,2)) else 0 End) StockAtCost,";
                strSql += " Sum(case Tmp_Flag When 'T' Then cast(((Tmp_SRate-Tmp_BRate)*Tmp_Qty) as decimal(15,2))  else 0 End) Trading,";
                strSql += " cast(sum((Tmp_BStt*Tmp_Qty)+ (Tmp_SStt*Tmp_Qty)) as decimal(15,2)) STT, ";
                strSql += " cast(Sum(case When Tmp_Flag ='X' and ( (ss_ChargeSTT = 'Y' and dateDiff(d,Tmp_BDt,Tmp_SDt) >= 365) Or (ss_ChargeSTT = 'N' and dateDiff(d,Tmp_BDt,Tmp_SDt) >= 365*3) ) Then  case when ((Tmp_LTCG='*') and (Tmp_SRate+Tmp_SStt) < Tmp_BRate) then 0 else ((Tmp_SRate-Tmp_BRate)*Tmp_Qty) end else 0 End)  as decimal(15,2)) LongTerm, ";
                strSql += " cast(Sum(case When Tmp_Flag ='X' and ( (ss_ChargeSTT = 'Y' and dateDiff(d,Tmp_BDt,Tmp_SDt) < 365) Or (ss_ChargeSTT = 'N' and dateDiff(d,Tmp_BDt,Tmp_SDt) < 365*3) ) Then  ((Tmp_SRate-Tmp_BRate)*Tmp_Qty)  else 0 End)  as decimal(15,2)) ShortTerm,";
                strSql += " Tmp_MarketRate MarketRate,Tmp_Flag,";
                strSql += " CAST((Sum(case When Tmp_Flag in ('B','S')  Then case Tmp_Flag When 'B' Then Tmp_Qty else -Tmp_Qty End * Tmp_MarketRate else 0 End)) as decimal(15,2)) StockAtMkt,";
                strSql += " CAST((Sum( case Tmp_Flag When 'B' Then Tmp_Qty*(Tmp_MarketRate-Tmp_BRate) When 'S' Then Tmp_Qty*(Tmp_SRate-Tmp_MarketRate) else 0 end)) as decimal(15,2))  UnRealGain ";
                strSql += " ,Tmp_SDt,Tmp_BDt from #TmpGainLoss,Securities,Client_Master,Branch_master " + Constants.vbNewLine;
                strSql += " Where  Tmp_Flag = 'S'  and cm_brboffcode = bm_branchcd and Tmp_Clientcd = cm_cd and Tmp_Scripcd = ss_cd " + Constants.vbNewLine;
                if (!string.IsNullOrEmpty(ClientCode.Trim()))
                {
                    strSql += " and  Tmp_Clientcd ='" + ClientCode + "'";
                }
                strSql += " Group By Tmp_Clientcd,bm_branchcd,cm_name,Tmp_Scripcd,ss_lname ,Tmp_MarketRate,Tmp_Flag,Tmp_SDt,Tmp_BDt ";
                strSql += " ,cm_add1,cm_add2,cm_add3, cm_add4, cm_pincode,cm_tele1,cm_tele2,cm_panno,cm_email  ";

                strSql += " Order by  ";
                strSql = strSql + Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(" cm_name,Tmp_Clientcd,", (Strings.Trim(Type) == "Notional" ? "Tmp_Flag," : "")), " ss_lname,"), (Strings.Trim(Type) == "Notional" ? "  case Tmp_Flag When  'B' Then Tmp_BDt when  'S' then Tmp_SDt end " : "Tmp_SDt,Tmp_BDt"));
                dt = myLib.OpenDataTable(strSql, curCon);

               
                
                
                HttpContext.Current.Session["missingtable"] = dt;



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
                strSql += " else 4 end) end  QtrSlab, TMp_Scripcd +' [ ' +  ss_lname  + ' ] ' as scripcode  ,cast((Tmp_BStt*Tmp_Qty) + (Tmp_SStt*Tmp_Qty) as decimal(15,2)) STT,Tmp_LTCG,Tmp_ActualRate,Tmp_112ARate";
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

                if (strignoresection == "N")
                {
                    ObjDataTable.Columns.Add("Tmp_112ARateN", typeof(string));
                    for (int I = 0; I < ObjDataTable.Rows.Count; I++)
                    {
                        if (!Convert.IsDBNull(ObjDataTable.Rows[I]["Tmp_112ARate"]))
                        {
                            if (ObjDataTable.Rows[I]["Tmp_LTCG"].ToString().Trim() == "*")
                            {
                                ObjDataTable.Rows[I]["Tmp_112ARateN"] = Convert.ToDecimal(ObjDataTable.Rows[I]["Tmp_112ARate"]).ToString("0.00") + ObjDataTable.Rows[I]["Tmp_LTCG"];
                            }
                            else
                            {
                                ObjDataTable.Rows[I]["Tmp_112ARateN"] = "NA";
                            }
                        }
                        else
                        {
                            ObjDataTable.Rows[I]["Tmp_112ARateN"] = "NA";
                        }
                    }
                }





            }
            return ObjDataTable;
        }


        private void prGainLoss(string ReportType, string strClient, string strFromDt, string strToDt, string strScripCd, bool blndividend = true, string strIgnoresection = "N", string AdvisoryTrade = "I", SqlConnection curCon = null)
        {
            DataTable dtTestFrom;
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            string strSql = "";
            bool blnTrdStlmnt;
            bool blnIncSttDel;
            bool blnIncSttTrd;

            //  strToDt = "20210219";
            DataTable dtFrom;
            DataTable dtTo;
            blnTrdStlmnt = Convert.ToBoolean((myLib.fnGetSysParam("TRDPLONSTLMT") == "Y") ? true : false);
            blnIncSttDel = Convert.ToBoolean((myLib.fnGetSysParam("GAINLOSSTTDL") == "Y") ? true : false);
            blnIncSttTrd = Convert.ToBoolean((myLib.fnGetSysParam("GAINLOSSTTTR") == "Y") ? true : false);
            //blnTrdStlmnt = false;
            //blnIncSttDel = true;
            //blnIncSttTrd = true;


            prCreateTempTableGainLoss(curCon);
            try
            {

                strSql = "Insert into #TmpTRX ";
                strSql += " select td_SRNO,td_NFiller2,td_dt, td_Stlmnt,td_TRXFlag ,td_TRDType,td_clientcd ,td_scripcd ,td_bsflag ,td_bqty,td_Sqty,td_Rate ,Round(td_ServiceTax+td_OtherChrgs1+td_OtherChrgs2,4),td_STT,td_Filler2,td_Remark,td_Filler1";
                strSql += " From TRX_INVPL, Client_mAster ,Branch_master Where cm_cd=td_clientcd and cm_brboffcode = bm_branchcd and td_dt <= '" + strToDt + "' and td_clientcd = '" + strClient + "'";
                //if (!string.IsNullOrEmpty(strScripCd))
                //    strSql += " and td_scripcd = '" + strScripCd + "'";

                //if (AdvisoryTrade == "E")
                //{
                //    strSql += " and td_NFiller1 <> 1 ";
                //}
                //else if (AdvisoryTrade == "O")
                //{
                //    strSql += " and td_NFiller1 = 1 ";


                if (strScripCd != "")
                {
                    strSql += " and td_scripcd = '" + strScripCd + "'";
                }

                if (ReportType == "Notional")
                {
                    // strSql += " and td_Filler2 = 'D' ";
                    strSql += " and isNull(td_Filler2,'') <> 'T' ";
                }

                if (AdvisoryTrade == "E")
                {
                    strSql += " and td_NFiller1 <> 1 ";
                }
                else if (AdvisoryTrade == "O")
                {
                    strSql += " and td_NFiller1 = 1 ";
                }

                strSql += " Order by td_clientcd,td_scripcd ,td_dt,td_TRXFlag,td_TRDType,td_Stlmnt,td_bsflag";
                myLib.ExecSQL(strSql, curCon);

                if (ReportType == "Notional")
                {
                }
                else
                {
                    strSql = " select * from #TmpTRX Where Tmp_RefNo > 0 Order by Tmp_tdSRNO ";

                    dtFrom = myLib.OpenDataTable(strSql, curCon);

                    for (int lngFrom = 0; lngFrom < dtFrom.Rows.Count; lngFrom++)
                    {
                        strSql = " Update a set a.Tmp_Bqty = a.Tmp_Bqty - b.Tmp_sqty ";
                        strSql += "  From #TmpTRX a , #TmpTRX b ";
                        strSql += " Where a.Tmp_tdSRNO = " + dtFrom.Rows[lngFrom]["Tmp_tdSRNO"] + " and b.Tmp_tdSRNO  = " + dtFrom.Rows[lngFrom]["Tmp_RefNo"];
                        myLib.ExecSQL(strSql, curCon);
                        strSql = " Delete #TmpTRX ";
                        strSql += " Where Tmp_tdSRNO = " + dtFrom.Rows[lngFrom]["Tmp_RefNo"];
                        myLib.ExecSQL(strSql, curCon);
                    }
                }
                strSql = " Delete #TmpTRX Where Tmp_BQTY+Tmp_SQTY = 0 ";
                myLib.ExecSQL(strSql, curCon);

                strSql = "Update #TmpTRX Set TMp_Flag = 'D'  Where TMp_Flag <> 'T' ";
                myLib.ExecSQL(strSql, curCon);


                if (ReportType == "Notional")
                {
                    // Closing

                    strSql = "Delete #TmpTRX Where TMp_Flag = 'T' and Tmp_Dt <= '" + strToDt + "'";
                    myLib.ExecSQL(strSql, curCon);

                    strSql = " select Tmp_Clientcd,Tmp_Scripcd,Sum(Tmp_Bqty-Tmp_Sqty) Net ";
                    strSql += " from #TmpTRX Where TMp_Flag = 'D' and Tmp_Dt <= '" + strToDt + "'";
                    strSql += " Group by Tmp_Clientcd,Tmp_Scripcd ";
                    strSql += " Having Sum(Tmp_Bqty-Tmp_Sqty) <> 0";
                    strSql += " Order by Tmp_Clientcd,Tmp_Scripcd ";
                    dtFrom = myLib.OpenDataTable(strSql, curCon);
                    if (dtFrom.Rows.Count > 0)
                    {
                        for (int lngFrom = 0, loopTo = dtFrom.Rows.Count - 1; lngFrom <= loopTo; lngFrom++)
                        {
                            double dblNetQty = 0;
                            dblNetQty = Math.Abs(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]));

                            //dblNetQty = Math.Abs(Convert.ToDouble("dfg"));
                            strSql = " select * from #TmpTRX Where TMp_Flag = 'D' and Tmp_Dt <= '" + strToDt + "' and Tmp_BSFlag = '" + Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, "B", "S") + "'";
                            strSql += " and Tmp_Clientcd = '" + dtFrom.Rows[lngFrom]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[lngFrom]["Tmp_Scripcd"] + "'";
                            strSql += " Order by Tmp_Dt desc,Tmp_Rate,Tmp_SRNO desc";
                            dtTo = myLib.OpenDataTable(strSql, curCon);


                            for (int lngTo = 0, loopTo1 = dtTo.Rows.Count - 1; lngTo <= loopTo1; lngTo++)


                            {
                                if (Microsoft.VisualBasic.CompilerServices.Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, dtTo.Rows[lngTo]["Tmp_BQty"], dtTo.Rows[lngTo]["Tmp_SQty"]), dblNetQty, false)))
                                {
                                    strSql = "Insert into #TmpTRX select Tmp_tdSRNO,Tmp_RefNo,Tmp_Dt,Tmp_Stlmnt,Tmp_TRXFlag,'',Tmp_Clientcd,Tmp_Scripcd,Tmp_BSFlag,";
                                    strSql += Operators.ConcatenateObject(Operators.ConcatenateObject(" ", Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_BQty"]) - dblNetQty, "Tmp_BQty")), " Tmp_BQty,");
                                    strSql += Operators.ConcatenateObject(Operators.ConcatenateObject(" ", Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) < 0, Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_SQty"]) - dblNetQty, "Tmp_SQty")), " Tmp_SQty,");
                                    strSql += " Tmp_Rate,Tmp_Chrgs,Tmp_STT,Tmp_Flag,Tmp_Remark,Tmp_filler1";
                                    strSql += " From #TmpTRX Where Tmp_SRNo = " + dtTo.Rows[lngTo]["Tmp_SRNO"];
                                    myLib.ExecSQL(strSql, curCon);
                                    strSql = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(" Update #TmpTRX set ", Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, "Tmp_BQty", "Tmp_SQty")), " = "), dblNetQty), " Where Tmp_SRNo = ")) + dtTo.Rows[lngTo]["Tmp_SRNO"];

                                    myLib.ExecSQL(strSql, curCon);
                                }

                                strSql = " Update #TmpTRX set Tmp_Flag = 'Z' Where Tmp_SRNo = " + Conversion.Val(dtTo.Rows[lngTo]["Tmp_SRNO"]);

                                myLib.ExecSQL(strSql, curCon);
                                dblNetQty -= (Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0 ? Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_BQty"]) : Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_SQty"]));

                                if (dblNetQty <= 0)
                                    break;
                            }
                        }
                    }

                    strSql = "Delete #TmpTRX Where TMp_Flag = 'D' and Tmp_Dt <= '" + strToDt + "'";
                    myLib.ExecSQL(strSql, curCon);
                    strSql = "Update #TmpTRX Set TMp_Flag = 'D'  Where TMp_Flag = 'Z' and Tmp_Dt <= '" + strToDt + "'";
                    myLib.ExecSQL(strSql, curCon);
                }
                // Closing
                else
                {
                    // Opening
                    strSql = " select Tmp_Clientcd,Tmp_Scripcd,Sum(Tmp_Bqty-Tmp_Sqty) Net ";
                    strSql += " from #TmpTRX Where TMp_Flag = 'D' and Tmp_Dt < '" + strFromDt + "'";
                    strSql += " Group by Tmp_Clientcd,Tmp_Scripcd ";
                    strSql += " Having Sum(Tmp_Bqty-Tmp_Sqty) <> 0";
                    strSql += " Order by Tmp_Clientcd,Tmp_Scripcd ";
                    dtFrom = myLib.OpenDataTable(strSql, curCon);
                    if (dtFrom.Rows.Count > 0)
                    {
                        for (int lngFrom = 0; lngFrom < dtFrom.Rows.Count; lngFrom++)
                        {
                            double dblNetQty = 0;
                            dblNetQty = Math.Abs(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]));
                            //dblNetQty = Math.Abs(Convert.ToDouble("dfg"));
                            strSql = " select * from #TmpTRX Where TMp_Flag = 'D' and Tmp_Dt < '" + strFromDt + "' and Tmp_BSFlag = '" + Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, "B", "S") + "'";
                            strSql += " and Tmp_Clientcd = '" + dtFrom.Rows[lngFrom]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[lngFrom]["Tmp_Scripcd"] + "'";
                            strSql += " Order by Tmp_Dt desc,Tmp_SRNO desc";

                            dtTo = myLib.OpenDataTable(strSql, curCon);

                            for (int lngTo = 0; lngTo < dtTo.Rows.Count; lngTo++)
                            {
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, dtTo.Rows[lngTo]["Tmp_BQty"], dtTo.Rows[lngTo]["Tmp_SQty"]), dblNetQty, false)))
                                {
                                    strSql = "Insert into #TmpTRX select Tmp_tdSRNO,Tmp_RefNo,Tmp_Dt,Tmp_Stlmnt,Tmp_TRXFlag,'',Tmp_Clientcd,Tmp_Scripcd,Tmp_BSFlag,";
                                    strSql += Operators.ConcatenateObject(Operators.ConcatenateObject(" ", Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_BQty"]) - dblNetQty, "Tmp_BQty")), " Tmp_BQty,");
                                    strSql += Operators.ConcatenateObject(Operators.ConcatenateObject(" ", Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_SQty"]) - dblNetQty, "Tmp_SQty")), " Tmp_SQty,");
                                    strSql += " Tmp_Rate,Tmp_Chrgs, Tmp_STT, Tmp_Flag,Tmp_Remark,Tmp_filler1";
                                    strSql += " From #TmpTRX Where Tmp_SRNo = " + dtTo.Rows[lngTo]["Tmp_SRNO"];
                                    myLib.ExecSQL(strSql, curCon);
                                    strSql = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(" Update #TmpTRX set ", Interaction.IIf(Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0, "Tmp_BQty", "Tmp_SQty")), " = "), dblNetQty), " Where Tmp_SRNo = ")) + dtTo.Rows[lngTo]["Tmp_SRNO"];
                                    myLib.ExecSQL(strSql, curCon);
                                }

                                strSql = " Update #TmpTRX set Tmp_Flag = 'Z' Where Tmp_SRNo = " + Conversion.Val(dtTo.Rows[lngTo]["Tmp_SRNO"]);
                                myLib.ExecSQL(strSql, curCon);
                                dblNetQty -= (Convert.ToDouble(dtFrom.Rows[lngFrom]["Net"]) > 0 ? Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_BQty"]) : Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_SQty"]));
                                if (dblNetQty <= 0)
                                    break;
                            }
                        }
                    }

                    strSql = "Delete #TmpTRX Where TMp_Flag = 'D' and Tmp_Dt < '" + strFromDt + "'";
                    myLib.ExecSQL(strSql, curCon);

                    strSql = "Update #TmpTRX Set TMp_Flag = 'D'  Where TMp_Flag = 'Z' and Tmp_Dt < '" + strFromDt + "'";
                    myLib.ExecSQL(strSql, curCon);
                    strSql = "Delete #TmpTRX Where TMp_Flag = 'T' and Tmp_Dt < '" + strFromDt + "'";
                    myLib.ExecSQL(strSql, curCon);
                }
                //dtTestFrom = myLib.OpenDataTable("select * from #TmpTRX", curCon);

                if (ReportType == "Notional")
                {

                    strSql = "Insert into #TmpGainLoss ";
                    strSql += " select Tmp_tdSRNO,Tmp_Clientcd,Tmp_Scripcd,case When Tmp_BSFlag = 'B' Then Tmp_BQty else Tmp_SQty End,case When Tmp_BSFlag = 'B' Then Tmp_Dt else '' end,case When Tmp_BSFlag = 'B' Then Tmp_Rate else 0 end,case When Tmp_BSFlag = 'B' Then Tmp_Chrgs else 0 end,case When Tmp_BSFlag = 'B' Then Tmp_STT else 0 end,";
                    strSql += " case When Tmp_BSFlag = 'S' Then Tmp_Dt else '' end,case When Tmp_BSFlag = 'S' Then Tmp_Rate else 0 end,case When Tmp_BSFlag = 'S' Then Tmp_Chrgs else 0 end,case When Tmp_BSFlag = 'S' Then Tmp_STT else 0 end,";
                    strSql += " Tmp_BSFlag Tmp_Flag,0, tmp_bsFlag, Tmp_Remark,Tmp_Stlmnt,Tmp_filler1";
                    strSql += " From #TmpTRX ";
                    strSql += " Order by Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt,Tmp_tdSRNO,Tmp_SRNO";
                    myLib.ExecSQL(strSql, curCon);
                }
                else
                {


                    // Opening
                    // //Trading Gain Loss
                    strSql = "Insert into #TmpGainLoss ";
                    strSql += " select Tmp_tdSRNO,Tmp_Clientcd,Tmp_Scripcd,Tmp_BQty,Tmp_Dt,Tmp_Rate,Tmp_Chrgs, Tmp_STT,'' Tmp_SDt,0 Tmp_SRate,0 Tmp_SChrg, 0 Tmp_SStt,'B' Tmp_Flag,0, Tmp_BsFLAg ,Tmp_Remark,Tmp_Stlmnt,Tmp_filler1";
                    strSql += " From #TmpTRX Where TMp_Flag = 'T' and Tmp_BSFlag = 'B'";
                    myLib.ExecSQL(strSql, curCon);

                    strSql = "select * from #TmpTRX Where TMp_Flag = 'T' and Tmp_BSFLAG = 'S'";
                    strSql += " Order by Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt";
                    dtFrom = myLib.OpenDataTable(strSql, curCon);

                    if (dtFrom.Rows.Count > 0)
                    {
                        for (int lngFrom = 0; lngFrom < dtFrom.Rows.Count; lngFrom++)
                        {
                            double dblQTy = Convert.ToDouble(dtFrom.Rows[lngFrom]["Tmp_SQty"]);
                            strSql = "select * from #TmpGainLoss with(index(#Idx_TmpGainLoss_Clientcd_Scripcd)) Where Tmp_Clientcd='" + dtFrom.Rows[lngFrom]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[lngFrom]["Tmp_Scripcd"] + "' and Tmp_BDt = '" + dtFrom.Rows[lngFrom]["Tmp_Dt"] + "' and Tmp_Flag='B'";
                            if (blnTrdStlmnt == true)
                            {
                                strSql += " and Tmp_Stlmnt = '" + dtFrom.Rows[lngFrom]["Tmp_Stlmnt"] + "'";
                            }

                            strSql += " Order by Tmp_BDt,Tmp_SRNO";
                            dtTo = myLib.OpenDataTable(strSql, curCon);

                            for (int lngTo = 0; lngTo < dtTo.Rows.Count; lngTo++)
                            {
                                if (Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_Qty"]) > dblQTy)
                                {

                                    strSql = "Insert into #TmpGainLoss";
                                    strSql += " select Tmp_tdSRNO,Tmp_Clientcd,Tmp_Scripcd," + (Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_Qty"]) - dblQTy) + ",Tmp_BDt,Tmp_BRate,Tmp_BChrg,Tmp_BStt,Tmp_SDt,Tmp_SRate,Tmp_SChrg,Tmp_SStt,Tmp_Flag,0, TMp_Bsflag,Tmp_Remark,Tmp_Stlmnt,Tmp_filler1";
                                    strSql += " From #TmpGainLoss";
                                    strSql += " Where Tmp_SRNO = " + dtTo.Rows[lngTo]["Tmp_SRNO"];
                                    myLib.ExecSQL(strSql, curCon);
                                    strSql = " Update #TmpGainLoss set Tmp_Qty = " + dblQTy;
                                    strSql += " Where Tmp_SRNO = " + dtTo.Rows[lngTo]["Tmp_SRNO"];
                                    myLib.ExecSQL(strSql, curCon);
                                }


                                strSql = " Update #TmpGainLoss set Tmp_Flag = 'T',Tmp_Sdt=" + dtFrom.Rows[lngFrom]["Tmp_Dt"] + ",Tmp_SRate=" + dtFrom.Rows[lngFrom]["Tmp_Rate"] + ",Tmp_SChrg=" + dtFrom.Rows[lngFrom]["Tmp_Chrgs"] + ", Tmp_SStt=" + dtFrom.Rows[lngFrom]["Tmp_STT"];
                                strSql += " Where Tmp_SRNO = " + dtTo.Rows[lngTo]["Tmp_SRNO"];
                                myLib.ExecSQL(strSql, curCon);
                                dblQTy -= Convert.ToDouble(dtTo.Rows[lngTo]["Tmp_Qty"]);
                                if (dblQTy <= 0)
                                    break;
                            }
                        }
                    }

                    //dtTestFrom = myLib.OpenDataTable("select * from #TmpGainLoss", curCon);
                    // Trading Gain Loss
                    // Delivery Gain Loss


                    strSql = "Insert into #TmpGainLoss ";
                    strSql += " select Tmp_tdSRNO,Tmp_Clientcd,Tmp_Scripcd,Tmp_BQty,Tmp_Dt,Tmp_Rate,Tmp_Chrgs, Tmp_STT,'' Tmp_SDt,0 Tmp_SRate,0 Tmp_SChrg, 0 Tmp_SStt,'B' Tmp_Flag,0, tmp_bsFlag,Tmp_Remark,Tmp_Stlmnt,Tmp_filler1";
                    strSql += " From #TmpTRX Where TMp_Flag <> 'T' and Tmp_BSFlag = 'B'";
                    strSql += " Order by Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt,Tmp_SRNO";
                    myLib.ExecSQL(strSql, curCon);

                    strSql = "select * from #TmpTRX Where TMp_Flag <> 'T' and Tmp_BSFLAG = 'S'";
                    strSql += " Order by Tmp_Clientcd,Tmp_Scripcd,Tmp_Dt,Tmp_tdSRNO";

                    dtFrom = myLib.OpenDataTable(strSql, curCon);
                    if (dtFrom.Rows.Count > 0)
                    {
                        int iFrom = 0;
                        for (iFrom = 0; iFrom < dtFrom.Rows.Count; iFrom++)
                        {
                            double dblQTy = Convert.ToDouble(dtFrom.Rows[iFrom]["Tmp_SQty"]);

                            strSql = "select * from #TmpGainLoss with(index(#Idx_TmpGainLoss_Clientcd_Scripcd)) Where Tmp_Clientcd='" + dtFrom.Rows[iFrom]["Tmp_Clientcd"] + "' and Tmp_Scripcd = '" + dtFrom.Rows[iFrom]["Tmp_Scripcd"] + "' and Tmp_BDt <= '" + dtFrom.Rows[iFrom]["Tmp_Dt"] + "' and Tmp_Flag='B'";
                            strSql += " Order by Tmp_BDt,Tmp_BRate desc,Tmp_tdSRNO,Tmp_SRNO";

                            dtTo = myLib.OpenDataTable(strSql, curCon);

                            int iTo = 0;
                            for (iTo = 0; iTo < dtTo.Rows.Count; iTo++)

                            {
                                if (Convert.ToDouble(dtTo.Rows[iTo]["Tmp_Qty"]) > dblQTy)
                                {
                                    strSql = "Insert into #TmpGainLoss";
                                    strSql += " select Tmp_tdSRNO,Tmp_Clientcd,Tmp_Scripcd," + (Convert.ToDouble(dtTo.Rows[iTo]["Tmp_Qty"]) - dblQTy) + ",Tmp_BDt,Tmp_BRate,Tmp_BChrg,Tmp_BStt, Tmp_SDt,Tmp_SRate,Tmp_SChrg,Tmp_SStt, Tmp_Flag,0, tmp_bsFlag ,tmp_remark,Tmp_Stlmnt,Tmp_filler1";
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

                                dblQTy -= Double.Parse((dtTo.Rows[iTo]["Tmp_Qty"].ToString()), System.Globalization.NumberStyles.Any);
                                if (Convert.ToDouble(dblQTy) <= 0)
                                    break;
                            }

                            if (Convert.ToDouble(dblQTy) > 0)
                            {
                                strSql = "Insert into #TmpGainLoss Values ('" + dtFrom.Rows[iFrom]["Tmp_TDSRNO"] + "','" + dtFrom.Rows[iFrom]["Tmp_Clientcd"] + "','" + dtFrom.Rows[iFrom]["Tmp_Scripcd"] + "'," + dblQTy + ",'',0,0,0,";
                                strSql += "'" + dtFrom.Rows[iFrom]["Tmp_dt"] + "'," + dtFrom.Rows[iFrom]["Tmp_Rate"] + "," + dtFrom.Rows[iFrom]["Tmp_Chrgs"] + "," + dtFrom.Rows[iFrom]["Tmp_STT"] + ",'S',0,'" + dtFrom.Rows[iFrom]["Tmp_BSFlag"] + "' , '" + dtFrom.Rows[iFrom]["Tmp_Remark"] + "','" + dtFrom.Rows[iFrom]["Tmp_Stlmnt"] + "','" + dtFrom.Rows[iFrom]["Tmp_Filler1"] + "' )";
                                myLib.ExecSQL(strSql, curCon);
                            }
                        }
                    }
                    //dtTestFrom = myLib.OpenDataTable("select * from #TmpGainLoss", curCon);

                    myLib.ExecSQL(" Delete from #TmpGainLoss where Tmp_Qty = 0", curCon);
                }

                //dtTestFrom = myLib.OpenDataTable("select * from #TmpGainLoss", curCon);
                // Delivery Gain Loss


                myLib.ExecSQL("insert into #TmpRates select distinct Tmp_Scripcd,0 from #TmpGainLoss", curCon);
                string strRMSVALATLTRT;
                strRMSVALATLTRT = myLib.GetSysPARM("RMSVALATLTRT");//GetSysParmSt



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

                myLib.ExecSQL("Alter Table #TmpGainLoss Add Tmp_ActualRate Money ", curCon);
                myLib.ExecSQL("Update #TmpGainLoss Set Tmp_ActualRate = Tmp_BRate ", curCon);
                myLib.ExecSQL("Alter Table #TmpGainLoss Add Tmp_112ARate Money ", curCon);

                if (strIgnoresection == "N" && fnchkTable(("Market_Rates" + fnRateYear() + "0131"), curCon) && (Convert.ToDouble(strToDt) > Convert.ToDouble(fnRateYear() + "0331")))
                {

                    strSQL = "alter table #TmpGainLoss add Tmp_112ARateFactor numeric ";
                    myLib.ExecSQL(strSQL, curCon);

                    strSQL = "Update #TmpGainLoss set Tmp_112ARateFactor = 1 ";
                    myLib.ExecSQL(strSQL, curCon);

                    strSQL = " select * from #TmpGainLoss Where Tmp_ReMark Like '%Split (%' and Tmp_ReMark Like '%)%' and Tmp_BDt > '" + fnRateYear() + "0131' ";
                    dtFrom = myLib.OpenDataTable(strSQL, curCon);
                    for (int lngFrom = 0; lngFrom < dtFrom.Rows.Count; lngFrom++)
                    {
                        double dblRatio = 0;
                        int intFromPos = 0;
                        int intToPos = 0;

                        if (Strings.InStr(dtFrom.Rows[lngFrom]["Tmp_Remark"].ToString(), "(") > 0 && Strings.InStr(dtFrom.Rows[lngFrom]["Tmp_Remark"].ToString(), ")") > 0)
                            intFromPos = Strings.InStr(dtFrom.Rows[lngFrom]["Tmp_Remark"].ToString(), "(") + 1;
                        intToPos = Strings.InStr(dtFrom.Rows[lngFrom]["Tmp_Remark"].ToString(), ")");
                        if ((intToPos - intFromPos) > 0)
                        {
                            dblRatio = Convert.ToDouble(Strings.Mid(dtFrom.Rows[lngFrom]["Tmp_Remark"].ToString(), intFromPos, intToPos - intFromPos));
                        }
                        if (dblRatio > 0)
                        {
                            strSql = "Update #TmpGainLoss set Tmp_112ARateFactor = " + dblRatio + " Where Tmp_SRNO = " + dtFrom.Rows[lngFrom]["Tmp_SRNO"].ToString() + "";
                            myLib.ExecSQL(strSql, curCon);
                        }
                    }

                    strSql = " Update #TmpGainLoss set Tmp_BDt = Tmp_Filler1  Where isDate(Tmp_Filler1) =  1 ";
                    myLib.ExecSQL(strSql, curCon);


                    strSql = "Update #TmpGainLoss  ";
                    strSql += " Set Tmp_BRate = Round(mk_Rate/Tmp_112ARateFactor,4),Tmp_112ARate = Round(mk_Rate/Tmp_112ARateFactor,4),Tmp_LTCG = '*', Tmp_BStt=0  ";
                    strSql += " From Market_Rates" + fnRateYear() + "0131, Securities";
                    strSql += " Where Tmp_scripcd = ss_cd and Tmp_scripcd = mk_scripcd and tmp_bdt <= '" + fnRateYear() + "0131' and tmp_sdt > '" + fnRateYear() + "0331' ";
                    strSql += " and DateDiff(d,Tmp_BDt,Tmp_SDt) >= 365    ";
                    strSql += " and Tmp_Flag = 'X' and Round(mk_Rate/Tmp_112ARateFactor,4) > (Tmp_BRate+Tmp_BStt) and ss_chargestt = 'Y'";
                    strSql += " and (Tmp_SRate-Tmp_SStt) > (Tmp_BRate+Tmp_BStt) ";
                    myLib.ExecSQL(strSql, curCon);

                    strSql = "Update #TmpGainLoss  ";
                    strSql += " Set Tmp_BRate = Round(mk_Rate/Tmp_112ARateFactor,4),Tmp_112ARate = Round(mk_Rate/Tmp_112ARateFactor,4),Tmp_LTCG = '*', Tmp_BStt=0  ";
                    strSql += " From Market_Rates" + fnRateYear() + "0131, Securities";
                    strSql += " Where Tmp_scripcd = ss_cd and Tmp_scripcd = mk_scripcd and tmp_bdt <= '" + fnRateYear() + "0131' ";
                    strSql += " and DateDiff(d,Tmp_BDt,'" + strToDt + "') >= 365    ";
                    strSql += " and Tmp_Flag = 'B' and Round(mk_Rate/Tmp_112ARateFactor,4) > (Tmp_BRate+Tmp_BStt) and ss_chargestt = 'Y'";
                    myLib.ExecSQL(strSql, curCon);


                    strSql = "Update #TmpGainLoss Set Tmp_112ARate = Round(mk_Rate/Tmp_112ARateFactor,4)  ";
                    strSql += " From Market_Rates" + fnRateYear() + "0131, Securities";
                    strSql += " Where Tmp_scripcd = ss_cd and Tmp_scripcd = mk_scripcd ";
                    myLib.ExecSQL(strSql, curCon);

                }
                else
                {
                    strSql = " Update #TmpGainLoss set Tmp_BDt = Tmp_Filler1  Where isDate(Tmp_Filler1) =  1 ";
                    myLib.ExecSQL(strSql, curCon);
                }
                myLib.ExecSQL("Alter Table #TmpGainLoss Add Tmp_ISIN char(12) DEFAULT ('') NOT NULL ", curCon);
                //dtTestFrom = myLib.OpenDataTable("select * from #TmpGainLoss", curCon);
                strSql = "update #TmpGainLoss set Tmp_ISIN = im_isin from ISIN Where im_scripcd = Tmp_scripcd and im_scripcd Not Between '600000' and '699999' " + " and im_priority = (Select min(im_priority) from ISIN Where im_scripcd = Tmp_scripcd and im_scripcd Not Between '600000' and '699999') ";
                myLib.ExecSQL(strSql, curCon);
            }
            catch (Exception ex)
            {
                //StackTrace st = new StackTrace(ex, true);
                ////Get the first stack frame
                //StackFrame frame1 = st.GetFrame[0];
                //StackFrame frame2 = st.GetFrame(1);
                //StackFrame frame3 = st.GetFrame(2);
                //int line = frame1.GetFileLineNumber();

                //throw new Exception(frame1 + "++++++" + frame2 + "++++++" + frame3 + "++++++" + st.FrameCount + "++++++" + ex.Message);

            }

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
            strSql += " Tmp_tdSRNO numeric,";
            strSql += " Tmp_RefNo numeric,";
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
            strSql += " Tmp_Remark VarChar(100),";
            strSql += " Tmp_Filler1 Char(8))";
            myLib.ExecSQL(strSql, curCon);



            strSql = "Create Index #Idx_TmpTRX_Clientcd_Scripcd on #TmpTRX ( Tmp_Clientcd ,Tmp_Scripcd )";
            myLib.ExecSQL(strSql, curCon);

            strSql = "Drop Table #TmpGainLoss";
            myLib.ExecSQL(strSql, curCon);


            strSql = "Create Table #TmpGainLoss ( ";
            strSql += " Tmp_SRNO Numeric Identity(1,1) primary key ,";
            strSql += " Tmp_TDSRNO Numeric,";
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
            strSql += " Tmp_Stlmnt Char(9),";
            strSql += " Tmp_Filler1 Char(8))";
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

            myLib.ExecSQL("if OBJECT_ID('tempdb..#TmpGainLossRep') is not null Drop Table #TmpGainLossRep", curCon);
            strSql = "Create Table #TmpGainLossRep ( ";
            strSql += " Tmp_SRNO Numeric Identity(1,1) Primary Key Clustered,";
            strSql += " Tmp_TDSRNO Numeric ,";
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
            strSql += " Tmp_Remark VarChar(100),";
            strSql += " Tmp_Stlmnt Char(9),";
            strSql += " Tmp_Filler1 Char(8),";
            strSql += "Tmp_LTCG char(1) DEFAULT('') NOT NULL";
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


        public DataTable FnGetDividend(string Code, string strFromDt, string strToDt, string AdvisoryTrade = "I")
        {
            DataTable ObjDataTable;
            string strSql = "";
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                //prGainLoss[ClientCode, FromDt, ToDt, "", true];
                // prGainLoss(Code, strFromDt, strToDt, "", true, "N", AdvisoryTrade, curCon);

                //strSql = "SELECT ca_clientcd ClientCode,   ltrim(rtrim(convert(char,convert(datetime,ca_date),103)))  DivDate,ca_scripcd ScripCode,convert(decimal(15,0),ca_OldQty) Qty, convert(decimal(15,2),ca_Value)  Rate,convert(decimal(15,2),ca_Amt)  Amount,ss_lname ScripName From #tmpfcaction  , Securities Where ca_scripcd = ss_cd  and ca_type = 'D' and ca_amt>0";
                //strSql += " and ca_date between '" + strFromDt + "' and '" + strToDt + "'";
                //strSql += " Order by  ca_clientcd,ca_date,ss_lname ";
                strSql = "SELECT DV_ClientCd ClientCode, ltrim(rtrim(convert(char,convert(datetime,DV_Dt),103)))  DivDate,DV_Scripcd ScripCode,convert(decimal(15,0),DV_NoOfShare) Qty, convert(decimal(15,2),DV_Amount/DV_NoOfShare)  Rate,convert(decimal(15,2),DV_Amount)  Amount,ss_lname ScripName From INVPL_DIVIDEND  , Securities Where DV_Scripcd = ss_cd and DV_Amount>0";
                strSql += " and DV_Dt between '" + strFromDt + "' and '" + strToDt + "' and DV_ClientCd = '" + Code + "'";
                if (AdvisoryTrade == "E")
                {
                    strSql += " and DV_Nfiller1 <> 1 ";
                }
                else if (AdvisoryTrade == "O")
                {
                    strSql += " and DV_Nfiller1 = 1 ";
                }

                strSql += " Order by  DV_ClientCd,DV_Dt,ss_lname ";
                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
            }
            return ObjDataTable;
        }


        public DataTable FnGetTradeListingSummary(string ClientCode, string FromDt, string ToDt, string AdvisoryTrade = "I")
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
                if (AdvisoryTrade == "E")
                {
                    strSql += " and td_NFiller1 <> 1 ";
                }
                else if (AdvisoryTrade == "O")
                {
                    strSql += " and td_NFiller1 = 1 ";
                }
                strSql += " group by td_scripcd,Rtrim(ss_lname) order by Rtrim(ss_lname)";
                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
                return ObjDataTable;
            }



        }


        public DataTable FnGetTradeListingDetail(string ClientCode, string FromDt, string ToDt, string ScripCode, string AdvisoryTrade = "")
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
                if (AdvisoryTrade != "")
                {
                    strSql += ",Case td_NFiller1 When 1 Then 'Yes' Else 'No' End Advisory ";
                }
                strSql += " from TRX_INVPL,Securities where td_clientcd='" + ClientCode + "' and td_scripcd = ss_cd ";
                strSql += " and td_scripcd = '" + ScripCode + "' and td_dt between '" + FromDt + "' and '" + ToDt + "' ";
                if (AdvisoryTrade == "E")
                {
                    strSql += " and td_NFiller1 <> 1 ";
                }
                else if (AdvisoryTrade == "O")
                {
                    strSql += " and td_NFiller1 = 1 ";
                }
                strSql += " order by  CONVERT (char,convert(datetime,td_dt),112)";
                ObjDataTable = myLib.OpenDataTable(strSql, curCon);
                return ObjDataTable;
            }

        }

        public string FnTradeInsert(string ClientCode, string ScripCode, string strDataXml)
        {
            DataTable ObjDataTable;
            var ObjTplusMOD = new TplusMOd();
            LibraryModel objutil = new LibraryModel();
            string strcomcode, strmsg = null, strsql;
            string strdate, strstlmnt, strTRDType, strbsflag, strmkrid;
            double dblqty, dblmarketrate, dblRate;
            int intAdvisory = 0;
            string strName = string.Empty;
            bool IsError = false;
            var IntrowAffected = default(int);
            string strxml;
            var SbErr = new StringBuilder();
            try
            {
                ObjDataTable = objDAL.OpendatatableXml(strDataXml);
                if (ObjDataTable.Rows.Count > 0)
                {
                    strcomcode = objDAL.fnFireQuery("Entity_master", "min(em_cd)", "Len(Ltrim(Rtrim(em_cd)))", "1", false);
                    strdate = ObjDataTable.Rows[0]["Date"].ToString();
                    strstlmnt = ObjDataTable.Rows[0]["stlmnt"].ToString();
                    strTRDType = ObjDataTable.Rows[0]["TRDType"].ToString();
                    strbsflag = ObjDataTable.Rows[0]["bsflag"].ToString();
                    strmkrid = Conversions.ToString(Interaction.IIf(string.IsNullOrEmpty(ObjDataTable.Rows[0]["mkrid"].ToString()), "Webservice", ObjDataTable.Rows[0]["mkrid"].ToString()));
                    dblqty = Conversions.ToDouble(ObjDataTable.Rows[0]["qty"]);
                    dblRate = Conversions.ToDouble(Interaction.IIf(ObjDataTable.Rows[0]["Rate"].ToString() == "", 0, ObjDataTable.Rows[0]["Rate"]));
                    strName = objlibrary.fnFireQuery("Client_master", "cm_name", "cm_cd", ClientCode, true);
                    for (int intColumn = 0, loopTo = ObjDataTable.Columns.Count - 1; intColumn <= loopTo; intColumn++)
                    {
                        if (ObjDataTable.Columns[intColumn].ToString().Trim() == "ADVISORY")
                        {
                            if (ObjDataTable.Rows[0]["Advisory"].ToString() == "1")
                            {
                                intAdvisory = 1;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(strName))
                    {
                        IsError = true;
                        strmsg = "Client  Not Found|";
                    }

                    strName = ObjTplusMOD.fnvalidate(ScripCode, 2);
                    if (string.IsNullOrEmpty(strName))
                    {
                        IsError = true;
                        strmsg += "Security  Not Found |";
                    }

                    if (string.IsNullOrEmpty(strdate))
                    {
                        IsError = true;
                        strmsg += "Date Cannot Be Blank|";
                    }

                    if (Fndatevalidate(strdate) == false)
                    {
                        IsError = true;
                        strmsg += "Invalid Date| ";
                    }

                    if (!string.IsNullOrEmpty(strstlmnt))
                    {
                        if (objutil.fnFireQuery("Settlements", "count(0)", "se_stlmnt", strstlmnt, true) == "0")
                        {
                            IsError = true;
                            strmsg += " Settlement not found|";
                        }
                    }

                    if (!(strTRDType == "DL" || strTRDType == "SQ"))
                    {
                        IsError = true;
                        strmsg += " Invalid  Trade  Type |";
                    }

                    if ((strbsflag == "B" || strbsflag == "S"))
                    {
                        IsError = true;
                        strmsg += " Invalid  B/S Flag|";
                    }

                    if (Conversion.Val(dblqty) <= 0d)
                    {
                        IsError = true;
                        strmsg += " Quantity Should be greater than zero|";
                    }

                    if (Conversion.Val(dblRate) < 0d)
                    {
                        IsError = true;
                        strmsg += " Rate Can not be Negative| ";
                    }

                    if (IsError == true)
                    {
                        SbErr.Append("<Response>");
                        SbErr.Append("<Status>N</Status>");
                        SbErr.Append("<Remarks> " + strmsg + "  </Remarks>");
                        SbErr.Append("</Response>");
                        return SbErr.ToString();
                    }

                    if (IsError == false)
                    {
                        strsql = " insert into TRX_INVPL(td_companycode, td_dt,td_stlmnt,td_TRXFlag,td_TRDType,td_clientcd,td_scripcd,td_bsflag,td_bqty,";
                        strsql += " td_Sqty,td_marketrate,td_brokerage,td_Rate,td_ServiceTax,td_STT,td_OtherChrgs1,td_OtherChrgs2,mkrid,mkrdt,";
                        strsql += " td_Filler1 ,td_Filler2,td_Filler3,td_NFiller1,td_NFiller2,td_NFiller3,td_Remark)";
                        strsql += " values('" + strcomcode + "','" + strdate + "', '" + strstlmnt.ToString().Trim() + "','M','" + strTRDType + "','" + Strings.Trim(ClientCode) + "','" + Strings.Trim(ScripCode) + "',";
                        strsql += "'" + ObjDataTable.Rows[0]["bsflag"].ToString().Trim() + "', ";
                        if (ObjDataTable.Rows[0]["bsflag"].ToString().Trim() == "B")
                        {
                            strsql += " " + dblqty + ",";
                            strsql += " 0 , ";
                        }
                        else
                        {
                            strsql += " 0, ";
                            strsql += "" + dblqty + ", ";
                        }

                        strsql += " " + dblRate + ",0," + dblRate + "," + ObjDataTable.Rows[0]["ServiceTax"] + ",";
                        strsql += " " + ObjDataTable.Rows[0]["STT"] + ", " + ObjDataTable.Rows[0]["OtherChrgs1"] + "," + ObjDataTable.Rows[0]["OtherChrgs2"] + ",'" + strmkrid + ("','" + DateTime.Today.Date.ToString("yyyyMMdd") + "',");
                        strsql += "'','',''," + intAdvisory + ",0,0,'')";
                        IntrowAffected = objDAL.ExecutesqlXml(strsql);
                    }

                    if (IntrowAffected > 0)
                    {
                        SbErr.Append("<Response>");
                        SbErr.Append("<Status>Y</Status>");
                        SbErr.Append("<Remarks>Saved</Remarks>");
                        SbErr.Append("</Response>");
                        return SbErr.ToString();
                    }
                }

                SbErr.Append("<Response>");
                SbErr.Append("<Status>N</Status>");
                SbErr.Append("<Remarks></Remarks>");
                SbErr.Append("</Response>");
                return SbErr.ToString();
            }
            catch (Exception ex)
            {
                SbErr.Append("<Response>");
                SbErr.Append("<Status>N</Status>");
                SbErr.Append("<Remarks>" + ex.Message.ToString() + "</Remarks>");
                SbErr.Append("</Response>");
                return SbErr.ToString();
            }
        }
        public string FnTradeUpdate(string ClientCode, string ScripCode, string strDataXml)
        {
            DataTable ObjDataTable;
            var ObjTplusMOD = new TplusMOd();
            LibraryModel lib = new LibraryModel();
            string strcomcode, strmsg = null, strsql;
            string strdate, strstlmnt, strTRDType, strbsflag, strmkrid, strflag;
            double dblqty, dblmarketrate, dblRate;
            int intAdvisory = 0;
            string strName = string.Empty;
            bool IsError = false;
            var IntrowAffected = default(int);
            var SbErr = new StringBuilder();
            bool blnAdvisoryFound = false;
            try
            {
                ObjDataTable = objDAL.OpendatatableXml(strDataXml);
                if (ObjDataTable.Rows.Count > 0)
                {
                    strflag = ObjDataTable.Rows[0]["Trxflag"].ToString().Trim();
                    strdate = ObjDataTable.Rows[0]["Date"].ToString();
                    strstlmnt = ObjDataTable.Rows[0]["stlmnt"].ToString();
                    strTRDType = ObjDataTable.Rows[0]["TRDType"].ToString();
                    strbsflag = ObjDataTable.Rows[0]["bsflag"].ToString().Trim();
                    dblqty = Convert.ToDouble(ObjDataTable.Rows[0]["qty"]);
                    dblRate = Convert.ToDouble(ObjDataTable.Rows[0]["Rate"]);
                    strName = lib.fnFireQuery("Client_master", "cm_name", "cm_cd", ClientCode, true);
                    for (int intColumn = 0, loopTo = ObjDataTable.Columns.Count - 1; intColumn <= loopTo; intColumn++)
                    {
                        if (ObjDataTable.Columns[intColumn].Caption.ToUpper() == "ADVISORY")
                        {
                            blnAdvisoryFound = true;
                            if (ObjDataTable.Rows[0]["Advisory"].ToString() == "1")
                            {
                                intAdvisory = 1;
                            }
                            else
                            {
                                intAdvisory = 0;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(strName))
                    {
                        IsError = true;
                        strmsg = "Client  Not Found|";
                    }

                    if (string.IsNullOrEmpty(strdate))
                    {
                        IsError = true;
                        strmsg += "Date Cannot Be Blank|";
                    }

                    if (Fndatevalidate(strdate) == false)
                    {
                        IsError = true;
                        strmsg += "Invalid Date| ";
                    }

                    if (!string.IsNullOrEmpty(strstlmnt))
                    {
                        if (lib.fnFireQuery("Settlements", "count(0)", "se_stlmnt", strstlmnt, true) == "0")
                        {
                            IsError = true;
                            strmsg += " Settlement not found|";
                        }
                    }

                    if (!(strTRDType == "DL" | strTRDType == "SQ"))
                    {
                        IsError = true;
                        strmsg += " Invalid  Trade  Type |";
                    }

                    if (!(strbsflag == "B" | strbsflag == "S"))
                    {
                        IsError = true;
                        strmsg += " Invalid  B/S Flag|";
                    }



                    if (Conversion.Val(dblqty) <= 0d)
                    {
                        IsError = true;
                        strmsg += " Quantity Should be greater than zero.|";
                    }

                    if (!(strflag == "O" | strflag == "M"))
                    {
                        IsError = true;
                        strmsg += " Invalid  flag|";
                    }

                    if (Conversion.Val(dblRate) < 0d)
                    {
                        IsError = true;
                        strmsg += " Rate Can not be Negative| ";
                    }

                    if (IsError == true)
                    {
                        SbErr.Append("<Error>");
                        SbErr.Append("<Status>N</Status>");
                        SbErr.Append("<Remarks> " + strmsg + "  </Remarks>");
                        SbErr.Append("</Error>");
                        return SbErr.ToString();
                    }

                    if (IsError == false)
                    {
                        if (strflag.ToString().Trim() == "M")
                        {
                            strsql = "UPDATE trx_invpl set td_dt = '" + strdate + "',td_stlmnt = '" + strstlmnt.ToString().Trim() + "',";
                            strsql += " td_bsflag = '" + strbsflag.ToString().Trim() + "',";
                            if (ObjDataTable.Rows[0]["bsflag"].ToString() == "B")
                            {
                                strsql += "td_bqty = " + dblqty + ",";
                                strsql += "td_sqty = 0 , ";
                            }
                            else
                            {
                                strsql += "td_sqty = " + dblqty + ",";
                                strsql += "td_bqty = 0 , ";
                            }

                            strsql += "td_TRDType= '" + strTRDType + "',td_marketrate = " + dblRate + ",";
                            strsql += "td_ServiceTax= " + ObjDataTable.Rows[0]["ServiceTax"] + " , td_STT = " + ObjDataTable.Rows[0]["STT"] + ", ";
                            strsql += " td_Rate = " + dblRate + ", td_OtherChrgs1=" + ObjDataTable.Rows[0]["OtherChrgs1"] + "";
                            strsql += ",td_OtherChrgs2= " + ObjDataTable.Rows[0]["OtherChrgs2"] + "";
                            if (blnAdvisoryFound)
                            {
                                strsql += ",td_NFiller1 = " + intAdvisory;
                            }

                            strsql += " where td_srno=" + ObjDataTable.Rows[0]["srno"].ToString() + ("  and  td_clientcd='" + ClientCode + "' and  td_TRXFlag='M'");
                            IntrowAffected = objDAL.ExecutesqlXml(strsql);

                        }
                        else if (strflag == "O")
                        {
                            strsql = "UPDATE trx_invpl set td_Rate = " + dblRate + ",td_marketrate = " + dblRate + "";
                            if (blnAdvisoryFound)
                            {
                                strsql += ",td_NFiller1 = " + intAdvisory;
                            }

                            strsql += " where td_srno=" + ObjDataTable.Rows[0]["srno"].ToString() + ("  and  td_clientcd='" + ClientCode + "' and td_TRXFlag='O'");
                            IntrowAffected = objDAL.ExecutesqlXml(strsql);

                        }

                        if (IntrowAffected > 0)
                        {
                            SbErr.Append("<Response>");
                            SbErr.Append("<Status>Y</Status>");
                            SbErr.Append("<Remarks>Saved</Remarks>");
                            SbErr.Append("</Response>");
                            return SbErr.ToString();
                        }
                    }
                }

                SbErr.Append("<Response>");
                SbErr.Append("<Status>N</Status>");
                SbErr.Append("<Remarks></Remarks>");
                SbErr.Append("</Response>");
                return SbErr.ToString();
            }
            catch (Exception ex)
            {
                SbErr.Append("<Response>");
                SbErr.Append("<Status>N</Status>");
                SbErr.Append("<Remarks>" + ex.Message.ToString() + "</Remarks>");
                SbErr.Append("</Response>");
                return SbErr.ToString();
            }
        }

        public string FnTradeDelete(string ClientCode, string strSRNO)
        {
            int IntrowAffected;
            string strSql = "delete from TRX_INVPL where td_srno=" + strSRNO + "  and  td_clientcd= '" + ClientCode + "' ";
            IntrowAffected = objDAL.ExecutesqlXml(strSql);
            var SbErr = new StringBuilder();
            if (IntrowAffected > 0)
            {
                SbErr.Append("<Response>");
                SbErr.Append("<Status>Y</Status>");
                SbErr.Append("<Remarks>Deleted</Remarks>");
                SbErr.Append("</Response>");
                return SbErr.ToString();
            }

            SbErr.Append("<Response>");
            SbErr.Append("<Status>N</Status>");
            SbErr.Append("<Remarks></Remarks>");
            SbErr.Append("</Response>");
            return SbErr.ToString();
        }

        public DataTable FnGetChargesDetails(string ClientCode, string FromDt, string ToDt)
        {

            string strSql = " select sh_clientcd ClientCode ,se_stdt TradeDate ,sh_stlmnt Stlmnt,sum(case when sh_recordsource='DMT' then sh_amount else 0 end) DematChrg , ";
            strSql += " sum(case when sh_recordsource='TRX' then sh_amount else 0 end) BSETrxChrg";
            strSql += " from SpecialCharges, Settlements";
            strSql += " where(se_stlmnt = sh_stlmnt)";
            strSql += " and se_stdt between '" + FromDt + "' and '" + ToDt + "'";
            strSql += " and sh_clientcd= '" + ClientCode + "'";
            strSql += " and sh_recordsource in ('DMT','TRX')";
            strSql += " group by sh_stlmnt,se_stdt,sh_clientcd";
            ObjDataTable = objDAL.OpenDataSet(strSql, false, 1000).Tables[0];
            return ObjDataTable;
        }

        public bool Fndatevalidate(string datestring)
        {
            DateTime dt;
            if (DateTime.TryParseExact(datestring, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public DataTable FnGetSLBMSummary(string ClientCode, string FromDt, string ToDt)
        {
            strSql = "select td_scripcd ScripCode, ss_lname ScripName, Cast(sum((td_sqty-td_bqty) * td_rate) as decimal(15,2)) as Amount from INVPL_SLBM, Securities ";
            strSql += " Where td_scripcd= ss_cd and td_dt between '" + FromDt + "' and '" + ToDt + "'";
            strSql += " and td_clientcd = '" + ClientCode + "'";
            strSql += " Group by td_scripcd,ss_lname ";
            ObjDataTable = objDAL.OpenDataSet(strSql, false, 1000).Tables[0];
            return ObjDataTable;
        }
        public DataTable FnGetSLBMDetail(string ClientCode, string FromDt, string ToDt)
        {
            strSql = "select td_dt Date, td_scripcd ScripCode, ss_lname ScripName,";
            strSql += "  Case td_bsflag when  'B' then 'Borrow' else 'Lend' end BSFlag,";
            strSql += "  Cast(Case td_bsflag when 'S' Then td_sqty else td_bqty end as decimal(15,0)) as Quantity , ";
            strSql += "  Cast(td_rate as decimal(15,2)) Rate , ";
            strSql += " Cast((Case td_bsflag when 'S' Then td_sqty else -td_bqty end) * td_Rate  as decimal(15,2)) Amount ";
            strSql += "  from INVPL_SLBM, Securities ";
            strSql += "  Where td_scripcd = ss_cd ";
            strSql += "  and td_dt between '" + FromDt + "' and '" + ToDt + "'";
            strSql += "  and td_clientcd = '" + ClientCode + "' ";
            strSql += "  Order by td_dt , ss_lname";
            ObjDataTable = objDAL.OpenDataSet(strSql, false, 1000).Tables[0];
            return ObjDataTable;
        }
        public DataTable FnGetCorporateAction(string ClientCode, string ScripCode, string FromDt, string ToDt, string IncCorpAct = "N")
        {
            strSql = fnGetQuery(ClientCode, ScripCode, FromDt, ToDt, IncCorpAct);
            ObjDataTable = objDAL.OpenDataSet(strSql, false, 1000).Tables[0];
            return ObjDataTable;
        }
        private string fnGetQuery(string ClientCode, string ScripCode, string FromDt, string ToDt, string IncCorpAct)
        {
            string fnGetQueryRet = null;
            string strFields;
            string strOrderBy;
            string strGroupby;
            string strWhere;
            string strcomcode;
            strcomcode = objDAL.fnFireQuery("Entity_master", "min(em_cd)", "Len(Ltrim(Rtrim(em_cd)))", "1", false);
            strFields = "";
            strOrderBy = "";
            strGroupby = "";
            strWhere = "";
            strFields = "td_clientcd ClientCode, cm_name ClientName, td_stlmnt stlmnt, td_dt TradeDate, ss_cd ScripCode, ss_name ScripName";
            strOrderBy = "Order By cm_name, td_clientcd, td_dt, td_stlmnt, ss_name, ss_cd";
            strSql = "select  " + strFields + Constants.vbNewLine;
            strSql += " ,case td_trxflag when 'N' then 'Normal' when 'C' then 'Corporate Action' when 'M' then 'Manual Entry' when 'O' then 'Opening' when 'I' then 'IPO Allotment' end as trxflag, " + Constants.vbNewLine;
            strSql += " Case td_TrdType When 'DL' then 'Delivery' when 'SQ' then 'Square Off' end as TrdType, " + Constants.vbNewLine;
            strSql += " cast(td_bqty as decimal(15,3)) BuyQty, cast(td_Sqty as decimal(15,3)) SellQty, cast((td_marketrate*(td_bqty+td_Sqty))/(td_bqty+td_Sqty) as decimal(15,3)) as Marketrate , " + Constants.vbNewLine;
            strSql += " cast((td_Rate*(td_bqty+td_Sqty))/(td_bqty+td_Sqty) as decimal(15,3)) NetRate, cast((td_ServiceTax*(td_bqty+td_Sqty)) as decimal(15,3)) ServiceTax,  " + Constants.vbNewLine;
            strSql += " cast((td_brokerage*(td_bqty+td_Sqty)) as decimal(15,3)) Brokerage, " + Constants.vbNewLine;
            strSql += " cast((td_STT*(td_bqty+td_Sqty)) as decimal(15,3)) STT, " + Constants.vbNewLine;
            strSql += " cast(((td_OtherChrgs1+td_OtherChrgs2)*(td_bqty+td_Sqty)) as decimal(15,3)) as OtherCharges," + Constants.vbNewLine;
            strSql += " cast((td_Rate*td_Bqty) as decimal(15,3)) BuyAmount," + Constants.vbNewLine;
            strSql += " cast((td_Rate*td_Sqty) as decimal(15,3)) SaleAmount," + Constants.vbNewLine;
            strSql += " cast((td_Rate*(td_Bqty-td_Sqty)) as decimal(15,3)) NetValue," + Constants.vbNewLine;
            strSql += " cast((td_Rate*(td_Bqty-td_Sqty))+((td_ServiceTax+td_STT+td_OtherChrgs1+td_OtherChrgs2)*(td_Bqty+td_Sqty)) as decimal(15,3))  BilledAmount,td_Remark Remark" + Constants.vbNewLine;
            strSql += " From TRX_INVPL, Client_master, Securities" + Constants.vbNewLine;
            strSql += " where td_clientcd = cm_cd and td_scripcd = ss_cd " + Constants.vbNewLine;
            strSql += " and td_dt between '" + FromDt + "' and '" + ToDt + "' " + Constants.vbNewLine;
            if (!string.IsNullOrEmpty(ClientCode.Trim()))
                strSql += " and cm_cd='" + ClientCode.Trim() + "'" + Constants.vbNewLine;
            if (IncCorpAct != "Y")
                strSql += " and td_trxFlag <> 'C' " + Constants.vbNewLine;
            strSql += strGroupby + Constants.vbNewLine;
            strSql += strOrderBy;
            fnGetQueryRet = strSql;
            return fnGetQueryRet;
        }

        public string INVPLREPROCESS()
        {
            LibraryModel lib = new LibraryModel();
            if (lib.fnFireQuery("process_log", "count(0)", "pr_process", "INVPLREPRC", true) == "0")
            {
                return "N";
            }
            else
            {
                return "Y";
            }
        }




    }
}