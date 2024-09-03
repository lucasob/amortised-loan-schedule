module AmortisedInterestSchedule.Main

open System.Text.Json
open AmortisedInterestSchedule.Command
open AmortisedInterestSchedule.Util

[<EntryPoint>]
let main argv =
    let parsed =
        argv
        |> parse
        |> function
            | Ok m -> toLoan m
            | Error s -> ParseError.DoubleParseError |> Error
        |> function
            | Ok(loan: Loan.Loan) -> JsonSerializer.Serialize(loan.IndicativeSchedule)
            | Error e -> e |> string

    printf $"{parsed}"
    0
