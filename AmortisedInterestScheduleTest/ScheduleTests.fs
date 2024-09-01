module Tests

open System
open Xunit
open AmortisedInterestSchedule.Loan
open AmortisedInterestSchedule.Term
open AmortisedInterestSchedule.Rate

[<Fact>]
let ``Calculates the correct amortised payment amount to 2 dps`` () =
    let loan =
        { Term = Create 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    // I've taken 85.61 as god's truth by checking against other amortisation calculators
    Assert.Equal(85.61, Math.Round(loan.AmortizedPaymentAmount, 2))

[<Fact>]
let ``Provides a schedule whose sum is exactly that of the amortised amounts, over the period`` () =
    let loan =
        { Term = Create 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    let scheduleTotal = ToInstalments loan |> List.map (fun i -> i.Total) |> List.sum
    let calculatedAmortisedPaymentTotal = Math.Round((loan.AmortizedPaymentAmount * loan.GetTerm), 2)
    Assert.Equal(calculatedAmortisedPaymentTotal, Math.Round(scheduleTotal, 2))

[<Fact>]
let ``The schedule terminates in a with a closing balance of exactly 0`` () =
    let loan =
        { Term = Create 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    let finalInstalment = ToInstalments loan |> List.last |> (fun i -> i.WithRounding 2)

    Assert.Equal(
        { OpeningBalance = 85.25
          ClosingBalance = 0.0
          Interest = 0.36
          Principal = 85.25 },
        finalInstalment
    )
