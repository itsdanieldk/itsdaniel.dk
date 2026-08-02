namespace Yggdrasil.Content

open System
open System.Globalization

module DateParser =

    let private parseIso (value: string) =
        match DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None) with
        | true, date -> Some date
        | _ -> None

    let tryParse (path: string) (field: string) (value: string option) =
        match value with
        | None
        | Some null -> Error $"{path}: {field}: required date is missing"
        | Some v ->
            match parseIso v with
            | Some d -> Ok d
            | None -> Error $"{path}: {field}: invalid ISO date \"{v}\""

    let tryParseOptional (path: string) (field: string) (value: string option) =
        match value with
        | None
        | Some null
        | Some "" -> Ok None
        | Some v ->
            match parseIso v with
            | Some d -> Ok(Some d)
            | None -> Error $"{path}: {field}: invalid ISO date \"{v}\""

    let toIsoDatetime (date: DateOnly) =
        date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T00:00:00.000Z"
