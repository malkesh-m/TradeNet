using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TRADENET.Models
{
    public class TradenetGlobalModel
    {
    }

    public class ConnectionModel
    {
        public string strSQL;
        public static string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ToString();
    }

    public class ClientModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class CodeNameAmountModel : ClientModel
    {
        public decimal Amt { get; set; }
        public decimal Amt2 { get; set; }

        public decimal futures { get; set; }
        public decimal options { get; set; }
    }

    public class ClientMasterModel
    {
        public string ClientCd { get; set; }
        public string ClientName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        public string City { get; set; }
        public string GroupCd { get; set; }
        public string FamilyCd { get; set; }
        public string BranchCd { get; set; }
        public string Status { get; set; }
    }

    public class ClientBrokMasterModel
    {
        public string ClientCd { get; set; }
        public string ClientName { get; set; }
        public string Panno { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string GroupCd { get; set; }
        public string GroupName { get; set; }
        public string FamilyCd { get; set; }
        public string FamilyName { get; set; }
        public string BranchCd { get; set; }
        public string BranchName { get; set; }
        public string CashBseBrk { get; set; }
        public string CashNSEBrok { get; set; }
        public string CashMCXBrok { get; set; }

        public string FOBseBrk { get; set; }
        public string FONSEBrok { get; set; }
        public string FOMCXBrok { get; set; }

        public string FXBseBrk { get; set; }
        public string FXNSEBrok { get; set; }
        public string FXMCXBrok { get; set; }

    }

    public class ClientKYCMasterModel
    {
        public string CK_fatherspouseflag { get; set; }
        public string CK_FatherPrefix { get; set; }
        public string CK_FatherFname { get; set; }

        public string CK_FatherMname { get; set; }
        public string CK_FatherLname { get; set; }

        public string CK_MotherPrefix { get; set; }

        public string CK_Motherfname { get; set; }

        public string CK_MotherMname { get; set; }

        public string CK_MotherLname { get; set; }

        public string CK_citizenship { get; set; }

        public string CK_Resistatus { get; set; }

        public string CK_CityOfBirth { get; set; }
        public string CK_CountyOfBirth { get; set; }

        public string CK_IdentityProof { get; set; }
        public string CK_IdentityProofID { get; set; }
        public string CK_IdentityProofExpDt { get; set; }


        public string CK_PermAddrProof { get; set; }
        public string CK_PermAddrProofID { get; set; }
        public string CK_PermAddrProofExpDt { get; set; }



        public string CK_AddrProof { get; set; }
        public string CK_AddrProofID { get; set; }
        public string CK_AddrProofExpDt { get; set; }

        public Decimal CK_SRNO { get; set; }

        public string CK_ActType { get; set; }

        public string Image0 { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }

    }


    public class BillSummaryModel : ClientModel
    {
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string Settlement { get; set; }
        public string Segment { get; set; }
        public string CompanyCode { get; set; }

        public string DueToUs { get; set; }
        public string DueToYou { get; set; }
        public Decimal BillNo { get; set; }
    }

    public class DPHoldingModel : ClientMasterModel
    {
        public string BOID { get; set; }
        public string BOName { get; set; }
        public string ISINCode { get; set; }
        public string CompName { get; set; }
        public string ISINName { get; set; }
        public decimal accPos { get; set; }
        public string accType { get; set; }
        public string BType { get; set; }
        public string BDescription { get; set; }
        public string MarketType { get; set; }
        public string Stlmnt { get; set; }
        public decimal secRate { get; set; }
        public decimal Valuation { get; set; }
    }

    public class RetainHoldingSummaryModel : ClientMasterModel
    {
        public string BType { get; set; }
        public string ScripCode { get; set; }
        public string Security { get; set; }

        public string ISIN { get; set; }

        public decimal Quantity { get; set; }

        public decimal Value { get; set; }

        public decimal Haircut { get; set; }

        public decimal ValueafterHaircut { get; set; }
    }

    

    public class LedgerModel : ClientMasterModel
    {
        public string cm_cd { get; set; }
        public string Date { get; set; }
        public string ExSeg { get; set; }
        public string Voucher { get; set; }
        public string Particular { get; set; }
        public Decimal Debit { get; set; }
        public Decimal Credit { get; set; }
        public Decimal Balance { get; set; }
        public String DrCrFlag { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public string ChequeNo { get; set; }
        public string Flag { get; set; }
        public string Common { get; set; }
        public string CommonDt { get; set; }
        public string UnClear { get; set; }
        public string ld_dpid { get; set; }
        public string td_exchange { get; set; }
        public string td_segment { get; set; }
        public string ld_clientcd { get; set; }

        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string Add3 { get; set; }
        public string Add4 { get; set; }
        public string Panno { get; set; }
        public string Pincode { get; set; }
        public string unclear { get; set; }
    }

    public class LedgerReportModel
    {
        public string CompanyLogo { get; set; }
        public string LedgerBalanceDate { get; set; }
        public IEnumerable<LedgerModel> LedgerModels { get; set; }
    }


    public class UserMasterModel
    {
        public string um_user_id { get; set; }
        public string um_passwd { get; set; }
        public string um_user_name { get; set; }
        public string um_loginflag { get; set; }
        public string Um_Locked { get; set; }

        public string um_status { get; set; }
    }

    public class DashBoardClientModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class DashBoardClientCountModel
    {
        public int Count1 { get; set; }
        public int Count2 { get; set; }
        public int Count3 { get; set; }
        public int Count4 { get; set; }
        public int Count5 { get; set; }

        public decimal Sum1 { get; set; }
        public decimal Sum2 { get; set; }
        public decimal Sum3 { get; set; }
        public decimal Sum4 { get; set; }
        public decimal Sum5 { get; set; }
    }

    public class LedgerViewModel : ClientMasterModel
    {

        public string Account { get; set; }
        public string DPID { get; set; }
        public string Exchange { get; set; }
        public decimal OpeningBal { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }

    }

    public class DashBoardClientSummaryModel : ClientMasterModel
    {
        public string FamilyName { get; set; }
        public string GroupName { get; set; }
        public string BranchName { get; set; }
        public decimal LedgerSum { get; set; }
        public decimal BenHoldingSum { get; set; }
        public decimal MarginShortFallSum { get; set; }
        public decimal RetainHoldingSum { get; set; }
        public decimal ShareCollateralSum { get; set; }
        public decimal RiskStatusSum { get; set; }
        public int SettledBefore { get; set; }
        public int DebitSince { get; set; }

    }

    public class OtherProductsModel
    {
        public string server { get; set; }
        public string database { get; set; }
        public string owner { get; set; }
        public string user { get; set; }
        public string password { get; set; }
    }

    public class CombinedMarginModel
    {
        public string ExchSeg { get; set; }
        public decimal IntialMargin { get; set; }
        public decimal MTMLoss { get; set; }
        public decimal TotalMargin { get; set; }
        public decimal IntialCollected { get; set; }
        public decimal MTMCollected { get; set; }
        public decimal IntitalShortFall { get; set; }
        public decimal MTMShortFall { get; set; }
    }
    public class MarginShortFallModel
    {
        public string ExchSeg { get; set; }
        public decimal SPAN { get; set; }
        public decimal Exposure { get; set; }
        public decimal Premium { get; set; }
        public decimal Initial { get; set; }
        public decimal Additional { get; set; }
        public decimal Collected { get; set; }
        public decimal MTMLoss { get; set; }
        public decimal Collected2 { get; set; }
        public decimal Shortfall { get; set; }
    }

    public class ShareCollateralModel
    {
        public string ScripCd { get; set; }
        public string ScripName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        public decimal Haircut { get; set; }
        public decimal NetValue { get; set; }

    }

    public class FOExposureModel
    {
        public string Description { get; set; }
        public string Exchange { get; set; }

        public decimal Buy { get; set; }

        public decimal Sell { get; set; }

        public decimal Net { get; set; }

        public decimal AvgRate { get; set; }

        public decimal ClosingRate { get; set; }

        public decimal Exposure { get; set; }
    }

    public class RiskManagementModel : ClientModel
    {
        public decimal TDay { get; set; }
        public decimal T2Day { get; set; }
        public decimal Uncleared { get; set; }
        public decimal CashDeposit { get; set; }

        public decimal ApprovedShares { get; set; }

        public decimal Margin { get; set; }

        public decimal Pool { get; set; }

        public decimal DpHolding { get; set; }

        public decimal Stock { get; set; }

        public decimal Net { get; set; }

        public decimal AboveDays { get; set; }

        public decimal Collection { get; set; }

        public decimal ActualRisk { get; set; }

        public decimal FundPayout { get; set; }

        public decimal ProjectedRisk { get; set; }

        public decimal SharePayout { get; set; }

        public decimal FOMargin { get; set; }

        public decimal StockBH { get; set; }
        public decimal OptionM2M { get; set; }

        public decimal DebitOlder5Days { get; set; }

        public decimal RMSLimit { get; set; }


        public decimal StockAH { get; set; }

        public decimal FDCollateral { get; set; }
    }

    public class BillsViewDashBoardModel : ClientModel
    {
        public int ClientCount { get; set; }
        public decimal BillAmount { get; set; }

    }

    public class SettlementQtyModel
    {
        public string Stlmnt { get; set; }
        public decimal Qty { get; set; }
    }

    public class BillSettlementModel : ClientModel
    {
        public string settlement { get; set; }
        public decimal amount { get; set; }
        public string billtype { get; set; }
        public string billdate { get; set; }

    }

    public class PLChargesModel
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class QuarterlySquareOffModel
    {
        public string qs_dt { get; set; }
        public string qs_cmcd { get; set; }
        public decimal qs_LedBal { get; set; }
        public decimal qs_LedBalM { get; set; }
        public decimal qs_TotalFunds { get; set; }
        public decimal qs_CollBeforeH { get; set; }
        public decimal qs_CollAfterH { get; set; }
        public decimal qs_BenfBeforeH { get; set; }
        public decimal qs_BenfAfterH { get; set; }
        public decimal qs_TotalShares { get; set; }
        public decimal qs_TotalFundShare { get; set; }
        public decimal qs_DebitLedger { get; set; }
        public decimal qs_DebitLedgerM { get; set; }
        public decimal qs_TotalDebitLedger { get; set; }
        public decimal qs_FundPayInCashT { get; set; }
        public decimal qs_FundPayInFOT { get; set; }
        public decimal qs_FundPayInFXT { get; set; }
        public decimal qs_FundPayInCashT1 { get; set; }
        public decimal qs_SharePayInCashT { get; set; }
        public decimal qs_SharePayInCashT1 { get; set; }
        public decimal qs_MarginFO { get; set; }
        public decimal qs_MarginFX { get; set; }
        public decimal qs_TotalMargin { get; set; }
        public decimal qs_TurnoverCash { get; set; }
        public decimal qs_TotalRetainReq { get; set; }
        public decimal qs_RetainFund { get; set; }
        public decimal qs_RetainShares { get; set; }
        public decimal qs_TotalRetain { get; set; }
        public decimal qs_ReleaseFund { get; set; }
        public decimal qs_ReleaseShares { get; set; }
        public decimal qs_TotalRelease { get; set; }
        public decimal qs_CollFD { get; set; }
        public decimal qs_CollBG { get; set; }
        public decimal qs_Netvalue { get; set; }
        public decimal qs_RetAllowed { get; set; }
    }


    public class BulkPayoutModel : ClientModel
    {
        public decimal Amt1 { get; set; }
        public decimal Amt2 { get; set; }
        public decimal Amt3 { get; set; }
        public decimal Billamt { get; set; }
        public decimal Otheramt { get; set; }
        public decimal Minamt { get; set; }
        public string Dpid { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }

    }

    public class PayoutReleaseModel
    {
        public string rq_dpid { get; set; }
        public string rq_clientcd { get; set; }
        public decimal rq_amt { get; set; }
        public string rq_date { get; set; }
        public decimal rq_relAmt { get; set; }
        public string rq_relflag { get; set; }
        public double rq_RcSrNo { get; set; }
        public string mkrid { get; set; }
        public string mkrdt { get; set; }
        public string mkrtm { get; set; }
        public string machineId { get; set; }
        public string rc_voucherno { get; set; }
    }

    public class ReceiptTableModel
    {
        public string mode { get; set; }
        public double rc_srno { get; set; }
        public string rc_voucherno { get; set; }
        public string rc_clientcd { get; set; }
        public string cm_name { get; set; }
        public string rc_receiptdt { get; set; }
        public decimal rc_amount { get; set; }
        public string rc_debitflag { get; set; }
        public string rc_particular { get; set; }
        public string rc_bankclientcd { get; set; }
        public string rc_cleareddt { get; set; }
        public int rc_entryno { get; set; }
        public string rc_chequeno { get; set; }
        public string rc_micr { get; set; }
        public string mkrid { get; set; }
        public string mkrdt { get; set; }
        public string rc_dpid { get; set; }
        public string rc_accyear { get; set; }
        public string rc_authid1 { get; set; }
        public string rc_authid2 { get; set; }
        public string rc_authdt1 { get; set; }
        public string rc_authdt2 { get; set; }
        public string rc_status { get; set; }
        public string rc_authremarks { get; set; }
        public string rc_commondt { get; set; }
        public string rc_common { get; set; }
        public string rc_BankActNo { get; set; }
        public double rc_batchno { get; set; }
        public string mkrtm { get; set; }
        public string rc_authtm1 { get; set; }
        public string rc_authtm2 { get; set; }
        public string rc_costcenter { get; set; }
        public IEnumerable<JsonComboModel> GetBankName { get; set; }
        public IEnumerable<JsonComboModel> GetAllMicr { get; set; }
        public IEnumerable<JsonComboModel> GetAllAc { get; set; }
        public byte[] ReceiptImage { get; set; }
    }


    public class SharePayoutModel : ClientModel
    {
        public decimal LegderBalance { get; set; }
        public decimal Holding { get; set; }
        public decimal RMSAmount { get; set; }
        public decimal ClientRequest { get; set; }
        public decimal RequestAmt { get; set; }
    }

    public class SharePayoutEditModel
    {
        public string ScripCode { get; set; }
        public string ScripName { get; set; }
        public decimal HoldQty { get; set; }
        public decimal HoldValue { get; set; }
        public decimal RequestQty { get; set; }
        public decimal RequestValue { get; set; }
        public decimal Rate { get; set; }
        public SharePayoutModel GetClientSummary { get; set; }
    }

    public class JsonComboModel
    {
        public string Display { get; set; }
        public string Value { get; set; }
    }

    public class ProfitLossFOModel : ClientModel
    {
        public string Desc { get; set; }
        public decimal? BFQTY { get; set; }
        public decimal? BuyQTY { get; set; }
        public decimal? SellQTY { get; set; }
        public decimal? Exercise { get; set; }
        public decimal? Assignment { get; set; }
        public decimal? OutQty { get; set; }
        public decimal? CloseOut { get; set; }
        public decimal MTMPremium { get; set; }

        public decimal? BuyRate { get; set; }

        public decimal? SellRate { get; set; }

        public decimal? BuyValue { get; set; }

        public decimal? SellValue { get; set; }
        public decimal? outRate { get; set; }
    }
    public class ProfitLossCashModel : ClientModel
    {
        public string Ord { get; set; }
        public string ScripCd { get; set; }
        public string Security { get; set; }
        public string TradeType { get; set; }
        public decimal? BuyQty { get; set; }
        public decimal? BuyRate { get; set; }
        public decimal? BuyValue { get; set; }
        public decimal? SellQty { get; set; }
        public decimal? SellRate { get; set; }
        public decimal? SellValue { get; set; }
        public decimal? NetQty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public decimal RealizedPL { get; set; }
        public decimal UnRealizedPL { get; set; }
        public List<PLChargesModel> ChargesList { get; set; }
    }


    public class DyanmicModel
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class EntityMasterModel
    {
        public string em_cd { get; set; }
        public string em_name { get; set; }
        public string em_add1 { get; set; }
        public string em_add2 { get; set; }
        public string em_add3 { get; set; }
        public string em_add4 { get; set; }

        public string em_panno { get; set; }
    }

    public class ClientMasterBillModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string Add3 { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string BillFrom { get; set; }
        public string BillTo { get; set; }
        public string Settlement { get; set; }
        public string PAN { get; set; }

    }

    public class BillPrintModel : ClientMasterModel
    {
        public EntityMasterModel GetEntityDetails { get; set; }

        public ClientMasterBillModel GetClientDetail { get; set; }
        public int Rectype { get; set; }
        public string Nodel { get; set; }
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        public string ScripCd { get; set; }
        public string ScripName { get; set; }
        public string Date { get; set; }
    }

    public class FOBillprintModel : ClientMasterModel
    {
        public EntityMasterModel GetEntityDetails { get; set; }

        public ClientMasterBillModel GetClientDetail { get; set; }
        public string Date { get; set; }
        public decimal SellQty { get; set; }
        public decimal BuyQty { get; set; }
        public decimal Credit { get; set; }
        public decimal SellRate { get; set; }
        public decimal BuyRate { get; set; }
        public string ContractDescriptor { get; set; }
        public decimal ClosingPr { get; set; }
        public decimal Debit { get; set; }
        public string tax_desc { get; set; }
        public decimal billno { get; set; }

        public string sm_desc { get; set; }

        public decimal tx_controlflag { get; set; }
    }


    public class RMSHawkEYEModel : ClientModel
    {
        public string Branchname { get; set; }
        public string Familyname { get; set; }
        public string FamilyCode { get; set; }
        public string BranchCode { get; set; }
        public string GroupCode { get; set; }
        public string RMCode { get; set; }
        public string Category { get; set; }
        public decimal CashLedBal { get; set; }
        public decimal FNOLedBal { get; set; }
        public decimal TotalLedBal { get; set; }
        public decimal CashHolding { get; set; }
        public decimal Collateral { get; set; }
        public decimal TotalHolding { get; set; }
        public decimal CurrCashExposure { get; set; }
        public decimal FNOM2M { get; set; }
        public decimal TotalCurrExposure { get; set; }
        public decimal CurrSpanMgn { get; set; }
        public decimal CurrExposureMgn { get; set; }
        public decimal TotalMgn { get; set; }
        public decimal PremiumDrCr { get; set; }
        public decimal OptionM2M { get; set; }
        public decimal Expenses { get; set; }
        public decimal FinalLedBal { get; set; }
        public decimal ActualRisk { get; set; }
        public decimal ProjectedRisk { get; set; }
        public decimal CollectionValue { get; set; }
        public decimal UnclearChq { get; set; }
        public decimal BODRMSLmt { get; set; }
        public decimal DPHolding { get; set; }
        public decimal AvailableMgn { get; set; }
        public decimal IncrDecMargin { get; set; }
        public decimal DELIVERYSELLVAL { get; set; }
        public decimal Adjst { get; set; }
        public decimal CashMTM { get; set; }
        public decimal NBFCLedger { get; set; }
        public decimal NBFCCollateral { get; set; }
        public decimal DebitGreater5Days { get; set; }
        public decimal TotalHoldingAHC { get; set; }
        public decimal MTFLedger { get; set; }
        public decimal MTFHolding { get; set; }
        public decimal MTFHoldingAHC { get; set; }
    }

    public class RMSClientPositionModel : ClientModel
    {
        public string ClientCode { get; set; }
        public string Scripcode { get; set; }
        public string SymbolSeries { get; set; }
        public Int64 BOD { get; set; }
        public decimal Rate { get; set; }
        public decimal BuyQty { get; set; }
        public decimal BuyAvgRate { get; set; }
        public decimal BuyValue { get; set; }
        public decimal SellQty { get; set; }
        public decimal SellAvgRate { get; set; }
        public decimal SellValue { get; set; }
        public decimal NetQty { get; set; }
        public decimal NetValue { get; set; }
        public decimal LTP { get; set; }
        public decimal LiqValue { get; set; }
        public decimal TotalMgn { get; set; }
        public decimal M2M { get; set; }
    }

    public class ContinuousDebitModel
    {
        public string Client { get; set; }
        public string CLName { get; set; }
        public decimal? Tday { get; set; }
        public decimal? UpToDate { get; set; }
        public decimal? CDebit { get; set; }
    }

    public class ClientPositionModel
    {
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string Client { get; set; }
        public string CLName { get; set; }
        public string Scrip { get; set; }
        public string ScripNm { get; set; }
        public string Date { get; set; }
        public string StlmntExch { get; set; }
        public decimal BuyQty { get; set; }
        public decimal BuyAmt { get; set; }
        public string BuyAvg { get; set; }
        public decimal SalesQty { get; set; }
        public decimal SalesAmt { get; set; }
        public string SalesAvg { get; set; }
        public decimal NetQty { get; set; }
        public decimal NetAmt { get; set; }
        public string NetAvg { get; set; }
        public string AvgRt { get; set; }

        public string ExchSeg { get; set; }

    }

    public class DPHodlingRMSModel
    {
        public string ScripCd { get; set; }
        public string ScripName { get; set; }
        public string ISIN { get; set; }
        public decimal HOLDING { get; set; }
        public decimal VALUE { get; set; }

    }
    public class ApprovedShareRMSModel
    {
        public string Segment { get; set; }
        public string ScripCd { get; set; }
        public string ScripName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal HairCut { get; set; }
        public decimal AmtAfterHairCut { get; set; }
    }


    public class StockRMSModel
    {
        public string ScripCode { get; set; }
        public string ScripName { get; set; }
        public decimal Holding { get; set; }
        public decimal ActulAmount { get; set; }
        public decimal AmtAfterHairCut { get; set; }
        public string Approved { get; set; }

    }
    public class StockRMSSModel
    {
        public string ScripCode { get; set; }
        public string rd_stlmnt { get; set; }
        public string se_shpayindt { get; set; }
        public string Type { get; set; }
        public decimal Holding { get; set; }
        public decimal ActulAmount { get; set; }
        public decimal HairCut { get; set; }
        public decimal AmtAfterHairCut { get; set; }
    }

    public class BrokerageModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal ABCt_Brokerage { get; set; }
        public decimal ABCt_RemShare { get; set; }
        public decimal ANCt_Brokerage { get; set; }
        public decimal ANCt_RemShare { get; set; }
        public decimal AMCt_Brokerage { get; set; }
        public decimal AMCt_RemShare { get; set; }

        public decimal ABFt_Brokerage { get; set; }
        public decimal ABFt_RemShare { get; set; }

        public decimal ANFt_Brokerage { get; set; }
        public decimal ANFt_RemShare { get; set; }

        public decimal AMFt_Brokerage { get; set; }
        public decimal AMFt_RemShare { get; set; }

        public decimal ANKt_Brokerage { get; set; }
        public decimal ANKt_RemShare { get; set; }

        public decimal AMKt_Brokerage { get; set; }
        public decimal AMKt_RemShare { get; set; }

        public decimal NCDEXFOGross { get; set; }
        public decimal NCDEXFOShare { get; set; }
    }

    public class OutstandingModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string td_seriesid { get; set; }
        public string sm_desc { get; set; }
        public string ExchSeg { get; set; }
        public string longQty { get; set; }
        public string avgrate { get; set; }
        public string shortqty { get; set; }
        public string avgqty { get; set; }
        public string netqty { get; set; }
        public string clrate { get; set; }
        public string value { get; set; }



    }

    public class Funded
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal Value { get; set; }
        public decimal MTM { get; set; }
        public decimal Margin { get; set; }
        public decimal MarginReq { get; set; }

    }
    public class Collateral
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal GrossValue { get; set; }
        public decimal HairCut { get; set; }
        public decimal NetValue { get; set; }
    }
    public class Trades
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Settelement { get; set; }
        public decimal BuyQty { get; set; }
        public decimal SellQty { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
    }

    public class MTPClientData
    {
        public string caption { get; set; }
        public string Tmp_Clientcd { get; set; }
        public string cm_name { get; set; }
        public decimal Tmp_Limit { get; set; }
        public decimal Tmp_FundedAmount { get; set; }
        public decimal Tmp_TplusBal { get; set; }
        public decimal Tmp_LoanBal { get; set; }
        public decimal Tmp_CollateralFund { get; set; }
        public decimal Tmp_CollateralValue { get; set; }
        public decimal Tmp_FundedMrgReq { get; set; }
        public decimal Tmp_M2MLoss { get; set; }
        public decimal Tmp_ShortFallExcess { get; set; }

    }

    public class MasterMTPRMS
    {
        public List<Funded> CustFunded { get; set; }
        public List<Collateral> CustCollateral { get; set; }
        public List<Trades> CustTrades { get; set; }
        public List<MTPClientData> CustMTPClient { get; set; }

    }
    public class ddlcmbproduct
    {
        public string cmbproductvalue { get; set; }
        public string cmbproductText { get; set; }
    }

    public class ClientMasterDetails
    {
        public string ClientCd { get; set; }
        public string ClientName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCd { get; set; }
        public string StdCode { get; set; }
        public string TeleOff { get; set; }
        public string TeleResi { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string PAN { get; set; }
        public string DOB { get; set; }
        public string Occupation { get; set; }
        public string Remarks { get; set; }
        public string UID { get; set; }
        public string DepositAC { get; set; }
        public string KRAAddress { get; set; }
        public string KRAStatus { get; set; }
        public string Remissier { get; set; }
        public string Constitution { get; set; }
        public string Status { get; set; }
        public string Introducer { get; set; }
        public string Group { get; set; }
        public string Family { get; set; }
        public string Type { get; set; }
        public string Branch { get; set; }
        public string Sex { get; set; }
        public string EmailId { get; set; }
        public string BillPrinting { get; set; }
        public string ContractPrinting { get; set; }
        public string POA { get; set; }
        public string AcctOpenDt { get; set; }
        public string LedgerBal { get; set; }
        public decimal OverallLedgerBal { get; set; }
        public string LastBillDt { get; set; }
        public string RM { get; set; }
        public string FormNo { get; set; }
        public string Password { get; set; }
        public string FreezeReason { get; set; }       
        public string grossincomedt { get; set; }
        public string grossincome { get; set; }
        public string networth { get; set; }
        public string networthdt { get; set; }

        public List<CLDPDetails> GetCLDPDetails { get; set; }
        public List<CLBankDetails> GetCLBankDetails { get; set; }
        public List<CDSLDPDetails> GetCDSLDPDetails { get; set; }
        public List<UCCStatus> GetUCCStatus { get; set; }
        public List<FamilyStatus> GetFamily { get; set; }
    }
    public class CLDPDetails
    {
        public string Code { get; set; }
        public string DPID { get; set; }
        public string ClientID { get; set; }
        public string AcctName { get; set; }
        public string Default { get; set; }
        public string DpName { get; set; }
        public string SharePayout { get; set; }


    }

    public class CLBankDetails
    {
        public string MICR { get; set; }
        public string IFSC { get; set; }
        public string ActType { get; set; }
        public string ActNO { get; set; }
        public string AcctName { get; set; }
        public string BankDetail { get; set; }
        public string Default { get; set; }
        public string cm_fundpayout { get; set; }
        public string cm_bankacttype { get; set; }
    }
    public class UCCStatus
    {
        public string BseCashUCC { get; set; }
        public string NseCashUCC { get; set; }
        public string NseFnOUCC { get; set; }
    }
    public class FamilyStatus
    {
        public string cmcd { get; set; }
        public string cmname { get; set; }
        public string Frezereason { get; set; }
        public string cesnm { get; set; }
        public string UCC { get; set; }
        public string Aadhar { get; set; }
        public string CKYC { get; set; }
        public string KRA { get; set; }
    }
    public class CDSLDPDetails
    {
        public string BOID { get; set; }
        public string SecHName { get; set; }
        public string SecHPANNo { get; set; }
        public string ThrdHName { get; set; }
        public string ThrdHPANNo { get; set; }
        public string BankName { get; set; }
        public string AcctNo { get; set; }
        public string Currency { get; set; }
        public string BankCd { get; set; }
        public string IFSC { get; set; }
        public string AcctType { get; set; }
        public string NomineeNm { get; set; }
        public string Header { get; set; }
        public string SecHMobNo { get; set; }
        public string ThrdHMobNo { get; set; }
        public string SecHEmail { get; set; }
        public string ThrdHEmail { get; set; }
        public string MICR { get; set; }
    }

    public class Changepwd
    {

        public string code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string old_pwd { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[RegularExpression("((?=.*[!@#$%]).{6,})", ErrorMessage = "Invalid password format")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string new_pwd { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("new_pwd", ErrorMessage = "The new password and confirmation password do not match.")]
        public string cnfm_pwd { get; set; }

    }

    public class BulkPayoutRequestModel
    {
        public string ExchangeSegment { get; set; }
        public decimal Payable { get; set; }
        public decimal RequestAmount { get; set; }
        public string ClientCode { get; set; }
        public string strDt { get; set; }
        public string dp_id { get; set; }
    }

    public class BulkPledgeRequestModel
    {
        public string ClientCode { get; set; }
        public int RequestQty { get; set; }
    }

    public class CreceiptBankNameModel
    {
        public string BankName { get; set; }
        public string BankCode { get; set; }
    }
    public class modMenuMaster
    {
        public string ModuleCd { get; set; }
        public string isVisible { get; set; }
    }
    public class modDashBoardUser
    {
        public bool blnShowDashBoard { get; set; }
        public bool blnShowSearchbtn { get; set; }
        public bool blnShowClientbtn { get; set; }
        public bool blnShowTradesbtn { get; set; }
        public bool blnShowBillbtn { get; set; }
        public bool blnShowRiskMgbtn { get; set; }
        public bool blnShowCharts { get; set; }
        public bool blnShowBrokerageChart { get; set; }
        public bool blnShowRMSChart { get; set; }
        public bool blnShowDebtorsChart { get; set; }
        public bool blnShowCreditorsChart { get; set; }
    }
    public class CommExchCircular
    {
        public string ec_Details { get; set; }
        public string ec_IssuseDt { get; set; }

    }
    public class CommECircular_attach
    {
        public string ea_filename { get; set; }
        public byte[] ea_document { get; set; }
        public string ea_srno { get; set; }
    }
    public class CommFAQs
    {
        public string FAQ_Question { get; set; }
        public string FAQ_Answer { get; set; }
    }
    public class CommMasterDetails
    {
        public List<CommExchCircular> CommExchCircular { get; set; }
        public List<CommECircular_attach> CommECircular_attach { get; set; }
        public List<CommFAQs> CommFAQs { get; set; }


    }

    public class buybackofferModel
    {

        public string se_stlmnt { get; set; }

        public decimal BB_Rate { get; set; }

        public string lblTDt { get; set; }
        public string BB_DematAct { get; set; }
        public string lblisin { get; set; }
        public string lblOfferDt { get; set; }
        public string se_stdt { get; set; }
        public string se_payoutdt { get; set; }
        public string se_payindt { get; set; }
        public string ss_bsymbol { get; set; }
        public string ss_nsymbol { get; set; }

        public string se_endt { get; set; }
        public string se_exchange { get; set; }

        public string cmbExchStlmnt { get; set; }
        // public string im_isin { get; set; }

    }
    public class BuyBackOfferSaveRequestModel
    {
        public string ClientCode { get; set; }
        public string Name { get; set; }
        public string DematAccount { get; set; }
        public decimal Holding { get; set; }
        public decimal Offered { get; set; }
        public decimal Value { get; set; }
        public string Status { get; set; }
        public string Security { get; set; }
        public string CKStatus { get; set; }
        public string PayInDt { get; set; }
    }
    public class DDLSettlementSearch
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ACHEntryClientdetail
    {
        public string ClName { get; set; }
        public string Email { get; set; }
        public string Mob { get; set; }
        public string Message { get; set; }
        public List<FillActDtls> ActDtls { get; set; }

    }
    public class FillActDtls
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class IPODetails
    {
        public string ClientCode { get; set; }
        public string DematNo { get; set; }
        public string UPIID { get; set; }
    }
    public class IPOBids
    {
        public string Quantity { get; set; }
        public Boolean Cutoff { get; set; }
        public string Price { get; set; }
    }
    public class IPOApplication
    {
        public string scripid { get; set; }
        public string clientname { get; set; }
        public string panno { get; set; }
        public string depository { get; set; }
        public string dpid { get; set; }
        public string clientbenfid { get; set; }
        public string bankname { get; set; }
        public string bankbranch { get; set; }
        public string bankcode { get; set; }
        public string location { get; set; }
        public string ifsccode { get; set; }
        public string accountnumber { get; set; }
        public string filler1 { get; set; }
        public string category { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string contact { get; set; }
        public string bidtype { get; set; }
        public string filler2 { get; set; }
        public string syndicatemembercode { get; set; }
        public string brokercode { get; set; }
        public string subbrokercode { get; set; }
        public string referencefield { get; set; }
        public string repartitiontype { get; set; }
        public string orderno { get; set; }
        public List<BidDetails> bids { get; set; }
    }
    public class BidDetails
    {
        public string quantity { get; set; }
        public string rate { get; set; }
        public string cuttoffflag { get; set; }
    }
    public class IPODetailsDB
    {
        public string category { get; set; }
        public int minOrder { get; set; }
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public decimal discount { get; set; }
        public decimal tickSize { get; set; }
    }
    public class IPOResponse
    {
        public string SrNo { get; set; }
        public string Client { get; set; }
        public string DematNo { get; set; }
        public string Response { get; set; }
    }
    public class OrderBidDetails
    {
        public string bidid { get; set; }
        public string quantity { get; set; }
        public string rate { get; set; }
        public string cuttoffflag { get; set; }
        public string orderno { get; set; }
        public string actioncode { get; set; }
    }
    public class IPOOrder
    {
        public string scripid { get; set; }
        public string applicationno { get; set; }
        public string category { get; set; }
        public string applicantname { get; set; }
        public string depository { get; set; }
        public string dpid { get; set; }
        public string clientbenfid { get; set; }
        public string chequereceivedflag { get; set; }
        public string chequeamount { get; set; }
        public string panno { get; set; }
        public string bankname { get; set; }
        public string location { get; set; }
        public string accountnumber_upiid { get; set; }
        public string ifsccode { get; set; }
        public string referenceno { get; set; }
        public string asba_upiid { get; set; }
        public List<OrderBidDetails> bids { get; set; }
    }
    public class LoginRequest
    {
        public string membercode { get; set; }
        public string loginid { get; set; }
        public string password { get; set; }
        public string ibbsid { get; set; }
    }
    public class BidResponse
    {
        public string bidid { get; set; }
        public string quantity { get; set; }
        public string rate { get; set; }
        public string cuttoffflag { get; set; }
        public string orderno { get; set; }
        public string actioncode { get; set; }
        public string errorcode { get; set; }
        public string message { get; set; }
    }
    public class IPOOrderResponse
    {
        public string scripid { get; set; }
        public string applicationno { get; set; }
        public string category { get; set; }
        public string applicantname { get; set; }
        public string depository { get; set; }
        public string dpid { get; set; }
        public string clientbenfid { get; set; }
        public string chequereceivedflag { get; set; }
        public string chequeamount { get; set; }
        public string panno { get; set; }
        public string bankname { get; set; }
        public string location { get; set; }
        public string accountnumber_upiid { get; set; }
        public string ifsccode { get; set; }
        public string referenceno { get; set; }
        public string asba_upiid { get; set; }
        public string statuscode { get; set; }
        public string statusmessage { get; set; }
        public List<BidResponse> bids { get; set; }
    }

    public class RiskManagmentResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal TDay { get; set; }
        public decimal T2Day { get; set; }
        //public decimal UnCleared { get; set; }
        //public decimal CashDeposit { get; set; }
        //public decimal ShareCollateral { get; set; }
        //public decimal Margin { get; set; }
        //public decimal Pool { get; set; }
        //public decimal DPHolding { get; set; }
        //public decimal Stoke { get; set; }
        //public decimal Net { get; set; }
        //public decimal AboveDays { get; set; }
        //public decimal Collection { get; set; }
        //public decimal ActualRisk { get; set; }
        //public decimal FundPayout { get; set; }
        //public decimal ProjectedRisk { get; set; }
        //public decimal SharePayout { get; set; }
        //public decimal FoMargin { get; set; }
        //public decimal StokeBH { get; set; }
        //public decimal ProvInt { get; set; }
        //public decimal AvailMargin { get; set; }
        //public decimal OptionM2M { get; set; }
        //public decimal DebitOlder5Days { get; set; }
        //public decimal RmsLimit { get; set; }
        //public decimal StokeAH { get; set; }
        //public decimal ExchApprStk { get; set; }
    }
}