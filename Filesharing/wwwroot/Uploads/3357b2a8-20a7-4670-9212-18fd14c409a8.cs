using Dapper;
using OasisIBS.Domain.Entites.Production;
using OasisIBS.Infrastructure.Persistence;

namespace OasisIBS.Web.Areas.Production.Controllers;
[Area("Production")]
[Route("Production/Policy/[action]")]
public class PoliciesEndorsementManagmentController : Controller
{
    private readonly AppDbContext _context;

    /* -------------------------- Get Policies/Endorsement Managment List -------------------------- */



    public PoliciesEndorsementManagmentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    //[ClaimRequirement("IsAdminMobile")]
    //public async Task<IActionResult> Index()
    //{
    //    //var model = await _clientService.InitalizePolicyEndorsFilter();
    //    //return View(model);
    //}

    [HttpPost]
    [ActionName("Search")]
    public async Task<IActionResult> GetPoliciesEndorsementManagment([FromBody] SearchPolicyParams Policy)
    {
        var strCond = "";


        if (!string.IsNullOrEmpty(Policy.FilterBranch))
            strCond += " And Branch=N'" + Policy.FilterBranch + "' ";

        if (!string.IsNullOrEmpty(Policy.filterInsurCompany))
            strCond += " And InsurComp=N'" + Policy.filterInsurCompany + "' ";


        if (!string.IsNullOrEmpty(Policy.FilterProducer))
            strCond += " And Producer=N'" + Policy.FilterProducer + "' ";

        if (!string.IsNullOrEmpty(Policy.filterCreatedBy) && Policy.FilterUser != "--All--")
            strCond += " And SavedUser Like N'%" + Policy.filterCreatedBy + "%' ";


        if (!string.IsNullOrEmpty(Policy.filterClientName))
            strCond += " And ClientName Like N'%" + Policy.filterClientName + "%' ";


        if (!string.IsNullOrEmpty(Policy.FilterClassOfBusniess))
            strCond += " And ClassName = N'" + Policy.FilterClassOfBusniess + "' ";


        if (!string.IsNullOrEmpty(Policy.filterPolicyNo))
            strCond += " And PolicyNo Like N'%" + Policy.filterPolicyNo + "%' ";


        if (!string.IsNullOrEmpty(Policy.filterClientDNCNNo))
            strCond += " And ClientDNCNNo Like N'%" + Policy.filterClientDNCNNo + "%' ";

        if (!string.IsNullOrEmpty(Policy.filterCompanyCommisionDNCNNo))
            strCond += " And CompCommDNCNNo Like N'%" + Policy.filterCompanyCommisionDNCNNo + "%' ";

        if (!string.IsNullOrEmpty(Policy.filterRequestNo))
            strCond += " And RequestNo Like N'%" + Policy.filterRequestNo + "%' ";

        if (!string.IsNullOrEmpty(Policy.filterEndorsNo))
            strCond += " And EndorsNo Like N'%" + Policy.filterEndorsNo + "%' ";

        if (!string.IsNullOrEmpty(Policy.filterPolicyEndorsType) && Policy.filterPolicyEndorsType != "--All--")
            strCond += " And EndorsType Like N'%" + Policy.filterPolicyEndorsType + "%' ";

        if (!string.IsNullOrEmpty(Policy.FilterUnderWriter))
            strCond += " And LeadNo in (Select LeadNo from SalesLeadNotes where SalesLeadNotes.SavedUser=N'" + Policy.FilterUnderWriter + "')";


// -----------------------------------------------------------------------------------------------------------
        if (!string.IsNullOrEmpty(Policy.FilterType))
        {
            strCond += " And Type In (";

            foreach (var type in Policy.FilterType.Split(","))
                strCond += "'" + type + "',";

            //remove the extra comma
            strCond = strCond.Remove(strCond.Length - 1, 1);
            strCond += ") ";
        }

        if (!string.IsNullOrEmpty(Policy.FilterLeadType))
        {
            strCond += " And LeadType In (";

            foreach (var leadType in Policy.FilterLeadType.Split(","))
                strCond += "'" + leadType + "',";

            //remove the extra comma
            strCond = strCond.Remove(strCond.Length - 1, 1);
            strCond += ") ";
        }


// -----------------------------------------------------------------------------------------------------------
        if (bool.Parse(Policy.filterIssue) == true)
        {
            strCond += "And IssueDate>=N'" + DateTime.Parse(Policy.filterIssueFrom).ToShortDateString().ToString() + " 00:00:01' " +
                       " And IssueDate<=N'" + DateTime.Parse(Policy.filterIssueTo).ToShortDateString() + " 23:23:59' ";
        }


        if (bool.Parse(Policy.filterInception) == true)
        {
            strCond += " And PeriodFrom>=N'" + DateTime.Parse(Policy.filterInceptionFrom).ToShortDateString().ToString() + " 00:00:01' " +
                       " And PeriodFrom<=N'" + DateTime.Parse(Policy.filterInceptionTo).ToShortDateString() + " 23:23:59' ";
        }


        if (bool.Parse(Policy.filterFinanceApprove) == true)
        {
            strCond += " And FinApprovedDate>=N'" + Policy.filterFinanceApproveFrom + " 00:00:01' " +
                       " And FinApprovedDate<=N'" + Policy.filterFinanceApproveTo + " 23:23:59' ";
        }

        if (bool.Parse(Policy.filterFinanceEntry) == true)
        {
            strCond += " And FinEntryDate>=N'" + DateTime.Parse(Policy.filterFinanceEntryFrom).ToShortDateString() + " 00:00:01' " +
                       " And FinEntryDate<=N'" + DateTime.Parse(Policy.filterFinanceEntryTo).ToShortDateString() + " 23:23:59' ";
        }

        if (bool.Parse(Policy.filterAmount) == true)
        {

            if (string.IsNullOrEmpty(Policy.filterAmountNo2))
            {
                strCond += "And" + " " + GetColumnName(Policy.filterField) + " " + Policy.filterOperatordList +
                    " " + Policy.filterAmountNo;
            }
            else
            {
                strCond += "And" + " " + GetColumnName(Policy.filterField) + " " + Policy.filterOperatordList +
                " " + Policy.filterAmountNo + " and " + Policy.filterAmountNo2;
            }
        }


        //Order By
        var strOrder = " Order By " + Policy.orderBy + " " + Policy.orderDir + " ";
        var QueryString = "";
        var strCondForCount = "";
        if (!string.IsNullOrEmpty(Policy.FilterStatus))
        {


            foreach (var status in Policy.FilterStatus.Split(","))
            {

                if (status == "Active")
                {
                    strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And Done=1 and DoneFin=1 and PeriodTo>=GetDate() " + strCond + "  UNION ";
                    QueryString += "Select  SNo,'Active' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
                               "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
                               "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
                               "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
                               "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
                               "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
                               "SavedUser,SavedDate," +
                               "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
                               "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
                               "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
                               "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
                               "From Policies " +
                               "Where  SNo<>0 And Done=1 and DoneFin=1 and PeriodTo>=GetDate() " + strCond + " UNION ";



                }
                else if (status == "Expired")
                {
                    strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And Done=1 and DoneFin=1 and PeriodTo<GetDate() " + strCond + "  UNION ";
                    QueryString += "Select  SNo,'Expired' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
                              "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
                              "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
                              "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
                              "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
                              "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
                              "SavedUser,SavedDate," +
                              "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
                              "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
                              "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
                              "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
                              "From Policies " +
                              "Where  SNo<>0  And Done=1 and DoneFin=1 and PeriodTo<GetDate() " + strCond + " UNION ";
                }
                else if (status == "Pending")
                {
                    strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null  And DoneFin=0 And Reject=0 And Done =0 " + strCond + "  UNION ";
                    QueryString += "Select  SNo,'Pending' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
                             "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
                             "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
                             "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
                             "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
                             "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
                             "SavedUser,SavedDate," +
                             "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
                             "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
                             "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
                             "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
                             "From Policies " +
                             "Where  SNo<>0   And DoneFin=0 And Reject=0 And Done =0  " + strCond + "UNION ";
                }
                else if (status == "Rejected by Finance")
                {
                    strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And DoneFin=0 And Reject=1 And FinRejectBy is not Null " + strCond + "  UNION ";
                    QueryString += "Select  SNo,'Rejected by Finance' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
                            "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
                            "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
                            "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
                            "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
                            "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
                            "SavedUser,SavedDate," +
                            "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
                            "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
                            "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
                            "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
                            "From Policies " +
                            "Where  SNo<>0  And DoneFin=0 And Reject=1 And FinRejectBy is not Null " + strCond + " UNION ";
                }
                else if (status == "Approved by Production")
                {
                    strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And Reject=0 And Done =1 and DoneFin=0  " + strCond + "  UNION ";
                    QueryString += "Select  SNo,'Approved by Production' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
                           "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
                           "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
                           "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
                           "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
                           "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
                           "SavedUser,SavedDate," +
                           "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
                           "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
                           "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
                           "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
                           "From Policies " +
                           "Where  SNo<>0  And Reject=0 And Done =1 and DoneFin=0 " + strCond + " UNION ";
                }
                else if (status == "Rejected by Production")
                {
                    strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And DoneFin=0 And Reject=1 And ProdRejectBy is not Null  " + strCond + "  UNION ";
                    QueryString += "Select  SNo,'Rejected by Production' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
                          "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
                          "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
                          "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
                          "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
                          "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
                          "SavedUser,SavedDate," +
                          "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
                          "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
                          "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
                          "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
                          "From Policies " +
                          "Where  SNo<>0  And DoneFin=0 And Reject=1 And ProdRejectBy is not Null " + strCond + " UNION ";
                }
                else if (status == "Reviewed by Finanace")
                {
                    strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And Reject=0 And FinReview =1  " + strCond + "  UNION  ";
                    QueryString += "Select  SNo,'Reviewed by Finanace' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
                         "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
                         "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
                         "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
                         "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
                         "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
                         "SavedUser,SavedDate," +
                         "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
                         "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
                         "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
                         "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
                         "From Policies " +
                         "Where  SNo<>0  And Reject=0 And FinReview = 1" + strCond + " UNION ";
                }


            }
            QueryString = QueryString.Remove(QueryString.Length - 6, 5);
            strCondForCount = strCondForCount.Remove(strCondForCount.Length - 6, 5);
        }
        if (string.IsNullOrEmpty(Policy.FilterStatus))
        {
            strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null  " + strCond;
            QueryString += "Select  SNo,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
                 "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
                 "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
                 "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
                 "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
                 "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
                 "SavedUser,SavedDate," +
                 "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
                 "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
                 "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
                 "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
                 "From Policies " +
                 "Where  SNo<0" + strCond;
            strOrder = " Order By SNo asc ";

                
            
        }

        Policy.Data = await _context.Connection.QueryAsync<PoliciesEndorsementsManagmentDto>(QueryString + strOrder +
                                                                     "OFFSET @Offset ROWS " +
                                                                     "FETCH NEXT @PageSize ROWS ONLY",
                                                                       new
                                                                       {
                                                                           Offset = 0,
                                                                           PageSize = 50
                                                                       }, commandTimeout: 300);


        return Json(new
        {
            data = Policy.Data
        });

    }

