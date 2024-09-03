module AmortisedInterestSchedule.Util

open System

type ParseError =
    | ShortParseError
    | DoubleParseError

let private matchParse (e: ParseError) =
    function
    | true, (v: 'T) -> v |> Ok
    | _ -> e |> Error

let tryParseShort (s: string) =
    (Int16.TryParse s) |> matchParse ParseError.ShortParseError

let tryParseDouble (s: string) =
    (Double.TryParse s) |> matchParse ParseError.DoubleParseError

let isOk =
    function
    | Ok _ -> true
    | Error _ -> false

module List =

    let rec private partition' n list =
        let taken, rest = List.splitAt n list

        if List.length rest < n then
            [ List.concat [ taken; rest ] ]
        else
            taken :: partition' n rest

    /// <summary>
    /// Essentially just me not knowing that List.chunkBySize was a function. Clojure calls it partition
    /// and frankly that's just a far better name. Regardless, this was a fun exercise.
    /// </summary>
    /// <param name="n">The size of the buckets</param>
    /// <param name="inputList">The input list of values</param>
    let partitionInto n inputList =
        match inputList with
        | i when List.length i = 1 -> [ i ]
        | i when List.length i > 1 -> partition' n inputList
        | _ -> []
