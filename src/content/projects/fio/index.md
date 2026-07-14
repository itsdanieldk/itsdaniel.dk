---
title: "FIO"
description: "A type-safe, purely functional effect system for F#."
date: 2025-11-02
updatedDate: 2026-07-13
tags: ["F#", "concurrency", "effect system"]
repoURL: "https://github.com/fs-fio/fio"
---

![FIO](./fio.png)

**FIO** is a type-safe, purely functional effect system for F#: an IO monad plus lightweight fibers (green threads) for building concurrent and asynchronous applications. Effects are described as pure, lazy values and executed by a pluggable runtime, so your program stays a composable, referentially transparent description until you hand it off to be run. The API takes its cues from [**ZIO**](https://zio.dev/) and [**Cats Effect**](https://typelevel.org/cats-effect/).

If you're wondering why you'd want to program this way in the first place, I wrote about that in [Understanding Functional Effect Systems](/notes/understanding-functional-effect-systems/).

## Key Features

- **Typed effects** — `FIO<'A, 'E>` tracks both the success value and the error in the type
- **Fibers & channels** — green threads via `.Fork()` / `.Join()` and typed message passing between them
- **Structured concurrency** — fail-fast parallel combinators (`ZipPar`, `Race`, `forEachPar`) that interrupt losers automatically
- **Finalizer guarantees** — `Ensuring` finalizers run on success, error, *and* interruption
- **Composable** — the `fio { }` computation expression plus a rich set of operators (`>>=`, `<&>`, `<|>`)

## Quick Start

```bash
dotnet add package FSharp.FIO
```

```fsharp
open FIO.DSL
open FIO.App
open FIO.Console

type App() =
    inherit FIOApp<unit, exn>()

    override _.effect = fio {
        do! Console.printLine "What is your name?" id
        let! name = Console.readLine id
        do! Console.printLine $"Hello, {name}!" id
    }

[<EntryPoint>]
let main _ = App().Run()
```

## Concurrency

Fork effects onto fibers, run them in parallel, and compose the results — on the first failure, the losing fibers are interrupted automatically:

```fsharp
open FIO.DSL

// Run two effects in parallel with <&> and collect both results as a tuple.
let taskA = FIO.succeed "Task A completed! ✅"
let taskB = FIO.succeed (200, "Task B OK ✅")
let both  = taskA <&> taskB

// Or fork/join explicitly.
let forked = FIO.succeed("Hello, concurrency! 🚀").Fork() >>= fun fiber -> fiber.Join()
```

## Runtimes

Effects are interpreted by a runtime, and you can pick the one that fits:

| Runtime               | Notes                                                            |
| --------------------- | ---------------------------------------------------------------- |
| `DirectRuntime`       | Single-threaded, synchronous. Handy for tests and simple programs. |
| `PollingRuntime`      | Multi-threaded, linear-time handling of blocked fibers (polling). |
| `SignalingRuntime`    | Multi-threaded, event-driven handling of blocked fibers.          |
| `WorkStealingRuntime` | Multi-threaded, work-stealing scheduler. **The default.**         |

## Ecosystem

Beyond the core [**FSharp.FIO**](https://www.nuget.org/packages/FSharp.FIO) package, extension libraries bring effects to the network stack: [**FSharp.FIO.Http**](https://www.nuget.org/packages/FSharp.FIO.Http) (Kestrel-based HTTP server), [**FSharp.FIO.Sockets**](https://www.nuget.org/packages/FSharp.FIO.Sockets) (TCP sockets), and [**FSharp.FIO.WebSockets**](https://www.nuget.org/packages/FSharp.FIO.WebSockets).

I initially developed FIO at [**DTU**](https://www.dtu.dk/english) and have kept building on it since. Explore the full API, examples, and documentation in the [**repository**](https://github.com/fs-fio/fio).