    public string GetColumnName(string col)
    {

        if (col == "Sum Insured")
            return "SumInsur";

        else if (col == "Total Premium")
            return "TotalPremium";

        else if (col == "Net Premium")
            return "NetPremium";

        else if (col == "Company Comm.")
            return "CompComm";

        else if (col == "Producer Comm.")
            return "ProducerComm";
        return null;
    }


    //[HttpPost]
    //public JsonResult GetPoliciesEndorsementManagment(PoliciesEndorsementsManagmentFilters Policy)
    //{
    //    //var  = new ();

    //    Policy = Helper.SetPaginationAndFiltersParameters<PoliciesEndorsementsManagmentFilters>(Request, Policy);

    //    var strCond = "";


    //    if (!string.IsNullOrEmpty(Policy.FilterBranch))
    //        strCond += " And Branch=N'" + Policy.FilterBranch + "' ";

    //    if (!string.IsNullOrEmpty(Policy.filterInsurCompany))
    //        strCond += " And InsurComp=N'" + Policy.filterInsurCompany + "' ";


    //    if (!string.IsNullOrEmpty(Policy.FilterProducer))
    //        strCond += " And Producer=N'" + Policy.FilterProducer + "' ";

    //    if (!string.IsNullOrEmpty(Policy.filterCreatedBy) && Policy.FilterUser != "--All--")
    //        strCond += " And SavedUser Like N'%" + Policy.filterCreatedBy + "%' ";


