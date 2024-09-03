module AmortisedInterestSchedule.Command

open AmortisedInterestSchedule.Rate
open AmortisedInterestSchedule.Util
open AmortisedInterestSchedule.Loan
open Microsoft.FSharp.Core

let parse (arguments: string array) =
    match (arguments |> List.ofArray) with
    | [] -> Error "This isn't possible"
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

let toLoan arguments =
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

    // This is diabolical, and I am sorry, but the other option is the fish operator (Kleisli composition)
    // and because everything here is independent (technically) it just doesn't feel right
    term
    |> Result.bind (fun t' -> rate |> Result.bind (fun r' -> amount |> Result.map (fun a' -> (t', r', a'))))
    |> function
        | Ok(t', r', a') ->
            { Term = Term.create t'
              Amount = a'
              Rate =
                { Amount = r'
                  Frequency = RateFrequency.Yearly } }
            |> Ok
        | Error e -> e |> Error
