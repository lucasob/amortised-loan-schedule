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

    // Indicative cost
    let indicativeCost =
        loan.IndicativeSchedule |> List.map (fun i -> i.Total) |> List.sum

    // Just parse it lol
    let disbursalDate = DateTime.Parse "2024-01-01T00:00:00Z"

    // A handle to a loan with an actual "real" schedule
    let disbursed = ofQuote loan disbursalDate

    // Get our due Dates
    let dueDates = disbursed.dueDates
    let spans = disbursed.spans |> List.map (fun s -> s.Days)
    let (Term v) = loan.Term

    // Are our ranges the correct length
    Assert.Equal(v |> int, List.length dueDates)
    Assert.Equal(v |> int, List.length spans)

    // Date Validation of start
    Assert.Equal(1, List.head dueDates |> (fun i -> i.Day))
    Assert.Equal(2, List.head dueDates |> (fun i -> i.Month))
    Assert.Equal(2024, List.head dueDates |> (fun i -> i.Year))

    // Date Validation of end
    Assert.Equal(1, List.last dueDates |> (fun i -> i.Day))
    Assert.Equal(1, List.last dueDates |> (fun i -> i.Month))
    Assert.Equal(2025, List.last dueDates |> (fun i -> i.Year))

    // Actually test the schedule
    let disbursedSchedule = disbursed.schedule
    let disbursedCost = disbursedSchedule |> List.map (fun i -> i.Total) |> List.sum

    // Compare indicative and disbursed
    let firstDue = List.head disbursedSchedule
    let quoted = List.head loan.IndicativeSchedule

    Assert.Equal(quoted.Total, firstDue.Total)
    
    // Tolerance of 1/365
    let errorRange value = (1. / 365.) * value

    // Given the quote uses some very "applied" calculations, we cannot actually quote to a p/cent.
    // This is also due to the fact years are kinda 365.25 days long.
    // Given we're within 0.27% (.25 days), I'm okay.
    Assert.InRange(
        disbursedCost,
        (indicativeCost - errorRange indicativeCost),
        (indicativeCost + errorRange indicativeCost)
    )


[<Fact>]
let ``At 18.71% for 1500 at 12 months`` () =
    // Based off a real loan

    let loan =
        { Term = Term 12s
          Amount = 1500.0
          Rate = { Amount = 0.1871; Frequency = Yearly } }

    let disbursalDate = DateTime.Parse("2023-11-17")

    let disbursedLoan = ofQuote loan disbursalDate

    let disbursedTotalCost =
        disbursedLoan
        |> (fun l -> l.schedule)
        |> List.map (fun i -> i.Total)
        |> List.sum

    
    // Tolerance of 1/365
    let errorRange value = (1. / 365.) * value
    let providedValue = 1656.97

    Assert.InRange(
        disbursedTotalCost,
        (providedValue - errorRange providedValue),
        (providedValue + errorRange providedValue)
    )

    let installmentAmount =
        disbursedLoan.schedule
        |> List.map (fun i -> i.Total)
        |> List.groupBy id
        |> List.map (fun (g, _) -> Math.Round(g, 2))
        |> List.head

    Assert.Equal(138.03, installmentAmount)