    //    if (!string.IsNullOrEmpty(Policy.filterClientName))
    //        strCond += " And ClientName Like N'%" + Policy.filterClientName + "%' ";


    //    if (!string.IsNullOrEmpty(Policy.FilterClassOfBusniess))
    //        strCond += " And ClassName = N'" + Policy.FilterClassOfBusniess + "' ";


    //    if (!string.IsNullOrEmpty(Policy.filterPolicyNo))
    //        strCond += " And PolicyNo Like N'%" + Policy.filterPolicyNo + "%' ";


    //    if (!string.IsNullOrEmpty(Policy.filterClientDNCNNo))
    //        strCond += " And ClientDNCNNo Like N'%" + Policy.filterClientDNCNNo + "%' ";

    //    if (!string.IsNullOrEmpty(Policy.filterCompanyCommisionDNCNNo))
    //        strCond += " And CompCommDNCNNo Like N'%" + Policy.filterCompanyCommisionDNCNNo + "%' ";

    //    if (!string.IsNullOrEmpty(Policy.filterRequestNo))
    //        strCond += " And RequestNo Like N'%" + Policy.filterRequestNo + "%' ";

    //    if (!string.IsNullOrEmpty(Policy.filterEndorsNo))
    //        strCond += " And EndorsNo Like N'%" + Policy.filterEndorsNo + "%' ";

    //    if (!string.IsNullOrEmpty(Policy.filterPolicyEndorsType) && Policy.filterPolicyEndorsType != "--All--")
    //        strCond += " And EndorsType Like N'%" + Policy.filterPolicyEndorsType + "%' ";

    //    if (!string.IsNullOrEmpty(Policy.FilterUnderWriter))
    //        strCond += " And LeadNo in (Select LeadNo from SalesLeadNotes where SalesLeadNotes.SavedUser=N'" + Policy.FilterUnderWriter + "')";


    //    if (!string.IsNullOrEmpty(Policy.FilterType))
    //    {
    //        strCond += " And Type In (";

    //        foreach (var type in Policy.FilterType.Split(","))
    //            strCond += "'" + type + "',";

    //        //remove the extra comma
    //        strCond = strCond.Remove(strCond.Length - 1, 1);
    //        strCond += ") ";
    //    }

    //    if (!string.IsNullOrEmpty(Policy.FilterLeadType))
    //    {
    //        strCond += " And LeadType In (";

    //        foreach (var leadType in Policy.FilterLeadType.Split(","))
    //            strCond += "'" + leadType + "',";

    //        //remove the extra comma
    //        strCond = strCond.Remove(strCond.Length - 1, 1);
    //        strCond += ") ";
    //    }



    //    if (bool.Parse(Policy.filterIssue) == true)
    //    {
    //        strCond += "And IssueDate>=N'" + DateTime.Parse(Policy.filterIssueFrom).ToShortDateString().ToString() + " 00:00:01' " +
    //                   " And IssueDate<=N'" + DateTime.Parse(Policy.filterIssueTo).ToShortDateString() + " 23:23:59' ";
    //    }


    //    if (bool.Parse(Policy.filterInception) == true)
    //    {
    //        strCond += " And PeriodFrom>=N'" + DateTime.Parse(Policy.filterInceptionFrom).ToShortDateString().ToString() + " 00:00:01' " +
    //                   " And PeriodFrom<=N'" + DateTime.Parse(Policy.filterInceptionTo).ToShortDateString() + " 23:23:59' ";
    //    }


    //    if (bool.Parse(Policy.filterFinanceApprove) == true)
    //    {
    //        strCond += " And FinApprovedDate>=N'" + Policy.filterFinanceApproveFrom + " 00:00:01' " +
    //                   " And FinApprovedDate<=N'" + Policy.filterFinanceApproveTo + " 23:23:59' ";
    //    }

    //    if (bool.Parse(Policy.filterFinanceEntry) == true)
    //    {
    //        strCond += " And FinEntryDate>=N'" + DateTime.Parse(Policy.filterFinanceEntryFrom).ToShortDateString() + " 00:00:01' " +
    //                   " And FinEntryDate<=N'" + DateTime.Parse(Policy.filterFinanceEntryTo).ToShortDateString() + " 23:23:59' ";
    //    }

    //    if (bool.Parse(Policy.filterAmount) == true)
    //    {

    //        if (string.IsNullOrEmpty(Policy.filterAmountNo2))
    //        {
    //            strCond += "And" + " " + GetColumnName(Policy.filterField) + " " + Policy.filterOperatordList +
    //                " " + Policy.filterAmountNo;
    //        }
    //        else
    //        {
    //            strCond += "And" + " " + GetColumnName(Policy.filterField) + " " + Policy.filterOperatordList +
    //            " " + Policy.filterAmountNo + " and " + Policy.filterAmountNo2;
    //        }
    //    }


    //    //Order By
    //    var strOrder = " Order By " + Policy.Pagination.SortColumn + " " + Policy.Pagination.SortColumnDirection + " ";
    //    var QueryString = "";
    //    var strCondForCount = "";
    //    if (!string.IsNullOrEmpty(Policy.FilterStatus))
    //    {


    //        foreach (var status in Policy.FilterStatus.Split(","))
    //        {

    //            if (status == "Active")
    //            {
    //                strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And Done=1 and DoneFin=1 and PeriodTo>=GetDate() " + strCond + "  UNION ";
    //                QueryString += "Select  SNo,'Active' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
    //                           "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
    //                           "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
    //                           "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
    //                           "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
    //                           "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
    //                           "SavedUser,SavedDate," +
    //                           "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
    //                           "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
    //                           "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
    //                           "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
    //                           "From Policies " +
    //                           "Where  SNo<>0 And Done=1 and DoneFin=1 and PeriodTo>=GetDate() " + strCond + " UNION ";



