module AmortisedInterestScheduleTest.MainTest

open Xunit
open AmortisedInterestSchedule
open AmortisedInterestSchedule.Loan.Quoted

[<Fact>]
let ``Bootstrap -> Run main with args returns valid calculation`` () =
    let args = [| "Main.fs"; "--rate"; "0.05"; "--amount"; "1000.0"; "--term"; "12" |]

    let output =
        Command.run args
        |> function
            | Ok instalments -> instalments |> List.last |> (fun i -> i.WithRounding 2)
            | Error _ -> failwith "Dead"

    let expected =
        { OpeningBalance = 85.25
          ClosingBalance = 0.0
          Interest = 0.36
          Principal = 85.25 }

    Assert.Equal(expected, output)
