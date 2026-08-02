namespace Yggdrasil.Content

[<AutoOpen>]
module Prelude =

    type ResultBuilder() =
        
        member _.Bind(x, f) =
            Result.bind f x

        member _.Return x =
            Ok x
        
        member _.ReturnFrom(x: Result<_, _>) =
            x

    let result = ResultBuilder()

    let collectResults (results: Result<'a, 'e> list) =
        let oks, errors =
            results
            |> List.fold
                (fun (oks, errors) r ->
                    match r with
                    | Ok v -> v :: oks, errors
                    | Error e -> oks, e :: errors)
                ([], [])

        match errors with
        | [] -> Ok(List.rev oks)
        | _ -> Error(List.rev errors)