    //            }
    //            else if (status == "Expired")
    //            {
    //                strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And Done=1 and DoneFin=1 and PeriodTo<GetDate() " + strCond + "  UNION ";
    //                QueryString += "Select  SNo,'Expired' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
    //                          "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
    //                          "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
    //                          "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
    //                          "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
    //                          "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
    //                          "SavedUser,SavedDate," +
    //                          "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
    //                          "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
    //                          "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
    //                          "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
    //                          "From Policies " +
    //                          "Where  SNo<>0  And Done=1 and DoneFin=1 and PeriodTo<GetDate() " + strCond + " UNION ";
    //            }
    //            else if (status == "Pending")
    //            {
    //                strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null  And DoneFin=0 And Reject=0 And Done =0 " + strCond + "  UNION ";
    //                QueryString += "Select  SNo,'Pending' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
    //                         "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
    //                         "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
    //                         "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
    //                         "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
    //                         "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
    //                         "SavedUser,SavedDate," +
    //                         "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
    //                         "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
    //                         "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
    //                         "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
    //                         "From Policies " +
    //                         "Where  SNo<>0   And DoneFin=0 And Reject=0 And Done =0  " + strCond + "UNION ";
    //            }
    //            else if (status == "Rejected by Finance")
    //            {
    //                strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And DoneFin=0 And Reject=1 And FinRejectBy is not Null " + strCond + "  UNION ";
    //                QueryString += "Select  SNo,'Rejected by Finance' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
    //                        "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
    //                        "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
    //                        "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
    //                        "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
    //                        "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
    //                        "SavedUser,SavedDate," +
    //                        "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
    //                        "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
    //                        "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
    //                        "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
    //                        "From Policies " +
    //                        "Where  SNo<>0  And DoneFin=0 And Reject=1 And FinRejectBy is not Null " + strCond + " UNION ";
    //            }
    //            else if (status == "Approved by Production")
    //            {
    //                strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And Reject=0 And Done =1 and DoneFin=0  " + strCond + "  UNION ";
    //                QueryString += "Select  SNo,'Approved by Production' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
    //                       "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
    //                       "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
    //                       "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
    //                       "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
    //                       "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
    //                       "SavedUser,SavedDate," +
    //                       "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
    //                       "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
    //                       "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
    //                       "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
    //                       "From Policies " +
    //                       "Where  SNo<>0  And Reject=0 And Done =1 and DoneFin=0 " + strCond + " UNION ";
    //            }
    //            else if (status == "Rejected by Production")
    //            {
    //                strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And DoneFin=0 And Reject=1 And ProdRejectBy is not Null  " + strCond + "  UNION ";
    //                QueryString += "Select  SNo,'Rejected by Production' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
    //                      "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
    //                      "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
    //                      "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
    //                      "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
    //                      "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
    //                      "SavedUser,SavedDate," +
    //                      "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
    //                      "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
    //                      "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
    //                      "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
    //                      "From Policies " +
    //                      "Where  SNo<>0  And DoneFin=0 And Reject=1 And ProdRejectBy is not Null " + strCond + " UNION ";
    //            }
    //            else if (status == "Reviewed by Finanace")
    //            {
    //                strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null And Reject=0 And FinReview =1  " + strCond + "  UNION  ";
    //                QueryString += "Select  SNo,'Reviewed by Finanace' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
    //                     "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
    //                     "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
    //                     "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
    //                     "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
    //                     "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
    //                     "SavedUser,SavedDate," +
    //                     "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
    //                     "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
    //                     "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
    //                     "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
    //                     "From Policies " +
    //                     "Where  SNo<>0  And Reject=0 And FinReview = 1" + strCond + " UNION ";
    //            }


    //        }
    //        QueryString = QueryString.Remove(QueryString.Length - 6, 5);
    //        strCondForCount = strCondForCount.Remove(strCondForCount.Length - 6, 5);
    //    }
    //    if (string.IsNullOrEmpty(Policy.FilterStatus))
    //    {
    //        strCondForCount += "Select COUNT(*) From Policies  Where " + " SNo Is Not Null  " + strCond;
    //        QueryString += "Select  SNo,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp," +
    //             "ClassName,IsNull(AccNo,N'') AccNo,PolicyNo," +
    //             "IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo," +
    //             "SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, " +
    //             "CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc," +
    //             "IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo," +
    //             "SavedUser,SavedDate," +
    //             "Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, " +
    //             "DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, " +
    //             "IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions " +
    //             "        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium " +
    //             "From Policies " +
    //             "Where  SNo<0" + strCond;
    //        strOrder = " Order By SNo asc ";
    //        //QueryString = QueryString.Remove(QueryString.Length - 6, 5);
    //        //strCondForCount = strCondForCount.Remove(strCondForCount.Length - 6, 5);
    //    }


    //    using (var con = new SqlConnection(ChedidModule.strconn))
    //    {
    //        var total = con.Query<int>(strCondForCount + strCond, commandTimeout: 300);
    //        int tot = 0;
    //        foreach (var item in total)
    //        {
    //            tot += item;
    //        }
    //        Policy.Pagination.RecordsTotal = tot;
    //        Policy.Data = con.Query<PoliciesEndorsementsManagmentDto>(QueryString + strOrder +
    //                                                             "OFFSET @Offset ROWS " +
    //                                                             "FETCH NEXT @PageSize ROWS ONLY",
    //                                                               new
    //                                                               {
    //                                                                   Offset = Policy.Pagination.Skip,
    //                                                                   PageSize = Policy.Pagination.PageSize
    //                                                               }, commandTimeout: 300);
    //    }

    //    return Json(new
    //    {
    //        draw = Policy.Pagination.Draw,
    //        recordsFiltered = Policy.Pagination.RecordsTotal,
    //        recordsTotal = Policy.Pagination.RecordsTotal,
    //        data = Policy.Data
    //    });

    //}


    //public string GetColumnName(string col)
    //{

    //    if (col == "Sum Insured")
    //        return "SumInsur";

    //    else if (col == "Total Premium")
    //        return "TotalPremium";

    //    else if (col == "Net Premium")
    //        return "NetPremium";

    //    else if (col == "Company Comm.")
    //        return "CompComm";

    //    else if (col == "Producer Comm.")
    //        return "ProducerComm";
    //    return null;
    //}

    //[HttpGet]
    //public ActionResult Index()
    //{
    //    var Policy = new PoliciesEndorsementsManagmentDto();
    //    Policy.BranchesList = MasterTablesHelper.GetBranchesList();
    //    Policy.ProducersList = MasterTablesHelper.GetProducersList();
    //    Policy.InsurCompany = MasterTablesHelper.GetInsuranceCompaniesList();
    //    Policy.ClassOfInsurance = MasterTablesHelper.GetInsuranceClassesList();
    //    Policy.PolicyEndorsType = MasterTablesHelper.GetPolicyEndorsTypeList();
    //    Policy.UserList = MasterTablesHelper.GetUsersList();
    //    Policy.ClientsList = MasterTablesHelper.GetClientList();
    //    Policy.FieldList = PoliciesEndorsementsManagmentHelper.GetFieldList();
    //    Policy.OperatordList = PoliciesEndorsementsManagmentHelper.GetOperatordList();
    //    Policy.Status = PoliciesEndorsementsManagmentHelper.GetProdcutionStatusList();



