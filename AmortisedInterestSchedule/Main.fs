module AmortisedInterestSchedule.Main

open System.Text.Json

[<EntryPoint>]
let main argv =
    match Command.run argv with
    | Ok instalments -> JsonSerializer.Serialize instalments
    | Error e -> e |> string
    |> printfn "%s"

    0
