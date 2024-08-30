module AmortisedInterestSchedule.Loan

open System
open AmortisedInterestSchedule.Term
open AmortisedInterestSchedule.Rate

type Loan =
    { Amount: float
      Term: Term
      Rate: Rate }

    member this.GetTerm =
        let (Term t) = this.Term
        t

    member this.GetRate = this.Rate.Amount / 12.0

    member this.AmortizedPaymentAmount =
        let value =
            this.Amount
            * ((this.GetRate * ((1.0 + this.GetRate) ** (this.GetTerm |> float)))
               / (((1.0 + this.GetRate) ** (this.GetTerm |> float)) - 1.0))

        Math.Round(value, 2)

type Instalment =
    { OpeningBalance: float
      ClosingBalance: float
      Principal: float
      Interest: float }

    member this.Total = this.Principal + this.Interest

let rec ToInst' (balance: float) periods (rate: float) payment instalments =
    if List.length instalments = periods then
        instalments
    else
        let interestAccruedInPeriod = Math.Round((rate * balance), 5)
        let balanceWithInterestApplied = Math.Round((interestAccruedInPeriod + balance), 5)
        let principalComponent = Math.Round((payment - interestAccruedInPeriod), 5)
        let closingBalancePostPayment = Math.Round((balanceWithInterestApplied - payment), 5)

        let nextInstalment =
            { OpeningBalance = balance
              Interest = interestAccruedInPeriod
              Principal = principalComponent
              ClosingBalance = closingBalancePostPayment }

        ToInst' nextInstalment.ClosingBalance periods rate payment (nextInstalment :: instalments)

let ToInstalments loan =
    ToInst' loan.Amount (loan.GetTerm |> int) loan.GetRate loan.AmortizedPaymentAmount []
    |> List.rev
