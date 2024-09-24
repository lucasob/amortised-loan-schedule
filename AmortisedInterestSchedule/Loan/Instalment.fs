module AmortisedInterestSchedule.Loan.Instalment

open System

type Status =
    | Pending
    | Paid
    | Late

type Quoted =
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

type Disbursed =
    { OpeningBalance: float
      ClosingBalance: float
      Principal: float
      Interest: float
      DueDate: DateTime
      PaidDate: DateTime option
      Status: Status }
    
type Instalment =
    | Quoted
    | Disbursed
