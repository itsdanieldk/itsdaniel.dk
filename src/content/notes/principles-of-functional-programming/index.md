---
title: "Principles of Functional Programming"
description: "The core functional principles and why they matter."
date: 2025-11-02
tags: ["functional programming", "F#", "software design"]
---

I spent my early programming years writing Java. It got the job done, but there was always a lot of ceremony: class hierarchies, verbose patterns, state scattered across objects that made bugs hard to track down. When a university course introduced me to F#, the contrast was immediate. The code was lean. The compiler's type inference caught mistakes before I could even run the program. I was writing less and breaking less.

That course changed how I approach all code, not just functional code. The principles I picked up have stayed with me because they're practical: fewer bugs in production, code that's easier to maintain, less time spent debugging things that shouldn't have been possible in the first place.

This post walks through those core ideas. I'll use F# for examples, but the principles apply regardless of what language you work in.

## Pure Functions

A pure function always returns the same output for the same input and has no side effects: no global state mutations, no I/O, no surprises.

```fsharp
let add x y = x + y // Always predictable
```

To see why this matters, consider the alternative:

```fsharp
let mutable taxRate = 0.25

let calculateTotal price =
    taxRate <- taxRate + 0.01 // Modifies external state
    price * (1.0 + taxRate)

calculateTotal 100.0 // 126.0
calculateTotal 100.0 // 127.0, different result, same input
```

Every call changes `taxRate`, so the result depends on when and how often you call it. Now compare:

```fsharp
let calculateTotal taxRate price =
    price * (1.0 + taxRate)

calculateTotal 0.25 100.0 // 125.0
calculateTotal 0.25 100.0 // 125.0, always the same
```

The pure version takes everything it needs as arguments. You can call it from anywhere and know exactly what it'll do. Testing is straightforward: no setup, no teardown, no mocking global state.

A useful way to think about purity: if you can replace a function call with its result and nothing changes, the function is pure. This property is called *referential transparency*.

```fsharp
let square x = x * x
let result = square 5 + square 5
// You can swap in 25 + 25 and the program behaves identically
```

Referential transparency is what makes pure functions composable and safe to refactor. It's also what lets compilers optimize aggressively.

## Immutability

Instead of changing existing values, you create new ones. This sounds wasteful, but it eliminates an entire class of bugs, especially in concurrent code where shared mutable state is the root of most headaches.

```fsharp
let numbers = [1; 2; 3]
let newNumbers = 0 :: numbers // [0; 1; 2; 3], original list unchanged
```

The performance concern is worth addressing head-on. Functional data structures are *persistent*, meaning they share structure with previous versions rather than copying everything. When you prepend `0` to `numbers` above, the new list reuses the entire original `[1; 2; 3]` in memory. Only the new node gets allocated. This structural sharing is why immutable operations are often cheaper than most people expect, though the tradeoff varies by data structure and access pattern.

When data can't change out from under you, reasoning about what your program does becomes straightforward. Combined with pure functions, immutability gives you a codebase where you can read any function in isolation and understand it fully.

## Expressions, Not Statements

In most imperative languages, you write *statements*, instructions that do something but don't produce a value. An `if` block executes code; a `for` loop iterates. In functional programming, almost everything is an *expression* that evaluates to a value.

```fsharp
let score = 72
let status = if score >= 50 then "Pass" else "Fail"
// status = "Pass"
```

There's no need for a mutable variable that gets assigned inside each branch. The `if/else` itself produces a value, and you bind it directly. This applies to `match` expressions too:

```fsharp
let level = 2
let description =
    match level with
    | 1 -> "Beginner"
    | 2 -> "Intermediate"
    | _ -> "Advanced"
// description = "Intermediate"
```

This might seem like a small syntactic difference, but it has real consequences. When everything is an expression, you can't accidentally leave a variable uninitialized. You can't forget to assign in one branch of a conditional. And because each expression produces a value, you can nest and compose them freely, which is the foundation for the function-centric style we'll look at next.

## First-Class and Higher-Order Functions

