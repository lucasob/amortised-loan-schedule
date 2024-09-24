module AmortisedInterestSchedule.Term


type Term = Term of int16

let private (|ValidTerm|IllegalTerm|) (v: int16) =
    match v with
    | v when v < 1s -> IllegalTerm
    | v when v > 60s -> IllegalTerm
    | _ -> ValidTerm

let create t =
    match t with
    | ValidTerm -> t |> Term
    | IllegalTerm -> failwith "No"

