---
title: "FIO"
description: "A type-safe, purely functional effect system for F#."
date: "Nov 02 2025"
repoURL: "https://github.com/fs-fio/fio"
---

![FIO](/projects/fio.png)

**FIO** is a type-safe, purely functional effect system for [**F#**](https://fsharp.org/), designed for building **highly concurrent** and **asynchronous** applications. It provides a lightweight [**DSL**](https://martinfowler.com/dsl.html) for writing composable programs using **functional effects**.

Inspired by [**ZIO**](https://zio.dev/) and [**Cats Effect**](https://typelevel.org/cats-effect/), **FIO** features:

- An **IO monad** for managing side effects
- **Fibers** (green threads) for scalable concurrency
- A focus on **purity**, **type safety**, and **performance**

**FIO** was developed as part of a master’s thesis in Computer Science at [**DTU**](https://www.dtu.dk/english).

Use **FIO** to easily compose functional effects, like so:

```fsharp
module FIOAppUsage

open FSharp.FIO.DSL
open FSharp.FIO.Lib.IO
open FSharp.FIO.App

type WelcomeApp() =
    inherit FIOApp<unit, exn> ()

    override _.effect = fio {
        do! FConsole.PrintLine "Hello! What is your name?"
        let! name = FConsole.ReadLine ()
        do! FConsole.PrintLine $"Hello, %s{name}! Welcome to FIO! 🪻💜"
    }

WelcomeApp().Run()
```

Check out the resource repository for more information.
