---
title: "FIO"
description: "A type-safe, purely functional effect system for F#."
date: 2025-11-02
repoURL: "https://github.com/fs-fio/fio"
---

![FIO](/projects/fio.png)

**FIO** is a type-safe, purely functional effect system for F#. Write highly concurrent applications using a composable DSL inspired by [**ZIO**](https://zio.dev/) and [**Cats Effect**](https://typelevel.org/cats-effect/).

**Key features:**

- **IO monad** for pure side effect management
- **Fibers** (green threads) with constant-time scheduling
- **Multiple runtimes**: Direct, Cooperative, and Concurrent
- Performance-validated with comprehensive benchmarks

Initially developed at [**DTU**](https://www.dtu.dk/english) and available on [**NuGet**](https://www.nuget.org/packages/FSharp.FIO).

**Example:**

```fsharp
module MyFIOProject

open FSharp.FIO.DSL
open FSharp.FIO.App
open FSharp.FIO.Lib.IO

type WelcomeApp() =
    inherit FIOApp<unit, exn> ()

    override _.effect = fio {
        do! FConsole.PrintLine "Hello! What is your name?"
        let! name = FConsole.ReadLine
        do! FConsole.PrintLine $"Hello, %s{name}! Welcome to FIO! 🪻💜"
    }

WelcomeApp().Run()
```

Explore the full API, benchmarks, and documentation in the [**repository**](https://github.com/fs-fio/fio).