    //    return View("../PoliciesEndorsementManagment/PoliciesEndorsementManagment", Policy);

    //}


    //[HttpPost]
    //public JsonResult GetLineOfBusinessList(string className)
    //{
    //    var PoliciesEndorsementsManagmentDto = new InsertPoliciesEndorsementsManagment();
    //    PoliciesEndorsementsManagmentDto.LineOfBusiness = MasterTablesHelper.GetLineOfBusinessList(className);
    //    return Json(PoliciesEndorsementsManagmentDto.LineOfBusiness);
    //}

    //[HttpPost]
    ////client search
    //public JsonResult GetPolicesByClientIdList(string ClientNo, string status, string ClientName)
    //{

    //    var PolicesList = MasterTablesHelper.GetPolicesByClientIdList(ClientNo, status, ClientName);


    //    return Json(new { data = PolicesList });
    //}
    ////
    //public JsonResult LoadPoliciyData(string PolicyNo /*= "GRH/12535100 END:702090"*/)
    //{
    //    var Policy = new InsertPoliciesEndorsementsManagment();
    //    Policy.ProducersList = MasterTablesHelper.GetProducersList();
    //    Policy.BranchList = MasterTablesHelper.GetBranchesList();
    //    Policy.ClassOfInsurance = MasterTablesHelper.GetInsuranceClassesList();
    //    Policy.LineOfBusiness = MasterTablesHelper.GetAllLineOfBusinessList();
    //    Policy.PolicyEndorsType = MasterTablesHelper.GetPolicyEndorsTypeList();
    //    Policy.ClientsList = MasterTablesHelper.GetClientList();
    //    Policy.UsersList = MasterTablesHelper.GetUsersList();

    //    var QueryString = " Select * From Policies Where PolicyNo=@PolicyNo";
    //    using (var con = new SqlConnection(ChedidModule.strconn))
    //    {

    //        Policy.InsertPoliciesEndorsementsManagmentDto = con.QueryFirstOrDefault<InsertPoliciesEndorsementsManagmentDto>(QueryString, new { PolicyNo = PolicyNo });

    //    }



    //    return Json(Policy);
    //    // return View("../PoliciesEndorsementManagment/PoliciesEndorsementNew", PoliciesEndorsementsManagment);
    //}
    //[HttpGet]
    //public ActionResult NewPoliciesEndorsements(long SNo)
    //{

    //    var Policy = new InsertPoliciesEndorsementsManagment();

    //    Policy.ProducersList = MasterTablesHelper.GetProducersList();
    //    Policy.BranchList = MasterTablesHelper.GetBranchesList();
    //    Policy.ClassOfInsurance = MasterTablesHelper.GetInsuranceClassesList();
    //    Policy.PolicyEndorsType = MasterTablesHelper.GetPolicyEndorsTypeList();
    //    Policy.ClientsList = MasterTablesHelper.GetClientList();
    //    Policy.InsuranceCompaniesList = MasterTablesHelper.GetInsuranceCompaniesList();
    //    Policy.LineOfBusiness = MasterTablesHelper.GetLineOfBusinessList("");
    //    //string queryperiodDate = "Select LockDate From LockDateProduction";

    //    //using (var con = new SqlConnection(ChedidModule.strconn))
    //    //{
    //    //    var periodDate = con.ExecuteScalar(queryperiodDate);
    //    //    ViewBag.period = periodDate;
    //    //}

    //    if (SNo != 0)
    //    {
    //        Policy.DocumentsModel.Documents = Helpers.Document.DownloadFiles(Helpers.Document.Folders.Production, SNo.ToString());
    //        Policy.DocumentsModel.SNo = SNo;
    //        Policy.DocumentsModel.FolderName = "Production";
    //        //var Policy = new InsertPoliciesEndorsementsManagment();
    //        Policy.Tenant = "Correction";
    //        var QueryString = " Select * From Policies Where SNo=@SNo";

    //        var QueryStringPoliciesPaymentsSchedule = "Select PoliciesSNo,PayDate,Percentage,Amount,SavedUser,Department from PoliciesPaymentsSchedule where PoliciesSNo=@PoliciesSNo";
    //        var QueryStringProducer = "Select PoliciesSNo,Producer,Percentage,Amount,SavedBy from PoliciesProducersCommissions where PoliciesSNo=@PoliciesSNo ";
    //        using (var con = new SqlConnection(ChedidModule.strconn))
    //        {
    //            Policy.InsertPoliciesEndorsementsManagmentDto = con.QueryFirstOrDefault<InsertPoliciesEndorsementsManagmentDto>(QueryString, new { SNo = SNo });
    //            Policy.InsertPoliciesPaymentsScheduleDto = con.Query<InsertPoliciesPaymentsScheduleDto>(QueryStringPoliciesPaymentsSchedule, new { PoliciesSNo = SNo });
    //            Policy.ProducersCorrection = con.Query<ProducerViewModel>(QueryStringProducer, new { PoliciesSNo = SNo });


    //            Policy.ProducersList = MasterTablesHelper.GetProducersList();
    //            Policy.BranchList = MasterTablesHelper.GetBranchesList();
    //            Policy.ClassOfInsurance = MasterTablesHelper.GetInsuranceClassesList();
    //            Policy.PolicyEndorsType = MasterTablesHelper.GetPolicyEndorsTypeList();
    //            Policy.ClientsList = MasterTablesHelper.GetClientList();
    //            Policy.UsersList = MasterTablesHelper.GetUsersList();
    //            Policy.LineOfBusiness = MasterTablesHelper.GetAllLineOfBusinessList();

    //        };
    //        Policy.InsertPoliciesEndorsementsManagmentDto.SNo = SNo;
    //        if (Policy.InsertPoliciesEndorsementsManagmentDto.EndorsType != "Policy")
    //        {

