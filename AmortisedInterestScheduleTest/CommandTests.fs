module AmortisedInterestScheduleTest.CommandTests

open Xunit
open AmortisedInterestSchedule.Command

[<Fact>]
let ``Given a valid map, confirm we can create a loan schedule`` () =
    let args = [| "Main.fs"; "--rate"; "0.05"; "--amount"; "1000.0"; "--term"; "12" |]
    let expected = Map [("--rate", "0.05"); ("--amount", "1000.0"); ("--term", "12") ] |> Map.toList
    let output = (args |> parse |> function | Ok m -> m | _ -> failwith "No") |> Map.toList
    
    Assert.Equal<List<string * string>>(expected, output)