In functional programming, functions are values. You can assign them to variables, pass them as arguments, and return them from other functions, just like any other data.

```fsharp
let add x y = x + y
let sum = add // Assign a function to a variable
sum 2 3 // 5

// Return a function from a function
let multiplier factor =
    let multiply x = x * factor
    multiply

let triple = multiplier 3
triple 5 // 15
```

Notice how `multiply` still has access to `factor` after `multiplier` returns. This is a *closure*: the inner function captures variables from its enclosing scope.

Once functions are values, you can write *higher-order functions*, functions that take other functions as arguments:

```fsharp
let doubled = List.map (fun x -> x * 2) [1; 2; 3] // [2; 4; 6]
let evens = List.filter (fun x -> x % 2 = 0) [1; 2; 3; 4] // [2; 4]
```

`List.map` and `List.filter` are workhorses of functional code. Instead of writing loops to transform data, you describe the transformation and let the function handle the iteration.

## Currying and Partial Application

In F#, every multi-parameter function is actually a chain of single-parameter functions. This is called *currying*. It means you can supply some arguments now and get back a function waiting for the rest.

```fsharp
let add x y = x + y // Curried by default
let add5 = add 5 // Partial application, returns a new function
add5 10 // 15

// Create a specialized function, then map it over a list
let numbers = [1; 2; 3; 4]
let add10 = add 10
List.map add10 numbers // [11; 12; 13; 14]
```

Partial application lets you build specialized functions from general ones without any extra boilerplate.

## Function Composition

Rather than nesting function calls or writing step-by-step procedures, you can compose small functions into pipelines.

```fsharp
let double x = x * 2
let increment x = x + 1

let process = increment >> double // Increment, then double
process 3 // 8
```

The `>>` operator chains two functions: the output of the first feeds into the input of the second. This keeps your code focused on *what* transforms happen, not the mechanics of passing values between steps.

## Recursion

Functional programming favors recursion over imperative loops. The reason is philosophical: traditional loops rely on mutable state (a counter that changes each iteration), which clashes with immutability.

```fsharp
let rec factorial n =
    if n <= 1 then 1
    else n * factorial (n - 1)

factorial 5 // 120

// Tail-recursive version (no stack growth)
let factorialTail n =
    let rec loop acc n =
        if n <= 1 then acc
        else loop (acc * n) (n - 1)
    loop 1 n
```

Every recursive function needs a base case (here, `if n <= 1`) to terminate. The tail-recursive version restructures the computation so the recursive call is the last thing the function does, letting the compiler optimize it into a loop under the hood.

That said, explicit recursion isn't your everyday tool. Higher-order functions like `List.map`, `List.filter`, and `List.fold` handle most iteration more clearly. Recursion is the mechanism they're built on, but those abstractions, and the declarative style they enable, are what you'll actually reach for.

## Declarative Thinking

Functional programming nudges you toward saying *what* you want rather than spelling out *how* to get there. To see the difference, consider summing the even numbers in a list.

An imperative approach might look like this:

```fsharp
let mutable total = 0
for x in [1; 2; 3; 4] do
    if x % 2 = 0 then
        total <- total + x
// total = 6
```

You're managing a mutable accumulator, iterating manually, and checking conditions inside the loop. It works, but the intent is buried in mechanics.

F# has a pipe operator (`|>`) that passes the result of one expression as the last argument to the next function. With it, the same logic reads top-to-bottom:

```fsharp
let totalEven =
    [1; 2; 3; 4]
    |> List.filter (fun x -> x % 2 = 0)
    |> List.sum // 6
```

No intermediate variables, no loop counters. The code reads as: "take this list, keep the even numbers, sum them." Each transformation step is explicit and independently testable.

If you step back, a pattern has been forming across all of these ideas. FP programs are built as pipelines of small, composable transformations over well-typed, immutable data. Pure functions ensure each step is predictable. Immutability guarantees data won't change between steps. Composition and piping connect the steps together. What remains is modeling the data itself.

## Algebraic Data Types and Pattern Matching

