module Tests

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
    Assert.Equal(85.61, loan.AmortizedPaymentAmount)

[<Fact>]
let ``Provides a schedule whose sum is exactly that of the amortised amounts, over the period`` () =
    let loan =
        { Term = Create 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    let scheduleTotal = ToInstalments loan |> List.map (fun i -> i.Total) |> List.sum
    Assert.Equal((loan.AmortizedPaymentAmount * (loan.GetTerm |> float)), scheduleTotal)

[<Fact>]
let ``The schedule terminates in a final payment that does not exceed the total amount payable`` () =
    let loan =
        { Term = Create 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    let finalInstalment = ToInstalments loan |> List.last

    Assert.Equal(
        { OpeningBalance = 85.22399
          ClosingBalance = 0.0
          Interest = 0.3551
          Principal = 85.2549 },
        finalInstalment
    )