    //            Policy.InsertPoliciesEndorsementsManagmentDto.EndorsRequestType = "Endorsement";
    //        }
    //        //else
    //        //{
    //        //    PoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.EndorsRequestType = "Renewal of";
    //        //}
    //        Policy.InsertPoliciesEndorsementsManagmentDto.SumInsur = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.SumInsur);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.NetPremium = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.NetPremium);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.DeductFees = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.DeductFees);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.Fees = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.Fees);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.CompCommPerc = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.CompCommPerc);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.VatValue = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.VatValue);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.CompCommVAT = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.CompCommVAT);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.FullPremium = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.FullPremium);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.TotalPremium = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.TotalPremium);
    //        Policy.InsertPoliciesEndorsementsManagmentDto.CompComm = Math.Abs(Policy.InsertPoliciesEndorsementsManagmentDto.CompComm);

    //    }

    //    return View("../PoliciesEndorsementManagment/PoliciesEndorsementNew", Policy);

    //}



    ////save new policy/endors
    //[HttpPost]
    //public ActionResult NewPoliciesEndorsements(InsertPoliciesEndorsementsManagment insertPoliciesEndorsementsManagment)
    //{


    //    var producerLists = JsonConvert.DeserializeObject<ProducerList[]>(insertPoliciesEndorsementsManagment.ProducersListPaased);
    //    var PaymentTerms = JsonConvert.DeserializeObject<PaymentTermsView[]>(insertPoliciesEndorsementsManagment.PaymentTerms);
    //    var listDocumentRequierments = JsonConvert.DeserializeObject<RequierdDocumentsList[]>(insertPoliciesEndorsementsManagment.RequierdDocuments);



    //    var insertPoliciesEndorsements = insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto;


    //    if (insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.EndorsRequestType == "New policy")
    //    {
    //        insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.EndorsType = "Policy";
    //    }
    //    if (insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.EndorsRequestType == "Renewal of")
    //    {
    //        insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.EndorsType = "Policy";
    //        insertPoliciesEndorsements.RenewalOf = insertPoliciesEndorsements.OasisPolRef;
    //        insertPoliciesEndorsements.OasisPolRef = string.Empty;
    //        insertPoliciesEndorsements.Renewal = "Renewal";
    //    }

    //    if (insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.EndorsRequestType == "Endorsement")
    //    {
    //        insertPoliciesEndorsements.Renewal = string.Empty;
    //    }


    //    if (insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.EndorsType == "Refund")
    //    {
    //        insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.NetPremium = insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.NetPremium * 1;
    //        insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.VatValue = insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.VatValue * 1;
    //        insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.CompCommVAT = insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.CompCommVAT * 1;
    //        insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.TotalPremium = insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.TotalPremium * 1;
    //        insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.CompComm = insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.CompComm * 1;
    //    }


    //    if (ModelState.IsValid)
    //    {

    //        using (var trans = new TransactionScope())
    //        {
    //            using (var con = new SqlConnection(ChedidModule.strconn))
    //            {

    //                int SNo = con.QuerySingle<int>("Insert Into Policies " +
    //                                       "(RequestNo, OasisPolRef, Branch, ClientNo, ClientName, Producer, PolicyHolder, AccNo, PolicyNo,EndorsType, EndorsNo, Renewal,RenewalOf," +
    //                                       " InsurComp, IssueDate, ClassName, " +
    //                                       "LineOfBusiness, MinDriverAge, PeriodFrom, PeriodTo, ClaimNoOfDays, CompExpAccs, CSNoOfDays, SumInsur,PaymentType, NetPremium, DeductFees, Fees, VatValue, VatPerc, TotalPremium, " +
    //                                       "CompCommPerc, CompComm, CompCommVAT,FullPremium, ProducerCommPerc, ProducerComm, Remarks,ClaimsSpecialConditions,CSSpecialConditions, " +
    //                                       "ClientDNCNNo, CompCommDNCNNo, SavedUser) " +
    //                                       "Values " +
    //                                       "(@RequestNo,@OasisPolRef,@Branch,@ClientNo,@ClientName,@Producer, @PolicyHolder, @AccNo,@PolicyNo,@EndorsType, @EndorsNo, @Renewal,@RenewalOf," +
    //                                       " @InsurComp, @IssueDate, @ClassName, " +
    //                                       "@LineOfBusiness, @MinDriverAge, @PeriodFrom, @PeriodTo, @ClaimNoOfDays, @CompExpAccs, @CSNoOfDays, @SumInsur,@PaymentType,@NetPremium, @DeductFees, @Fees, @VatValue,@VatPerc, @TotalPremium, " +
    //                                       "@CompCommPerc,@CompComm, @CompCommVAT,@FullPremium, @ProducerCommPerc,@ProducerComm, @Remarks,@ClaimsSpecialConditions,@CSSpecialConditions, " +
    //                                       "@ClientDNCNNo,@CompCommDNCNNo, @SavedUser) select SCOPE_IDENTITY()",
    //                         new
    //                         {
    //                             RequestNo = insertPoliciesEndorsements.RequestNo,
    //                             OasisPolRef = insertPoliciesEndorsements.OasisPolRef,
    //                             Branch = insertPoliciesEndorsements.Branch,
    //                             ClientNo = insertPoliciesEndorsements.ClientNo,
    //                             ClientName = insertPoliciesEndorsements.ClientName,
    //                             Producer = insertPoliciesEndorsements.Producer,
    //                             PolicyHolder = insertPoliciesEndorsements.PolicyHolder,
    //                             AccNo = insertPoliciesEndorsements.AccNo,
    //                             PolicyNo = insertPoliciesEndorsements.PolicyNo,
    //                             EndorsType = insertPoliciesEndorsements.EndorsType,
    //                             EndorsNo = insertPoliciesEndorsements.EndorsNo,
    //                             Renewal = insertPoliciesEndorsements.Renewal,
    //                             RenewalOf = insertPoliciesEndorsements.RenewalOf,
    //                             InsurComp = insertPoliciesEndorsements.InsurComp,
    //                             IssueDate = insertPoliciesEndorsements.IssueDate,
    //                             ClassName = insertPoliciesEndorsements.ClassName,
    //                             LineOfBusiness = insertPoliciesEndorsements.LineOfBusiness,
    //                             MinDriverAge = insertPoliciesEndorsements.MinDriverAge,
    //                             PeriodFrom = insertPoliciesEndorsements.PeriodFrom,
    //                             PeriodTo = insertPoliciesEndorsements.PeriodTo,
    //                             ClaimNoOfDays = insertPoliciesEndorsements.ClaimNoOfDays,
    //                             CompExpAccs = insertPoliciesEndorsements.CompExpAccs,
    //                             CSNoOfDays = insertPoliciesEndorsements.CSNoOfDays,
    //                             SumInsur = insertPoliciesEndorsements.SumInsur,
    //                             PaymentType = "Direct to Insurance Company"/*insertPoliciesEndorsements.PaymentType*/,
    //                             NetPremium = insertPoliciesEndorsements.NetPremium,
    //                             DeductFees = insertPoliciesEndorsements.DeductFees,
    //                             Fees = insertPoliciesEndorsements.Fees,
    //                             VatValue = insertPoliciesEndorsements.VatValue,
    //                             VatPerc = insertPoliciesEndorsements.VatPerc,
    //                             TotalPremium = insertPoliciesEndorsements.TotalPremium,
    //                             CompCommPerc = insertPoliciesEndorsements.CompCommPerc,
    //                             CompComm = insertPoliciesEndorsements.CompComm,
    //                             CompCommVAT = insertPoliciesEndorsements.CompCommVAT,
    //                             FullPremium = insertPoliciesEndorsements.FullPremium,
    //                             ProducerCommPerc = insertPoliciesEndorsements.ProducerCommPerc,
    //                             ProducerComm = insertPoliciesEndorsements.ProducerComm,
    //                             Remarks = insertPoliciesEndorsements.Remarks,
    //                             ClaimsSpecialConditions = insertPoliciesEndorsements.ClaimsSpecialConditions,
    //                             CSSpecialConditions = insertPoliciesEndorsements.CSSpecialConditions,
    //                             ClientDNCNNo = insertPoliciesEndorsements.ClientDNCNNo,
    //                             CompCommDNCNNo = insertPoliciesEndorsements.CompCommDNCNNo,
    //                             SavedUser = HttpContext.GetFullName(),


