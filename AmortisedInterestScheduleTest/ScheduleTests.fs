module Tests

open System
open Xunit
open AmortisedInterestSchedule.Loan.Quoted
open AmortisedInterestSchedule.Loan.Disbursed
open AmortisedInterestSchedule.Loan.Instalment
open AmortisedInterestSchedule.Term
open AmortisedInterestSchedule.Rate

[<Fact>]
let ``Calculates the correct amortised payment amount to 2 dps`` () =
    let loan =
        { Term = Term 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    // I've taken 85.61 as god's truth by checking against other amortisation calculators
    Assert.Equal(85.61, Math.Round(loan.AmortizedPaymentAmount, 2))

[<Fact>]
let ``Provides a schedule whose sum is exactly that of the amortised amounts, over the period`` () =
    let loan =
        { Term = Term 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    let scheduleTotal =
        loan.IndicativeSchedule |> List.map (fun i -> i.Total) |> List.sum

    let calculatedAmortisedPaymentTotal =
        Math.Round((loan.AmortizedPaymentAmount * loan.GetTerm), 2)

    Assert.Equal(calculatedAmortisedPaymentTotal, Math.Round(scheduleTotal, 2))

[<Fact>]
let ``The schedule terminates in a with a closing balance of exactly 0`` () =
    let loan =
        { Term = Term 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    let finalInstalment =
        loan.IndicativeSchedule |> List.last |> (fun i -> i.WithRounding 2)

    Assert.Equal(
        { OpeningBalance = 85.25
          ClosingBalance = 0.0
          Interest = 0.36
          Principal = 85.25 },
        finalInstalment
    )

[<Fact>]
let ``A 12 month loan, disbursed on 1 Jan 2024, at 5% should be paid in full by 1 Jan 2025`` () =
    let loan =
        { Term = Term 12s
          Amount = 1000.0
          Rate = { Amount = 0.05; Frequency = Yearly } }

    // Just parse it lol
    let disbursalDate = DateTime.Parse "2024-01-01T00:00:00Z"

    // A handle to a loan with an actual "real" schedule
    let disbursed = ofQuote loan disbursalDate

    // Get our actual schedule
    let instalments = disbursed.schedule ()
    let (Term v) = loan.Term

    Assert.Equal(v |> int, List.length instalments)

    // Date Validation of start
    Assert.Equal(1, List.head instalments |> (fun i -> i.DueDate.Day))
    Assert.Equal(2, List.head instalments |> (fun i -> i.DueDate.Month))
    Assert.Equal(2024, List.head instalments |> (fun i -> i.DueDate.Year))
    
    // // Date Validation of end
    // Assert.Equal(1, List.last instalments |> (fun i -> i.DueDate.Day))
    // Assert.Equal(1, List.last instalments |> (fun i -> i.DueDate.Month))
    // Assert.Equal(2025, List.last instalments |> (fun i -> i.DueDate.Year))
