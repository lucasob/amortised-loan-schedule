module AmortisedInterestSchedule.Loan

open System
open AmortisedInterestSchedule.Term
open AmortisedInterestSchedule.Rate

type Loan =
    { Amount: double
      Term: Term
      Rate: Rate }

    member this.GetTerm =
        let (Term t) = this.Term
        t |> double

    member this.GetRate = this.Rate.Amount / 12.0

    member this.AmortizedPaymentAmount =
        this.Amount
        * ((this.GetRate * ((1.0 + this.GetRate) ** this.GetTerm))
           / (((1.0 + this.GetRate) ** (this.GetTerm |> float)) - 1.0))

type Instalment =
    { OpeningBalance: float
      ClosingBalance: float
      Principal: float
      Interest: float }

    member this.Total = this.Principal + this.Interest

    member this.WithRounding(places: int) =
        { this with
            OpeningBalance = Math.Round(this.OpeningBalance, places)
            ClosingBalance = Math.Round(this.ClosingBalance, places)
            Principal = Math.Round(this.Principal, places)
            Interest = Math.Round(this.Interest, places) }

let rec private ToInst' (outstandingBalance: float) periods (rate: float) amortisedPaymentAmount instalments =
    if List.length instalments = periods then
        instalments
    else
        let interestAccruedInPeriod = rate * outstandingBalance
        let balanceWithInterestApplied = interestAccruedInPeriod + outstandingBalance

        let principalComponent =
            if List.length instalments = (periods - 1) then
                balanceWithInterestApplied - interestAccruedInPeriod
            else
                (amortisedPaymentAmount - interestAccruedInPeriod)

        let actualPaymentAmount = principalComponent + interestAccruedInPeriod
        let closingBalancePostPayment = balanceWithInterestApplied - actualPaymentAmount

        let nextInstalment =
            { OpeningBalance = outstandingBalance
              Interest = interestAccruedInPeriod
              Principal = principalComponent
              ClosingBalance = closingBalancePostPayment }

        ToInst' nextInstalment.ClosingBalance periods rate amortisedPaymentAmount (nextInstalment :: instalments)

let ToInstalments loan =
    ToInst' loan.Amount (loan.GetTerm |> int) loan.GetRate loan.AmortizedPaymentAmount []
    |> List.rev