    //                         });


    //                foreach (var item in listDocumentRequierments)
    //                {
    //                    item.SavedBy = HttpContext.GetUserName();
    //                    item.PoliciesSNo = SNo;
    //                }

    //                con.Execute("Insert Into PoliciesListOfRequiredDocuments " +
    //                "(PoliciesSNo , Status , RequiredDocuments , SavedOn , SavedBy) " +
    //                "Values (@PoliciesSNo,@Status,@RequiredDocuments,@SavedOn,@SavedBy) ",
    //                listDocumentRequierments);

    //                #region PoliciesPaymentsSchedule

    //                foreach (var item in PaymentTerms)
    //                {
    //                    if (!string.IsNullOrEmpty(item.PaymentTermsDate))
    //                    {


    //                        con.Execute("Insert Into PoliciesPaymentsSchedule (PoliciesSNo,PayDate,Percentage,Amount,SavedUser,Department) Values " +
    //                                            "(@PoliciesSNo,@PayDate,@Percentage,@Amount,@SavedUser,@Department)",
    //                         new
    //                         {
    //                             PoliciesSNo = SNo,
    //                             Amount = item.PaymentTermsAmount,
    //                             Department = insertPoliciesEndorsements.Branch,
    //                             PayDate = item.PaymentTermsDate,
    //                             Percentage = item.PaymentTermsPerc,
    //                             SavedUser = HttpContext.GetFullName()


    //                         });
    //                    }
    //                }

    //                #endregion

    //                #region producer

    //                foreach (var item in producerLists)
    //                {
    //                    if (!string.IsNullOrEmpty(item.Name))
    //                    {


    //                        con.Execute("Insert Into PoliciesProducersCommissions (PoliciesSNo,Producer,Percentage,Amount,SavedBy) Values " +
    //                                  "(@PoliciesSNo,@Producer,@Percentage,@Amount,@SavedBy)",
    //                         new
    //                         {
    //                             PoliciesSNo = SNo,
    //                             Amount = 0,
    //                             Department = insertPoliciesEndorsements.ProducerCommPerc,
    //                             Producer = item.Name,
    //                             Percentage = 0,
    //                             SavedBy = HttpContext.GetFullName()


    //                         });
    //                    }
    //                }

    //                #endregion

    //                //#region Insert RequiredDocuments
    //                //foreach (var item in insertPoliciesListOfRequiredDocuments)
    //                //{

    //                //    Document.UploadFiles(Document.Folders.Production, item.PoliciesSNo, item.File);
    //                //    con.Query<InsertPoliciesListOfRequiredDocumentsDto>("Insert Into PoliciesListOfRequiredDocuments(PoliciesSNo, Status, RequiredDocuments, SavedBy) Values " +
    //                //                          "(@PoliciesSNo,@Status,@RequiredDocuments,@SavedBy",
    //                //      new
    //                //      {
    //                //          SNo,
    //                //          item.RequiredDocuments,
    //                //          item.Status,
    //                //          item.SavedBy

    //                //      });
    //                //}
    //                //#endregion

    //                if (insertPoliciesEndorsementsManagment.DocumentsModel.Documents.Count > 0)
    //                    Helpers.Document.UploadFiles(Helpers.Document.Folders.Production, SNo.ToString(), insertPoliciesEndorsementsManagment.DocumentsModel.Documents);
    //            }
    //            trans.Complete();
    //            //Alert("This is success message", NotificationType.success);
    //        }

    //    }



    //    insertPoliciesEndorsementsManagment.ProducersList = MasterTablesHelper.GetProducersList();
    //    insertPoliciesEndorsementsManagment.BranchList = MasterTablesHelper.GetBranchesList();
    //    insertPoliciesEndorsementsManagment.ClassOfInsurance = MasterTablesHelper.GetInsuranceClassesList();
    //    insertPoliciesEndorsementsManagment.PolicyEndorsType = MasterTablesHelper.GetPolicyEndorsTypeList();
    //    insertPoliciesEndorsementsManagment.ClientsList = MasterTablesHelper.GetClientList();
    //    insertPoliciesEndorsementsManagment.InsuranceCompaniesList = MasterTablesHelper.GetInsuranceCompaniesList();
    //    insertPoliciesEndorsementsManagment.LineOfBusiness = MasterTablesHelper.GetLineOfBusinessList(insertPoliciesEndorsementsManagment.InsertPoliciesEndorsementsManagmentDto.ClassName);
    //    insertPoliciesEndorsementsManagment.PaymentTermsList = JsonConvert.DeserializeObject<PaymentTermsView[]>(insertPoliciesEndorsementsManagment.PaymentTerms);
    //    insertPoliciesEndorsementsManagment.ProducersListType = JsonConvert.DeserializeObject<ProducerList[]>(insertPoliciesEndorsementsManagment.ProducersListPaased);
    //    return View("../PoliciesEndorsementManagment/PoliciesEndorsementNew", insertPoliciesEndorsementsManagment);

