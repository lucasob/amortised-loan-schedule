module AmortisedInterestSchedule.Loan.Disbursed

open System
open AmortisedInterestSchedule.Loan.Quoted
open AmortisedInterestSchedule.Loan.Instalment
open AmortisedInterestSchedule.Loan.Core
open AmortisedInterestSchedule.Term
open AmortisedInterestSchedule.Rate

type DisbursedLoan =
    { Amount: double
      Term: Term
      Rate: Rate
      DisbursalDate: DateTime }
    
    member this.GetTerm =
        let (Term term) = this.Term
        term |> double

    member this.paymentAmount =
        amortisedPaymentAmount this.Amount this.Rate.Amount 12.0 this.GetTerm

    member this.schedule: Disbursed list = [] 


let ofQuote (quote: QuotedLoan) disbursalDate =
    { Amount = quote.Amount
      Term = quote.Term
      Rate = quote.Rate
      DisbursalDate = disbursalDate }