Functional programming gives you tools to model data precisely. The simplest example is `Option`, a type that explicitly represents the presence or absence of a value, replacing `null` entirely.

```fsharp
let found = List.tryFind (fun x -> x > 3) [1; 2; 3; 4; 5]
// found : int option = Some 4

let missing = List.tryFind (fun x -> x > 10) [1; 2; 3; 4; 5]
// missing : int option = None
```

There's no `null`, no `NullReferenceException`. The type itself tells you a value might not be there, and the compiler won't let you use it without handling both cases.

`Option` is actually a *discriminated union*, an algebraic data type with named cases. You can define your own to model domain-specific data:

```fsharp
type Payment =
    | Cash of decimal
    | Card of string * decimal

let describe payment =
    match payment with
    | Cash amount -> sprintf "Paid %.2f in cash" amount
    | Card (number, amount) -> sprintf "Paid %.2f by card %s" amount number

describe (Card ("**** 4242", 2.3M)) // "Paid 2.30 by card **** 4242"
```

The `match` expression is exhaustive. If you add a new case to `Payment` and forget to handle it, the compiler warns you. Your type system catches logic errors before your users do.

The deeper principle here is designing your types so that invalid data can't exist in the first place. If a payment is always either cash or card, the type enforces that. There's no "unknown" state, no forgotten edge case. You encode your business rules into the type system, and the compiler holds you to them.

## Explicit Error Handling

Instead of throwing exceptions, functional code uses types like `Result` to represent outcomes explicitly.

```fsharp
let divide x y =
    if y = 0 then Error "Division by zero"
    else Ok (x / y)

match divide 10 2 with
| Ok result -> printfn "Result: %d" result
| Error msg -> printfn "Error: %s" msg
```

The caller can't ignore the error case. The type system forces you to deal with it. No more uncaught exceptions crashing your program three layers up the call stack.

Where this really pays off is composition. Real-world code often chains multiple operations that can each fail. Without functional error handling, you end up with deeply nested `if` checks or scattered `try/catch` blocks. With `Result`, you chain operations using `Result.bind`. If any step fails, the rest are skipped and the error carries through:

```fsharp
let validatePositive n =
    if n > 0 then Ok n
    else Error "Must be positive"

let validateSmall n =
    if n < 100 then Ok n
    else Error "Must be less than 100"

5   |> validatePositive |> Result.bind validateSmall // Ok 5
-3  |> validatePositive |> Result.bind validateSmall // Error "Must be positive"
200 |> validatePositive |> Result.bind validateSmall // Error "Must be less than 100"
```

Each validation is a small, testable function. The pipeline stops at the first failure and carries the error forward. No exceptions, no nested conditionals. Just data flowing through functions.

## Separation of Effects

One of the more advanced ideas: keep your core logic pure and push side effects to the edges of your program. Business rules stay deterministic and testable. I/O, state mutations, and network calls live in a thin outer shell.

```fsharp
// Pure core logic
let calculateDiscount total =
    if total > 100.0M then total * 0.9M
    else total

// Impure shell (I/O at the edges)
let processOrder () =
    let total = readTotalFromDatabase() // Side effect
    let discounted = calculateDiscount total // Pure
    saveToDatabase discounted // Side effect
```

The pure `calculateDiscount` function is trivial to test: pass in a number, check the output. The impure `processOrder` is a thin wrapper that orchestrates I/O around it. When something breaks, you know where to look.

## Why These Principles Matter

These aren't theoretical ideals. They solve real, everyday problems. Pure functions and immutability eliminate bugs that stem from hidden state. Code becomes testable without elaborate setup. Refactoring gets safer because each function can be reasoned about on its own. And when you come back to a codebase months later, the absence of side effects means you can pick up where you left off without re-learning the implementation.

Looking back, the shift from Java to F# wasn't really about switching languages. It was about learning to think differently: favoring data over objects, transformations over mutations, types over runtime checks. Those habits stuck, and I carry them into everything I write now, whether it's F#, TypeScript, or anything else. Start with the principles, and the language matters less than you'd think.
