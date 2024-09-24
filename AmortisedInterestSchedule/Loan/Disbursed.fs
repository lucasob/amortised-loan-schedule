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
        term |> int

    member this.paymentAmount =
        amortisedPaymentAmount this.Amount this.Rate.Amount 12.0 this.GetTerm

    member this.dueDates =
        [1 .. this.GetTerm] |> List.map this.DisbursalDate.AddMonths
        
    member this.spans =
        let calculated = this.dueDates |> List.pairwise |> List.map (fun (l, r) -> r.Subtract(l))
        let opening = (List.head this.dueDates).Subtract(this.DisbursalDate)
        opening :: calculated


let ofQuote (quote: QuotedLoan) disbursalDate =
    { Amount = quote.Amount
      Term = quote.Term
      Rate = quote.Rate
      DisbursalDate = disbursalDate }
