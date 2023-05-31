Select SNo,FullName,Producer From Clients


Select * From Clients order by FullName

Insert Into Groups (GroupName) Values ('Case')

select * from Groups

Delete From Groups Where SNo= 2061

Select * From ClientsGroups where GroupName = 'Ahmed'

Select IsNull(Count(*),0) From ClientsGroups Where GroupName='ABB' and ClientID=467

DELETE FROM Groups Where SNo = 2069;

Select SNo,IsNull(ClientID,'') ClientID From ClientsGroups

Select CommericalNo From Clients where CommericalNo Is Not Null

select FullName from Clients


Select SNo as ID, CommericalNo as Name From Clients where CommericalNo Is Not Null and not CommericalNo  =' '

Select Distinct SNo as ID,GroupName as Name From Groups where GroupName Is Not Null


Select ClientsGroups.SNo,IsNull(ClientsGroups.ClientID,'') ClientID , Clients.FullName as [Client Name] 
From ClientsGroups inner join Clients on ClientsGroups.SNo = Clients.SNo Where ClientsGroups.GroupName='ABB'



-------------------------------------------------------------------------------------------------------------------------

Select SNo,FullName,Producer From Clients


Select * From Clients order by FullName

Insert Into Groups (GroupName) Values ('Case')


Select * From ClientsGroups where GroupName = 'Ahmed'

Select IsNull(Count(*),0) From ClientsGroups Where GroupName='ABB' and ClientID=467


DELETE FROM Groups Where SNo = 2069;

Select SNo,IsNull(ClientID,'') ClientID From ClientsGroups

Select CommericalNo From Clients where CommericalNo Is Not Null

select FullName from Clients


Select SNo as ID, CommericalNo as Name From Clients where CommericalNo Is Not Null and not CommericalNo  = ' '

Select Distinct SNo as ID,GroupName as Name From Groups where GroupName Is Not Null


Select Groups.SNo,IsNull(Clients.SNo,'') ClientID , Clients.FullName as [Client Name] From Groups inner join Clients on Groups.SNo = Clients.SNo Where Groups.GroupName='Athar Alkhir Group'


select * from Groups

Select * From ClientsGroups

Select * From Clients where SNo = 37

Select ClientsGroups.SNo,IsNull(ClientsGroups.ClientID,'') ClientID , Clients.FullName as [Client Name] 
From ClientsGroups inner join Clients on ClientsGroups.ClientID = Clients.SNo Where ClientsGroups.GroupName='Ahmed'


Select SNo,FullName,Producer From Clients where SNo Is Not Null




---------------------------------------------------------------------------------------------------------------------


Select Distinct SNo ID, FullName Name From Users Order By FullName

select * from Policies where ClientNo = 372


Select  SNo,'Active' PolicyStatus,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,ClientNo,ClientName,LineOfBusiness,Producer,InsurComp,
ClassName,IsNull(AccNo,N'') AccNo,PolicyNo,
IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType,ClientDNCNNo,
SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium,
CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc,
IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo,
SavedUser,SavedDate,
Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, 
DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,FinVoucherNo, 
IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions 
Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium 
From Policies
Where  SNo<>0 And Done=1 and DoneFin=1 and PeriodTo>=GetDate()  


Select SNo,IsNull(DeliveryStatus,N'') DeliveryStatus,Branch,OasisPolRef,IsNull(ReversalOf,N'') ReversalOf,
ClientNo,ClientName,IsNull(PolicyHolder,N'') PolicyHolder,LineOfBusiness,Producer,InsurComp,
ClassName,IsNull(AccNo,N'') AccNo,PolicyNo,
IsNull(EndorsNo,N'') EndorsNo,IsNull(Renewal,N'') Renewal ,IsNull(EndorsType,N'Policy') EndorsType, 
IsNull(CertificationNo,N'') CertificationNo,ClientDNCNNo,
SumInsur,NetPremium,Fees,VatPerc,VatValue,TotalPremium, 
CompCommDNCNNo,CompCommPerc,CompComm,CompCommVAT,ProducerComm,ProducerCommPerc,
IsNull(IssueDate,N'') IssueDate,PeriodFrom,PeriodTo,
SavedUser,SavedDate,
Done,ApprovedUser,ApprovedDate,Reject,ProdRejectInfo,ProdRejectBy,ProdRejectOn,DeliveryUpdatedBy,DeliveryUpdatedOn, 
DoneFin,FinApprovedUser,FinApprovedDate,FinEntryDate,FinRejectInfo,FinRejectBy,FinRejectOn,IsNull(FinVoucherNo,N'') FinVoucherNo, 
IsNull((Select Sum(SettledCr)-Sum(SettledDr) From Transactions 
        Where Transactions.PolRef=Policies.SNo and Transactions.TransType=N'Production Voucher' and Transactions.AccNo Like N'999000-40100%'), 0) PaidPremium, 
IsNull((Select Distinct Top 1 DocSNo From DebitCreditNotes Where PoliciesSNo=Policies.SNo),N'') DebitCreditNoteDocSNo,
IsNull((Select Distinct Top 1 DocSNo From DebitCreditNotesCommission Where PoliciesSNo=Policies.SNo),N'') TaxInvoiceDocSNo 
From Policies 
Where  SNo Is Not Null


Select * From PoliciesPaymentsSchedule

Select IsNull(PayDate, N'') PayDate , IsNull(Amount, N'') Amount ,IsNull(VATAmount, N'') VATAmount From PoliciesPaymentsSchedule where SNo

Select * From PoliciesProducersCommissions

Select IsNull(Producer, N'') Producer , IsNull(Percentage, N'') Percentage , IsNull(Amount, N'') Amount From PoliciesProducersCommissions where SNo



select DeliveryStatus from Policies where DeliveryStatus = 'Delivered'
select DeliveryStatus from Policies where DeliveryStatus = 'UnDelivered'
select DeliveryStatus from Policies where DeliveryStatus = 'Pending'

