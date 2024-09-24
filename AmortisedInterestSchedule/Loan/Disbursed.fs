module AmortisedInterestSchedule.Loan.Disbursed

open System
open AmortisedInterestSchedule.Loan.Quoted
open AmortisedInterestSchedule.Loan.Instalment
open AmortisedInterestSchedule.Term
open AmortisedInterestSchedule.Rate

type DisbursedLoan =
    { Amount: double
      Term: Term
      Rate: Rate
      DisbursalDate: DateTime }

    member this.schedule() =
        [ { OpeningBalance = this.Amount
            ClosingBalance = 0
            Principal = 0
            Interest = 0
            DueDate = this.DisbursalDate.AddMonths(1)
            PaidDate = None
            Status = Status.Pending } ]

let ofQuote (quote: QuotedLoan) disbursalDate =
    { Amount = quote.Amount
      Term = quote.Term
      Rate = quote.Rate
      DisbursalDate = disbursalDate }
