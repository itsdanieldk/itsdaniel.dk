namespace Yggdrasil.Content

open System

type Page =
    { Id: string
      Title: string
      Description: string
      Heading: string
      Emoji: string option
      Body: string }

type Note =
    { Id: string
      Title: string
      Description: string
      Date: DateOnly
      UpdatedDate: DateOnly option
      Body: string
      ReadingTime: string
      Tags: string list
      Draft: bool
      Featured: bool }

type Project =
    { Id: string
      Title: string
      Description: string
      Date: DateOnly
      UpdatedDate: DateOnly option
      Body: string
      ReadingTime: string
      Tags: string list
      Draft: bool
      Featured: bool
      DemoUrl: string option
      RepoUrl: string option }

type EntrySummary =
    { Id: string
      Title: string
      Description: string
      Date: DateOnly
      UpdatedDate: DateOnly option
      Body: string
      ReadingTime: string
      Draft: bool }

[<RequireQualifiedAccess>]
type FeedEntry =
    | Note of Note
    | Project of Project

    member this.Summary =
        match this with
        | Note n ->
            { Id = n.Id
              Title = n.Title
              Description = n.Description
              Date = n.Date
              UpdatedDate = n.UpdatedDate
              Body = n.Body
              ReadingTime = n.ReadingTime
              Draft = n.Draft }
        | Project p ->
            { Id = p.Id
              Title = p.Title
              Description = p.Description
              Date = p.Date
              UpdatedDate = p.UpdatedDate
              Body = p.Body
              ReadingTime = p.ReadingTime
              Draft = p.Draft }

    member this.Id =
        match this with
        | Note n -> n.Id
        | Project p -> p.Id

    member this.Title =
        match this with
        | Note n -> n.Title
        | Project p -> p.Title

    member this.Description =
        match this with
        | Note n -> n.Description
        | Project p -> p.Description

    member this.Date =
        match this with
        | Note n -> n.Date
        | Project p -> p.Date

    member this.UpdatedDate =
        match this with
        | Note n -> n.UpdatedDate
        | Project p -> p.UpdatedDate

    member this.Body =
        match this with
        | Note n -> n.Body
        | Project p -> p.Body

    member this.ReadingTime =
        match this with
        | Note n -> n.ReadingTime
        | Project p -> p.ReadingTime

    member this.Draft =
        match this with
        | Note n -> n.Draft
        | Project p -> p.Draft

type Fragrance =
    { Id: string
      Name: string
      House: string
      Url: string
      Rating: float option
      Note: string option
      Concentration: string option
      Image: string
      Image2x: string
      Wishlist: bool
      Draft: bool }
