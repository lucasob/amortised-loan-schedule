module AmortisedInterestSchedule.Command

open AmortisedInterestSchedule.Rate
open AmortisedInterestSchedule.Util
open AmortisedInterestSchedule.Loan
open Microsoft.FSharp.Core

let toLoanMap (arguments: string array) =
    match (arguments |> List.ofArray) with
    | _ :: parameters ->
        parameters
        |> List.partitionInto 2
        |> List.map (function
            | [ k; v ] -> Some(k, v)
            | _ -> None)
        |> List.choose Some
        |> List.map Option.get
        |> Map.ofList
        |> Ok
    | _ -> Error "This isn't possible"

let private toLoan arguments =
    let term =
        Map.tryFind "--term" arguments
        |> function
            | Some t -> tryParseShort t
            | None -> ParseError.DoubleParseError |> Error

    let rate =
        Map.tryFind "--rate" arguments
        |> function
            | Some t -> tryParseDouble t
            | None -> ParseError.DoubleParseError |> Error

    let amount =
        Map.tryFind "--amount" arguments
        |> function
            | Some t -> tryParseDouble t
            | None -> ParseError.DoubleParseError |> Error

    match (term, rate, amount) with
    | Ok t, Ok r, Ok a ->
        Ok
            { Term = Term.create t
              Amount = a
              Rate =
                { Amount = r
                  Frequency = RateFrequency.Yearly } }
    | _ -> "Poo" |> Error

let run args =
    args
    |> toLoanMap
    |> function
        | Ok m -> toLoan m
        | Error s -> s |> Error
    |> function
        | Ok loan -> Ok loan.IndicativeSchedule
        | Error e -> e |> Error
