module Yggdrasil.Tests.Main

open Expecto

[<EntryPoint>]
let main argv =

    testList
        "Yggdrasil"
        [ ContentTests.tests
          WebTests.tests
          OgImageTests.tests
          AssetsTests.tests
          ConfigTests.tests
          SiteConfigTests.tests ]
    |> runTestsWithCLIArgs [] argv