    //}

    //[HttpGet]
    //public async Task<IActionResult> LoadPoliciyData(string ClientNo, string SNo)
    //{
    //    var policies = new PolicyViewModel();

    //    var policiesPaymentsSchedule = new PoliciesPaymentsScheduleViewModel();
    //    var QueryString = " Select * From Policies Where SNo=@SNo";
    //    var QueryStringPoliciesPaymentsSchedule = "Select PoliciesSNo,PayDate,Percentage,Amount,SavedUser,Department from PoliciesPaymentsSchedule where PoliciesSNo=@PoliciesSNo";
    //    var QueryStringProducer = "Select PoliciesSNo,Producer,Percentage,Amount,SavedBy from PoliciesProducersCommissions where PoliciesSNo=@PoliciesSNo ";
    //    using (var con = new SqlConnection(ChedidModule.strconn))
    //    {

    //        policies = await con.QueryFirstOrDefaultAsync<PolicyViewModel>(QueryString, new { SNo = SNo });
    //        policies.PoliciesPaymentsSchedule = await con.QueryAsync<PoliciesPaymentsScheduleViewModel>(QueryStringPoliciesPaymentsSchedule, new { PoliciesSNo = SNo });
    //        policies.ProducerViewModel = await con.QueryAsync<ProducerViewModel>(QueryStringProducer, new { PoliciesSNo = SNo });
    //    }
    //    policies.DocumentsModel.Documents = Helpers.Document.DownloadFiles(Helpers.Document.Folders.Production, SNo.ToString());
    //    policies.DocumentsModel.SNo = long.Parse(SNo);
    //    policies.DocumentsModel.FolderName = "Production";
    //    return View("../PoliciesEndorsementManagment/PolicyDetails", policies);

    //}




    //public JsonResult UpdatePolicyStatus(PolicyPreview req)
    //{

    //    using (var conn = new SqlConnection(ChedidModule.strconn))
    //    {
    //        conn.Execute("Update Policies SET ClientDNCNNo=@ClientDNCNNo,CompCommDNCNNo=@CompCommDNCNNo,DeliveryStatus=@DeliveryStatus,DeliveryUpdatedBy=@DeliveryUpdatedBy,DeliveryUpdatedOn=GetDate() WHERE SNo=" + req.SNo, new
    //        {
    //            ClientDNCNNo = req.ClientDNCNNo,
    //            CompCommDNCNNo = req.CompCommDNCNNo,
    //            DeliveryStatus = req.DeliveryStatus,
    //            DeliveryUpdatedBy = HttpContext.GetUserName()
    //        });
    //    }
    //    return Json("");
    //}

    //public ActionResult GetClient(ClientDto client)
    //{
    //    IEnumerable<ClientDto> Clients = new List<ClientDto>();
    //    string strCond = "";
    //    string orderby = "Order by FullName";
    //    if (!string.IsNullOrEmpty(client.FullName))
    //    {
    //        strCond += " And FullName Like N'%" + client.FullName + "%'";
    //    }
    //    if (client.SNo != null)
    //    {
    //        strCond = " And SNo= N'" + client.SNo + "'";
    //    }
    //    if (client.SNo == null && string.IsNullOrEmpty(client.FullName))
    //    {
    //        strCond = "";
    //    }
    //    using (var transactionScope = new TransactionScope())
    //    {
    //        using (var conn = new SqlConnection(ChedidModule.strconn))
    //        {
    //            /* --------------------------- Get Client Details --------------------------- */
    //            Clients = conn.Query<ClientDto>("Select Top 100 SNo,FullName,Producer,Status From Clients Where SNo Is Not Null " + strCond + orderby);
    //        }

    //        transactionScope.Complete();
    //    }
    //    return Json(new { data = Clients });
    //}

    //public ActionResult GetRequest(RequestSearchResponceDto request)
    //{
    //    IEnumerable<RequestSearchDto> Clients = new List<RequestSearchDto>();

    //    StringBuilder strCod = new StringBuilder();

    //    if (!string.IsNullOrEmpty(request.FullName))
    //    {
    //        strCod.Append(" ClientName Like N'%" + request.FullName + "%'");
    //    }
    //    if (request.dateFrom != null)
    //    {
    //        strCod.Append(" and SavedDate>=N'" + request.dateFrom + "'");
    //    }
    //    if (request.dateTo != null)
    //    {
    //        strCod.Append(" and SavedDate<=N'" + request.dateTo + "'");
    //    }
    //    using (var transactionScope = new TransactionScope())
    //    {
    //        using (var conn = new SqlConnection(ChedidModule.strconn))
    //        {
    //            /* --------------------------- Get Client Details --------------------------- */
    //            var query = "Select RequestNo,EndorsType,ClientID,ClientName,ClassOfBusiness,LineOfBusiness,Status," +
    //                "PolicySerial,ClientPolicySNo From CRMRequests Where" + strCod + " and  Status =N'Closed' And RequestNo Not In (Select Distinct RequestNo From Policies Where DoneFin = 1)" +
    //                " Order by ClientName";
    //            Clients = conn.Query<RequestSearchDto>(query);
    //        }

    //        transactionScope.Complete();
    //    }
    //    return Json(new { data = Clients });
    //}


    //public JsonResult GetRequiredDocumentsNameList(string requestType)
    //{


    //    IEnumerable<RequiredDocuments> data;

    //    var QueryString = "Select DocName From ListOfRequiredDocumentsPolicies " + "Where PolicyIssueType=N'" + requestType + "'" +
    //        " Order By DocName";
    //    using (var con = new SqlConnection(ChedidModule.strconn))
    //    {
    //        data = con.Query<RequiredDocuments>(QueryString);
    //    }
    //    return Json(data);

    //}


    //public JsonResult GetLockDate()
    //{
    //    string queryperiodDate = "Select LockDate From LockDateProduction";

    //    var periodDate = new object();

    //    using (var con = new SqlConnection(ChedidModule.strconn))
    //        periodDate = con.ExecuteScalar(queryperiodDate);

    //    return Json(periodDate);
    //}
}

public class PaymentTermsView
{
    public string PaymentTermsDate { get; set; }
    public string PaymentTermsPerc { get; set; }
    public string PaymentTermsAmount { get; set; }
}

public class ProducerList
{

    public string Name { get; set; }
}

