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
        
    member this.schedule =
        let dayRate = this.Rate.Amount / 365.
        let rec calc outstandingBalance rate (context: (DateTime * TimeSpan) list) =
            match context with
            | [] -> []
            | [dueDate, daysToDate] ->  // last case
                let interestInPeriod = rate * (daysToDate.Days |> double) * outstandingBalance
                let paymentAmount = outstandingBalance
                let principalAmount = paymentAmount - interestInPeriod
                let closingBalance = outstandingBalance - paymentAmount
                let latestInstalment =
                    {OpeningBalance = outstandingBalance
                     ClosingBalance = closingBalance
                     Principal = principalAmount
                     Interest = interestInPeriod
                     DueDate = dueDate
                     PaidDate = None
                     Status = Status.Pending }
                latestInstalment :: (calc closingBalance rate [])
                
            | (dueDate, daysToDate) :: rest ->
                let interestInPeriod = rate * (daysToDate.Days |> double) * outstandingBalance
                let paymentAmount = this.paymentAmount
                let closingBalance = outstandingBalance + interestInPeriod - paymentAmount
                let principalAmount = paymentAmount - interestInPeriod
                let latestInstalment =
                    {OpeningBalance = outstandingBalance
                     ClosingBalance = closingBalance
                     Principal = principalAmount
                     Interest = interestInPeriod
                     DueDate = dueDate
                     PaidDate = None
                     Status = Status.Pending }
                latestInstalment :: (calc closingBalance rate rest)
                
        let context = List.zip this.dueDates this.spans
        calc this.Amount dayRate context


let ofQuote (quote: QuotedLoan) disbursalDate =
    { Amount = quote.Amount
      Term = quote.Term
      Rate = quote.Rate
      DisbursalDate = disbursalDate }
